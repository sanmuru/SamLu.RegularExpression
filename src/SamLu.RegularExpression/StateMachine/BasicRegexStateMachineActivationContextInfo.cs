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
    public class BasicRegexStateMachineActivationContextInfo<T> : RegexStateMachineActivationContextInfoBase<T>
    {
        protected RangeInfo<T> rangeInfo;
        protected ISet<T> accreditedSet;

        public override ISet<T> AccreditedSet => this.accreditedSet;

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

        public override IRegexDFA<T> ActivateRegexDFA() => new BasicRegexDFA<T>();

        public override TRegexDFA ActivateRegexDFAFromDumplication<TRegexDFA>(TRegexDFA dfa)
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

        public override IRegexDFAState<T> ActivateRegexDFAState(bool isTerminal = false) => new BasicRegexDFAState<T>(isTerminal);

        public override TRegexDFAState ActivateRegexDFAStateFromDumplication<TRegexDFAState>(TRegexDFAState state)
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

        public override IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            return new RangeSetRegexFATransition<T, BasicRegexDFAState<T>>(new RangeSet<T>(this.GetAccreditedSetIntersectResult(this.AccreditedSet, set), this.rangeInfo));
        }

        public override IRegexFSMEpsilonTransition<T> ActivateRegexFSMEpsilonTransition() => new BasicRegexFSMEpsilonTransition<T>();

        public override IRegexNFA<T> ActivateRegexNFA() => new BasicRegexNFA<T>();

        public override TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa)
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

        public override IRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false) => new BasicRegexNFAState<T>(isTerminal);

        public override TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state)
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

        public override TRegexNFATransition ActivateRegexNFATransitionFromDumplication<TRegexNFATransition>(TRegexNFATransition transition)
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

        public override IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex)
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
        
        protected override Predicate<T> GetPredicateFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition)
        {
            if (transition == null) return null;
            else if (transition is RegexCondition<T>)
                return ((RegexCondition<T>)transition).Condition;
            else if (transition is RangeSetRegexFATransition<T, BasicRegexDFAState<T>>)
                return ((RangeSetRegexFATransition<T, BasicRegexDFAState<T>>)transition).Predicate;
            else
                return transition.CanAccept;
        }

        protected override IAcceptInputTransition<T> ActivateRegexDFATransitionFromPredicate(Predicate<T> predicate)
        {
            return new BasicRegexFATransition<T, BasicRegexDFAState<T>>(predicate);
        }

        public override ISet<T> GetAccreditedSetFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition)
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
        
        protected virtual ISet<T> GetAccreditedSetFromRegexFSMTransition<TRegexFAState>(RangeSetRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return transition.Set;
        }
    }
}
