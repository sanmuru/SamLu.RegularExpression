using SamLu.Collections.ObjectModel;
using SamLu.Diagnostics;
using SamLu.RegularExpression;
using SamLu.RegularExpression.Adapter;
using SamLu.RegularExpression.Extend;
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
            Program.TestFunctionalTransitions(
                Regex.Range('0', '9').NoneOrMany().NonGreedy() +
                Regex.Range('0', '9').Many().NonGreedy().Group("sec", true).GroupReference(out RegexGroupReference<char> sec_GroupReference) +
                sec_GroupReference
            );

#if false
            RangeSet<char> set = new RangeSet<char>(new CharRangeInfo());
            set.Add('a');
            set.Add('c');
            ;

            Dictionary<int, int> d = new Dictionary<int, int>();
            var chars = Regex.Range('\0', 'z', true, false);
            var tchars = Regex.Range(new TT<char>('a'), new TT<char>('z'), false, true);

            var phone = Regex.Range(0, 9).RepeatMany(10);

            Func<int, RegexObject<char>> func = (count) =>
            {
                var junkPrefix = Regex.Const('0').NoneOrMany();
                Func<int, char> convertor = Convert.ToChar;
                return junkPrefix + Enumerable.Range(0, count).Select(num => num.ToString().Select(c => Regex.Const(c)).ConcatMany()).UnionMany();
            };
            var section = func(255);
            var dot = Regex.Const('.');
            var colon = Regex.Const(':');
            var port = func(9999);
            var ipAddress = new RegexObject<char>[] { section, dot, section, dot, section, dot, section, new RegexObject<char>[] { colon, port }.ConcatMany().Optional() }.ConcatMany();

            IRegexFAProvider<char> char_Provider = new RegexFAProvider<char>(new MyCharRegexRunContextInfo());

            var char_nfa = char_Provider.GenerateRegexFSMFromRegexObject(ipAddress, RegexOptions.None);
            //var debuginfo = char_nfa.GetDebugInfo();
            var char_dfa = char_Provider.GenerateRegexDFAFromRegexFSM(char_nfa);
            ;

            Action<RegexObject<char>> action =
                regexObj =>
                {
                    var ___nfa = char_Provider.GenerateRegexFSMFromRegexObject(regexObj, RegexOptions.None);
                    var ___dfa = char_Provider.GenerateRegexDFAFromRegexFSM(___nfa);

                    IEnumerable<char> inputs = Enumerable.Repeat<Func<int, int>>(new Random().Next, 25).Select(nextFunc => (char)('a' - 1 + nextFunc(4)));
                    char[] ___charArray = inputs.ToArray();
                    IRegexFSM<char> ___fsm;
#if false
                    ___fsm = new RegexFSM<char>() { StartState = ___dfa.StartState };
#else
                    ___fsm = ___dfa;
#endif
                    ___fsm.TransitMany(___charArray);

                    var ___matches = ___fsm.Matches;
                };
            action?.Invoke(Regex.Const('a').Optional().Concat(Regex.Const('b').Concat(Regex.Const('c').Optional())));
            
#endif

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

            var string_nfa = string_Provider.GenerateRegexFSMFromRegexObject(ipAddress_adaptor, RegexOptions.None);
            var string_dfa = string_Provider.GenerateRegexDFAFromRegexFSM(string_nfa);
            ;

            Random random = new Random();
            int 样本树 = 100;
            var ipAddressStrFragments =
                Enumerable.Repeat(
                    new Tuple<Func<int>, Func<double>>(
                        (() => random.Next(4, 6)),
                        (() => random.NextDouble() * 1.15)
                    ),
                    样本树
                )
                .Select(tuple =>
                {
                    int groupCount = tuple.Item1();
                    int sectionMax = 255;
                    int portMax = 9999;
                    return
                        Enumerable.Repeat<IEnumerable<string>>(
                            new string[]
                            {
                                ((int)(sectionMax*tuple.Item2())).ToString(),
                                "."
                            },
                            3)
                        .Aggregate((ss1, ss2) => ss1.Concat(ss2))
                        .Concat(groupCount > 3 ?
                            new string[] { ((int)(sectionMax * tuple.Item2())).ToString() }
                                .Concat(groupCount > 4 ?
                                    new string[]
                                    {
                                        ":",
                                        ((int)(portMax*tuple.Item2())).ToString()
                                    } :
                                    Enumerable.Empty<string>()
                                ) :
                            Enumerable.Empty<string>()
                        );
                });
