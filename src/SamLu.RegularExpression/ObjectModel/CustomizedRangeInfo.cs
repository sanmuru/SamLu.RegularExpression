using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    /// <summary>
    /// 提供用于创建、合并、求差和检测等操作范围的属性和实例方法的类型的虚基类，并且帮助创建 <see cref="IRange{T}"/> 对象。
    /// </summary>
    /// <typeparam name="T">范围的内容的类型。</typeparam>
    public class CustomizedRangeInfo<T> : RangeInfo<T>
    {
        /// <summary>
        /// 内部获取指定对象的前一个对象的方法。
        /// </summary>
        protected Func<T, T> getPrevFunc;
        /// <summary>
        /// 内部获取指定对象的后一个对象的方法。
        /// </summary>
        protected Func<T, T> getNextFunc;

        /// <summary>
        /// 子类调用的构造函数，使用默认的比较器 <see cref="Comparer{T}.Default"/> 的比较方法 <see cref="Comparer{T}.Compare(T, T)"/> 初始化子类的新实例。
        /// </summary>
        protected CustomizedRangeInfo() : base() { }

        /// <summary>
        /// 初始化 <see cref="CustomizedRangeInfo{T}"/> 类的新实例，该实例使用指定的获取指定对象的前/后一个对象的方法及比较方法。
        /// </summary>
        /// <param name="getPrevFunc">指定的获取指定对象的前一个对象的方法。</param>
        /// <param name="getNextFunc">指定的获取指定对象的后一个对象的方法。</param>
        /// <param name="comparison">指定的比较方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="getPrevFunc"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="getNextFunc"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> 的值为 null 。</exception>
        public CustomizedRangeInfo(Func<T, T> getPrevFunc, Func<T, T> getNextFunc, Comparison<T> comparison) : base(comparison)
        {
            if (getPrevFunc == null) throw new ArgumentNullException(nameof(getPrevFunc));
            if (getNextFunc == null) throw new ArgumentNullException(nameof(getNextFunc));

            this.getPrevFunc = getPrevFunc;
            this.getNextFunc = getNextFunc;
        }

        /// <summary>
        /// 获取指定对象的前一个对象。
        /// </summary>
        /// <param name="value">指定的对象。</param>
        /// <returns>指定对象的前一个对象。</returns>
        /// <exception cref="InvalidOperationException">内部获取指定对象的前一个对象的方法发生错误，无法获取指定对象的前一个对象。</exception>
        public override T GetPrev(T value)
        {
            try { return this.getPrevFunc(value); }
            catch (Exception innerException)
            {
                throw new InvalidOperationException("内部获取指定对象的前一个对象的方法发生错误。", innerException);
            }
        }

        /// <summary>
        /// 获取指定对象的后一个对象。
        /// </summary>
        /// <param name="value">指定的对象。</param>
        /// <returns>指定对象的后一个对象。</returns>
        /// <exception cref="InvalidOperationException">内部获取指定对象的后一个对象的方法发生错误，无法获取指定对象的后一个对象。</exception>
        public override T GetNext(T value)
        {
            try { return this.getNextFunc(value); }
            catch (Exception innerException)
            {
                throw new InvalidOperationException("内部获取指定对象的后一个对象的方法发生错误。", innerException);
            }
        }
    }
}
