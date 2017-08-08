using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexConditionAdaptor<TSource, TTarget> : RegexCondition<TTarget>
    {
        protected Predicate<TSource> sourceCondition;

        protected RegexConditionAdaptContextInfo<TSource, TTarget> contextInfo;

        public RegexConditionAdaptor(Predicate<TSource> condition, Func<TTarget, TSource> targetSelector) :
            this(
                condition,
                new RegexConditionAdaptContextInfo<TSource, TTarget>(
                    null,
                    targetSelector ?? throw new ArgumentNullException(nameof(targetSelector))
                )
            )
        { }

        public RegexConditionAdaptor(Predicate<TSource> condition, RegexConditionAdaptContextInfo<TSource, TTarget> contextInfo) : base()
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.sourceCondition = condition;
            this.contextInfo = contextInfo;

            base.condition = target => this.sourceCondition(this.contextInfo.TargetSelector(target));
        }
    }
}
