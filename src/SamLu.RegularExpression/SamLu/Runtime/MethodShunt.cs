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

        public static MethodShuntResult DynamicInvokeShunt(this object target, MethodShuntSource source, params object[] parameters)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (MethodShunt.shuntDic.ContainsKey(source))
            {
                foreach (var key in MethodShunt.shuntDic[source])
                {
                    if (MethodShunt.DynamicInvokeShuntInternal(target, key, parameters, out object returnValue))
                        return new MethodShuntResult(true, returnValue);
                }
            }

            return MethodShuntResult.Unsuccess;
        }

        [Obsolete]
        private static bool DynamicInvokeShuntInternal(object target, MethodShuntKey key, object[] parameters, out object returnValue)
        {
            var pairs =
                from pi in key.Method.GetParameters()
                join pv in parameters.Select((pv, index) => new { Value = pv, Index = index })
                on pi.Position equals pv.Index
                select new { Parameter = pi, Value = pv.Value }
                into pair
                orderby pair.Parameter.Position
                select pair;

            bool success = pairs.All(pair =>
                pair.Parameter.ParameterType.IsAssignableFromValue(pair.Value)
            );

            if (success)
            {
                returnValue = key.Method.Invoke(target, parameters);
                return true;
            }
            else
            {
                returnValue = null;
                return false;
            }
        }
        
        internal static bool IsAssignableFromValue(this Type type, object value)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            if (value == null)
                return !type.IsValueType;
            else
                return type.IsAssignableFrom(value.GetType());
        }
    }
}
