using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class CharRange : ComparableRange<char>
    {
        public CharRange() : this(char.MinValue, char.MaxValue) { }

        public CharRange(char minValue, char maxValue, bool canTakeMinValue = true, bool canTakeMaxValue = true) : base(minValue, maxValue, canTakeMinValue, canTakeMaxValue) { }
    }
}
