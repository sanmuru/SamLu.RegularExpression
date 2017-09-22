using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public delegate bool RegexFSMTransitProxyHandler<T>(IRegexFSMTransition<T> transition, object[] args);

    public interface IRegexFSMTransitionProxy<T> : IRegexFSMTransition<T>
    {
        bool TransitProxy(T input, RegexFSMTransitProxyHandler<T> handler, params object[] args);
    }

    public delegate bool RegexFSMTransitProxyHandler<T, TState>(IRegexFSMTransition<T, TState> transition, object[] args) where TState : IRegexFSMState<T>;

    public interface IRegexFSMTransitionProxy<T, TState> : IRegexFSMTransitionProxy<T>, IRegexFSMTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        bool TransitProxy(T input, RegexFSMTransitProxyHandler<T, TState> handler, params object[] args);
    }
}
