using SamLu.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.ObjectModel
{
    /// <summary>
    /// 作为正则构造的状态机的缓存数据之一的捕获栈。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public partial class CaptureStack<T> : IEnumerable<CaptureStack<T>.Item>, ICollection, IEnumerable
    {
        private Stack<Item> innerStack = new Stack<Item>();

        /// <summary>
        /// 初始化 <see cref="CaptureStack{T}"/> 类的新实例。
        /// </summary>
        public CaptureStack() { }

        /// <summary>
        /// 获取 <see cref="CaptureStack{T}"/> 中包含的元素数。
        /// </summary>
        /// <value>
        /// <see cref="CaptureStack{T}"/> 中包含的元素数。
        /// </value>
        public int Count => this.innerStack.Count;

        /// <summary>
        /// 从 <see cref="CaptureStack{T}"/> 中移除所有对象。
        /// </summary>
        public void Clear() => this.innerStack.Clear();

        /// <summary>
        /// 确定某元素是否在 <see cref="CaptureStack{T}"/> 中。
        /// </summary>
        /// <param name="item">要在 System.Collections.Generic.Stack`1 中定位的对象。</param>
        /// <returns>如果在 <see cref="CaptureStack{T}"/> 中找到 <paramref name="item"/> ，则为 true ；否则为 false 。</returns>
        public bool Contains(Item item) => this.innerStack.Contains(item);

        /// <summary>
        /// 从指定的数组索引处开始复制 <see cref="CaptureStack{T}"/> 到现有一维 <see cref="Array"/> 。
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/> ，它是从 <see cref="CaptureStack{T}"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(Item[] array, int arrayIndex) => this.innerStack.CopyTo(array, arrayIndex);

        /// <summary>
        /// 返回 <see cref="CaptureStack{T}"/> 的枚举数。
        /// </summary>
        /// <returns><see cref="CaptureStack{T}"/> 的枚举数。</returns>
        public IEnumerator<Item> GetEnumerator() => this.innerStack.GetEnumerator();

        /// <summary>
        /// 返回 <see cref="CaptureStack{T}"/> 的顶部的对象而不删除它。
        /// </summary>
        /// <returns>在 <see cref="CaptureStack{T}"/> 顶部的对象。</returns>
        public Item Peek() => this.innerStack.Peek();

        /// <summary>
        /// 移除并返回 <see cref="CaptureStack{T}"/> 的顶部的对象。
        /// </summary>
        /// <returns>从 <see cref="CaptureStack{T}"/> 顶部删除的对象。</returns>
        public Item Pop() => this.innerStack.Pop();

        /// <summary>
        /// 在 <see cref="CaptureStack{T}"/> 的顶部插入一个对象。
        /// </summary>
        /// <param name="item">要推入到 <see cref="CaptureStack{T}"/> 中的对象。</param>
        public void Push(Item item) => this.innerStack.Push(item);

        /// <summary>
        /// 复制 <see cref="CaptureStack{T}"/> 到新数组。
        /// </summary>
        /// <returns>包含 <see cref="CaptureStack{T}"/> 的元素的副本的新数组。</returns>
        public Item[] ToArray() => this.innerStack.ToArray();

        #region Interfaces Implementation
        object ICollection.SyncRoot => ((ICollection)this.innerStack).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)this.innerStack).IsSynchronized;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.innerStack).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => ((ICollection)this.innerStack).GetEnumerator();
        #endregion
    }

    partial class CaptureStack<T>
    {
        /// <summary>
        /// 表示 <see cref="CaptureStack{T}"/> 的项。
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 获取构造此实例时 <see cref="StateStack{T}"/> 的 <see cref="StateStack{T}.Count"/> 。
            /// </summary>
            public int StateStackCount { get; set; }
            /// <summary>
            /// 获取捕获的 ID 标识符。
            /// </summary>
            public object CaptureIDToken { get; set; }
            /// <summary>
            /// 获取捕获的 ID 。
            /// </summary>
            public object CaptureID { get; set; }
            /// <summary>
            /// 获取捕获的起始位置。
            /// </summary>
            public int CaptureStart { get; set; }
            /// <summary>
            /// 获取捕获的长度。
            /// </summary>
            public int CaptureLength { get; set; }
        }
    }
}
