using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexBalanceGroupSubItem<T, TSeed> : RegexBalanceGroupItem<T>
    {
        protected Func<TSeed, TSeed> method;
        protected Predicate<TSeed> predicate;
        public sealed override Delegate Method => this.method;

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
