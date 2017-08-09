using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexRangeAdaptor<TSource, TTarget> : RegexRange<TTarget>, IAdaptor<TSource, TTarget>, IRange<TSource>
    {
        /// <summary>
        /// 一个默认的范围正则适配器的源大小比较方法。
        /// </summary>
        public static readonly Comparison<TSource> DefaultSourceComparison = Comparer<TSource>.Default.Compare;

        protected TSource sourceMinimum;
        protected TSource sourceMaximum;
        
        protected Comparison<TSource> sourceComparison;

        protected AdaptContextInfo<TSource, TTarget> contextInfo;

        public override TTarget Minimum
        {
            get
            {
                if (this.contextInfo.AlwaysAdaptSource)
                {
                    if (this.contextInfo.TryAdaptSource(this.sourceMinimum, out TTarget target, out Exception innerException))
                        base.minimum = target;
                    else
                        throw new InvalidOperationException("适配源发生错误。", innerException);
                }

                return base.minimum;
            }
        }

        public override TTarget Maximum
        {
            get
            {
                if (this.contextInfo.AlwaysAdaptSource)
                {
                    if (this.contextInfo.TryAdaptSource(this.sourceMaximum, out TTarget target, out Exception innerException))
                        base.maximum = target;
                    else
                        throw new InvalidOperationException("适配源发生错误。", innerException);
                }

                return base.maximum;
            }
        }
        
        public AdaptContextInfo<TSource, TTarget> ContextInfo => this.contextInfo;

        public RegexRangeAdaptor(
            TSource minimum, TSource maximum,
            Func<TSource, TTarget> sourceAdaptor, Func<TTarget, TSource> targetAdaptor,
            bool canTakeMinimum = true, bool canTakeMaximum = true
        ) : this(
            minimum, maximum,
            sourceAdaptor, targetAdaptor,
            canTakeMinimum, canTakeMaximum,
            RegexRangeAdaptor<TSource, TTarget>.DefaultSourceComparison
        )
        { }

        public RegexRangeAdaptor(
            TSource minimum, TSource maximum,
            Func<TSource, TTarget> sourceAdaptor, Func<TTarget, TSource> targetAdaptor,
            bool canTakeMinimum, bool canTakeMaximum,
            Comparison<TSource> comparison) :
            this(
                minimum, maximum,
                canTakeMinimum, canTakeMaximum,
                comparison,
                new AdaptContextInfo<TSource, TTarget>(
                    sourceAdaptor ?? throw new ArgumentNullException(nameof(sourceAdaptor)),
                    targetAdaptor ?? throw new ArgumentNullException(nameof(targetAdaptor))
                )
            )
        { }

        public RegexRangeAdaptor(
            TSource minimum, TSource maximum,
            bool canTakeMinimum, bool canTakeMaximum,
            Comparison<TSource> comparison,
            AdaptContextInfo<TSource,TTarget> contextInfo
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
                Exception innerException;

                if (this.contextInfo.TryAdaptSource(this.sourceMinimum, out TTarget targetMinimum, out innerException))
                    base.minimum = targetMinimum;
                else
                    throw new InvalidOperationException("在初始化常量时适配源发生错误", innerException);

                if (this.contextInfo.TryAdaptSource(this.sourceMinimum, out TTarget targetMaximum, out innerException))
                    base.maximum = targetMaximum;
                else
                    throw new InvalidOperationException("在初始化常量时适配源发生错误", innerException);
            }

            base.condition =
                target =>
                {
                    if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                    {
                        return (
                            (base.canTakeMinimum ?
                                this.sourceComparison(this.sourceMinimum, source) <= 0 :
                                this.sourceComparison(this.sourceMinimum, source) < 0
                            ) &&
                            (base.canTakeMaximum ?
                                this.sourceComparison(source, this.sourceMaximum) <= 0 :
                                this.sourceComparison(source, this.sourceMaximum) < 0
                            )
                        );
                    }
                    else return false;
                };
        }

        #region IRange{TSource} Implementations
        TSource IRange<TSource>.Minimum => this.sourceMinimum;
        TSource IRange<TSource>.Maximum => this.sourceMaximum;

        Comparison<TSource> IRange<TSource>.Comparison => this.sourceComparison;
        #endregion
    }
}
