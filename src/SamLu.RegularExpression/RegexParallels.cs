using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示并联正则。按顺序测试是否匹配内部正则对象列表的某一项。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexParallels<T> : RegexObject<T>
    {
        /// <summary>
        /// 并联正则内部的正则对象列表。
        /// </summary>
        protected List<RegexObject<T>> parallels;
        /// <summary>
        /// 获取并联正则内部的正则对象列表。
        /// </summary>
        /// <value>
        /// 并联正则内部的正则对象列表。
        /// <para>列表是只读的。</para>
        /// </value>
        public IList<RegexObject<T>> Parallels => new ReadOnlyCollection<RegexObject<T>>(this.parallels);

        /// <summary>
        /// 初始化 <see cref="RegexParallels{T}"/> 类的新实例。子类继承的默认构造函数。
        /// </summary>
        protected RegexParallels() { }

        /// <summary>
        /// 初始化 <see cref="RegexParallels{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="parallels">要作为内部正则对象列表的参数数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>s
        public RegexParallels(params RegexObject<T>[] parallels) :
            this(parallels?.AsEnumerable() ?? throw new ArgumentNullException(nameof(parallels)))
        { }

        /// <summary>
        /// 初始化 <see cref="RegexParallels{T}"/> 类的新实例。该实例指定内部的正则对象列表。
        /// </summary>
        /// <param name="parallels">指定的内部正则对象列表。</param>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>
        public RegexParallels(IEnumerable<RegexObject<T>> parallels)
        {
            if (parallels == null) throw new ArgumentNullException(nameof(parallels));

            this.parallels = new List<RegexObject<T>>(parallels.Where(item => item != null));
        }

        /// <summary>
        /// 将此正则对象与另一个正则对象并联。
        /// </summary>
        /// <param name="regex">另一个正则对象。</param>
        /// <returns>并联后形成的新正则对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        /// <seealso cref="RegexObject{T}.Unions(RegexObject{T})"/>
        public override RegexObject<T> Unions(RegexObject<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (regex is RegexParallels<T> parallels)
                return new RegexParallels<T>(this.parallels.Concat(parallels.parallels));
            else
                return new RegexParallels<T>(this, regex);
        }

        protected internal override RegexObject<T> Clone()
        {
            return new RegexParallels<T>(this.parallels);
        }

        public override string ToString()
        {
            return $"({string.Join("|", this.parallels)})";
        }
    }
}