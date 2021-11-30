using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示并联正则。按顺序测试是否匹配内部正则对象列表的某一项。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexParallels<T> : RegexObject<T>
    {
        /// <summary>
        /// 并联正则内部的正则对象列表。
        /// </summary>
        protected List<RegexObject<T>> parallels;
        /// <summary>
        /// 获取并联正则内部的正则对象列表。
        /// </summary>
        /// <value>
        /// 并联正则内部的正则对象列表。
        /// <para>列表是只读的。</para>
        /// </value>
        public IList<RegexObject<T>> Parallels => new ReadOnlyCollection<RegexObject<T>>(this.parallels);

        /// <summary>
        /// 初始化 <see cref="RegexParallels{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <param name="parallels">要作为内部正则对象列表的参数数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>s
        public RegexParallels(RegexProvider<T> provider, params RegexObject<T>[] parallels) :
            this(provider, parallels switch { null => throw new ArgumentNullException(nameof(parallels)), _ => (IEnumerable<RegexObject<T>>)parallels })
        { }

        /// <summary>
        /// 初始化 <see cref="RegexParallels{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <param name="parallels">指定的内部正则对象列表。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>
        public RegexParallels(RegexProvider<T> provider, IEnumerable<RegexObject<T>> parallels) : base(provider)
        {
            ArgumentNullException.ThrowIfNull(parallels);

            this.parallels = new(parallels.Where(item => item is not null));
        }

        /// <inheritdoc/>
        /// <seealso cref="RegexObject{T}.Union(RegexParallels{T})"/>
        protected override RegexObject<T> Union(RegexParallels<T> parallels) =>
            this.provider.CreateRegexObject(
                parallels,
                this,
                this.parallels.Concat(parallels.parallels)
            );

        /// <inheritdoc/>
        protected internal override RegexParallels<T> Clone() => this.provider.CreateRegexObject(this, this.parallels);

        /// <inheritdoc/>
        public override int GetHashCode() => this.Parallels.Select(item => item.GetHashCode()).Aggregate(0, (hashcodeX, hashcodeY) => hashcodeX ^ hashcodeY);

        /// <summary>
        /// 获取并联正则对象的字符串表示。
        /// </summary>
        /// <returns>并联正则对象的字符串表示。</returns>
        public override string? ToString()
        {
            var parallels = this.Parallels;

            if (parallels.Count == 0) return string.Empty;
            else if (parallels.Count == 1)
                return $"{parallels[0]}";
            else
                return $"({string.Join("|", parallels)})";
        }
    }
}