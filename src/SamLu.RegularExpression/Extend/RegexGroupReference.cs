using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexGroupReference<T> : RegexObject<T>
    {
        private object id;

        public object GroupID => this.id;

        public RegexGroupReference() : this(null) { }

        public RegexGroupReference(object id)
        {
            this.id = id;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexGroupReference<T>(this.id);
        }
    }
}
