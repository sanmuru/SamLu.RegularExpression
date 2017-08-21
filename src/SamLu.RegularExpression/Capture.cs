using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    public class Capture<T>
    {
        protected int index;
        protected int length;

        protected IEnumerable<T> input;

        public int Index => this.index;
        public int Length => this.length;

        public virtual IEnumerable<T> Value => this.input.Skip(this.index).Take(this.length);

        protected Capture() { }

        protected internal Capture(IEnumerable<T> input, int index, int length) : this()
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            this.index = index;
            this.length = length;
            this.input = input;
        }

        protected internal IEnumerable<T> GetOriginalNodes() => this.input;

        protected internal IEnumerable<T> GetBeforeNodes() => this.input.Take(index);

        protected internal IEnumerable<T> GetAfterNodes() => this.input.Skip(this.index + this.length);
    }
}
