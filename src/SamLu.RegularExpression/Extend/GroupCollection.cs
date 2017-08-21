using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class GroupCollection<T> : ICollection<Group<T>>
    {
        private ICollection<Group<T>> groups;

        public int Count => this.groups.Count;

        public Group<T> this[int index] => this.groups.ElementAt(index);

        public Group<T> this[object id] => throw new NotImplementedException();

        public void Add(Group<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.groups.Add(item);
        }

        public void Clear() => this.groups.Clear();

        public bool Contains(Group<T> item) => this.groups.Contains(item);

        public void CopyTo(Group<T>[] array, int arrayIndex) => this.groups.CopyTo(array, arrayIndex);

        public IEnumerator<Group<T>> GetEnumerator() => this.groups.GetEnumerator();

        public bool Remove(Group<T> item) => this.groups.Remove(item);

        bool ICollection<Group<T>>.IsReadOnly => this.groups.IsReadOnly;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
