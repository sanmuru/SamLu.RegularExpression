using SamLu.Collections.ObjectModel;
using SamLu.RegularExpression.ObjectModel;
using SamLu.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public abstract class RegexStateMachineActivationContextInfoBase<T> : IRegexStateMachineActivationContextInfo<T>
    {
        public abstract ISet<T> AccreditedSet { get; }

        public virtual IRegexDFA<T> ActivateRegexDFA() => new RegexDFA<T>();

        public static readonly MethodShuntSource ActivateRegexDFAFromDumplicationSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod("ActivateRegexDFAFromDumplication"));

        public virtual TRegexDFA ActivateRegexDFAFromDumplication<TRegexDFA>(TRegexDFA dfa) where TRegexDFA : IRegexDFA<T>
        {
            return (TRegexDFA)this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFAFromDumplicationSource, dfa);
        }

        public virtual IRegexDFAState<T> ActivateRegexDFAState(bool isTerminal = false) =>
            isTerminal ?
                CommonRegexFAState<T>.TerminalDFAState :
                CommonRegexFAState<T>.DFAState;

        public abstract TRegexDFAState ActivateRegexDFAStateFromDumplication<TRegexDFAState>(TRegexDFAState state) where TRegexDFAState : IRegexDFAState<T>;

        public virtual IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            if (set is RangeSet<T> rangeSet)
                return BasicRegexFATransition<T>.Adapt(
                    new RangeSetRegexFATransition<T, BasicRegexDFAState<T>>(rangeSet)
                );
            else
                return new BasicRegexFATransition<T>(set.Contains);
        }

        public virtual IRegexFSMEpsilonTransition<T> ActivateRegexFSMEpsilonTransition() => new RegexFSMEpsilonTransition<T>();

        public virtual IRegexNFA<T> ActivateRegexNFA() => new RegexNFA<T>();

        public abstract TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa) where TRegexNFA : IRegexNFA<T>;

        public virtual IRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false) =>
            isTerminal ?
                CommonRegexFAState<T>.TerminalNFAState :
                CommonRegexFAState<T>.NFAState;

        public abstract TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state) where TRegexNFAState : IRegexNFAState<T>;

        public abstract TRegexNFATransition ActivateRegexNFATransitionFromDumplication<TRegexNFATransition>(TRegexNFATransition transition) where TRegexNFATransition : IRegexFSMTransition<T>;

        public virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return new BasicRegexFATransition<T>(regex.Condition);
        }

        public virtual IAcceptInputTransition<T> CombineRegexDFATransitions(IEnumerable<IAcceptInputTransition<T>> transitions)
        {
            if (transitions == null) throw new ArgumentNullException(nameof(transitions));

            IAcceptInputTransition<T>[] transitionArray = transitions.Where(transition => transition != null).ToArray();
            switch (transitionArray.Length)
            {
                case 0:
                    return null;
                case 1:
                    return transitionArray[0];
                default:
                    return this.ActivateRegexDFATransitionFromPredicate(
                        transitionArray
                            .Select(this.GetPredicateFromRegexAcceptInputTransition)
                            .Aggregate((predicate1, predicate2) => predicate1 + predicate2)
                    );
            }
        }

        protected virtual Predicate<T> GetPredicateFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return transition.CanAccept;
        }

        protected virtual IAcceptInputTransition<T> ActivateRegexDFATransitionFromPredicate(Predicate<T> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return new BasicRegexFATransition<T>(predicate);
        }

        public abstract ISet<T> GetAccreditedSetFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition);

        public virtual ISet<T> GetAccreditedSetExceptResult(ISet<T> first, ISet<T> second)
        {
            return new SetGroup<T>(
                new[] { first, second },
                SetGroup<T>.ExceptGroupPredicate
            );
        }

        public virtual ISet<T> GetAccreditedSetIntersectResult(ISet<T> first, ISet<T> second)
        {
            return new SetGroup<T>(
                new[] { first, second },
                SetGroup<T>.IntersectGroupPredicate
            );
        }

        public virtual ISet<T> GetAccreditedSetSymmetricExceptResult(ISet<T> first, ISet<T> second)
        {
            return new SetGroup<T>(
                new[] { first, second },
                SetGroup<T>.SymmetricExceptGroupPredicate
            );
        }

        public virtual ISet<T> GetAccreditedSetUnionResult(ISet<T> first, ISet<T> second)
        {
            return new SetGroup<T>(
                new[] { first, second },
                SetGroup<T>.UnionGroupPredicate
            );
        }
    }
}
