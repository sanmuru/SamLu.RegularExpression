using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexDFA<T> : RegexFSM<T>, IRegexDFA<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexDFA{T}"/> 类的新实例。
        /// </summary>
        public RegexDFA() : base() { }

        #region AttachTransition
        /// <summary>
        /// 为 <see cref="RegexDFA{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            this.AttachTransition((IRegexDFAState<T>)state, (IAcceptInputTransition<T>)transition);

        /// <summary>
        /// 为 <see cref="RegexDFA{T}"/> 的一个指定状态添加指定接受输入转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="InvalidOperationException">在 <paramref name="acceptInputTransition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public bool AttachTransition(IRegexDFAState<T> state, IAcceptInputTransition<T> acceptInputTransition)
        {
            if (acceptInputTransition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向正则表达式构造的确定的有限自动机模型中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(acceptInputTransition))
                );

            return base.AttachTransition(state, acceptInputTransition);
        }
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 从 <see cref="RegexDFA{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public override bool RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            this.RemoveTransition(state, transition);

        /// <summary>
        /// 从 <see cref="RegexDFA{T}"/> 的一个指定状态移除指定接受输入转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="InvalidOperationException">在 <paramref name="acceptInputTransition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public bool RemoveTransition(IRegexDFAState<T> state, IAcceptInputTransition<T> acceptInputTransition)
        {
            if (acceptInputTransition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向正则表达式构造的确定的有限自动机模型中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(acceptInputTransition))
                );

            return base.RemoveTransition(state, acceptInputTransition);
        }
        #endregion
    }

    /// <summary>
    /// 表示正则表达式构造的确定的有限自动机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的确定的有限自动机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">正则构造的确定的有限自动机的转换的类型。</typeparam>
    public class RegexDFA<T, TState, TTransition> : RegexFSM<T, TState, TTransition>, IRegexDFA<T, TState, TTransition>
        where TState : IRegexDFAState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>, IAcceptInputTransition<T>
    {
        /// <summary>
        /// 初始化 <see cref="RegexDFA{T, TState, TTransition}"/> 类的新实例。
        /// </summary>
        public RegexDFA() : base() { }

        /// <summary>
        /// 为 <see cref="RegexDFA{T, TState, TTransition}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public override bool AttachTransition(TState state, TTransition transition)
        {
            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向正则表达式构造的确定的有限自动机模型中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.AttachTransition(state, transition);
        }

        /// <summary>
        /// 从 <see cref="RegexDFA{T, TState, TTransition}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public override bool RemoveTransition(TState state, TTransition transition)
        {
            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向正则表达式构造的确定的有限自动机模型中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.RemoveTransition(state, transition);
        }

        #region IRegexDFA{T} Implementation
        bool IRegexDFA<T>.AttachTransition(IRegexDFAState<T> state, IAcceptInputTransition<T> acceptInputTransition) =>
            this.AttachTransition((TState)state, (TTransition)acceptInputTransition);

        bool IRegexDFA<T>.RemoveTransition(IRegexDFAState<T> state, IAcceptInputTransition<T> acceptInputTransition) =>
            this.RemoveTransition((TState)state, (TTransition)acceptInputTransition);
        #endregion
    }
}
