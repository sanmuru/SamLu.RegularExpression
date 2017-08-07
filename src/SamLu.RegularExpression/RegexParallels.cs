using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public class RegexParallels<T> : RegexObject<T>
    {
        protected List<RegexObject<T>> parallels;
        public ICollection<RegexObject<T>> Parallels => new ReadOnlyCollection<RegexObject<T>>(this.parallels);

        public RegexParallels(params RegexObject<T>[] parallels) : this(parallels?.AsEnumerable()) { }

        public RegexParallels(IEnumerable<RegexObject<T>> parallels)
        {
            if (parallels == null) throw new ArgumentNullException(nameof(parallels));

            this.parallels = new List<RegexObject<T>>(parallels.Where(item => item != null));
        }

        public override RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexParallels<T> parallels)
                return new RegexParallels<T>(this.parallels.Concat(parallels.parallels));
            else
                return new RegexParallels<T>(this, regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexParallels<T>(this.parallels);
        }

        public override string ToString()
        {
            return $"({string.Join("|", this.parallels)})";
        }
    }
}