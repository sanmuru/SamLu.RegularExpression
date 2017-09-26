using SamLu.IO;
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
        protected Func<object, object[], bool> predicate;

        [RegexFunctionalTransitionMetadata]
        public Func<object, object[], bool> Predicate => this.predicate;

        protected RegexPredicateTransition() { }

        public RegexPredicateTransition(Func<object, object[], bool> predicate) : this() =>
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        #region IRegexFSMTransitionProxy{T} Implementation
        bool IRegexFSMTransitionProxy<T>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T> handler, params object[] args) => this.Predicate(this, args);
        #endregion
    }

    public class RegexPredicateTransition<T, TState> : RegexFunctionalTransition<T, TState>, IRegexFSMTransitionProxy<T, TState>
        where TState : IRegexFSMState<T>
    {
        protected Func<object, object[], bool> predicate;

        [RegexFunctionalTransitionMetadata]
        public Func<object, object[], bool> Predicate => this.predicate;

        protected RegexPredicateTransition() { }

        public RegexPredicateTransition(Func<object, object[], bool> predicate) : this() =>
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        #region IRegexFSMTransitionProxy{T}/IRegexFSMTransitionProxy{T, TState} Implementation
        bool IRegexFSMTransitionProxy<T>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T> handler, params object[] args) => this.Predicate(this, args);

        bool IRegexFSMTransitionProxy<T, TState>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T, TState> handler, params object[] args) => this.Predicate(this, args);
        #endregion
    }
}
