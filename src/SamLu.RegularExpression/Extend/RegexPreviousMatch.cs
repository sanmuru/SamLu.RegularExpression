using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则表达式匹配过程中的上一个匹配所在位置。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RegexPreviousMatch<T> : RegexObject<T>, IRegexAnchorPoint<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexPreviousMatch{T}"/> 类的新实例。
        /// </summary>
        public RegexPreviousMatch() { }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexPreviousMatch<T>();
        }
    }
}
