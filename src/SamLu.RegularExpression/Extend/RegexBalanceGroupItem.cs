using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则平衡组的项。所有提供正则平衡组项支持的正则模块应继承此类。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public abstract class RegexBalanceGroupItem<T> : RegexGroup<T>
    {
        internal RegexBalanceGroup<T> ___balanceGroup;

        /// <summary>
        /// 获取正则平衡组的项在符合条件时执行的方法。
        /// </summary>
        public abstract Delegate Method { get; }

        protected RegexBalanceGroupItem(RegexObject<T> regex) :
            base(regex ?? throw new ArgumentNullException(nameof(regex)), null, false)
        { }
    }
}
