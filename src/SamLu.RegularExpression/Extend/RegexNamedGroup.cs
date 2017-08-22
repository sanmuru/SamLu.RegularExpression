using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexNamedGroup<T> : RegexGroup<T>
    {
        protected RegexNamedGroup(RegexObject<T> regex, string name = null, bool isCaptive = true) : base(regex, name, isCaptive) { }
    }
}
