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
    public class ComparableRange<T> : IRange<T>, IEquatable<ComparableRange<T>>
        where T : IComparable<T>
    {
        public static readonly Comparison<T> DefaultComparison = Comparer<T>.Default.Compare;

        private T minimum;
        private T maximum;

        private bool canTakeMinimum;
        private bool canTakeMaximum;

        public T Minimum => this.minimum;
        public T Maximum => this.maximum;

        public bool CanTakeMinimum => this.canTakeMinimum;
        public bool CanTakeMaximum => this.canTakeMaximum;

        protected ComparableRange() { }

        public ComparableRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) : this()
        {
            this.minimum = minimum;
            this.maximum = maximum;

            this.canTakeMinimum = canTakeMinimum;
            this.canTakeMaximum = canTakeMaximum;
        }

        public bool Equals(ComparableRange<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return
                (object.Equals(this.Minimum, other.Minimum) &&
                object.Equals(this.Maximum, other.Maximum)) &&
                (this.CanTakeMinimum == other.CanTakeMinimum &&
                this.CanTakeMaximum == other.CanTakeMaximum);
        }

        #region IRange{T} Implementations
        Comparison<T> IRange<T>.Comparison => ComparableRange<T>.DefaultComparison;
        #endregion
    }
}
