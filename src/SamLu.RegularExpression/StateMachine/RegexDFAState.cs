using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的确定的有限自动机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexDFAState<T> : DFAState<RegexFATransition<T, RegexDFAState<T>>>
    {
        /// <summary>
        /// 获取或设置一个值，指示该有限自动机的状态是否为结束状态。
        /// </summary>
        public bool IsTerminal { get; set; }

        /// <summary>
        /// 初始化 <see cref="RegexDFAState{T}"/> 类的新实例。
        /// </summary>
        public RegexDFAState() : this(false) { }

        /// <summary>
        /// 初始化 <see cref="RegexDFAState{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public RegexDFAState(bool isTerminal)
        {
            this.IsTerminal = isTerminal;
        }
    }
}
