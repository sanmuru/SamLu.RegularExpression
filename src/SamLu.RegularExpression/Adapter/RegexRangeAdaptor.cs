using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexRangeAdaptor<TSource, TTarget> : RegexRange<TTarget>
    {
        /// <summary>
        /// 一个默认的范围正则适配器的源大小比较方法。
        /// </summary>
        public static readonly Comparison<TSource> DefaultSourceComparison = Comparer<TSource>.Default.Compare;

        protected TSource sourceMinimum;
        protected TSource sourceMaximum;
        
        protected Comparison<TSource> sourceComparison;

        protected RegexConditionAdaptContextInfo<TSource, TTarget> contextInfo;

        public override TTarget Minimum =>
            this.contextInfo.AlwaysAdaptSource ?
                (base.minimum = this.contextInfo.SourceSelector(this.sourceMinimum)) :
                base.minimum;

        public override TTarget Maximum =>
            this.contextInfo.AlwaysAdaptSource ?
                (base.maximum = this.contextInfo.SourceSelector(this.sourceMaximum)) :
                base.maximum;

        public RegexRangeAdaptor(
            TSource minimum, TSource maximum,
            Func<TSource, TTarget> sourceSelector, Func<TTarget, TSource> targetSelector,
            bool canTakeMinimum = true, bool canTakeMaximum = true
        ) : this(
            minimum, maximum,
            sourceSelector, targetSelector,
            canTakeMinimum, canTakeMaximum,
            RegexRangeAdaptor<TSource, TTarget>.DefaultSourceComparison
        )
        { }

        public RegexRangeAdaptor(
            TSource minimum, TSource maximum,
            Func<TSource, TTarget> sourceSelector, Func<TTarget, TSource> targetSelector,
            bool canTakeMinimum, bool canTakeMaximum,
            Comparison<TSource> comparison) :
            this(
                minimum, maximum,
                canTakeMinimum, canTakeMaximum,
                comparison,
                new RegexConditionAdaptContextInfo<TSource, TTarget>(
                    sourceSelector ?? throw new ArgumentNullException(nameof(sourceSelector)),
                    targetSelector ?? throw new ArgumentNullException(nameof(targetSelector))
                )
            )
        { }

        public RegexRangeAdaptor(
            TSource minimum, TSource maximum,
            bool canTakeMinimum, bool canTakeMaximum,
            Comparison<TSource> comparison,
            RegexConditionAdaptContextInfo<TSource,TTarget> contextInfo
        )
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            if (comparison(minimum, maximum) > 0)
                throw new ArgumentOutOfRangeException(
                    string.Format("{0}, {1}", nameof(minimum), nameof(maximum)),
                    string.Format("{0}, {1}", minimum, maximum),
                    string.Format("范围最小值不能大于最大值。")
                );
            else if ((!this.canTakeMinimum || !this.canTakeMaximum) && comparison(minimum, maximum) == 0)
                throw new ArgumentOutOfRangeException(
                    string.Format("{0}, {1}", nameof(minimum), nameof(maximum)),
                    string.Format("{0}, {1}", minimum, maximum),
                    string.Format("无法构成有效范围。")
                );

            this.sourceMinimum = minimum;
            this.sourceMaximum = maximum;

            base.canTakeMinimum = canTakeMinimum;
            base.canTakeMaximum = canTakeMaximum;

            this.sourceComparison = comparison;
            this.contextInfo = contextInfo;

            if (contextInfo.OnlyAdaptConstSourceWhenInitialization)
            {
                base.minimum = this.contextInfo.SourceSelector(this.sourceMinimum);
                base.maximum = this.contextInfo.SourceSelector(this.sourceMaximum);
            }
            
            base.condition =
                target =>
                    (base.canTakeMinimum ?
                        this.sourceComparison(this.sourceMinimum, this.contextInfo.TargetSelector(target)) <= 0 :
                        this.sourceComparison(this.sourceMinimum, this.contextInfo.TargetSelector(target)) < 0
                    ) &&
                    (base.canTakeMaximum ?
                        this.sourceComparison(this.contextInfo.TargetSelector(target), this.sourceMaximum) <= 0 :
                        this.sourceComparison(this.contextInfo.TargetSelector(target), this.sourceMaximum) < 0
                    );

        }
    }
}
