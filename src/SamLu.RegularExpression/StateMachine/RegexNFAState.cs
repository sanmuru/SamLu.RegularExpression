using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的非确定的有限自动机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexNFAState<T> : NFAState<RegexFATransition<T, RegexNFAState<T>>, RegexNFAEpsilonTransition<T>>, IRegexFSMState<T, RegexFATransition<T, RegexNFAState<T>>>
    {
        /// <summary>
        /// 初始化 <see cref="RegexNFAState{T}"/> 类的新实例。
        /// </summary>
        public RegexNFAState() : this(false) { }

        /// <summary>
        /// 初始化 <see cref="RegexNFAState{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public RegexNFAState(bool isTerminal) : base(isTerminal) { }

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public bool AttachTransition(RegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (epsilonTransition == null) throw new ArgumentNullException(nameof(epsilonTransition));

            return base.AttachTransition(epsilonTransition);
        }

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="epsilonTransition">要添加的 ε 转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="epsilonTransition"/> 的值为 null 。</exception>
        public bool RemoveTransition(RegexNFAEpsilonTransition<T> epsilonTransition)
        {
            if (epsilonTransition == null) throw new ArgumentNullException(nameof(epsilonTransition));

            return base.AttachTransition(epsilonTransition);
        }

        /// <summary>
        /// 获取可以接受指定输入并进行转换的转换。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>可以接受指定输入并进行转换的转换。</returns>
        public RegexFATransition<T, RegexNFAState<T>> GetTransitTransition(T input)
        {
            throw new NotSupportedException();
        }

        #region IRegexFSMTransition{T} Implementation
        IRegexFSMTransition<T> IRegexFSMState<T>.GetTransitTransition(T input) =>
            this.GetTransitTransition(input);
        #endregion
    }
}
