using SamLu.Diagnostics;
using SamLu.RegularExpression.ObjectModel;
using SamLu.Runtime;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的以 <see cref="RangeSet{T}"/> 划定接受输入范围的有限自动机的转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TRegexFAState">正则表达式构造的有限自动机的状态的类型。</typeparam>
    [DebugInfoProxy(typeof(RangeSetRegexFATransition<,>._DebugInfo), new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 })]
    public class RangeSetRegexFATransition<T, TRegexFAState> : BasicRegexFATransition<T, TRegexFAState>
        where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
    {
        /// <summary>
        /// <see cref="RangeSetRegexFATransition{T, TRegexFAState}"/> 内部范围集。
        /// </summary>
        protected RangeSet<T> set;
        /// <summary>
        /// 获取范围集的只读形式。
        /// </summary>
        public ISet<T> Set => new ReadOnlySet<T>(this.set);

        /// <summary>
        /// 初始化 <see cref="RangeSetRegexFATransition{T, TRegexFAState}"/> 的新实例，该实例使用指定的 <see cref="RangeSet{T}"/> 对象。
        /// </summary>
        /// <param name="set">指定的范围集对象。</param>
        public RangeSetRegexFATransition(RangeSet<T> set) : 
            base((set ?? throw new ArgumentNullException(nameof(set))).Contains) => this.set = set;

        /// <summary>
        /// 为 <see cref="RangeSetRegexFATransition{T, TRegexFAState}"/> 提供调试信息。
        /// </summary>
        internal class _DebugInfo : IDebugInfo
        {
            /// <summary>
            /// 基础正则表达式（ Basic Regular Expression ）构造的以 <see cref="RangeSet{T}"/> 划定接受输入范围的有限自动机的转换。
            /// </summary>
            protected RangeSetRegexFATransition<T, TRegexFAState> transition;

            string IDebugInfo.DebugInfo =>
                (this.transition.set as IEnumerable<T>).Any() ? $"< {this.transition.set.GetDebugInfo()} >" : string.Empty;

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="transition">基础正则表达式（ Basic Regular Expression ）构造的以 <see cref="RangeSet{T}"/> 划定接受输入范围的有限自动机的转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RangeSetRegexFATransition<T, TRegexFAState> transition, params object[] args) =>
                this.transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }
    }
}
