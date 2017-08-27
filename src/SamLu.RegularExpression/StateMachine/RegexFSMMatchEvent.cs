using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public delegate void RegexFSMMatchEventHandler<T>(object sender, RegexFSMMatchEventArgs<T> e);

    public class RegexFSMMatchEventArgs<T> : EventArgs
    {
        private Match<T> match;

        public Match<T> Match => this.match;

        public RegexFSMMatchEventArgs(Match<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));

            this.match = match;
        }
    }
}
