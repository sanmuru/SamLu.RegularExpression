using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    public class RegexOptions
    {
        public static RegexOptions None => new RegexOptions();

        [OptionBitOffset(0)]
        public bool ExplicitCapture { get; protected set; }
        [OptionBitOffset(1)]
        public bool Compiled { get; protected set; }
        [OptionBitOffset(2)]
        public bool RightToLeft { get; protected set; }

        protected RegexOptions() : this(false, false, false) { }

        public RegexOptions(bool explicitCapture, bool compiled, bool rightToLeft)
        {
            this.ExplicitCapture = explicitCapture;
            this.Compiled = compiled;
            this.RightToLeft = rightToLeft;
        }
    }
}
