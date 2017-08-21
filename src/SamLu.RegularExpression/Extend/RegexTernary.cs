using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexTernary<T> : RegexMultiBranch<T>
    {
        public RegexObject<T> Condition => base.branches.First().Key;
        public RegexObject<T> TruePattern => base.branches.First().Value;
        public RegexObject<T> FalsePattern => base.otherwisePattern;

        public RegexTernary(RegexObject<T> condition, RegexObject<T> truePattern, RegexObject<T> falsePattern) :
            base(
                new[]
                {
                    (
                        condition ?? throw new ArgumentNullException(nameof(condition)),
                        truePattern ?? throw new ArgumentNullException(nameof(truePattern))
                    )
                },
                falsePattern ?? throw new ArgumentNullException(nameof(falsePattern))
            )
        { }
    }
}
