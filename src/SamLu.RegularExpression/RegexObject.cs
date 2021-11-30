using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示正则对象。所有提供正则支持的正则模块应继承此类。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public abstract class RegexObject<T> : IEquatable<RegexObject<T>>, ICloneable
    {
        /// <summary>
        /// 内部储存的提供操作正则对象的方法的对象。
        /// </summary>
        protected RegexProvider<T> provider;

        /// <summary>
        /// 使用指定的 <see cref="RegexProvider{T}"/> 初始化正则对象。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> 的值为 <see langword="null"/> 。</exception>
        protected RegexObject(RegexProvider<T> provider) => this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// 将此正则对象与另一个正则对象串联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> <see langword="null"/> 。</exception>
        public virtual RegexObject<T> Concat(RegexObject<T> regex)
        {
            ArgumentNullException.ThrowIfNull(nameof(regex));

            if (regex is RegexRepeat<T> repeat && this.Equals(repeat.InnerRegex))
                return this.Concat(repeat);
            else if (regex is RegexSeries<T> series && series.Series.All(item => !this.Equals(item)))
                return this.Concat(series);
            else
                return provider.CreateRegexObject<RegexSeries<T>>(null, this, regex);
        }

        /// <summary>
        /// 将此正则对象与另一个重复正则对象串联。
        /// </summary>
        /// <param name="repeat">另一个重复正则对象。其内部正则对象与此正则对象相等。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        protected virtual RegexObject<T> Concat(RegexRepeat<T> repeat) =>
            this.provider.CreateRegexObject(
                repeat,
                this,
                repeat.MinimumCount.HasValue ? repeat.MinimumCount + 1 : null,
                repeat.MaximumCount.HasValue ? repeat.MaximumCount + 1 : null
            );

        /// <summary>
        /// 将此正则对象与另一个串联正则对象串联。
        /// </summary>
        /// <param name="series">另一个串联正则对象。其内部列表中的所有正则对象均不与此正则对象相等。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        protected virtual RegexObject<T> Concat(RegexSeries<T> series) =>
            this.provider.CreateRegexObject(
                series,
                series.Series.Prepend(this)
            );

        /// <summary>
        /// 将此正则对象与另一个正则对象并联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> <see langword="null"/> 。</exception>
        public virtual RegexObject<T> Unions(RegexObject<T> regex)
        {
            ArgumentNullException.ThrowIfNull(regex);

            if (regex is RegexParallels<T> parallels && parallels.Parallels.All(item => !this.Equals(item)))
                return this.Union(parallels);
            else
                return provider.CreateRegexObject<RegexParallels<T>>(null, this, regex);
        }

        /// <summary>
        /// 将此正则对象与另一个并联正则对象并联。
        /// </summary>
        /// <param name="parallels">另一个并联正则对象。其内部列表中的所有正则对象均不与此正则对象相等。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        protected virtual RegexObject<T> Union(RegexParallels<T> parallels) =>
            this.provider.CreateRegexObject(
                parallels,
                parallels.Parallels.Prepend(this)
            );

        /// <summary>
        /// 确定指定的正则对象是否等于当前的正则对象。此相等比较也确保两个正则对象内部的 <see cref="RegexProvider{T}"/> 对象引用相等。
        /// </summary>
        /// <param name="other">指定的正则对象。</param>
        /// <returns>
        /// <para>若两个正则对象内部的 <see cref="RegexProvider{T}"/> 对象引用不相等，则返回 <see langword="false"/> 。</para>
        /// <para>如果 <paramref name="other"/> 与当前的正则对象相同，则返回 <see langword="true"/> ；否则返回 <see langword="false"/> 。</para>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 <see langword="null"/> 。</exception>
        public bool Equals(RegexObject<T>? other)
        {
            ArgumentNullException.ThrowIfNull(other);

            return object.ReferenceEquals(this.provider, other.provider) && this.EqualsInternal(other);
        }

        /// <summary>
        /// 确定指定的正则对象是否等于当前的正则对象。
        /// </summary>
        /// <param name="regex">指定的正则对象。</param>
        /// <returns>如果 <paramref name="regex"/> 与当前的正则对象相同，则返回 true ；否则返回 false 。</returns>
        protected virtual bool EqualsInternal(RegexObject<T> regex) => object.ReferenceEquals(this, regex);

        /// <inheritdoc/>
        public sealed override bool Equals(object? obj) => obj is RegexObject<T> regexObject && this.Equals(regexObject);

        /// <inheritdoc/>
        public override abstract int GetHashCode();

        /// <summary>
        /// 串联两个正则对象。
        /// </summary>
        /// <param name="left">第一个正则对象。</param>
        /// <param name="right">第二个正则对象。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> <see langword="null"/> 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="right"/> <see langword="null"/> 。</exception>
        /// <seealso cref="Concat(RegexObject{T})"/>
        public static RegexObject<T> operator +(RegexObject<T> left, RegexObject<T> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return left.Concat(right);
        }

        /// <summary>
        /// 并联两个正则对象。
        /// </summary>
        /// <param name="left">第一个正则对象。</param>
        /// <param name="right">第二个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> <see langword="null"/> 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="right"/> <see langword="null"/> 。</exception>
        /// <seealso cref="Unions(RegexObject{T})"/>
        public static RegexObject<T> operator |(RegexObject<T> left, RegexObject<T> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return left.Unions(right);
        }

        /// <summary>
        /// 创建指定正则对象的重复正则。
        /// </summary>
        /// <param name="count">重复的次数。</param>
        /// <param name="regex">指定的正则对象。</param>
        /// <returns>指定正则对象的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> <see langword="null"/> 。</exception>
        /// <seealso cref="operator *(RegexObject{T}, ulong)"/>
        public static RegexRepeat<T> operator *(ulong count, RegexObject<T> regex) => regex * count;

        /// <summary>
        /// 创建指定正则对象的重复正则。
        /// </summary>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="count">重复的次数。</param>
        /// <returns>指定正则对象的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> <see langword="null"/> 。</exception>
        /// <seealso cref="operator *(ulong, RegexObject{T})"/>
        public static RegexRepeat<T> operator *(RegexObject<T> regex, ulong count)
        {
            ArgumentNullException.ThrowIfNull(regex);

            if (regex is RegexRepeat<T> repeat)
            {
                if (!repeat.MinimumCount.HasValue && !repeat.MaximumCount.HasValue)
                    return repeat;
                else
                    return regex.provider.CreateRegexObject(
                        repeat,
                        repeat.InnerRegex,
                        repeat.MinimumCount * count,
                        repeat.MaximumCount * count
                    );
            }
            else
                return regex.provider.CreateRegexObject<RegexRepeat<T>>(null, regex, count, count);
        }

        /// <summary>
        /// 创建与当前对象实例一致的新实例。
        /// </summary>
        /// <returns>与当前对象实例一致的新实例。</returns>
        protected internal abstract RegexObject<T> Clone();

        object ICloneable.Clone() => this.Clone();
    }
}
