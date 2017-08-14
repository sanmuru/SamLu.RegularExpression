using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class CharRangeInfo : RangeInfo<char>
    {
        public CharRangeInfo() : base() { }
        
        protected CharRangeInfo(Comparison<char> comparison) : base(comparison) { }

        protected override IRange<char> CreateInternal(char minimum, char maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            new CharRange(minimum, maximum, canTakeMinimum, canTakeMaximum);

        public override char GetPrev(char value)
        {
            if (value == char.MinValue) throw new InvalidOperationException();

            return (char)(value - 1);
        }

        public override char GetNext(char value)
        {
            if (value == char.MaxValue) throw new InvalidOperationException();

            return (char)(value + 1);
        }
    }
}
