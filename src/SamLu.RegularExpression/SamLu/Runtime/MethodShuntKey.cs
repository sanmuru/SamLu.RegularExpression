using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.Runtime
{
    public sealed class MethodShuntKey : MethodShuntSource
    {
        public MethodShuntSource Source { get; private set; }

        internal MethodShuntKey(MethodShuntSource source, MethodInfo method) : base(method)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            this.Source = source;
        }
    }
}
