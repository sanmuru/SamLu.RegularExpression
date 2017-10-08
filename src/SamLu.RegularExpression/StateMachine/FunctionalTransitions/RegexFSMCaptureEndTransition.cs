using SamLu.Diagnostics;
using SamLu.RegularExpression.Diagnostics;
using SamLu.RegularExpression.Extend;
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
    /// 表示正则构造的有限状态机的捕获结束功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMCaptureEndTransition<>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMCaptureEndTransition<T> : RegexFSMFunctionalTransition<T>
    {
        private RegexGroup<T> group;

        [RegexFSMFunctionalTransitionMetadata]
        public RegexGroup<T> Group => this.group;

        public RegexFSMCaptureEndTransition(RegexGroup<T> group) =>
            this.group = group ?? throw new ArgumentNullException(nameof(group));

        /// <summary>
        /// 为 <see cref="RegexFSMCaptureEndTransition{T}"/> 提供调试信息。
        /// </summary>
        internal sealed class _DebugInfo : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMCaptureEndTransition<T>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureEndTransition{T}"/> 的显式名称。
            /// </summary>
            protected override string Name => "end";

            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureEndTransition{T}"/> 的显式参数序列。
            /// </summary>
            protected override IEnumerable<string> Parameters => null;

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMCaptureEndTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }

    /// <summary>
    /// 表示正则构造的有限状态机的捕获结束功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMCaptureEndTransition<,>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public sealed class RegexFSMCaptureEndTransition<T, TState> : RegexFSMFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        private RegexGroup<T> group;

        [RegexFSMFunctionalTransitionMetadata]
        public RegexGroup<T> Group => this.group;

        public RegexFSMCaptureEndTransition(RegexGroup<T> group) =>
            this.group = group ?? throw new ArgumentNullException(nameof(group));
        
        /// <summary>
        /// 为 <see cref="RegexFSMCaptureEndTransition{T, TState}"/> 提供调试信息。
        /// </summary>
        internal sealed class _DebugInfo : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMCaptureEndTransition<T, TState>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureEndTransition{T, TState}"/> 的显式名称。
            /// </summary>
            protected override string Name => "end";

            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureEndTransition{T, TState}"/> 的显式参数序列。
            /// </summary>
            protected override IEnumerable<string> Parameters => null;

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMCaptureEndTransition<T, TState> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }
}
