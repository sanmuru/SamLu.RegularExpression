using SamLu.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace SamLu.RegularExpression.StateMachine.ObjectModel
{
    /// <summary>
    /// 作为正则构造的状态机的缓存数据之一的状态栈。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public partial class StateStack<T> : IEnumerable<StateStack<T>.Item>, ICollection, IEnumerable
    {
        private Stack<Item> innerStack = new Stack<Item>();
        private CaptureStack<T> captureStack;

        /// <summary>
        /// 初始化 <see cref="StateStack{T}"/> 类的新实例。
        /// </summary>
        /// <param name="captureStack"></param>
        public StateStack(CaptureStack<T> captureStack)
        {
            if (captureStack == null) throw new ArgumentNullException(nameof(captureStack));

            this.captureStack = captureStack;
        }

        /// <summary>
        /// 获取 <see cref="StateStack{T}"/> 中包含的元素数。
        /// </summary>
        /// <value>
        /// <see cref="StateStack{T}"/> 中包含的元素数。
        /// </value>
        public int Count => this.innerStack.Count;

        /// <summary>
        /// 从 <see cref="StateStack{T}"/> 中移除所有对象。
        /// </summary>
        public void Clear()
        {
            lock (((ICollection)this).SyncRoot)
            {
                this.innerStack.Clear();
                this.captureStack.Clear(); 
            }
        }

        /// <summary>
        /// 确定某元素是否在 <see cref="StateStack{T}"/> 中。
        /// </summary>
        /// <param name="item">要在 System.Collections.Generic.Stack`1 中定位的对象。</param>
        /// <returns>如果在 <see cref="StateStack{T}"/> 中找到 <paramref name="item"/> ，则为 true ；否则为 false 。</returns>
        public bool Contains(Item item) => this.innerStack.Contains(item);

        /// <summary>
        /// 从指定的数组索引处开始复制 <see cref="StateStack{T}"/> 到现有一维 <see cref="Array"/> 。
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/> ，它是从 <see cref="StateStack{T}"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(Item[] array, int arrayIndex) => this.innerStack.CopyTo(array, arrayIndex);

        /// <summary>
        /// 返回 <see cref="StateStack{T}"/> 的枚举数。
        /// </summary>
        /// <returns><see cref="StateStack{T}"/> 的枚举数。</returns>
        public IEnumerator<Item> GetEnumerator() => this.innerStack.GetEnumerator();

        /// <summary>
        /// 返回 <see cref="StateStack{T}"/> 的顶部的对象而不删除它。
        /// </summary>
        /// <returns>在 <see cref="StateStack{T}"/> 顶部的对象。</returns>
        public Item Peek() => this.innerStack.Peek();

        /// <summary>
        /// 移除并返回 <see cref="StateStack{T}"/> 的顶部的对象。
        /// </summary>
        /// <returns>从 <see cref="StateStack{T}"/> 顶部删除的对象。</returns>
        public Item Pop()
        {
            lock (((ICollection)this).SyncRoot)
            {
                var item = this.innerStack.Pop();

                while (this.captureStack.Count != 0 && this.captureStack.Peek().StateStackCount > this.Count)
                    this.captureStack.Pop();

                return item;
            }
        }

        /// <summary>
        /// 在 <see cref="StateStack{T}"/> 的顶部插入一个对象。
        /// </summary>
        /// <param name="item">要推入到 <see cref="StateStack{T}"/> 中的对象。</param>
        public void Push(Item item) => this.innerStack.Push(item);

        /// <summary>
        /// 复制 <see cref="StateStack{T}"/> 到新数组。
        /// </summary>
        /// <returns>包含 <see cref="StateStack{T}"/> 的元素的副本的新数组。</returns>
        public Item[] ToArray() => this.innerStack.ToArray();

        #region Interfaces Implementation
        object ICollection.SyncRoot => ((ICollection)this.innerStack).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)this.innerStack).IsSynchronized;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.innerStack).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => ((ICollection)this.innerStack).GetEnumerator();
        #endregion
    }

    partial class StateStack<T>
    {
        /// <summary>
        /// 表示 <see cref="StateStack{T}"/> 的项。
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 获取初始化此实例时正则构造的状态机的 <see cref="IRegexFSM{T}.Index"/> 。
            /// </summary>
            public int RegexFSMIndex { get; set; }
            /// <summary>
            /// 获取初始化此实例时正则构造的状态机的当前状态。
            /// </summary>
            public IRegexFSMState<T> State { get; set; }
            /// <summary>
            /// 获取由初始化此实例时正则构造的状态机的当前状态的转换集构造的读取源。
            /// </summary>
            public IReaderSource<IRegexFSMTransition<T>> TransitionReader { get; set; }
        }
    }
}
