using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.IO
{
    /// <summary>
    /// 表示以节点数组作为读取源的节点读取器。
    /// </summary>
	/// <typeparam name="TNode">节点的类型。</typeparam>
    public class NodeArrayReader<TNode> : NodeReader<TNode[], TNode>
    {
        /// <summary>
        /// 使用指定的节点数组初始化 <see cref="NodeArrayReader{TNode}"/> 类的新实例。
        /// </summary>
        /// <param name="array">指定的节点数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 的值为 null 。</exception>
        public NodeArrayReader(TNode[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            this.Reader = array;
            base.buffer.AddRange(array);
            
            base.readFunc = reader => default(TNode);
            base.eofFunc = reader => true;
        }
    }
}
