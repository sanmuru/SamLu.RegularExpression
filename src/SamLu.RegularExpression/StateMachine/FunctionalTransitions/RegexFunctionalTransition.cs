using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public abstract class RegexFunctionalTransition<T, TState> : FSMTransition<TState>, IRegexFSMTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        #region IRegexFSMTransition{T} Implementation
        IRegexFSMState<T> IRegexFSMTransition<T>.Target => this.Target;

        bool IRegexFSMTransition<T>.SetTarget(IRegexFSMState<T> state) =>
            base.SetTarget((TState)state);
        #endregion
    }
}
