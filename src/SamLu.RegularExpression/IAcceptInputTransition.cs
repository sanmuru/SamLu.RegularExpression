using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public interface IAcceptInputTransition<T> : IRegexFSMTransition<T>
    {
        bool CanAccept(T input);
    }
}
