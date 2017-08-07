using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    #region Debugger Support
    [DebuggerDisplay("{InnerRegex}{__DebugString,nq}")]
    #endregion
    public class RegexRepeat<T> : RegexObject<T>
    {
        #region Debugger Support
        private string __DebugString =>
            $"{'{'}{this.MinimumCount ?? ulong.MinValue},{(this.IsInfinte ? string.Empty : this.MaximumCount.Value.ToString())}{'}'}";
        #endregion

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
            return $"{this.InnerRegex}{'{'}{this.MinimumCount ?? ulong.MinValue},{(this.IsInfinte ? string.Empty : this.MaximumCount.Value.ToString())}{'}'}";
        }
    }
}