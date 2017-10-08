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

        #region ActivateRegexDFAFromDumplication
        public static readonly MethodShuntSource ActivateRegexDFAFromDumplicationSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod("ActivateRegexDFAFromDumplication"));

        public virtual TRegexDFA ActivateRegexDFAFromDumplication<TRegexDFA>(TRegexDFA dfa) where TRegexDFA : IRegexDFA<T>
        {
            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFAFromDumplicationSource, dfa);
            if (result.Success)
                return (TRegexDFA)result.ReturnValue;
            else
                throw new NotSupportedException($"不支持的确定的正则构造的有限自动机的类型：{typeof(TRegexDFA)}");
        }
        #endregion

        public virtual IRegexDFAState<T> ActivateRegexDFAState(bool isTerminal = false) =>
            isTerminal ?
                CommonRegexFAState<T>.TerminalDFAState :
                CommonRegexFAState<T>.DFAState;

        #region ActivateRegexDFAStateFromDumplication
        public static readonly MethodShuntSource ActivateRegexDFAStateFromDumplicationSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexDFAStateFromDumplication)));

        public virtual TRegexDFAState ActivateRegexDFAStateFromDumplication<TRegexDFAState>(TRegexDFAState state) where TRegexDFAState : IRegexDFAState<T>
        {
            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFAStateFromDumplicationSource, state);
            if (result.Success)
                return (TRegexDFAState)result.ReturnValue;
            else
                throw new NotSupportedException($"不支持的确定的正则构造的有限自动机的状态的类型：{typeof(TRegexDFAState)}");
        }
        #endregion

        #region ActivateRegexDFATransitionFromAccreditedSet
        public static readonly MethodShuntSource ActivateRegexDFATransitionFromAccreditedSetSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexDFATransitionFromAccreditedSet), new[] { typeof(ISet<T>) }));

        public virtual IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(ISet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFATransitionFromAccreditedSetSource, set);
            if (result.Success)
                return (IAcceptInputTransition<T>)result.ReturnValue;
            else
                throw new NotSupportedException($"不支持的集的类型：{set.GetType()}");
        }

        protected static readonly MethodShuntKey ActivateRegexDFATransitionFromAccreditedSetKey_RangeSet = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexDFATransitionFromAccreditedSetSource,
            typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexDFATransitionFromAccreditedSet), new[] { typeof(RangeSet<T>) })
        );

        protected virtual IAcceptInputTransition<T> ActivateRegexDFATransitionFromAccreditedSet(RangeSet<T> rangeSet)
        {
            if (rangeSet == null) throw new ArgumentNullException(nameof(rangeSet));

            return BasicRegexFATransition<T>.Adapt(
                new RangeSetRegexFATransition<T, BasicRegexDFAState<T>>(rangeSet)
            );
        }
        #endregion

        public virtual IRegexFSMEpsilonTransition<T> ActivateRegexFSMEpsilonTransition() => new RegexFSMEpsilonTransition<T>();

        public virtual IRegexNFA<T> ActivateRegexNFA() => new RegexNFA<T>();

        #region ActivateRegexNFAFromDumplication
        public static readonly MethodShuntSource ActivateRegexNFAFromDumplicationSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexNFAFromDumplication)));

        public virtual TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa) where TRegexNFA : IRegexNFA<T>
        {
            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFAFromDumplicationSource, nfa);
            if (result.Success)
                return (TRegexNFA)result.ReturnValue;
            else
                throw new NotSupportedException($"不支持的非确定的正则构造的有限自动机的类型：{nfa.GetType()}");
        }
        #endregion

        public virtual IRegexNFAState<T> ActivateRegexNFAState(bool isTerminal = false) =>
            isTerminal ?
                CommonRegexFAState<T>.TerminalNFAState :
                CommonRegexFAState<T>.NFAState;

        #region ActivateRegexNFAStateFromDumplication
        public static readonly MethodShuntSource ActivateRegexNFAStateFromDumplicationSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexNFAStateFromDumplication)));

        public virtual TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state) where TRegexNFAState : IRegexNFAState<T>
        {
            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFAStateFromDumplicationSource, state);
            if (result.Success)
                return (TRegexNFAState)result.ReturnValue;
            else
                throw new NotSupportedException($"不支持的非确定的正则构造的有限自动机的状态的类型：{state.GetType()}");
        }
        #endregion

        #region ActivateRegexNFATransitionFromDumplication
        public static readonly MethodShuntSource ActivateRegexNFATransitionFromDumplicationSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexNFATransitionFromDumplication)));

        public virtual TRegexNFATransition ActivateRegexNFATransitionFromDumplication<TRegexNFATransition>(TRegexNFATransition transition) where TRegexNFATransition : IRegexFSMTransition<T>
        {
            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFATransitionFromDumplicationSource, transition);
            if (result.Success)
                return (TRegexNFATransition)result.ReturnValue;
            else
                throw new NotSupportedException($"不支持的非确定的正则构造的有限自动机的转换的类型：{transition.GetType()}");
        }
        #endregion

        #region ActivateRegexNFATransitionFromRegexCondition
        public static readonly MethodShuntSource ActivateRegexNFATransitionFromRegexConditionSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(ActivateRegexNFATransitionFromRegexCondition), new[] { typeof(RegexCondition<T>) }));

        public virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            var result = this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFATransitionFromRegexConditionSource, regex);
            if (result.Success)
                return (IAcceptInputTransition<T>)result.ReturnValue;
            else
                return new BasicRegexFATransition<T>(regex.Condition);
        }

        protected static readonly MethodShuntKey ActivateRegexNFATransitionFromRegexConditionKey_RegexConst = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFATransitionFromRegexConditionSource,
            typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(
                nameof(ActivateRegexNFATransitionFromRegexConst),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            )
        );
        
        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexConst(RegexConst<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return new BasicRegexFATransition<T>(t => this.AccreditedSet.Any(_t => regex.Condition(_t)));
        }

        protected static readonly MethodShuntKey ActivateRegexNFATransitionFromRegexConditionKey_RegexRange = MethodShunt.Register(
            RegexStateMachineActivationContextInfoBase<T>.ActivateRegexNFATransitionFromRegexConditionSource,
            typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(
                nameof(ActivateRegexNFATransitionFromRegexRange),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            )
        );

        protected virtual IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexRange(RegexRange<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return new BasicRegexFATransition<T>(regex.Condition);
        }
        #endregion

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

        #region GetAccreditedSetFromRegexAcceptInputTransition
        public static readonly MethodShuntSource GetAccreditedSetFromRegexAcceptInputTransitionSource = MethodShunt.CreateSource(typeof(RegexStateMachineActivationContextInfoBase<T>).GetMethod(nameof(GetAccreditedSetFromRegexAcceptInputTransition)));

        public virtual ISet<T> GetAccreditedSetFromRegexAcceptInputTransition(IAcceptInputTransition<T> transition)
        {
            return (ISet<T>)this.DynamicInvokeShunt(RegexStateMachineActivationContextInfoBase<T>.GetAccreditedSetFromRegexAcceptInputTransitionSource, transition);
        }
        #endregion

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
