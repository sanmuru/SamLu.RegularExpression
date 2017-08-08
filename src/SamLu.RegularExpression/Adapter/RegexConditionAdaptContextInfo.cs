using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexConditionAdaptContextInfo<TSource, TTarget>
    {
        public Func<TSource, TTarget> SourceSelector { get; protected set; }

        public Func<TTarget, TSource> TargetSelector { get; protected set; }

        public AdaptOption AdaptOption { get; protected set; }

        public bool OnlyAdaptConstSourceWhenInitialization =>
            (this.AdaptOption & AdaptOption.OnlyAdaptConstSourceWhenInitialization) != 0;

        public bool AlwaysAdaptSource => !this.OnlyAdaptConstSourceWhenInitialization;

        public RegexConditionAdaptContextInfo(Func<TSource, TTarget> sourceSelector, Func<TTarget, TSource> targetSelector)
        {
            this.SourceSelector = sourceSelector ?? (source => default(TTarget));
            this.TargetSelector = targetSelector ?? (target => default(TSource));
        }
    }
}
