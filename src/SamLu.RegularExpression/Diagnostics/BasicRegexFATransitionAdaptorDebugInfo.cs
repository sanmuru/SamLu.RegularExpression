using SamLu.Diagnostics;
using SamLu.RegularExpression.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    public class BasicRegexFATransitionAdaptorDebugInfo<T, TRegexFAState> : IDebugInfo
        where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
    {
        protected BasicRegexFATransitionAdaptor<T, TRegexFAState> transition;

        public virtual string DebugInfo => this.transition.InnerTransition.GetDebugInfo();

        public BasicRegexFATransitionAdaptorDebugInfo(BasicRegexFATransitionAdaptor<T, TRegexFAState> transition, params object[] args)
        {
            this.transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }
    }
}
