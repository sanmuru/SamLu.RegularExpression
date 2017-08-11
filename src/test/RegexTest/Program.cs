using SamLu.Collections.ObjectModel;
using SamLu.RegularExpression;
using SamLu.RegularExpression.Adapter;
using SamLu.RegularExpression.ObjectModel;
using SamLu.RegularExpression.StateMachine;
using SamLu.StateMachine;
using SamLu.StateMachine.Diagnostics;
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

            Func<int, RegexRepeat<char>> func = (count) =>
                  Regex.Range('0', '9').Repeat(1, (ulong)count);
            var section = func(3);
            var dot = Regex.Const('.');
            var colon = Regex.Const(':');
            var port = func(4);
            var ipAddress = new RegexObject<char>[] { section, dot, section, dot, section, dot, section, new RegexObject<char>[] { colon, port }.ConcatMany().Optional() }.ConcatMany();

            IRegexFAProvider<char> char_Provider = new RegexFAProvider<char>(new MyCharRegexRunContextInfo());

            Action<RegexObject<char>> action =
                regexObj =>
                {
                    var ___nfa = char_Provider.GenerateNFAFromRegexObject(regexObj);
                    var ___dfa = char_Provider.GenerateDFAFromNFA(___nfa);
                };
            action?.Invoke(Regex.Const('a').Optional().Concat(Regex.Const('b').Concat(Regex.Const('c').Optional())));

            RegexNFA<char> char_nfa = char_Provider.GenerateNFAFromRegexObject(ipAddress);
            var debuginfo = char_nfa.GetDebugInfo();
            RegexDFA<char> char_dfa = char_Provider.GenerateDFAFromNFA(char_nfa);
            ;

            Func<int, int, RegexRange<string>> func_adpator = (min, max) =>
               new RegexRangeAdaptor<int, string>(
                   min, max,
                   (source => source.ToString()),
                   (target => int.Parse(target))
               );
            var section_adpator = func_adpator(0, 255);
            var dot_adaptor = new RegexConstAdaptor<char, string>('.', (source => source.ToString()), (target => target[0]));
            var colon_adaptor = new RegexConstAdaptor<char, string>(':', (source => source.ToString()), (target => target[0]));
            var port_adaptor = func_adpator(0, 9999);

            var ipAddress_adaptor =
                new RegexObject<string>[] { section_adpator, dot_adaptor, section_adpator, dot_adaptor, section_adpator, dot_adaptor, section_adpator, new RegexObject<string>[] { colon_adaptor, port_adaptor }.ConcatMany().Optional() }.ConcatMany();

            IRegexFAProvider<string> string_Provider = new RegexFAProvider<string>(new MyStringRegexRunContextInfo());
            
            RegexNFA<string> string_nfa = string_Provider.GenerateNFAFromRegexObject(ipAddress_adaptor);
            RegexDFA<string> string_dfa = string_Provider.GenerateDFAFromNFA(string_nfa);
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

            public override string ToString()
            {
                return $"< {this.set} >";
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

            public override string ToString()
            {
                return $"< {this.set} >";
            }
        }

        public class MyCharRegexRunContextInfo : IRegexRunContextInfo<char>
        {
            private ISet<char> set;

            public ISet<char> AccreditedSet => this.set;

            public MyCharRegexRunContextInfo()
            {
                this.set = new CharRangeSet();
            }

            public RegexDFA<char> ActivateRegexDFA()
            {
                return new RegexDFA<char>();
            }

            public RegexDFAState<char> ActivateRegexDFAState(bool isTerminal = false)
            {
                return new RegexDFAState<char>(isTerminal);
            }

            public RegexFATransition<char, RegexDFAState<char>> ActivateRegexDFATransitionFromAccreditedSet(ISet<char> set)
            {
                if (set == null) throw new ArgumentNullException(nameof(set));

                return new RegexFATransition<char, RegexDFAState<char>>(c => set.Contains(c));
            }

            public RegexNFA<char> ActivateRegexNFA()
            {
                return new RegexNFA<char>();
            }

            public RegexNFAEpsilonTransition<char> ActivateRegexNFAEpsilonTransition()
            {
                return new RegexNFAEpsilonTransition<char>();
            }

            public RegexNFA<char> ActivateRegexNFAFromDumplication(RegexNFA<char> nfa)
            {
                return new RegexNFA<char>();
            }

            public RegexNFAState<char> ActivateRegexNFAState(bool isTerminal = false)
            {
                return new RegexNFAState<char>(isTerminal);
            }

            public RegexNFAState<char> ActivateRegexNFAStateFromDumplication(RegexNFAState<char> state)
            {
                return new RegexNFAState<char>(state.IsTerminal);
            }

            public RegexFATransition<char, RegexNFAState<char>> ActivateRegexNFATransitionFromDumplication(RegexFATransition<char, RegexNFAState<char>> transition)
            {
                if (transition is RangeRegexNFATransition range)
                    return new RangeRegexNFATransition(range.Range);
                else if (transition is SetRegexNFATransition set)
                    return new SetRegexNFATransition(set.Set);
                else return new RegexFATransition<char, RegexNFAState<char>>(transition.Predicate);
            }

            public class SetRegexNFATransition : RegexFATransition<char, RegexNFAState<char>>
            {
                private ISet<char> set;
                public ISet<char> Set => this.set;

                public SetRegexNFATransition(ISet<char> set) :
                    base(
                        (set ?? throw new ArgumentNullException(nameof(set))).Contains
                    )
                {
                    this.set = set;
                }

                public override string ToString()
                {
                    return $"< {this.set} >";
                }
            }

            [DebugInfoProxy(typeof(RangeRegexNFATransition._DebugInfo))]
            public class RangeRegexNFATransition : RegexFATransition<char, RegexNFAState<char>>
            {
                private IRange<char> range;
                public IRange<char> Range => this.range;

                public RangeRegexNFATransition(IRange<char> range) :
                    base(
                        new Func<IRange<char>, Predicate<char>>((_range) =>
                        {
                            return c =>
                                (_range.CanTakeMinimum ?
                                    _range.Comparison(_range.Minimum, c) <= 0 :
                                    _range.Comparison(_range.Minimum, c) < 0) &&
                                (_range.CanTakeMaximum ?
                                    _range.Comparison(c, _range.Maximum) <= 0 :
                                    _range.Comparison(c, _range.Maximum) < 0);
                        })(range ?? throw new ArgumentNullException(nameof(range)))
                    )
                {
                    this.range = range;
                }

                public class _DebugInfo
                {
                    private RangeRegexNFATransition transition;

                    public static string QuoteChar(char c)
                    {
                        switch (c)
                        {
                            case '\0': return "'\\0'";
                            case '\\': return "'\\'";
                            case '\'': return "'\\''";
                            case '"': return "'\"'";
                            case '\a': return "'\\a'";
                            case '\b': return "'\\b'";
                            case '\t': return "'\\t'";
                            case '\f': return "'\\f'";
                            case '\v': return "'\\v'";
                            case '\r': return "'\\r'";
                            case '\n': return "'\\n'";
                            default: return $"'{c}'";
                        }
                    }

                    public string DebugInfo
                    {
                        get
                        {
                            if (
                                this.transition.Range.Comparison(this.transition.Range.Minimum, this.transition.Range.Maximum) == 0 &&
                                    (this.transition.Range.CanTakeMinimum && this.transition.Range.CanTakeMaximum)
                            )
                                return $"< {_DebugInfo.QuoteChar(this.transition.Range.Minimum)} >";
                            else
                                return $"< {(this.transition.Range.CanTakeMinimum ? '[' : '(')}{_DebugInfo.QuoteChar(this.transition.Range.Minimum)},{_DebugInfo.QuoteChar(this.transition.Range.Maximum)}{(this.transition.Range.CanTakeMaximum ? ']' : ')')} >";
                        }
                    }

                    public _DebugInfo(RangeRegexNFATransition transition, params object[] args)
                    {
                        this.transition = transition ?? throw new ArgumentNullException(nameof(transition));
                    }
                }

                public override string ToString()
                {
                    return $"< {(this.range.CanTakeMinimum ? '[' : '(')}{this.range.Minimum},{this.range.Maximum}{(this.range.CanTakeMaximum ? ']' : ')')} >";
                }
            }

            public RegexFATransition<char, RegexNFAState<char>> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<char> regex)
            {
                if (regex is IRange<char> range)
                    return new RangeRegexNFATransition(range);
                else
                    return new RegexFATransition<char, RegexNFAState<char>>(regex.Condition);
            }

            public RegexFATransition<char, RegexDFAState<char>> CombineRegexDFATransitions(IEnumerable<RegexFATransition<char, RegexDFAState<char>>> dfaTransitions)
            {
                throw new NotImplementedException();
            }

            public ISet<char> GetAccreditedSetFromRegexNFATransition(RegexFATransition<char, RegexNFAState<char>> transition)
            {
                if (transition is SetRegexNFATransition set)
                    return set.Set;
                else if (transition is RangeRegexNFATransition range)
                    return new CharRangeSet(range.Range.Minimum, range.Range.Maximum, range.Range.CanTakeMinimum, range.Range.CanTakeMaximum);
                else
                    return new HashSet<char>(this.AccreditedSet.Where(c => transition.Predicate(c)));
            }

            public ISet<char> GetAccreditedSetExceptResult(ISet<char> first, ISet<char> second)
            {
                return new SetGroup<char>(new[] { first, second }, SetGroup<char>.ExceptGroupPredicate);
            }

            public ISet<char> GetAccreditedSetIntersectResult(ISet<char> first, ISet<char> second)
            {
                return new SetGroup<char>(new[] { first, second }, SetGroup<char>.IntersectGroupPredicate);
            }

            public ISet<char> GetAccreditedSetSymmetricExceptResult(ISet<char> first, ISet<char> second)
            {
                return new SetGroup<char>(new[] { first, second }, SetGroup<char>.SymmetricExceptGroupPredicate);
            }

            public ISet<char> GetAccreditedSetUnionResult(ISet<char> first, ISet<char> second)
            {
                return new SetGroup<char>(new[] { first, second }, SetGroup<char>.UnionGroupPredicate);
            }
        }

        public class MyStringRegexRunContextInfo : IRegexRunContextInfo<string>
        {
            private HashSet<string> set;

            public ISet<string> AccreditedSet => this.set;

            public MyStringRegexRunContextInfo()
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

                return this.ActivateRegexNFAState(state.IsTerminal);
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
