using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.Runtime
{
    public class MethodShuntResult
    {
        public static MethodShuntResult Unsuccess => new MethodShuntResult(false, null);

        public bool Success { get; private set; }

        public object ReturnValue { get; private set; }

        internal MethodShuntResult(bool success, object returnValue)
        {
            this.Success = success;
            this.ReturnValue = returnValue;
        }
    }
}
