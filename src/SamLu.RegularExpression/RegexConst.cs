using SamLu.RegularExpression.StateMachine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示常量正则。匹配单个输入对象是否与内部常量对象相等。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public sealed class RegexConst<T> : RegexRange<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexConst{T}"/> 类的新实例。该实例使用指定的对象作为内部储存的常量对象以及默认的常量正则的值相等性比较方法。
        /// </summary>
        /// <param name="provider">提供操作正则对象的方法的对象。</param>
        /// <param name="constValue">指定的对象。</param>
        public RegexConst(RegexProvider<T> provider, T? constValue) : base(provider, constValue, constValue, canTakeMinimum: true, canTakeMaximum: true) { }

        /// <inheritdoc/>
        protected internal override RegexConst<T> Clone() => this.provider.CreateRegexObject(this, this.minimum);

        /// <summary>
        /// 获取常量正则对象的字符串表示。
        /// </summary>
        /// <returns>常量正则对象的字符串表示。</returns>
        public override string? ToString() => this.minimum?.ToString();

        /// <inheritdoc/>
        public override (RegexTransition<T> start, RegexTransition<T> end) GenerateStateMachineSegment()
        {
            var transition = this.provider.CreateRegexTransition((input, inputSymbols) => inputSymbols.Equals(input, this.minimum));
            return (transition, transition);
        }
    }
}