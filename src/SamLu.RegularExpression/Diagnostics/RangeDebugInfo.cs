using SamLu.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    /// <summary>
    /// 为 <see cref="IRange{T}"/> 及实现其的类型提供调试信息。
    /// </summary>
    /// <typeparam name="T">范围的内容的类型。</typeparam>
    public class RangeDebugInfo<T> : IDebugInfo
    {
        /// <summary>
        /// 要获取调试信息的范围对象。
        /// </summary>
        protected IRange<T> range;
        /// <summary>
        /// 获取调试信息时的参数。
        /// </summary>
        protected object[] args;

        /// <summary>
        /// 获取调试信息。
        /// </summary>
        public virtual string DebugInfo
        {
            get
            {
                if (this.range.Comparison(this.range.Minimum, this.range.Maximum) == 0 && (this.range.CanTakeMinimum && this.range.CanTakeMaximum))
                    return this.range.Minimum.GetDebugInfo();
                else
                    return $"{(this.range.CanTakeMinimum ? '[' : '(')}{this.range.Minimum.GetDebugInfo()},{this.range.Maximum.GetDebugInfo()}{(this.range.CanTakeMaximum ? ']' : ')')}";
            }
        }

        /// <summary>
        /// 此为支持获取调试信息的类型的必要约定。初始化 <see cref="RangeDebugInfo{T}"/> 的新实例。
        /// </summary>
        /// <param name="range">要获取调试信息的范围对象。</param>
        /// <param name="args">获取调试信息时的可选参数。</param>
        public RangeDebugInfo(IRange<T> range, params object[] args)
        {
            this.range = range ?? throw new ArgumentNullException(nameof(range));
            this.args = args;
        }
    }
}
