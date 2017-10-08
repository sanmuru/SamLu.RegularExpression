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
    /// 表示正则构造的有限状态机的非贪婪匹配功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMNonGreedyRepeatTransition<>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMNonGreedyRepeatTransition<T> : RegexFSMRepeatTransitionBase<T>
    {
        internal sealed class _DebugInfo : RegexFSMRepeatTransitionBase<T>._DebugInfoBase<RegexFSMNonGreedyRepeatTransition<T>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T}"/> 的显式名称。
            /// </summary>
            protected override string Name => "non-greedy";

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMNonGreedyRepeatTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }

    /// <summary>
    /// 表示正则构造的有限状态机的非贪婪匹配功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMNonGreedyRepeatTransition<,>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public sealed class RegexFSMNonGreedyRepeatTransition<T, TState> : RegexFSMRepeatTransitionBase<T, TState>
        where TState : IRegexFSMState<T>
    {
        internal sealed class _DebugInfo : RegexFSMRepeatTransitionBase<T, TState>._DebugInfoBase<RegexFSMNonGreedyRepeatTransition<T, TState>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T, TState}"/> 的显式名称。
            /// </summary>
            protected override string Name => "non-greedy";

            /// <summary>
            /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T, TState}"/> 的显式参数序列。
            /// </summary>
            protected override IEnumerable<string> Parameters => null;

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMNonGreedyRepeatTransition<T, TState> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }
}
