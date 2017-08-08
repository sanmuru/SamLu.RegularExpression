using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示重复正则。匹配内部正则对象重复指定的次数。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    #region Debugger Support
    [DebuggerDisplay("{InnerRegex}{__DebugString,nq}")]
    #endregion
    public class RegexRepeat<T> : RegexObject<T>
    {
        #region Debugger Support
        private string __DebugString =>
            $"{'{'}{this.MinimumCount ?? ulong.MinValue},{(this.IsInfinte ? string.Empty : this.MaximumCount.Value.ToString())}{'}'}";
        #endregion

        /// <summary>
        /// 重复正则内部的正则对象。
        /// </summary>
        protected RegexObject<T> innerRegex;
        /// <summary>
        /// 获取重复正则内部的正则对象。
        /// </summary>
        public virtual RegexObject<T> InnerRegex => this.innerRegex;

        /// <summary>
        /// 重复正则内部的最小重复次数。
        /// </summary>
        protected ulong? minimumCount = null;
        /// <summary>
        /// 重复正则内部的最大重复次数。
        /// </summary>
        protected ulong? maximumCount = null;

        /// <summary>
        /// 获取重复正则内部的最小重复次数。
        /// </summary>
        /// <value>
        /// 重复正则内部的最小重复次数。
        /// <para>当值为 null 时，与值为 0 时意义相同，但在正则对象合并时的行为不同。在对重复正则进行连接、重复操作时，只要其中一个重复正则的 <see cref="MinimumCount"/> 的值为 null， 则结果的重复正则的 <see cref="MinimumCount"/> 的值为 null 。</para>
        /// </value>
        public virtual ulong? MinimumCount => this.minimumCount;
        /// <summary>
        /// 获取重复正则内部的最大重复次数。
        /// </summary>
        /// <value>
        /// 重复正则内部的最大重复次数。
        /// <para>当值为 null 时，指示重复正则的最大重复次数为无限。</para>
        /// </value>
        public virtual ulong? MaximumCount => this.maximumCount;

        /// <summary>
        /// 获取一个值，指示重复正则的最大重复次数是否为无限。
        /// </summary>
        public virtual bool IsInfinte => !this.maximumCount.HasValue;

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。子类继承的默认构造函数。
        /// </summary>
        protected RegexRepeat() { }

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。该实例指定内部的正则对象。
        /// </summary>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        protected RegexRepeat(RegexObject<T> regex) : this()
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            this.innerRegex = regex;
        }

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。该实例指定内部的正则对象，重复次数的最小值、最大值。
        /// </summary>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="maximumCount">最大重复次数。</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentOutOfRangeException">重复次数最小值 <paramref name="minimumCount"/> 大于重复次数最大值 <paramref name="maximumCount"/> 。</exception>
        public RegexRepeat(RegexObject<T> regex, ulong? minimumCount, ulong? maximumCount) : this(regex)
        {
            if ((minimumCount.HasValue && maximumCount.HasValue) &&
                (minimumCount.Value > maximumCount.Value)
            )
                throw new ArgumentOutOfRangeException(
                    string.Format("{0}, {1}", nameof(minimumCount), nameof(maximumCount)),
                    string.Format("{0}, {1}", minimumCount, maximumCount),
                    string.Format("次数最小值不能大于最大值。")
                );

            this.minimumCount = minimumCount;
            this.maximumCount = maximumCount;
        }

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。该实例指定内部的正则对象，重复次数的最小值、最大值。
        /// </summary>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="maximumCount">最大重复次数。</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentOutOfRangeException">重复次数最小值 <paramref name="minimumCount"/> 大于重复次数最大值 <paramref name="maximumCount"/> 。</exception>
        public RegexRepeat(RegexObject<T> regex, ulong minimumCount, ulong maximumCount) : this(regex, (ulong?)minimumCount, (ulong?)maximumCount) { }

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。该实例指定内部的正则对象，重复次数的最大值是否为无限。
        /// </summary>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <param name="isInfinite">一个值，指示重复次数的最大值是否为无限。</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public RegexRepeat(RegexObject<T> regex, bool isInfinite) : this(regex, null, isInfinite) { }

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。该实例指定内部的正则对象，重复次数的最小值以及重复次数的最大值是否为无限。
        /// </summary>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="isInfinite">一个值，指示重复次数的最大值是否为无限。</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public RegexRepeat(RegexObject<T> regex, ulong? minimumCount, bool isInfinite) : this(regex, minimumCount, (isInfinite ? null : (ulong?)(minimumCount ?? ulong.MinValue))) { }

        /// <summary>
        /// 初始化 <see cref="RegexRepeat{T}"/> 类的新实例。该实例指定内部的正则对象，重复次数的最小值以及重复次数的最大值是否为无限。
        /// </summary>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="isInfinite">一个值，指示重复次数的最大值是否为无限。</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public RegexRepeat(RegexObject<T> regex, ulong minimumCount, bool isInfinite) : this(regex, (ulong?)minimumCount, isInfinite) { }

        /// <summary>
        /// 将此重复正则与另一个正则对象串联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>串联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="RegexObject{T}.Concat(RegexObject{T})"/>
        public override RegexObject<T> Concat(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexRepeat<T> repeat && this.InnerRegex.Equals(repeat.InnerRegex))
                return new RegexRepeat<T>(
                    this.InnerRegex,
                    ((this.MinimumCount.HasValue && repeat.MinimumCount.HasValue) ? this.MinimumCount + repeat.MinimumCount : null),
                    ((this.MaximumCount.HasValue && repeat.MaximumCount.HasValue) ? this.MaximumCount + repeat.MaximumCount : null)
                );
            else return base.Concat(regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexRepeat<T>(this.innerRegex, this.minimumCount, this.maximumCount);
        }

        public override string ToString()
        {
            if (this.IsInfinte && (this.MinimumCount ?? ulong.MinValue) == ulong.MinValue)
                return $"{this.InnerRegex}*";
            else if (this.IsInfinte && this.MinimumCount == 1)
                return $"{this.InnerRegex}+";
            else if ((this.MinimumCount ?? ulong.MinValue) == ulong.MinValue && this.MaximumCount == 1)
                return $"{this.InnerRegex}?";
            else
                return $"{this.InnerRegex}{'{'}{this.MinimumCount ?? ulong.MinValue},{(this.IsInfinte ? string.Empty : this.MaximumCount.Value.ToString())}{'}'}";
        }
    }
}