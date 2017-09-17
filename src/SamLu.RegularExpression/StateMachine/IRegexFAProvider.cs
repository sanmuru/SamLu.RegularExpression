using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public interface IRegexFAProvider<T>
    {
        IRegexFSM<T> GenerateRegexFSMFromRegexObject(RegexObject<T> regex, RegexOptions options);

        IRegexDFA<T> GenerateRegexDFAFromRegexFSM(IRegexFSM<T> nfa);
    }
}
