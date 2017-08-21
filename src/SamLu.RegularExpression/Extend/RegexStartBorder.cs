using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则表达式的开始边界。匹配序列的开头。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public sealed class RegexStartBorder<T> : RegexObject<T>, IRegexAnchorPoint<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexStartBorder{T}"/> 类的新实例。
        /// </summary>
        public RegexStartBorder() { }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexStartBorder<T>();
        }
    }
}
