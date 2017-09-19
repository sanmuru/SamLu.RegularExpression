using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    /// <summary>
    /// 标记正则表达式构造的有限状态机的功能转换的元数据。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegexFunctionalTransitionMetadataAttribute : Attribute
    {
        private object[] index;
        /// <summary>
        /// 获取修饰索引器的 <see cref="RegexFunctionalTransitionMetadataAttribute"/> 指定的索引。
        /// </summary>
        public object[] Index => this.index;

        /// <summary>
        /// 获取 <see cref="RegexFunctionalTransitionMetadataAttribute"/> 修饰的属性表示的元数据的别名。
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 初始化 <see cref="RegexFunctionalTransitionMetadataAttribute"/> 的新实例，该实例指定修饰索引器的 <see cref="RegexFunctionalTransitionMetadataAttribute"/> 的索引。
        /// </summary>
        /// <param name="index">修饰索引器的 <see cref="RegexFunctionalTransitionMetadataAttribute"/> 指定的索引。</param>
        public RegexFunctionalTransitionMetadataAttribute(params object[] index) => this.index = index;
    }
}
