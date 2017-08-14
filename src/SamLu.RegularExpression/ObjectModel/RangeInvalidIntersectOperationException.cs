using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class RangeInvalidIntersectOperationException<T> : RangeInvalidOperationException<T>
    {
        public RangeInvalidIntersectOperationException() : this(new IRange<T>[0]) { }

        public RangeInvalidIntersectOperationException(params IRange<T>[] ranges) : this("对范围的无效交集操作。", ranges) { }
        
        public RangeInvalidIntersectOperationException(string message) : base(message) { }

        public RangeInvalidIntersectOperationException(string message, params IRange<T>[] ranges) : base(message, ranges) { }

        public RangeInvalidIntersectOperationException(string message, Exception innerException) : base(message, innerException) { }

        public RangeInvalidIntersectOperationException(string message, IRange<T>[] ranges, Exception innerException) : base(message, ranges, innerException) { }
    }
}
