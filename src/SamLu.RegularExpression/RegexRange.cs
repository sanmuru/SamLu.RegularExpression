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
    /// <typeparam name="T"></typeparam>
    #region Debugger Support
    [DebuggerDisplay("{__Debugger__CanTakeMinimum,nq}{Minimum},{Maximum}{__Debugger__CanTakeMaximum,nq}")]
    [DebuggerTypeProxy(typeof(RegexRangeDebugView<>))]
    #endregion
    public class RegexRange<T> : RegexCondition<T>
    {
        #region Debugger Support
        private string __Debugger__CanTakeMinimum => this.CanTakeMinimum ? "[" : "(";
        private string __Debugger__CanTakeMaximum => this.CanTakeMaximum ? "]" : ")";
        #endregion

        /// <summary>
        /// 一个默认的范围正则的值大小比较方法。
        /// </summary>
        public static readonly Comparison<T> DefaultComparison = Comparer<T>.Default.Compare;

        protected T minimum;
        protected T maximum;

        protected bool canTakeMinimum;
        protected bool canTakeMaximum;

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

        public virtual Comparison<T> Comparison => this.comparison;

        protected RegexRange() : base() { }

        public RegexRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) : this(minimum, maximum, canTakeMinimum, canTakeMaximum, RegexRange<T>.DefaultComparison) { }

        public RegexRange(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison) : base()
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