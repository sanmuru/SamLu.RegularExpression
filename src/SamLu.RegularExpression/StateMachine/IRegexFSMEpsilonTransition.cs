using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的有限状态机的 ε 转换应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexFSMEpsilonTransition<T> : IRegexFSMTransition<T>, IEpsilonTransition
    {
    }

    /// <summary>
    /// 定义了正则表达式构造的有限状态机的 ε 转换应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">有限状态机的状态的类型。</typeparam>
    public interface IRegexFSMEpsilonTransition<T, TState> : IRegexFSMEpsilonTransition<T>, IRegexFSMTransition<T, TState>, IEpsilonTransition<TState>
        where TState : IRegexFSMState<T>
    {
    }
}
