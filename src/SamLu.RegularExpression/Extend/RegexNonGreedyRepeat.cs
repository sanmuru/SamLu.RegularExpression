using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示非贪婪的重复正则。匹配内部正则对象重复指定的可能重复次数的最小值。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RegexNonGreedyRepeat<T> : RegexObject<T>
    {
        /// <summary>
        /// 非贪婪的重复正则内部的重复正则对象。
        /// </summary>
        protected RegexRepeat<T> innerRepeat;

        /// <summary>
        /// 获取非贪婪的重复正则内部的重复正则对象。
        /// </summary>
        public virtual RegexRepeat<T> InnerRepeat => this.innerRepeat;

        /// <summary>
        /// 初始化 <see cref="RegexNonGreedyRepeat{T}"/> 类的新实例。子类继承的默认构造函数。
        /// </summary>
        protected RegexNonGreedyRepeat() { }

        /// <summary>
        /// 初始化 <see cref="RegexNonGreedyRepeat{T}"/> 类的新实例。该实例指定内部的重复正则对象。
        /// </summary>
        /// <param name="repeat">指定的内部重复正则对象。</param>
        public RegexNonGreedyRepeat(RegexRepeat<T> repeat) =>
            this.innerRepeat = repeat ?? throw new ArgumentNullException(nameof(repeat));

        protected internal override RegexObject<T> Clone()
        {
            return new RegexNonGreedyRepeat<T>(this.innerRepeat);
        }
    }
}
