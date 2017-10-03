using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 通用的表示正则表达式构造的有限状态机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public sealed class CommonRegexFAState<T> : RegexFSMState<T>, IRegexNFAState<T>, IRegexDFAState<T>
    {
        /// <summary>
        /// 创建新的 NFA 非结束状态。
        /// </summary>
        public static IRegexNFAState<T> NFAState => new CommonRegexFAState<T>(false, false);

        /// <summary>
        /// 创建新的 NFA 结束状态。
        /// </summary>
        public static IRegexNFAState<T> TerminalNFAState => new CommonRegexFAState<T>(false, true);

        /// <summary>
        /// 创建新的 DFA 非结束状态。
        /// </summary>
        public static IRegexDFAState<T> DFAState => new CommonRegexFAState<T>(true, false);

        /// <summary>
        /// 创建新的 DFA 结束状态。
        /// </summary>
        public static IRegexDFAState<T> TerminalDFAState => new CommonRegexFAState<T>(true, true);

        private bool isDFAState;

        /// <summary>
        /// 获取一个值，指示此 <see cref="CommonRegexFAState{T}"/> 是否为 NFA 状态。
        /// </summary>
        public bool IsNFAState { get => !this.isDFAState; }

        /// <summary>
        /// 获取一个值，指示此 <see cref="CommonRegexFAState{T}"/> 是否为 DFA 状态。
        /// </summary>
        public bool IsDFAState { get => this.IsDFAState; }

        /// <summary>
        /// 初始化 <see cref="CommonRegexFAState{T}"/> 类的新实例。
        /// </summary>
        /// <param name="isDFAState">一个值，指示此 <see cref="CommonRegexFAState{T}"/> 是否为 DFA 状态。</param>
        /// <param name="isTerminal">一个值，指示此 <see cref="CommonRegexFAState{T}"/> 是否为结束状态。</param>
        private CommonRegexFAState(bool isDFAState, bool isTerminal) : base(isTerminal) => this.isDFAState = isDFAState;

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 不为 <see cref="IAcceptInputTransition{T}"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个非接受输入转换。</exception>
        public sealed override bool AttachTransition(IRegexFSMTransition<T> transition)
        {
            if (this.IsDFAState)
            {
                if (transition is IEpsilonTransition)
                    throw new InvalidOperationException(
                        "试图向正则表达式构造的确定的有限自动机模型中添加一个 ε 转换。",
                        new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                    );
                else if (transition is IAcceptInputTransition<T>)
                    return base.AttachTransition((IAcceptInputTransition<T>)transition);
                else
                    throw new InvalidOperationException(
                        "试图向正则表达式构造的确定的有限自动机模型中添加一个非接受输入转换。",
                        new ArgumentException("无法接受的非接受输入转换。", nameof(transition))
                    );
            }
            else
                return base.AttachTransition(transition);
        }

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 不为 <see cref="IAcceptInputTransition{T}"/> 接口的实例时抛出。试图向正则表达式构造的确定的有限自动机模型的状态中添加一个非接受输入转换。</exception>
        public sealed override bool RemoveTransition(IRegexFSMTransition<T> transition)
        {
            if (this.IsDFAState)
            {
                if (transition is IEpsilonTransition)
                    throw new InvalidOperationException(
                        "试图向正则表达式构造的确定的有限自动机模型中添加一个 ε 转换。",
                        new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                    );
                else if (transition is IAcceptInputTransition<T>)
                    return base.RemoveTransition((IAcceptInputTransition<T>)transition);
                else
                    throw new InvalidOperationException(
                        "试图向正则表达式构造的确定的有限自动机模型中添加一个非接受输入转换。",
                        new ArgumentException("无法接受的非接受输入转换。", nameof(transition))
                    );
            }
            else
                return base.RemoveTransition(transition);
        }

        #region IRegexNFAState<T>, IRegexDFAState<T> Implementation
        bool IRegexNFAState<T>.AttachTransition(IRegexFSMEpsilonTransition<T> epsilonTransition) => this.AttachTransition(epsilonTransition);

        bool IRegexNFAState<T>.RemoveTransition(IRegexFSMEpsilonTransition<T> epsilonTransition) => this.RemoveTransition(epsilonTransition);

        bool INFAState.AttachTransition(IEpsilonTransition epsilonTransition) => this.AttachTransition(epsilonTransition);

        bool INFAState.RemoveTransition(IEpsilonTransition epsilonTransition) => this.RemoveTransition(epsilonTransition);

        bool IRegexDFAState<T>.AttachTransition(IAcceptInputTransition<T> acceptInputTransition) => this.AttachTransition(acceptInputTransition);

        bool IRegexDFAState<T>.RemoveTransition(IAcceptInputTransition<T> acceptInputTransition) => this.RemoveTransition(acceptInputTransition);
        #endregion
    }
}
