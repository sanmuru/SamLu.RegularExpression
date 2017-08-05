using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public class RegexConst<T> : RegexObject<T>
    {
        /// <summary>
        /// 一个默认的常量正则的值相等性比较方法。
        /// </summary>
        public static readonly EqualityComparison<T> DefaultEqualityComparison = EqualityComparer<T>.Default.Equals;

        protected T constValue;

        protected EqualityComparison<T> equalityComparison;

        public T ConstValue => this.constValue;

        public RegexConst(T constValue) : this(constValue, RegexConst<T>.DefaultEqualityComparison) { }

        protected RegexConst(T constValue, EqualityComparison<T> equalityComparison)
        {
            this.constValue = constValue;

            this.equalityComparison = equalityComparison;
        }
        
        public override RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if ((regex is RegexRange<T> range) &&
                (range.Comparison(range.Minimum, this.constValue) <= 0 &&
                range.Comparison(this.constValue, range.Maximum) <= 0)
            )
                return regex;
            else return base.Unions(regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexConst<T>(this.constValue);
        }
    }
}