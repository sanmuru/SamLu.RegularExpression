using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public sealed class RegexCaptureIDStorageTransition<T> : RegexFunctionalTransition<T>
    {
        private object id;

        [RegexFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexCaptureIDStorageTransition(object id) => this.id = id;
    }

    public sealed class RegexCaptureIDStorageTransition<T, TState> : RegexFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        private object id;

        [RegexFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexCaptureIDStorageTransition(object id) => this.id = id;
    }
}
