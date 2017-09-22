using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public sealed class RegexCaptureIDCheckTransition<T> : RegexPredicateTransition<T>
    {
        private object id;

        [RegexFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexCaptureIDCheckTransition(object id, Func<RegexCaptureIDCheckTransition<T>, object[], bool> predicate) : base()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.id = id;
            base.predicate = (sender, args) => predicate((RegexCaptureIDCheckTransition<T>)sender, args);
        }
    }

    public sealed class RegexCaptureIDCheckTransition<T, TState> : RegexPredicateTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        private object id;

        [RegexFunctionalTransitionMetadata]
        public object ID => this.id;

        public RegexCaptureIDCheckTransition(object id, Func<RegexCaptureIDCheckTransition<T, TState>, object[], bool> predicate) : base()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.id = id;
            base.predicate = (sender, args) => predicate((RegexCaptureIDCheckTransition<T, TState>)sender, args);
        }
    }
}
