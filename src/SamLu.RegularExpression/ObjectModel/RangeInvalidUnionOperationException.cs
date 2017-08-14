using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class RangeInvalidUnionOperationException<T> : RangeInvalidOperationException<T>
    {
        public RangeInvalidUnionOperationException() : this(new IRange<T>[0]) { }

        public RangeInvalidUnionOperationException(params IRange<T>[] ranges) : this("对范围的无效并集操作。", ranges) { }

        public RangeInvalidUnionOperationException(string message) : base(message) { }

        public RangeInvalidUnionOperationException(string message, params IRange<T>[] ranges) : base(message, ranges) { }

        public RangeInvalidUnionOperationException(string message, Exception innerException) : base(message, innerException) { }

        public RangeInvalidUnionOperationException(string message, IRange<T>[] ranges, Exception innerException) : base(message, ranges, innerException) { }
    }
}
