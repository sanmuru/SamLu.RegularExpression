using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class RangeInvalidExceptOperationException<T> : RangeInvalidOperationException<T>
    {
        public RangeInvalidExceptOperationException() : this(new IRange<T>[0]) { }

        public RangeInvalidExceptOperationException(params IRange<T>[] ranges) : this("对范围的无效差集操作。", ranges) { }

        public RangeInvalidExceptOperationException(string message) : base(message) { }

        public RangeInvalidExceptOperationException(string message, params IRange<T>[] ranges) : base(message, ranges) { }

        public RangeInvalidExceptOperationException(string message, Exception innerException) : base(message, innerException) { }

        public RangeInvalidExceptOperationException(string message, IRange<T>[] ranges, Exception innerException) : base(message, ranges, innerException) { }
    }
}
