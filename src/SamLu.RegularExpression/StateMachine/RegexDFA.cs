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
    public class RegexDFA<T> : DFA<RegexDFAState<T>, RegexFATransition<T, RegexDFAState<T>>>
    {
        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool Transit(T input)
        {
            // 遍历当前状态的所有转换。
            foreach (var transition in this.CurrentState.Transitions)
                // 若有转换接受输入，则进行转换操作。
                if (transition.Predicate(input))
                {
                    return this.Transit(transition);
                }

            // 无转换接受输入，操作不成功。
            return false;
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
}
