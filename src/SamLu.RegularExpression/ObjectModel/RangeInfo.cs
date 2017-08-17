using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    /// <summary>
    /// 提供用于创建、合并、求差和检测等操作范围的属性和实例方法的类型的虚基类，并且帮助创建 <see cref="IRange{T}"/> 对象。
    /// </summary>
    /// <typeparam name="T">范围的内容的类型。</typeparam>
    public abstract class RangeInfo<T> :
        IEqualityComparer<(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)>
    {
        /// <summary>
        /// <see cref="RangeInfo{T}"/> 使用的比较方法。
        /// </summary>
        protected Comparison<T> comparison;
        /// <summary>
        /// 获取 <see cref="RangeInfo{T}"/> 使用的比较方法。
        /// </summary>
        public Comparison<T> Comparison => this.comparison;

        /// <summary>
        /// 子类调用的构造函数，使用默认的比较器 <see cref="Comparer{T}.Default"/> 的比较方法 <see cref="Comparer{T}.Compare(T, T)"/> 初始化子类的新实例。
        /// </summary>
        protected RangeInfo() : this(Comparer<T>.Default.Compare) { }

        /// <summary>
        /// 子类调用的构造函数，使用指定的比较方法初始化子类的新实例。
        /// </summary>
        /// <param name="comparison">指定的比较方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> 的值为 null 。</exception>
        protected RangeInfo(Comparison<T> comparison) =>
            this.comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));

        /// <summary>
        /// 获取指定对象的前一个对象。
        /// </summary>
        /// <param name="value">指定的对象。</param>
        /// <returns>指定对象的前一个对象。</returns>
        /// <exception cref="InvalidOperationException">指定对象的前一个对象不存在。</exception>
        public abstract T GetPrev(T value);

        /// <summary>
        /// 获取指定对象的后一个对象。
        /// </summary>
        /// <param name="value">指定的对象。</param>
        /// <returns>指定对象的后一个对象。</returns>
        /// <exception cref="InvalidOperationException">指定对象的后一个对象不存在。</exception>
        public abstract T GetNext(T value);

        #region Equals
        /// <summary>
        /// 确定指定的两个范围是否相等。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>一个值，指示两个范围是否相等。</returns>
        public bool Equals(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        ) =>
            this.comparison(
                (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum)),
                (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum))
            ) == 0 &&
            this.comparison(
                (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum)),
                (firstCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum))
            ) == 0;
        
        /// <summary>
        /// 确定指定的两个范围是否相等。
        /// </summary>
        /// <param name="x">第一个范围。</param>
        /// <param name="y">第二个范围。</param>
        /// <returns>一个值，指示两个范围是否相等。</returns>
        public bool Equals((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) x, (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) y) =>
            this.Equals(
                x.minimum, x.maximum, x.canTakeMinimum, x.canTakeMaximum,
                y.minimum, y.maximum, y.canTakeMinimum, y.canTakeMaximum
            );
        #endregion

        #region GetEnumerable
        /// <summary>
        /// 获取指定范围的枚举。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>指定范围的枚举。</returns>
        /// <exception cref="InvalidRangeException{T}">范围是无效的。</exception>
        public IEnumerable<T> GetEnumerable(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            if (!this.IsValid(minimum, maximum, canTakeMinimum, canTakeMaximum))
                throw new InvalidRangeException<T>(minimum, maximum, canTakeMinimum, canTakeMaximum, this.comparison);

            return this.GetEnumerableInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        /// <summary>
        /// 子类重写时，提供获取指定范围的枚举的实现。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>指定范围的枚举。</returns>
        protected virtual IEnumerable<T> GetEnumerableInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            T start = canTakeMinimum ? minimum : this.GetNext(minimum);
            T end = canTakeMaximum ? maximum : this.GetPrev(maximum);

            for (T current = start; this.comparison(current, end) != 0; current = this.GetNext(current))
                yield return current;
            yield return end;
        }

        /// <summary>
        /// 获取指定范围的枚举。
        /// </summary>
        /// <param name="range">指定的范围</param>
        /// <returns>指定范围的枚举。</returns>
        /// <exception cref="InvalidRangeException{T}">范围是无效的。</exception>
        public IEnumerable<T> GetEnumerable((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range) =>
            this.GetEnumerable(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        #endregion

        #region Adapt
        /// <summary>
        /// 适配指定的范围。
        /// </summary>
        /// <param name="firstExtremum">第一个最值。</param>
        /// <param name="secondExtremum">第二个最值。</param>
        /// <param name="canTakeFirstExtremum">一个值，指示是否能取到第一个最值。</param>
        /// <param name="canTakeSecondExtremum">一个值，指示是否能取到第二个最值。</param>
        /// <returns>一个范围，使用内部比较方法，由指定的范围适配。</returns>
        public IRange<T> Adapt(T firstExtremum, T secondExtremum, bool canTakeFirstExtremum, bool canTakeSecondExtremum)
        {
            T minimum = default(T), maximum = default(T);
            bool canTakeMinimum = false, canTakeMaximum = false;
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

        /// <summary>
        /// 适配指定的范围。
        /// </summary>
        /// <param name="other">指定的范围。</param>
        /// <returns>一个范围，使用内部比较方法，由指定的范围适配。</returns>
        public virtual IRange<T> Adapt(IRange<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (other.Comparison == this.comparison)
                return other;
            else
                return this.Adapt(other.Minimum, other.Maximum, other.CanTakeMinimum, other.CanTakeMaximum);
        }
        #endregion

        #region Create
        /// <summary>
        /// 创建 <see cref="IRange{T}"/> 的新对象实例，该实例表示指定的范围。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns><see cref="IRange{T}"/> 的新对象实例，该实例表示指定的范围。</returns>
        public IRange<T> Create(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            if (!this.IsValid(minimum, maximum, canTakeMinimum, canTakeMaximum))
                throw new InvalidRangeException<T>(minimum, maximum, canTakeMaximum, canTakeMaximum, this.comparison);

            return this.CreateInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        /// <summary>
        /// 子类重写时，提供创建表示指定的范围的 <see cref="IRange{T}"/> 的新对象实例的实现。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns><see cref="IRange{T}"/> 的新对象实例，该实例表示指定的范围。</returns>
        protected virtual IRange<T> CreateInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            new Range<T>(minimum, maximum, canTakeMinimum, canTakeMaximum, this.comparison);

        /// <summary>
        /// 创建 <see cref="IRange{T}"/> 的新对象实例，该实例表示指定的范围。
        /// </summary>
        /// <param name="range">指定的范围。</param>
        /// <returns><see cref="IRange{T}"/> 的新对象实例，该实例表示指定的范围。</returns>
        public IRange<T> Create((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range) =>
            this.Create(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        #endregion

        #region Union
        /// <summary>
        /// 对两个指定范围进行并集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">并集的最小值。</param>
        /// <param name="resultMaximum">并集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到并集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到并集的最大值。</param>
        /// <exception cref="RangeInvalidUnionOperationException{T}">两个范围既不相交也不相邻，无法进行并集操作。</exception>
        public void Union(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (
                this.IsOverlap(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
                ) ||
                this.IsNextTo(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
                )
            )
                this.UnionInternal(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            else throw new RangeInvalidUnionOperationException<T>(
                this.Create(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum),
                this.Create(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum)
            );
        }

        /// <summary>
        /// 子类重写时，提供对两个指定范围进行并集操作的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">并集的最小值。</param>
        /// <param name="resultMaximum">并集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到并集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到并集的最大值。</param>
        protected virtual void UnionInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (this.comparison(firstMinimum, secondMinimum) < 0)
            {
                resultMinimum = firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum;
            }
            else if (this.comparison(firstMinimum, secondMinimum) > 0)
            {
                resultMinimum = secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum);
                resultCanTakeMinimum = secondCanTakeMinimum;
            }
            else
            {
                resultMinimum = (firstCanTakeMinimum || secondCanTakeMinimum) ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum || secondCanTakeMinimum;
            }

            if (this.comparison(firstMaximum, secondMaximum) < 0)
            {
                resultMaximum = secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum);
                resultCanTakeMaximum = secondCanTakeMaximum;
            }
            else if (this.comparison(firstMaximum, secondMaximum) > 0)
            {
                resultMaximum = firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum;
            }
            else
            {
                resultMaximum = (firstCanTakeMaximum || secondCanTakeMaximum) ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum || secondCanTakeMaximum;
            }
        }

        /// <summary>
        /// 对两个指定范围进行并集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>两个指定范围的并集。</returns>
        /// <exception cref="RangeInvalidUnionOperationException{T}">两个范围既不相交也不相邻，无法进行并集操作。</exception>
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

        /// <summary>
        /// 对两个及以上指定范围进行并集操作。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <param name="rest">剩余的范围。</param>
        /// <returns>两个及以上指定范围的并集。</returns>
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

        /// <summary>
        /// 尝试对两个指定范围进行并集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">并集的最小值。</param>
        /// <param name="resultMaximum">并集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到并集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到并集的最大值。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
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

        /// <summary>
        /// 尝试对两个指定范围进行并集操作。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <param name="resultRange">并集的范围。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public bool TryUnion(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange,
            out (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) resultRange
        )
        {
            resultRange = (default(T), default(T), false, false);

            if (this.TryUnion(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum,
                out resultRange.minimum, out resultRange.maximum, out resultRange.canTakeMinimum, out resultRange.canTakeMaximum
            )) return true;
            else return false;
        }
        #endregion

        #region Intersect
        /// <summary>
        /// 对两个指定范围进行交集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">交集的最小值。</param>
        /// <param name="resultMaximum">交集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到交集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到交集的最大值。</param>
        /// <exception cref="RangeInvalidUnionOperationException{T}">两个范围既不相交也不相邻，无法进行交集操作。</exception>
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

        /// <summary>
        /// 子类重写时，提供对两个指定范围进行交集操作的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">交集的最小值。</param>
        /// <param name="resultMaximum">交集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到交集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到交集的最大值。</param>
        protected virtual void IntersectInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (this.comparison(firstMinimum, secondMinimum) < 0)
            {
                resultMinimum = secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum);
                resultCanTakeMinimum = secondCanTakeMinimum;
            }
            else if (this.comparison(firstMinimum, secondMinimum) > 0)
            {
                resultMinimum = firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum;
            }
            else
            {
                resultMinimum = (firstCanTakeMinimum && secondCanTakeMinimum) ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum && secondCanTakeMinimum;
            }

            if (this.comparison(firstMinimum, secondMinimum) < 0)
            {
                resultMaximum = firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum;
            }
            else if (this.comparison(firstMinimum, secondMinimum) > 0)
            {
                resultMaximum = secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum);
                resultCanTakeMaximum = secondCanTakeMaximum;
            }
            else
            {
                resultMaximum = (firstCanTakeMaximum && secondCanTakeMaximum) ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum && secondCanTakeMaximum;
            }
        }

        /// <summary>
        /// 对两个指定范围进行交集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>两个指定范围的交集。</returns>
        /// <exception cref="RangeInvalidUnionOperationException{T}">两个范围既不相交也不相邻，无法进行交集操作。</exception>
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

        /// <summary>
        /// 对两个及以上指定范围进行交集操作。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <param name="rest">剩余的范围。</param>
        /// <returns>两个及以上指定范围的交集。</returns>
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

        /// <summary>
        /// 尝试对两个指定范围进行交集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">交集的最小值。</param>
        /// <param name="resultMaximum">交集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到交集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到交集的最大值。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public bool TryIntersect(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            resultMinimum = resultMaximum = default(T);
            resultCanTakeMinimum = resultCanTakeMaximum = false;

            try
            {
                this.Intersect(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            }
            catch (Exception) { return false; }

            return true;
        }

        /// <summary>
        /// 尝试对两个指定范围进行交集操作。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <param name="resultRange">交集的范围。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public bool TryIntersect(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange,
            out (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) resultRange
        )
        {
            resultRange = (default(T), default(T), false, false);

            if (this.TryIntersect(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum,
                out resultRange.minimum, out resultRange.maximum, out resultRange.canTakeMinimum, out resultRange.canTakeMaximum
            )) return true;
            else return false;
        }
        #endregion

        #region Except
        /// <summary>
        /// 对两个指定范围进行差集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">差集的最小值。</param>
        /// <param name="resultMaximum">差集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到差集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到差集的最大值。</param>
        /// <exception cref="RangeInvalidExceptOperationException{T}">两个范围既不相交也不相邻，无法进行差集操作。</exception>
        public void Except(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (
                this.IsOverlapInternal(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
                ) &&
                !this.IsSubsetOf(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
                )
            )
            {
                this.ExceptInternal(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            }
            else
            {
                resultMinimum = resultMaximum = default(T);
                resultCanTakeMinimum = resultCanTakeMaximum = false;

                throw new RangeInvalidExceptOperationException<T>(
                    this.Create(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum),
                    this.Create(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum)
                );
            }
        }

        /// <summary>
        /// 子类重写时，提供对两个指定范围进行差集操作的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">差集的最小值。</param>
        /// <param name="resultMaximum">差集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到差集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到差集的最大值。</param>
        protected virtual void ExceptInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (this.IsProperSupersetOfInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            ))
            {
                if (this.comparison(
                    (firstCanTakeMinimum?firstMinimum:this.GetNext(firstMinimum)),
                    (secondCanTakeMinimum?secondMinimum:this.GetNext(secondMinimum))
                )==0)
                {
                    resultMinimum = secondMaximum;
                    resultMaximum = firstMaximum;
                    resultCanTakeMinimum = !secondCanTakeMaximum;
                    resultCanTakeMaximum = firstCanTakeMaximum;
                }
                else
                {
                    resultMinimum = firstMinimum;
                    resultMaximum = secondMinimum;
                    resultCanTakeMinimum = firstCanTakeMinimum;
                    resultCanTakeMaximum = !secondCanTakeMinimum;
                }
            }
            else
            {
                if (this.comparison(
                    (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum)),
                    (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum))
                ) < 0)
                {
                    resultMinimum = firstMinimum;
                    resultCanTakeMinimum = firstCanTakeMinimum;
                }
                else
                {
                    resultMinimum = secondMinimum;
                    resultCanTakeMinimum = secondCanTakeMinimum;
                }

                if (this.comparison(
                    (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum)),
                    (secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum))
                ) < 0)
                {
                    resultMaximum = firstMaximum;
                    resultCanTakeMaximum = firstCanTakeMaximum;
                }
                else
                {
                    resultMaximum = secondMaximum;
                    resultCanTakeMaximum = secondCanTakeMaximum;
                }
            }
        }

        /// <summary>
        /// 对两个指定范围进行差集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>两个指定范围的差集。</returns>
        /// <exception cref="RangeInvalidExceptOperationException{T}">两个范围既不相交也不相邻，无法进行差集操作。</exception>
        public (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) Except(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range = new ValueTuple<T, T, bool, bool>();
            this.Except(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                out range.minimum, out range.maximum, out range.canTakeMinimum, out range.canTakeMaximum
            );

            return range;
        }

        /// <summary>
        /// 尝试对两个指定范围进行差集操作。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <param name="resultMinimum">差集的最小值。</param>
        /// <param name="resultMaximum">差集的最大值。</param>
        /// <param name="resultCanTakeMinimum">一个值，指示是否能取到差集的最小值。</param>
        /// <param name="resultCanTakeMaximum">一个值，指示是否能取到差集的最大值。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public bool TryExcept(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out T resultMinimum, out T resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            resultMinimum = resultMaximum = default(T);
            resultCanTakeMinimum = resultCanTakeMaximum = false;

            try
            {
                this.Except(
                    firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                    secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum,
                    out resultMinimum, out resultMaximum, out resultCanTakeMinimum, out resultCanTakeMaximum
                );
            }
            catch (Exception) { return false; }

            return true;
        }

        /// <summary>
        /// 尝试对两个指定范围进行差集操作。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <param name="resultRange">差集的范围。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public bool TryExcept(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange,
            out (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) resultRange
        )
        {
            resultRange = (default(T), default(T), false, false);

            if (this.TryExcept(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum,
                out resultRange.minimum, out resultRange.maximum, out resultRange.canTakeMinimum, out resultRange.canTakeMaximum
            )) return true;
            else return false;
        }
        #endregion

        #region IsValid
        /// <summary>
        /// 检测范围是否合法。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>当范围合法时，则为 true ；否则为 false 。</returns>
        public bool IsValid(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            int compareValue = this.comparison(minimum, maximum);
            if (compareValue > 0)
                return false;
            else if (compareValue == 0)
                return canTakeMinimum && canTakeMaximum;
            else
                return this.IsValidInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        /// <summary>
        /// 子类重写时，提供检测范围是否合法的实现。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>当范围合法时，则为 true ；否则为 false 。</returns>
        protected virtual bool IsValidInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            (this.comparison(this.GetPrev(maximum), this.GetNext(minimum)) >= 0 || (canTakeMinimum || canTakeMaximum));

        /// <summary>
        /// 检测范围是否合法。
        /// </summary>
        /// <param name="range">要检测的范围。</param>
        /// <returns>当范围合法时，则为 true ；否则为 false 。</returns>
        public bool IsValid((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) range) =>
            this.IsValid(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        #endregion

        #region IsSubsetOf
        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的子集。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsSubsetOfInternal(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsSubsetOf(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (!this.IsValid(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum))
                throw new InvalidRangeException<T>(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum, this.comparison);
            if (!this.IsValid(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum))
                throw new InvalidRangeException<T>(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum, this.comparison);

            return this.IsSubsetOfInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            );
        }

        /// <summary>
        /// 子类重写时，提供对两个范围进行检测，第一个范围是否为第二个范围的子集的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的子集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsSubsetOfInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            int minimumCompareResult = this.comparison(
                (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum)),
                (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum))
            );
            int maximumCompareResult = this.comparison(
                (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum)),
                (secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum))
            );

            return minimumCompareResult <= 0 && maximumCompareResult <= 0;
        }

        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的子集。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <returns>若第一个范围为第二个范围的子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsSubsetOf(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsSubsetOf(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsSubsetOf(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion

        #region IsSupersetOf
        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的超集。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsSubsetOfInternal(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsSupersetOf(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (!this.IsValid(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum))
                throw new InvalidRangeException<T>(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum, this.comparison);
            if (!this.IsValid(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum))
                throw new InvalidRangeException<T>(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum, this.comparison);

            return this.IsSupersetOfInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            );
        }

        /// <summary>
        /// 子类重写时，提供对两个范围进行检测，第一个范围是否为第二个范围的超集的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的超集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsSupersetOfInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            int minimumCompareResult = this.comparison(
                (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum)),
                (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum))
            );
            int maximumCompareResult = this.comparison(
                (secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum)),
                (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum))
            );

            return minimumCompareResult <= 0 && maximumCompareResult <= 0;
        }

        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的超集。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <returns>若第一个范围为第二个范围的超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsSubsetOf(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsSupersetOf(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsSupersetOf(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion

        #region IsProperSubsetOf
        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的真子集。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的真子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsProperSubsetOfInternal(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsProperSubsetOf(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (!this.IsValid(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum))
                throw new InvalidRangeException<T>(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum, this.comparison);
            if (!this.IsValid(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum))
                throw new InvalidRangeException<T>(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum, this.comparison);

            return this.IsProperSubsetOfInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            );
        }

        /// <summary>
        /// 子类重写时，提供对两个范围进行检测，第一个范围是否为第二个范围的真子集的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的真子集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsProperSubsetOfInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            int minimumCompareResult = this.comparison(
                (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum)),
                (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum))
            );
            int maximumCompareResult = this.comparison(
                (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum)),
                (secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum))
            );

            return
                (minimumCompareResult <= 0 && maximumCompareResult < 0) ||
                (minimumCompareResult < 0 && maximumCompareResult <= 0);
        }

        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的真子集。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <returns>若第一个范围为第二个范围的真子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsProperSubsetOf(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsProperSubsetOf(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsProperSubsetOf(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion

        #region IsProperSupersetOf
        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的真超集。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的真超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsProperSubsetOfInternal(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsProperSupersetOf(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (!this.IsValid(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum))
                throw new InvalidRangeException<T>(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum, this.comparison);
            if (!this.IsValid(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum))
                throw new InvalidRangeException<T>(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum, this.comparison);

            return this.IsProperSupersetOfInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            );
        }

        /// <summary>
        /// 子类重写时，提供对两个范围进行检测，第一个范围是否为第二个范围的真超集的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>若第一个范围为第二个范围的真超集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsProperSupersetOfInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            int minimumCompareResult = this.comparison(
                (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum)),
                (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum))
            );
            int maximumCompareResult = this.comparison(
                (secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum)),
                (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum))
            );

            return
                (minimumCompareResult <= 0 && maximumCompareResult < 0) ||
                (minimumCompareResult < 0 && maximumCompareResult <= 0);
        }

        /// <summary>
        /// 对两个范围进行检测，第一个范围是否为第二个范围的真超集。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <returns>若第一个范围为第二个范围的真超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="InvalidRangeException{T}">第一个范围为无效范围。</exception>
        /// <exception cref="InvalidRangeException{T}">第二个范围为无效范围。</exception>
        /// <seealso cref="IsProperSubsetOf(T, T, bool, bool, T, T, bool, bool)"/>
        public bool IsProperSupersetOf(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsProperSupersetOf(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion

        #region IsNextTo
        /// <summary>
        /// 检测两个范围是否相邻。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>如果两个范围相邻，则为 true ；否则为 false 。</returns>
        public bool IsNextTo(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (!this.IsValid(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum))
                throw new InvalidRangeException<T>(firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum, this.comparison);
            if (!this.IsValid(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum))
                throw new InvalidRangeException<T>(secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum, this.comparison);

            return this.IsNextToInternal(
                firstMinimum, firstMaximum, firstCanTakeMinimum, firstCanTakeMaximum,
                secondMinimum, secondMaximum, secondCanTakeMinimum, secondCanTakeMaximum
            );
        }

        /// <summary>
        /// 子类重写时，提供检测两个范围是否相邻的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>如果两个范围相邻，则为 true ；否则为 false 。</returns>
        protected virtual bool IsNextToInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            (T minimum, T maximum) firstRange = (
                (firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum)),
                (firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum))
            );
            (T minimum, T maximum) secondRange = (
                (secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum)),
                (secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum))
            );

            if (this.comparison(firstRange.maximum, secondRange.minimum) < 0)
                return this.comparison(this.GetNext(firstRange.maximum), secondRange.minimum) == 0;
            else if (this.comparison(firstRange.minimum, secondRange.maximum) > 0)
                return this.comparison(this.GetPrev(firstRange.minimum), secondRange.maximum) == 0;
            else return false;
        }

        /// <summary>
        /// 检测两个范围是否相邻。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <returns>如果两个范围相邻，则为 true ；否则为 false 。</returns>
        public bool IsNextTo(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsNextTo(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion

        #region IsOverlap
        /// <summary>
        /// 检测两个范围是否相交。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>如果两个范围相交，则为 true ；否则为 false 。</returns>
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

        /// <summary>
        /// 子类重写时，提供检测两个范围是否相交的实现。
        /// </summary>
        /// <param name="firstMinimum">第一个范围的最小值。</param>
        /// <param name="firstMaximum">第一个范围的最大值。</param>
        /// <param name="firstCanTakeMinimum">一个值，指示是否能取到第一个范围的最小值。</param>
        /// <param name="firstCanTakeMaximum">一个值，指示是否能取到第一个范围的最大值。</param>
        /// <param name="secondMinimum">第二个范围的最小值。</param>
        /// <param name="secondMaximum">第二个范围的最大值。</param>
        /// <param name="secondCanTakeMinimum">一个值，指示是否能取到第二个范围的最小值。</param>
        /// <param name="secondCanTakeMaximum">一个值，指示是否能取到第二个范围的最大值。</param>
        /// <returns>如果两个范围相交，则为 true ；否则为 false 。</returns>
        protected virtual bool IsOverlapInternal(
            T firstMinimum, T firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            T secondMinimum, T secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (this.comparison(firstMaximum, secondMinimum) < 0 || this.comparison(secondMaximum, firstMinimum) < 0) return false;

            if (this.comparison(firstMaximum, secondMinimum) == 0)
            {
                if (firstCanTakeMaximum && secondCanTakeMinimum) return true;
            }
            else if (this.comparison(firstMaximum, this.GetNext(secondMinimum)) == 0)
            {
                if (firstCanTakeMaximum || secondCanTakeMinimum) return true;
            }
            else return true;

            if (this.comparison(secondMaximum, firstMinimum) == 0)
            {
                if (firstCanTakeMaximum && secondCanTakeMinimum) return true;
            }
            else if (this.comparison(secondMaximum, this.GetNext(firstMinimum)) == 0)
            {
                if (firstCanTakeMaximum || secondCanTakeMinimum) return true;
            }
            else return true;

            return false;
        }

        /// <summary>
        /// 检测两个范围是否相交。
        /// </summary>
        /// <param name="firstRange">第一个范围。</param>
        /// <param name="secondRange">第二个范围。</param>
        /// <returns>如果两个范围相交，则为 true ；否则为 false 。</returns>
        public bool IsOverlap(
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) firstRange,
            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) secondRange
        ) =>
            this.IsOverlap(
                firstRange.minimum, firstRange.maximum, firstRange.canTakeMinimum, firstRange.canTakeMaximum,
                secondRange.minimum, secondRange.maximum, secondRange.canTakeMinimum, secondRange.canTakeMaximum
            );
        #endregion

        #region IEqualityComparer{T,T,bool,bool} Implementations
        int IEqualityComparer<(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)>.GetHashCode((T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) obj) =>
            obj.GetHashCode();
        #endregion
    }
}
