using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    /// <summary>
    /// 定义了正则表达式构造的有限状态机的功能转换应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexFunctionalTransition<T> : IRegexFSMTransition<T> { }

    /// <summary>
    /// 定义了正则表达式构造的有限状态机的功能转换应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">有限状态机的状态的类型。</typeparam>
    public interface IRegexFunctionalTransition<T, TState> : IRegexFunctionalTransition<T>, IRegexFSMTransition<T, TState> where TState : IRegexFSMState<T> { }
}
