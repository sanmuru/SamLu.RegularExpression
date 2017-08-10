using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    [DebuggerDisplay("Count = {Count}")]
    public class CharRangeSet : RangeSet<char>
    {
        public CharRangeSet() : base(new CharRange()) { }

        public CharRangeSet(char minValue, char maxValue, bool canTakeMinValue = true, bool canTakeMaxValue = true) : base(new CharRange(minValue, maxValue, canTakeMinValue, canTakeMaxValue)) { }

        public override int Count =>
            (this.Maximum - this.Minimum + 1) -
                ((this.CanTakeMinimum ? 0 : 1) + (this.CanTakeMaximum ? 0 : 1));

        public override bool IsReadOnly => true;

        public override void Clear() => throw new NotSupportedException();

        public override void CopyTo(char[] array, int arrayIndex)
        {
            this.ToList().CopyTo(array, arrayIndex);
        }

        public override IEnumerator<char> GetEnumerator()
        {
            if (this.Count == 0) yield break;
            else
            {
                char minValue = this.CanTakeMinimum ? this.Minimum : (char)(this.Minimum + 1);
                char maxValue = this.CanTakeMaximum ? this.Maximum : (char)(this.Maximum - 1);
                for (char c = minValue; c < maxValue; c++) yield return c;
            }

            yield break;
        }

        public override bool Add(char item) => throw new NotSupportedException();

        public override void ExceptWith(IEnumerable<char> other) => throw new NotSupportedException();

        public override void ExceptWith(IRange<char> range) => throw new NotSupportedException();

        public override void IntersectWith(IEnumerable<char> other) => throw new NotSupportedException();

        public override bool IntersectWith(IRange<char> range) => throw new NotSupportedException();

        public override bool IsProperSubsetOf(IEnumerable<char> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return false;

            return (other is IRange<char> range) && this.IsProperSubsetOf(range);
        }

        public override bool IsProperSubsetOf(IRange<char> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return false;

            bool minimumTest = false;
            bool maximumTest = false;

            if (this.Minimum < range.Minimum) return false;
            else if (this.Minimum == range.Minimum)
            {
                if (!this.CanTakeMinimum && range.CanTakeMinimum) return false;
                else if (this.CanTakeMinimum && !range.CanTakeMinimum) minimumTest = true;
            }
            else if (this.Minimum + 1 == range.Minimum)
            {
                if (this.CanTakeMinimum || !range.CanTakeMinimum) minimumTest = true;
            }
            else minimumTest = true;

            if (range.Maximum < range.Maximum) return false;
            else if (range.Maximum == range.Maximum)
            {
                if (!this.CanTakeMaximum && range.CanTakeMaximum) return false;
                else if (this.CanTakeMaximum && !range.CanTakeMaximum) maximumTest = true;
            }
            else if (range.Maximum + 1 == range.Maximum)
            {
                if (this.CanTakeMaximum || !range.CanTakeMaximum) maximumTest = true;
            }
            else maximumTest = true;

            return minimumTest && maximumTest;
        }

        public override bool IsProperSupersetOf(IEnumerable<char> other)
        {
            return (other is IRange<char> range) && this.IsProperSupersetOf(range);
        }

        public override bool IsProperSupersetOf(IRange<char> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return false;

            bool minimumTest = false;
            bool maximumTest = false;

            if (range.Minimum < this.Minimum) return false;
            else if (range.Minimum == this.Minimum)
            {
                if (!range.CanTakeMinimum && this.CanTakeMinimum) return false;
                else if (range.CanTakeMinimum && !this.CanTakeMinimum) minimumTest = true;
            }
            else if (range.Minimum + 1 == this.Minimum)
            {
                if (range.CanTakeMinimum || !this.CanTakeMinimum) minimumTest = true;
            }
            else minimumTest = true;

            if (range.Maximum < this.Maximum) return false;
            else if (range.Maximum == this.Maximum)
            {
                if (!range.CanTakeMaximum && this.CanTakeMaximum) return false;
                else if (range.CanTakeMaximum && !this.CanTakeMaximum) maximumTest = true;
            }
            else if (range.Maximum + 1 == this.Maximum)
            {
                if (range.CanTakeMaximum || !this.CanTakeMaximum) maximumTest = true;
            }
            else maximumTest = true;

            return minimumTest && maximumTest;
        }

        public override bool IsSubsetOf(IEnumerable<char> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return (other is IRange<char> range) && this.IsSubsetOf(range);
        }

        public override bool IsSubsetOf(IRange<char> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return false;

            bool minimumTest = false;
            bool maximumTest = false;

            if (this.Minimum < range.Minimum) return false;
            else if (this.Minimum == range.Minimum)
            {
                if (!this.CanTakeMinimum && range.CanTakeMinimum) return false;
                else minimumTest = true;
            }
            else minimumTest = true;

            if (this.Maximum < range.Maximum) return false;
            else if (this.Maximum == range.Maximum)
            {
                if (!this.CanTakeMaximum && range.CanTakeMaximum) return false;
                else maximumTest = true;
            }
            else maximumTest = true;

            return minimumTest && maximumTest;
        }

        public override bool IsSupersetOf(IEnumerable<char> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return (other is IRange<char> range) && this.IsSupersetOf(range);
        }

        public override bool IsSupersetOf(IRange<char> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return false;

            bool minimumTest = false;
            bool maximumTest = false;

            if (range.Minimum < this.Minimum) return false;
            else if (range.Minimum == this.Minimum)
            {
                if (!range.CanTakeMinimum && this.CanTakeMinimum) return false;
                else minimumTest = true;
            }
            else minimumTest = true;

            if (range.Maximum < this.Maximum) return false;
            else if (range.Maximum == this.Maximum)
            {
                if (!range.CanTakeMaximum && this.CanTakeMaximum) return false;
                else maximumTest = true;
            }
            else maximumTest = true;

            return minimumTest && maximumTest;
        }

        public override bool Overlaps(IEnumerable<char> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return (other is IRange<char> range) && this.Overlaps(range);
        }

        public override bool Overlaps(IRange<char> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return false;

            if (this.Maximum < range.Minimum || this.Minimum > range.Maximum) return false;

            if (this.Maximum == range.Minimum)
            {
                if (this.CanTakeMaximum && range.CanTakeMinimum) return true;
            }
            else if (this.Maximum - range.Minimum == 1)
            {
                if (this.CanTakeMaximum || range.CanTakeMinimum) return true;
            }
            else return true;

            if (range.Maximum == this.Minimum)
            {
                if (this.CanTakeMaximum && range.CanTakeMinimum) return true;
            }
            else if (range.Minimum - this.Maximum == 1)
            {
                if (this.CanTakeMaximum || range.CanTakeMinimum) return true;
            }
            else return true;

            return false;
        }

        public override bool SetEquals(IEnumerable<char> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return (other is IRange<char> range) && this.SetEquals(range);
        }

        public override bool SetEquals(IRange<char> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return true;

            if (this.CanTakeMinimum == range.CanTakeMinimum)
            {
                if (this.Minimum != range.Minimum) return false;
            }
            else if (this.CanTakeMinimum)
            {
                if (this.Minimum - range.Minimum != 1) return false;
            }
            else if (range.CanTakeMinimum)
            {
                if (range.Minimum - this.Minimum != 1) return false;
            }
            
            if (this.CanTakeMaximum == range.CanTakeMaximum)
            {
                if (this.Maximum != range.Maximum) return false;
            }
            else if (this.CanTakeMaximum)
            {
                if (this.Maximum - range.Maximum != 1) return false;
            }
            else if (range.CanTakeMaximum)
            {
                if (range.Maximum - this.Maximum != 1) return false;
            }

            return true;
        }

        public override void SymmetricExceptWith(IEnumerable<char> other) => throw new NotSupportedException();

        public override bool SymmetricExceptWith(IRange<char> range) => throw new NotSupportedException();

        public override void UnionWith(IEnumerable<char> other) => throw new NotSupportedException();

        public override bool UnionWith(IRange<char> range) => throw new NotSupportedException();

        protected override bool AddOutOfRange(char item) => throw new NotSupportedException();

        protected override bool RemoveInRange(char item) => throw new NotSupportedException();
    }
}
