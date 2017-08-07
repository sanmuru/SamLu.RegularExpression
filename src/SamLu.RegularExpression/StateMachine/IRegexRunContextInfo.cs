using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了运行正则表达式的上下文信息应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">自动机接受处理的对象的类型。</typeparam>
    public interface IRegexRunContextInfo<T>
    {
        /// <summary>
        /// 可接受的对象集。
        /// </summary>
        ISet<T> AccreditedSet { get; }

        /// <summary>
        /// 远程创建 <see cref="RegexNFA{T}"/> 类的新实例。
        /// </summary>
        /// <returns><see cref="RegexNFA{T}"/> 类的实例。</returns>
        RegexNFA<T> ActivateRegexNFA();

        /// <summary>
        /// 远程创建 <see cref="RegexDFA{T}"/> 类的新实例。
        /// </summary>
        /// <returns><see cref="RegexDFA{T}"/> 类的实例。</returns>
        RegexDFA<T> ActivateRegexDFA();

        /// <summary>
        /// 远程创建 <see cref="RegexNFAState{T}"/> 类的新实例。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该有限自动机的状态是否为结束状态。默认为 false 。</param>
        /// <returns><see cref="RegexNFAState{T}"/> 类的新实例。</returns>
        RegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false);

        /// <summary>
        /// 远程创建 <see cref="RegexDFAState{T}"/> 类的新实例。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该有限自动机的状态是否为结束状态。默认为 false 。</param>
        /// <returns><see cref="RegexDFAState{T}"/> 类的新实例。</returns>
        RegexDFAState<T> ActivateRegexDFAState(bool isTerminal = false);

        /// <summary>
        /// 从条件正则（ <see cref="RegexCondition{T}"/> ）对象中获取信息，远程创建 NFA 转换的新实例。
        /// </summary>
        /// <param name="regex">指定的条件正则对象。</param>
        /// <returns>NFA 转换的新实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        RegexFATransition<T, RegexNFAState<T>> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex);

        /// <summary>
        /// 远程创建 <see cref="RegexNFAEpsilonTransition{T}"/> 类的新实例。
        /// </summary>
        /// <returns><see cref="RegexNFAEpsilonTransition{T}"/> 类的新实例。</returns>
        RegexNFAEpsilonTransition<T> ActivateRegexNFAEpsilonTransition();

        /// <summary>
        /// 从可接受的对象集中获取信息，远程创建 DFA 转换的新实例。
        /// </summary>
        /// <param name="set">指定的可接受的对象集。</param>
        /// <returns>DFA 转换的新实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> 的值为 null 。</exception>
        RegexFATransition<T, RegexDFAState<T>> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set);

        /// <summary>
        /// 合并多个指向相同目标状态的 DFA 转换。
        /// </summary>
        /// <param name="dfaTransitions">多个指向相同目标状态的 DFA 转换。</param>
        /// <returns>合并后的 DFA 转换。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dfaTransitions"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException"><paramref name="dfaTransitions"/> 中的 DFA 转换不全指向相同目标状态。</exception>
        RegexFATransition<T, RegexDFAState<T>> CombineRegexDFATransitions(IEnumerable<RegexFATransition<T, RegexDFAState<T>>> dfaTransitions);
    }
}
