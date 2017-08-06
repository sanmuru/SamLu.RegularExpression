using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了 NFA 转换到 DFA 的上下文信息应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">自动机接受处理的对象的类型。</typeparam>
    public interface INFAtoDFAContextInfo<T>
    {
        /// <summary>
        /// 可接受的对象集。
        /// </summary>
        ISet<T> AccreditedSet { get; }
    }
}
