using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class Group<T> : Capture<T>
    {
        protected IList<Capture<T>> captures;

        public virtual ICollection<Capture<T>> Captures => new ReadOnlyCollection<Capture<T>>(this.captures);
        public virtual bool Success => this.captures?.Count > 0;

        protected Group() : base() { }

        protected internal Group(IEnumerable<T> input, (int index, int length)[] captures)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (captures == null) throw new ArgumentNullException(nameof(captures));

            base.input = input;
            if (captures.Any())
            {
                this.captures = new List<Capture<T>>(captures.Select(capture => new Capture<T>(input, capture.index, capture.length)));

                var last = captures.Last();
                base.index = last.index;
                base.length = last.length;
            }
        }
    }
}
