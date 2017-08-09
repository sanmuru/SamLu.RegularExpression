using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.ObjectModel
{
    public class ReadOnlySet<T> : IReadOnlySet<T>, ISet<T>
    {
        private ISet<T> set;

        public ReadOnlySet(ISet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            this.set = set;
        }

        public int Count => this.set.Count;

        public bool Contains(T item) => this.set.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => this.set.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => this.set.GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<T> other) => this.set.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => this.set.IsProperSubsetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => this.set.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => this.set.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => this.set.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => this.set.SetEquals(other);

        #region ISet{T} Implementations
        bool ICollection<T>.IsReadOnly => this.set.IsReadOnly;

        bool ISet<T>.Add(T item) => throw new NotSupportedException();

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        void ICollection<T>.Clear() => throw new NotSupportedException();

        void ISet<T>.ExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => this.set.GetEnumerator();

        void ISet<T>.IntersectWith(IEnumerable<T> other) => throw new NotSupportedException();

        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

        void ISet<T>.UnionWith(IEnumerable<T> other) => throw new NotSupportedException();
        #endregion
    }
}
