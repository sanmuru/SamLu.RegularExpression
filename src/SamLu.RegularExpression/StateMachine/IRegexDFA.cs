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
    }

    /// <summary>
    /// 定义了正则表达式构造的确定的有限自动机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">确定的有限自动机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">确定的有限自动机的转换的类型。</typeparam>
    public interface IRegexDFA<T, TState, TTransition> : IRegexDFA<T>, IRegexFSM<T, TState, TTransition>, IDFA<TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
    }
}
