using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexConstAdaptor<TSource, TTarget> : RegexConst<TTarget>
    {
        /// <summary>
        /// 一个默认的常量正则适配器的源相等性比较方法。
        /// </summary>
        public static readonly EqualityComparison<TSource> DefaultSourceEqualityComparison = EqualityComparer<TSource>.Default.Equals;

        protected TSource sourceConstValue;

        protected EqualityComparison<TSource> sourceEqualityComparison;

        protected RegexConditionAdaptContextInfo<TSource, TTarget> contextInfo;

        public override TTarget ConstValue =>
            this.contextInfo.AlwaysAdaptSource ?
                (base.constValue = this.contextInfo.SourceSelector(this.sourceConstValue)) :
                base.constValue;
        
        public RegexConstAdaptor(TSource constValue, Func<TSource, TTarget> sourceSelector, Func<TTarget, TSource> targetSelector) : this(constValue, sourceSelector, targetSelector, RegexConstAdaptor<TSource, TTarget>.DefaultSourceEqualityComparison) { }

        public RegexConstAdaptor(TSource constValue, Func<TSource, TTarget> sourceSelector, Func<TTarget, TSource> targetSelector, EqualityComparison<TSource> equalityComparison) :
            this(
                constValue,
                equalityComparison,
                new RegexConditionAdaptContextInfo<TSource, TTarget>(
                    sourceSelector ?? throw new ArgumentNullException(nameof(sourceSelector)),
                    targetSelector ?? throw new ArgumentNullException(nameof(targetSelector))
                )
            )
        { }

        public RegexConstAdaptor(TSource constValue, EqualityComparison<TSource> equalityComparison, RegexConditionAdaptContextInfo<TSource,TTarget> contextInfo)
        {
            if (equalityComparison == null) throw new ArgumentNullException(nameof(equalityComparison));
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.sourceConstValue = constValue;
            this.sourceEqualityComparison = equalityComparison;
            this.contextInfo = contextInfo;

            base.condition = target => this.sourceEqualityComparison(
                this.sourceConstValue, 
                this.contextInfo.TargetSelector(target)
            );

            if (contextInfo.OnlyAdaptConstSourceWhenInitialization)
                base.constValue = this.contextInfo.SourceSelector(constValue);
        }
    }
}
