using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的非确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class BasicRegexNFA<T> : RegexNFA<T, BasicRegexNFAState<T>, BasicRegexFATransition<T, BasicRegexNFAState<T>>, BasicRegexFSMEpsilonTransition<T>>
    {
        /// <summary>
        /// 初始化 <see cref="BasicRegexNFA{T}"/> 类的新实例。
        /// </summary>
        public BasicRegexNFA() : base() { }
    }
}
