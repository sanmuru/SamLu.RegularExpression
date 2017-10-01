using SamLu.Diagnostics;
using SamLu.RegularExpression.Diagnostics;
using SamLu.Runtime;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    /// <summary>
    /// 表示正则构造的有限状态机的非贪婪匹配功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMNonGreedyRepeatTransitionDebugInfo<>),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMNonGreedyRepeatTransition<T> : RegexFSMFunctionalTransition<T> { }

    /// <summary>
    /// 表示正则构造的有限状态机的非贪婪匹配功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMNonGreedyRepeatTransitionDebugInfo<,>),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public sealed class RegexFSMNonGreedyRepeatTransition<T, TState> : RegexFSMFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    { }
}
