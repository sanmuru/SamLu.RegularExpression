using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class BasicRegexDFA<T> : RegexFSM<T, BasicRegexDFAState<T>, BasicRegexFATransition<T, BasicRegexDFAState<T>>>, IDFA<BasicRegexDFAState<T>, BasicRegexFATransition<T, BasicRegexDFAState<T>>>
    {
        /// <summary>
        /// 为 <see cref="BasicRegexDFA{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public override bool AttachTransition(BasicRegexDFAState<T> state, BasicRegexFATransition<T, BasicRegexDFAState<T>> transition)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向确定的有限自动机模型中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.AttachTransition(state, transition);
        }

        /// <summary>
        /// 从 <see cref="BasicRegexDFA{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图从确定的有限自动机模型的状态中移除一个 ε 转换。</exception>
        public override bool RemoveTransition(BasicRegexDFAState<T> state, BasicRegexFATransition<T, BasicRegexDFAState<T>> transition)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图从确定的有限自动机模型中移除一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.RemoveTransition(state, transition);
        }
    }
}
