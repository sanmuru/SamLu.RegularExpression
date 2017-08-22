using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexBalanceGroupCloseItem<T, TSeed> : RegexBalanceGroupItem<T>
    {
        protected Predicate<TSeed> method;

        public sealed override Delegate Method => this.method;

        public RegexBalanceGroupCloseItem(RegexObject<T> regex, Predicate<TSeed> method) : base(regex) =>
            this.method = method ?? throw new ArgumentNullException(nameof(method));

        protected internal override RegexObject<T> Clone()
        {
            return new RegexBalanceGroupCloseItem<T, TSeed>(base.innerRegex, this.method);
        }
    }
}
