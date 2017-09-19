using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了构造激活正则表达式状态机的上下文信息应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">自动机接受处理的对象的类型。</typeparam>
    public interface IRegexStateMachineActivationContextInfo<T>
    {
        /// <summary>
        /// 可接受的对象集。
        /// </summary>
        ISet<T> AccreditedSet { get; }

        /// <summary>
        /// 远程创建 <see cref="IRegexNFA{T}"/> 的实例。
        /// </summary>
        /// <returns><see cref="IRegexNFA{T}"/> 的实例。</returns>
        IRegexNFA<T> ActivateRegexNFA();

        /// <summary>
        /// 远程创建指定 <typeparamref name="TRegexNFA"/> 对象的空副本。
        /// </summary>
        /// <param name="nfa">指定正则构造非确定自动机。</param>
        /// <typeparam name="TRegexNFA">正则构造非确定自动机对象的类型。</typeparam>
        /// <returns>指定正则构造非确定自动机的空副本。</returns>
        TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa) where TRegexNFA : IRegexNFA<T>;

        /// <summary>
        /// 远程创建 <see cref="IRegexNFAState{T}"/> 的新实例。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该有限自动机的状态是否为结束状态。默认为 false 。</param>
        /// <returns><see cref="IRegexNFAState{T}"/> 的新实例。</returns>
        IRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false);

        /// <summary>
        /// 远程创建指定 <typeparamref name="TRegexNFAState"/> 对象的空副本。
        /// </summary>
        /// <param name="state">指定正则构造非确定自动机的状态。</param>
        /// <returns>指定正则构造非确定自动机的状态的空副本。</returns>
        TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state) where TRegexNFAState : IRegexNFAState<T>;

        /// <summary>
        /// 远程创建 <see cref="IRegexDFA{T}"/> 的实例。
        /// </summary>
        /// <returns><see cref="IRegexDFA{T}"/> 的实例。</returns>
        IRegexDFA<T> ActivateRegexDFA();

        /// <summary>
        /// 远程创建 <see cref="IRegexFSMState{T}"/> 的新实例。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该有限自动机的状态是否为结束状态。默认为 false 。</param>
        /// <returns><see cref="IRegexFSMState{T}"/> 的新实例。</returns>
        IRegexFSMState<T> ActivateRegexDFAState(bool isTerminal = false);

        /// <summary>
        /// 从条件正则（ <see cref="RegexCondition{T}"/> ）对象中获取信息，远程创建 NFA 转换的新实例。
        /// </summary>
        /// <param name="regex">指定的条件正则对象。</param>
        /// <returns>NFA 转换的新实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        BasicRegexFATransition<T> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex);

        /// <summary>
        /// 远程创建指定正则构造非确定自动机的转换的空副本。
        /// </summary>
        /// <param name="transition">指定正则构造非确定自动机的转换。</param>
        /// <returns>指定正则构造非确定自动机的转换的空副本。</returns>
        TRegexNFATransition ActivateRegexNFATransitionFromDumplication<TRegexNFATransition>(TRegexNFATransition transition) where TRegexNFATransition : IRegexFSMTransition<T>;

        /// <summary>
        /// 远程创建 <see cref="IRegexFSMEpsilonTransition{T}"/> 的新实例。
        /// </summary>
        /// <returns><see cref="IRegexFSMEpsilonTransition{T}"/> 的新实例。</returns>
        IRegexFSMEpsilonTransition<T> ActivateRegexNFAEpsilonTransition();

        /// <summary>
        /// 从可接受的对象集中获取信息，远程创建 DFA 转换的新实例。
        /// </summary>
        /// <param name="set">指定的可接受的对象集。</param>
        /// <returns>DFA 转换的新实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> 的值为 null 。</exception>
        IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set);

        ISet<T> GetAccreditedSetFromRegexNFATransition(BasicRegexFATransition<T> transition);

        ISet<T> GetAccreditedSetExceptResult(ISet<T> first, ISet<T> second);

        ISet<T> GetAccreditedSetIntersectResult(ISet<T> first, ISet<T> second);

        ISet<T> GetAccreditedSetSymmetricExceptResult(ISet<T> first, ISet<T> second);

        ISet<T> GetAccreditedSetUnionResult(ISet<T> first, ISet<T> second);

        /// <summary>
        /// 合并多个指向相同目标状态的 DFA 转换。
        /// </summary>
        /// <param name="dfaTransitions">多个指向相同目标状态的 DFA 转换。</param>
        /// <returns>合并后的 DFA 转换。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dfaTransitions"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException"><paramref name="dfaTransitions"/> 中的 DFA 转换不全指向相同目标状态。</exception>
        IAcceptInputTransition<T> CombineRegexDFATransitions(IEnumerable<IAcceptInputTransition<T>> dfaTransitions);
    }
}
