using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public sealed class RegexCaptureIDCheckTransition<T> : RegexFunctionalTransition<T>, IAcceptInputTransition<T>
    {
        private object id;

        [RegexFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexCaptureIDCheckTransition(object id) => this.id = id;

        public bool CanAccept(T input)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class RegexCaptureIDCheckTransition<T, TState> : RegexFunctionalTransition<T, TState>, IAcceptInputTransition<T>
        where TState : IRegexFSMState<T>
    {
        private object id;

        [RegexFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexCaptureIDCheckTransition(object id) => this.id = id;

        public bool CanAccept(T input)
        {
            throw new NotImplementedException();
        }
    }
}
