using SamLu.IO;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public static class RegexFSMUtility
    {
        #region RecurGetStates
        /// <summary>
        /// 递归获取指定起始状态开始能达到的所有状态的集合。
        /// </summary>
        /// <param name="startState">指定的起始状态。</param>
        /// <returns>指定起始状态开始能达到的所有状态的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="startState"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMState<T>> RecurGetStates<T>(this IRegexFSMState<T> startState)
        {
            if (startState == null) throw new ArgumentNullException(nameof(startState));

            HashSet<IRegexFSMState<T>> states = new HashSet<IRegexFSMState<T>>();
            RegexFSMUtility.RecurGetStatesInternal(startState, states);

            return states;
        }

        /// <summary>
        /// 递归获取指定转换开始能达到的所有状态的集合。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <returns>指定转换开始能达到的所有状态的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMState<T>> RecurGetStates<T>(this IRegexFSMTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            return transition.Target.RecurGetStates();
        }

        private static void RecurGetStatesInternal<T>(IRegexFSMState<T> startState, HashSet<IRegexFSMState<T>> states)
        {
            if (states.Add(startState))
            {
                foreach (var transition in startState.Transitions.Where(transition => transition.Target != null))
                    RegexFSMUtility.RecurGetStatesInternal(transition.Target, states);
            }
        }
        #endregion

        #region RecurGetTransitions
        /// <summary>
        /// 递归获取指定起始状态开始能经到的所有转换的集合。
        /// </summary>
        /// <param name="startState">指定的起始状态</param>
        /// <returns>指定起始状态开始能经到的所有转换的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="startState"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMTransition<T>> RecurGetTransitions<T>(this IRegexFSMState<T> startState)
        {
            if (startState == null) throw new ArgumentNullException(nameof(startState));

            HashSet<IRegexFSMTransition<T>> transitions = new HashSet<IRegexFSMTransition<T>>();
            foreach (var transition in startState.Transitions)
                transitions.UnionWith(transition.RecurGetTransitions());

            return transitions;
        }

        /// <summary>
        /// 递归获取指定转换开始能经到的所有转换的集合。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <returns>指定转换开始能经到的所有转换的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMTransition<T>> RecurGetTransitions<T>(this IRegexFSMTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            HashSet<IRegexFSMTransition<T>> transitions = new HashSet<IRegexFSMTransition<T>>();
            RegexFSMUtility.RecurGetTransitionsInternal(transition, transitions);

            return transitions;
        }

        private static void RecurGetTransitionsInternal<T>(IRegexFSMTransition<T> transition, HashSet<IRegexFSMTransition<T>> transitions)
        {
            if (transitions.Add(transition))
            {
                if (transition.Target != null)
                {
                    foreach (var _transition in transition.Target.Transitions.Where(t => t != null))
                        RegexFSMUtility.RecurGetTransitionsInternal(_transition, transitions);
                }
            }
        }
        #endregion

        #region RecurGetReachableStates
        /// <summary>
        /// 递归获取指定起始状态开始无须接受输入就能达到的所有状态的集合。
        /// </summary>
        /// <param name="startState">指定的起始状态。</param>
        /// <returns>指定起始状态开始无须接受输入就能达到的所有状态的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="startState"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMState<T>> RecurGetReachableStates<T>(this IRegexFSMState<T> startState)
        {
            if (startState == null) throw new ArgumentNullException(nameof(startState));

            HashSet<IRegexFSMState<T>> states = new HashSet<IRegexFSMState<T>>();
            RegexFSMUtility.RecurGetReachableStatesInternal(startState, states);

            return states;
        }

        /// <summary>
        /// 递归获取指定转换开始无须接受输入就能达到的所有状态的集合。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <returns>指定转换开始无须接受输入就能达到的所有状态的集合。</returns>
        public static IEnumerable<IRegexFSMState<T>> RecurGetReachableStates<T>(this IRegexFSMTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            HashSet<IRegexFSMState<T>> states = new HashSet<IRegexFSMState<T>> { transition.Target };
            RegexFSMUtility.RecurGetReachableStatesInternal(transition.Target, states);

            return states;
        }

        private static void RecurGetReachableStatesInternal<T>(IRegexFSMState<T> startState, HashSet<IRegexFSMState<T>> states)
        {
            foreach (var transition in startState.Transitions.Where(_transition => _transition != null && _transition.Target != null))
            {
                if (transition is IEpsilonTransition && states.Add(transition.Target))
                    RegexFSMUtility.RecurGetReachableStatesInternal(transition.Target, states);
            }
        }
        #endregion

        #region RecurGetReachableTransitions
        /// <summary>
        /// 递归获取指定起始状态开始无须接受输入就能经到的所有转换的集合。
        /// </summary>
        /// <param name="startState">指定的起始状态</param>
        /// <returns>指定起始状态开始无须接受输入就能经到的所有转换的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="startState"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMTransition<T>> RecurGetReachableTransitions<T>(this IRegexFSMState<T> startState)
        {
            if (startState == null) throw new ArgumentNullException(nameof(startState));

            HashSet<IRegexFSMTransition<T>> transitions = new HashSet<IRegexFSMTransition<T>>();
            foreach (var transition in startState.Transitions.Where(_transition => _transition != null))
            {
                if (transitions.Add(transition) && transition is IEpsilonTransition)
                    transitions.UnionWith(transition.RecurGetReachableTransitions());
            }

            return transitions;
        }

        /// <summary>
        /// 递归获取指定转换开始无须接受输入就能经到的所有转换的集合。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <returns>指定转换开始无须接受输入就能经到的所有转换的集合。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        public static IEnumerable<IRegexFSMTransition<T>> RecurGetReachableTransitions<T>(this IRegexFSMTransition<T> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            HashSet<IRegexFSMTransition<T>> transitions = new HashSet<IRegexFSMTransition<T>>();
            RegexFSMUtility.RecurGetReachableTransitionsInternal(transition, transitions);

            return transitions;
        }

        private static void RecurGetReachableTransitionsInternal<T>(IRegexFSMTransition<T> transition, HashSet<IRegexFSMTransition<T>> transitions)
        {
            foreach (var t in transition.Target.Transitions)
            {
                if (transitions.Add(t) && t is IEpsilonTransition)
                    RegexFSMUtility.RecurGetReachableTransitionsInternal(transition, transitions);
            }
        }
        #endregion

        #region RecurGetPredicated
        internal static (IEnumerable<IRegexFSMState<T>> states, IEnumerable<IRegexFSMTransition<T>> transitions) RecurGetPredicated<T>(this IRegexFSMState<T> startState, Predicate<IRegexFSMState<T>> statePredicate = null, Predicate<IRegexFSMTransition<T>> transitionPredicate = null)
        {
            if (startState == null) throw new ArgumentNullException(nameof(startState));

            HashSet<IRegexFSMState<T>> states = new HashSet<IRegexFSMState<T>>();
            HashSet<IRegexFSMTransition<T>> transitions = new HashSet<IRegexFSMTransition<T>>();
            RegexFSMUtility.RecurGetPredicatedInternal(startState, statePredicate, transitionPredicate, states, transitions);

            return (states, transitions);
        }

        internal static (IEnumerable<IRegexFSMState<T>> states, IEnumerable<IRegexFSMTransition<T>> transitions) RecurGetPredicated<T>(this IRegexFSMTransition<T> transition, Predicate<IRegexFSMState<T>> statePredicate = null, Predicate<IRegexFSMTransition<T>> transitionPredicate = null)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            HashSet<IRegexFSMState<T>> states = new HashSet<IRegexFSMState<T>>();
            HashSet<IRegexFSMTransition<T>> transitions = new HashSet<IRegexFSMTransition<T>>();
            RegexFSMUtility.RecurGetPredicatedInternal(transition, statePredicate, transitionPredicate, states, transitions);

            return (states, transitions);
        }

        private static void RecurGetPredicatedInternal<T>(this IRegexFSMState<T> startState, Predicate<IRegexFSMState<T>> statePredicate, Predicate<IRegexFSMTransition<T>> transitionPredicate, HashSet<IRegexFSMState<T>> states, HashSet<IRegexFSMTransition<T>> transitions)
        {
            foreach (var transition in startState.Transitions)
            {
                if (!(transitionPredicate?.Invoke(transition) == false) && transitions.Add(transition))
                {
                    RegexFSMUtility.RecurGetPredicatedInternal(transition, statePredicate, transitionPredicate, states, transitions);
                }
            }
        }

        private static void RecurGetPredicatedInternal<T>(this IRegexFSMTransition<T> transition, Predicate<IRegexFSMState<T>> statePredicate, Predicate<IRegexFSMTransition<T>> transitionPredicate, HashSet<IRegexFSMState<T>> states, HashSet<IRegexFSMTransition<T>> transitions)
        {
            if (!(statePredicate?.Invoke(transition.Target) == false) && states.Add(transition.Target))
            {
                RegexFSMUtility.RecurGetPredicatedInternal(transition.Target, statePredicate, transitionPredicate, states, transitions);
            }
        }
        #endregion

        private static IEnumerable<(IRegexFSMState<T>, IAcceptInputTransition<T>, IRegexFSMTransition<T>[])> M<T>(this IRegexFSMState<T> state)
        {
            return MInternal(state, new HashSet<IRegexFSMState<T>>())
                .Select(transitions =>
                    (state, (IAcceptInputTransition<T>)transitions.First(), transitions.Skip(1).Reverse().ToArray())
                );
        }

        private static IEnumerable<IEnumerable<IRegexFSMTransition<T>>> MInternal<T>(IRegexFSMState<T> state, HashSet<IRegexFSMState<T>> states)
        {
            if (states.Add(state))
            {
                foreach (var transition in state.GetOrderedTransitions())
                {
                    if (transition is IAcceptInputTransition<T>)
                    {
                        yield return new[] { transition };
                    }
                    else
                    {
                        foreach (var transitions in MInternal(transition.Target, states))
                            yield return transitions.Concat(new[] { transition });
                    }
                }
            }
            else
                yield break;
        }

        /// <summary>
        /// 最小化 <see cref="IRegexFSM{T}"/> 。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="fsm">将要进行最小化的正则构造的有限状态机。</param>
        /// <returns>经过最小化后的正则构造的有限状态机。</returns>
        public static IRegexFSM<T> Optimize<T>(this IRegexFSM<T> fsm)
        {
            fsm.EpsilonClosure();

            if (fsm.StartState.RecurGetTransitions().All(transition => transition is IAcceptInputTransition<T>))
                return fsm;
            else
            {
                var result = new RegexFSM<T>() { StartState = new RegexStateState<T>(fsm.StartState) };

                var states = fsm.States;
                var transitions = fsm.StartState.RecurGetTransitions().ToList();
                var signedStates = states.Where(state =>
                    state != fsm.StartState && // 不是起始状态
                    !state.IsTerminal && // 不是结束状态
                    transitions
                        .Where(transition => transition.Target == state)
                        .All(transition => !(transition is IAcceptInputTransition<T>))
                );
                var unsignedStates = states.Except(signedStates);
                var groupTransitions = unsignedStates
                    .SelectMany(state => state.M())
                    .Select(group => new RegexFunctionalTransitionGroupTransition<T>(group))
                    .ToArray();

                var D = new Dictionary<IRegexFSMState<T>, IRegexFSMState<T>>();
                Func<IRegexFSMState<T>, IRegexFSMState<T>> func = (formerState) =>
                {
                    if (D.ContainsKey(formerState)) return D[formerState];
                    else
                    {
                        var newState = new RegexStateState<T>(formerState);
                        D.Add(formerState, newState);
                        return newState;
                    }
                };
                D.Add(fsm.StartState, result.StartState);
                foreach (var groupTransition in groupTransitions)
                {
                    result.AttachTransition(func(groupTransition.StateFrom), groupTransition);
                    result.SetTarget(groupTransition, func(groupTransition.StateTo));
                }

                return result;
            }
        }

        private sealed class RegexStateState<T> : FSMState<IRegexFSMTransition<T>>, IRegexFSMState<T>
        {
            public IRegexFSMState<T> InnerState { get; private set; }

            public RegexStateState(IRegexFSMState<T> state) =>
                this.InnerState = state ?? throw new ArgumentNullException(nameof(state));

            public IEnumerable<IRegexFSMTransition<T>> GetOrderedTransitions() =>
                this.InnerState.GetOrderedTransitions();
        }

        private sealed class RegexFunctionalTransitionGroupTransition<T> : FSMTransition<IRegexFSMState<T>>, IAcceptInputTransition<T>, IRegexFSMTransitionProxy<T>
        {
            public IRegexFSMState<T> StateFrom { get; private set; }
            public IRegexFSMState<T> StateTo => this.AcceptInputTransition.Target;

            public IAcceptInputTransition<T> AcceptInputTransition { get; private set; }
            public IRegexFSMTransition<T>[] FunctionalTransitions { get; private set; }

            public RegexFunctionalTransitionGroupTransition((IRegexFSMState<T> stateFrom, IAcceptInputTransition<T> acceptInputTransition, IRegexFSMTransition<T>[] functionalTransitions) transitionGroup)
            {
                this.StateFrom = transitionGroup.stateFrom;
                this.AcceptInputTransition = transitionGroup.acceptInputTransition;
                this.FunctionalTransitions = transitionGroup.functionalTransitions;
            }

            public bool TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T> handler, params object[] args)
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var backTraceService = fsm.GetService<RegexFSM<T>.BackTraceService>();
                    var timepoint = backTraceService.GetTimepoint();
                    
                    foreach (var functionalTransition in this.FunctionalTransitions)
                    {
                        if (!handler(functionalTransition, args))
                        {
                            // 复原
                            backTraceService.BackTrace(timepoint, true);
                            return false;
                        }
                    }

                    if (readerSource.HasNext() && this.CanAccept(readerSource.Peek()))
                    {
                        readerSource.Read();
                        return true;
                    }
                    else return false;
                }
                else throw new InvalidOperationException();
            }

            public bool CanAccept(T input)
            {
                return this.AcceptInputTransition.CanAccept(input);
            }
        }
    }
}
