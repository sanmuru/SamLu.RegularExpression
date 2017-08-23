using SamLu.Collections.ObjectModel;
using SamLu.Diagnostics;
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
            RangeSet<char> set = new RangeSet<char>(new CharRangeInfo());
            set.Add('a');
            set.Add('c');
            ;

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
            //var debuginfo = char_nfa.GetDebugInfo();
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
            public MyRegexFAProvider(IRegexStateMachineActivationContextInfo<T> contextInfo) : base(contextInfo) { }

            protected override RegexFATransition<T, RegexNFAState<T>> GenerateNFATransitionFromRegexCondition(RegexCondition<T> condition, RegexNFA<T> nfa, RegexNFAState<T> state)
            {
                return base.GenerateNFATransitionFromRegexCondition(condition, nfa, state);
            }
        }

        #region char
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

        public class MyCharRegexRunContextInfo : IRegexStateMachineActivationContextInfo<char>
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

                return new RangeSetRegexDFATransition(set);
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
            public class RangeSetRegexDFATransition : RegexFATransition<char, RegexDFAState<char>>
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

            public RegexFATransition<char, RegexNFAState<char>> ActivateRegexNFATransitionFromRegexCondition(RegexCondition<char> regex)
            {
                if (regex is IRange<char> range)
                    return new RangeRegexNFATransition(range);
                else
                    return new RegexFATransition<char, RegexNFAState<char>>(regex.Condition);
            }

            public RegexFATransition<char, RegexDFAState<char>> CombineRegexDFATransitions(IEnumerable<RegexFATransition<char, RegexDFAState<char>>> dfaTransitions)
            {
                if (dfaTransitions == null) throw new ArgumentNullException(nameof(dfaTransitions));

                CharRangeSet set = new CharRangeSet();
                set.UnionWith(
                    dfaTransitions
                        .Where(transition => transition != null)
                        .SelectMany(transition => 
                            ((transition as RangeSetRegexDFATransition).Set as CharRangeSet) as IEnumerable<IRange<char>>
                        )
                );
                return this.ActivateRegexDFATransitionFromAccreditedSet(set);
            }

            public ISet<char> GetAccreditedSetFromRegexNFATransition(RegexFATransition<char, RegexNFAState<char>> transition)
            {
                if (transition is SetRegexNFATransition set)
                    return set.Set;
                else if (transition is RangeRegexNFATransition range)
                    return new CharRangeSet()
                    {
                        new CharRange(range.Range.Minimum,range.Range.Maximum,range.Range.CanTakeMinimum,range.Range.CanTakeMaximum)
                    };
                else
                    return new HashSet<char>(this.AccreditedSet.Where(c => transition.Predicate(c)));
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
            private RangeInfo<string> rangeInfo = new CustomizedRangeInfo<string>(
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

            public ISet<string> AccreditedSet => this.set;

            public MyStringRegexRunContextInfo()
            {
                this.set = new RangeSet<string>(this.rangeInfo);
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

                return new RangeSetRegexFATransition<string, RegexDFAState<string>>(set as RangeSet<string>);
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

                RangeSet<string> set = new RangeSet<string>(this.rangeInfo);
                if (regex is RegexConst<string> regexConst)
                    set.Add(regexConst.ConstValue);
                else if (regex is RegexRange<string> regexRange)
                    set.AddRange(regexRange.Minimum, regexRange.Maximum, regexRange.CanTakeMinimum, regexRange.CanTakeMaximum);
                else if (regex is IRange<string> range)
                    set.AddRange(range.Minimum, range.Maximum, range.CanTakeMinimum, range.CanTakeMaximum);
                else throw new NotSupportedException();
                return new RangeSetRegexFATransition<string, RegexNFAState<string>>(set);
            }

            public RegexFATransition<string, RegexNFAState<string>> ActivateRegexNFATransitionFromDumplication(RegexFATransition<string, RegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                var field = typeof(RangeSetRegexFATransition<string, RegexNFAState<string>>).GetField("set", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return new RangeSetRegexFATransition<string, RegexNFAState<string>>(
                    field.GetValue(transition) as RangeSet<string>
                );
            }

            public RegexFATransition<string, RegexDFAState<string>> CombineRegexDFATransitions(IEnumerable<RegexFATransition<string, RegexDFAState<string>>> dfaTransitions)
            {
                throw new NotImplementedException();
            }

            public ISet<string> GetAccreditedSetFromRegexNFATransition(RegexFATransition<string, RegexNFAState<string>> transition)
            {
                if (transition == null) throw new ArgumentNullException(nameof(transition));

                var field = typeof(RangeSetRegexFATransition<string, RegexNFAState<string>>).GetField("set", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
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
