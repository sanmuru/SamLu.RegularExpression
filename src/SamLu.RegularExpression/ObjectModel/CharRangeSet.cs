﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    /// <summary>
    /// 内部以字符范围为元数据组织的集。
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class CharRangeSet : RangeSet<char>
    {
        /// <summary>
        /// 获取 <see cref="CharRangeSet"/> 中包含的元素数。
        /// </summary>
        public override int Count =>
            base.ranges.Count == 0 ?
                0 :
                base.ranges.Sum(range =>
                    (range.Maximum - range.Minimum + 1) -
                        ((range.CanTakeMinimum ? 0 : 1) + (range.CanTakeMaximum ? 0 : 1))
                );

        /// <summary>
        /// 初始化 <see cref="CharRangeSet"/> 的新实例。
        /// </summary>
        public CharRangeSet() : base(new CharRangeInfo()) { }
    }
}
