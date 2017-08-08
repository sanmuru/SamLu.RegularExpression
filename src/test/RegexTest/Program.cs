using SamLu.RegularExpression;
using SamLu.RegularExpression.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();
            var chars = Regex.Range('\0', 'z', true, false);
            var tchars = Regex.Range(new TT<char>('a'), new TT<char>('z'), false, true);

            var phone = Regex.Range(0, 9).RepeatMany(null);
            
            Func<RegexRange<string>> func = () =>
                new RegexRangeAdaptor<int, string>(
                    0, 255,
                    (source => source.ToString()),
                    (target => int.Parse(target))
                );
            var ipAddress = new RegexObject<string>[] { func(), Regex.Const("."), func(), Regex.Const("."), func(), Regex.Const("."), func() }.ConcatMany();
            ;
        }

        [System.Diagnostics.DebuggerDisplay("{t}")]
        struct TT<T> : IComparable<TT<T>>, IEquatable<TT<T>>
            where T : IComparable<T>, IEquatable<T>
        {
            private T t;

            public TT(T t) => this.t = t;

            public int CompareTo(TT<T> other)
            {
                return this.t.CompareTo(other.t);
            }

            public bool Equals(TT<T> other)
            {
                return this.t.Equals(other.t);
            }

            public override string ToString()
            {
                return this.t.ToString();
            }
        }
    }
}
