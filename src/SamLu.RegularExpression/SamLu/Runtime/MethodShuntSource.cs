using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.Runtime
{
    public class MethodShuntSource
    {
        public MethodInfo Method { get; private set; }

        internal MethodShuntSource(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            this.Method = method;
        }
    }
}
