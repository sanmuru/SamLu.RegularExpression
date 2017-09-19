using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    /// <summary>
    /// 表示正则构造的有限状态机的功能转换。所有提供特殊行为支持的正则构造的有限状态机的转换应继承此类，或自行实现 <see cref="IRegexFunctionalTransition{T}"/> 接口。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public abstract class RegexFunctionalTransition<T> : FSMTransition, IRegexFunctionalTransition<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFunctionalTransition{T}"/> 的用户数据字典。
        /// </summary>
        [RegexFunctionalTransitionMetadata]
        public IDictionary<object, object> UserData { get; } = new Dictionary<object, object>();

        /// <summary>
        /// 获取 <see cref="RegexFunctionalTransition{T}"/> 指向的状态。
        /// </summary>
        new public virtual IRegexFSMState<T> Target => (IRegexFSMState<T>)base.Target;

        #region SetTarget
        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public sealed override bool SetTarget(IState state) => base.SetTarget(state);

        /// <summary>
        /// 将转换的目标设为指定状态。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> 的值为 null 。</exception>
        public virtual bool SetTarget(IRegexFSMState<T> state) => base.SetTarget(state);
        #endregion
    }

    /// <summary>
    /// 表示正则构造的有限状态机的功能转换。所有提供特殊行为支持的正则构造的有限状态机的转换应继承此类，或自行实现 <see cref="IRegexFunctionalTransition{T, TState}"/> 接口。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    public abstract class RegexFunctionalTransition<T, TState> : FSMTransition<TState>, IRegexFunctionalTransition<T, TState>
        where TState : IRegexFSMState<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFunctionalTransition{T, TState}"/> 的用户数据字典。
        /// </summary>
        [RegexFunctionalTransitionMetadata]
        public IDictionary<object, object> UserData { get; } = new Dictionary<object, object>();

        #region IRegexFSMTransition{T} Implementation
        IRegexFSMState<T> IRegexFSMTransition<T>.Target => this.Target;

        bool IRegexFSMTransition<T>.SetTarget(IRegexFSMState<T> state) =>
            base.SetTarget((TState)state);
        #endregion
    }

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
