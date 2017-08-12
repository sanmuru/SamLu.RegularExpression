using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class RangeNotOverlapException : Exception
    {
        public RangeNotOverlapException() : base() { }

        public RangeNotOverlapException(string message) : base(message) { }

        public RangeNotOverlapException(string message, Exception innerException) : base(message, innerException) { }
    }
}
