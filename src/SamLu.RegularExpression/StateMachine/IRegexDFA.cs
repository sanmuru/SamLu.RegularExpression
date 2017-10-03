using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的确定的有限自动机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexDFA<T> : IRegexFSM<T>, IDFA
    {
        /// <summary>
        /// 为 <see cref="IRegexDFA{T}"/> 的一个指定状态添加指定接受输入转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        bool AttachTransition(IRegexDFAState<T> state, IAcceptInputTransition<T> acceptInputTransition);

        /// <summary>
        /// 从 <see cref="IRegexDFA{T}"/> 的一个指定状态移除指定接受输入转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        bool RemoveTransition(IRegexDFAState<T> state, IAcceptInputTransition<T> acceptInputTransition);
    }

    /// <summary>
    /// 定义了正则表达式构造的确定的有限自动机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">确定的有限自动机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">确定的有限自动机的转换的类型。</typeparam>
    public interface IRegexDFA<T, TState, TTransition> : IRegexDFA<T>, IRegexFSM<T, TState, TTransition>, IDFA<TState, TTransition>
        where TState : IRegexDFAState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>, IAcceptInputTransition<T>
    {
    }
}
