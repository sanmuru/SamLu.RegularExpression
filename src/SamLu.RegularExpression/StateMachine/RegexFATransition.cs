using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的有限自动机的转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TRegexFAState">正则表达式构造的有限自动机的状态的类型。</typeparam>
    internal class RegexFATransition<T, TRegexFAState> : FSMTransition<TRegexFAState>, ITransition<TRegexFAState>
        where TRegexFAState : IState
    {
        private Predicate<T> predicate;
        /// <summary>
        /// 获取一个方法，该方法确定 <see cref="RegexFATransition{T, TRegexFAState}"/> 接受的输入是否满足条件。
        /// </summary>
        public virtual Predicate<T> Predicate => this.predicate;

        /// <summary>
        /// 初始化 <see cref="RegexFATransition{T, TRegexFAState}"/> 类的新实例。
        /// </summary>
        protected RegexFATransition() { }

        /// <summary>
        /// 初始化 <see cref="RegexFAStateGroup{TRegexFAState}"/> 类的新实例。该实例使用指定的确定 <see cref="RegexFATransition{T, TRegexFAState}"/> 接受的输入是否满足条件的方法。
        /// </summary>
        /// <param name="predicate">指定的确定 <see cref="RegexFATransition{T, TRegexFAState}"/> 接受的输入是否满足条件的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 的值为 null 。</exception>
        public RegexFATransition(Predicate<T> predicate) : this()
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }
    }
}
