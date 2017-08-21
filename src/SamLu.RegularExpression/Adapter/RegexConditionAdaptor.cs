using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexConditionAdaptor<TSource, TTarget> : RegexCondition<TTarget>, IAdaptor<TSource, TTarget>
    {
        protected Predicate<TSource> sourceCondition;

        protected AdaptContextInfo<TSource, TTarget> contextInfo;
        
        public AdaptContextInfo<TSource, TTarget> ContextInfo => this.contextInfo;

        public RegexConditionAdaptor(Predicate<TSource> condition, AdaptDelegate<TTarget, TSource> targetAdaptor) :
            this(
                condition,
                new AdaptContextInfo<TSource, TTarget>(
                    null,
                    targetAdaptor ?? throw new ArgumentNullException(nameof(targetAdaptor))
                )
            )
        { }

        public RegexConditionAdaptor(Predicate<TSource> condition, AdaptContextInfo<TSource, TTarget> contextInfo) : base()
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.sourceCondition = condition;
            this.contextInfo = contextInfo;

            base.condition =
                target =>
                {
                    if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                        return this.sourceCondition(source);
                    else return false;
                };
        }
    }
}
