using SamLu.RegularExpression;
using SamLu.RegularExpression.Adapter;
using SamLu.RegularExpression.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();
            var chars = Regex.Range('\0', 'z', true, false);
            var tchars = Regex.Range(new TT<char>('a'), new TT<char>('z'), false, true);

            var phone = Regex.Range(0, 9).RepeatMany(10);
            
            Func<int, int, RegexRange<string>> func = (min, max) =>
                new RegexRangeAdaptor<int, string>(
                    min, max,
                    (source => source.ToString()),
                    (target => int.Parse(target))
                );
            var section = func(0, 255);
            var dot = Regex.Const(".");
            var colon = Regex.Const(":");
            var port = func(0, 9999);

            var ipAddress = new RegexObject<string>[] { section, dot, section, dot, section, dot, section, new RegexObject<string>[] { colon, port }.ConcatMany().Optional() }.ConcatMany();

            IRegexFAProvider<string> provider = new RegexFAProvider<string>(new MyRegexRunContextInfo());

            RegexNFA<string> nfa = provider.GenerateNFAFromRegexObject(ipAddress);
            RegexDFA<string> dfa = provider.GenerateDFAFromNFA(nfa);
            ;
        }

        public class MyRegexFAProvider<T> : RegexFAProvider<T>
        {
            public MyRegexFAProvider(IRegexRunContextInfo<T> contextInfo) : base(contextInfo) { }

            protected override RegexFATransition<T, RegexNFAState<T>> GenerateNFATransitionFromRegexCondition(RegexCondition<T> condition, RegexNFA<T> nfa, RegexNFAState<T> state)
            {
                return base.GenerateNFATransitionFromRegexCondition(condition, nfa, state);
            }
        }

        public class MyRegexNFATransition<T> : RegexFATransition<T, RegexNFAState<T>>
        {
            private ISet<T> set;
            protected Predicate<T> predicate;

            public override Predicate<T> Predicate => this.predicate;

            protected MyRegexNFATransition() : base() { }

            public MyRegexNFATransition(ISet<T> set) : this()
            {
                if (set == null) throw new ArgumentNullException(nameof(set));

                this.set = set;
                predicate = t => this.set.Contains(t);
            }
        }

        public class MyRegexNFATransitionAdaptor<TSource, TTarget> : MyRegexNFATransition<TTarget>, IAdaptor<TSource, TTarget>
        {
            private ISet<TSource> set;
            private AdaptContextInfo<TSource, TTarget> contextInfo;

            public virtual AdaptContextInfo<TSource, TTarget> ContextInfo => this.contextInfo;

            protected MyRegexNFATransitionAdaptor() : base() { }

            public MyRegexNFATransitionAdaptor(ISet<TSource> set, AdaptContextInfo<TSource, TTarget> contextInfo) : this()
            {
                this.set = set;
                this.contextInfo = contextInfo;

                base.predicate = target =>
                  {
                      if (this.contextInfo.TryAdaptTarget(target, out TSource source))
                          return this.set.Contains(source);
                      else return false;
                  };
            }
        }

        public class MyRegexRunContextInfo : IRegexRunContextInfo<string>
        {
            private HashSet<string> set;

            public ISet<string> AccreditedSet => this.set;

            public MyRegexRunContextInfo()
            {
                this.set = new HashSet<string>();
                set.Add(".");
                foreach (var item in Enumerable.Range(0, 256).Select(i => i.ToString()))
                    set.Add(item);
            }

            public RegexDFA<string> ActivateRegexDFA()
            {
                return new RegexDFA<string>();
            }

            public RegexDFAState<string> ActivateRegexDFAState(bool isTerminal = false)
            {
                return new RegexDFAState<string>(isTerminal);
            }

            public RegexFATransition<string, RegexDFAState<string>> ActivateRegexDFATransitionFromAccreditedSet(ISet<string> set)
            {
                if (set == null) throw new ArgumentNullException(nameof(set));

                return new RegexFATransition<string, RegexDFAState<string>>(s => set.Contains(s));
            }

            public RegexNFA<string> ActivateRegexNFA()
            {
                return new RegexNFA<string>();
            }

            public RegexNFA<string> ActivateRegexNFAFromDumplication(RegexNFA<string> nfa)
            {
                if (nfa == null) throw new ArgumentNullException(nameof(nfa));

                return this.ActivateRegexNFA();
            }

            public RegexNFAEpsilonTransition<string> ActivateRegexNFAEpsilonTransition()
            {
                return new RegexNFAEpsilonTransition<string>();
            }

            public RegexNFAState<string> ActivateRegexNFAState(bool isTerminal = false)
            {
                return new RegexNFAState<string>(isTerminal);
            }

            public RegexNFAState<string> ActivateRegexNFAStateFromDumplication(RegexNFAState<string> state)
            {
                if (state == null) throw new ArgumentNullException(nameof(state));

                return this.ActivateRegexNFAState();
            }

            public RegexFATransition<string, RegexNFAState<string>> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<string> regex)
            {
                if (regex == null) throw new ArgumentNullException(nameof(regex));

                return new RegexFATransition<string, RegexNFAState<string>>(s => regex.Condition(s));
            }

            public RegexFATransition<string, RegexNFAState<string>> ActivateRegexNFATransitionFromDumplication(RegexFATransition<string, RegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                return new RegexFATransition<string, RegexNFAState<string>>(transition.Predicate);
            }

            public RegexFATransition<string, RegexDFAState<string>> CombineRegexDFATransitions(IEnumerable<RegexFATransition<string, RegexDFAState<string>>> dfaTransitions)
            {
                throw new NotImplementedException();
            }

            public ISet<string> GetAccreditedSetFromRegexNFATransition(RegexFATransition<string, RegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                return new HashSet<string>(this.AccreditedSet.Where(item => transition.Predicate(item)));
            }

            public ISet<string> GetAccreditedSetExceptResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                return new SamLu.Collections.ObjectModel.SetGroup<string>(
                    new[] { first, second },
                    (predications => predications.Count() == 2 && (predications.First() && !predications.Last()))
                );
            }

            public ISet<string> GetAccreditedSetIntersectResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                return new SamLu.Collections.ObjectModel.SetGroup<string>(
                    new[] { first, second },
                    SamLu.Collections.ObjectModel.SetGroup<string>.IntersectGroupPredicate
                );
            }

            public ISet<string> GetAccreditedSetSymmetricExceptResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                return new SamLu.Collections.ObjectModel.SetGroup<string>(
                    new[] { first, second },
                    (predications => predications.Count() == 2 && (predications.First() ^ predications.Last()))
                );
            }

            public ISet<string> GetAccreditedSetUnionResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                return new SamLu.Collections.ObjectModel.SetGroup<string>(
                    new[] { first, second },
                    SamLu.Collections.ObjectModel.SetGroup<string>.UnionGroupPredicate
                );
            }
        }

        [System.Diagnostics.DebuggerDisplay("{t}")]
        struct TT<T> : IComparable<TT<T>>, IEquatable<TT<T>>
            where T : IComparable<T>, IEquatable<T>
        {
            private T t;

            public TT(T t) => this.t = t;

            public int CompareTo(TT<T> other)
            {
                return this.t.CompareTo(other.t);
            }

            public bool Equals(TT<T> other)
            {
                return this.t.Equals(other.t);
            }

            public override string ToString()
            {
                return this.t.ToString();
            }
        }
    }
}
