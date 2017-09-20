using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则三目对象。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexTernary<T> : RegexMultiBranch<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexTernary{T}"/> 的检测条件。
        /// </summary>
        public RegexMultiBranchBranchPredicate<T> Condition => ((IEnumerable<RegexMultiBranchBranch<T>>)base.branches).First().Predicate;

        /// <summary>
        /// 获取 <see cref="RegexTernary{T}"/> 的真条件正则模式。
        /// </summary>
        public RegexObject<T> TruePattern => ((IEnumerable<RegexMultiBranchBranch<T>>)base.branches).First().Pattern;

        /// <summary>
        /// 获取 <see cref="RegexTernary{T}"/> 的假条件正则模式。
        /// </summary>
        public RegexObject<T> FalsePattern => base.otherwisePattern;

        public RegexTernary(RegexMultiBranchBranchPredicate<T> condition, RegexObject<T> truePattern, RegexObject<T> falsePattern) :
            base(
                new[]
                {
                    (
                        condition ?? throw new ArgumentNullException(nameof(condition)),
                        truePattern ?? throw new ArgumentNullException(nameof(truePattern))
                    )
                },
                falsePattern ?? throw new ArgumentNullException(nameof(falsePattern))
            )
        { }
    }
}
