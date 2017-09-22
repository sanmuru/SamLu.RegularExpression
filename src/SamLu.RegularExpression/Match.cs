using SamLu.RegularExpression.Extend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 表示单个正则表达式匹配的结果。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class Match<T> : Group<T>
    {
        /// <summary>
        /// 获取空组。 所有失败的匹配都返回此空匹配。
        /// </summary>
        public static Match<T> Empty =>
            new Match<T>(Enumerable.Empty<T>(), 0, 0, Enumerable.Empty<Group<T>>());

        protected IList<Group<T>> groups;

        public GroupCollection<T> Groups => new GroupCollection<T>(this.groups);
        
        public override bool Success => this.groups.Any(group => group.Success);

        protected Match() : base() { }

        protected internal Match(IEnumerable<T> input, int index, int length, IEnumerable<Group<T>> groups) : base()
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (groups == null) throw new ArgumentNullException(nameof(groups));

            base.index = index;
            base.length = length;
            base.input = input;

            base.captures = new List<Capture<T>> { new Capture<T>(input, index, length) };
            this.groups = new List<Group<T>>(new[] { new Group<T>(input, new[] { (index, length) }) }.Concat(groups));
        }
    }
}
