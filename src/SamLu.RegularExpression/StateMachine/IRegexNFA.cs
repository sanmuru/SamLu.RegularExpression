using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的非确定的有限自动机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexNFA<T> : IRegexFSM<T>, INFA
    {
        /// <summary>
        /// 向 <see cref="IRegexNFA{T}"/> 的一个指定状态添加指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        bool AttachTransition(IRegexNFAState<T> state, IRegexFSMEpsilonTransition<T> epsilonTransition);

        /// <summary>
        /// 从 <see cref="IRegexNFA{T}"/> 的一个指定状态移除指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        bool RemoveTransition(IRegexNFAState<T> state, IRegexFSMEpsilonTransition<T> epsilonTransition);
    }

    /// <summary>
    /// 定义了正则表达式构造的非确定的有限自动机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">非确定的有限自动机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">非确定的有限自动机的转换的类型。</typeparam>
    /// <typeparam name="TEpsilonTransition">非确定的有限自动机的 ε 转换的类型。</typeparam>
    public interface IRegexNFA<T, TState, TTransition, TEpsilonTransition> : IRegexNFA<T>, IRegexFSM<T, TState, TTransition>, INFA<TState, TTransition, TEpsilonTransition>
        where TState : IRegexNFAState<T, TTransition, TEpsilonTransition>
        where TTransition : class, IRegexFSMTransition<T, TState>
        where TEpsilonTransition : TTransition, IRegexFSMEpsilonTransition<T, TState>
    {
    }
}
