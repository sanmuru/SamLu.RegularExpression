using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexZeroLengthObject<T> : RegexObject<T>, IRegexAnchorPoint<T>
    {
        private RegexObject<T> innerRegex;
        public RegexObject<T> InnerRegex => this.innerRegex;

        public RegexZeroLengthObject(RegexObject<T> regex)=>
            this.innerRegex=regex??throw new ArgumentNullException(nameof(regex));

        protected internal override RegexObject<T> Clone()
        {
            return new RegexZeroLengthObject<T>(this.innerRegex);
        }
    }
}
