using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则复数分支对象。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexMultiBranch<T> : RegexObject<T>
    {
        protected RegexMultiBranchBranchCollection<T> branches;
        protected RegexObject<T> otherwisePattern;

        /// <summary>
        /// 获取正则复数分支的所有分支。
        /// </summary>
        public RegexMultiBranchBranchCollection<T> Branches => this.branches;
        /// <summary>
        /// 获取正则复数分支的另外正则模式。
        /// </summary>
        public RegexObject<T> OtherwisePattern => this.otherwisePattern;

        public RegexMultiBranch(RegexObject<T> otherwisePattern) : this(Enumerable.Empty<(RegexMultiBranchBranchPredicate<T> predicate, RegexObject<T> pattern)>(), otherwisePattern) { }

        public RegexMultiBranch(IDictionary<RegexMultiBranchBranchPredicate<T>, RegexObject<T>> branches, RegexObject<T> otherwisePattern)
        {
            if (branches == null) throw new ArgumentNullException(nameof(branches));
            if (otherwisePattern == null) throw new ArgumentNullException(nameof(otherwisePattern));

            this.branches = new RegexMultiBranchBranchCollection<T>(branches);
            this.otherwisePattern = otherwisePattern;
        }

        public RegexMultiBranch(IEnumerable<(RegexMultiBranchBranchPredicate<T> predicate, RegexObject<T> pattern)> branches, RegexObject<T> otherwisePattern) :
            this(
                (branches ?? throw new ArgumentNullException(nameof(branches)))
                    .ToDictionary(
                        (branch => branch.predicate),
                        (branch => branch.pattern)
                    ),
                otherwisePattern
            )
        { }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexMultiBranch<T>(this.branches, this.otherwisePattern);
        }
    }
}
