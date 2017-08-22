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
        private RegexGroup<T> group;

        public object GroupID => this.id;
        public RegexGroup<T> Group
        {
            get
            {
                if (this.IsDetermined) return this.group;
                else
                    throw new InvalidOperationException();
            }
        }
        public bool IsDetermined => this.group != null;

        public RegexGroupReference() : this(null) { }

        public RegexGroupReference(object id)=>
            this.id = id ?? throw new ArgumentNullException(nameof(id));

        public RegexGroupReference(RegexGroup<T> group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            this.group = group;
            this.id = group.ID;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexGroupReference<T>(this.id);
        }
    }
}
