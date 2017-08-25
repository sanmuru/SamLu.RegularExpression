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
    public class RegexNFA<T> : RegexFSM<T, RegexNFAState<T>, RegexFATransition<T, RegexNFAState<T>>>, INFA<RegexNFAState<T>, RegexFATransition<T, RegexNFAState<T>>, RegexNFAEpsilonTransition<T>>
    {
        /// <summary>
        /// 向 <see cref="NFA{TState, TTransition, TEpsilonTransition}"/> 的一个指定状态添加指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public virtual bool AttachTransition(RegexNFAState<T> state, RegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return base.AttachTransition(state, epsilonTransition);
        }

        /// <summary>
        /// 从 <see cref="NFA{TState, TTransition, TEpsilonTransition}"/> 的一个指定状态移除指定 ε 转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public virtual bool RemoveTransition(RegexNFAState<T> state, RegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return base.RemoveTransition(state, epsilonTransition);
        }

        /// <summary>
        /// 最小化 NFA 。
        /// </summary>
        public virtual void Optimize()
        {
            this.EpsilonClosure();
        }

        #region INFA Implementation
        bool INFA.AttachTransition(IState state, IEpsilonTransition epsilonTransition) => this.AttachTransition((RegexNFAState<T>)state, (RegexFATransition<T, RegexNFAState<T>>)epsilonTransition);

        bool INFA.RemoveTransition(IState state, IEpsilonTransition epsilonTransition) => this.RemoveTransition((RegexNFAState<T>)state, (RegexFATransition<T, RegexNFAState<T>>)epsilonTransition);
        #endregion
    }
}
