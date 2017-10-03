using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的确定的有限自动机的状态应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexDFAState<T> : IRegexFSMState<T>, IDFAState
    {
        /// <summary>
        /// 添加指定的接受输入转换。
        /// </summary>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="acceptInputTransition"/> 的值为 null 。</exception>
        bool AttachTransition(IAcceptInputTransition<T> acceptInputTransition);

        /// <summary>
        /// 移除指定的接受输入转换。
        /// </summary>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="acceptInputTransition"/> 的值为 null 。</exception>
        bool RemoveTransition(IAcceptInputTransition<T> acceptInputTransition);
    }

    /// <summary>
    /// 定义了正则表达式构造的确定的有限自动机的状态应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TTransition">非确定的有限自动机的转换的类型。</typeparam>
    public interface IRegexDFAState<T, TTransition> : IRegexDFAState<T>, IRegexFSMState<T, TTransition>, IDFAState<TTransition>
        where TTransition : IRegexFSMTransition<T>
    { }
}
