using SamLu.Diagnostics;
using SamLu.RegularExpression.StateMachine.FunctionalTransitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    /// <summary>
    /// 为 <see cref="IRegexFSMFunctionalTransition{T}"/> 及派生、实现其的类型提供调试信息。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TFunctionalTransition">正则表达式构造的有限状态机的功能转换的类型。</typeparam>
    public abstract class RegexFSMFunctionalTransitionDebugInfoBase<T, TFunctionalTransition> : IDebugInfo
        where TFunctionalTransition : IRegexFSMFunctionalTransition<T>
    {
        /// <summary>
        /// 正则表达式构造的有限状态机的功能转换
        /// </summary>
        protected TFunctionalTransition functionalTransition;
        /// <summary>
        /// 调试信息的参数列表。
        /// </summary>
        protected object[] args;

        /// <summary>
        /// 获取 <see cref="functionalTransition"/> 的显式名称。
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// 获取 <see cref="functionalTransition"/> 的显式参数序列。
        /// </summary>
        protected abstract IEnumerable<string> Parameters { get; }

        /// <summary>
        /// 获取调试信息。
        /// </summary>
        protected virtual string DebugInfo =>
            string.Format("ft:'{0}'{1}",
                this.Name,
                (this.Parameters == null ? string.Empty : $" = {{{string.Join(",", this.Parameters)}}}")
            );

        /// <summary>
        /// 使用规范参数列表初始化 <see cref="RegexFSMFunctionalTransitionDebugInfoBase{T, TFunctionalTransition}"/> 类的新实例。
        /// </summary>
        /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
        /// <param name="args">获取调试信息的参数列表。</param>
        protected RegexFSMFunctionalTransitionDebugInfoBase(TFunctionalTransition functionalTransition, params object[] args)
        {
            if (functionalTransition == null) throw new ArgumentNullException(nameof(functionalTransition));
            
            this.functionalTransition = functionalTransition;
            this.args = args;
        }

        #region IDebugInfo Implementation
        string IDebugInfo.DebugInfo => this.DebugInfo;
        #endregion
    }
}
