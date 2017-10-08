using SamLu.Diagnostics;
using SamLu.RegularExpression.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public abstract class RegexFSMRepeatTransitionBase<T> : RegexFSMFunctionalTransition<T>
    {
        [RegexFSMFunctionalTransitionMetadata]
        public IRegexFSMState<T> StateFrom { get; set; }

        [RegexFSMFunctionalTransitionMetadata]
        public IRegexFSMState<T> StateTo { get; set; }

        protected abstract internal class _DebugInfoBase<TRepeatTransition> : RegexFSMFunctionalTransitionDebugInfoBase<T, TRepeatTransition>
            where TRepeatTransition : RegexFSMRepeatTransitionBase<T>
        {
            protected IRegexFSM<T> fsm;

            protected override IEnumerable<string> Parameters
            {
                get
                {
                    var states = this.fsm.States;
                    return new[]
                    {
                        this.functionalTransition.StateFrom == null ? null :
                            $"from: ({this.functionalTransition.StateFrom.GetDebugInfo(this.fsm, states)})",
                        this.functionalTransition.StateTo == null ? null :
                            $"to: ({this.functionalTransition.StateTo.GetDebugInfo(this.fsm, states)})"
                    };
                }
            }

            protected _DebugInfoBase(TRepeatTransition functionalTransition, params object[] args) : base(functionalTransition, args)
            {
                this.fsm = (IRegexFSM<T>)args[0];
            }
        }
    }

    public abstract class RegexFSMRepeatTransitionBase<T, TState> : RegexFSMFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        [RegexFSMFunctionalTransitionMetadata]
        public TState StateFrom { get; set; }

        [RegexFSMFunctionalTransitionMetadata]
        public TState StateTo { get; set; }

        protected abstract internal class _DebugInfoBase<TRepeatTransition> : RegexFSMFunctionalTransitionDebugInfoBase<T, TRepeatTransition>
            where TRepeatTransition : RegexFSMRepeatTransitionBase<T, TState>
        {
            protected IRegexFSM<T> fsm;

            protected override IEnumerable<string> Parameters
            {
                get
                {
                    var states = this.fsm.States;
                    return new[]
                    {
                        $"from: ({this.functionalTransition.StateFrom.GetDebugInfo(this.fsm, states)})",
                        $"to: ({this.functionalTransition.StateTo.GetDebugInfo(this.fsm, states)})"
                    };
                }
            }

            protected _DebugInfoBase(TRepeatTransition functionalTransition, params object[] args) : base(functionalTransition, args)
            {
                this.fsm = (IRegexFSM<T>)args[0];
            }
        }
    }
}
