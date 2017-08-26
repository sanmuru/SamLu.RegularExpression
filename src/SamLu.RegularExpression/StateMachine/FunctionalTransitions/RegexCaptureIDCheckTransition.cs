using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public class RegexCaptureIDCheckTransition<T, TState> : RegexFunctionalTransition<T, TState>, IAcceptInputTransition<T>
        where TState : IRegexFSMState<T>
    {
        private object id;

        public object ID => this.id;

        public RegexCaptureIDCheckTransition(object id) => this.id = id;
    }
}
