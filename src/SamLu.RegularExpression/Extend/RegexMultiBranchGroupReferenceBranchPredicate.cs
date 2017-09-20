using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则复数分支的正则组引用检测条件。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexMultiBranchGroupReferenceBranchPredicate<T> : RegexMultiBranchBranchPredicate<T>
    {
        private RegexGroupReference<T> groupReference;

        /// <summary>
        /// 获取 <see cref="RegexMultiBranchGroupReferenceBranchPredicate{T}"/> 的正则组引用。
        /// </summary>
        public RegexGroupReference<T> GroupReference => this.groupReference;

        public RegexMultiBranchGroupReferenceBranchPredicate(RegexGroupReference<T> groupReference) =>
            this.groupReference = groupReference ?? throw new ArgumentNullException(nameof(groupReference));
    }
}
