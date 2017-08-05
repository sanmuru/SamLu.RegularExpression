using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public class RegexRepeat<T> : RegexObject<T>
    {
        private RegexObject<T> innerRegex;
        public RegexObject<T> InnerRegex => this.innerRegex;

        private ulong? minimum = null;
        private ulong? maximum = null;

        public ulong? Minimum => this.minimum;
        public ulong? Maximum => this.maximum;

        public bool IsInfinte => !this.maximum.HasValue;

        protected RegexRepeat(RegexObject<T> regex)
        {
            this.innerRegex = regex;
        }

        public RegexRepeat(RegexObject<T> regex, ulong? minimum, ulong? maximum) : this(regex)
        {
            if ((minimum.HasValue && maximum.HasValue) &&
                (minimum.Value > maximum.Value)
            )
                throw new ArgumentOutOfRangeException(
                    string.Format("{0}, {1}", nameof(minimum), nameof(maximum)),
                    string.Format("{0}, {1}", minimum, maximum),
                    string.Format("次数最小值不能大于最大值。")
                );

            this.minimum = minimum;
            this.maximum = maximum;
        }

        public RegexRepeat(RegexObject<T> regex, ulong minimum, ulong maximum) : this(regex, (ulong?)minimum, (ulong?)maximum) { }

        public RegexRepeat(RegexObject<T> regex, bool isInfinite) : this(regex, null, isInfinite) { }

        public RegexRepeat(RegexObject<T> regex, ulong? minimum, bool isInfinite) : this(regex, minimum, null) { }

        public RegexRepeat(RegexObject<T> regex, ulong minimum, bool isInfinite) : this(regex, (ulong?)minimum, isInfinite) { }

        public override RegexObject<T> Concat(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexRepeat<T> repeat && this.innerRegex.Equals(repeat.innerRegex))
                return new RegexRepeat<T>(
                    this.innerRegex,
                    ((this.Minimum.HasValue && repeat.Minimum.HasValue) ? this.minimum + repeat.minimum : null),
                    ((this.Maximum.HasValue && repeat.Maximum.HasValue) ? this.maximum + repeat.maximum : null)
                );
            else return base.Concat(regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexRepeat<T>(this.innerRegex, this.minimum, this.maximum);
        }
    }
}