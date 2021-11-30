using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 提供操作正则对象的方法的工厂类。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public abstract class RegexProvider<T>
    {
        private class CreatorInterfaceComparer : Comparer<(Type returnType, Type parameterType)>
        {
            public override int Compare((Type returnType, Type parameterType) x, (Type returnType, Type parameterType) y)
            {
                if (x.returnType.IsAssignableTo(y.returnType)) return -1; // 返回类型在继承链中越低位，则离构造器目标类型越远。
                else if (x.returnType.IsAssignableFrom(y.returnType)) return 1; // 返回类型在继承链中越高位，则离构造器目标类型越近。
                else if (x.parameterType.IsAssignableFrom(y.parameterType)) return -1; // 第一个参数的类型在继承链中越高位，则离构造器目标类型越远。
                else if (x.parameterType.IsAssignableTo(y.parameterType)) return 1; // 第一个参数的类型在继承链中越低位，则离构造器目标类型近。
                else return 0; // 双方的返回类型和第一个参数的类型均不构成继承关系的，视为相等。
            }
        }

        /// <summary>
        /// 创建正则类型 <typeparamref name="TObject"/> 的新实例。
        /// </summary>
        /// <typeparam name="TObject">要创建的正则类型。</typeparam>
        /// <param name="original">参照的母本。</param>
        /// <param name="args">创建新实例所需的参数。</param>
        /// <returns>正则类型 <typeparamref name="TObject"/> 的新实例。</returns>
        public virtual TObject CreateRegexObject<TObject>(TObject? original, params object?[] args) where TObject : RegexObject<T> => this.CreateRegexObjectWithReflection(original, args);

        /// <summary>
        /// 使用反射技术调用正则类型 <typeparamref name="TObject"/> 的构造器并创建新实例。
        /// </summary>
        /// <typeparam name="TObject">要创建的正则类型。</typeparam>
        /// <param name="original">参照的母本。</param>
        /// <param name="args">创建新实例所需的参数。</param>
        /// <returns>正则类型 <typeparamref name="TObject"/> 的新实例。</returns>
        /// <exception cref="CreatorNotSupportedException">不存在构造类型 <typeparamref name="TObject"/> 的构造器。</exception>
        protected internal TObject CreateRegexObjectWithReflection<TObject>(TObject? original, params object?[] args)
            where TObject : RegexObject<T>
        {
            var lookup = (
                from mi in this.GetType().GetMethods()
                where mi.Name.StartsWith("Create")
                where !mi.IsGenericMethod
                where mi.ReturnType is not null && mi.ReturnType.IsAssignableTo(typeof(TObject))
                where mi.Name == "Create" + mi.ReturnType.Name
                let pis = mi.GetParameters()
                where pis.Length == 2
                let firstParam = pis[0]
                let secondParam = pis[1]
                where firstParam.ParameterType.IsAssignableFrom(typeof(TObject))
                where secondParam.ParameterType == typeof(object[])
                
                select mi
            ).ToLookup(mi => mi.ReturnType);

            for (Type type = typeof(TObject); type != typeof(RegexObject<T>); type = type.BaseType!)
            {
                if (!lookup.Contains(type)) continue;

                var creators = lookup[type].OrderBy(
                    mi => (mi.ReturnType, mi.GetParameters()[0].ParameterType),
                    new CreatorInterfaceComparer()
                );
                foreach (var creator in creators)
                {
                    var result = creator.Invoke(this, new object?[] { original, args }) as TObject;
                    if (result is not null)
                        return result;
                }
            }

            throw new CreatorNotSupportedException(typeof(TObject));
        }
    }
}
