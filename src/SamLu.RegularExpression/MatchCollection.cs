using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 包含所有正则匹配的集合。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SamLu.DebugView.CollectionDebugView<>))]
    public class MatchCollection<T> : IReadOnlyList<Match<T>>, ICollection
    {
        private IReadOnlyList<Match<T>> innerList;

        /// <summary>
        /// 获取 <see cref="MatchCollection{T}"/> 中包含的元素数。
        /// </summary>
        public int Count => this.innerList.Count;

        /// <summary>
        ///  获取可用于同步对 <see cref="MatchCollection{T}"/> 的访问的对象。
        /// </summary>
        public object SyncRoot => this;

        /// <summary>
        /// 获取一个值，该值指示是否同步对 <see cref="MatchCollection{T}"/> 的访问（线程安全）。
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// 获取位于只读列表中指定索引处的元素。
        /// </summary>
        /// <param name="index">要获取的元素的索引（索引从零开始）。</param>
        /// <returns>在只读列表中指定索引处的元素。</returns>
        public Match<T> this[int index] => this.innerList[index];

        /// <summary>
        /// 初始化 <see cref="MatchCollection{T}"/> 类的新实例，该实例指定包含的匹配集合。
        /// </summary>
        /// <param name="matches">指定包含的匹配集合。</param>
        public MatchCollection(IEnumerable<Match<T>> matches) :
            this((matches ?? throw new ArgumentNullException(nameof(matches))).ToList())
        { }

        /// <summary>
        /// 初始化 <see cref="MatchCollection{T}"/> 类的新实例，该实例指定包含的匹配列表。
        /// </summary>
        /// <param name="matches">指定包含的匹配列表。</param>
        public MatchCollection(IList<Match<T>> matches)
        {
            if (matches == null) throw new ArgumentNullException(nameof(matches));

            this.innerList = new ReadOnlyCollection<Match<T>>(matches);
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<Match<T>> GetEnumerator() => this.innerList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.innerList.GetEnumerator();

        /// <summary>
        /// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="MatchCollection{T}"/> 的元素复制到一个 <see cref="Array"/> 。
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/> ，它是从 <see cref="MatchCollection{T}"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，从此处开始复制。</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 的值为 null 。</exception>
        /// <exception cref=" ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 的值小于 0 。</exception>
        /// <exception cref="ArgumentException">源 <see cref="MatchCollection{T}"/> 中的元素数目大于从目标 <paramref name="array"/> 的 <paramref name="arrayIndex"/> 从头到尾的可用空间。</exception>
        public void CopyTo(Match<T>[] array, int arrayIndex) => (this.innerList as ICollection<Match<T>>).CopyTo(array, arrayIndex);

        void ICollection.CopyTo(Array array, int index) => (this.innerList as ICollection).CopyTo(array, index);
    }
}
