using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示范围正则。匹配单个对象是否落在内部范围之间。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexRange<T> : RegexObject<T>
    {
        /// <summary>
        /// 内部范围的最小值。
        /// </summary>
        protected T? minimum;
        /// <summary>
        /// 内部范围的最大值。
        /// </summary>
        protected T? maximum;

        /// <summary>
        /// 一个值，指示内部范围是否取到最小值。
        /// </summary>
        protected bool canTakeMinimum;
        /// <summary>
        /// 一个值，指示内部范围是否取到最大值。
        /// </summary>
        protected bool canTakeMaximum;

        /// <summary>
        /// 获取内部范围的最小值。
        /// </summary>
        public virtual T? Minimum => this.minimum;
        /// <summary>
        /// 获取内部范围的最大值。
        /// </summary>
        public virtual T? Maximum => this.maximum;

        /// <summary>
        /// 获取一个值，指示内部范围是否取到最小值。
        /// </summary>
        public bool CanTakeMinimum => this.canTakeMinimum;
        /// <summary>
        /// 获取一个值，指示内部范围是否取到最大值。
        /// </summary>
        public bool CanTakeMaximum => this.canTakeMaximum;

        /// <summary>
        /// 初始化 <see cref="RegexRange{T}"/> 类的新实例。该实例指定范围的最小值、最大值，是否能取到最小值、最大值以及值大小比较方法。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <param name="minimum">指定的范围的最小值。</param>
        /// <param name="maximum">指定的范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示内部范围是否取到最小值。默认值为 <see langword="true"/> 。</param>
        /// <param name="canTakeMaximum">一个值，指示内部范围是否取到最大值。默认值为 <see langword="true"/> 。</param>
        public RegexRange(RegexProvider<T> provider, T? minimum, T? maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) : base(provider)
        {
            this.minimum = minimum;
            this.maximum = maximum;

            this.canTakeMinimum = canTakeMinimum;
            this.canTakeMaximum = canTakeMaximum;
        }

        /// <inheritdoc/>
        protected internal override RegexObject<T> Clone() => this.provider.CreateRegexObject(this, minimum, maximum, canTakeMaximum, canTakeMaximum);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int minHashCode = this.Minimum?.GetHashCode() ?? 0;
            if (!this.CanTakeMinimum) minHashCode = ~minHashCode;
            int maxHashCode = this.Maximum?.GetHashCode() ?? 0;
            if (!this.CanTakeMaximum) maxHashCode = ~maxHashCode;

            return minHashCode ^ maxHashCode;
        }

        /// <summary>
        /// 获取范围正则对象的字符串表示。
        /// </summary>
        /// <returns>范围正则对象的字符串表示。</returns>
        public override string? ToString() => $"{(this.CanTakeMinimum ? '[' : '(')}{this.Minimum},{this.Maximum}{(this.CanTakeMaximum ? ']' : ')')}";
    }
}