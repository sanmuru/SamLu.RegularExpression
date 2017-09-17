using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的非确定的有限自动机的状态应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexNFAState<T> : IRegexFSMState<T>, INFAState
    {
        /// <summary>
        /// 添加指定的 ε 转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        bool AttachTransition(IRegexFSMEpsilonTransition<T> epsilonTransition);

        /// <summary>
        /// 移除指定的 ε 转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        bool RemoveTransition(IRegexFSMEpsilonTransition<T> epsilonTransition);
    }

    /// <summary>
    /// 定义了正则表达式构造的非确定的有限自动机的状态应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TTransition">非确定的有限自动机的转换的类型。</typeparam>
    /// <typeparam name="TEpsilonTransition">非确定的有限自动机的 ε 转换的类型。</typeparam>
    public interface IRegexNFAState<T, TTransition, TTEpsilonTransition> : IRegexNFAState<T>, IRegexFSMState<T, TTransition>, INFAState<TTransition, TTEpsilonTransition>
        where TTransition : class, IRegexFSMTransition<T>
        where TTEpsilonTransition : TTransition, IRegexFSMEpsilonTransition<T>
    {
    }
}
