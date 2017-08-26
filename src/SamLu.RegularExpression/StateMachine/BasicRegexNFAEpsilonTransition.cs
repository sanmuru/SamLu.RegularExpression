using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的有限自动机的 ε 转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class BasicRegexNFAEpsilonTransition<T> : BasicRegexFATransition<T, BasicRegexNFAState<T>>, IEpsilonTransition<BasicRegexNFAState<T>>
    {
        /// <summary>
        /// 获取一个方法，该方法确定 <see cref="BasicRegexNFAEpsilonTransition{T}"/> 接受的输入是否满足条件。
        /// </summary>
        /// <remarks>ε 转换不支持获取此方法。</remarks>
        /// <exception cref="NotSupportedException">ε 转换不支持获取此方法。</exception>
        public sealed override Predicate<T> Predicate => throw new NotSupportedException("ε 转换不支持获取此方法。");

        public BasicRegexNFAEpsilonTransition() : base() { }
    }
}
