using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的确定的有限自动机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexDFAState<T> : DFAState<RegexFATransition<T, RegexDFAState<T>>>, IRegexFSMState<T, RegexFATransition<T, RegexDFAState<T>>>
    {
        /// <summary>
        /// 初始化 <see cref="RegexDFAState{T}"/> 类的新实例。
        /// </summary>
        public RegexDFAState() : base() { }

        /// <summary>
        /// 初始化 <see cref="RegexDFAState{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public RegexDFAState(bool isTerminal) : base(isTerminal) { }

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public override bool AttachTransition(RegexFATransition<T, RegexDFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向确定的有限自动机模型的状态中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.AttachTransition(transition);
        }

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图从确定的有限自动机模型的状态中移除一个 ε 转换。</exception>
        public override bool RemoveTransition(RegexFATransition<T, RegexDFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向确定的有限自动机模型的状态中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.RemoveTransition(transition);
        }

        /// <summary>
        /// 获取可以接受指定输入并进行转换的转换。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>可以接受指定输入并进行转换的转换。</returns>
        public RegexFATransition<T, RegexDFAState<T>> GetTransitTransition(T input)
        {
            // 遍历当前状态的所有转换。
            foreach (var transition in this.Transitions)
                // 若有转换接受输入，则进行转换操作。
                if (transition.Predicate(input))
                    return transition;

            // 无转换接受输入
            return null;
        }

        #region IRegexFSMTransition{T} Implementation
        IRegexFSMTransition<T> IRegexFSMState<T>.GetTransitTransition(T input) =>
            this.GetTransitTransition(input);
        #endregion
    }
}
