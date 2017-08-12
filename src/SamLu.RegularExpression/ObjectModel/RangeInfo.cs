﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public abstract class RangeInfo<T>
    {
        protected Comparison<T> comparison;
        public Comparison<T> Comparison => this.comparison;

        protected RangeInfo() : this(Comparer<T>.Default.Compare) { }

        protected RangeInfo(Comparison<T> comparison) =>
            this.comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));

        public abstract T GetPrev(T value);

        public abstract T GetNext(T value);

        #region GetEnumerable
        public IEnumerable<T> GetEnumerable(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            if (!this.IsValid(minimum, maximum, canTakeMinimum, canTakeMaximum))
                throw new InvalidRangeException<T>(minimum, maximum, canTakeMinimum, canTakeMaximum, this.comparison);

            return this.GetEnumerableInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        protected virtual IEnumerable<T> GetEnumerableInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            T start = canTakeMinimum ? minimum : this.GetNext(minimum);
            T end = canTakeMaximum ? maximum : this.GetPrev(maximum);
            for (T current = start; this.comparison(current, end) != 0; current = this.GetNext(current))
                yield return current;
        }

        public IEnumerable<T> GetEnumerable((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range) =>
            this.GetEnumerable(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        #endregion

        #region Suit
        public IRange<T> Suit(T firstExtremum, T secondExtremum, bool canTakeFirstExtremum, bool canTakeSecondExtremum)
        {
            T minimum = default(T), maximum = default(T);
            bool canTakeMinimum = true, canTakeMaximum = false;
            if (this.comparison(firstExtremum, secondExtremum) <= 0)
            {
                minimum = firstExtremum;
                maximum = secondExtremum;
                canTakeMinimum = canTakeFirstExtremum;
                canTakeMaximum = canTakeSecondExtremum;
            }
            else if (this.comparison(firstExtremum, secondExtremum) >= 0)
            {
                minimum = secondExtremum;
                maximum = firstExtremum;
                canTakeMinimum = canTakeSecondExtremum;
                canTakeMaximum = canTakeFirstExtremum;
            }

            return this.Create(minimum, maximum, canTakeMaximum, canTakeMaximum);
        }

        public virtual IRange<T> Suit(IRange<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other.Comparison == this.comparison)
                return other;
            else
                return this.Suit(other.Minimum, other.Maximum, other.CanTakeMinimum, other.CanTakeMaximum);
        }
        #endregion

        #region Create
        public IRange<T> Create(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            if (!this.IsValid(minimum, maximum, canTakeMinimum, canTakeMaximum))
                throw new InvalidRangeException<T>(minimum, maximum, canTakeMaximum, canTakeMaximum, this.comparison);

            return this.CreateInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        protected virtual IRange<T> CreateInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            new Range<T>(minimum, maximum, canTakeMinimum, canTakeMaximum, this.comparison);

        public IRange<T> Create((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range) =>
            this.Create(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        #endregion

        #region Union
        public void Union(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            bool canUnion = false;
            if (this.IsOverlap(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            )) canUnion = true;
            else
            {
                if (this.comparison(firstMaximum, secondMinimum) == 0 && (firstCanTakeMaximum || secondCanTakeMinimum))
                    canUnion = true;
                else if (this.comparison(this.GetPrev(firstMaximum), secondMinimum) == 0 && (!firstCanTakeMaximum && !secondCanTakeMinimum))
                    canUnion = true;
                else if (this.comparison(firstMinimum, secondMaximum) == 0 && (firstCanTakeMinimum || secondCanTakeMaximum))
                    canUnion = true;
                else if (this.comparison(firstMinimum, this.GetNext(secondMaximum)) == 0 && (!firstCanTakeMinimum && !secondCanTakeMaximum))
                    canUnion = true;
                else
                    canUnion = false;
            }

            if (canUnion)
                this.UnionInternal(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            else throw new RangeNotOverlapException();
        }

        protected abstract void UnionInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        );

        public (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) Union(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range = new ValueTuple<T, T, bool, bool>();
            this.Union(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                out range.minimum, out range.maximum, out range.canTakeMinimum, out range.canTakeMaximum
            );

            return range;
        }

        public (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) Union(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange,
            params (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)[] rest
        )
        {
            var ranges = new[] { firstRange, secondRange }.Concat(rest ?? Enumerable.Empty<(T, T, bool, bool)>());

            var collection = new Collection<(T, T, bool, bool)>();
            foreach (var range in ranges)
            {
                if (!this.IsValid(range.Item1, range.Item2, range.Item3, range.Item4))
                    throw new InvalidRangeException<T>(range.Item1, range.Item2, range.Item3, range.Item4, this.comparison);

                var result = range;
                var enumerator = collection.GetEnumerator();
                collection = new Collection<(T, T, bool, bool)>(); // 新建空集合。
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (this.IsOverlap(current, result))
                        this.UnionInternal(
                            current.Item1, current.Item2, current.Item3, current.Item4,
                            result.Item1, result.Item2, result.Item3, result.Item4,
                            out result.Item1, out result.Item2, out result.Item3, out result.Item4
                        );
                    else collection.Add(current);
                }

                collection.Add(result);
            }

            if (collection.Count == 1) return collection.ElementAt(0);
            else
                throw new RangeNotOverlapException();
        }

        public bool TryUnion(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            resultMinimum = resultMaximum = default(T);
            resultCanTakeMinimum = resultCanTakeMaximum = false;

            try
            {
                this.Union(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            }
            catch (Exception) { return false; }

            return true;
        }
        #endregion

        #region Intersect
        public void Intersect(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (this.IsOverlap(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            ))
                this.IntersectInternal(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            else
            {
                resultMinimum = resultMaximum = default(T);
                resultCanTakeMinimum = resultCanTakeMaximum = false;
            }
        }

        protected abstract void IntersectInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        );

        public (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) Intersect(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range = new ValueTuple<T, T, bool, bool>();
            this.Intersect(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                out range.minimum, out range.maximum, out range.canTakeMinimum, out range.canTakeMaximum
            );

            return range;
        }

        public (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) Intersect(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange,
            params (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)[] rest
        )
        {
            var ranges = new[] { firstRange, secondRange }.Concat(rest ?? Enumerable.Empty<(T, T, bool, bool)>()).ToList();
            ranges.ForEach(range =>
            {
                if (!this.IsValid(range.Item1, range.Item2, range.Item3, range.Item4))
                    throw new InvalidRangeException<T>(range.Item1, range.Item2, range.Item3, range.Item4, this.comparison);
            });

            return ranges.Aggregate((range1, range2) =>
            {
                (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) result = new ValueTuple<T, T, bool, bool>();
                this.IntersectInternal(
                    range1.Item1, range1.Item2, range1.Item3, range1.Item4,
                    range2.Item1, range2.Item2, range2.Item3, range2.Item4,
                    out result.minimum, out result.maximum, out result.canTakeMinimum, out result.canTakeMaximum
                );

                return result;
            });
        }
        #endregion

        #region IsValid
        public bool IsValid(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            int compareValue = this.comparison(minimum, maximum);
            if (compareValue > 0)
                return false;
            else if (compareValue == 0 && !(canTakeMinimum && canTakeMaximum))
                return false;
            else
                return !this.IsValidInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        public abstract bool IsValidInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum);

        public bool IsValid((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range) =>
            !this.IsValid(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        #endregion

        #region IsOverlap
        public bool IsOverlap(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (!this.IsValid(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum))
                throw new InvalidRangeException<T>(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum, this.comparison);
            if (!this.IsValid(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum))
                throw new InvalidRangeException<T>(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum, this.comparison);

            return this.IsOverlapInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            );
        }

        protected abstract bool IsOverlapInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        );

        public bool IsOverlap(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsOverlap(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion
    }

    public static class RangeInfo
    {

    }
}
