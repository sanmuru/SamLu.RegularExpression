using SamLu.Diagnostics;
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
    /// 表示正则构造的有限状态机的贪婪匹配功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMGreedyRepeatTransition<>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMGreedyRepeatTransition<T> : RegexFSMRepeatTransitionBase<T>
    {
        internal sealed class _DebugInfo : RegexFSMRepeatTransitionBase<T>._DebugInfoBase<RegexFSMGreedyRepeatTransition<T>>
        {

            /// <summary>
            /// 获取 <see cref="RegexFSMGreedyRepeatTransition{T}"/> 的显式名称。
            /// </summary>
            protected override string Name => "greedy";

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMGreedyRepeatTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }

    /// <summary>
    /// 表示正则构造的有限状态机的贪婪匹配功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMGreedyRepeatTransition<,>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public sealed class RegexFSMGreedyRepeatTransition<T, TState> : RegexFSMRepeatTransitionBase<T, TState>
        where TState : IRegexFSMState<T>
    {
        internal sealed class _DebugInfo : RegexFSMRepeatTransitionBase<T, TState>._DebugInfoBase<RegexFSMGreedyRepeatTransition<T, TState>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMGreedyRepeatTransition{T, TState}"/> 的显式名称。
            /// </summary>
            protected override string Name => "greedy";

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMGreedyRepeatTransition<T, TState> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }
}
