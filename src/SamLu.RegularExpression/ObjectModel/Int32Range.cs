using SamLu.RegularExpression.DebugView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    [DebuggerTypeProxy(typeof(RangeDebugView<>))]
    public class Int32Range : ComparableRange<int>
    {
        public Int32Range() : this(int.MinValue, int.MaxValue) { }
        
        public Int32Range(int minValue, int maxValue, bool canTakeMinValue = true, bool canTakeMaxValue = true) : base(minValue, maxValue, canTakeMinValue, canTakeMaxValue) { }
    }
}
