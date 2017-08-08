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
        /// 初始化 <see cref="RegexSeries{T}"/> 类的新实例。子类继承的默认构造函数。
        /// </summary>
        protected RegexSeries() { }

        /// <summary>
        /// 初始化 <see cref="RegexSeries{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="series">要作为内部正则对象列表的参数数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> 的值为 null 。</exception>
        public RegexSeries(params RegexObject<T>[] series) :
            this(series?.AsEnumerable() ?? throw new ArgumentNullException(nameof(series)))
        { }

        /// <summary>
        /// 初始化 <see cref="RegexSeries{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="series">指定的内部正则对象列表。</param>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> 的值为 null 。</exception>
        public RegexSeries(IEnumerable<RegexObject<T>> series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));

            this.series = new List<RegexObject<T>>(series.Where(item => item != null));
        }

        /// <summary>
        /// 将此串联正则与另一个正则对象串联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="RegexObject{T}.Concat(RegexObject{T})"/>
        public override RegexObject<T> Concat(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexSeries<T> series)
                return new RegexSeries<T>(this.series.Concat(series.series));
            else
                return new RegexSeries<T>(this, regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexSeries<T>(this.series);
        }

        public override string ToString()
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