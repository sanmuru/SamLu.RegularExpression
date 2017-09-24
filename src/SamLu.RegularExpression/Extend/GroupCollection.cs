using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class GroupCollection<T> : IReadOnlyCollection<Group<T>>
    {
        private Match<T> match;
        private IList<(object id, Group<T> group)> groups;

        public int Count => this.groups.Count;

        public Group<T> this[int index] => this.groups.ElementAt(index).group;

        public IReadOnlyCollection<Group<T>> this[object id] =>
            new ReadOnlyCollection<Group<T>>(
                this.groups
                    .Where(item=>object.Equals(id,item.id))
                    .Select(item=>item.group)
                .ToList()
            );
        
        protected internal GroupCollection(Match<T> match, IEnumerable<(object id, Group<T> group)> collection)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            this.match = match;
            this.groups = new List<(object, Group<T>)>(collection);
        }

        public IEnumerator<Group<T>> GetEnumerator() =>
            this.groups.Select(item => item.group).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
