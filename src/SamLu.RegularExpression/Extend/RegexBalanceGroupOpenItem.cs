using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则平衡组的开始项。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    /// <typeparam name="TSeed">正则平衡组的开始项的内部累加器参数的类型。</typeparam>
    public class RegexBalanceGroupOpenItem<T, TSeed> : RegexBalanceGroupItem<T>
    {
        protected Func<TSeed> method;

        /// <summary>
        /// 获取正则平衡组的开始项在符合条件时执行的方法。
        /// </summary>
        public sealed override Delegate Method => this.method;

        public RegexBalanceGroupOpenItem(RegexObject<T> regex, Func<TSeed> method) : base(regex) =>
            this.method = method ?? throw new ArgumentNullException(nameof(method));

        protected internal override RegexObject<T> Clone()
        {
            return new RegexBalanceGroupOpenItem<T, TSeed>(base.innerRegex, this.method);
        }
    }
}
