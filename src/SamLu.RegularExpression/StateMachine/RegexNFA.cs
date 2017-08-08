using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的非确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexNFA<T> : NFA<RegexNFAState<T>, RegexFATransition<T, RegexNFAState<T>>, RegexNFAEpsilonTransition<T>>
    {
    }
}
