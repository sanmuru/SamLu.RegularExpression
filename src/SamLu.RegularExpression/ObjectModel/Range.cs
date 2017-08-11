using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class Range<T> : IRange<T>
    {
        public static readonly Comparison<T> DefaultComparison = Comparer<T>.Default.Compare;

        protected T minimum;
        protected T maximum;

        protected bool canTakeMinimum;
        protected bool canTakeMaximum;

        protected Comparison<T> comparison;

        public T Minimum => this.minimum;
        public T Maximum => this.maximum;

        public bool CanTakeMinimum => this.canTakeMinimum;
        public bool CanTakeMaximum => this.canTakeMaximum;

        public Comparison<T> Comparison => this.comparison;

        protected Range() { }

        public Range(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) :
            this(minimum, maximum, canTakeMinimum, canTakeMaximum, Range<T>.DefaultComparison)
        { }

        public Range(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison) : this()
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));

            this.minimum = minimum;
            this.maximum = maximum;

            this.canTakeMinimum = canTakeMinimum;
            this.canTakeMaximum = canTakeMaximum;

            this.comparison = comparison;
        }
    }
}
