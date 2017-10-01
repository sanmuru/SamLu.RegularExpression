using SamLu.Diagnostics;
using SamLu.RegularExpression.StateMachine;
using SamLu.RegularExpression.StateMachine.FunctionalTransitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    /// <summary>
    /// 为 <see cref="RegexFSMCaptureIDStorageTransition{T}"/> 提供调试信息。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexFSMCaptureIDStorageTransitionDebugInfo<T> : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMCaptureIDStorageTransition<T>>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMCaptureIDStorageTransition{T}"/> 的显式名称。
        /// </summary>
        protected override string Name => "storage";

        /// <summary>
        /// 获取 <see cref="RegexFSMCaptureIDStorageTransition{T}"/> 的显式参数序列。
        /// </summary>
        protected override IEnumerable<string> Parameters =>
            new string[] { $"id = {{{base.functionalTransition.ID.GetDebugInfo()}}}" };

        /// <summary>
        /// 使用规范参数列表初始化 <see cref="RegexFSMCaptureIDStorageTransitionDebugInfo{T}"/> 类的新实例。
        /// </summary>
        /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
        /// <param name="args">获取调试信息的参数列表。</param>
        public RegexFSMCaptureIDStorageTransitionDebugInfo(RegexFSMCaptureIDStorageTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
    }

    /// <summary>
    /// 为 <see cref="RegexFSMCaptureIDStorageTransition{T, TState}"/> 提供调试信息。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    public class RegexFSMCaptureIDStorageTransitionDebugInfo<T, TState> : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMCaptureIDStorageTransition<T, TState>>
        where TState : IRegexFSMState<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMCaptureIDStorageTransition{T, TState}"/> 的显式名称。
        /// </summary>
        protected override string Name => "storage";

        /// <summary>
        /// 获取 <see cref="RegexFSMCaptureIDStorageTransition{T, TState}"/> 的显式参数序列。
        /// </summary>
        protected override IEnumerable<string> Parameters =>
            new string[] { $"id = {{{base.functionalTransition.ID.GetDebugInfo()}}}" };

        /// <summary>
        /// 使用规范参数列表初始化 <see cref="RegexFSMCaptureIDStorageTransitionDebugInfo{T, TState}"/> 类的新实例。
        /// </summary>
        /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
        /// <param name="args">获取调试信息的参数列表。</param>
        public RegexFSMCaptureIDStorageTransitionDebugInfo(RegexFSMCaptureIDStorageTransition<T, TState> functionalTransition, params object[] args) : base(functionalTransition, args) { }
    }
}
