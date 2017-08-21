using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexMultiBranch<T> : RegexObject<T>
    {
        protected Dictionary<RegexObject<T>, RegexObject<T>> branches;
        protected RegexObject<T> otherwisePattern;

        public IDictionary<RegexObject<T>, RegexObject<T>> Branches => new ReadOnlyDictionary<RegexObject<T>, RegexObject<T>>(this.branches);
        public RegexObject<T> OtherwisePattern => this.otherwisePattern;

        public RegexMultiBranch(RegexObject<T> otherwisePattern) : this(Enumerable.Empty<(RegexObject<T> condition, RegexObject<T> pattern)>(), otherwisePattern) { }

        public RegexMultiBranch(IDictionary<RegexObject<T>,RegexObject<T>> branches, RegexObject<T> otherwisePattern)
        {
            if (branches == null) throw new ArgumentNullException(nameof(branches));
            if (otherwisePattern == null) throw new ArgumentNullException(nameof(otherwisePattern));

            this.branches = new Dictionary<RegexObject<T>, RegexObject<T>>(branches);
            this.otherwisePattern = otherwisePattern;
        }

        public RegexMultiBranch(IEnumerable<(RegexObject<T> condition, RegexObject<T> pattern)> branches, RegexObject<T> otherwisePattern) :
            this(
                (branches ?? throw new ArgumentNullException(nameof(branches)))
                    .ToDictionary(
                        (branch => branch.condition),
                        (branch => branch.pattern)
                    ),
                otherwisePattern
            )
        { }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexMultiBranch<T>(this.branches, this.otherwisePattern);
        }
    }
}
