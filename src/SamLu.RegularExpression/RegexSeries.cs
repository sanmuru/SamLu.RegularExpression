using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示串联正则。按顺序匹配内部正则对象列表各项。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexSeries<T> : RegexObject<T>
    {
        /// <summary>
        /// 串联正则内部的正则对象列表。
        /// </summary>
        protected List<RegexObject<T>> series;
        /// <summary>
        /// 获取串联正则内部的正则对象列表。
        /// </summary>
        /// <value>
        /// 串联正则内部的正则对象列表。
        /// <para>列表是只读的。</para>
        /// </value>
        public IList<RegexObject<T>> Series => new ReadOnlyCollection<RegexObject<T>>(this.series);

        /// <summary>
        /// 初始化 <see cref="RegexSeries{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <param name="series">要作为内部正则对象列表的参数数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> <see langword="null"/> 。</exception>
        public RegexSeries(RegexProvider<T> provider, params RegexObject<T>[] series) :
            this(provider, series switch { null => throw new ArgumentNullException(nameof(series)), _ => (IEnumerable<RegexObject<T>>)series })
        { }

        /// <summary>
        /// 初始化 <see cref="RegexSeries{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <param name="series">指定的内部正则对象列表。</param>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> <see langword="null"/> 。</exception>
        public RegexSeries(RegexProvider<T> provider, IEnumerable<RegexObject<T>> series) : base(provider)
        {
            ArgumentNullException.ThrowIfNull(series);

            this.series = new(series.Where(item => item is not null));
        }

        /// <inheritdoc/>
        /// <seealso cref="RegexObject{T}.Concat(RegexObject{T})"/>
        protected override RegexObject<T> Concat(RegexSeries<T> series) =>
            this.provider.CreateRegexObject(
                this,
                this.series.Concat(series.series)
            );

        /// <inheritdoc/>
        protected internal override RegexSeries<T> Clone() => this.provider.CreateRegexObject(this, this.series);

        /// <inheritdoc/>
        public override int GetHashCode() => this.series.Select(item => item.GetHashCode()).Aggregate(0, (hashcodeX, hashcodeY) => hashcodeX ^ hashcodeY);

        /// <summary>
        /// 获取串联正则对象的字符串表示。
        /// </summary>
        /// <returns>串联正则对象的字符串表示。</returns>
        public override string? ToString()
        {
            var series = this.Series;

            if (series.Count == 0) return string.Empty;
            else if (series.Count == 1)
                return $"{series[0]}";
            else
                return $"({string.Join(string.Empty, series)})";
        }
    }
}