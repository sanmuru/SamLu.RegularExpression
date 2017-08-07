using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public class RegexRange<T> : RegexCondition<T>
    {
        /// <summary>
        /// 一个默认的范围正则的值大小比较方法。
        /// </summary>
        public static readonly Comparison<T> DefaultComparison = Comparer<T>.Default.Compare;

        private T minimum;
        private T maximum;

        private bool canTakeMinimum;
        private bool canTakeMaximum;

        private Comparison<T> comparison;

        public T Minimum => this.minimum;
        public T Maximum => this.maximum;

        public bool CanTakeMinimum => this.canTakeMinimum;
        public bool CanTakeMaximum => this.canTakeMaximum;

        internal Comparison<T> Comparison => this.comparison;

        public RegexRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) : this(minimum, maximum, canTakeMinimum, canTakeMaximum, RegexRange<T>.DefaultComparison) { }

        protected RegexRange(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison) :
            base(
                comparison == null ?
                    null :
                    new Predicate<T>(t =>
                        (canTakeMinimum ?
                            comparison(minimum, t) <= 0 :
                            comparison(minimum, t) < 0
                        ) &&
                        (canTakeMaximum ?
                            comparison(t, maximum) <= 0 :
                            comparison(t, maximum) < 0)
                    )
            )
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
                    (this.comparison(range.minimum, this.maximum) <= 0 &&
                    range.comparison(range.minimum, this.maximum) <= 0)
                )
                {
                    T newMinimum, newMaximum;
                    bool newCanTakeMinimum, newCanTakeMaximum;
                    int i;

                    i = this.comparison(this.minimum, range.minimum);
                    if (i < 0)
                    {
                        newMinimum = this.minimum;
                        newCanTakeMinimum = this.canTakeMinimum;
                    }
                    else if (i > 0)
                    {
                        newMinimum = range.minimum;
                        newCanTakeMinimum = range.canTakeMinimum;
                    }
                    else
                    {
                        newMinimum = this.minimum;
                        newCanTakeMinimum = this.canTakeMinimum || range.canTakeMinimum;
                    }

                    i = this.comparison(this.maximum, range.maximum);
                    if (i < 0)
                    {
                        newMaximum = this.maximum;
                        newCanTakeMaximum = this.canTakeMaximum;
                    }
                    else if (i > 0)
                    {
                        newMaximum = range.maximum;
                        newCanTakeMaximum = range.canTakeMaximum;
                    }
                    else
                    {
                        newMaximum = this.maximum;
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
            return new RegexRange<T>(this.minimum, this.maximum, this.canTakeMinimum, this.CanTakeMaximum, this.comparison);
        }
    }
}