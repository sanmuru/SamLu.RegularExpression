using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的有限状态机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexFSM<T> : IFSM
    {
        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 的当前状态。
        /// </summary>
        new IRegexFSMState<T> CurrentState { get; }
        
        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 的起始状态。
        /// </summary>
        new IRegexFSMState<T> StartState { get; }

        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 的状态集。
        /// </summary>
        new ICollection<IRegexFSMState<T>> States { get; }
        
        event RegexFSMMatchEventHandler<T> Match;

        void EndMatch();

        /// <summary>
        /// 向 <see cref="IRegexFSM{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        bool AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition);

        /// <summary>
        /// 从 <see cref="IRegexFSM{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        bool RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition);

        /// <summary>
        /// 将 <see cref="IRegexFSM{T}"/> 的一个指定转换的目标设为指定状态。
        /// </summary>
        /// <param name="transition">指定的目标。</param>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        bool SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state);

        /// <summary>
        /// 接受一个指定输入序列并进行一组转换动作。
        /// </summary>
        /// <param name="inputs">指定的输入序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputs"/> 的值为 null 。</exception>
        void TransitMany(IEnumerable<T> inputs);
    }

    /// <summary>
    /// 定义了正则表达式构造的有限状态机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">有限状态机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">有限状态机的转换的类型。</typeparam>
    public interface IRegexFSM<T, TState, TTransition> : IRegexFSM<T>, IFSM<TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
    }
}
