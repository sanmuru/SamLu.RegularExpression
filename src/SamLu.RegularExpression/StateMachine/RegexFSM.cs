using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的有限状态机。
    /// </summary>
    public class RegexFSM<T> : FSM, IRegexFSM<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的当前状态。
        /// </summary>
        new public virtual IRegexFSMState<T> CurrentState
        {
            get => (IRegexFSMState<T>)base.CurrentState;
            protected set => this.CurrentState = value;
        }

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的起始状态。
        /// </summary>
        new public IRegexFSMState<T> StartState
        {
            get => (IRegexFSMState<T>)base.StartState;
            set => base.StartState = value;
        }

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的状态集。
        /// </summary>
        new public virtual ICollection<IRegexFSMState<T>> States =>
            new ReadOnlyCollection<IRegexFSMState<T>>(
                this.StartState == null ?
                    new List<IRegexFSMState<T>>() :
                    this.StartState.RecurGetStates().Cast<IRegexFSMState<T>>().ToList()
            );

        #region AttachTransition
        /// <summary>
        /// 为 <see cref="RegexFSM{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool AttachTransition(IState state, ITransition transition) =>
            this.AttachTransition((IRegexFSMState<T>)state, (IRegexFSMTransition<T>)transition);

        /// <summary>
        /// 为 <see cref="RegexFSM{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            base.AttachTransition(state, transition);
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 从 <see cref="RegexFSM{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool RemoveTransition(IState state, ITransition transition) =>
            this.RemoveTransition((IRegexFSMState<T>)state, (IRegexFSMTransition<T>)transition);

        /// <summary>
        /// 从 <see cref="RegexFSM{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            base.RemoveTransition(state, transition);
        #endregion

        #region SetTarget
        /// <summary>
        /// 将 <see cref="RegexFSM{T}"/> 的一个指定转换的目标设为指定状态。
        /// </summary>
        /// <param name="transition">指定的目标。</param>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool SetTarget(ITransition transition, IState state) =>
            this.SetTarget((IRegexFSMTransition<T>)transition, (IRegexFSMState<T>)state);

        /// <summary>
        /// 将 <see cref="RegexFSM{T}"/> 的一个指定转换的目标设为指定状态。
        /// </summary>
        /// <param name="transition">指定的目标。</param>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state) =>
            base.SetTarget(transition, state);
        #endregion

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool Transit(T input)
        {
            // 获取可以接受输入并进行转换的转换。
            var transition = this.CurrentState.GetTransitTransition(input);

            if (transition == null)
                // 无可行的转换，操作不成功。
                return false;
            else
                return this.Transit(transition);
        }

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <typeparam name="TInput">输入的类型。</typeparam>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public override bool Transit<TInput>(TInput input)
        {
            if (input is T)
                return this.Transit((T)(object)input);
            else
                return base.Transit(input);
        }
    }

    /// <summary>
    /// 表示正则表达式构造的有限状态机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">有限状态机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">有限状态机的转换的类型。</typeparam>
    public class RegexFSM<T, TState, TTransition> : FSM<TState, TTransition>, IRegexFSM<T, TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T,TState>
    {
        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool Transit(T input)
        {
            // 获取可以接受输入并进行转换的转换。
            var transition = this.CurrentState.GetTransitTransition(input);

            if (transition == null)
                // 无可行的转换，操作不成功。
                return false;
            else
                return this.Transit(transition);
        }

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <typeparam name="TInput">输入的类型。</typeparam>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public override bool Transit<TInput>(TInput input)
        {
            if (input is T)
                return this.Transit((T)(object)input);
            else
                return base.Transit(input);
        }

        #region IRegexFSM{T} Implementation
        IRegexFSMState<T> IRegexFSM<T>.CurrentState => base.CurrentState;

        IRegexFSMState<T> IRegexFSM<T>.StartState => base.StartState;

        ICollection<IRegexFSMState<T>> IRegexFSM<T>.States =>
            new ReadOnlyCollection<IRegexFSMState<T>>(
                this.StartState == null ?
                    new List<IRegexFSMState<T>>() :
                    ((IEnumerable<IRegexFSMState<T>>)this.StartState.RecurGetStates<TState, TTransition>()).ToList()
            );

        bool IRegexFSM<T>.AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            this.AttachTransition((TState)state, (TTransition)transition);

        bool IRegexFSM<T>.RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            this.RemoveTransition((TState)state, (TTransition)transition);

        bool IRegexFSM<T>.SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state) =>
            this.SetTarget((TTransition)transition, (TState)state);
        #endregion
    }
}
