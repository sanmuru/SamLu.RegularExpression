using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexBalanceGroupOpenItem<T, TSeed> : RegexBalanceGroupItem<T>
    {
        protected Func<TSeed> method;

        public override Delegate Method => this.method;

        public RegexBalanceGroupOpenItem(RegexObject<T> regex, Func<TSeed> method) : base(regex) =>
            this.method = method ?? throw new ArgumentNullException(nameof(method));

        protected internal override RegexObject<T> Clone()
        {
            return new RegexBalanceGroupOpenItem<T, TSeed>(base.innerRegex, this.method);
        }
    }
}