#if false
            IRegexFSM<char> char_fsm = char_dfa;
            var matchesArray =
                ipAddressStrFragments
                    .Select(ipAddressStrFragment => string.Join(string.Empty, ipAddressStrFragment))
                    .Select(ipAddressStr =>
                    {
                        char_fsm.TransitMany(ipAddressStr);
                        return char_fsm.Matches;
                    })
                    .ToArray();
            ;
#else
            IRegexFSM<string> string_fsm = string_dfa;
            var matchesArray =
                ipAddressStrFragments
                .Select(ipAddressStr =>
                {
                    string_fsm.TransitMany(ipAddressStr);
                    return string_fsm.Matches;
                })
                .ToArray();
            ;
#endif
        }

        private static void TestFunctionalTransitions(RegexObject<char> regex)
        {
            RegexFAProvider<char> provider = new MyRegexFAProvider<char>(new MyCharRegexRunContextInfo());
            var nfa = provider.GenerateRegexFSMFromRegexObject(regex, RegexOptions.None);
            var dfa = provider.GenerateRegexDFAFromRegexFSM(nfa);
            IRegexFSM<char> fsm = dfa;
            ;

            var input = "1234564567";
            fsm.TransitMany(input);
            MatchCollection<char> matches = fsm.Matches;
            ;
        }

        public class MyRegexFAProvider<T> : RegexFAProvider<T>
        {
            public MyRegexFAProvider(IRegexStateMachineActivationContextInfo<T> contextInfo) : base(contextInfo) { }
        }

#region char
        public class MyRegexNFATransition<T> : BasicRegexFATransition<T, BasicRegexNFAState<T>>
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

        public class MyCharRegexRunContextInfo : IRegexStateMachineActivationContextInfo<char>
        {
            private ISet<char> set;

            public ISet<char> AccreditedSet => this.set;

            public MyCharRegexRunContextInfo()
            {
                this.set = new CharRangeSet();
            }

            public IRegexNFA<char> ActivateRegexNFA()
            {
                return new BasicRegexNFA<char>();
            }

            public TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa)
                where TRegexNFA : IRegexNFA<char>
            {
                if (typeof(BasicRegexNFA<char>).IsAssignableFrom(typeof(TRegexNFA)))
                    return (TRegexNFA)(object)this.ActivateRegexNFAFromDumplication((BasicRegexNFA<char>)(object)nfa);
                else
                    return nfa;
            }

            private BasicRegexNFA<char> ActivateRegexNFAFromDumplication(BasicRegexNFA<char> nfa)
            {
                return new BasicRegexNFA<char>();
            }

            public IRegexDFA<char> ActivateRegexDFA()
            {
                return new BasicRegexDFA<char>();
            }

            public TRegexDFA ActivateRegexDFAFromDumplication<TRegexDFA>(TRegexDFA nfa)
                where TRegexDFA : IRegexDFA<char>
            {
                if (typeof(BasicRegexDFA<char>).IsAssignableFrom(typeof(TRegexDFA)))
                    return (TRegexDFA)(object)this.ActivateRegexDFAFromDumplication((BasicRegexDFA<char>)(object)nfa);
                else
                    return nfa;
            }

            private BasicRegexDFA<char> ActivateRegexDFAFromDumplication(BasicRegexDFA<char> nfa)
            {
                return new BasicRegexDFA<char>();
            }

            public IRegexNFAState<char> ActivateRegexNFAState(bool isTerminal = false)
            {
                return new BasicRegexNFAState<char>(isTerminal);
            }

            public TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state)
                where TRegexNFAState : IRegexNFAState<char>
            {
                if (typeof(BasicRegexNFAState<char>).IsAssignableFrom(typeof(TRegexNFAState)))
                    return (TRegexNFAState)(object)this.ActivateRegexNFAStateFromDumplication((BasicRegexNFAState<char>)(object)state);
                else
                    return state;
            }

            private BasicRegexNFAState<char> ActivateRegexNFAStateFromDumplication(BasicRegexNFAState<char> state)
            {
                return new BasicRegexNFAState<char>(state.IsTerminal);
            }

            public IRegexDFAState<char> ActivateRegexDFAState(bool isTerminal = false)
            {
                return new BasicRegexDFAState<char>(isTerminal);
            }

            public TRegexNFATransition ActivateRegexNFATransitionFromDumplication<TRegexNFATransition>(TRegexNFATransition transition)
                where TRegexNFATransition : IRegexFSMTransition<char>
            {
                if (typeof(BasicRegexFATransition<char, BasicRegexNFAState<char>>).IsAssignableFrom(typeof(TRegexNFATransition)))
                    return (TRegexNFATransition)(object)this.ActivateRegexNFATransitionFromDumplication((BasicRegexFATransition<char, BasicRegexNFAState<char>>)(object)transition);
                else
                    return transition;
            }

            private BasicRegexFATransition<char, BasicRegexNFAState<char>> ActivateRegexNFATransitionFromDumplication(BasicRegexFATransition<char, BasicRegexNFAState<char>> transition)
            {
                if (transition is RangeRegexNFATransition range)
                    return new RangeRegexNFATransition(range.Range);
                else if (transition is SetRegexNFATransition set)
                    return new SetRegexNFATransition(set.Set);
                else return new BasicRegexFATransition<char, BasicRegexNFAState<char>>(transition.Predicate);
            }

            public IRegexFSMEpsilonTransition<char> ActivateRegexFSMEpsilonTransition()
            {
                return new BasicRegexFSMEpsilonTransition<char>();
            }

            public TRegexDFAState ActivateRegexDFAStateFromDumplication<TRegexDFAState>(TRegexDFAState state)
                where TRegexDFAState : IRegexDFAState<char>
            {
                if (typeof(BasicRegexDFAState<char>).IsAssignableFrom(typeof(TRegexDFAState)))
                    return (TRegexDFAState)(object)this.ActivateRegexDFAStateFromDumplication((BasicRegexDFAState<char>)(object)state);
                else
                    return state;
            }

            private BasicRegexDFAState<char> ActivateRegexDFAStateFromDumplication(BasicRegexDFAState<char> state)
            {
                return new BasicRegexDFAState<char>(state.IsTerminal);
            }

            public IAcceptInputTransition<char> ActivateRegexDFATransitionFromAccreditedSet(ISet<char> set)
            {
                if (set == null) throw new ArgumentNullException(nameof(set));

                return new RangeSetRegexDFATransition(set);
            }

