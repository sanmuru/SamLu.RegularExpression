using SamLu.Diagnostics;
using SamLu.RegularExpression.DebugView;
using SamLu.RegularExpression.Diagnostics;
using SamLu.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    /// <summary>
    /// 内部以范围为元数据组织的集。
    /// </summary>
    /// <typeparam name="T">集包含的对象的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebugInfoProxy(typeof(RangeSetDebugInfo<>), new[] { TypeParameterFillin.TypeParameter_0 })]
    public class RangeSet<T> : ISet<T>, ISet<IRange<T>>
    {
        /// <summary>
        /// 内部的范围集合。
        /// </summary>
        protected ICollection<IRange<T>> ranges;
        /// <summary>
        /// 内部的比较器。
        /// </summary>
        protected IComparer<T> comparer;
        /// <summary>
        /// 内部的集合说明。
        /// </summary>
        protected RangeInfo<T> rangeInfo;

        /// <summary>
        /// 获取 <see cref="RangeSet{T}"/> 的范围集合表示。
        /// </summary>
        public ICollection<IRange<T>> Ranges =>
            new ReadOnlyCollection<IRange<T>>(((IEnumerable<IRange<T>>)this).ToList());
        /// <summary>
        /// 获取 <see cref="RangeSet{T}"/> 的比较器。
        /// </summary>
        public IComparer<T> Comparer => this.comparer;
        /// <summary>
        /// 获取 <see cref="RangeSet{T}"/> 的集合说明。
        /// </summary>
        public RangeInfo<T> RangeInfo => this.rangeInfo;

        /// <summary>
        /// 获取 <see cref="RangeSet{T}"/> 中包含的元素数。
        /// </summary>
        public virtual int Count
        {
            get
            {
                int count = 0;
                var enumerator = this.GetEnumerator();
                while (enumerator.MoveNext()) count++;

                return count;
            }
        }

        /// <summary>
        /// 子类继承的默认构造函数，初始化通用字段。
        /// </summary>
        protected RangeSet() =>
            this.ranges = new Collection<IRange<T>>();

        /// <summary>
        /// 初始化 <see cref="RangeSet{T}"/> 类的新实例，该实例使用指定的范围说明。
        /// </summary>
        /// <param name="rangeInfo">指定的范围说明。</param>
        /// <exception cref="ArgumentNullException"><paramref name="rangeInfo"/> 的值为 null 。</exception>
        public RangeSet(RangeInfo<T> rangeInfo) : this()
        {
            if (rangeInfo == null) throw new ArgumentNullException(nameof(rangeInfo));

            this.comparer = Comparer<T>.Create(rangeInfo.Comparison);
            this.rangeInfo = rangeInfo;
        }

        /// <summary>
        /// 初始化 <see cref="RangeSet{T}"/> 类的新实例，该实例包含从指定的集合复制的元素且使用指定的范围说明。
        /// </summary>
        /// <param name="collection">其元素被复制到 <see cref="RangeSet{T}"/> 的集合。</param>
        /// <param name="rangeInfo">指定的范围说明。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="rangeInfo"/> 的值为 null 。</exception>
        public RangeSet(IEnumerable<T> collection, RangeInfo<T> rangeInfo) : this(rangeInfo) =>
            this.UnionWith(collection ?? throw new ArgumentNullException(nameof(collection)));

        /// <summary>
        /// 初始化 <see cref="RangeSet{T}"/> 类的新实例，该实例包含从指定的范围集合复制的元素且使用指定的范围说明。
        /// </summary>
        /// <param name="collection">其元素被复制到 <see cref="RangeSet{T}"/> 的范围集合。</param>
        /// <param name="rangeInfo">指定的范围说明。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="rangeInfo"/> 的值为 null 。</exception>
        public RangeSet(IEnumerable<IRange<T>> collection, RangeInfo<T> rangeInfo) : this(rangeInfo) =>
            this.UnionWith(collection ?? throw new ArgumentNullException(nameof(collection)));

        #region Add
        /// <summary>
        /// 向 <see cref="RangeSet{T}"/> 中添加一个对象。
        /// </summary>
        /// <param name="item">要添加的对象。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <seealso cref="AddOutOfRange(T)"/>
        public bool Add(T item)
        {
            if (this.Contains(item)) return false;
            else
            {
                this.AddOutOfRange(item);
                return true;
            }
        }

        /// <summary>
        /// 向 <see cref="RangeSet{T}"/> 中添加一个范围。
        /// </summary>
        /// <param name="range">要添加的范围。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="AddRangeInternal(T, T, bool, bool)"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool Add(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            var suitRange = this.rangeInfo.Adapt(range);
            return this.AddRangeInternal(suitRange.Minimum, suitRange.Maximum, suitRange.CanTakeMinimum, suitRange.CanTakeMaximum);
        }

        /// <summary>
        /// 向 <see cref="RangeSet{T}"/> 中添加一个范围。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>一个值，指示操作是否成功</returns>
        /// <exception cref="InvalidRangeException{T}"><paramref name="minimum"/> 大于 <paramref name="maximum"/> 。</exception>
        /// <exception cref="InvalidRangeException{T}">无效的范围。</exception>
        /// <seealso cref="AddRangeInternal(T, T, bool, bool)"/>
        /// <seealso cref="RangeInfo{T}.IsValid(T, T, bool, bool)"/>
        public bool AddRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true)
        {
            if (this.comparer.Compare(minimum, maximum) > 0)
                throw new InvalidRangeException<T>(
                    "最小值大于最大值。",
                    minimum, maximum, canTakeMinimum, canTakeMaximum, this.rangeInfo.Comparison,
                    new ArgumentOutOfRangeException(
                        $"{nameof(minimum)}, {nameof(maximum)}",
                        $"{minimum}, {maximum}",
                        "最小值大于最大值。"
                    )
                );

            if (!this.rangeInfo.IsValid(minimum, maximum, canTakeMinimum, canTakeMaximum))
                throw new InvalidRangeException<T>(
                    minimum, maximum, canTakeMinimum, canTakeMaximum, this.comparer.Compare
                );
            else
            {
                return this.AddRangeInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
            }
        }

        /// <summary>
        /// 子类重写时，提供向 <see cref="RangeSet{T}"/> 中添加一个范围的实现。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>一个值，指示操作是否成功</returns>
        protected virtual bool AddRangeInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            // 计算内部所有与指定范围相交/未相交的范围。
            var dic =
                (from range in this.ranges
                 group range by
                     this.rangeInfo.IsOverlap(
                         minimum, maximum, canTakeMinimum, canTakeMaximum,
                         range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                     ) ? 1 // 相交
                         : (this.rangeInfo.IsNextTo(
                             minimum, maximum, canTakeMinimum, canTakeMaximum,
                             range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                         ) ? 0 // 相邻
                             : -1 // 不相交
                         )
                ).ToDictionary(
                    (group => group.Key),
                    (group => group.ToArray())
                );

            (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) newRange = (minimum, maximum, canTakeMinimum, canTakeMaximum); // 将要添加的范围。
            if (dic.ContainsKey(1))
            { // 含有相交的范围。
                var overlappedRanges = dic[1];
                if (overlappedRanges.Length == 1)
                { // 如果相交的范围是唯一的。
                    var overlappedRange = overlappedRanges[0];
                    if (this.rangeInfo.IsSupersetOf(
                        overlappedRange.Minimum, overlappedRange.Maximum, overlappedRange.CanTakeMinimum, overlappedRange.CanTakeMaximum,
                        minimum, maximum, canTakeMinimum, canTakeMaximum
                    ))
                        // 如果相交的范围为指定范围的超集，则指定范围中的每一个元素都已经存在于范围集中，操作失败。
                        return false;
                }

                // 计算指定范围以及所有与其相交的范围的并集。
                newRange = overlappedRanges.Aggregate(
                    newRange,
                    (seed, range) => this.rangeInfo.Union(
                        seed.minimum, seed.maximum, seed.canTakeMinimum, seed.canTakeMaximum,
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                    )
                );

                // 从内部范围集合中将所有相交的范围删除。
                foreach (var overlappedRange in overlappedRanges)
                    this.ranges.Remove(overlappedRange);
            }
            if (dic.ContainsKey(0))
            { // 含有相邻的范围。
                var nearbyRanges = dic[0];
                // 计算指定范围以及所有与其相邻的范围的并集。
                newRange = nearbyRanges.Aggregate(
                    newRange,
                    (seed, range) => this.rangeInfo.Union(
                        seed.minimum, seed.maximum, seed.canTakeMinimum, seed.canTakeMaximum,
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                    )
                );

                // 从内部范围集合中将所有相邻的范围删除。
                foreach (var nearbyRange in nearbyRanges)
                    this.ranges.Remove(nearbyRange);
            }

            // 向内部范围集合添加新范围。
            this.ranges.Add(this.rangeInfo.Create(newRange));
            return true; // 操作成功。
        }

        /// <summary>
        /// 子类重写时，提供向 <see cref="RangeSet{T}"/> 中添加其表示的范围集合之外的项的实现。
        /// </summary>
        /// <param name="item">要添加的项。</param>
        protected virtual void AddOutOfRange(T item) => this.AddRangeInternal(item, item, true, true);
        #endregion

        #region Remove
        /// <summary>
        /// 从 <see cref="RangeSet{T}"/> 中移除一个对象。
        /// </summary>
        /// <param name="item">要移除的对象。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <seealso cref="RemoveInRange(T)"/>
        public bool Remove(T item)
        {
            if (this.Contains(item))
            {
                return this.RemoveInRange(item);
            }
            else return false;
        }

        /// <summary>
        /// 从 <see cref="RangeSet{T}"/> 中移除一个范围
        /// </summary>
        /// <param name="range">要移除的范围。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        public virtual bool Remove(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            var suitRange = this.rangeInfo.Adapt(range);
            return this.RemoveRangeInternal(suitRange.Minimum, suitRange.Maximum, suitRange.CanTakeMinimum, suitRange.CanTakeMaximum);
        }

        /// <summary>
        /// 从 <see cref="RangeSet{T}"/> 中移除一个范围
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>一个值，指示操作是否成功</returns>
        /// <exception cref="InvalidRangeException{T}"><paramref name="minimum"/> 大于 <paramref name="maximum"/> 。</exception>
        /// <exception cref="InvalidRangeException{T}">无效的范围。</exception>
        /// <seealso cref="RemoveRangeInternal(T, T, bool, bool)"/>
        /// <seealso cref="RangeInfo{T}.IsValid(T, T, bool, bool)"/>
        public bool RemoveRange(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true)
        {
            if (this.comparer.Compare(minimum, maximum) > 0)
                throw new InvalidRangeException<T>(
                    "最小值大于最大值。",
                    minimum, maximum, canTakeMinimum, canTakeMaximum, this.rangeInfo.Comparison,
                    new ArgumentOutOfRangeException(
                        $"{nameof(minimum)}, {nameof(maximum)}",
                        $"{minimum}, {maximum}",
                        "最小值大于最大值。"
                    )
                );

            if (!this.rangeInfo.IsValid(minimum, maximum, canTakeMinimum, canTakeMaximum))
                throw new InvalidRangeException<T>(
                    minimum, maximum, canTakeMinimum, canTakeMaximum, this.comparer.Compare
                );
            else
                return this.RemoveRangeInternal(minimum, maximum, canTakeMinimum, canTakeMaximum);
        }

        /// <summary>
        /// 子类重写时，提供从 <see cref="RangeSet{T}"/> 中移除一个范围的实现。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns>一个值，指示操作是否成功</returns>
        protected virtual bool RemoveRangeInternal(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum)
        {
            // 计算与指定范围相交的范围。
            var overlappedRanges = this.ranges.Where(range =>
                this.rangeInfo.IsOverlap(
                    minimum, maximum, canTakeMinimum, canTakeMaximum,
                    range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                )
            ).ToArray();

            if (overlappedRanges.Length == 0)
                // 不存在相交的范围，则操作失败。
                return false;
            else
            { // 存在相交的范围。
                foreach (var overlappedRange in overlappedRanges)
                {
                    // 从内部范围集合中移除相交的范围。
                    this.ranges.Remove(overlappedRange);

                    if (this.rangeInfo.IsSupersetOf(
                        minimum, maximum, canTakeMinimum, canTakeMaximum,
                        overlappedRange.Minimum, overlappedRange.Maximum, overlappedRange.CanTakeMinimum, overlappedRange.CanTakeMaximum
                    ))
                        // 指定范围是相交范围的超集。
                        continue;
                    else
                    {
                        if (this.rangeInfo.TryExcept(
                            (overlappedRange.Minimum, overlappedRange.Maximum, overlappedRange.CanTakeMinimum, overlappedRange.CanTakeMaximum),
                            (minimum, maximum, canTakeMinimum, canTakeMaximum),
                            out (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum) newRange
                        ))
                            this.ranges.Add(this.rangeInfo.Create(newRange));
                        else
                        { // 指定范围是相交范围的真子集且两者最小/大值都不相等。
                            this.ranges.Add(this.rangeInfo.Create(
                                overlappedRange.Minimum,
                                minimum,
                                overlappedRange.CanTakeMaximum,
                                !canTakeMinimum
                            ));
                            this.ranges.Add(this.rangeInfo.Create(
                                maximum,
                                overlappedRange.Maximum,
                                !canTakeMaximum,
                                overlappedRange.CanTakeMaximum
                            ));
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 子类重写时，提供从 <see cref="RangeSet{T}"/> 中移除其表示的范围集合之内的项的实现。
        /// </summary>
        /// <param name="item">要移除的项。</param>
        protected virtual bool RemoveInRange(T item) => this.RemoveRangeInternal(item, item, true, true);
        #endregion

        /// <summary>
        /// 从 <see cref="RangeSet{T}"/> 中移除所有项。
        /// </summary>
        public virtual void Clear() => this.ranges.Clear();

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 是否包含特定项。
        /// </summary>
        /// <param name="item">要确定的项。</param>
        /// <returns>一个值，指示 <see cref="RangeSet{T}"/> 是否包含特定项。</returns>
        public virtual bool Contains(T item) =>
            this.ranges.Any(range =>
                (range.CanTakeMinimum ?
                    this.comparer.Compare(range.Minimum, item) <= 0 :
                    this.comparer.Compare(range.Minimum, item) < 0
                ) &&
                (range.CanTakeMaximum ?
                    this.comparer.Compare(item, range.Maximum) <= 0 :
                    this.comparer.Compare(item, range.Maximum) < 0
                )
            );

        /// <summary>
        /// 从目标数组的指定索引处开始，将整个 <see cref="RangeSet{T}"/> 复制到兼容的一维数组。
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/> ，它是从 <see cref="RangeSet{T}"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，从此处开始复制。</param>
        public virtual void CopyTo(T[] array, int arrayIndex) => ((IEnumerable<T>)this).ToArray().CopyTo(array, arrayIndex);

        /// <summary>
        /// 获取 <see cref="RangeSet{T}"/> 的枚举数。
        /// </summary>
        /// <returns><see cref="RangeSet{T}"/> 的枚举数。</returns>
        /// <seealso cref="RangeInfo{T}.GetEnumerable(T, T, bool, bool)"/>
        public virtual IEnumerator<T> GetEnumerator()
        {
            if (this.ranges.Count == 0) yield break;
            else
            {
                IEnumerable<IRange<T>> sorted;
                if (this.ranges.Count == 1)
                    sorted = this.ranges;
                else
                {
                    sorted = this.ranges.OrderBy(
                        range => range,
                        Comparer<IRange<T>>.Create(
                            (range1, range2) =>
                            {
                                if (this.comparer.Compare(range1.Minimum, range2.Minimum) == 0)
                                    return range1.CanTakeMinimum ? 1 : -1;
                                else
                                    return this.comparer.Compare(range1.Minimum, range2.Minimum);
                            }
                        )
                    );
                }

                foreach (var range in sorted)
                    foreach (var item in this.rangeInfo.GetEnumerable(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum))
                        yield return item;
            }
        }

        #region ExceptWith
        /// <summary>
        /// 从当前 <see cref="RangeSet{T}"/> 对象中移除指定集合中的所有元素。
        /// </summary>
        /// <param name="other">要从 <see cref="RangeSet{T}"/> 对象中移除的项的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="ExceptWithInternal(IEnumerable{T})"/>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) this.Clear();

            this.ExceptWithInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供从当前 <see cref="RangeSet{T}"/> 对象中移除指定集合中的所有元素的实现。
        /// </summary>
        /// <param name="other">要从 <see cref="RangeSet{T}"/> 对象中移除的项的集合。</param>
        /// <seealso cref="Remove(T)"/>
        protected virtual void ExceptWithInternal(IEnumerable<T> other)
        {
            foreach (var item in other)
                this.Remove(item);
        }

        /// <summary>
        /// 从当前 <see cref="RangeSet{T}"/> 对象中移除指定范围。
        /// </summary>
        /// <param name="range">要从 <see cref="RangeSet{T}"/> 对象中移除的范围。</param>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <see cref="ExceptWithInternal(IRange{T})"/>
        /// <see cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual void ExceptWith(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            this.ExceptWithInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供从当前 <see cref="RangeSet{T}"/> 对象中移除指定范围的实现。
        /// </summary>
        /// <param name="range">要从 <see cref="RangeSet{T}"/> 对象中移除的范围。</param>
        /// <seealso cref="RemoveRangeInternal(T, T, bool, bool)"/>
        protected virtual void ExceptWithInternal(IRange<T> range) =>
            this.RemoveRangeInternal(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum);

        /// <summary>
        /// 从当前 <see cref="RangeSet{T}"/> 对象中移除指定范围集合中的所有元素。
        /// </summary>
        /// <param name="other">要从 <see cref="RangeSet{T}"/> 对象中移除的范围的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <see cref="ExceptWithInternal(IEnumerable{IRange{T}})"/>
        /// <see cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual void ExceptWith(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) this.Clear();

            this.ExceptWithInternal(
                other
                    .Where(range => range != null)
                    .Select(range => this.rangeInfo.Adapt(range))
            );
        }

        /// <summary>
        /// 子类重写时，提供从当前 <see cref="RangeSet{T}"/> 对象中移除指定范围集合中的所有元素的实现。
        /// </summary>
        /// <param name="other">要从 <see cref="RangeSet{T}"/> 对象中移除的范围的集合。</param>
        /// <see cref="ExceptWithInternal(IRange{T})"/>
        protected virtual void ExceptWithInternal(IEnumerable<IRange<T>> other)
        {
            foreach (var range in other)
                this.ExceptWithInternal(range);
        }
        #endregion

        #region IntersectWith
        /// <summary>
        /// 修改当前的 <see cref="RangeSet{T}"/> 对象，以仅包含该对象和指定集合中存在的元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IntersectWithInternal(IEnumerable{T})"/>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return;

            this.IntersectWithInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供修改当前的 <see cref="RangeSet{T}"/> 对象，以仅包含该对象和指定集合中存在的元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <seealso cref="Contains(T)"/>
        /// <seealso cref="Clear"/>
        /// <seealso cref="AddOutOfRange(T)"/>
        protected virtual void IntersectWithInternal(IEnumerable<T> other)
        {
            var intersection =
                other
                    .Distinct(new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0))
                    .Where(item => this.Contains(item)).ToArray();

            this.Clear();
            foreach (var item in intersection)
                this.AddOutOfRange(item);
        }

        /// <summary>
        /// 修改当前的 <see cref="RangeSet{T}"/> 对象，以仅包含该对象和指定范围内的元素。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="IntersectWithInternal(IRange{T})"/>
        public virtual void IntersectWith(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            this.IntersectWithInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供修改当前的 <see cref="RangeSet{T}"/> 对象，以仅包含该对象和指定范围内的元素的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <seealso cref="IntersectWithInternal(IEnumerable{IRange{T}})"/>
        protected virtual void IntersectWithInternal(IRange<T> range) =>
            this.IntersectWithInternal(new[] { range });

        /// <summary>
        /// 修改当前的 <see cref="RangeSet{T}"/> 对象，以仅包含该对象和指定范围集合中存在的元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IntersectWithInternal(IEnumerable{IRange{T}})"/>
        public virtual void IntersectWith(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return;

            this.IntersectWithInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供修改当前的 <see cref="RangeSet{T}"/> 对象，以仅包含该对象和指定集合中存在的元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <seealso cref="Clear"/>
        /// <seealso cref="AddRangeInternal(T, T, bool, bool)"/>
        protected virtual void IntersectWithInternal(IEnumerable<IRange<T>> other)
        {
            var intersection =
                from range1 in this.ranges.ToList() // 将延迟执行的基础集合切断放在这一步。
                from range2 in other
                where this.rangeInfo.IsOverlap(
                    range1.Minimum, range1.Maximum, range1.CanTakeMinimum, range1.CanTakeMaximum,
                    range2.Minimum, range2.Maximum, range2.CanTakeMinimum, range2.CanTakeMaximum
                )
                select this.rangeInfo.Intersect(
                    range1.Minimum, range1.Maximum, range1.CanTakeMinimum, range1.CanTakeMaximum,
                    range2.Minimum, range2.Maximum, range2.CanTakeMinimum, range2.CanTakeMaximum
                );

            this.Clear(); // 清空内部范围集合供容纳并集。

            foreach (var range in intersection)
                this.AddRangeInternal(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
        }
        #endregion

        #region SymmetricExceptWith
        /// <summary>
        /// 修改当前 <see cref="RangeSet{T}"/> 对象以仅包含存在于该对象中或存在于指定集合中的元素（但并非两者）。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="SymmetricExceptWithInternal(IEnumerable{T})"/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) this.Clear();

            this.SymmetricExceptWithInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供修改当前 <see cref="RangeSet{T}"/> 对象以仅包含存在于该对象中或存在于指定集合中的元素（但并非两者）的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <seealso cref="AddOutOfRange(T)"/>
        /// <seealso cref="RemoveInRange(T)"/>
        protected virtual void SymmetricExceptWithInternal(IEnumerable<T> other)
        {
            foreach (var item in other.Distinct(new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0)))
            {
                if (this.Contains(item))
                    this.RemoveInRange(item);
                else
                    this.AddOutOfRange(item);
            }
        }

        /// <summary>
        /// 修改当前 <see cref="RangeSet{T}"/> 对象以仅包含存在于该对象中或存在于指定范围中的元素（但并非两者）
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="SymmetricExceptWith(IRange{T})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual void SymmetricExceptWith(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            this.SymmetricExceptWithInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供修改当前 <see cref="RangeSet{T}"/> 对象以仅包含存在于该对象中或存在于指定范围中的元素（但并非两者）的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <seealso cref="SymmetricExceptWithInternal(IEnumerable{IRange{T}})"/>
        protected virtual void SymmetricExceptWithInternal(IRange<T> range) =>
            this.SymmetricExceptWithInternal(new[] { range });

        /// <summary>
        /// 修改当前 <see cref="RangeSet{T}"/> 对象以仅包含存在于该对象中或存在于指定范围集合中的元素（但并非两者）。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        public virtual void SymmetricExceptWith(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) this.Clear();

            this.SymmetricExceptWithInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供修改当前 <see cref="RangeSet{T}"/> 对象以仅包含存在于该对象中或存在于指定范围集合中的元素（但并非两者）的实现了。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        protected virtual void SymmetricExceptWithInternal(IEnumerable<IRange<T>> other)
        {
            var dic =
                (from range in this.ranges
                 from item in other
                 group new
                 {
                     ThisRange = range, // 内部范围集合中的项。
                     OtherRange = item // 范围集合中的项.
                 } by this.rangeInfo.IsOverlap(
                         range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum,
                         item.Minimum, item.Maximum, item.CanTakeMinimum, item.CanTakeMaximum
                     )
                 )
                 .ToDictionary(
                     (group => group.Key),
                     (group => group.ToArray())
                 );

            if (dic.ContainsKey(true))
            { // 包含重叠范围。
                var intersection = dic[true].Select(pair =>
                    this.rangeInfo.Intersect(
                        (pair.ThisRange.Minimum, pair.ThisRange.Maximum, pair.ThisRange.CanTakeMinimum, pair.ThisRange.CanTakeMaximum),
                        (pair.OtherRange.Minimum, pair.OtherRange.Maximum, pair.OtherRange.CanTakeMinimum, pair.OtherRange.CanTakeMaximum)
                    )
                );
                foreach (var range in intersection)
                    this.RemoveRangeInternal(range.minimum, range.maximum, range.canTakeMinimum, range.canTakeMaximum);
            }
            if (dic.ContainsKey(false))
            { // 包含不重叠范围
                foreach (var range in dic[false].Select(pair => pair.OtherRange))
                    this.AddRangeInternal(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum);
            }
        }
        #endregion

        #region UnionWith
        /// <summary>
        /// 修改当前 <see cref="RangeSet{T}"/> 对象以包含存在于该对象中、指定集合中或两者中的所有元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="UnionWithInternal(IEnumerable{T})"/>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return;

            this.UnionWithInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供修改当前 <see cref="RangeSet{T}"/> 对象以包含存在于该对象中、指定集合中或两者中的所有元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <seealso cref="Add(T)"/>
        protected virtual void UnionWithInternal(IEnumerable<T> other)
        {
            foreach (var item in other)
                this.Add(item);
        }

        /// <summary>
        /// 修改当前 <see cref="RangeSet{T}"/> 对象以包含存在于该对象中、指定范围中或两者中的所有元素。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual void UnionWith(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            this.UnionWith(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供修改当前 <see cref="RangeSet{T}"/> 对象以包含存在于该对象中、指定范围中或两者中的所有元素的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <seealso cref="AddRangeInternal(T, T, bool, bool)"/>
        protected virtual void UnionWithInternal(IRange<T> range) =>
            this.AddRangeInternal(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMinimum);

        /// <summary>
        /// 修改当前 <see cref="RangeSet{T}"/> 对象以包含存在于该对象中、指定范围集合中或两者中的所有元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="UnionWithInternal(IEnumerable{IRange{T}})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual void UnionWith(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return;

            this.UnionWithInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供修改当前 <see cref="RangeSet{T}"/> 对象以包含存在于该对象中、指定范围集合中或两者中的所有元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <seealso cref="UnionWithInternal(IRange{T})"/>
        protected virtual void UnionWithInternal(IEnumerable<IRange<T>> other)
        {
            foreach (var range in other)
                this.UnionWithInternal(range);
        }
        #endregion

        #region IsProperSubsetOf
        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的真子集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsProperSubsetOfInternal(IEnumerable{T})"/>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return false;

            return this.IsProperSubsetOfInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的真子集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真子集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsProperSubsetOfInternal(IEnumerable<T> other)
        {
            var set = new HashSet<T>(other, new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0));
            foreach (var item in this)
                if (!set.Remove(item)) return false;

            return set.Count != 0;
        }

        /// <summary>
        /// 。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的真子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="IsProperSubsetOfInternal(IRange{T})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool IsProperSubsetOf(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            return this.IsProperSubsetOfInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的真子集的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的真子集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="IsProperSubsetOfInternal(IEnumerable{T})"/>
        protected virtual bool IsProperSubsetOfInternal(IRange<T> range) =>
            this.IsProperSubsetOfInternal(this.rangeInfo.GetEnumerable(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum));

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的真子集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsProperSubsetOfInternal(IEnumerable{IRange{T}})"/>
        public virtual bool IsProperSubsetOf(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return false;

            return this.IsProperSubsetOfInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的真子集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真子集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="IsProperSubsetOfInternal(IEnumerable{T})"/>
        /// <seealso cref="RangeInfo{T}.GetEnumerable(T, T, bool, bool)"/>
        protected virtual bool IsProperSubsetOfInternal(IEnumerable<IRange<T>> other) =>
            this.IsProperSubsetOfInternal(
                other.SelectMany(range =>
                    this.rangeInfo.GetEnumerable(
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                    )
                )
            );
        #endregion

        #region IsProperSupersetOf
        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的真超集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsProperSupersetOfInternal(IEnumerable{T})"/>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return false;

            return this.IsProperSupersetOfInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的真超集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真超集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsProperSupersetOfInternal(IEnumerable<T> other)
        {
            var set = new HashSet<T>(this, new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0));
            foreach (var item in other)
                if (!set.Remove(item)) return false;

            return set.Count != 0;
        }

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的真超集。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的真超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="IsProperSupersetOfInternal(IRange{T})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool IsProperSupersetOf(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            return this.IsProperSupersetOfInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的真超集的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的真超集，则为 true ；否则为 false 。</returns>
        /// <seealse cref="IsProperSupersetOfInternal(IEnumerable{T})"/>
        /// <seealso cref="RangeInfo{T}.GetEnumerable(T, T, bool, bool)"/>
        protected virtual bool IsProperSupersetOfInternal(IRange<T> range) =>
            this.IsProperSupersetOfInternal(this.rangeInfo.GetEnumerable(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum));

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的真超集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsProperSupersetOfInternal(IEnumerable{IRange{T}})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool IsProperSupersetOf(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return false;

            return this.IsProperSupersetOfInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的真超集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的真超集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="IsProperSupersetOfInternal(IEnumerable{T})"/>
        /// <seealso cref="RangeInfo{T}.GetEnumerable(T, T, bool, bool)"/>
        protected virtual bool IsProperSupersetOfInternal(IEnumerable<IRange<T>> other) =>
            this.IsProperSupersetOfInternal(
                other.SelectMany(range =>
                    this.rangeInfo.GetEnumerable(
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                    )
                )
            );
        #endregion

        #region IsSubsetOf
        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的子集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsSubsetOfInternal(IEnumerable{T})"/>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return this.IsSubsetOfInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的子集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的子集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsSubsetOfInternal(IEnumerable<T> other)
        {
            var set = other.Distinct(new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0));

            return ((IEnumerable<T>)this).All(item => set.Contains(item));
        }

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的子集。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="IsSubsetOfInternal(IRange{T})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool IsSubsetOf(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            return this.IsSubsetOfInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的子集的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的子集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="IsSubsetOfInternal(IEnumerable{IRange{T}})"/>
        protected virtual bool IsSubsetOfInternal(IRange<T> range) =>
            this.IsSubsetOfInternal(new[] { range });

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的子集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的子集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsSubsetOfInternal(IEnumerable{IRange{T}})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool IsSubsetOf(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return this.IsSubsetOfInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的子集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的子集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="RangeInfo{T}.IsSubsetOf(T, T, bool, bool, T, T, bool, bool)"/>
        protected virtual bool IsSubsetOfInternal(IEnumerable<IRange<T>> other) =>
            this.ranges.All(range =>
                other.Any(item =>
                    this.rangeInfo.IsSubsetOf(
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum,
                        item.Minimum, item.Maximum, item.CanTakeMinimum, item.CanTakeMaximum
                    )
                )
            );
        #endregion

        #region IsSupersetOf
        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的超集。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsSupersetOfInternal(IEnumerable{T})"/>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return this.IsSupersetOfInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定集合的超集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的超集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="Contains(T)"/>
        protected virtual bool IsSupersetOfInternal(IEnumerable<T> other)
        {
            var set = other.Distinct(new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0));

            return set.All(item => this.Contains(item));
        }

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的超集。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="IsSupersetOfInternal(IRange{T})"/>
        public virtual bool IsSupersetOf(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            return this.IsSupersetOfInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围表示的集合的超集的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="range"/> 表示的集合的超集，则为 true ；否则为 false 。</returns>
        /// <seealso cref="IsSupersetOfInternal(IEnumerable{IRange{T}})"/>
        protected virtual bool IsSupersetOfInternal(IRange<T> range) =>
            this.IsSupersetOfInternal(new[] { range });

        /// <summary>
        /// 确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的超集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的超集，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="IsSupersetOfInternal(IEnumerable{IRange{T}})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool IsSupersetOf(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return this.IsSupersetOfInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供确定 <see cref="RangeSet{T}"/> 对象是否为指定范围集合的超集的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象是 <paramref name="other"/> 的超集，则为 true ；否则为 false 。</returns>
        protected virtual bool IsSupersetOfInternal(IEnumerable<IRange<T>> other) =>
            other.All(item =>
                this.ranges.Any(range =>
                    this.RangeInfo.IsSupersetOf(
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum,
                        item.Minimum, item.Maximum, item.CanTakeMinimum, item.CanTakeMaximum
                    )
                )
            );
        #endregion

        #region Overlaps
        /// <summary>
        /// 确定当前的 <see cref="RangeSet{T}"/> 对象和指定的集合是否共享通用元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>
        /// <para>如果 <see cref="RangeSet{T}"/> 对象不含有任何元素，则为 false 。</para>
        /// <para>如果 <see cref="RangeSet{T}"/> 对象和 <paramref name="other"/> 共享至少一个公共元素，则为 true ；否则为 false 。</para></returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="OverlapsInternal(IEnumerable{T})"/>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return this.Count != 0;

            return this.OverlapsInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供确定当前的 <see cref="RangeSet{T}"/> 对象和指定的集合是否共享通用元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象和 <paramref name="other"/> 共享至少一个公共元素，则为 true ；否则为 false 。</returns>
        protected virtual bool OverlapsInternal(IEnumerable<T> other) =>
            other.Any(item => this.Contains(item));

        /// <summary>
        /// 确定当前的 <see cref="RangeSet{T}"/> 对象和指定的范围表示的集合是否共享通用元素。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象和 <paramref name="range"/> 表示的集合共享至少一个公共元素，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> 的值为 null 。</exception>
        /// <seealso cref="OverlapsInternal(IRange{T})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool Overlaps(IRange<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            return this.OverlapsInternal(this.rangeInfo.Adapt(range));
        }

        /// <summary>
        /// 子类重写时，提供确定当前的 <see cref="RangeSet{T}"/> 对象和指定的范围表示的集合是否共享通用元素的实现。
        /// </summary>
        /// <param name="range">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象和 <paramref name="range"/> 表示的集合共享至少一个公共元素，则为 true ；否则为 false 。</returns>
        /// <seealso cref="OverlapsInternal(IEnumerable{IRange{T}})"/>
        protected virtual bool OverlapsInternal(IRange<T> range) =>
            this.OverlapsInternal(new[] { range });

        /// <summary>
        /// 确定当前的 <see cref="RangeSet{T}"/> 对象和指定的范围集合是否共享通用元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象和 <paramref name="other"/> 共享至少一个公共元素，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="OverlapsInternal(IEnumerable{IRange{T}})"/>
        /// <seealso cref="RangeInfo{T}.Adapt(IRange{T})"/>
        public virtual bool Overlaps(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return this.Count != 0;

            return this.OverlapsInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供确定当前的 <see cref="RangeSet{T}"/> 对象和指定的范围集合是否共享通用元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <see cref="RangeSet{T}"/> 对象和 <paramref name="other"/> 共享至少一个公共元素，则为 true ；否则为 false 。</returns>
        /// <seealso cref="RangeInfo{T}.IsOverlap(T, T, bool, bool, T, T, bool, bool)"/>
        protected virtual bool OverlapsInternal(IEnumerable<IRange<T>> other) =>
            this.ranges.Any(range1 =>
                other.Any(range2 =>
                    this.rangeInfo.IsOverlap(
                         range1.Minimum, range1.Maximum, range1.CanTakeMinimum, range1.CanTakeMaximum,
                         range2.Minimum, range2.Maximum, range2.CanTakeMinimum, range2.CanTakeMaximum
                    )
                )
        );
        #endregion

        #region SetEquals
        /// <summary>
        /// 确定是否 <see cref="RangeSet{T}"/> 对象和指定集合包含相同的元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <seealso cref="ranges"/> 对象与 <paramref name="other"/> 相等，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="SetEqualsInternal(IEnumerable{T})"/>
        public virtual bool SetEquals(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return this.SetEqualsInternal(other);
        }

        /// <summary>
        /// 子类重写时，提供确定是否 <see cref="RangeSet{T}"/> 对象和指定集合包含相同的元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的集合。</param>
        /// <returns>如果 <seealso cref="ranges"/> 对象与 <paramref name="other"/> 相等，则为 true ；否则为 false 。</returns>
        protected virtual bool SetEqualsInternal(IEnumerable<T> other)
        {
            IEqualityComparer<T> equalityComparer = new EqualityComparisonComparer<T>((x, y) => this.comparer.Compare(x, y) == 0);

            return ((IEnumerable<T>)this).SequenceEqual(
                other
                    .Distinct(equalityComparer)
                    .OrderBy((item => item), this.comparer),
                equalityComparer
            );
        }

        /// <summary>
        /// 确定是否 <see cref="RangeSet{T}"/> 对象和指定范围集合包含相同的元素。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <seealso cref="ranges"/> 对象与 <paramref name="other"/> 相等，则为 true ；否则为 false 。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> 的值为 null 。</exception>
        /// <seealso cref="SetEqualsInternal(IEnumerable{IRange{T}})"/>
        public virtual bool SetEquals(IEnumerable<IRange<T>> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other == this) return true;

            return this.SetEqualsInternal(other.Where(item => item != null).Select(item => this.rangeInfo.Adapt(item)));
        }

        /// <summary>
        /// 子类重写时，提供确定是否 <see cref="RangeSet{T}"/> 对象和指定范围集合包含相同的元素的实现。
        /// </summary>
        /// <param name="other">要与当前的 <see cref="RangeSet{T}"/> 对象进行比较的范围集合。</param>
        /// <returns>如果 <seealso cref="ranges"/> 对象与 <paramref name="other"/> 相等，则为 true ；否则为 false 。</returns>
        /// <seealso cref="SetEqualsInternal(IEnumerable{T})"/>
        protected virtual bool SetEqualsInternal(IEnumerable<IRange<T>> other) =>
            this.SetEqualsInternal(
                other.SelectMany(range =>
                    this.rangeInfo.GetEnumerable(
                        range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum
                    )
                )
            );
        #endregion

        #region ISet{T} Implementations
        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item) => this.Add(item);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion

        #region ISet{IRange{T}} Implementations
        int ICollection<IRange<T>>.Count => this.ranges.Count;

        bool ICollection<IRange<T>>.IsReadOnly => false;

        void ICollection<IRange<T>>.Add(IRange<T> item) => this.Add(item);

        bool ICollection<IRange<T>>.Contains(IRange<T> item) => this.IsSupersetOf(item);

        void ICollection<IRange<T>>.CopyTo(IRange<T>[] array, int arrayIndex) =>
            this.ranges
                .OrderBy(
                    (range => (range.Minimum, range.CanTakeMinimum)),
                    Comparer<(T minimum, bool canTakeMinimum)>.Create((x, y) =>
                        this.comparer.Compare(
                            x.canTakeMinimum ? x.minimum : this.rangeInfo.GetNext(x.minimum),
                            y.canTakeMinimum ? y.minimum : this.rangeInfo.GetNext(y.minimum)
                        )
                    )
                )
                .ToList()
                .CopyTo(array, arrayIndex);

        IEnumerator<IRange<T>> IEnumerable<IRange<T>>.GetEnumerator() =>
            this.ranges
                .OrderBy(
                    (range => (range.Minimum, range.CanTakeMinimum)),
                    Comparer<(T minimum, bool canTakeMinimum)>.Create((x, y) =>
                        this.comparer.Compare(
                            x.canTakeMinimum ? x.minimum : this.rangeInfo.GetNext(x.minimum),
                            y.canTakeMinimum ? y.minimum : this.rangeInfo.GetNext(y.minimum)
                        )
                    )
                )
                .GetEnumerator();
        #endregion
    }
}
