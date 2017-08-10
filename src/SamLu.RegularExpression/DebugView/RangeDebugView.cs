using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.DebugView
{
    internal class RangeDebugView<T>
    {
        private RegexRange<T> regex;

        //public string Value => $"{(this.regex.CanTakeMinimum ? '[' : '(')}{this.regex.Minimum},{this.regex.Maximum}{(this.regex.CanTakeMaximum ? ']' : ')')}";

        public T Minimum => this.regex.Minimum;
        public T Maximum => this.regex.Maximum;

        public bool CanTakeMinimum => this.regex.CanTakeMinimum;
        public bool CanTakeMaximum => this.regex.CanTakeMaximum;

        public RangeDebugView(RegexRange<T> regex)
        {
            this.regex = regex;
        }
    }
}
