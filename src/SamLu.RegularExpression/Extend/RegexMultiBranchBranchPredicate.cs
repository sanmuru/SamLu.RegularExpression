using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示 <see cref="RegexMultiBranchBranch{T}"/> 的检测条件。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public abstract class RegexMultiBranchBranchPredicate<T> : IRegexAnchorPoint<T> { }
}
