using SamLu.RegularExpression.Extend;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class BasicRegexDFA<T> : RegexDFA<T, BasicRegexDFAState<T>, BasicRegexFATransition<T, BasicRegexDFAState<T>>>
    {
        /// <summary>
        /// 初始化 <see cref="BasicRegexDFA{T}"/> 类的新实例。
        /// </summary>
        public BasicRegexDFA() : base() { }
    }
}
