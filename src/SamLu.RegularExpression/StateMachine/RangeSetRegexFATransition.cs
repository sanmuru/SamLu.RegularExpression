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
    [DebugInfoProxy(typeof(RangeSetRegexFATransition<,>._DebugInfo), new[] { TypeParameterFillin.TypeParameter_0, TypeParameterFillin.TypeParameter_1 })]
    public class RangeSetRegexFATransition<T, TRegexFAState> : RegexFATransition<T, TRegexFAState>
        where TRegexFAState : IRegexFSMState<T>
    {
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

        internal class _DebugInfo : IDebugInfo
        {
            private RangeSetRegexFATransition<T, TRegexFAState> transition;

            string IDebugInfo.DebugInfo =>
                (this.transition.set as IEnumerable<T>).Any() ? $"< {this.transition.set.GetDebugInfo()} >" : string.Empty;

            public _DebugInfo(RangeSetRegexFATransition<T, TRegexFAState> transition, params object[] args) =>
                this.transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }
    }
}
