using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.StateMachine;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的非确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexNFA<T> : RegexFSM<T>, IRegexNFA<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexNFA{T}"/> 类的新实例。
        /// </summary>
        public RegexNFA() : base() { }

        #region AttachTransition
        /// <summary>
        /// 为 <see cref="RegexNFA{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) => base.AttachTransition(state, transition);

        /// <summary>
        /// 为 <see cref="RegexNFA{T}"/> 的一个指定状态添加指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool AttachTransition(IRegexNFAState<T> state, IRegexFSMEpsilonTransition<T> epsilonTransition) => base.AttachTransition(state, epsilonTransition);
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 从 <see cref="RegexNFA{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) => base.RemoveTransition(state, transition);

        /// <summary>
        /// 从 <see cref="RegexNFA{T}"/> 的一个指定状态移除指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool RemoveTransition(IRegexNFAState<T> state, IRegexFSMEpsilonTransition<T> epsilonTransition) => base.RemoveTransition(state, epsilonTransition);
        #endregion

        /// <summary>
        /// 最小化 NFA 。
        /// </summary>
        /// <seealso cref="FSMUtility.EpsilonClosure(IFSM)"/>
        public void Optimize()
        {
            this.EpsilonClosure();
        }

        #region INFA Implementation
        bool INFA.AttachTransition(INFAState state, IEpsilonTransition epsilonTransition) => this.AttachTransition((IRegexNFAState<T>)state, (IRegexFSMEpsilonTransition<T>)epsilonTransition);

        bool INFA.RemoveTransition(INFAState state, IEpsilonTransition epsilonTransition) => this.RemoveTransition((IRegexNFAState<T>)state, (IRegexFSMEpsilonTransition<T>)epsilonTransition);
        #endregion
    }

    /// <summary>
    /// 表示正则表达式构造的非确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">非确定的有限自动机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">非确定的有限自动机的转换的类型。</typeparam>
    /// <typeparam name="TEpsilonTransition">非确定的有限自动机的 ε 转换的类型。</typeparam>
    public class RegexNFA<T, TState, TTransition, TEpsilonTransition> : RegexFSM<T, TState, TTransition>, IRegexNFA<T, TState, TTransition, TEpsilonTransition>
        where TState : IRegexNFAState<T, TTransition, TEpsilonTransition>
        where TTransition : class, IRegexFSMTransition<T, TState>, IAcceptInputTransition<T>
        where TEpsilonTransition : TTransition, IRegexFSMEpsilonTransition<T, TState>
    {
        /// <summary>
        /// 初始化 <see cref="RegexNFA{T, TState, TTransition, TEpsilonTransition}"/> 类的新实例。
        /// </summary>
        public RegexNFA() : base() { }

        /// <summary>
        /// 向 <see cref="RegexNFA{T, TState, TTransition, TEpsilonTransition}"/> 的一个指定状态添加指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public virtual bool AttachTransition(TState state, TEpsilonTransition epsilonTransition) => base.AttachTransition(state, epsilonTransition);

        /// <summary>
        /// 从 <see cref="IRegexNFA{T, TState, TTransition, TEpsilonTransition}"/> 的一个指定状态移除指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public virtual bool RemoveTransition(TState state, TEpsilonTransition epsilonTransition) => base.RemoveTransition(state, epsilonTransition);

        /// <summary>
        /// 最小化 NFA 。
        /// </summary>
        /// <seealso cref="FSMUtility.EpsilonClosure{TState, TTransition, TEpsilonTransition}(INFA{TState, TTransition, TEpsilonTransition})"/>
        public virtual void Optimize()
        {
            this.EpsilonClosure();
        }

        #region INFA Implementation
        bool IRegexNFA<T>.AttachTransition(IRegexNFAState<T> state, IRegexFSMEpsilonTransition<T> epsilonTransition) => this.AttachTransition((TState)state, (TTransition)epsilonTransition);

        bool IRegexNFA<T>.RemoveTransition(IRegexNFAState<T> state, IRegexFSMEpsilonTransition<T> epsilonTransition) => this.RemoveTransition((TState)state, (TTransition)epsilonTransition);

        bool INFA.AttachTransition(INFAState state, IEpsilonTransition epsilonTransition) => this.AttachTransition((TState)state, (TTransition)epsilonTransition);

        bool INFA.RemoveTransition(INFAState state, IEpsilonTransition epsilonTransition) => this.RemoveTransition((TState)state, (TTransition)epsilonTransition);
        #endregion
    }
}
