using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    /// <summary>
    /// 提供用于创建、合并、求差和检测等操作以 32 位有符号整数为内容的范围的属性和实例方法。
    /// </summary>
    public class Int32RangeInfo : RangeInfo<int>
    {
        /// <summary>
        /// 使用默认的比较器初始化 <see cref="Int32RangeInfo"/> 类的新实例。
        /// </summary>
        public Int32RangeInfo() : base() { }

        /// <summary>
        /// 使用指定的比较方法初始化 <see cref="Int32RangeInfo"/> 类的新实例。
        /// </summary>
        /// <param name="comparison">指定的比较方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> 的值为 null 。</exception>
        protected Int32RangeInfo(Comparison<int> comparison) : base(comparison) { }

        /// <summary>
        /// 提供创建表示指定的范围的 <see cref="Int32Range"/> 的新对象实例的实现。
        /// </summary>
        /// <param name="minimum">范围的最小值。</param>
        /// <param name="maximum">范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示是否能取到范围的最小值。</param>
        /// <param name="canTakeMaximum">一个值，指示是否能取到范围的最大值。</param>
        /// <returns><see cref="Int32Range"/> 的新对象实例，该实例表示指定的范围。</returns>
        protected override IRange<int> CreateInternal(int minimum, int maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            new Int32Range(minimum, maximum, canTakeMinimum, canTakeMaximum, base.comparison);

        /// <summary>
        /// 获取指定对象的前一个对象。
        /// </summary>
        /// <param name="value">指定的对象。</param>
        /// <returns>指定对象的前一个对象。</returns>
        /// <exception cref="InvalidOperationException">当 <paramref name="value"/> 与 <see cref="int.MinValue"/> 相等时，指定对象的前一个对象不存在。</exception>
        public override int GetPrev(int value)
        {
            if (value == int.MinValue) throw new InvalidOperationException();

            return value - 1;
        }

        /// <summary>
        /// 获取指定对象的后一个对象。
        /// </summary>
        /// <param name="value">指定的对象。</param>
        /// <returns>指定对象的后一个对象。</returns>
        /// <exception cref="InvalidOperationException">当 <paramref name="value"/> 与 <see cref="int.MaxValue"/> 相等时，指定对象的后一个对象不存在。</exception>
        public override int GetNext(int value)
        {
            if (value == int.MaxValue) throw new InvalidOperationException();

            return value + 1;
        }
    }
}
