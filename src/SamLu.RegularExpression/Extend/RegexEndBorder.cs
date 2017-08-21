using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则表达式的结束边界。匹配序列的结尾。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RegexEndBorder<T> : RegexObject<T>, IRegexAnchorPoint<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexEndBorder{T}"/> 类的新实例。
        /// </summary>
        public RegexEndBorder() { }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexEndBorder<T>();
        }
    }
}
