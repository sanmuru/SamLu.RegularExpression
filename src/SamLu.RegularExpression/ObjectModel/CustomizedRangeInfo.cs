using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class CustomizedRangeInfo<T> : RangeInfo<T>
    {
        protected Func<T, T> getPrevFunc;
        protected Func<T, T> getNextFunc;

        protected CustomizedRangeInfo() : base() { }

        public CustomizedRangeInfo(Func<T, T> getPrevFunc, Func<T, T> getNextFunc, Comparison<T> comparison) : base(comparison)
        {
            if (getPrevFunc == null) throw new ArgumentNullException(nameof(getPrevFunc));
            if (getNextFunc == null) throw new ArgumentNullException(nameof(getNextFunc));

            this.getPrevFunc = getPrevFunc;
            this.getNextFunc = getNextFunc;
        }
        
        public override T GetPrev(T value) => this.getPrevFunc(value);

        public override T GetNext(T value) => this.getNextFunc(value);
    }
}
