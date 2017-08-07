using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示正则对象。所有提供正则支持的正则模块应继承此类。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public abstract class RegexObject<T> : IEquatable<RegexObject<T>>, ICloneable
    {
        /// <summary>
        /// 将此正则对象与另一个正则对象串联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public virtual RegexObject<T> Concat(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexRepeat<T> repeat && this.Equals(repeat.InnerRegex))
                return new RegexRepeat<T>(
                    this,
                    repeat.MinimumCount.HasValue ? repeat.MinimumCount + 1 : null,
                    repeat.MaximumCount.HasValue ? repeat.MaximumCount + 1 : null
                );
            else if (regex is RegexSeries<T> series)
                return new RegexSeries<T>(new RegexObject<T>[] { this }.Concat(series.Series));
            else
                return new RegexSeries<T>(this, regex);
        }

        /// <summary>
        /// 将此正则对象与另一个正则对象并联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public virtual RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexParallels<T> parallels)
                return new RegexParallels<T>(new RegexObject<T>[] { this }.Concat(parallels.Parallels));
            else
                return new RegexParallels<T>(this, regex);
        }

        /// <summary>
        /// 确定指定的正则对象是否等于当前的正则对象。
        /// </summary>
        /// <param name="regex">指定的正则对象。</param>
        /// <returns>如果 <paramref name="regex"/> 与当前的正则对象相同，则返回 true ；否则返回 false 。</returns>
        public virtual bool Equals(RegexObject<T> regex)
        {
            if (regex == null) return false;
            else return object.ReferenceEquals(this, regex);
        }

        /// <summary>
        /// 串联两个正则对象。
        /// </summary>
        /// <param name="left">第一个正则对象。</param>
        /// <param name="right">第二个正则对象。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="right"/> 的值为 null 。</exception>
        /// <seealso cref="Concat(RegexObject{T})"/>
        public static RegexObject<T> operator +(RegexObject<T> left, RegexObject<T> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            return left.Concat(right);
        }

        /// <summary>
        /// 并联两个正则对象。
        /// </summary>
        /// <param name="left">第一个正则对象。</param>
        /// <param name="right">第二个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="right"/> 的值为 null 。</exception>
        /// <seealso cref="Unions(RegexObject{T})"/>
        public static RegexObject<T> operator |(RegexObject<T> left, RegexObject<T> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            return left.Unions(right);
        }

        /// <summary>
        /// 创建指定正则对象的重复正则。
        /// </summary>
        /// <param name="count">重复的次数。</param>
        /// <param name="regex">指定的正则对象。</param>
        /// <returns>指定正则对象的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="operator *(RegexObject{T}, ulong)"/>
        public static RegexRepeat<T> operator *(ulong count, RegexObject<T> regex) => regex * count;

        /// <summary>
        /// 创建指定正则对象的重复正则。
        /// </summary>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="count">重复的次数。</param>
        /// <returns>指定正则对象的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="operator *(ulong, RegexObject{T})"/>
        public static RegexRepeat<T> operator *(RegexObject<T> regex, ulong count)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexRepeat<T> repeat)
            {
                if (!repeat.MinimumCount.HasValue && !repeat.MaximumCount.HasValue)
                    return repeat;
                else
                    return new RegexRepeat<T>(
                        repeat.InnerRegex,
                        repeat.MinimumCount * count,
                        repeat.MaximumCount * count
                    );
            }
            else
                return new RegexRepeat<T>(regex, count, count);
        }

        protected internal abstract RegexObject<T> Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}