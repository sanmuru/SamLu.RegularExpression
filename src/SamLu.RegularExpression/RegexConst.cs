using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示常量正则。匹配单个输入对象是否与内部常量对象相等。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexConst<T> : RegexCondition<T>, IRange<T>
    {
        /// <summary>
        /// 一个默认的常量正则的值相等性比较方法。
        /// </summary>
        public static readonly EqualityComparison<T> DefaultEqualityComparison = EqualityComparer<T>.Default.Equals;

        /// <summary>
        /// 常量正则内部储存的常量对象。
        /// </summary>
        protected T constValue;

        /// <summary>
        /// 常量正则内部的值相等性比较方法。
        /// </summary>
        protected EqualityComparison<T> equalityComparison;

        /// <summary>
        /// 获取常量正则内部储存的常量对象。
        /// </summary>
        public virtual T ConstValue => this.constValue;

        /// <summary>
        /// 获取常量正则内部的值相等性比较方法。
        /// </summary>
        public virtual EqualityComparison<T> EqualityComparison => this.equalityComparison;
        
        /// <summary>
        /// 初始化 <see cref="RegexConst{T}"/> 类的新实例。子类继承的默认构造函数。
        /// </summary>
        protected RegexConst() : base() { }

        /// <summary>
        /// 初始化 <see cref="RegexConst{T}"/> 类的新实例。该实例使用指定的对象作为内部储存的常量对象以及默认的常量正则的值相等性比较方法。
        /// </summary>
        /// <param name="constValue">指定的对象。</param>
        public RegexConst(T constValue) : this(constValue, RegexConst<T>.DefaultEqualityComparison) { }

        /// <summary>
        /// 初始化 <see cref="RegexConst{T}"/> 类的新实例。该实例使用指定的对象作为内部储存的常量对象以及指定的常量正则的值相等性比较方法。
        /// </summary>
        /// <param name="constValue">指定的对象。</param>
        /// <param name="equalityComparison">指定的常量正则的值相等性比较方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="equalityComparison"/> 的值为 null 。</exception>
        public RegexConst(T constValue, EqualityComparison<T> equalityComparison) : this()
        {
            if (equalityComparison == null) throw new ArgumentNullException(nameof(equalityComparison));

            this.constValue = constValue;

            this.equalityComparison = equalityComparison;
            base.condition = t => equalityComparison(constValue, t);
        }

        /// <summary>
        /// 将此常量正则与另一个正则对象并联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="RegexObject{T}.Unions(RegexObject{T})"/>
        public override RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if ((regex is RegexRange<T> range) &&
                (range.Comparison(range.Minimum, this.ConstValue) <= 0 &&
                range.Comparison(this.ConstValue, range.Maximum) <= 0)
            )
                return regex;
            else return base.Unions(regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexConst<T>(this.constValue, this.equalityComparison);
        }

        public override string ToString()
        {
            return this.ConstValue.ToString();
        }

        #region IRange{T} Implementations
        T IRange<T>.Minimum => this.ConstValue;
        T IRange<T>.Maximum => this.ConstValue;

        bool IRange<T>.CanTakeMaximum => true;
        bool IRange<T>.CanTakeMinimum => true;

        private Comparison<T> comparison;
        Comparison<T> IRange<T>.Comparison =>
            this.comparison ??
                ((x, y) =>
                    this.EqualityComparison(x, y) ? 0 :
                        RegexRange<T>.DefaultComparison(x, y)
                );
        #endregion
    }
}