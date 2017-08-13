using SamLu.Diagnostics;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public static class RegexFAUtility
    {
        [Obsolete("Use IRegexFAProvider{T}.GenerateDFAFromNFA Instead.", true)]
        public static RegexDFA<T> ToDFA<T>(this RegexNFA<T> nfa, IRegexRunContextInfo<T> contextInfo)
        {
            if (nfa == null) throw new ArgumentNullException(nameof(nfa));

            nfa.Optimize();

            RegexDFA<T> dfa = new RegexDFA<T>() { StartState = new RegexDFAState<T>() };

            // 队列 Q 放置的是未被处理的已经创建了的 NFA 状态组（及其对应的 DFA 状态）。
            var Q = new Queue<(RegexFAStateGroup<RegexNFAState<T>>, RegexDFAState<T>)>();
            // 集合 C 放置的是已经存在的 NFA 状态组（及其对应的 DFA 状态）。
            var C = new Collection<(RegexFAStateGroup<RegexNFAState<T>>, RegexDFAState<T>)>();
            // 集合 D 放置的是处理后连接指定两个 DFA 状态的所有转换接受的对象的并集。
            var D = new Dictionary<(RegexDFAState<T>, RegexDFAState<T>), ISet<T>>();

            var startTuple = (new RegexFAStateGroup<RegexNFAState<T>>(nfa.StartState), dfa.StartState);
            Q.Enqueue(startTuple);
            C.Add(startTuple);

            while (Q.Count != 0)
            {
                // 从队列 Q 中取出一个状态，计算从这个状态输出的所有转换所接受的对象集的并集。
                (var group, var dfaStateFrom) = Q.Dequeue();
                var set = new HashSet<T>(
                    group
                        .Select(__state => __state.Transitions.AsEnumerable())
                        .Aggregate((ts1, ts2) => ts1.Union(ts2))
                        .Select(__transition => contextInfo.AccreditedSet.Where(element => __transition.Predicate(element)))
                        .Aggregate((es1, es2) => es1.Union(es2))
                );

                // 然后对该集合中的每一个对象寻找接受其的转换，把这些转换的目标状态的并集 T 计算出来。
                foreach (var item in set)
                {
                    RegexFAStateGroup<RegexNFAState<T>> newGroup = new RegexFAStateGroup<RegexNFAState<T>>(
                        group
                            .SelectMany(__state => __state.Transitions)
                            .Where(__transition => __transition.Predicate(item))
                            .Select(__transition => __transition.Target)
                    );

                    if (newGroup.Count == 0) continue;
                    else
                    {
                        (RegexFAStateGroup<RegexNFAState<T>>, RegexDFAState<T> dfaState)? tuple =
                            C
                                .Cast<(RegexFAStateGroup<RegexNFAState<T>>, RegexDFAState<T>)?>()
                                .FirstOrDefault(_tuple =>
                                {
                                    (RegexFAStateGroup<RegexNFAState<T>> __nfaStateGroup, RegexDFAState<T>) t = _tuple.Value;
                                    return t.__nfaStateGroup.Equals(group);
                                });
                        RegexDFAState<T> dfaStateTo;
                        if (tuple.HasValue)
                            // 如果 C 中含有获得接受了指定输入的 NFA 状态集。
                            dfaStateTo = tuple.Value.dfaState;
                        else
                        {
                            dfaStateTo = new RegexDFAState<T>();
                            // 如果接受了指定输入的 NFA 状态集中有结束状态。
                            bool isTerminal = newGroup.Any(__state => __state.IsTerminal);
                            dfaStateTo.IsTerminal = isTerminal;

                            // 将新状态集存入队列，进行后续处理。
                            var newTuple = (newGroup, dfaStateTo);
                            Q.Enqueue(newTuple);
                            C.Add(newTuple);
                        }

                        var __key = (dfaStateFrom, dfaStateTo);
                        ISet<T> __set;
                        if (!D.ContainsKey(__key))
                        {
                            __set = new HashSet<T>();
                            D.Add(__key, __set);
                        }
                        else __set = D[__key];
                        __set.Add(item);
                    }
                }
            }

            foreach (var pair in D)
            {
                (RegexDFAState<T> dfaStateFrom, RegexDFAState<T> dfaStateTo) = pair.Key;
                ISet<T> set = pair.Value;

                RegexFATransition<T, RegexDFAState<T>> dfaTransition = new RegexFATransition<T, RegexDFAState<T>>(__t => set.Contains(__t));
                dfa.AttachTransition(dfaStateFrom, dfaTransition);
                dfa.SetTarget(dfaTransition, dfaStateTo);
            }

            return dfa;
        }

#if DEBUG
        [Obsolete("实现 SamLu.IDebugInfo 接口或提供自定义的获取调试信息的约束的实现。User SamLu.DebugInfo.GetDebugInfo{T} Instead.", false)]
        public static string GetStringInfo<T>(this RegexNFA<T> nfa)
        {
            var states = nfa.States.ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("StartState: ({0}), total: {1}{2}", nfa.StartState.GetStringInfo(states), states.Count, Environment.NewLine);
            sb.Append(string.Join(
                Environment.NewLine,
                (from state in states
                 from transition in state.Transitions
                 select string.Format("  ({0}) --{1}-> ({2})", state.GetStringInfo(states), transition, transition.Target.GetStringInfo(states))
                ).ToArray()
            ));

            return sb.ToString();
        }

        [Obsolete("实现 SamLu.IDebugInfo 接口或提供自定义的获取调试信息的约束的实现。User SamLu.DebugInfo.GetDebugInfo{T} Instead.", false)]
        public static string GetStringInfo<T>(this RegexDFA<T> nfa)
        {
            var states = nfa.States.ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("StartState: ({0}), total: {1}{2}", nfa.StartState.GetStringInfo(states), states.Count, Environment.NewLine);
            sb.Append(string.Join(
                Environment.NewLine,
                (from state in states
                 from transition in state.Transitions
                 select string.Format("  ({0}) --{1}-> ({2})", state.GetStringInfo(states), transition.GetStringInfo(), transition.Target.GetStringInfo(states))
                ).ToArray()
            ));

            return sb.ToString();
        }

        [Obsolete("实现 SamLu.IDebugInfo 接口或提供自定义的获取调试信息的约束的实现。User SamLu.DebugInfo.GetDebugInfo{T} Instead.", false)]
        public static string GetStringInfo<T>(this RegexFATransition<T, RegexNFAState<T>> nfaTransition)
        {
            if (nfaTransition is RegexNFAEpsilonTransition<T> epsilonTransition)
                return epsilonTransition.GetStringInfo();
            else if (nfaTransition.GetType() == typeof(RegexFATransition<T, RegexNFAState<T>>))
                return string.Empty;
            else return nfaTransition.ToString();
        }

        [Obsolete("实现 SamLu.IDebugInfo 接口或提供自定义的获取调试信息的约束的实现。User SamLu.DebugInfo.GetDebugInfo{T} Instead.", false)]
        public static string GetStringInfo<T>(this RegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (epsilonTransition.GetType() == typeof(RegexNFAEpsilonTransition<T>))
                return string.Empty;
            else return epsilonTransition.ToString();
        }

        [Obsolete("实现 SamLu.IDebugInfo 接口或提供自定义的获取调试信息的约束的实现。User SamLu.DebugInfo.GetDebugInfo{T} Instead.", false)]
        public static string GetStringInfo<T>(this RegexFATransition<T, RegexDFAState<T>> dfaTransition)
        {
            if (dfaTransition.GetType() == typeof(RegexFATransition<T, RegexDFAState<T>>))
                return string.Empty;
            else return dfaTransition.ToString();
        }
#endif
    }
}
