using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    public class RegexCondition<T> : RegexObject<T>
    {
        private Predicate<T> condition;
        public Predicate<T> Condition => this.condition;

        public RegexCondition(Predicate<T> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            this.condition = condition;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexCondition<T>(this.condition);
        }
    }
}