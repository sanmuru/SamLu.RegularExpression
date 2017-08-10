using SamLu.RegularExpression.DebugView;
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
    #region Debugger Support
    [DebuggerDisplay("{__Debugger__CanTakeMinimum,nq}{Minimum},{Maximum}{__Debugger__CanTakeMaximum,nq}")]
    [DebuggerTypeProxy(typeof(RangeDebugView<>))]
    #endregion
    public class RegexRange<T> : RegexCondition<T>, IRange<T>
    {
        #region Debugger Support
        private string __Debugger__CanTakeMinimum => this.CanTakeMinimum ? "[" : "(";
        private string __Debugger__CanTakeMaximum => this.CanTakeMaximum ? "]" : ")";
        #endregion

        /// <summary>
        /// 一个默认的范围正则的值大小比较方法。
        /// </summary>
        public static readonly Comparison<T> DefaultComparison = Comparer<T>.Default.Compare;

        /// <summary>
        /// 内部范围的最小值。
        /// </summary>
        protected T minimum;
        /// <summary>
        /// 内部范围的最大值。
        /// </summary>
        protected T maximum;

        /// <summary>
        /// 一个值，指示内部范围是否取到最小值。
        /// </summary>
        protected bool canTakeMinimum;
        /// <summary>
        /// 一个值，指示内部范围是否取到最大值。
        /// </summary>
        protected bool canTakeMaximum;

        /// <summary>
        /// 正则内部的值大小比较方法。
        /// </summary>
        protected Comparison<T> comparison;

        /// <summary>
        /// 获取内部范围的最小值。
        /// </summary>
        public virtual T Minimum => this.minimum;
        /// <summary>
        /// 获取内部范围的最大值。
        /// </summary>
        public virtual T Maximum => this.maximum;

        /// <summary>
        /// 获取一个值，指示内部范围是否取到最小值。
        /// </summary>
        public bool CanTakeMinimum => this.canTakeMinimum;
        /// <summary>
        /// 获取一个值，指示内部范围是否取到最大值。
        /// </summary>
        public bool CanTakeMaximum => this.canTakeMaximum;

        /// <summary>
        /// 获取范围正则内部的值大小比较方法。
        /// </summary>
        public virtual Comparison<T> Comparison => this.comparison;

        /// <summary>
        /// 初始化 <see cref="RegexRange{T}"/> 类的新实例。子类继承的默认构造函数。
        /// </summary>
        protected RegexRange() : base() { }

        /// <summary>
        /// 初始化 <see cref="RegexRange{T}"/> 类的新实例。该实例指定范围的最小值、最大值，是否能取到最小值、最大值。
        /// </summary>
        /// <param name="minimum">指定的范围的最小值。</param>
        /// <param name="maximum">指定的范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示内部范围是否取到最小值。默认为 true 。</param>
        /// <param name="canTakeMaximum">一个值，指示内部范围是否取到最大值。默认为 true 。</param>
        public RegexRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) : this(minimum, maximum, canTakeMinimum, canTakeMaximum, RegexRange<T>.DefaultComparison) { }

        /// <summary>
        /// 初始化 <see cref="RegexRange{T}"/> 类的新实例。该实例指定范围的最小值、最大值，是否能取到最小值、最大值以及值大小比较方法。
        /// </summary>
        /// <param name="minimum">指定的范围的最小值。</param>
        /// <param name="maximum">指定的范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示内部范围是否取到最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示内部范围是否取到最大值。</param>
        /// <param name="comparison">指定的值大小比较方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> 的值为 null 。</exception>
        public RegexRange(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison) : this()
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));

            if (comparison(minimum, maximum) > 0)
                throw new ArgumentOutOfRangeException(
                    string.Format("{0}, {1}", nameof(minimum), nameof(maximum)),
                    string.Format("{0}, {1}", minimum, maximum),
                    string.Format("范围最小值不能大于最大值。")
                );
            else if ((!this.canTakeMinimum || !this.canTakeMaximum) && comparison(minimum, maximum) == 0)
                throw new ArgumentOutOfRangeException(
                    string.Format("{0}, {1}", nameof(minimum), nameof(maximum)),
                    string.Format("{0}, {1}", minimum, maximum),
                    string.Format("无法构成有效范围。")
                );

            this.minimum = minimum;
            this.maximum = maximum;

            this.canTakeMinimum = canTakeMinimum;
            this.canTakeMaximum = canTakeMaximum;

            this.comparison = comparison;
            base.condition =
                t =>
                    (canTakeMinimum ?
                        comparison(minimum, t) <= 0 :
                        comparison(minimum, t) < 0
                    ) &&
                    (canTakeMaximum ?
                        comparison(t, maximum) <= 0 :
                        comparison(t, maximum) < 0
                    );
        }

        /// <summary>
        /// 将此范围正则与另一个正则对象并联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="RegexObject{T}.Unions(RegexObject{T})"/>
        public override RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexConst<T> regexConst)
                return regex.Unions(this);
            else if (regex is RegexRange<T> range)
            {
                if (this.comparison != range.comparison)
                    throw new NotSupportedException("无法对使用不同比较方法的范围正则进行结合。");
                else if (
                    (this.comparison(range.Minimum, this.Maximum) <= 0 &&
                    range.comparison(range.Minimum, this.Maximum) <= 0)
                )
                {
                    T newMinimum, newMaximum;
                    bool newCanTakeMinimum, newCanTakeMaximum;
                    int i;

                    i = this.comparison(this.Minimum, range.Minimum);
                    if (i < 0)
                    {
                        newMinimum = this.Minimum;
                        newCanTakeMinimum = this.canTakeMinimum;
                    }
                    else if (i > 0)
                    {
                        newMinimum = range.Minimum;
                        newCanTakeMinimum = range.canTakeMinimum;
                    }
                    else
                    {
                        newMinimum = this.Minimum;
                        newCanTakeMinimum = this.canTakeMinimum || range.canTakeMinimum;
                    }

                    i = this.comparison(this.Maximum, range.Maximum);
                    if (i < 0)
                    {
                        newMaximum = this.Maximum;
                        newCanTakeMaximum = this.canTakeMaximum;
                    }
                    else if (i > 0)
                    {
                        newMaximum = range.Maximum;
                        newCanTakeMaximum = range.canTakeMaximum;
                    }
                    else
                    {
                        newMaximum = this.Maximum;
                        newCanTakeMaximum = this.canTakeMaximum || range.canTakeMaximum;
                    }

                    return new RegexRange<T>(
                        newMinimum, newMaximum,
                        newCanTakeMinimum, newCanTakeMaximum,
                        this.comparison
                    );
                }
            }

            return base.Unions(regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexRange<T>(this.minimum, this.maximum, this.canTakeMinimum, this.canTakeMaximum, this.comparison);
        }

        public override string ToString()
        {
            return $"{(this.CanTakeMinimum ? '[' : '(')}{this.Minimum},{this.Maximum}{(this.CanTakeMaximum ? ']' : ')')}";
        }
    }
}