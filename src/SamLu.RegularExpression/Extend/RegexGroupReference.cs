using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 表示正则组的引用。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexGroupReference<T> : RegexObject<T>
    {
        private object id;
        private RegexGroup<T> group;

        /// <summary>
        /// 获取此正则组引用指向的 <see cref="RegexGroup{T}"/> 的 ID 。
        /// </summary>
        public object GroupID => this.id;

        /// <summary>
        /// 获取此正则组引用指向的 <see cref="RegexGroup{T}"/> 。
        /// </summary>
        public RegexGroup<T> Group
        {
            get
            {
                if (this.IsDetermined) return this.group;
                else
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 获取一个值，指示此正则组引用是否已经确定。
        /// </summary>
        public bool IsDetermined => this.group != null;

        protected RegexGroupReference() : this(null) { }

        public RegexGroupReference(object id)=>
            this.id = id ?? throw new ArgumentNullException(nameof(id));

        public RegexGroupReference(RegexGroup<T> group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            this.group = group;
            this.id = group.ID;
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexGroupReference<T>(this.id);
        }
    }
}
