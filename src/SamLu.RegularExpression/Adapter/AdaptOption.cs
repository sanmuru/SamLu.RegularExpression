using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    /// <summary>
    /// 定义了一组适配选项。
    /// </summary>
    public enum AdaptOption
    {
        /// <summary>
        /// 只在初始化时调用源适配器对常量源进行适配。
        /// </summary>
        OnlyAdaptConstSourceWhenInitialization = 0x01
    }
}