#region NFATransition 类型
            public class SetRegexNFATransition : BasicRegexFATransition<char, BasicRegexNFAState<char>>
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
            public class RangeRegexNFATransition : BasicRegexFATransition<char, BasicRegexNFAState<char>>
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

                internal class _DebugInfo : IDebugInfo
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

            [DebugInfoProxy(typeof(RangeSetRegexDFATransition._DebugInfo))]
            public class RangeSetRegexDFATransition : BasicRegexFATransition<char, BasicRegexDFAState<char>>
            {
                private ISet<char> set;
                public ISet<char> Set => this.set;

                public RangeSetRegexDFATransition(ISet<char> set) :
                    base(
                        (set ?? throw new ArgumentNullException(nameof(set))).Contains
                    )
                {
                    this.set = set;
                }
                
                internal class _DebugInfo : IDebugInfo
                {
                    private RangeSetRegexDFATransition transition;

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
                            Func<IRange<char>, string> func = range =>
                             {
                                 if (
                                 range.Comparison(range.Minimum, range.Maximum) == 0 &&
                                     (range.CanTakeMinimum && range.CanTakeMaximum)
                             )
                                     return $"{_DebugInfo.QuoteChar(range.Minimum)}";
                                 else
                                     return $"{(range.CanTakeMinimum ? '[' : '(')}{_DebugInfo.QuoteChar(range.Minimum)},{_DebugInfo.QuoteChar(range.Maximum)}{(range.CanTakeMaximum ? ']' : ')')}";
                             };
                            return $"< {string.Join("∪", ((IEnumerable<IRange<char>>)transition.set).Select(func))} >";
                        }
                    }

                    public _DebugInfo(RangeSetRegexDFATransition transition, params object[] args)
                    {
                        if (transition == null) throw new ArgumentNullException(nameof(transition));

                        this.transition = transition;
                    }
                }
            }
