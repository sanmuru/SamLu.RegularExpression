using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using SamLu.DebugView;

namespace SamLu.RegularExpression.StateMachine
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DefaultMember("Item")]
    internal sealed class RegexFAStateGroup<TRegexFAState> : ICollection<TRegexFAState>, IEquatable<RegexFAStateGroup<TRegexFAState>>
        where TRegexFAState : IState
    {
        private HashSet<TRegexFAState> states;
        //public ICollection<TRegexFAState> States => new ReadOnlyCollection<TRegexFAState>(this.states);

        /// <summary>
        /// 获取 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中包含的元素数。
        /// </summary>
        public int Count => this.states.Count;

        /// <summary>
        /// 初始化 <see cref="RegexFAStateGroup{TRegexFAState}"/> 类的新实例，该实例为空。
        /// </summary>
        public RegexFAStateGroup()
        {
            this.states = new HashSet<TRegexFAState>();
        }

        /// <summary>
        /// 初始化 <see cref="RegexFAStateGroup{TRegexFAState}"/> 类的新实例，该实例包含从指定集合复制的元素。
        /// </summary>
        /// <param name="collection">一个集合，其元素被复制到新列表中。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 的值为 null 。</exception>
        public RegexFAStateGroup(IEnumerable<TRegexFAState> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            this.states = new HashSet<TRegexFAState>(collection);
        }

        /// <summary>
        /// 初始化 <see cref="RegexFAStateGroup{TRegexFAState}"/> 类的新实例，该实例包含从指定元素数组复制的元素。
        /// </summary>
        /// <param name="collection">一个元素数组，其元素被复制到新列表中。</param>
        public RegexFAStateGroup(params TRegexFAState[] collection) : this(collection?.AsEnumerable()) { }

        public override bool Equals(object obj)
        {
            return (obj != null && obj is RegexFAStateGroup<TRegexFAState> group && this.Equals(group));
        }

        public bool Equals(RegexFAStateGroup<TRegexFAState> group)
        {
            if (this.Count == 0)
            {
                if (group == null || group.Count == 0) return true;
                else return false;
            }
            else
            {
                if (group == null || group.Count == 0) return false;
                else if (this.Count != group.Count) return false;
                else
                {
                    List<TRegexFAState> list = new List<TRegexFAState>(group.states);
                    foreach (var state in this.states)
                        if (list.Contains(state))
                            list.Remove(state);
                        else return false;

                    return list.Count == 0;
                }
            }
        }

        /// <summary>
        /// 将对象添加到 <see cref="RegexFAStateGroup{TRegexFAState}"/> 的结尾处。
        /// </summary>
        /// <param name="item">要添加到 <see cref="RegexFAStateGroup{TRegexFAState}"/> 末尾的对象。</param>
        /// <exception cref="ArgumentNullException">如果 <paramref name="item"/> 的值为 null 。</exception>
        public void Add(TRegexFAState item)
        {
            if (!typeof(TRegexFAState).IsValueType && item == null)
                throw new ArgumentNullException(nameof(item));

            this.states.Add(item);
        }

        /// <summary>
        /// 从 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中移除所有元素。
        /// </summary>
        public void Clear() => this.states.Clear();

        /// <summary>
        /// 确定某元素是否在 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中。
        /// </summary>
        /// <param name="item">要在 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中定位的对象。</param>
        /// <returns>如果在 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中找到 item，则为 true；否则为 false。</returns>
        /// <exception cref="ArgumentNullException">如果 <paramref name="item"/> 的值为 null 。</exception>
        public bool Contains(TRegexFAState item)
        {
            if (!typeof(TRegexFAState).IsValueType && item == null)
                throw new ArgumentNullException(nameof(item));

            return this.states.Contains(item);
        }

        /// <summary>
        /// 从目标数组的指定索引处开始，将整个 <see cref="RegexFAStateGroup{TRegexFAState}"/> 复制到兼容的一维数组。
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/>，它是从 <see cref="RegexFAStateGroup{TRegexFAState}"/> 复制的元素的目标。<see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(TRegexFAState[] array, int arrayIndex) => this.states.CopyTo(array, arrayIndex);

        /// <summary>
        /// 从 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">要从 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中删除的对象。</param>
        /// <returns>如果成功移除了 item，则为 true；否则为 false。 如果在 <see cref="RegexFAStateGroup{TRegexFAState}"/> 中没有找到 item，则此方法也会返回 false 。</returns>
        /// <exception cref="ArgumentNullException">如果 <paramref name="item"/> 的值为 null 。</exception>
        public bool Remove(TRegexFAState item)
        {
            if (!typeof(TRegexFAState).IsValueType && item == null)
                throw new ArgumentNullException(nameof(item));

            return this.states.Remove(item);
        }

        /// <summary>
        /// 返回循环访问 <see cref="RegexFAStateGroup{TRegexFAState}"/> 的枚举器。
        /// </summary>
        /// <returns><see cref="RegexFAStateGroup{TRegexFAState}"/> 的枚举器。</returns>
        public IEnumerator<TRegexFAState> GetEnumerator() => this.states.GetEnumerator();

        #region ICollection{TRegexFAState} Implementation
        bool ICollection<TRegexFAState>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
