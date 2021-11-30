using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 当不存在构造某一类型的构造器时抛出异常。
    /// </summary>
    public class CreatorNotSupportedException : NotSupportedException
    {
        /// <summary>
        /// 使用指定构造器类型实例化 <see cref="CreatorNotSupportedException"/> 对象。
        /// </summary>
        /// <param name="creatorType">指定构造器类型。</param>
        public CreatorNotSupportedException(Type? creatorType = null) : base(creatorType switch
        {
            null => "不存在构造此类型的构造器。",
            _ => $"不存在构造类型 '{creatorType.FullName}' 的构造器。"
        })
        { }
    }
}
