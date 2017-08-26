using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public class RegexCaptureIDStorageTransition<T, TState> : RegexFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        private object id;

        public object ID => this.id;

        public RegexCaptureIDStorageTransition(object id) => this.id = id;
    }
}
