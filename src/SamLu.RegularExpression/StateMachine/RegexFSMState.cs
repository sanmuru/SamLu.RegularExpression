using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的有限状态机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexFSMState<T> : FSMState, IRegexFSMState<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMState{T}"/> 的转换集。
        /// </summary>
        new public virtual ICollection<IRegexFSMTransition<T>> Transitions =>
            new ReadOnlyCollection<IRegexFSMTransition<T>>(base.Transitions.Cast<IRegexFSMTransition<T>>().ToList());

        /// <summary>
        /// 初始化 <see cref="RegexFSMState{T}"/> 类的新实例。
        /// </summary>
        public RegexFSMState() : base() { }

        /// <summary>
        /// 初始化 <see cref="RegexFSMState{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public RegexFSMState(bool isTerminal) : base(isTerminal) { }

        #region AttachTransition
        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public sealed override bool AttachTransition(ITransition transition) => this.AttachTransition((IRegexFSMTransition<T>)transition);

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public virtual bool AttachTransition(IRegexFSMTransition<T> transition) => base.AttachTransition(transition);
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public sealed override bool RemoveTransition(ITransition transition) => this.RemoveTransition((IRegexFSMTransition<T>)transition);

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public virtual bool RemoveTransition(IRegexFSMTransition<T> transition) => base.RemoveTransition(transition);
        #endregion

        #region IRegexFSMState{T} Implementation
        IEnumerable<IRegexFSMTransition<T>> IRegexFSMState<T>.GetOrderedTransitions() => this.GetOrderedTransitions();
        #endregion
    }

    /// <summary>
    /// 表示正则表达式构造的有限状态机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TTransition">正则表达式构造的有限状态机的转换的类型。</typeparam>
    public class RegexFSMState<T, TTransition> : FSMState<TTransition>, IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMState{T, TTransition}"/> 的转换集。
        /// </summary>
        new public virtual ICollection<IRegexFSMTransition<T>> Transitions =>
            new ReadOnlyCollection<IRegexFSMTransition<T>>(base.Transitions.Cast<IRegexFSMTransition<T>>().ToList());

        /// <summary>
        /// 初始化 <see cref="RegexFSMState{T, TTransition}"/> 类的新实例。
        /// </summary>
        public RegexFSMState() : base() { }

        /// <summary>
        /// 初始化 <see cref="RegexFSMState{T, TTransition}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public RegexFSMState(bool isTerminal) : base(isTerminal) { }

        #region IRegexFSMState{T} Implementation
        bool IRegexFSMState<T>.AttachTransition(IRegexFSMTransition<T> transition) => base.AttachTransition((TTransition)transition);

        bool IRegexFSMState<T>.RemoveTransition(IRegexFSMTransition<T> transition) => base.AttachTransition((TTransition)transition);

        IEnumerable<IRegexFSMTransition<T>> IRegexFSMState<T>.GetOrderedTransitions() =>
            (IEnumerable<IRegexFSMTransition<T>>)(((IRegexFSMState<T, TTransition>)this).GetOrderedTransitions());

        IEnumerable<TTransition> IRegexFSMState<T, TTransition>.GetOrderedTransitions() =>
            this.GetOrderedTransitions<T, TTransition>();
        #endregion
    }
}
