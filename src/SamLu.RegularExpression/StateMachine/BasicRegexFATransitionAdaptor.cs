using SamLu.RegularExpression.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.StateMachine;
using SamLu.Diagnostics;
using SamLu.Runtime;
using SamLu.RegularExpression.Diagnostics;

namespace SamLu.RegularExpression.StateMachine
{
    [DebugInfoProxy(
        typeof(BasicRegexFATransitionAdaptorDebugInfo<,>),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 })]
    public class BasicRegexFATransitionAdaptor<T, TRegexFAState> : BasicRegexFATransition<T>, IAdaptor<BasicRegexFATransition<T>, BasicRegexFATransition<T, TRegexFAState>>
        where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
    {
        protected BasicRegexFATransition<T, TRegexFAState> innerTransition;

        public BasicRegexFATransition<T, TRegexFAState> InnerTransition => this.innerTransition;

        /// <summary>
        /// 获取一个方法，该方法确定 <see cref="BasicRegexFATransitionAdaptor{T, TRegexFAState}"/> 接受的输入是否满足条件。
        /// </summary>
        public override Predicate<T> Predicate => this.innerTransition.Predicate;

        /// <summary>
        /// 获取或设置表示 <see cref="BasicRegexFATransitionAdaptor{T, TRegexFAState}"/> 的转换动作。在转换转换时进行。
        /// </summary>
        public override IAction TransitAction
        {
            get => this.innerTransition.TransitAction;
            set => this.innerTransition.TransitAction = value;
        }

        public BasicRegexFATransitionAdaptor(BasicRegexFATransition<T, TRegexFAState> transition) : base() =>
            this.innerTransition = transition ?? throw new ArgumentNullException(nameof(transition));

        AdaptContextInfo<BasicRegexFATransition<T>, BasicRegexFATransition<T, TRegexFAState>> IAdaptor<BasicRegexFATransition<T>, BasicRegexFATransition<T, TRegexFAState>>.ContextInfo => throw new NotImplementedException();
    }
}
