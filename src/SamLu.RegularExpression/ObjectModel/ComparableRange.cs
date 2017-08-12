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
    public class ComparableRange<T> : Range<T>, IEquatable<ComparableRange<T>>
        where T : IComparable<T>
    {
        protected ComparableRange() : base() { }

        public ComparableRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) :
            base(minimum, maximum, canTakeMinimum, canTakeMaximum)
        { }

        public bool Equals(ComparableRange<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return
                (object.Equals(this.Minimum, other.Minimum) &&
                object.Equals(this.Maximum, other.Maximum)) &&
                (this.CanTakeMinimum == other.CanTakeMinimum &&
                this.CanTakeMaximum == other.CanTakeMaximum);
        }
    }
}
