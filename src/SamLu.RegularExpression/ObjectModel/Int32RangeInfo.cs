using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class Int32RangeInfo : RangeInfo<int>
    {
        public Int32RangeInfo() : base() { }

        protected Int32RangeInfo(Comparison<int> comparison) : base(comparison) { }

        protected override IRange<int> CreateInternal(int minimum, int maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            new Int32Range(minimum, maximum, canTakeMinimum, canTakeMaximum, base.comparison);

        public override int GetPrev(int value)
        {
            if (value == int.MinValue) throw new InvalidOperationException();

            return value - 1;
        }

        public override int GetNext(int value)
        {
            if (value == int.MaxValue) throw new InvalidOperationException();

            return value + 1;
        }
    }
}
