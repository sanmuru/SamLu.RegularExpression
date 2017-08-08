using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class RegexConstAdaptor<TSource, TTarget> : RegexConst<TTarget>, IAdaptor<TSource, TTarget>
    {
        /// <summary>
        /// 一个默认的常量正则适配器的源相等性比较方法。
        /// </summary>
        public static readonly EqualityComparison<TSource> DefaultSourceEqualityComparison = EqualityComparer<TSource>.Default.Equals;

        protected TSource sourceConstValue;

        protected EqualityComparison<TSource> sourceEqualityComparison;

        protected AdaptContextInfo<TSource, TTarget> contextInfo;

        public override TTarget ConstValue
        {
            get
            {
                if (this.contextInfo.AlwaysAdaptSource)
                {
                    if (this.contextInfo.TryAdaptSource(this.sourceConstValue, out TTarget target, out Exception innerException))
                        base.constValue = target;
                    else
                        throw new InvalidOperationException("适配源发生错误。", innerException);
                }

                return base.constValue;
            }
        }

        public AdaptContextInfo<TSource, TTarget> ContextInfo => this.contextInfo;

        public RegexConstAdaptor(TSource constValue, Func<TSource, TTarget> sourceAdaptor, Func<TTarget, TSource> targetAdaptor) : this(constValue, sourceAdaptor, targetAdaptor, RegexConstAdaptor<TSource, TTarget>.DefaultSourceEqualityComparison) { }

        public RegexConstAdaptor(TSource constValue, Func<TSource, TTarget> sourceAdaptor, Func<TTarget, TSource> targetAdaptor, EqualityComparison<TSource> equalityComparison) :
            this(
                constValue,
                equalityComparison,
                new AdaptContextInfo<TSource, TTarget>(
                    sourceAdaptor ?? throw new ArgumentNullException(nameof(sourceAdaptor)),
                    targetAdaptor ?? throw new ArgumentNullException(nameof(targetAdaptor))
                )
            )
        { }

        public RegexConstAdaptor(TSource constValue, EqualityComparison<TSource> equalityComparison, AdaptContextInfo<TSource,TTarget> contextInfo)
        {
            if (equalityComparison == null) throw new ArgumentNullException(nameof(equalityComparison));
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.sourceConstValue = constValue;
            this.sourceEqualityComparison = equalityComparison;
            this.contextInfo = contextInfo;

            base.condition =
                target =>
                {
                    if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                        return this.sourceEqualityComparison(this.sourceConstValue, source);
                    else return false;
                };

            if (contextInfo.OnlyAdaptConstSourceWhenInitialization)
            {
                if (this.contextInfo.TryAdaptSource(this.sourceConstValue, out TTarget target, out Exception innerException))
                    base.constValue = target;
                else
                    throw new InvalidOperationException("在初始化常量时适配源发生错误", innerException);
            }
        }
    }
}
