using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的非确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    internal class RegexNFA<T> : NFA<RegexNFAState<T>, RegexFATransition<T, RegexNFAState<T>>, RegexFAEpsilonTransition<T, RegexNFAState<T>>>
    {
        /// <summary>
        /// 最小化 NFA 。
        /// </summary>
        public void Optimize()
        {
            this.EpsilonClosure();
        }

        /// <summary>
        /// 消除 NFA 中的所有 ε 闭包。
        /// </summary>
        private void EpsilonClosure()
        {
            if (this.StartState
                .RecurGetTransitions<RegexNFAState<T>, RegexFATransition<T, RegexNFAState<T>>>()
                .Any(transition=>transition is IEpsilonTransition)
            )
            {
                var states = this.States;
                // 计算有效状态
                var avaliableStates = states.Where(state =>
                    // 起始状态
                    state == this.StartState ||
                    // 存在非 ε 转换的输入转换
                    states.SelectMany(_state => _state.Transitions)
                        .Where(transition => transition.Target == state)
                        .Any(transition => !(transition is IEpsilonTransition))
                );

                foreach (var avaliableState in avaliableStates)
                {
                    // 计算状态 avaliableState 的 ε 闭包。
                    // 所谓一个状态的 ε 闭包就是从这个状态出发，仅通过 ε 转换就可以到达的所有状态的集合。
                    var epsilonClosure = avaliableState.RecurGetReachableStates<RegexNFAState<T>, RegexFATransition<T, RegexNFAState<T>>>().ToList();
                    // 把状态 avaliableState 从其 ε 闭包中排除出去。
                    epsilonClosure.Remove(avaliableState);

                    // 复制所有有效转换到状态 avaliableState 。
                    var avaliableTransitions = epsilonClosure
                        .SelectMany(state => state.Transitions)
                        .Where(transition => !(transition is IEpsilonTransition));
                    foreach (var avaliableTransition in avaliableTransitions)
                        this.AttachTransition(avaliableState, avaliableTransition);

                    // 移除状态 avaliableState 的所有 ε 转换。
                    // 与此同时，由于此状态机框架的实现方式：移除某个转换且其所指向的状态不为状态机任意可达转换的目标时，此状态不可达，即被排除于状态机外。
                    var epsilonTransitions = avaliableState.Transitions
                        .Where(transition => transition is IEpsilonTransition);
                    foreach (var epsilonTransition in epsilonTransitions)
                        this.RemoveTransition(avaliableState, epsilonTransition);

                    // 如果存在一个有效状态可以仅通过 ε 转换到达结束状态的话，那么这个状态应该被标记为结束状态。
                    if (epsilonClosure.Any(state => state.IsTerminal))
                        avaliableState.IsTerminal = true;
                }
            }
        }
    }
}
