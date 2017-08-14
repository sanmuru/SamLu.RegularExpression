using SamLu.Diagnostics;
using SamLu.RegularExpression.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    /// <summary>
    /// 为 <see cref="RangeSet{T}"/> 及其子类提供调试信息。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeSetDebugInfo<T> : IDebugInfo
    {
        private RangeSet<T> rangeSet;

        /// <summary>
        /// 获取调试信息。
        /// </summary>
        public string DebugInfo =>
            string.Join("∪", this.rangeSet.Ranges.Select(range => range.GetDebugInfo()));

        /// <summary>
        /// 此为支持获取调试信息的类型的必要约定。初始化 <see cref="RangeSetDebugInfo{T}"/> 的新实例。
        /// </summary>
        /// <param name="rangeSet">要获取调试信息的范围集对象。</param>
        /// <param name="args">获取调试信息时的可选参数。</param>
        public RangeSetDebugInfo(RangeSet<T> rangeSet, params object[] args) =>
            this.rangeSet = rangeSet ?? throw new ArgumentNullException(nameof(rangeSet));
    }
}
