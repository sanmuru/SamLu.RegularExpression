using SamLu.Diagnostics;
using SamLu.RegularExpression.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    public class RangeSetDebugView<T> : IDebugInfo
    {
        private RangeSet<T> rangeSet;

        public string DebugInfo =>
            string.Join("∪", this.rangeSet.Ranges.Select(range => range.GetDebugInfo()));

        public RangeSetDebugView(RangeSet<T> rangeSet, params object[] args) =>
            this.rangeSet = rangeSet ?? throw new ArgumentNullException(nameof(rangeSet));
    }
}
