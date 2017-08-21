using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexUncaptiveGroup<T> : RegexGroup<T>
    {
        public sealed override bool IsCaptive => false;

        public RegexUncaptiveGroup(RegexObject<T> regex) : base(regex, null, false) { }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexUncaptiveGroup<T>(base.innerRegex);
        }
    }
}
