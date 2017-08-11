using SamLu.RegularExpression.DebugView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    [DebuggerTypeProxy(typeof(RangeDebugView<>))]
    public abstract class RangeSet<T> : ISet<T>
    {
        protected IRange<T> range;
        
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        protected RangeSet() { }

        protected RangeSet(IRange<T> range) : this() =>
            this.range = range ?? throw new ArgumentNullException(nameof(range));

        public virtual bool Add(T item)
        {
            if (this.Contains(item)) return false;
            else return this.AddOutOfRange(item);
        }

        protected abstract bool AddOutOfRange(T item);

        public abstract void Clear();

        public virtual bool Contains(T item) =>
            (this.range.CanTakeMinimum ?
                this.range.Comparison(this.range.Minimum, item) <= 0 :
                this.range.Comparison(this.range.Minimum, item) < 0
            ) &&
            (this.range.CanTakeMaximum ?
                this.range.Comparison(item, this.range.Maximum) <= 0 :
                this.range.Comparison(item, this.range.Maximum) < 0
            );

        public abstract void CopyTo(T[] array, int arrayIndex);

        public virtual void ExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) this.Clear();

            foreach (var item in other)
                this.Remove(item);
        }

        public virtual void ExceptWith(IRange<T> range)
        {
            if (this.range.Comparison == range.Comparison)
            {
#warning 未完成。
            }
            else throw new NotSupportedException();
        }

        public abstract IEnumerator<T> GetEnumerator();

        public abstract void IntersectWith(IEnumerable<T> other);

        public abstract bool IntersectWith(IRange<T> range);

        public abstract bool IsProperSubsetOf(IEnumerable<T> other);

        public abstract bool IsProperSubsetOf(IRange<T> range);

        public abstract bool IsProperSupersetOf(IEnumerable<T> other);

        public abstract bool IsProperSupersetOf(IRange<T> range);

        public abstract bool IsSubsetOf(IEnumerable<T> other);

        public abstract bool IsSubsetOf(IRange<T> range);

        public abstract bool IsSupersetOf(IEnumerable<T> other);

        public abstract bool IsSupersetOf(IRange<T> range);

        public abstract bool Overlaps(IEnumerable<T> other);

        public abstract bool Overlaps(IRange<T> range);

        public virtual bool Remove(T item)
        {
            if (this.Contains(item))
                return this.RemoveInRange(item);
            else return false;
        }

        protected abstract bool RemoveInRange(T item);

        public abstract bool SetEquals(IEnumerable<T> other);

        public virtual bool SetEquals(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (range == this) return true;

            if (this.range.Equals(range)) return true;
            else return false;
        }

        public abstract void SymmetricExceptWith(IEnumerable<T> other);

        public abstract bool SymmetricExceptWith(IRange<T> range);

        public abstract void UnionWith(IEnumerable<T> other);

        public abstract bool UnionWith(IRange<T> range);

        #region ISet{T} Implementations
        void ICollection<T>.Add(T item) => this.Add(item);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
