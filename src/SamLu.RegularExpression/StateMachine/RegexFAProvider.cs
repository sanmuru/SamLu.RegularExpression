using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    internal class RegexFAProvider<T> : IRegexFAProvider<T>
    {
        private IRegexRunContextInfo<T> contextInfo;
        public IRegexRunContextInfo<T> ContextInfo => this.contextInfo;

        public RegexFAProvider(IRegexRunContextInfo<T> contextInfo)
        {
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.contextInfo = contextInfo;
        }

        public RegexNFA<T> GenerateNFAFromRegexObject(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            RegexNFA<T> nfa = this.contextInfo.ActivateRegexNFA();
            RegexNFAState<T> startState = this.contextInfo.ActivateRegexNFAState();
            nfa.StartState = startState;

            RegexFATransition<T, RegexNFAState<T>> transition = this.GenerateNFATransitionFromRegexObject(regex, nfa, startState);
            RegexNFAState<T> endState = this.contextInfo.ActivateRegexNFAState(true);

            nfa.SetTarget(transition, endState);

            return nfa;
        }

        protected virtual RegexFATransition<T, RegexNFAState<T>> GenerateNFATransitionFromRegexObject(
            RegexObject<T> regex,
            RegexNFA<T> nfa,
            RegexNFAState<T> state
        )
        {
            if (regex is RegexCondition<T> condition)
            {
                RegexFATransition<T, RegexNFAState<T>> transition = this.contextInfo.ActivateRegexNFATransitionFromRegexCondition(condition);
                nfa.AttachTransition(state, transition);

                return transition;
            }
            else if (regex is RegexRepeat<T> repeat)
                return this.GenerateNFATransitionFromRegexRepeat(repeat, nfa, state);
            else if (regex is RegexSeries<T> series)
            {
                RegexNFAState<T> nextState = state;
                foreach (var item in series.Series)
                {
                    var transition = this.GenerateNFATransitionFromRegexObject(item, nfa, nextState);
                    nextState = this.contextInfo.ActivateRegexNFAState();
                }

                RegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                nfa.AttachTransition(nextState, epsilonTransition);

                return epsilonTransition;
            }
            else if (regex is RegexParallels<T> parallels)
            {
                RegexNFAState<T> endState = this.contextInfo.ActivateRegexNFAState();

                foreach (var item in parallels.Parallels)
                {
                    RegexFATransition<T, RegexNFAState<T>> transition = this.GenerateNFATransitionFromRegexObject(item, nfa, state);
                    nfa.AttachTransition(state, transition);
                }

                RegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                nfa.AttachTransition(endState, epsilonTransition);

                return epsilonTransition;
            }
            else throw new NotSupportedException(string.Format("不支持的正则类型：{0}", regex.GetType()));
        }

        protected virtual RegexFATransition<T, RegexNFAState<T>> GenerateNFATransitionFromRegexRepeat(
            RegexRepeat<T> repeat,
            RegexNFA<T> nfa,
            RegexNFAState<T> state
        )
        {
            ulong count;
            if (repeat.IsInfinte)
                count = repeat.MinimumCount ?? ulong.MinValue;
            else
                count = repeat.MaximumCount.Value;

            var transition = this.GenerateNFATransitionFromRegexRepeatInternal(repeat.InnerRegex, nfa, state, count, out IList<RegexNFAState<T>> nodes);

            if (repeat.IsInfinte)
            {
                RegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                var reversedNodes = nodes.Reverse();
                nfa.AttachTransition(reversedNodes.First(), epsilonTransition);
                nfa.SetTarget(epsilonTransition, reversedNodes.Skip(1).First());
            }
            else
            {
                if (count > 0)
                {
                    RegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                    foreach (var _state in nodes.Take((int)(repeat.MaximumCount.Value - (repeat.MaximumCount ?? ulong.MinValue))))
                        nfa.AttachTransition(_state, epsilonTransition);

                    nfa.SetTarget(transition, nodes.Reverse().ElementAt((int)(repeat.MinimumCount ?? ulong.MinValue)));
                }
            }

            return transition;
        }

        private RegexFATransition<T, RegexNFAState<T>> GenerateNFATransitionFromRegexRepeatInternal(
            RegexObject<T> innerRegex,
            RegexNFA<T> nfa,
            RegexNFAState<T> state,
            ulong count,
            out IList<RegexNFAState<T>> nodes)
        {
            nodes = new List<RegexNFAState<T>> { state };

            RegexNFAState<T> nextState = state;
            for (ulong index = ulong.MinValue; index < count; index++)
            {
                RegexFATransition<T, RegexNFAState<T>> transition = this.GenerateNFATransitionFromRegexObject(innerRegex, nfa, nextState);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(transition, nextState);

                nodes.Add(nextState);
            }

            RegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
            nfa.AttachTransition(nextState, epsilonTransition);

            return epsilonTransition;
        }

        public RegexDFA<T> GenerateDFAFromNFA(RegexNFA<T> nfa)
        {
            return nfa.ToDFA(this.contextInfo);
        }
    }
}
