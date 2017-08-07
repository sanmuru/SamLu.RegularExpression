using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public interface IRegexFAProvider<T>
    {
        RegexNFA<T> GenerateNFAFromRegexObject(RegexObject<T> regex);

        RegexDFA<T> GenerateDFAFromNFA(RegexNFA<T> nfa);
    }
}
