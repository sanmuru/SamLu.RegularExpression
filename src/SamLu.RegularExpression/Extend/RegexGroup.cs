using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexGroup<T> : RegexObject<T>
    {
        private static int nextID = 0;
        public static int NextID => RegexGroup<T>.nextID++;

        protected RegexObject<T> innerRegex;
        protected object id;
        protected bool isCaptive;

        public RegexObject<T> InnerRegex => this.innerRegex;
        public object ID => this.id;
        public virtual bool IsCaptive => this.isCaptive;
        public bool IsAnonymous => this.id == null;

        public RegexGroup(RegexObject<T> regex, bool isCaptive = true) : this(regex, null, isCaptive) { }

        public RegexGroup(RegexObject<T> regex, object id, bool isCaptive)
        {
            this.innerRegex = regex;
            this.id = id;
            this.isCaptive = isCaptive;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexGroup<T>(this.innerRegex, this.id, this.isCaptive);
        }
    }
}
