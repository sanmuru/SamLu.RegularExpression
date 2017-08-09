using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    public interface IRange<T>
    {
        T Minimum { get; }
        T Maximum { get; }

        bool CanTakeMinimum { get; }
        bool CanTakeMaximum { get; }

        Comparison<T> Comparison { get; }
    }
}
