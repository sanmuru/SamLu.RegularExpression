using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public class RegexSeries<T> : RegexObject<T>
    {
        protected List<RegexObject<T>> series;
        public ICollection<RegexObject<T>> Series => new ReadOnlyCollection<RegexObject<T>>(this.series);

        public RegexSeries(params RegexObject<T>[] series) : this(series?.AsEnumerable()) { }

        public RegexSeries(IEnumerable<RegexObject<T>> series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));

            this.series = new List<RegexObject<T>>(series.Where(item => item != null));
        }

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
            return string.Join(string.Empty, this.series);
        }
    }
}