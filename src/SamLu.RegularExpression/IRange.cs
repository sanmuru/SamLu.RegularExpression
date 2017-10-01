using SamLu.Diagnostics;
using SamLu.RegularExpression.Diagnostics;
using SamLu.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示范围的对象应遵循的约束。
    /// </summary>
    /// <typeparam name="T">范围的内容的类型。</typeparam>
    [DebugInfoProxy(typeof(RangeDebugInfo<>), new[] { TypeParameterFillin.TypeParameter_1 })]
    public interface IRange<T>
    {
        /// <summary>
        /// 获取范围的最小值。
        /// </summary>
        T Minimum { get; }
        /// <summary>
        /// 获取范围的最大值。
        /// </summary>
        T Maximum { get; }

        /// <summary>
        /// 获取一个值，指示范围是否能取到最小值。
        /// </summary>
        bool CanTakeMinimum { get; }
        /// <summary>
        /// 获取一个值，指示范围是否能取到最大值。
        /// </summary>
        bool CanTakeMaximum { get; }

        /// <summary>
        /// 获取范围指定的比较方法。
        /// </summary>
        Comparison<T> Comparison { get; }
    }
}