#endregion

            public IAcceptInputTransition<char> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<char> regex)
            {
                BasicRegexFATransition<char, BasicRegexNFAState<char>> transition;

                if (regex is IRange<char> range)
                    transition = new RangeRegexNFATransition(range);
                else
                    transition = new BasicRegexFATransition<char, BasicRegexNFAState<char>>(regex.Condition);

                return transition;
            }

            public IAcceptInputTransition<char> CombineRegexDFATransitions(IEnumerable<IAcceptInputTransition<char>> dfaTransitions)
            {
                if (dfaTransitions == null) throw new ArgumentNullException(nameof(dfaTransitions));

                CharRangeSet set = new CharRangeSet(
                    dfaTransitions
                        .Where(transition => transition != null)
                        .SelectMany(transition => 
                            this.GetAccreditedSetFromRegexAcceptInputTransition(transition)
                        )
                );
                return this.ActivateRegexDFATransitionFromAccreditedSet(set);
            }

            public ISet<char> GetAccreditedSetFromRegexAcceptInputTransition(IAcceptInputTransition<char> transition)
            {
                if (transition is SetRegexNFATransition set)
                    return set.Set;
                else if (transition is RangeRegexNFATransition range)
                    return new CharRangeSet()
                    {
                        new CharRange(range.Range.Minimum,range.Range.Maximum,range.Range.CanTakeMinimum,range.Range.CanTakeMaximum)
                    };
                else
                    return new CharRangeSet(this.AccreditedSet.Where(c => transition.CanAccept(c)));
            }

            public ISet<char> GetAccreditedSetExceptResult(ISet<char> first, ISet<char> second)
            {
                CharRangeSet set = new CharRangeSet();
                set.UnionWith(first);
                set.ExceptWith(second);
                return set;
            }

            public ISet<char> GetAccreditedSetIntersectResult(ISet<char> first, ISet<char> second)
            {
                CharRangeSet set = new CharRangeSet();
                set.UnionWith(first);
                set.IntersectWith(second);
                return set;
            }

            public ISet<char> GetAccreditedSetSymmetricExceptResult(ISet<char> first, ISet<char> second)
            {
                CharRangeSet set = new CharRangeSet();
                set.UnionWith(first);
                set.SymmetricExceptWith(second);
                return set;
            }

            public ISet<char> GetAccreditedSetUnionResult(ISet<char> first, ISet<char> second)
            {
                CharRangeSet set = new CharRangeSet();
                set.UnionWith(first);
                set.UnionWith(second);
                return set;
            }
        }
#endregion

