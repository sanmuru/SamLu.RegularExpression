using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public interface IRegexFAProvider<T>
    {
        BasicRegexNFA<T> GenerateNFAFromRegexObject(RegexObject<T> regex);

        BasicRegexDFA<T> GenerateDFAFromNFA(BasicRegexNFA<T> nfa);
    }
}
