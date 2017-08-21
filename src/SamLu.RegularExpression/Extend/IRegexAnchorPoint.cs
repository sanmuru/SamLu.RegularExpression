using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 定位点或原子零宽度断言会使匹配成功或失败，具体取决于匹配序列中的当前位置，但它们不会使引擎在序列中前进或使用项。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public interface IRegexAnchorPoint<T>
    {
    }
}
