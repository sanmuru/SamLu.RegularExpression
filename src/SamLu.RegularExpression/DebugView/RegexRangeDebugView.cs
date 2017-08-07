using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.DebugView
{
    internal class RegexRangeDebugView<T>
    {
        private RegexRange<T> regex;

        //public string Value => $"{(this.regex.CanTakeMinimum ? '[' : '(')}{this.regex.Minimum},{this.regex.Maximum}{(this.regex.CanTakeMaximum ? ']' : ')')}";

        public T Minimum => this.regex.Minimum;
        public T Maximum => this.regex.Maximum;

        public bool CanTakeMinimum => this.regex.CanTakeMinimum;
        public bool CanTakeMaximum => this.regex.CanTakeMaximum;

        public RegexRangeDebugView(RegexRange<T> regex)
        {
            this.regex = regex;
        }
    }
}
