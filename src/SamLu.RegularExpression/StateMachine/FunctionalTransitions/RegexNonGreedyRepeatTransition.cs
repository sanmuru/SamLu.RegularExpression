using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public sealed class RegexNonGreedyRepeatTransition<T> : RegexFunctionalTransition<T> { }

    public sealed class RegexNonGreedyRepeatTransition<T, TState> : RegexFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    { }
}
