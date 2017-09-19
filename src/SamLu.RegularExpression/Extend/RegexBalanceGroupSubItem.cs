using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则平衡组的子项。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    /// <typeparam name="TSeed">正则平衡组的子项的内部累加器参数的类型。</typeparam>
    public class RegexBalanceGroupSubItem<T, TSeed> : RegexBalanceGroupItem<T>
    {
        protected Func<TSeed, TSeed> method;
        protected Predicate<TSeed> predicate;

        /// <summary>
        /// 获取正则平衡组的子项在符合条件时执行的方法。
        /// </summary>
        public sealed override Delegate Method => this.method;

        /// <summary>
        /// 获取正则平衡组的子项的检测内部累加器参数是否符合条件的方法。
        /// </summary>
        public Predicate<TSeed> Predicate => this.predicate;

        public RegexBalanceGroupSubItem(RegexObject<T> regex, Func<TSeed, TSeed> method, Predicate<TSeed> predicate = null) : base(regex)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.predicate = predicate;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexBalanceGroupSubItem<T, TSeed>(base.innerRegex, this.method, this.predicate);
        }
    }
}
