using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.DebugView
{
    internal class CollectionDebugView<T>
    {
        private ICollection<T> collection;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.collection.Count];
                this.collection.CopyTo(array, 0);
                return array;
            }
        }

        public CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            this.collection = collection;
        }
    }
}
