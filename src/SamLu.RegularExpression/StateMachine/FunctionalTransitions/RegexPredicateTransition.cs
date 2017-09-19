using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public class RegexPredicateTransition<T> : RegexFunctionalTransition<T>, IRegexFSMTransitionProxy<T>
    {
        private Func<object, object[], bool> predicate;

        [RegexFunctionalTransitionMetadata]
        public Func<object, object[], bool> Predicate => this.predicate;

        public RegexPredicateTransition(Func<object, object[], bool> predicate) =>
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        public bool TransitProxy(RegexFSMTransitProxyHandler<T> handler, params object[] args)
        {
            return this.Predicate(this, args);
        }
    }
}
