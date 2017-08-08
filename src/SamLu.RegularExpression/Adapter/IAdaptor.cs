using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public interface IAdaptor<TSource, TTarget>
    {
        AdaptContextInfo<TSource, TTarget> ContextInfo { get; }
    }
}
