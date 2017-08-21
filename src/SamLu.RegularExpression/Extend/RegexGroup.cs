using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public abstract class RegexGroup<T> : RegexObject<T>
    {
        private static int nextID = 0;
        public static int NextID => RegexGroup<T>.nextID++;

        protected RegexObject<T> innerRegex;
        protected object id;
        protected bool isCaptive;

        public RegexObject<T> InnerRegex => this.innerRegex;
        public object ID => this.id;
        public virtual bool IsCaptive => this.isCaptive;

        public RegexGroup(RegexObject<T> regex, bool isCaptive = true) : this(regex, RegexGroup<T>.NextID, isCaptive) { }

        public RegexGroup(RegexObject<T> regex, object id, bool isCaptive)
        {
            this.innerRegex = regex;
            this.id = id;
            this.isCaptive = isCaptive;
        }

    }
}
