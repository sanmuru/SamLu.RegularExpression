using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.IO
{
    /// <summary>
    /// 表示节点读取器。
    /// </summary>
    /// <typeparam name="TReader">内部读取器的类型。</typeparam>
    /// <typeparam name="TNode">节点的类型。</typeparam>
    public class NodeReader<TReader, TNode> : IReaderSource<TNode>
    {
        /// <summary>
        /// 获取 <see cref="NodeReader{TReader, TNode}"/> 内部的读取器。
        /// </summary>
        public virtual TReader Reader { get; protected set; }

        /// <summary>
        /// 内部的节点缓存列表。
        /// </summary>
        protected internal List<TNode> buffer = new List<TNode>();
        /// <summary>
        /// 获取 <see cref="NodeReader{TReader, TNode}"/> 的缓冲区。
        /// </summary>
        public virtual TNode[] Buffer { get { return this.buffer.ToArray(); } }

        /// <summary>
        /// 内部的指针位置。
        /// </summary>
        protected int position = 0;
        /// <summary>
        /// 获取 <see cref="NodeReader{TReader, TNode}"/> 对象中的指针位置。
        /// </summary>
        public virtual int Position
        {
            get { return this.position; }
            set
            {
                if (value < 0) throw new InvalidOperationException("指针指向一个不合法的位置。");

                if (value >= this.buffer.Count) this._ReadBuffer(value - this.buffer.Count);
                if (value >/*=*/ this.buffer.Count) throw new InvalidOperationException("指针指向一个不合法的位置。");

                this.position = value;
            }
        }

        /// <summary>
        /// 封装内部读取器读取数据的方法的委托对象。
        /// </summary>
        protected Func<TReader, TNode> readFunc;
        /// <summary>
        /// 封装验证内部读取器是否读取到末尾的方法的委托对象。
        /// </summary>
        protected Func<TReader, bool> eofFunc;

        /// <summary>
		/// 使用指定的内部读取器、读取方法和判断读取结束的方法初始化 <see cref="NodeReader{TReader, TNode}"/> 类的新实例。
		/// </summary>
		/// <param name="reader">指定的内部读取器。</param>
		/// <param name="readFunc">指定的读取方法。</param>
		/// <param name="eofFunc">指定的判断读取结束的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="readFunc"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eofFunc"/> 的值为 null 。</exception>
		public NodeReader(TReader reader, Func<TReader, TNode> readFunc, Func<TReader, bool> eofFunc)
        {
            if (readFunc == null) throw new ArgumentNullException(nameof(reader));
            if (eofFunc == null) throw new ArgumentNullException(nameof(eofFunc));

            this.Reader = reader;
            this.readFunc = readFunc;
            this.eofFunc = eofFunc;
        }

        /// <summary>
        /// 使用指定的节点数组初始化 <see cref="NodeReader{TReader, TNode}"/> 类的新实例。
        /// </summary>
        /// <param name="nodes">指定的节点数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="nodes"/> 的值为 null 。</exception>
        public NodeReader(params TNode[] nodes) :
            this((IEnumerable<TNode>)(nodes ?? throw new ArgumentNullException(nameof(nodes))))
        { }

        /// <summary>
        /// 使用指定的节点序列初始化 <see cref="NodeReader{TReader, TNode}"/> 类的新实例。
        /// </summary>
        /// <param name="nodes">指定的节点序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="nodes"/> 的值为 null 。</exception>
        public NodeReader(IEnumerable<TNode> nodes) : this(nodes.GetEnumerator())
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            if (typeof(TReader).IsAssignableFrom(typeof(IEnumerable<TNode>))) this.Reader = (TReader)nodes;
        }

        /// <summary>
        /// 使用指定的节点枚举器初始化 <see cref="NodeReader{TReader, TNode}"/> 类的新实例。
        /// </summary>
        /// <param name="enumerator"></param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> 的值为 null 。</exception>
        public NodeReader(IEnumerator<TNode> enumerator)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));

            bool isEof = !enumerator.MoveNext();
            this.readFunc = reader =>
            {
                if (isEof) return default(TNode);
                else
                {
                    TNode next = enumerator.Current;
                    isEof = !enumerator.MoveNext();
                    return next;
                }
            };
            this.eofFunc = reader => isEof;

            if (typeof(TReader).IsAssignableFrom(typeof(IEnumerator<TNode>))) this.Reader = (TReader)enumerator;
        }

        /// <summary>
        /// 调用内部读取器的读取方法向缓冲区读入指定长度的节点。
        /// </summary>
        /// <param name="count">读取节点序列的长度。</param>
        /// <returns>实际读取的节点序列的长度。</returns>
        protected virtual int _ReadBuffer(int count)
        {
            int i = 0;
            while (i++ < count && !this.eofFunc(this.Reader))
                this.buffer.Add(this.readFunc(this.Reader));

            return i - 1;
        }

        #region Peek
        /// <summary>
        /// 返回下一个节点，但不使用。
        /// </summary>
        /// <returns>下一个节点。</returns>
        /// <exception cref="InvalidOperationException">内部读取器已抵达结尾。</exception>
        public virtual TNode Peek()
        {
            if (this.Position == this.buffer.Count) this._ReadBuffer(10);

            if (this.Position == this.buffer.Count) throw new InvalidOperationException("内部读取器已抵达结尾。");
            else return this.buffer[this.Position];
        }

        /// <summary>
        /// 返回当前指针位置后指定长度节点，但并不使用。
        /// </summary>
        /// <param name="count">返回的指定长度。</param>
        /// <returns>实际返回的长度。</returns>
        public virtual IList<TNode> Peek(int count)
        {
            if (this.Position + count >= this.buffer.Count) this._ReadBuffer(count);

            return new List<TNode>(this.PeekInternal(count));
        }

        /// <summary>
        /// 返回当前指针位置后指定长度节点，存入指定数组中，但不使用。
        /// </summary>
        /// <param name="targetArray">承载返回结果的数组。</param>
        /// <param name="index">覆盖的起始位置。</param>
        /// <param name="count">返回的指定长度。</param>
        /// <returns>实际返回的长度。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetArray"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引 <paramref name="index"/> 超出范围。</exception>
        public virtual int Peek(TNode[] targetArray, int index, int count)
        {
            if (targetArray == null) throw new ArgumentNullException(nameof(targetArray));
            if (index < 0 || index >= targetArray.Length) throw new ArgumentOutOfRangeException(nameof(index), index, "索引超出范围。");

            var content = this.Peek(count);
            content.CopyTo(targetArray, index);

            return content.Count;
        }

        private IEnumerable<TNode> PeekInternal(int count)
        {
            for (int i = 0; i < count && (i + count) < this.buffer.Count; i++)
                yield return this.buffer[this.Position + i];
        }
        #endregion

        #region Read
        /// <summary>
        /// 读取下一个节点，并将指针位置提升一位。
        /// </summary>
        /// <returns>下一个节点。</returns>
        /// <exception cref="InvalidOperationException">内部读取器已抵达结尾。</exception>
        public virtual TNode Read()
        {
            if (this.Position == this.buffer.Count) this._ReadBuffer(10);

            if (this.Position == this.buffer.Count) throw new InvalidOperationException("内部读取器已抵达结尾。");
            else return this.ReadInternal();
        }

        /// <summary>
        /// 读取当前指针位置后指定长度节点，并将指针位置提升指定长度。
        /// </summary>
        /// <param name="count">读取的指定长度。</param>
        /// <returns>实际读取的长度。</returns>
        public virtual IList<TNode> Read(int count)
        {
            if (this.Position + count >= this.buffer.Count) this._ReadBuffer(count);

            return new List<TNode>(this.ReadInternal(System.Math.Min(this.buffer.Count - this.Position, count)));
        }

        /// <summary>
        /// 读取当前指针位置后指定长度节点，存入指定数组中，并将指针位置提升指定长度。
        /// </summary>
        /// <param name="targetArray">承载读取结果的数组。</param>
        /// <param name="index">覆盖的起始位置。</param>
        /// <param name="count">读取的指定长度。</param>
        /// <returns>实际读取的长度。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetArray"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引 <paramref name="index"/> 超出范围。</exception>
        public virtual int Read(TNode[] targetArray, int index, int count)
        {
            if (targetArray == null) throw new ArgumentNullException(nameof(targetArray));
            if (index < 0 || index >= targetArray.Length) throw new ArgumentOutOfRangeException(nameof(index), index, "索引超出范围。");

            var content = this.Read(count);
            content.CopyTo(targetArray, index);

            return content.Count;
        }

        private TNode ReadInternal()
        {
            return this.buffer[this.Position++];
        }

        private IEnumerable<TNode> ReadInternal(int count)
        {
            for (int i = 0; i < count && (i + count) < this.buffer.Count; i++)
                yield return this.ReadInternal();
        }
        #endregion

        /// <summary>
        /// 将指针回退指定长度个位置。
        /// </summary>
        /// <param name="count">指定的回退长度。</param>
        /// <returns>实际回退后的指针位置。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0 。</exception>
        public virtual int Rollback(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), count, "回退长度应大于等于 0 。");

            this.Position = System.Math.Max(0, this.Position - count);
            return this.Position;
        }

        /// <summary>
        /// 返回一个结果指示内部读取器是否已经读到末尾。为 true 则表示内部读取器是否已经读到末尾；否则为 false 。
        /// </summary>
        /// <returns>为 true 则表示内部读取器是否已经读到末尾；否则为 false 。</returns>
        public virtual bool IsEnd()
        {
            return (this.Position >= this.buffer.Count) && this.eofFunc(this.Reader);
        }

        bool IReaderSource<TNode>.HasNext() { return !this.IsEnd(); }
    }

    /// <summary>
    /// 定义一系列创建、使用<see cref="NodeReader{TReader, TNode}"/>的方法。
    /// </summary>
    public static class NodeReader
    {
        /// <summary>
        /// 使用指定的字符串创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <param name="str">指定的字符串。</param>
        /// <returns>使用指定的字符串创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 的值为 null 。</exception>
        public static NodeReader<string, char> CreateReader(this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            return new NodeReader<string, char>(str.ToCharArray());
        }

        /// <summary>
        /// 使用指定的内部读取器、读取方法和判断读取结束的方法创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TReader">内部读取器类型。</typeparam>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="reader">指定的内部读取器。</param>
        /// <param name="readFunc">指定的读取方法。</param>
        /// <param name="eofFunc">指定的判断读取结束的方法。</param>
        /// <returns>使用指定的内部读取器、读取方法和判断读取结束的方法创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="readFunc"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eofFunc"/> 的值为 null 。</exception>
        public static NodeReader<TReader, TNode> CreateReader<TReader, TNode>(this TReader reader, Func<TReader, TNode> readFunc, Func<TReader, bool> eofFunc)
        {
            if (readFunc == null) throw new ArgumentNullException(nameof(readFunc));
            if (eofFunc == null) throw new ArgumentNullException(nameof(eofFunc));

            return new NodeReader<TReader, TNode>(reader, readFunc, eofFunc);
        }

        /// <summary>
        /// 使用指定的内部读取器创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TReader">内部读取器类型。</typeparam>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="reader">指定的内部读取器。</param>
        /// <returns>使用指定的内部读取器创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> 的值为 null 。</exception>
        public static NodeReader<TReader, TNode> CreateReader<TReader, TNode>(this TReader reader)
            where TReader : IReaderSource<TNode>
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return new NodeReader<TReader, TNode>(reader, _reader => _reader.Read(), _reader => !_reader.HasNext());
        }

        /// <summary>
        /// 使用指定的内部读取器创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="reader">指定的内部读取器。</param>
        /// <returns>使用指定的内部读取器创建的 <see cref="NodeReader{TReader, TNode}"/>对象。 </returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> 的值为 null 。</exception>
        public static NodeReader<IReaderSource<TNode>, TNode> CreateReader<TNode>(this IReaderSource<TNode> reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            return new NodeReader<IReaderSource<TNode>, TNode>(reader, _reader => _reader.Read(), _reader => !_reader.HasNext());
        }

        /// <summary>
        /// 使用指定的节点数组创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="nodes">指定的节点数组。</param>
        /// <returns>使用指定的节点数组创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="nodes"/> 的值为 null 。</exception>
        public static NodeReader<TNode[], TNode> CreateReader<TNode>(params TNode[] nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            return new NodeArrayReader<TNode>(nodes);
        }

        /// <summary>
        /// 使用指定的节点序列创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="nodes">指定的节点序列。</param>
        /// <returns>使用指定的节点序列创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="nodes"/> 的值为 null 。</exception>
        public static NodeReader<IEnumerable<TNode>, TNode> CreateReader<TNode>(this IEnumerable<TNode> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            return new NodeReader<IEnumerable<TNode>, TNode>(nodes);
        }

        /// <summary>
        /// 使用指定的节点枚举器创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="enumerator">指定的节点枚举器。</param>
        /// <returns>使用指定的节点枚举器创建的 <see cref="NodeReader{TReader, TNode}"/>对象。 </returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> 的值为 null 。</exception>
        public static NodeReader<IEnumerator<TNode>, TNode> CreateReader<TNode>(this IEnumerator<TNode> enumerator)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));

            return new NodeReader<IEnumerator<TNode>, TNode>(enumerator);
        }

        /// <summary>
        /// 使用指定的内部读取器、反射获取的读取方法和指定的判断读取结束的方法创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TReader">内部读取器类型。</typeparam>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="reader">指定的内部读取器。</param>
        /// <param name="eofFunc">指定的判断读取结束的方法。</param>
        /// <returns>使用指定的内部读取器、反射获取的读取方法和指定的判断读取结束的方法创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eofFunc"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TReader"/> 不存在无参数的 Read 方法。</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TReader"/> 的无参数的 Read 方法的返回参数不能转型为 <typeparamref name="TNode"/> 。</exception>
        public static NodeReader<TReader, TNode> CreateReader<TReader, TNode>(this TReader reader, Func<TReader, bool> eofFunc)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (eofFunc == null) throw new ArgumentNullException(nameof(eofFunc));

            System.Reflection.MethodInfo mi = typeof(TReader).GetMethod("Read", Type.EmptyTypes);
            if (mi == null) throw new InvalidOperationException("不存在合法的 Read 方法。");
            if (!typeof(TNode).IsAssignableFrom(mi.ReturnType)) throw new InvalidOperationException(" Read 方法返回参数不合法。");

            return NodeReader.CreateReader(reader, _reader => (TNode)mi.Invoke(mi, null), eofFunc);
        }

        /// <summary>
        /// 使用指定的内部读取器、反射获取的读取方法和指定的判断读取结束的方法创建 <see cref="NodeReader{TReader, TNode}"/> 对象。
        /// </summary>
        /// <typeparam name="TReader">内部读取器类型。</typeparam>
        /// <typeparam name="TNode">节点的类型。</typeparam>
        /// <param name="reader">指定的内部读取器。</param>
        /// <param name="_node_">指定 <typeparamref name="TNode"/> 的节点实例。</param>
        /// <param name="eofFunc">指定的判断读取结束的方法。</param>
        /// <returns>使用指定的内部读取器、反射获取的读取方法和指定的判断读取结束的方法创建的 <see cref="NodeReader{TReader, TNode}"/> 对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eofFunc"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TReader"/> 不存在无参数的 Read 方法。</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TReader"/> 的无参数的 Read 方法的返回参数不能转型为 <typeparamref name="TNode"/> 。</exception>
        public static NodeReader<TReader, TNode> CreateReader<TReader, TNode>(this TReader reader, TNode _node_, Func<TReader, bool> eofFunc)
        {
            return NodeReader.CreateReader<TReader, TNode>(reader, eofFunc);
        }
    }
}
