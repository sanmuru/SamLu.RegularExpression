using SamLu.RegularExpression.Extend;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public sealed class RegexCaptureEndTransition<T> : RegexFunctionalTransition<T>
    {
        private RegexGroup<T> group;

        [RegexFunctionalTransitionMetadata]
        public RegexGroup<T> Group => this.group;

        public RegexCaptureEndTransition(RegexGroup<T> group) =>
            this.group = group ?? throw new ArgumentNullException(nameof(group));
    }

    public sealed class RegexCaptureEndTransition<T, TState> : RegexFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    { }
}
