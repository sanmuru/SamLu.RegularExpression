using SamLu.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    public class CharRangeDebugInfo : RangeDebugInfo<char>
    {
        public override string DebugInfo
        {
            get
            {
                if (this.range.Comparison(this.range.Minimum, this.range.Maximum) == 0 && (this.range.CanTakeMinimum && this.range.CanTakeMaximum))
                    return this.range.Minimum.GetDebugInfo();
                else
                    return $"{(this.range.CanTakeMinimum ? '[' : '(')}{this.range.Minimum.GetDebugInfo()},{this.range.Maximum.GetDebugInfo()}{(this.range.CanTakeMaximum ? ']' : ')')}";
            }
        }

        public CharRangeDebugInfo(IRange<char> range, params object[] args) : base(range, args) { }
    }
}