#region string
        public class MyStringRegexRunContextInfo : IRegexStateMachineActivationContextInfo<string>
        {
            private RangeSet<string> set;
            private RangeInfo<string> rangeInfo;

            public ISet<string> AccreditedSet => this.set;

            public MyStringRegexRunContextInfo()
            {
                this.rangeInfo = new CustomizedRangeInfo<string>(
                    (item =>
                    {
                        int result;
                        if (item == ":")
                            return int.MaxValue.ToString();
                        else if (int.TryParse(item, out result))
                        {
                            if (result == int.MinValue)
                                return ".";
                            else
                                return (result - 1).ToString();
                        }
                        else //if (item == ".")
                            throw new InvalidOperationException();
                    }),
                    (item =>
                    {
                        int result;
                        if (item == ".")
                            return int.MinValue.ToString();
                        else if (int.TryParse(item, out result))
                        {
                            if (result == int.MaxValue)
                                return ":";
                            else
                                return (result + 1).ToString();
                        }
                        else //if (item == ":")
                            throw new InvalidOperationException();
                    }),
                    ((x, y) =>
                    {
                        int xParse, yParse;
                        bool canXParse = int.TryParse(x, out xParse);
                        bool canYParse = int.TryParse(y, out yParse);

                        if (canXParse && canYParse) return xParse.CompareTo(yParse);
                        else if (!canXParse && !canYParse)
                        {
                            if (x == "." || x == ":" || y == "." || y == ":") return x.CompareTo(y);
                            else throw new InvalidOperationException();
                        }
                        else if (canXParse)
                        {
                            if (y == ".") return 1;
                            else if (y == ":") return -1;
                            else throw new InvalidOperationException();
                        }
                        else// if (canYParse)\
                        {
                            if (x == ".") return -1;
                            else if (x == ":") return 1;
                            else throw new InvalidOperationException();
                        }
                    })
                );
                this.set = new RangeSet<string>(this.rangeInfo);
            }

            public IRegexDFA<string> ActivateRegexDFA()
            {
                return new BasicRegexDFA<string>();
            }

            public TRegexDFA ActivateRegexDFAFromDumplication<TRegexDFA>(TRegexDFA dfa)
                where TRegexDFA : IRegexDFA<string>
            {
                if (typeof(BasicRegexDFA<string>).IsAssignableFrom(typeof(TRegexDFA)))
                    return (TRegexDFA)(object)this.ActivateRegexDFAFromDumplication((BasicRegexDFA<string>)(object)dfa);
                else
                    return dfa;
            }

            private BasicRegexDFA<string> ActivateRegexDFAFromDumplication(BasicRegexDFA<string> dfa)
            {
                if (dfa == null) throw new ArgumentNullException(nameof(dfa));

                return new BasicRegexDFA<string>();
            }

            public IRegexDFAState<string> ActivateRegexDFAState(bool isTerminal = false)
            {
                return new BasicRegexDFAState<string>(isTerminal);
            }

            public TRegexDFAState ActivateRegexDFAStateFromDumplication<TRegexDFAState>(TRegexDFAState state)
                where TRegexDFAState : IRegexDFAState<string>
            {
                if (typeof(BasicRegexDFAState<string>).IsAssignableFrom(typeof(TRegexDFAState)))
                    return (TRegexDFAState)(object)this.ActivateRegexDFAStateFromDumplication((BasicRegexDFAState<string>)(object)state);
                else
                    return state;
            }

            private BasicRegexDFAState<string> ActivateRegexDFAStateFromDumplication(BasicRegexDFAState<string> state)
            {
                if (state == null) throw new ArgumentNullException(nameof(state));

                return new BasicRegexDFAState<string>(state.IsTerminal);
            }

            public IAcceptInputTransition<string> ActivateRegexDFATransitionFromAccreditedSet(ISet<string> set)
            {
                if (set == null) throw new ArgumentNullException(nameof(set));

                return new RangeSetRegexFATransition<string, BasicRegexDFAState<string>>(set as RangeSet<string>);
            }

            public IRegexNFA<string> ActivateRegexNFA()
            {
                return new BasicRegexNFA<string>();
            }

            public TRegexNFA ActivateRegexNFAFromDumplication<TRegexNFA>(TRegexNFA nfa) where TRegexNFA : IRegexNFA<string>
            {
                if (typeof(BasicRegexNFA<string>).IsAssignableFrom(typeof(TRegexNFA)))
                    return (TRegexNFA)(object)this.ActivateRegexNFAFromDumplication((BasicRegexNFA<string>)(object)nfa);
                else
                    return nfa;
            }

            private BasicRegexNFA<string> ActivateRegexNFAFromDumplication(BasicRegexNFA<string> nfa)
            {
                if (nfa == null) throw new ArgumentNullException(nameof(nfa));

                return new BasicRegexNFA<string>();
            }

            public IRegexFSMEpsilonTransition<string> ActivateRegexFSMEpsilonTransition()
            {
                return new BasicRegexFSMEpsilonTransition<string>();
            }

            public IRegexNFAState<string> ActivateRegexNFAState(bool isTerminal = false)
            {
                return new BasicRegexNFAState<string>(isTerminal);
            }

            public TRegexNFAState ActivateRegexNFAStateFromDumplication<TRegexNFAState>(TRegexNFAState state)
                where TRegexNFAState : IRegexNFAState<string>
            {
                if (typeof(BasicRegexNFAState<string>).IsAssignableFrom(typeof(TRegexNFAState)))
                    return (TRegexNFAState)(object)this.ActivateRegexNFAStateFromDumplication((BasicRegexNFAState<string>)(object)state);
                else
                    return state;
            }

            private BasicRegexNFAState<string> ActivateRegexNFAStateFromDumplication(BasicRegexNFAState<string> state)
            {
                if (state == null) throw new ArgumentNullException(nameof(state));

                return new BasicRegexNFAState<string>(state.IsTerminal);
            }

            public IAcceptInputTransition<string> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<string> regex)
            {
                if (regex == null) throw new ArgumentNullException(nameof(regex));

                RangeSet<string> set = new RangeSet<string>(this.rangeInfo);
                if (regex is RegexConst<string> regexConst)
                    set.Add(regexConst.ConstValue);
                else if (regex is RegexRange<string> regexRange)
                    set.AddRange(regexRange.Minimum, regexRange.Maximum, regexRange.CanTakeMinimum, regexRange.CanTakeMaximum);
                else if (regex is IRange<string> range)
                    set.AddRange(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum);
                else throw new NotSupportedException();

                return new RangeSetRegexFATransition<string, BasicRegexNFAState<string>>(set);
            }

            public TRegexFSMTransition ActivateRegexNFATransitionFromDumplication<TRegexFSMTransition>(TRegexFSMTransition transition)
                where TRegexFSMTransition : IRegexFSMTransition<string>
            {
                if (typeof(BasicRegexFATransition<string, BasicRegexNFAState<string>>).IsAssignableFrom(typeof(TRegexFSMTransition)))
                    return (TRegexFSMTransition)(object)this.ActivateRegexNFATransitionFromDumplication((BasicRegexFATransition<string, BasicRegexNFAState<string>>)(object)transition);
                else
                    return transition;
            }

            private BasicRegexFATransition<string, BasicRegexNFAState<string>> ActivateRegexNFATransitionFromDumplication(BasicRegexFATransition<string, BasicRegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                if (transition is RangeSetRegexFATransition<string, BasicRegexNFAState<string>> rangeSetTransition)
                    return this.ActivateRegexNFATransitionFromDumplication(rangeSetTransition);
                else
                    return transition;
            }

            private RangeSetRegexFATransition<string, BasicRegexNFAState<string>> ActivateRegexNFATransitionFromDumplication(RangeSetRegexFATransition<string, BasicRegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                var field = typeof(RangeSetRegexFATransition<string, BasicRegexNFAState<string>>).GetField("set", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return new RangeSetRegexFATransition<string, BasicRegexNFAState<string>>(
                    field.GetValue(transition) as RangeSet<string>
                );
            }

            public IAcceptInputTransition<string> CombineRegexDFATransitions(IEnumerable<IAcceptInputTransition<string>> dfaTransitions)
            {
                if (dfaTransitions == null) throw new ArgumentNullException(nameof(dfaTransitions));

                RangeSet<string> set = new RangeSet<string>(
                    dfaTransitions
                        .Where(transition => transition != null)
                        .SelectMany(transition =>
                            this.GetAccreditedSetFromRegexAcceptInputTransition(transition)
                        ),
                    this.rangeInfo
                );
                return this.ActivateRegexDFATransitionFromAccreditedSet(set);
            }

            public ISet<string> GetAccreditedSetFromRegexAcceptInputTransition(IAcceptInputTransition<string> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                if (transition is BasicRegexFATransition<string, BasicRegexNFAState<string>>)
                    return this.GetAccreditedSetFromRegexNFATransition((BasicRegexFATransition<string, BasicRegexNFAState<string>>)transition);
                else
                    return new RangeSet<string>(this.AccreditedSet.Where(transition.CanAccept), this.rangeInfo);
            }
            
            private ISet<string> GetAccreditedSetFromRegexNFATransition(BasicRegexFATransition<string, BasicRegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                var field = typeof(RangeSetRegexFATransition<string, BasicRegexNFAState<string>>).GetField("set", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return field.GetValue(transition) as RangeSet<string>;
            }

            public ISet<string> GetAccreditedSetExceptResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                RangeSet<string> set = new RangeSet<string>(this.rangeInfo);
                set.UnionWith(first);
                set.ExceptWith(second);
                return set;
            }

            public ISet<string> GetAccreditedSetIntersectResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                RangeSet<string> set = new RangeSet<string>(this.rangeInfo);
                set.UnionWith(first);
                set.IntersectWith(second);
                return set;
            }

            public ISet<string> GetAccreditedSetSymmetricExceptResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                RangeSet<string> set = new RangeSet<string>(this.rangeInfo);
                set.UnionWith(first);
                set.SymmetricExceptWith(second);
                return set;
            }

            public ISet<string> GetAccreditedSetUnionResult(ISet<string> first, ISet<string> second)
            {
                if (first == null) throw new ArgumentNullException(nameof(first));
                if (second == null) throw new ArgumentNullException(nameof(second));

                RangeSet<string> set = new RangeSet<string>(this.rangeInfo);
                set.UnionWith(first);
                set.UnionWith(second);
                return set;
            }
        }
#endregion

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
