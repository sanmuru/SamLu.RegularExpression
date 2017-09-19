using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    public static class RegexFunctionalTransition
    {
        public static Dictionary<string, object> GetMetadata<T>(this IRegexFunctionalTransition<T> functionalTransition)
        {
            if (functionalTransition == null) throw new ArgumentNullException(nameof(functionalTransition));

            Dictionary<string, object> metadata = new Dictionary<string, object>();

            Type type = functionalTransition.GetType();
            // 获取字段。
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                if (fieldInfo.GetCustomAttribute<RegexFunctionalTransitionMetadataAttribute>() is RegexFunctionalTransitionMetadataAttribute attribute)
                {
                    metadata.Add(
                        attribute.Alias ?? fieldInfo.Name,
                        fieldInfo.GetValue(fieldInfo.IsStatic ? null : functionalTransition)
                    );
                }
            // 获取实例属性。
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                if (propertyInfo.GetCustomAttribute<RegexFunctionalTransitionMetadataAttribute>() is RegexFunctionalTransitionMetadataAttribute attribute)
                {
                    metadata.Add(
                        attribute.Alias ?? propertyInfo.Name,
                        propertyInfo.GetValue(functionalTransition, attribute.Index)
                    );
                }
            // 获取静态属性。
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                if (propertyInfo.GetCustomAttribute<RegexFunctionalTransitionMetadataAttribute>() is RegexFunctionalTransitionMetadataAttribute attribute)
                {
                    metadata.Add(
                        attribute.Alias ?? propertyInfo.Name,
                        propertyInfo.GetValue(null, attribute.Index)
                    );
                }

            return metadata;
        }
    }
}
