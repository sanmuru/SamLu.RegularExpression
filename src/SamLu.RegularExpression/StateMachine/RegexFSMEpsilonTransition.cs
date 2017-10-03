using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的有限状态机的 ε 转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public sealed class RegexFSMEpsilonTransition<T> : FSMTransition, IRegexFSMEpsilonTransition<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMEpsilonTransition{T}"/> 指向的状态。
        /// </summary>
        new public IRegexFSMState<T> Target => (IRegexFSMState<T>)base.Target;

        /// <summary>
        /// 初始化 <see cref="RegexFSMEpsilonTransition{T}"/> 类的新实例。
        /// </summary>
        public RegexFSMEpsilonTransition() : base() { }

        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">要设为目标的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public sealed override bool SetTarget(IState state) => this.SetTarget((IRegexFSMState<T>)state);

        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">要设为目标的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public bool SetTarget(IRegexFSMState<T> state) => base.SetTarget(state);
    }
}
