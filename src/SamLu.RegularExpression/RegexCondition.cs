using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示条件正则。所有提供对接受对象进行直接检测支持的正则模块应继承此类。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexCondition<T> : RegexObject<T>
    {
        protected Predicate<T> condition;
        /// <summary>
        /// 获取条件正则内部对接受对象进行直接检测的方法。
        /// </summary>
        public virtual Predicate<T> Condition => this.condition;

        protected RegexCondition() { }

        /// <summary>
        /// 使用指定检测方法初始化 <see cref="RegexCondition{T}"/> 类的新实例。
        /// </summary>
        /// <param name="condition">一个方法，该方法对接受对象进行直接检测。</param>
        public RegexCondition(Predicate<T> condition) : this()
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            this.condition = condition;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexCondition<T>(this.condition);
        }
    }
}