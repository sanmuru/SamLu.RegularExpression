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
        typeof(RegexFSMCaptureIDCheckTransitionDebugInfo<>),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMCaptureIDCheckTransition<T> : RegexFSMPredicateTransition<T>
    {
        private object id;

        [RegexFSMFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexFSMCaptureIDCheckTransition(object id, Func<RegexFSMCaptureIDCheckTransition<T>, object[], bool> predicate) : base()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.id = id;
            base.predicate = (sender, args) => predicate((RegexFSMCaptureIDCheckTransition<T>)sender, args);
        }
    }

    /// <summary>
    /// 表示正则构造的有限状态机的捕获检测功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMCaptureIDCheckTransitionDebugInfo<,>),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public sealed class RegexFSMCaptureIDCheckTransition<T, TState> : RegexFSMPredicateTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        private object id;

        [RegexFSMFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexFSMCaptureIDCheckTransition(object id, Func<RegexFSMCaptureIDCheckTransition<T, TState>, object[], bool> predicate) : base()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.id = id;
            base.predicate = (sender, args) => predicate((RegexFSMCaptureIDCheckTransition<T, TState>)sender, args);
        }
    }
}
