using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public abstract class RegexObject<T> : IEquatable<RegexObject<T>>, ICloneable
    {
        public virtual RegexObject<T> Concat(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexRepeat<T> repeat && this.Equals(repeat.InnerRegex))
                return new RegexRepeat<T>(
                    this,
                    repeat.Minimum.HasValue ? repeat.Minimum + 1 : null,
                    repeat.Maximum.HasValue ? repeat.Maximum + 1 : null
                );
            else if (regex is RegexSeries<T> series)
                return new RegexSeries<T>(new RegexObject<T>[] { this }.Concat(series.Series));
            else
                return new RegexSeries<T>(this, regex);
        }

        public virtual RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexParallels<T> parallels)
                return new RegexParallels<T>(new RegexObject<T>[] { this }.Concat(parallels.Parallels));
            else
                return new RegexParallels<T>(this, regex);
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj is RegexObject<T> regex) && this.Equals(regex);
        }

        public virtual bool Equals(RegexObject<T> regex)
        {
            if (regex == null) return false;
            else return object.ReferenceEquals(this, regex);
        }

        public static RegexObject<T> operator +(RegexObject<T> left, RegexObject<T> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            return left.Concat(right);
        }

        public static RegexObject<T> operator |(RegexObject<T> left, RegexObject<T> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            return left.Unions(right);
        }

        protected internal abstract RegexObject<T> Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}