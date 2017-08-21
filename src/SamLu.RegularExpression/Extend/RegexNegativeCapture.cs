using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexNegativeCaptureGroup<T> : RegexObject<T>
    {
        private RegexGroup<T> innerGroup;
        public RegexGroup<T> Group => this.innerGroup;

        public RegexNegativeCaptureGroup(RegexGroup<T> group) =>
            this.innerGroup = group ?? throw new ArgumentNullException(nameof(group));

        protected internal override RegexObject<T> Clone()
        {
            return new RegexNegativeCaptureGroup<T>(this.innerGroup);
        }
    }
}
