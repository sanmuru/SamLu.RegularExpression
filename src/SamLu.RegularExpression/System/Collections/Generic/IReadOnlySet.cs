using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    [DefaultMember("Item")]
    public interface IReadOnlySet<T> : IReadOnlyCollection<T>
    {
        //
        // 摘要:
        //     确定当前集是否为指定集合的真（严格）子集。
        //
        // 参数:
        //   other:
        //     要与当前集进行比较的集合。
        //
        // 返回结果:
        //     如果当前集是 other 的真子集，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     other 为 null。
        bool IsProperSubsetOf(IEnumerable<T> other);
        //
        // 摘要:
        //     确定当前集是否为指定集合的真（严格）超集。
        //
        // 参数:
        //   other:
        //     要与当前集进行比较的集合。
        //
        // 返回结果:
        //     如果当前集是 other 的真超集，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     other 为 null。
        bool IsProperSupersetOf(IEnumerable<T> other);
        //
        // 摘要:
        //     确定一个集是否为指定集合的子集。
        //
        // 参数:
        //   other:
        //     要与当前集进行比较的集合。
        //
        // 返回结果:
        //     如果当前集是 other 的子集，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     other 为 null。
        bool IsSubsetOf(IEnumerable<T> other);
        //
        // 摘要:
        //     确定当前集是否为指定集合的超集。
        //
        // 参数:
        //   other:
        //     要与当前集进行比较的集合。
        //
        // 返回结果:
        //     如果当前集是 other 的超集，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     other 为 null。
        bool IsSupersetOf(IEnumerable<T> other);
        //
        // 摘要:
        //     确定当前集是否与指定的集合重叠。
        //
        // 参数:
        //   other:
        //     要与当前集进行比较的集合。
        //
        // 返回结果:
        //     如果当前集与 other 至少共享一个通用元素，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     other 为 null。
        bool Overlaps(IEnumerable<T> other);
        //
        // 摘要:
        //     确定当前集与指定的集合是否包含相同的元素。
        //
        // 参数:
        //   other:
        //     要与当前集进行比较的集合。
        //
        // 返回结果:
        //     true 如果当前集等于 other; 否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     other 为 null。
        bool SetEquals(IEnumerable<T> other);
    }
}
