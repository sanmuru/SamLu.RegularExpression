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

        private ulong? minimumCount = null;
        private ulong? maximumCount = null;

        public ulong? MinimumCount => this.minimumCount;
        public ulong? MaximumCount => this.maximumCount;

        public bool IsInfinte => !this.maximumCount.HasValue;

        protected RegexRepeat(RegexObject<T> regex)
        {
            this.innerRegex = regex;
        }

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

        public RegexRepeat(RegexObject<T> regex, ulong minimumCount, ulong maximumCount) : this(regex, (ulong?)minimumCount, (ulong?)maximumCount) { }

        public RegexRepeat(RegexObject<T> regex, bool isInfinite) : this(regex, null, isInfinite) { }

        public RegexRepeat(RegexObject<T> regex, ulong? minimumCount, bool isInfinite) : this(regex, minimumCount, (isInfinite ? null : (ulong?)(minimumCount ?? ulong.MinValue))) { }

        public RegexRepeat(RegexObject<T> regex, ulong minimumCount, bool isInfinite) : this(regex, (ulong?)minimumCount, isInfinite) { }

        public override RegexObject<T> Concat(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexRepeat<T> repeat && this.innerRegex.Equals(repeat.innerRegex))
                return new RegexRepeat<T>(
                    this.innerRegex,
                    ((this.MinimumCount.HasValue && repeat.MinimumCount.HasValue) ? this.minimumCount + repeat.minimumCount : null),
                    ((this.MaximumCount.HasValue && repeat.MaximumCount.HasValue) ? this.maximumCount + repeat.maximumCount : null)
                );
            else return base.Concat(regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexRepeat<T>(this.innerRegex, this.minimumCount, this.maximumCount);
        }
    }
}