using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    [DebuggerDisplay("Count = {Count}")]
    public class CharRangeSet : RangeSet<char>, IReadOnlySet<char>
    {
        public CharRangeSet() : base(new CharRangeInfo()) { }
    }
}
