using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的有限自动机的转换。
    /// </summary>
    public class BasicRegexFATransitionBase<T> : FSMTransition, IRegexFSMTransition<T>, IAcceptInputTransition<T>
    {
        private Predicate<T> predicate;
        /// <summary>
        /// 获取一个方法，该方法确定 <see cref="BasicRegexFATransitionBase{T}"/> 接受的输入是否满足条件。
        /// </summary>
        public virtual Predicate<T> Predicate => this.predicate;

        /// <summary>
        /// 初始化 <see cref="BasicRegexFATransition{T, TRegexFAState}"/> 类的新实例。
        /// </summary>
        protected BasicRegexFATransitionBase() { }

        /// <summary>
        /// 初始化 <see cref="BasicRegexFATransitionBase{T}"/> 类的新实例。该实例使用指定的确定 <see cref="BasicRegexFATransitionBase{T}"/> 接受的输入是否满足条件的方法。
        /// </summary>
        /// <param name="predicate">指定的确定 <see cref="BasicRegexFATransitionBase{T}"/> 接受的输入是否满足条件的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 的值为 null 。</exception>
        public BasicRegexFATransitionBase(Predicate<T> predicate) : this()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }

        public BasicRegexFATransitionBase(BasicRegexFATransition<T, BasicRegexNFAState<T>> transition) :
            this((transition ?? throw new ArgumentNullException(nameof(transition))).Predicate)
        { }

        public BasicRegexFATransitionBase(BasicRegexFATransition<T, BasicRegexDFAState<T>> transition) :
            this((transition ?? throw new ArgumentNullException(nameof(transition))).Predicate)
        { }

        public BasicRegexFATransitionBase(IAcceptInputTransition<T> transition) :
            this((transition ?? throw new ArgumentNullException(nameof(transition))).CanAccept)
        { }

        public static BasicRegexFATransitionBase<T> Adapt<TRegexFAState>(BasicRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>> =>
            new BasicRegexFATransitionBase<T>((transition ?? throw new ArgumentNullException(nameof(transition))).Predicate);

        /// <summary>
        /// 获取 <see cref="BasicRegexFATransitionBase{T}"/> 指向的状态。
        /// </summary>
        new public virtual IRegexFSMState<T> Target => (IRegexFSMState<T>)base.Target;

        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool SetTarget(IRegexFSMState<T> state) => base.SetTarget(state);

        public bool CanAccept(T input)
        {
            return this.Predicate(input);
        }
    }

    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的有限自动机的转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TRegexFAState">正则表达式构造的有限自动机的状态的类型。</typeparam>
    public class BasicRegexFATransition<T, TRegexFAState> : FSMTransition<TRegexFAState>, IRegexFSMTransition<T, TRegexFAState>, IAcceptInputTransition<T>
        where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
    {
        private Predicate<T> predicate;
        /// <summary>
        /// 获取一个方法，该方法确定 <see cref="BasicRegexFATransition{T, TRegexFAState}"/> 接受的输入是否满足条件。
        /// </summary>
        public virtual Predicate<T> Predicate => this.predicate;

        /// <summary>
        /// 初始化 <see cref="BasicRegexFATransition{T, TRegexFAState}"/> 类的新实例。
        /// </summary>
        protected BasicRegexFATransition() { }

        /// <summary>
        /// 初始化 <see cref="BasicRegexFATransition{T, TRegexFAState}"/> 类的新实例。该实例使用指定的确定 <see cref="BasicRegexFATransition{T, TRegexFAState}"/> 接受的输入是否满足条件的方法。
        /// </summary>
        /// <param name="predicate">指定的确定 <see cref="BasicRegexFATransition{T, TRegexFAState}"/> 接受的输入是否满足条件的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 的值为 null 。</exception>
        public BasicRegexFATransition(Predicate<T> predicate) : this()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }

        public bool CanAccept(T input)
        {
            return this.Predicate(input);
        }

        #region IRegexFSMTransition{T} Implementation
        IRegexFSMState<T> IRegexFSMTransition<T>.Target => this.Target;

        bool IRegexFSMTransition<T>.SetTarget(IRegexFSMState<T> state) =>
            base.SetTarget((TRegexFAState)state);
        #endregion
    }
}
