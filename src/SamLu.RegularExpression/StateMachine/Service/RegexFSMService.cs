using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.Service
{
    public abstract class RegexFSMService<T> : IRegexFSMService<T>
    {
        public abstract void Connect(IRegexFSM<T> fsm);
    }

    public abstract class RegexFSMService<T, TRegexFSM> : IRegexFSMService<T, TRegexFSM>
        where TRegexFSM : IRegexFSM<T>
    {
        public abstract void Connect(TRegexFSM fsm);
        
        #region IRegexFSMService Implementation
        void IRegexFSMService<T>.Connect(IRegexFSM<T> fsm) =>
            this.Connect((TRegexFSM)fsm);
        #endregion
    }

    public abstract class RegexFSMService<T, TState, TTransition> : IRegexFSMService<T, TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
        public abstract void Connect(IRegexFSM<T, TState, TTransition> fsm);
        
        #region IRegexFSMService Implementation
        void IRegexFSMService<T>.Connect(IRegexFSM<T> fsm) =>
            this.Connect((IRegexFSM<T, TState, TTransition>)fsm);
        #endregion
    }

    public abstract class RegexFSMService<T, TState, TTransition, TRegexFSM> : IRegexFSMService<T, TState, TTransition, TRegexFSM>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
        where TRegexFSM : IRegexFSM<T, TState, TTransition>
    {
        public abstract void Connect(TRegexFSM fsm);

        #region IRegexFSMService Implementation
        void IRegexFSMService<T, TState, TTransition>.Connect(IRegexFSM<T, TState, TTransition> fsm) =>
            this.Connect((TRegexFSM)fsm);

        void IRegexFSMService<T>.Connect(IRegexFSM<T> fsm) =>
            this.Connect((TRegexFSM)fsm);
        #endregion
    }
}
