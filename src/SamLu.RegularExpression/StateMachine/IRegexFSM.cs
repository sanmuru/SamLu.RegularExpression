using SamLu.RegularExpression.StateMachine.Service;
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
        /// 获取或设置 <see cref="IRegexFSM{T}"/> 的起始状态。
        /// </summary>
        new IRegexFSMState<T> StartState { get; set; }

        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 的状态集。
        /// </summary>
        new ICollection<IRegexFSMState<T>> States { get; }

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
        /// <param name="transition">指定的转换。</param>
        /// <param name="state">要设为目标的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        bool SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state);

        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 匹配过程的输入对象序列。
        /// </summary>
        IEnumerable<T> Inputs { get; }

        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 匹配过程的在 <see cref="Inputs"/> 中的当前位置。
        /// </summary>
        int Index { get; }

        /// <summary>
        /// <see cref="IRegexFSM{T}"/> 的匹配事件。
        /// </summary>
        event RegexFSMMatchEventHandler<T> Match;

        /// <summary>
        /// <see cref="IRegexFSM{T}"/> 的所有匹配。
        /// </summary>
        MatchCollection<T> Matches { get; }
        
        /// <summary>
        /// 记录一个匹配。
        /// </summary>
        /// <param name="idToken">匹配的 ID 标志符。</param>
        /// <param name="id">捕获的 ID 。</param>
        /// <param name="start">捕获的开始位置。</param>
        /// <param name="length">捕获的长度。</param>
        void Capture(object idToken, object id, int start, int length);

        /// <summary>
        /// 尝试获取 <see cref="IRegexFSM{T}"/> 的指定匹配。
        /// </summary>
        /// <param name="idToken">匹配的 ID 标志符。</param>
        /// <param name="id">捕获的 ID 。</param>
        /// <param name="start">捕获的开始位置。</param>
        /// <param name="length">捕获的长度。</param>
        /// <returns>一个值，指示 <see cref="IRegexFSM{T}"/> 是否含有指定的匹配。</returns>
        bool TryGetLastCapture(object idToken, object id, out int start, out int length);

        /// <summary>
        /// 接受一个指定输入序列并进行一组转换动作。
        /// </summary>
        /// <param name="inputs">指定的输入序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputs"/> 的值为 null 。</exception>
        void TransitMany(IEnumerable<T> inputs);

        /// <summary>
        /// 获取 <see cref="IRegexFSM{T}"/> 的服务。
        /// </summary>
        /// <typeparam name="TService">服务的类型。</typeparam>
        /// <returns><see cref="IRegexFSM{T}"/> 的指定类型的服务。</returns>
        TService GetService<TService>() where TService : IRegexFSMService<T>, new();
    }

    /// <summary>
    /// 定义了正则表达式构造的有限状态机应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">正则构造的有限状态机的转换的类型。</typeparam>
    public interface IRegexFSM<T, TState, TTransition> : IRegexFSM<T>, IFSM<TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
        /// <summary>
        /// 获取 <see cref="IRegexFSM{T, TState, TTransition}"/> 的服务。
        /// </summary>
        /// <typeparam name="TService">服务的类型。</typeparam>
        /// <returns><see cref="IRegexFSM{T, TState, TTransition}"/> 的指定类型的服务。</returns>
        new TService GetService<TService>() where TService : IRegexFSMService<T, TState, TTransition>, new();
    }
}
