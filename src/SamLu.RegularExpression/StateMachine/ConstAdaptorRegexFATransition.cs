using SamLu.RegularExpression.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class ConstAdaptorRegexFATransition<TSource, TTarget, TRegexFAState> :
        BasicRegexFATransition<TTarget, TRegexFAState>,
        IAdaptor<TSource, TTarget>
        where TRegexFAState :
            IRegexFSMState<TTarget, BasicRegexFATransition<TTarget, TRegexFAState>>
    {
        public AdaptContextInfo<TSource, TTarget> ContextInfo { get; protected set; }

        /// <summary>
        /// 初始化 <see cref="ConstAdaptorRegexFATransition{TSource, TTarget, TRegexFAState}"/> 类的新实例。
        /// </summary>
        protected ConstAdaptorRegexFATransition() : base() { }

        public ConstAdaptorRegexFATransition(
            Predicate<TSource> predicate,
            AdaptDelegate<TSource, TTarget> sourceAdaptor, AdaptDelegate<TTarget, TSource> targetAdaptor
        ) :
            this(
                predicate ?? throw new ArgumentNullException(nameof(predicate)),
                new AdaptContextInfo<TSource, TTarget>(
                    sourceAdaptor ?? throw new ArgumentNullException(nameof(sourceAdaptor)),
                    targetAdaptor ?? throw new ArgumentNullException(nameof(targetAdaptor))
                )
            )
        { }

        public ConstAdaptorRegexFATransition(
            Predicate<TSource> predicate,
            AdaptContextInfo<TSource, TTarget> contextInfo
        ) :
            base(new Func<Predicate<TSource>, AdaptContextInfo<TSource, TTarget>, Predicate<TTarget>>((_predicate, _contextInfo) =>
                target => _predicate(_contextInfo.TargetAdaptor(target))
            )
            (
                predicate ?? throw new ArgumentNullException(nameof(predicate)),
                contextInfo ?? throw new ArgumentNullException(nameof(contextInfo))
            ))
        {
            this.ContextInfo = contextInfo;
        }

        protected internal ConstAdaptorRegexFATransition(Predicate<TTarget> predicate) :
            base(predicate ?? throw new ArgumentNullException(nameof(predicate)))
        { }
    }
}
