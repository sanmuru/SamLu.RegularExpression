using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRegexFSMTransition<T> : ITransition
    {
    }

    public interface IRegexFSMTransition<T, TState> : IRegexFSMTransition<T>, ITransition<TState>
        where TState : IRegexFSMState<T>
    {

    }
}
