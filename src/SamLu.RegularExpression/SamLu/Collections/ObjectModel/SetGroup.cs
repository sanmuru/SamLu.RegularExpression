using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.Collections.ObjectModel
{
    public class SetGroup<T> : ISet<T>, IList<ISet<T>>
    {
        public static readonly Predicate<IEnumerable<bool>> UnionGroupPredicate = (predications => predications.Any(predication => predication));
        public static readonly Predicate<IEnumerable<bool>> IntersectGroupPredicate = (predications => predications.All(predication => predication));
        public static readonly Predicate<IEnumerable<bool>> ExceptGroupPredicate = (predications => predications.Count() <= 1 || (predications.First() && predications.All(predication => !predication)));
        public static readonly Predicate<IEnumerable<bool>> SymmetricExceptGroupPredicate = (predications => predications.Count() <= 1 || (predications.First() && predications.All(predication => predication ^ predications.First())));

        protected IList<ISet<T>> setList;
        protected ISet<T> unionSet;
        protected ISet<T> exceptSet;
        protected Predicate<IEnumerable<bool>> groupPredicate;
        protected IEqualityComparer<T> comparer;

        /// <summary>
        /// 获取用于确定集中的值是否相等的 <see cref="IEqualityComparer{T}"/> 对象。
        /// </summary>
        /// <value>
        /// 用于确定集中的值是否相等的 <see cref="IEqualityComparer{T}"/> 对象。
        /// </value>
        public IEqualityComparer<T> Comparer => this.comparer;

        public SetGroup(Predicate<IEnumerable<bool>> groupPredicate) : this(groupPredicate, EqualityComparer<T>.Default) { }

        public SetGroup(Predicate<IEnumerable<bool>> groupPredicate, IEqualityComparer<T> comparer) : this(Enumerable.Empty<ISet<T>>(), groupPredicate, comparer) { }

        public SetGroup(IEnumerable<ISet<T>> sets, Predicate<IEnumerable<bool>> groupPredicate) : this(sets, groupPredicate, EqualityComparer<T>.Default) { }

        public SetGroup(IEnumerable<ISet<T>> sets, Predicate<IEnumerable<bool>> groupPredicate, IEqualityComparer<T> comparer)
        {
            if (sets == null) throw new ArgumentNullException(nameof(sets));
            if (groupPredicate == null) throw new ArgumentNullException(nameof(groupPredicate));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            this.setList = new List<ISet<T>>(sets);
            this.unionSet = new HashSet<T>(comparer);
            this.exceptSet = new HashSet<T>(comparer);
            this.groupPredicate = groupPredicate;
            this.comparer = comparer;
        }

        public bool Add(T item)
        {
            if (this.Contains(item)) return false;
            else
            {
                if (this.exceptSet.Contains(item))
                    return this.exceptSet.Remove(item);
                else
                    return this.unionSet.Add(item);
            }
        }

        public void Add(ISet<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.setList.Add(item);
        }

        public void Clear()
        {
            this.setList.Clear();
            this.unionSet.Clear();
            this.exceptSet.Clear();
        }

        public bool Contains(T item)
        {
            return (this.unionSet.Contains(item) && !this.exceptSet.Contains(item)) || this.groupPredicate(this.setList.Select(set => set.Contains(item)));
        }

        public bool Remove(T item)
        {
            if (!this.Contains(item)) return false;
            else
            {
                if (this.unionSet.Contains(item))
                    return this.unionSet.Remove(item);
                else
                    return this.exceptSet.Add(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            foreach (var item in other.Where(item => this.Contains(item)))
                this.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> setListResult;
            if (this.setList.Count == 0)
                setListResult = Enumerable.Empty<T>();
            else if (this.setList.Count == 1)
                setListResult = this.setList[0];
            else
                setListResult = new HashSet<T>(this.setList.SelectMany(set => set)).Where(t => this.Contains(t));

            return this.unionSet.Except(this.exceptSet, this.Comparer).Concat(setListResult).GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            foreach (var item in ((IEnumerable<T>)this).Except(other.Where(item => this.Contains(other))))
                this.Remove(item);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            foreach (var item in other)
                if (this.Contains(item))
                    this.Remove(item);
                else
                    this.Add(item);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            foreach (var item in other.Where(item => !this.Contains(item)))
                this.Add(item);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) => this.IsProperSubsetOf(other, new HashSet<T>(((IEnumerable<T>)this).Union(other)));

        public bool IsProperSubsetOf(IEnumerable<T> other, ISet<T> accreditedSet)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            var enumerable = accreditedSet
                .Where(item => this.Contains(item) ^ other.Contains(item)).ToArray();
            return enumerable.Any() && enumerable.All(item => other.Contains(item) && !this.Contains(item));
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) => this.IsProperSupersetOf(other, new HashSet<T>(((IEnumerable<T>)this).Union(other)));

        public bool IsProperSupersetOf(IEnumerable<T> other, ISet<T> accreditedSet)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            var enumerable = accreditedSet
                .Where(item => this.Contains(item) ^ other.Contains(item)).ToArray();
            return enumerable.Any() && enumerable.All(item => !other.Contains(item) && this.Contains(item));
        }

        public bool IsSubsetOf(IEnumerable<T> other) => this.IsSubsetOf(other, new HashSet<T>(((IEnumerable<T>)this).Union(other)));

        public bool IsSubsetOf(IEnumerable<T> other, ISet<T> accreditedSet)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            var enumerable = accreditedSet
                .Where(item => this.Contains(item) ^ other.Contains(item)).ToArray();
            return !enumerable.Any() || enumerable.All(item => other.Contains(item) && !this.Contains(item));
        }

        public bool IsSupersetOf(IEnumerable<T> other) => this.IsSupersetOf(other, new HashSet<T>(((IEnumerable<T>)this).Union(other)));

        public bool IsSupersetOf(IEnumerable<T> other, ISet<T> accreditedSet)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            var enumerable = accreditedSet
                .Where(item => this.Contains(item) ^ other.Contains(item)).ToArray();
            return !enumerable.Any() || enumerable.All(item => !other.Contains(item) && this.Contains(item));
        }

        public bool Overlaps(IEnumerable<T> other) => this.Overlaps(other, new HashSet<T>(((IEnumerable<T>)this).Union(other)));

        public bool Overlaps(IEnumerable<T> other, ISet<T> accreditedSet)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            return accreditedSet.Any(item => other.Contains(item) && this.Contains(item));
        }

        public bool SetEquals(IEnumerable<T> other) => this.SetEquals(other, new HashSet<T>(((IEnumerable<T>)this).Union(other)));

        public bool SetEquals(IEnumerable<T> other, ISet<T> accreditedSet)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            return accreditedSet.All(item => other.Contains(item) && this.Contains(item));
        }

        #region IList{T} Implementations
        int ICollection<T>.Count => ((IEnumerable<T>)this).Count(item => this.Contains(item));

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item) => this.Add(item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => ((IEnumerable<T>)this).ToList().CopyTo(array, arrayIndex);
        #endregion

        #region IList{ISet{T}} Implementations
        ISet<T> IList<ISet<T>>.this[int index] { get => this.setList[index]; set => this.setList[index] = value; }

        int ICollection<ISet<T>>.Count => this.setList.Count();

        bool ICollection<ISet<T>>.IsReadOnly => false;

        void ICollection<ISet<T>>.Add(ISet<T> item) => this.Add(item);

        void ICollection<ISet<T>>.Clear() => this.Clear();

        bool ICollection<ISet<T>>.Contains(ISet<T> item) => this.setList.Contains(item);

        void ICollection<ISet<T>>.CopyTo(ISet<T>[] array, int arrayIndex) => this.setList.CopyTo(array, arrayIndex);

        IEnumerator<ISet<T>> IEnumerable<ISet<T>>.GetEnumerator() => this.setList.GetEnumerator();

        int IList<ISet<T>>.IndexOf(ISet<T> item) => this.setList.IndexOf(item);

        void IList<ISet<T>>.Insert(int index, ISet<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.setList.Insert(index, item);
        }

        bool ICollection<ISet<T>>.Remove(ISet<T> item) => this.setList.Remove(item);

        void IList<ISet<T>>.RemoveAt(int index) => this.setList.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
