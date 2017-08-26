﻿using System;
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
        /// 远程创建 <see cref="BasicRegexNFA{T}"/> 类的新实例。
        /// </summary>
        /// <returns><see cref="BasicRegexNFA{T}"/> 类的实例。</returns>
        BasicRegexNFA<T> ActivateRegexNFA();

        /// <summary>
        /// 远程创建指定 <see cref="BasicRegexNFA{T}"/> 对象的空副本。
        /// </summary>
        /// <param name="nfa">指定正则构造非确定自动机。</param>
        /// <returns>指定正则构造非确定自动机的空副本。</returns>
        BasicRegexNFA<T> ActivateRegexNFAFromDumplication(BasicRegexNFA<T> nfa);

        /// <summary>
        /// 远程创建 <see cref="BasicRegexDFA{T}"/> 类的新实例。
        /// </summary>
        /// <returns><see cref="BasicRegexDFA{T}"/> 类的实例。</returns>
        BasicRegexDFA<T> ActivateRegexDFA();

        /// <summary>
        /// 远程创建 <see cref="BasicRegexNFAState{T}"/> 类的新实例。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该有限自动机的状态是否为结束状态。默认为 false 。</param>
        /// <returns><see cref="BasicRegexNFAState{T}"/> 类的新实例。</returns>
        BasicRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false);

        /// <summary>
        /// 远程创建指定 <see cref="BasicRegexNFAState{T}"/> 对象的空副本。
        /// </summary>
        /// <param name="state">指定正则构造非确定自动机的状态。</param>
        /// <returns>指定正则构造非确定自动机的状态的空副本。</returns>
        BasicRegexNFAState<T> ActivateRegexNFAStateFromDumplication(BasicRegexNFAState<T> state);

        /// <summary>
        /// 远程创建 <see cref="BasicRegexDFAState{T}"/> 类的新实例。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该有限自动机的状态是否为结束状态。默认为 false 。</param>
        /// <returns><see cref="BasicRegexDFAState{T}"/> 类的新实例。</returns>
        BasicRegexDFAState<T> ActivateRegexDFAState(bool isTerminal = false);

        /// <summary>
        /// 从条件正则（ <see cref="RegexCondition{T}"/> ）对象中获取信息，远程创建 NFA 转换的新实例。
        /// </summary>
        /// <param name="regex">指定的条件正则对象。</param>
        /// <returns>NFA 转换的新实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        BasicRegexFATransition<T, BasicRegexNFAState<T>> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex);

        /// <summary>
        /// 远程创建指定正则构造非确定自动机的转换的空副本。
        /// </summary>
        /// <param name="transition">指定正则构造非确定自动机的转换。</param>
        /// <returns>指定正则构造非确定自动机的转换的空副本。</returns>
        BasicRegexFATransition<T, BasicRegexNFAState<T>> ActivateRegexNFATransitionFromDumplication(BasicRegexFATransition<T, BasicRegexNFAState<T>> transition);

        /// <summary>
        /// 远程创建 <see cref="BasicRegexNFAEpsilonTransition{T}"/> 类的新实例。
        /// </summary>
        /// <returns><see cref="BasicRegexNFAEpsilonTransition{T}"/> 类的新实例。</returns>
        BasicRegexNFAEpsilonTransition<T> ActivateRegexNFAEpsilonTransition();

        /// <summary>
        /// 从可接受的对象集中获取信息，远程创建 DFA 转换的新实例。
        /// </summary>
        /// <param name="set">指定的可接受的对象集。</param>
        /// <returns>DFA 转换的新实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> 的值为 null 。</exception>
        BasicRegexFATransition<T, BasicRegexDFAState<T>> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set);

        ISet<T> GetAccreditedSetFromRegexNFATransition(BasicRegexFATransition<T, BasicRegexNFAState<T>> transition);

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
        BasicRegexFATransition<T, BasicRegexDFAState<T>> CombineRegexDFATransitions(IEnumerable<BasicRegexFATransition<T, BasicRegexDFAState<T>>> dfaTransitions);
    }
}
