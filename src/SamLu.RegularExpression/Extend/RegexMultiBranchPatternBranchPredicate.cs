using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则复数分支的正则模式检测条件。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexMultiBranchPatternBranchPredicate<T> : RegexMultiBranchBranchPredicate<T>
    {
        private RegexObject<T> pattern;

        /// <summary>
        /// 获取 <see cref="RegexMultiBranchPatternBranchPredicate{T}"/> 的正则模式。
        /// </summary>
        public RegexObject<T> Pattern => this.pattern;

        public RegexMultiBranchPatternBranchPredicate(RegexObject<T> pattern) =>
            this.pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
    }
}
