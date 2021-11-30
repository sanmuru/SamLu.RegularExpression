using SamLu.StateMachine.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class EnumInputSymbols<T> : InputSymbols<T> where T : struct, Enum
    {
        protected static readonly IList<T> members;
        
        static EnumInputSymbols()
        {
            IEnumerable<T> members;
            if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                members = 
                    from values in Enum.GetValues<T>().GetOptionalCombinations()
                    let sum = values.Sum(t => (long)Convert.ChangeType(t, Enum.GetUnderlyingType(typeof(T))))
                    let result = (T)Convert.ChangeType(sum, typeof(T))
                    select result;
            else
                members = Enum.GetValues<T>();
            EnumInputSymbols<T>.members = members.ToList().AsReadOnly();
        }

        public override bool HasNext(T value)
        {
            int index = EnumInputSymbols<T>.members.IndexOf(value);
            return index >= 0 && index < EnumInputSymbols<T>.members.Count - 1;
        }

        public override bool HasPrevious(T value)
        {
            int index = EnumInputSymbols<T>.members.IndexOf(value);
            return index >= 1 && index < EnumInputSymbols<T>.members.Count;
        }

        public override T Previous(T value)
        {
            int index = EnumInputSymbols<T>.members.IndexOf(value);
            return EnumInputSymbols<T>.members[index - 1];
        }

        public override T Next(T value)
        {
            int index = EnumInputSymbols<T>.members.IndexOf(value);
            return EnumInputSymbols<T>.members[index + 1];
        }

        public override bool NextTo(T x, T y)
        {
            int indexX = EnumInputSymbols<T>.members.IndexOf(x);
            int indexY = EnumInputSymbols<T>.members.IndexOf(y);

            return (indexX >= 0 && indexY >= 0) &&
                Math.Abs(indexX - indexY) == 1;
        }
    }
}
