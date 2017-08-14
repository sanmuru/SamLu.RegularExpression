using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class RangeInvalidOperationException<T> : InvalidOperationException
    {
        private IRange<T>[] ranges;
        public IRange<T>[] Ranges
        {
            get
            {
                IRange<T>[] ranges = new IRange<T>[this.ranges.Length];
                this.ranges.CopyTo(ranges, this.ranges.Length);

                return ranges;
            }
        }

        public RangeInvalidOperationException() : this(new IRange<T>[0]) { }

        public RangeInvalidOperationException(IRange<T>[] ranges) : this("对范围的无效操作。", ranges) { }

        public RangeInvalidOperationException(string message) : this(message, new IRange<T>[0]) { }

        public RangeInvalidOperationException(string message, IRange<T>[] ranges) : base(message) =>
            this.ranges = ranges ?? throw new ArgumentNullException(nameof(ranges));

        public RangeInvalidOperationException(string message, Exception innerException) : this(message, new IRange<T>[0], innerException) { }

        public RangeInvalidOperationException(string message, IRange<T>[] ranges, Exception innerException) : base(message, innerException) =>
            this.ranges = ranges ?? throw new ArgumentNullException(nameof(ranges));
    }
}
