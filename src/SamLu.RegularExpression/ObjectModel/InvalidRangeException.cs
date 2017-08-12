using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class InvalidRangeException<T> : Exception
    {
        private (T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison) range;

        public InvalidRangeException() : base() { }

        public InvalidRangeException(string message) : base(message) { }

        public InvalidRangeException(T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison) :
            this("无效的范围。", minimum, maximum, canTakeMinimum, canTakeMaximum, comparison, null)
        { }

        public InvalidRangeException(string message, Exception innerException) : base(message, innerException) { }

        public InvalidRangeException(string message, T minimum, T maximum, bool canTakeMinimum, bool canTakeMaximum, Comparison<T> comparison, Exception innerException) : this(message, innerException)
        {
            this.range = (minimum, maximum, canTakeMinimum, canTakeMaximum, comparison);
        }
    }
}
