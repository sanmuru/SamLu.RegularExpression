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
    /// 表示正则构造的有限状态机的捕获检测功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMCaptureCheckTransition<>._Debug),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMCaptureCheckTransition<T> : RegexFSMPredicateTransition<T>
    {
        private object idToken;
        private object id;

        [RegexFSMFunctionalTransitionMetadata]
        public object IDToken => this.idToken;

        [RegexFSMFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexFSMCaptureCheckTransition(object idToken, object id, Func<RegexFSMCaptureCheckTransition<T>, object[], bool> predicate) : base()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.idToken = idToken;
            this.id = id;
            base.predicate = (sender, args) => predicate((RegexFSMCaptureCheckTransition<T>)sender, args);
        }

        /// <summary>
        /// 为 <see cref="RegexFSMCaptureCheckTransition{T}"/> 提供调试信息。
        /// </summary>
        internal sealed class _Debug : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMCaptureCheckTransition<T>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureCheckTransition{T}"/> 的显式名称。
            /// </summary>
            protected override string Name => "check";

            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureCheckTransition{T}"/> 的显式参数序列。
            /// </summary>
            protected override IEnumerable<string> Parameters =>
                new string[] { $"id = {{{base.functionalTransition.ID.GetDebugInfo()}}}" };

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_Debug"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _Debug(RegexFSMCaptureCheckTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }

    /// <summary>
    /// 表示正则构造的有限状态机的捕获检测功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMCaptureCheckTransition<,>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public sealed class RegexFSMCaptureCheckTransition<T, TState> : RegexFSMPredicateTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        private object idToken;
        private object id;

        [RegexFSMFunctionalTransitionMetadata]
        public object IDToken => this.idToken;

        [RegexFSMFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexFSMCaptureCheckTransition(object idToken, object id, Func<RegexFSMCaptureCheckTransition<T, TState>, object[], bool> predicate) : base()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.idToken = idToken;
            this.id = id;
            base.predicate = (sender, args) => predicate((RegexFSMCaptureCheckTransition<T, TState>)sender, args);
        }

        /// <summary>
        /// 为 <see cref="RegexFSMCaptureCheckTransition{T, TState}"/> 提供调试信息。
        /// </summary>
        internal sealed class _DebugInfo : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMCaptureCheckTransition<T, TState>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureCheckTransition{T, TState}"/> 的显式名称。
            /// </summary>
            protected override string Name => "check";

            /// <summary>
            /// 获取 <see cref="RegexFSMCaptureCheckTransition{T, TState}"/> 的显式参数序列。
            /// </summary>
            protected override IEnumerable<string> Parameters =>
                new string[] { $"id = {{{base.functionalTransition.ID.GetDebugInfo()}}}" };

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMCaptureCheckTransition<T, TState> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }
}
