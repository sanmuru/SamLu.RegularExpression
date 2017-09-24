using SamLu.RegularExpression.Extend;
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
    public class BasicRegexDFA<T> : RegexFSM<T, BasicRegexDFAState<T>, BasicRegexFATransition<T, BasicRegexDFAState<T>>>, IRegexDFA<T, BasicRegexDFAState<T>, BasicRegexFATransition<T, BasicRegexDFAState<T>>>
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

        private int start;
        private int index;
        private int? top;
        private Queue<T> Q;

        /// <summary>
        /// 初始化状态机实例必要字段，进行匹配前准备工作。
        /// </summary>
        protected override void BeginMatch(IEnumerable<T> inputs)
        {
            base.BeginMatch(inputs);

            this.start = 0;
            this.index = this.start;
            this.top = null;
            this.Q = new Queue<T>();
        }

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected internal override bool Transit(T input)
        {
            bool isTerminal = this.CurrentState.IsTerminal;

            if (base.Transit(input))
            {
                if (isTerminal)
                {
                    this.top = this.index;
                }

                this.Q.Enqueue(input);
                this.index++;

                return true;
            }
            else
            {
                if (this.top.HasValue)
                {
                    // 提交一个匹配。
                    this.OnMatch(new RegexFSMMatchEventArgs<T>(new Match<T>(
                        this.Inputs, this.start, this.top.Value - this.start + 1, Enumerable.Empty<(object, Group<T>)>()
                    )));

                    // 将表示提交的匹配的数据清零。
                    this.index = this.top.Value + 1;
                    this.top = null;
                    for (int i = 0; i < this.index; i++) this.Q.Dequeue();

                    // 复制出剩余元素。
                    T[] rest = new T[this.Q.Count + 1];
                    this.Q.CopyTo(rest, 0);
                    rest[rest.Length - 1] = input; // 将最后一个元素设置为当前输入。
                    this.Q.Clear();
                    
                    // 重置状态机。
                    this.Reset();
                    foreach (var t in rest)
                    { // 从起始状态开始匹配剩余元素。
                        if (!this.Transit(t))
                            // 若未匹配完且出现匹配失败。
                            return false; // 转换失败。
                    }

                    // 成功匹配所有剩余元素。
                    return true; // 转换成功。
                }
                else return false;
            }
        }

        public override void EndMatch()
        {
            if (this.top.HasValue)
            {
                // 提交一个匹配。
                this.OnMatch(new RegexFSMMatchEventArgs<T>(new Match<T>(
                    this.Inputs, this.start, this.top.Value - this.start + 1, Enumerable.Empty<(object, Group<T>)>()
                )));
            }
        }
    }
}
