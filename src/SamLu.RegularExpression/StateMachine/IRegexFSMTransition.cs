using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的有限状态机的转换应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexFSMTransition<T> : ITransition
    {
        /// <summary>
        /// 获取或设置 <see cref="IRegexFSMTransition{T}"/> 指向的状态。
        /// </summary>
        new IRegexFSMState<T> Target { get; }

        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        bool SetTarget(IRegexFSMState<T> state);
    }

    /// <summary>
    /// 定义了正则表达式构造的有限状态机的转换应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">有限状态机的状态的类型。</typeparam>
    public interface IRegexFSMTransition<T, TState> : IRegexFSMTransition<T>, ITransition<TState>
        where TState : IRegexFSMState<T>
    {
    }
}
