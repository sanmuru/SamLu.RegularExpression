using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public sealed class RegexPositiveCaptureTransition<T> : FSMTransition, IRegexFunctionalTransition<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexPositiveCaptureTransition{T}"/> 指向的状态。
        /// </summary>
        new public IRegexFSMState<T> Target => (IRegexFSMState<T>)base.Target;

        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public bool SetTarget(IRegexFSMState<T> state) => base.SetTarget(state);
    }

    public sealed class RegexPositiveCaptureTransition<T, TState> : FSMTransition<TState>, IRegexFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        #region IRegexFSMTransition{T} Implementation
        IRegexFSMState<T> IRegexFSMTransition<T>.Target => this.Target;

        bool IRegexFSMTransition<T>.SetTarget(IRegexFSMState<T> state) =>
            base.SetTarget((TState)state);
        #endregion
    }
}
