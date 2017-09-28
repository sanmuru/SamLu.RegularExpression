using SamLu.Collections.ObjectModel;
using SamLu.RegularExpression.Adapter;
using SamLu.RegularExpression.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class BasicRegexStateMachineActivationContextInfo<T> : IRegexStateMachineActivationContextInfo<T>
    {
        protected RangeInfo<T> rangeInfo;
        protected ISet<T> accreditedSet;

        public virtual ISet<T> AccreditedSet => this.accreditedSet;

        protected BasicRegexStateMachineActivationContextInfo(RangeInfo<T> rangeInfo)
        {
            if (rangeInfo == null) throw new ArgumentNullException(nameof(rangeInfo));

            this.rangeInfo = rangeInfo;
        }

        public BasicRegexStateMachineActivationContextInfo(RangeSet<T> accreditedSet)
        {
            if (accreditedSet == null) throw new ArgumentNullException(nameof(accreditedSet));

            this.rangeInfo = accreditedSet.RangeInfo;
            this.accreditedSet = accreditedSet;
        }

        public virtual IRegexDFA<T> ActivateRegexDFA() => new BasicRegexDFA<T>();

        public virtual TRegexDFA ActivateRegexDFAFromDumplication<TRegexDFA>(TRegexDFA dfa) where TRegexDFA : IRegexDFA<T>
        {
            if (typeof(BasicRegexDFA<T>).IsAssignableFrom(typeof(TRegexDFA)))
                return (TRegexDFA)(object)this.ActivateRegexDFAFromDumplication((BasicRegexDFA<T>)(object)dfa);
            else
                throw new NotSupportedException();
        }

        protected virtual BasicRegexDFA<T> ActivateRegexDFAFromDumplication(BasicRegexDFA<T> dfa)
        {
            if (dfa == null) throw new ArgumentNullException(nameof(dfa));

            return new BasicRegexDFA<T>() { StartState = dfa.StartState };
        }

        public virtual IRegexFSMState<T> ActivateRegexDFAState(bool isTerminal = false) => new BasicRegexDFAState<T>(isTerminal);

        public virtual TRegexDFAState ActivateRegexDFAStateFromDumplication<TRegexDFAState>(TRegexDFAState state) where TRegexDFAState : IRegexFSMState<T>
        {
            if (typeof(BasicRegexDFAState<T>).IsAssignableFrom(typeof(TRegexDFAState)))
                return (TRegexDFAState)(object)this.ActivateRegexDFAStateFromDumplication((BasicRegexDFAState<T>)(object)state);
            else
                throw new NotSupportedException();
        }

        protected virtual BasicRegexDFAState<T> ActivateRegexDFAStateFromDumplication(BasicRegexDFAState<T> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return new BasicRegexDFAState<T>(state.IsTerminal);
        }

        public virtual IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            return new RangeSetRegexFATransition<T, BasicRegexDFAState<T>>(new RangeSet<T>(this.GetAccreditedSetIntersectResult(this.AccreditedSet, set), rangeInfo));
        }

        public virtual IRegexFSMEpsilonTransition<T> ActivateRegexFSMEpsilonTransition() => new BasicRegexFSMEpsilonTransition<T>();

        public virtual IRegexNFA<T> ActivateRegexNFA() => new BasicRegexNFA<T>();

        public virtual TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa) where TRegexNFA : IRegexNFA<T>
        {
            if (typeof(BasicRegexNFA<T>).IsAssignableFrom(typeof(TRegexNFA)))
                return (TRegexNFA)(object)this.ActivateRegexNFAFromDumplication((BasicRegexNFA<T>)(object)nfa);
            else
                throw new NotSupportedException();
        }

        protected virtual BasicRegexNFA<T> ActivateRegexNFAFromDumplication(BasicRegexNFA<T> nfa)
        {
            if (nfa == null) throw new ArgumentNullException(nameof(nfa));

            return new BasicRegexNFA<T>() { StartState = nfa.StartState };
        }

        public virtual IRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false) => new BasicRegexNFAState<T>(isTerminal);

        public virtual TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state) where TRegexNFAState : IRegexNFAState<T>
        {
            if (typeof(BasicRegexNFAState<T>).IsAssignableFrom(typeof(TRegexNFAState)))
                return (TRegexNFAState)(object)this.ActivateRegexNFAStateFromDumplication((BasicRegexNFAState<T>)(object)state);
            else
                throw new NotSupportedException();
        }

        protected virtual BasicRegexNFAState<T> ActivateRegexNFAStateFromDumplication(BasicRegexNFAState<T> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return new BasicRegexNFAState<T>(state.IsTerminal);
        }

        public virtual TRegexNFATransition ActivateRegexNFATransitionFromDumplication<TRegexNFATransition>(TRegexNFATransition transition) where TRegexNFATransition : IRegexFSMTransition<T>
        {
            if (typeof(BasicRegexFATransition<T, BasicRegexNFAState<T>>).IsAssignableFrom(typeof(TRegexNFATransition)))
                return (TRegexNFATransition)(object)this.ActivateRegexNFATransitionFromDumplication((BasicRegexFATransition<T, BasicRegexNFAState<T>>)(object)transition);
            else
                return transition;
        }

        protected virtual BasicRegexFATransition<T, BasicRegexNFAState<T>> ActivateRegexNFATransitionFromDumplication(BasicRegexFATransition<T, BasicRegexNFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transition is RangeSetRegexFATransition<T, BasicRegexNFAState<T>>)
                return this.ActivateRegexNFATransitionFromDumplication((RangeSetRegexFATransition<T, BasicRegexNFAState<T>>)transition);
            else
                return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(transition.Predicate);
        }

        protected virtual RangeSetRegexFATransition<T, BasicRegexNFAState<T>> ActivateRegexNFATransitionFromDumplication(RangeSetRegexFATransition<T, BasicRegexNFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return new RangeSetRegexFATransition<T, BasicRegexNFAState<T>>(new RangeSet<T>(transition.Set, this.rangeInfo));
        }

        public virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexConst<T>)
                return this.ActivateRegexNFATransitionFromRegexConst((RegexConst<T>)regex);
            else if (regex is RegexRange<T>)
                return this.ActivateRegexNFATransitionFromRegexRange((RegexRange<T>)regex);
            else
                return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(regex.Condition);
        }

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexConst(RegexConst<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            Type type = regex.GetType();
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(RegexConstAdaptor<,>) && type.GetGenericArguments()[1] == typeof(T))
            )
            {
                return (IAcceptInputTransition<T>)
                    (this
                        .GetType()
                        .GetMethod(nameof(ActivateRegexNFATransitionFromRegexConstAdaptor))
                        .MakeGenericMethod(type.GetGenericArguments())
                        .Invoke(this, new object[] { regex })
                    );
            }
            else
                return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(regex.Condition);
        }

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexConstAdaptor<TTarget>(RegexConstAdaptor<TTarget, T> regex)
        {
            return new ConstAdaptorRegexFATransition<TTarget, T, BasicRegexNFAState<T>>(regex.Condition);
        }

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexRange(RegexRange<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            Type type = regex.GetType();
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(RegexRangeAdaptor<,>) && type.GetGenericArguments()[1] == typeof(T))
            )
            {
                return (IAcceptInputTransition<T>)
                    (this
                        .GetType()
                        .GetMethod(nameof(ActivateRegexNFATransitionFromRegexConst))
                        .MakeGenericMethod(type.GetGenericArguments())
                        .Invoke(this, new object[] { regex })
                    );
            }
            else
                return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(regex.Condition);
        }

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexRangeAdaptor<TTarget>(RegexRangeAdaptor<TTarget, T> regex)
        {
            return new ConstAdaptorRegexFATransition<TTarget, T, BasicRegexNFAState<T>>(regex.Condition);
        }

        public virtual IAcceptInputTransition<T> CombineRegexDFATransitions(IEnumerable<IAcceptInputTransition<T>> transitions)
        {
            if (transitions == null) throw new ArgumentNullException(nameof(transitions));

            Predicate<T> predicate = transitions
                .Select(transition =>
                {
                    if (transition == null) return null;
                    else if (transition is RegexCondition<T>)
                        return ((RegexCondition<T>)transition).Condition;
                    else if (transition is RangeSetRegexFATransition<T, BasicRegexDFAState<T>>)
                        return ((RangeSetRegexFATransition<T, BasicRegexDFAState<T>>)transition).Predicate;
                    else
                        return transition.CanAccept;
                })
                .Aggregate(
                    (Predicate<T>)null,
                    ((seed, _predicate) => seed + _predicate)
                );
            return new BasicRegexFATransition<T, BasicRegexDFAState<T>>(predicate);
        }

        public virtual ISet<T> GetAccreditedSetFromRegexFSMTransition(IAcceptInputTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            Type type = transition.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BasicRegexFATransition<,>) && type.GetGenericArguments()[0] == typeof(T))
                return (ISet<T>)this.GetType().GetMethod(nameof(GetAccreditedSetFromBasicRegexFATransition)).Invoke(this, new object[] { transition });
            else
                return new RangeSet<T>(this.AccreditedSet.Where(transition.CanAccept), this.rangeInfo);
        }

        protected virtual ISet<T> GetAccreditedSetFromBasicRegexFATransition<TRegexFAState>(BasicRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transition is RangeSetRegexFATransition<T, TRegexFAState>)
                return ((RangeSetRegexFATransition<T, TRegexFAState>)transition).Set;
            else
                return new RangeSet<T>(this.AccreditedSet.Where(item => transition.Predicate(item)), this.rangeInfo);
        }

        public virtual ISet<T> GetAccreditedSetExceptResult(ISet<T> first, ISet<T> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return new SetGroup<T>(new[] { first, second }, SetGroup<T>.ExceptGroupPredicate);
        }

        protected virtual ISet<T> GetAccreditedSetFromRegexFSMTransition<TRegexFAState>(RangeSetRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return transition.Set;
        }

        public virtual ISet<T> GetAccreditedSetIntersectResult(ISet<T> first, ISet<T> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return new SetGroup<T>(new[] { first, second }, SetGroup<T>.IntersectGroupPredicate);
        }

        public virtual ISet<T> GetAccreditedSetSymmetricExceptResult(ISet<T> first, ISet<T> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return new SetGroup<T>(new[] { first, second }, SetGroup<T>.SymmetricExceptGroupPredicate);
        }

        public virtual ISet<T> GetAccreditedSetUnionResult(ISet<T> first, ISet<T> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return new SetGroup<T>(new[] { first, second }, SetGroup<T>.UnionGroupPredicate);
        }
    }
}
