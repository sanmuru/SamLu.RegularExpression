using SamLu.RegularExpression.Extend;
using SamLu.RegularExpression.StateMachine.FunctionalTransitions;
using SamLu.RegularExpression.StateMachine.Service;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class RegexFAProvider<T> : IRegexFAProvider<T>
    {
        private IRegexStateMachineActivationContextInfo<T> contextInfo;
        public IRegexStateMachineActivationContextInfo<T> ContextInfo => this.contextInfo;

        public RegexFAProvider(IRegexStateMachineActivationContextInfo<T> contextInfo)
        {
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.contextInfo = contextInfo;
        }

        #region GenerateRegexFSMFromRegexObject

        public IRegexFSM<T> GenerateRegexFSMFromRegexObject(RegexObject<T> regex, RegexOptions options)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            IRegexNFA<T> nfa = this.contextInfo.ActivateRegexNFA();
            IRegexNFAState<T> startState = this.contextInfo.ActivateRegexNFAState();
            nfa.StartState = startState;

            IRegexFSMTransition<T> transition = this.GenerateNFATransitionFromRegexObject(regex, nfa, startState);
            IRegexNFAState<T> endState = this.contextInfo.ActivateRegexNFAState(true);

            nfa.SetTarget(transition, endState);

            return nfa;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexObject(
            RegexObject<T> regex,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            if (regex is IRegexAnchorPoint<T> anchorPoint)
                return this.GenerateNFATransitionFromIRegexAnchorPoint(anchorPoint, nfa, state);
            else if (regex is RegexGroup<T> group)
                return this.GenerateNFATransitionFromRegexGroup(group, nfa, state);
            else if (regex is RegexGroupReference<T> groupReference)
                return this.GenerateNFATransitionFromRegexGroupReference(groupReference, nfa, state);
            else if (regex is RegexMultiBranch<T> multiBranch)
                return this.GenerateNFATransitionFromRegexMultiBranch(multiBranch, nfa, state);
            else if (regex is RegexCondition<T> condition)
                return this.GenerateNFATransitionFromRegexCondition(condition, nfa, state);
            else if (regex is RegexRepeat<T> repeat)
                return this.GenerateNFATransitionFromRegexRepeat(repeat, nfa, state);
            else if (regex is RegexNonGreedyRepeat<T> nonGreedyRepeat)
                return this.GenerateNFATransitionFromRegexNonGreedyRepeat(nonGreedyRepeat, nfa, state);
            else if (regex is RegexSeries<T> series)
                return this.GenerateNFATransitionFromRegexSeries(series, nfa, state);
            else if (regex is RegexParallels<T> parallels)
                return this.GenerateNFATransitionFromRegexParallels(parallels, nfa, state);
            else throw new NotSupportedException(string.Format("不支持的正则类型：{0}", regex.GetType()));
        }

        #region
        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromIRegexAnchorPoint(
            IRegexAnchorPoint<T> anchorPoint,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            if (anchorPoint is RegexZeroLengthObject<T> zeroLength)
                return this.GenerateNFATransitionFromRegexZeroLengthObject(zeroLength, nfa, state);
            else if (anchorPoint is RegexStartBorder<T> startBorder)
                return this.GenerateNFATransitionFromRegexStartBorder(startBorder, nfa, state);
            else if (anchorPoint is RegexEndBorder<T> endBorder)
                return this.GenerateNFATransitionFromRegexEndBorder(endBorder, nfa, state);
            else if (anchorPoint is RegexPreviousMatch<T> previousMatch)
                return this.GenerateNFATransitionFromRegexPreviousMatch(previousMatch, nfa, state);
            else
                throw new NotSupportedException();
        }

        private IRegexFSMTransition<T> GenerateNFATransitionFromRegexZeroLengthObject(
            RegexZeroLengthObject<T> zeroLength,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            const string PROGRESS_SERVICE_KEY = "PROGRESS_SERVICE";
            const string PROGRESS_KEY = "TIMEPOINT";

            IRegexNFAState<T> nextState = state;

            RegexFSMPredicateTransition<T> predicateTransition;

            predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var progressService = fsm.GetService<RegexFSM<T>.ProgressService>();
                    var progress = progressService.GetProgress();
                    fsm.UserData[PROGRESS_SERVICE_KEY] = progressService;
                    fsm.UserData[PROGRESS_KEY] = progress;

                    return true;
                }
                return false;
            });
            nfa.AttachTransition(nextState, predicateTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(predicateTransition, nextState);

            var transition = this.GenerateNFATransitionFromRegexObject(zeroLength.InnerRegex, nfa, nextState);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(transition, nextState);

            predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var progressService = (RegexFSM<T>.ProgressService)fsm.UserData[PROGRESS_SERVICE_KEY];
                    var progress = (RegexFSM<T>.ProgressService.Progress)fsm.UserData[PROGRESS_KEY];
                    progressService.SetProgress(progress);

                    return true;
                }
                return false;
            });

            return predicateTransition;
        }

        private IRegexFSMTransition<T> GenerateNFATransitionFromRegexStartBorder(
            RegexStartBorder<T> startBorder,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            var predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
                (args.FirstOrDefault() is IRegexFSM<T> fsm) &&
                    fsm.Index == 0
            );
            nfa.AttachTransition(state, predicateTransition);

            return predicateTransition;
        }

        private IRegexFSMTransition<T> GenerateNFATransitionFromRegexEndBorder(
            RegexEndBorder<T> endBorder,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            var predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
                (args.FirstOrDefault() is IRegexFSM<T> fsm) &&
                    fsm.Index == fsm.Inputs.Count()
            );
            nfa.AttachTransition(state, predicateTransition);

            return predicateTransition;
        }

        private IRegexFSMTransition<T> GenerateNFATransitionFromRegexPreviousMatch(
            RegexPreviousMatch<T> previousMatch,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            var predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var lastMatch = fsm.Matches?.LastOrDefault();
                    if (lastMatch == null)
                        // 没有上一个匹配。
                        return true;
                    else
                        // 状态机当前索引与上一个匹配的结尾索引相邻。
                        return fsm.Index == lastMatch.Index + lastMatch.Length;
                }
                else return false;
            });
            nfa.AttachTransition(state, predicateTransition);

            return predicateTransition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexGroupReference(
            RegexGroupReference<T> groupReference,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            RegexGroup<T> group;
            if (groupReference.IsDetermined)
                group = groupReference.Group;
            else
            {
                IRegexFSMEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();

                var groups = this.regexGroups.Where(_group => _group.ID == groupReference.GroupID).ToArray();
                switch (groups.Length)
                {
                    case 0:
                        //throw new InvalidOperationException("未找到引用的正则组。");
                        epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();
                        nfa.AttachTransition(state, epsilonTransition);
                        return epsilonTransition;
                    case 1:
                        group = groups[0];
                        break;
                    default:
                        group = new RegexGroup<T>(
                            groups.Select(_group => _group.InnerRegex).UnionMany()
                        );
                        break;
                        //throw new InvalidOperationException("找到多个重复 ID 的正则组。");
                        //epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                        //nfa.AttachTransition(state, epsilonTransition);
                        //return epsilonTransition;
                }
            }

            return this.GenerateNFATransitionFromRegexGroup(group, nfa, state);
        }

        private IList<RegexGroup<T>> regexGroups = new List<RegexGroup<T>>();
        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexGroup(
            RegexGroup<T> group,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            const string CAPTURE_SERVICE_KEY = "CAPTURE_SERVICE";

            this.regexGroups.Add(group);

            IRegexNFAState<T> nextState = state;

            var captureStartTransition = new RegexFSMCaptureStartTransition<T>(group);
            captureStartTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var regexGroups = (Stack<RegexGroup<T>>)fsm.UserData[RegexProvider<T>.Cache.REGEX_GROUPS_CACHE_KEY];

                    var _group = ((RegexFSMCaptureStartTransition<T>)sender).Group;
                    regexGroups.Push(_group);

                    if (_group.IsCaptive)
                    {
                        CaptureService<T> captureService = fsm.GetService<CaptureService<T>>();
                        captureService.StartCapture(_group, _group.ID);

                        ((RegexFSMCaptureStartTransition<T>)sender).UserData[CAPTURE_SERVICE_KEY] = captureService;
                    }
                }
            });
            nfa.AttachTransition(nextState, captureStartTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(captureStartTransition, nextState);

            IRegexFSMTransition<T> transition;
            if (group is RegexBalanceGroup<T> balanceGroup)
                transition = this.GenerateNFATransitionFromRegexBalanceGroup(balanceGroup, nfa, nextState);
            else if (group is RegexBalanceGroupItem<T> balanceGroupItem)
                transition = this.GenerateNFATransitionFromRegexBalanceGroupItem(balanceGroupItem, nfa, nextState);
            else
                transition = this.GenerateNFATransitionFromRegexObject(group.InnerRegex, nfa, nextState);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(transition, nextState);

            if (group.IsCaptive)
            {
                var captureIDStorageTransition = new RegexFSMCaptureIDStorageTransition<T>(group.ID);
                captureIDStorageTransition.TransitAction += new CustomizedAction((sender, args) =>
                {
                    var _group = ((RegexFSMCaptureStartTransition<T>)sender).Group;

                    if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                    {
                        CaptureService<T> captureService = (CaptureService<T>)((RegexFSMCaptureIDStorageTransition<T>)sender).UserData[CAPTURE_SERVICE_KEY];
                        captureService.EndCapture(_group, fsm.Capture);
                    }
                });
                captureIDStorageTransition.UserData[CAPTURE_SERVICE_KEY] = captureStartTransition.UserData[CAPTURE_SERVICE_KEY];
                nfa.AttachTransition(nextState, captureIDStorageTransition);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(captureIDStorageTransition, nextState);
            }

            var captureEndTransition = new RegexFSMCaptureEndTransition<T>(group);
            captureEndTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var regexGroups = (Stack<RegexGroup<T>>)fsm.UserData[RegexProvider<T>.Cache.REGEX_GROUPS_CACHE_KEY];

                    regexGroups.Pop();
                }
            });
            nfa.AttachTransition(nextState, captureEndTransition);

            return captureEndTransition;
        }

        private IDictionary<RegexBalanceGroup<T>, Stack<object>> balanceGroupCache = new Dictionary<RegexBalanceGroup<T>, Stack<object>>();
        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexBalanceGroup(
            RegexBalanceGroup<T> balanceGroup,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;

            var captureStartTransition = new RegexFSMCaptureStartTransition<T>(balanceGroup);
            captureStartTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    if (!balanceGroupCache.ContainsKey(balanceGroup))
                        balanceGroupCache.Add(balanceGroup, new Stack<object>());
                }
            });

            var transition = this.GenerateNFATransitionFromRegexObject(balanceGroup.InnerRegex, nfa, nextState);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(transition, nextState);

            var predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    return (balanceGroupCache.ContainsKey(balanceGroup) && balanceGroupCache[balanceGroup] is Stack<object> seedStack) && seedStack.Count != 0;
                }
                else return false;
            });
            nfa.AttachTransition(nextState, predicateTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(predicateTransition, nextState);

            var captureEndTransition = new RegexFSMCaptureEndTransition<T>(balanceGroup);
            predicateTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    balanceGroupCache[balanceGroup].Pop();
                }
            });
            nfa.AttachTransition(nextState, captureEndTransition);

            return captureEndTransition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexBalanceGroupItem(
            RegexBalanceGroupItem<T> balanceGroupItem,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexFSMTransition<T> transition;
            if (
                this.GenerateNFATransitionFromRegexBalanceGroup__ItemInternal(
                    typeof(RegexBalanceGroupOpenItem<,>),
                    nameof(this.GenerateNFATransitionFromRegexBalanceGroupOpenItem),
                    balanceGroupItem, nfa, state, out transition
                ) ||
                this.GenerateNFATransitionFromRegexBalanceGroup__ItemInternal(
                    typeof(RegexBalanceGroupSubItem<,>),
                    nameof(this.GenerateNFATransitionFromRegexBalanceGroupSubItem),
                    balanceGroupItem, nfa, state, out transition
                ) ||
                this.GenerateNFATransitionFromRegexBalanceGroup__ItemInternal(
                    typeof(RegexBalanceGroupCloseItem<,>),
                    nameof(this.GenerateNFATransitionFromRegexBalanceGroupCloseItem),
                    balanceGroupItem, nfa, state, out transition
                )
            )
                return transition;
            else
            {
                IRegexNFAState<T> nextState = state;

                RegexFSMPredicateTransition<T> predicateTransition;

                predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
                {
                    if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                    {
                        var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                        return (balanceGroupCache.ContainsKey(balanceGroupItem.___balanceGroup) && balanceGroupCache[balanceGroupItem.___balanceGroup] is Stack<object> seedStack) && seedStack.Count != 0 &&
                            // 方法的返回值是 bool 。
                            balanceGroupItem.Method.Method.ReturnType == typeof(bool);
                    }
                    else return false;
                });
                predicateTransition.TransitAction += new CustomizedAction((sender, args) =>
                    ((IRegexFSMFunctionalTransition<T>)sender).UserData["RESULT"] = (bool)balanceGroupItem.Method.DynamicInvoke()
                );
                nfa.AttachTransition(nextState, predicateTransition);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(predicateTransition, nextState);
                predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
                    (bool)((IRegexFSMFunctionalTransition<T>)sender).UserData["RESULT"]
                );
                nfa.AttachTransition(nextState, predicateTransition);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(predicateTransition, nextState);

                transition = this.GenerateNFATransitionFromRegexObject(balanceGroupItem.InnerRegex, nfa, state);

                return transition;
            }
        }

        private bool GenerateNFATransitionFromRegexBalanceGroup__ItemInternal(
            Type __itemTypeDefinition,
            string handlerName,
            RegexBalanceGroupItem<T> item,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state,
            out IRegexFSMTransition<T> transition
        )
        {
            Type itemType = item.GetType();
            IEnumerable<Type> sourceTypes;
            if (__itemTypeDefinition.IsInterface)
                sourceTypes = itemType.GetInterfaces();
            else
            {
                List<Type> baseTypes = new List<Type> { itemType };
                for (Type type = itemType; type.BaseType != typeof(object); type = type.BaseType) baseTypes.Add(itemType);
                sourceTypes = baseTypes;
            }
            Type supportedType = sourceTypes.FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == __itemTypeDefinition);
            if (supportedType != null)
            { // 存在指定的类型。
                var method = itemType.GetMethod(handlerName, BindingFlags.IgnoreCase | BindingFlags.NonPublic).MakeGenericMethod(supportedType.GetGenericArguments());

                transition = (IRegexFSMTransition<T>)method.Invoke(method.IsStatic ? null : this, new object[] { item, nfa, state });
                return true;
            }
            else
            { // 不存在指定的类型。
                transition = null;
                return false;
            }
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexBalanceGroupOpenItem<TSeed>(
            RegexBalanceGroupOpenItem<T, TSeed> openItem,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;

            RegexFSMPredicateTransition<T> predicateTransition;

            predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    return (balanceGroupCache.ContainsKey(openItem.___balanceGroup) && balanceGroupCache[openItem.___balanceGroup] is Stack<object> seedStack) && seedStack.Count != 0;
                }
                else return false;
            });
            predicateTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    balanceGroupCache[openItem.___balanceGroup].Push(openItem.Method.DynamicInvoke(null));
                }
            });
            nfa.AttachTransition(nextState, predicateTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(predicateTransition, nextState);

            var transition = this.GenerateNFATransitionFromRegexObject(openItem.InnerRegex, nfa, state);

            return transition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexBalanceGroupSubItem<TSeed>(
            RegexBalanceGroupSubItem<T, TSeed> subItem,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;

            RegexFSMPredicateTransition<T> predicateTransition;

            predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    return (balanceGroupCache.ContainsKey(subItem.___balanceGroup) && balanceGroupCache[subItem.___balanceGroup] is Stack<object> seedStack) && seedStack.Count != 0;
                }
                else return false;
            });
            predicateTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    var seedStack = balanceGroupCache[subItem.___balanceGroup];
                    seedStack.Push(subItem.Method.DynamicInvoke(seedStack.Pop()));
                }
            });
            nfa.AttachTransition(nextState, predicateTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(predicateTransition, nextState);
            predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    var stack = balanceGroupCache[subItem.___balanceGroup];
                    return subItem.Predicate((TSeed)stack.Pop());
                }
                else return false;
            });
            nfa.AttachTransition(nextState, predicateTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(predicateTransition, nextState);

            var transition = this.GenerateNFATransitionFromRegexObject(subItem.InnerRegex, nfa, state);

            return transition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexBalanceGroupCloseItem<TSeed>(
            RegexBalanceGroupCloseItem<T, TSeed> closeItem,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;

            RegexFSMPredicateTransition<T> predicateTransition;

            predicateTransition = new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    return ((balanceGroupCache.ContainsKey(closeItem.___balanceGroup) && balanceGroupCache[closeItem.___balanceGroup] is Stack<object> seedStack) && seedStack.Count != 0) &&
                        (bool)closeItem.Method.DynamicInvoke(seedStack.Pop());
                }
                else return false;
            });
            predicateTransition.TransitAction += new CustomizedAction((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)fsm.UserData[RegexProvider<T>.Cache.BALANCE_GROUP_CACHE_KEY];

                    var stack = balanceGroupCache[closeItem.___balanceGroup];
                    stack.Push(closeItem.Method.DynamicInvoke(stack.Pop()));
                }
            });
            nfa.AttachTransition(nextState, predicateTransition);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(predicateTransition, nextState);

            var transition = this.GenerateNFATransitionFromRegexObject(closeItem.InnerRegex, nfa, state);

            return transition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexMultiBranch(
            RegexMultiBranch<T> multiBranch,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;
            IRegexFSMTransition<T> transition;
            foreach (var branch in multiBranch.Branches)
            {
                transition = this.GenerateNFATransitionFromRegexMultiBranchBranch(branch, nfa, nextState);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(transition, nextState);
            }

            transition = this.GenerateNFATransitionFromRegexObject(multiBranch.OtherwisePattern, nfa, nextState);

            return transition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexMultiBranchBranch(
            RegexMultiBranchBranch<T> branch,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;
            IRegexFSMTransition<T> transition;

            transition = this.GenerateNFATransitionFromRegexMultiBranchBranchPredicate(branch.Predicate, nfa, nextState);
            nextState = this.contextInfo.ActivateRegexNFAState();
            nfa.SetTarget(transition, nextState);

            transition = this.GenerateNFATransitionFromRegexObject(branch.Pattern, nfa, state);

            return transition;
        }

        private IRegexFSMTransition<T> GenerateNFATransitionFromRegexMultiBranchBranchPredicate(
            RegexMultiBranchBranchPredicate<T> predicate,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;

            IRegexFSMTransition<T> transition;
            if (predicate is RegexMultiBranchPatternBranchPredicate<T> patternPredicate)
                transition = this.GenerateNFATransitionFromRegexMultiBranchPatternBranchPredicate(patternPredicate, nfa, nextState);
            else if (predicate is RegexMultiBranchGroupReferenceBranchPredicate<T> groupReferencePredicate)
                transition = this.GenerateNFATransitionFromRegexMultiBranchGroupReferenceBranchPredicate(groupReferencePredicate, nfa, nextState);
            else
                throw new NotSupportedException();

            return transition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexMultiBranchPatternBranchPredicate(
            RegexMultiBranchPatternBranchPredicate<T> patternPredicate,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexAnchorPoint<T> pattern;
            if (patternPredicate.Pattern is IRegexAnchorPoint<T>)
                pattern = (IRegexAnchorPoint<T>)patternPredicate.Pattern;
            else
                pattern = new RegexZeroLengthObject<T>(patternPredicate.Pattern);

            return this.GenerateNFATransitionFromIRegexAnchorPoint(pattern, nfa, state);
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexMultiBranchGroupReferenceBranchPredicate(
            RegexMultiBranchGroupReferenceBranchPredicate<T> groupReferencePredicate,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            return new RegexFSMPredicateTransition<T>((sender, args) =>
            {
                if (args.FirstOrDefault() is IRegexFSM<T> fsm)
                {
                    var regexGroups = (Stack<RegexGroup<T>>)fsm.UserData[RegexProvider<T>.Cache.REGEX_GROUPS_CACHE_KEY];

                    RegexGroupReference<T> groupReference = groupReferencePredicate.GroupReference;
                    if (groupReference.IsDetermined)
                        return true;
                    else
                    {
                        var groups = regexGroups.Where(_group => _group.ID == groupReference.GroupID).ToArray();
                        switch (groups.Length)
                        {
                            case 0:
                                //throw new InvalidOperationException("未找到引用的正则组。");
                                return false;
                            case 1:
                                return true;
                            default:
                                return true;
                                //throw new InvalidOperationException("找到多个重复 ID 的正则组。");
                                //return false;
                        }
                    }
                }
                else return false;
            });
        }

        protected virtual IAcceptInputTransition<T> GenerateNFATransitionFromRegexCondition(
            RegexCondition<T> condition,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IAcceptInputTransition<T> transition = this.contextInfo.ActivateRegexNFATransitionFromRegexCondition(condition);
            nfa.AttachTransition(state, transition);

            return transition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexRepeat(
            RegexRepeat<T> repeat,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            ulong count;
            if (repeat.IsInfinte)
                count = repeat.MinimumCount ?? ulong.MinValue;
            else
                count = repeat.MaximumCount.Value;

            var transition = this.GenerateNFATransitionFromRegexRepeatInternal(repeat.InnerRegex, nfa, state, count, out IList<IRegexNFAState<T>> nodes);

            if (count > ulong.MinValue)
            {
                if (repeat.IsInfinte)
                {
                    IRegexFSMEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();
                    var reversedNodes = nodes.Reverse();
                    nfa.AttachTransition(reversedNodes.First(), epsilonTransition);
                    nfa.SetTarget(epsilonTransition, reversedNodes.Skip(1).First());
                }
                else
                {
                    IRegexFSMEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();
                    foreach (var _state in nodes.Take((int)(repeat.MaximumCount.Value - (repeat.MinimumCount ?? ulong.MinValue))))
                        nfa.AttachTransition(_state, epsilonTransition);

                    nfa.SetTarget(epsilonTransition, nodes.Reverse().ElementAt((int)(repeat.MinimumCount ?? ulong.MinValue)));
                }
            }

            return transition;
        }

        private IRegexFSMTransition<T> GenerateNFATransitionFromRegexRepeatInternal(
            RegexObject<T> innerRegex,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state,
            ulong count,
            out IList<IRegexNFAState<T>> nodes)
        {
            nodes = new List<IRegexNFAState<T>> { state };

            IRegexNFAState<T> nextState = state;
            for (ulong index = ulong.MinValue; index < count; index++)
            {
                IRegexFSMTransition<T> transition = this.GenerateNFATransitionFromRegexObject(innerRegex, nfa, nextState);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(transition, nextState);

                nodes.Add(nextState);
            }

            IRegexFSMEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();
            nfa.AttachTransition(nextState, epsilonTransition);

            return epsilonTransition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexNonGreedyRepeat(RegexNonGreedyRepeat<T> nonGreedyRepeat, IRegexNFA<T> nfa, IRegexNFAState<T> state)
        {
            IRegexNFAState<T> nextState = state;

            var nonGreedyRepeatTransition = new RegexFSMNonGreedyRepeatTransition<T>();
            nfa.AttachTransition(nextState, nonGreedyRepeatTransition);

            return this.GenerateNFATransitionFromRegexRepeat(nonGreedyRepeat.InnerRepeat, nfa, nextState);
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexSeries(
            RegexSeries<T> series,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> nextState = state;
            foreach (var item in series.Series)
            {
                var transition = this.GenerateNFATransitionFromRegexObject(item, nfa, nextState);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(transition, nextState);
            }

            IRegexFSMEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();
            nfa.AttachTransition(nextState, epsilonTransition);

            return epsilonTransition;
        }

        protected virtual IRegexFSMTransition<T> GenerateNFATransitionFromRegexParallels(
            RegexParallels<T> parallels,
            IRegexNFA<T> nfa,
            IRegexNFAState<T> state
        )
        {
            IRegexNFAState<T> endState = this.contextInfo.ActivateRegexNFAState();

            foreach (var item in parallels.Parallels)
            {
                IRegexFSMTransition<T> transition = this.GenerateNFATransitionFromRegexObject(item, nfa, state);
                nfa.AttachTransition(state, transition);
            }

            IRegexFSMEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexFSMEpsilonTransition();
            nfa.AttachTransition(endState, epsilonTransition);

            return epsilonTransition;
        }

        #endregion
        #endregion

        public IRegexDFA<T> GenerateRegexDFAFromRegexFSM(IRegexFSM<T> nfa)
        {
            if (nfa == null) throw new ArgumentNullException(nameof(nfa));

            nfa.Optimize();

            IRegexDFA<T> dfa = this.contextInfo.ActivateRegexDFA();
            dfa.StartState = this.contextInfo.ActivateRegexDFAState();

            // 队列 Q 放置的是未被处理的已经创建了的 NFA 状态组（及其对应的 DFA 状态）。
            var Q = new Queue<(RegexFAStateGroup<T, IRegexFSMState<T>>, IRegexFSMState<T>)>();
            // 集合 C 放置的是已经存在的 NFA 状态组（及其对应的 DFA 状态）。
            var C = new Collection<(RegexFAStateGroup<T, IRegexFSMState<T>>, IRegexFSMState<T>)>();
#if false
            // 集合 D 放置的是处理后连接指定两个 DFA 状态的所有转换接受的对象的并集。
            var D = new Dictionary<(IRegexFSMState<T>, IRegexFSMState<T>), IList<ISet<T>>>();
#endif

            var startTuple = (new RegexFAStateGroup<T, IRegexFSMState<T>>(nfa.StartState), dfa.StartState);
            Q.Enqueue(startTuple);
            C.Add(startTuple);

            while (Q.Count != 0)
            {
                /* 从队列 Q 中取出一个状态。 */
                (var group, var dfaStateFrom) = Q.Dequeue();

                /* 计算从这个状态输出的所有转换及其所接受的对象集的并集。 */
                // 计算所有输出转换。
                var transitions = new HashSet<IRegexFSMTransition<T>>(
                        group.SelectMany(__state => __state.Transitions)
                    );
                /* 优化：构建转换/可接受对象集字典。 */
                var accreditedSetsDic = transitions
                    .ToDictionary(
                        (transition => transition),
                        (transition => this.contextInfo.GetAccreditedSetFromRegexAcceptInputTransition((IAcceptInputTransition<T>)transition))
                    );
                // 计算接受的对象集的并集。
                var sets = accreditedSetsDic.Values.ToArray();

                if (sets.Length > 1)
                {
                    /* 优化：对并集进行拆分，拆分成最小单位的字符集。 */
                    // 不需要拆分的字符集（与并集中的其他字符集都没有相交）。
                    var reservedSets = sets
                    //.Where(l_set => l_set != null)
                    .Where(l_set =>
                        sets
                            //.Where(r_set => r_set != null)
                            .Where(r_set => l_set != r_set)
                            .All(r_set => !l_set.Overlaps(r_set))
                    ).ToArray();
                    var elseSets = sets.Except(reservedSets).ToList();

                    if (elseSets.Count == 0) sets = reservedSets;
                    else if (elseSets.Count == 1) sets = reservedSets.Concat(elseSets).ToArray();
                    else
                    {
                        var combinations = Math.Combination.GetCombinationsWithRank(elseSets, 2);
                        var splitedSets = combinations
                            .SelectMany(combination =>
                            {
                                var l_set = combination[0];
                                var r_set = combination[1];

                                if (l_set.IsProperSubsetOf(r_set))
                                    return new[] { l_set, this.contextInfo.GetAccreditedSetExceptResult(r_set, l_set) };
                                else if (l_set.IsProperSupersetOf(r_set))
                                    return new[] { this.contextInfo.GetAccreditedSetExceptResult(l_set, r_set), r_set };
                                else
                                    return new[] { this.contextInfo.GetAccreditedSetUnionResult(l_set, r_set) };
                            });

                        sets = reservedSets.Concat(splitedSets).ToArray();
                    }
                }

                /* 然后对这个并集中的每一个对象集寻找接受其的转换，把这些转换的目标状态的并集 newGroup 计算出来。 */
                foreach (var set in sets)
                {
                    var newGroup = new RegexFAStateGroup<T, IRegexFSMState<T>>(new HashSet<IRegexFSMState<T>>(
                        accreditedSetsDic
                            .Where(pair => pair.Value.IsSupersetOf(set))
                            .Select(pair => pair.Key.Target)
                    ));

                    if (newGroup.Count == 0) continue;
                    else
                    {
                        (RegexFAStateGroup<T, IRegexFSMState<T>>, IRegexFSMState<T> dfaState)? tuple =
                            C
                                .Cast<(RegexFAStateGroup<T, IRegexFSMState<T>>, IRegexFSMState<T>)?>()
                                .FirstOrDefault(_tuple =>
                                {
                                    (RegexFAStateGroup<T, IRegexFSMState<T>> __nfaStateGroup, IRegexFSMState<T>) t = _tuple.Value;
                                    return t.__nfaStateGroup.Equals(newGroup);
                                });
                        IRegexFSMState<T> dfaStateTo;
                        if (tuple.HasValue)
                            // 如果 C 中含有获得接受了指定输入的 NFA 状态集。
                            dfaStateTo = tuple.Value.dfaState;
                        else
                        {
                            // 如果接受了指定输入的 NFA 状态集中有结束状态。
                            bool isTerminal = newGroup.Any(__state => __state.IsTerminal);
                            dfaStateTo = this.contextInfo.ActivateRegexDFAState(isTerminal);

                            // 将新状态集存入队列，进行后续处理。
                            var newTuple = (newGroup, dfaStateTo);
                            Q.Enqueue(newTuple);
                            C.Add(newTuple);
                        }

                        IAcceptInputTransition<T> dfaTransition = this.contextInfo.ActivateRegexDFATransitionFromAccreditedSet(set);
                        dfa.AttachTransition(dfaStateFrom, dfaTransition);
                        dfa.SetTarget(dfaTransition, dfaStateTo);

#if false
                        var __key = (dfaStateFrom, dfaStateTo);
                        IList<ISet<T>> __list;
                        if (!D.ContainsKey(__key))
                        {
                            __list = new List<ISet<T>>();
                            D.Add(__key, __list);
                        }
                        else __list = D[__key];
                        __list.Add(set);
#endif
                    }
                }
            }

#if false
            foreach (var pair in D)
            {
                (IRegexFSMState<T> dfaStateFrom, IRegexFSMState<T> dfaStateTo) = pair.Key;
                ISet<T> set;
                if (pair.Value.Count == 1) set = pair.Value[0];
                else set = pair.Value.Aggregate((s1, s2) => this.contextInfo.GetAccreditedSetUnionResult(s1, s2));

                IAcceptInputTransition<T> dfaTransition = this.contextInfo.ActivateRegexDFATransitionFromAccreditedSet(set);
                dfa.AttachTransition(dfaStateFrom, dfaTransition);
                dfa.SetTarget(dfaTransition, dfaStateTo);
            }
#endif

            return dfa;
        }
    }
}
