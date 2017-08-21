using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptionBitOffsetAttribute : Attribute
    {
        private int offset;

        public int Offset => this.offset;

        public OptionBitOffsetAttribute(int offset)
        {
            this.offset = offset;
        }
    }
}
