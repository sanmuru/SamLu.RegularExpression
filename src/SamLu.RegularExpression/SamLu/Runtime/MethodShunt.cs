using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.Runtime
{
    public static class MethodShunt
    {
        private static readonly Dictionary<MethodShuntSource, HashSet<MethodShuntKey>> shuntDic = new Dictionary<MethodShuntSource, HashSet<MethodShuntKey>>();

        public static MethodShuntSource CreateSource(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var source = new MethodShuntSource(method);
            MethodShunt.shuntDic.Add(source, new HashSet<MethodShuntKey>());

            return source;
        }

        public static MethodShuntKey Register(MethodShuntSource source, MethodInfo method)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (method == null) throw new ArgumentNullException(nameof(method));

            var key = new MethodShuntKey(source, method);
            MethodShunt.shuntDic[source].Add(key);

            return key;
        }

        public static object DynamicInvokeShunt(this object target, MethodShuntSource source, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
