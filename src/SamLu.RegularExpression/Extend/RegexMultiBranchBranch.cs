using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示 <see cref="RegexMultiBranch{T}"/> 的分支。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexMultiBranchBranch<T>
    {
        private RegexMultiBranchBranchPredicate<T> predicate;
        private RegexObject<T> pattern;

        /// <summary>
        /// 获取 <see cref="RegexMultiBranch{T}"/> 的分支的检测条件。
        /// </summary>
        public RegexMultiBranchBranchPredicate<T> Predicate => this.predicate;

        /// <summary>
        /// 获取 <see cref="RegexMultiBranch{T}"/> 的分支的正则模式。
        /// </summary>
        public RegexObject<T> Pattern
        {
            get => this.pattern;
            set => this.pattern = value ?? throw new ArgumentNullException(nameof(value));
        }

        public RegexMultiBranchBranch(RegexMultiBranchBranchPredicate<T> predicate, RegexObject<T> pattern)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            this.predicate = predicate;
            this.pattern = pattern;
        }
    }
}
