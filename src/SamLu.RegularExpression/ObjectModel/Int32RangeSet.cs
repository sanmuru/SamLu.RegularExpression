using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    /// <summary>
    /// 内部以 32 位有符号整数范围为元数据组织的集。
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class Int32RangeSet : RangeSet<int>
    {
        /// <summary>
        /// 获取 <see cref="Int32RangeSet"/> 中包含的元素数。
        /// </summary>
        public override int Count =>
            base.ranges.Count == 0 ?
                0 :
                base.ranges.Sum(range =>
                    (range.Maximum - range.Minimum + 1) -
                        ((range.CanTakeMinimum ? 0 : 1) + (range.CanTakeMaximum ? 0 : 1))
                );

        /// <summary>
        /// 初始化 <see cref="Int32RangeSet"/> 的新实例。
        /// </summary>
        public Int32RangeSet() : base(new Int32RangeInfo()) { }

        /// <summary>
        /// 初始化 <see cref="Int32RangeSet"/> 类的新实例，该实例包含从指定的集合复制的元素。
        /// </summary>
        /// <param name="collection">其元素被复制到 <see cref="Int32RangeSet"/> 的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 的值为 null 。</exception>
        public Int32RangeSet(ICollection<int> collection) : base(collection, new Int32RangeInfo()) { }

        /// <summary>
        /// 初始化 <see cref="Int32RangeSet"/> 类的新实例，该实例包含从指定的范围集合复制的元素。
        /// </summary>
        /// <param name="collection">其元素被复制到 <see cref="Int32RangeSet"/> 的范围集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 的值为 null 。</exception>
        public Int32RangeSet(ICollection<IRange<int>> collection) : base(collection, new Int32RangeInfo()) { }
    }
}
