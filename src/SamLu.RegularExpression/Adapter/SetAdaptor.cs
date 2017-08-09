using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class SetAdaptor<TSource, TTarget> : ISet<TTarget>
    {
        protected ISet<TSource> sourceSet;
        protected IEqualityComparer<TSource> comparer;

        protected AdaptContextInfo<TSource, TTarget> contextInfo;

        public IEqualityComparer<TSource> Comparer => this.comparer;

        public int Count => this.sourceSet.Count;

        public bool IsReadOnly => this.sourceSet.IsReadOnly;

        public bool Add(TTarget item)
        {
            if (this.contextInfo.TryAdaptTarget(item, out TSource source))
                return this.sourceSet.Add(source);
            else return false;
        }

        public void Clear()
        {
            this.sourceSet.Clear();
        }

        public bool Contains(TTarget item)
        {
            if (this.contextInfo.TryAdaptTarget(item, out TSource source))
                return this.sourceSet.Contains(source);
            else return false;
        }

        public void CopyTo(TTarget[] array) => this.CopyTo(array, 0);

        public void CopyTo(TTarget[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            TSource[] sourceArray = new TSource[array.Length];
            this.sourceSet.CopyTo(sourceArray, arrayIndex);

            for (int index = 0; index < this.sourceSet.Count; index++)
            {
                if (this.contextInfo.TryAdaptSource(sourceArray[arrayIndex + index], out TTarget target, out Exception innerException))
                    array[arrayIndex + index] = target;
                else throw new InvalidOperationException("在适配源时发生错误。", innerException);
            }
        }

        public void ExceptWith(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (this.sourceSet.Count == 0) return;

            if (other == this) this.Clear();

            foreach (TTarget target in other)
                this.Remove(target);
        }

        public IEnumerator<TTarget> GetEnumerator()
        {
            foreach (TSource source in this.sourceSet)
            {
                if (this.contextInfo.TryAdaptSource(source, out TTarget target, out Exception innerException))
                    yield return target;
                else throw new InvalidOperationException("在适配源时发生错误。", innerException);
            }

            yield break;
        }

        public void IntersectWith(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (this.sourceSet.Count == 0) return;

            if (other == this) return;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            this.sourceSet.IntersectWith(set);
        }

        public bool IsProperSubsetOf(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other == this) return false;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            return this.sourceSet.IsProperSubsetOf(set);
        }

        public bool IsProperSupersetOf(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other == this) return false;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            return this.sourceSet.IsProperSupersetOf(set);
        }

        public bool IsSubsetOf(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other == this) return true;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            return this.sourceSet.IsSubsetOf(set);
        }

        public bool IsSupersetOf(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other == this) return true;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            return this.sourceSet.IsSupersetOf(set);
        }

        public bool Overlaps(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other == this) return true;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            return this.sourceSet.Overlaps(set);
        }

        public bool Remove(TTarget item)
        {
            if (this.contextInfo.TryAdaptTarget(item, out TSource source))
                return this.sourceSet.Remove(source);
            else return false;
        }

        public bool SetEquals(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other == this) return true;

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            return this.sourceSet.SetEquals(set);
        }

        public void SymmetricExceptWith(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (this.sourceSet.Count == 0) return;

            if (other == this) this.Clear();

            ISet<TSource> set = new HashSet<TSource>(this.Comparer);
            foreach (TTarget target in other)
            {
                if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    set.Add(source);
            }
            this.sourceSet.SymmetricExceptWith(set);
        }

        public void UnionWith(IEnumerable<TTarget> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (this.sourceSet.Count == 0) return;

            if (other == this) return;

            foreach (TTarget target in other)
                this.Add(target);
        }

        #region ISet{TTarget} Implementations
        void ICollection<TTarget>.Add(TTarget item)
        {
            this.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.sourceSet.GetEnumerator();
        }
        #endregion
    }
}
