using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.Service
{
    public interface IRegexFSMService<T>
    {
        void Connect(IRegexFSM<T> fsm);
    }

    public interface IRegexFSMService<T, TRegexFSM> : IRegexFSMService<T>
        where TRegexFSM : IRegexFSM<T>
    {
        void Connect(TRegexFSM fsm);
    }

    public interface IRegexFSMService<T, TState, TTransition> : IRegexFSMService<T>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
        void Connect(IRegexFSM<T, TState, TTransition> fsm);
    }

    public interface IRegexFSMService<T, TState, TTransition, TRegexFSM> : IRegexFSMService<T, TRegexFSM>, IRegexFSMService<T, TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
        where TRegexFSM : IRegexFSM<T, TState, TTransition>
    {
        void Connect(TRegexFSM fsm);
    }
}
