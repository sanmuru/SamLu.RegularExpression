using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class BasicRegexNFAStateBase<T> : NFAState, IRegexNFAState<T>
    {
        /// <summary>
        /// 获取 <see cref="BasicRegexNFAStateBase{T}"/> 的转换集。
        /// </summary>
        new public virtual ICollection<IRegexFSMTransition<T>> Transitions =>
            new ReadOnlyCollection<IRegexFSMTransition<T>>(base.Transitions.Cast<IRegexFSMTransition<T>>().ToList());

        /// <summary>
        /// 初始化 <see cref="BasicRegexNFAStateBase{T}"/> 类的新实例。
        /// </summary>
        public BasicRegexNFAStateBase() : base() { }
        /// <summary>
        /// 初始化 <see cref="BasicRegexDFAStateBase{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public BasicRegexNFAStateBase(bool isTerminal) : base(isTerminal) { }

        public BasicRegexNFAStateBase(BasicRegexNFAStateBase<T> state) :
            this((state ?? throw new ArgumentNullException(nameof(state))).IsTerminal)
        {
            // 复制转换集合。
            foreach (var transition in state.Transitions)
                this.AttachTransition(transition);

            // 复制动作。
            this.EntryAction = state.EntryAction;
            this.ExitAction = state.ExitAction;
            this.InputAction = state.InputAction;
            this.TransitAction = state.TransitAction;
        }

        #region AttachTransition
        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public sealed override bool AttachTransition(ITransition transition) => base.AttachTransition(transition);

        /// <summary>
        /// 添加指定的 ε 转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public sealed override bool AttachTransition(IEpsilonTransition epsilonTransition) => base.AttachTransition(epsilonTransition);

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public virtual bool AttachTransition(IRegexFSMTransition<T> transition) => base.AttachTransition(transition);

        /// <summary>
        /// 添加指定的 ε 转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public virtual bool AttachTransition(IRegexFSMEpsilonTransition<T> epsilonTransition) => base.AttachTransition(epsilonTransition);
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public sealed override bool RemoveTransition(ITransition transition) => base.RemoveTransition(transition);

        /// <summary>
        /// 移除指定的 ε 转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public sealed override bool RemoveTransition(IEpsilonTransition epsilonTransition) => base.RemoveTransition(epsilonTransition);

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public virtual bool RemoveTransition(IRegexFSMTransition<T> transition) => base.RemoveTransition(transition);

        /// <summary>
        /// 移除指定的 ε 转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public virtual bool RemoveTransition(IRegexFSMEpsilonTransition<T> epsilonTransition) => base.RemoveTransition(epsilonTransition);
        #endregion

        /// <summary>
        /// 获取可以接受指定输入并进行转换的转换。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>可以接受指定输入并进行转换的转换。</returns>
        public virtual IRegexFSMTransition<T> GetTransitTransition(T input)
        {
            throw new NotSupportedException();
        }

        #region IRegexFSMState{T} Implementation
        IEnumerable<IRegexFSMTransition<T>> IRegexFSMState<T>.GetOrderedTransitions() =>
            this.Transitions.OrderBy(
                (transition => transition),
                Comparer<IRegexFSMTransition<T>>.Create((x, y) =>
                {
                    bool f1 = x is IRegexFSMEpsilonTransition<T>;
                    bool f2 = y is IRegexFSMEpsilonTransition<T>;
                    if (f1 ^ f2)
                        return f1 ? -1 : 1;
                    else
                        return 0;
                })
            );
        #endregion
    }

    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的非确定的有限自动机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class BasicRegexNFAState<T> : NFAState<BasicRegexFATransition<T, BasicRegexNFAState<T>>, BasicRegexNFAEpsilonTransition<T>>, IRegexNFAState<T, BasicRegexFATransition<T, BasicRegexNFAState<T>>, BasicRegexNFAEpsilonTransition<T>>
    {
        /// <summary>
        /// 初始化 <see cref="BasicRegexNFAState{T}"/> 类的新实例。
        /// </summary>
        public BasicRegexNFAState() : this(false) { }

        /// <summary>
        /// 初始化 <see cref="BasicRegexNFAState{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public BasicRegexNFAState(bool isTerminal) : base(isTerminal) { }
        
        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public override bool AttachTransition(BasicRegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (epsilonTransition == null) throw new ArgumentNullException(nameof(epsilonTransition));

            return base.AttachTransition(epsilonTransition);
        }

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public override bool RemoveTransition(BasicRegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (epsilonTransition == null) throw new ArgumentNullException(nameof(epsilonTransition));

            return base.AttachTransition(epsilonTransition);
        }

        /// <summary>
        /// 获取可以接受指定输入并进行转换的转换。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>可以接受指定输入并进行转换的转换。</returns>
        public virtual BasicRegexFATransition<T, BasicRegexNFAState<T>> GetTransitTransition(T input)
        {
            throw new NotSupportedException();
        }

        #region IRegexFSMTransition{T} Implementation
        ICollection<IRegexFSMTransition<T>> IRegexFSMState<T>.Transitions =>
            new ReadOnlyCollection<IRegexFSMTransition<T>>(
                base.Transitions.Cast<IRegexFSMTransition<T>>().ToList()
            );

        bool IRegexNFAState<T>.AttachTransition(IRegexFSMEpsilonTransition<T> epsilonTransition) =>
            this.AttachTransition((BasicRegexNFAEpsilonTransition<T>)epsilonTransition);

        bool IRegexNFAState<T>.RemoveTransition(IRegexFSMEpsilonTransition<T> epsilonTransition) =>
            this.RemoveTransition((BasicRegexNFAEpsilonTransition<T>)epsilonTransition);

        IRegexFSMTransition<T> IRegexFSMState<T>.GetTransitTransition(T input) =>
            this.GetTransitTransition(input);

        bool IRegexFSMState<T>.AttachTransition(IRegexFSMTransition<T> transition) =>
            base.AttachTransition((BasicRegexFATransition<T, BasicRegexNFAState<T>>)transition);

        bool IRegexFSMState<T>.RemoveTransition(IRegexFSMTransition<T> transition) =>
            base.RemoveTransition((BasicRegexFATransition<T, BasicRegexNFAState<T>>)transition);

        IEnumerable<IRegexFSMTransition<T>> IRegexFSMState<T>.GetOrderedTransitions() =>
            this.Transitions.OrderBy(
                (transition => transition),
                Comparer<BasicRegexFATransition<T, BasicRegexNFAState<T>>>.Create((x, y) =>
                {
                    bool f1 = x is BasicRegexNFAEpsilonTransition<T>;
                    bool f2 = y is BasicRegexNFAEpsilonTransition<T>;
                    if (f1 ^ f2)
                        return f1 ? -1 : 1;
                    else
                        return 0;
                })
            );

        bool INFAState.AttachTransition(IEpsilonTransition epsilonTransition) =>
            this.AttachTransition((BasicRegexNFAEpsilonTransition<T>)epsilonTransition);

        bool INFAState.RemoveTransition(IEpsilonTransition epsilonTransition) =>
            this.RemoveTransition((BasicRegexNFAEpsilonTransition<T>)epsilonTransition);
        #endregion
    }
}
