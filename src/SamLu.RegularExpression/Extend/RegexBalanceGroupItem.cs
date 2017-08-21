using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public abstract class RegexBalanceGroupItem<T> : RegexGroup<T>
    {
        internal RegexBalanceGroup<T> ___balanceGroup;

        public abstract Delegate Method { get; }

        protected RegexBalanceGroupItem(RegexObject<T> regex) :
            base(regex ?? throw new ArgumentNullException(nameof(regex)), null, false)
        { }
    }
}
