using SamLu.Collections.ObjectModel;
using SamLu.RegularExpression.Adapter;
using SamLu.RegularExpression.ObjectModel;
using SamLu.Runtime;
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

        #region ActivateRegexDFAFromDumplication
        protected static readonly MethodShuntKey ActivateRegexDFAFromDumplicationKey_BasicRegexDFA = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFAFromDumplicationSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexDFAFromDumplication), new[] { typeof(BasicRegexDFA<T>) })
        );

        protected virtual BasicRegexDFA<T> ActivateRegexDFAFromDumplication(BasicRegexDFA<T> dfa)
        {
            if (dfa == null) throw new ArgumentNullException(nameof(dfa));

            return new BasicRegexDFA<T>() { StartState = dfa.StartState };
        }
        #endregion

        public override IRegexDFAState<T> ActivateRegexDFAState(bool isTerminal = false) => new BasicRegexDFAState<T>(isTerminal);

        #region ActivateRegexDFAStateFromDumplication
        protected static readonly MethodShuntKey ActivateRegexDFAStateFromDumplicationKey_BasicRegexDFAState = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFAStateFromDumplicationSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexDFAStateFromDumplication), new[] { typeof(BasicRegexDFAState<T>) })
        );

        protected virtual BasicRegexDFAState<T> ActivateRegexDFAStateFromDumplication(BasicRegexDFAState<T> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return new BasicRegexDFAState<T>(state.IsTerminal);
        }
        #endregion

        public override IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            return new RangeSetRegexFATransition<T, BasicRegexDFAState<T>>(new RangeSet<T>(this.GetAccreditedSetIntersectResult(this.AccreditedSet, set), this.rangeInfo));
        }

        public override IRegexFSMEpsilonTransition<T> ActivateRegexFSMEpsilonTransition() => new BasicRegexFSMEpsilonTransition<T>();

        public override IRegexNFA<T> ActivateRegexNFA() => new BasicRegexNFA<T>();

        #region ActivateRegexNFAFromDumplication
        protected static readonly MethodShuntKey ActivateRegexNFAFromDumplicationKey_BasicRegexNFA = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFAFromDumplicationSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFAFromDumplication), new[] { typeof(BasicRegexNFA<T>) })
        );

        protected virtual BasicRegexNFA<T> ActivateRegexNFAFromDumplication(BasicRegexNFA<T> nfa)
        {
            if (nfa == null) throw new ArgumentNullException(nameof(nfa));

            return new BasicRegexNFA<T>() { StartState = nfa.StartState };
        }
        #endregion

        public override IRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false) => new BasicRegexNFAState<T>(isTerminal);

        #region ActivateRegexNFAStateFromDumplication
        protected static readonly MethodShuntKey ActivateRegexNFAStateFromDumplicationKey_BasicRegexNFAState = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFAStateFromDumplicationSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFAStateFromDumplication), new[] { typeof(BasicRegexNFAState<T>) })
        );

        protected virtual BasicRegexNFAState<T> ActivateRegexNFAStateFromDumplication(BasicRegexNFAState<T> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return new BasicRegexNFAState<T>(state.IsTerminal);
        }
        #endregion

        #region ActivateRegexNFATransitionFromDumplication
        protected static readonly MethodShuntKey ActivateRegexNFATransitionFromDumplicationKey_BasicRegexNFATransition = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFATransitionFromDumplicationSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFATransitionFromDumplication), new[] { typeof(BasicRegexFATransition<T, BasicRegexNFAState<T>>) })
        );
        protected static readonly MethodShuntSource ActivateRegexNFATransitionFromDumplicationSource_BasicRegexNFATransition = MethodShunt.CreateSource(typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFATransitionFromDumplication), new[] { typeof(BasicRegexFATransition<T, BasicRegexNFAState<T>>) }));

        protected virtual BasicRegexFATransition<T, BasicRegexNFAState<T>> ActivateRegexNFATransitionFromDumplication(BasicRegexFATransition<T, BasicRegexNFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            var result = this.DynamicInvokeShunt(BasicRegexStateMachineActivationContextInfo<T>.ActivateRegexNFATransitionFromDumplicationSource_BasicRegexNFATransition, transition);
            if (result.Success)
                return (BasicRegexFATransition<T, BasicRegexNFAState<T>>)result.ReturnValue;
            else
                return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(transition.Predicate);
        }

        protected static readonly MethodShuntKey ActivateRegexNFATransitionFromDumplicationKey_RangeSetRangeNFATransition = MethodShunt.Register(
            BasicRegexStateMachineActivationContextInfo<T>.ActivateRegexNFATransitionFromDumplicationSource_BasicRegexNFATransition,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFATransitionFromDumplication), new[] { typeof(RangeSetRegexFATransition<T, BasicRegexNFAState<T>>) })
        );

        protected virtual RangeSetRegexFATransition<T, BasicRegexNFAState<T>> ActivateRegexNFATransitionFromDumplication(RangeSetRegexFATransition<T, BasicRegexNFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return new RangeSetRegexFATransition<T, BasicRegexNFAState<T>>(new RangeSet<T>(transition.Set, this.rangeInfo));
        }
        #endregion

        #region ActivateRegexNFATransitionFromRegexCondition
        public override IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFATransitionFromRegexConditionSource, regex);
            if (result.Success)
                return (IAcceptInputTransition<T>)result.ReturnValue;
            else
                return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(regex.Condition);
        }

        #region ActivateRegexNFATransitionFromRegexConst
        protected static readonly MethodShuntSource ActivateRegexNFATransitionFromRegexConstSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexNFATransitionFromRegexConst)));

        protected override IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexConst(RegexConst<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            var result = this.DynamicInvokeShunt(BasicRegexStateMachineActivationContextInfo<T>.ActivateRegexNFATransitionFromRegexConstSource, regex);
            if (result.Success)
                return (IAcceptInputTransition<T>)result.ReturnValue;
            else
            {
                if (this.AccreditedSet.Any(t => regex.Condition(t)))
                    return new RangeSetRegexFATransition<T, BasicRegexNFAState<T>>(new RangeSet<T>(new[] { (IRange<T>)regex }, this.rangeInfo));
                else
                    return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(t => false);
            }
        }

        protected static readonly MethodShuntKey ActivateRegexNFATransitionFromRegexConstKey_RegexConstAdaptor = MethodShunt.Register(
            BasicRegexStateMachineActivationContextInfo<T>.ActivateRegexNFATransitionFromRegexConstSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFATransitionFromRegexConstAdaptor))
        );

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexConstAdaptor<TTarget>(RegexConstAdaptor<TTarget, T> regex)
        {
            return new ConstAdaptorRegexFATransition<TTarget, T, BasicRegexNFAState<T>>(t => this.AccreditedSet.Any(_t => regex.Condition(_t)));
        }
        #endregion

        #region ActivateRegexNFATransitionFromRegexRange
        protected static readonly MethodShuntSource ActivateRegexNFATransitionFromRegexRangeSource = MethodShunt.CreateSource(typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFATransitionFromRegexRange)));

        protected override IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexRange(RegexRange<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            var result = this.DynamicInvokeShunt(BasicRegexStateMachineActivationContextInfo<T>.ActivateRegexNFATransitionFromRegexRangeSource, regex);
            if (result.Success)
                return (IAcceptInputTransition<T>)result.ReturnValue;
            else
            {
                if (this.AccreditedSet.Any(t => regex.Condition(t)))
                    return new RangeSetRegexFATransition<T, BasicRegexNFAState<T>>(new RangeSet<T>(new[] { (IRange<T>)regex }, this.rangeInfo));
                else
                    return new BasicRegexFATransition<T, BasicRegexNFAState<T>>(t => false);
            }
        }

        protected static readonly MethodShuntKey ActivateRegexNFATransitionFromRegexRangeKey_RegexRangeAdaptor = MethodShunt.Register(
            BasicRegexStateMachineActivationContextInfo<T>.ActivateRegexNFATransitionFromRegexRangeSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(ActivateRegexNFATransitionFromRegexRangeAdaptor))
        );

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexRangeAdaptor<TTarget>(RegexRangeAdaptor<TTarget, T> regex)
        {
            return new ConstAdaptorRegexFATransition<TTarget, T, BasicRegexNFAState<T>>(t => this.AccreditedSet.Any(_t => regex.Condition(_t)));
        }
        #endregion
        #endregion

        #region GetPredicateFromRegexAcceptInputTransition
        protected static readonly MethodShuntSource GetPredicateFromRegexAcceptInputTransitionSource = MethodShunt.CreateSource(typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(GetPredicateFromRegexAcceptInputTransition)));

        protected override Predicate<T> GetPredicateFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition)
        {
            if (transition == null) return null;
            else
            {
                var result = this.DynamicInvokeShunt(BasicRegexStateMachineActivationContextInfo<T>.GetPredicateFromRegexAcceptInputTransitionSource, transition);
                if (result.Success)
                    return (Predicate<T>)result.ReturnValue;
                else
                    return transition.CanAccept;
            }
        }

        protected static readonly MethodShuntKey GetPredicateFromRegexAcceptInputTransitionKey_RangeSetRegexFATransition = MethodShunt.Register(
            BasicRegexStateMachineActivationContextInfo<T>.GetPredicateFromRegexAcceptInputTransitionSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(GetPredicateFromBasicRegexFATransition))
        );

        protected virtual Predicate<T> GetPredicateFromBasicRegexFATransition<TRegexFAState>(BasicRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return transition.Predicate;
        }
        #endregion

        protected override IAcceptInputTransition<T> ActivateRegexDFATransitionFromPredicate(Predicate<T> predicate)
        {
            return new BasicRegexFATransition<T, BasicRegexDFAState<T>>(predicate);
        }

        #region GetAccreditedSetFromRegexAcceptInputTransition
        public static readonly MethodShuntSource GetAccreditedSetFromRegexAcceptInputTransitionSource = MethodShunt.CreateSource(typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(GetAccreditedSetFromRegexAcceptInputTransition)));

        public override ISet<T> GetAccreditedSetFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            var result = this.DynamicInvokeShunt(BasicRegexStateMachineActivationContextInfo<T>.GetAccreditedSetFromRegexAcceptInputTransitionSource, transition);
            if (result.Success)
                return (ISet<T>)result.ReturnValue;
            else
                return new RangeSet<T>(this.AccreditedSet.Where(transition.CanAccept), this.rangeInfo);
        }

        protected static readonly MethodShuntKey GetAccreditedSetFromRegexAcceptInputTransitionKey_BasicRegexFATransition = MethodShunt.Register(
            BasicRegexStateMachineActivationContextInfo<T>.GetAccreditedSetFromRegexAcceptInputTransitionSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(GetAccreditedSetFromBasicRegexFATransition))
        );
        protected static readonly MethodShuntSource GetAccreditedSetFromBasicRegexFATransitionSource = MethodShunt.CreateSource(typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(GetAccreditedSetFromBasicRegexFATransition))
        );

        protected virtual ISet<T> GetAccreditedSetFromBasicRegexFATransition<TRegexFAState>(BasicRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            var result = this.DynamicInvokeShunt(BasicRegexStateMachineActivationContextInfo<T>.GetAccreditedSetFromBasicRegexFATransitionSource, transition);
            if (result.Success)
                return (ISet<T>)result.ReturnValue;
            else
                return new RangeSet<T>(this.AccreditedSet.Where(item => transition.Predicate(item)), this.rangeInfo);
        }

        protected static readonly MethodShuntKey GetAccreditedSetFromBasicRegexFATransitionKey_RangeSetRegexFATransition = MethodShunt.Register(
            BasicRegexStateMachineActivationContextInfo<T>.GetAccreditedSetFromBasicRegexFATransitionSource,
            typeof(BasicRegexStateMachineActivationContextInfo<T>).GetMethod(nameof(GetAccreditedSetFromRangeSetRegexFATransition))
        );

        protected virtual ISet<T> GetAccreditedSetFromRangeSetRegexFATransition<TRegexFAState>(RangeSetRegexFATransition<T, TRegexFAState> transition)
            where TRegexFAState : IRegexFSMState<T, BasicRegexFATransition<T, TRegexFAState>>
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return transition.Set;
        }
        #endregion
    }
}
