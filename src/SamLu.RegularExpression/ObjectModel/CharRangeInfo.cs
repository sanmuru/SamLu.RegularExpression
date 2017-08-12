using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class CharRangeInfo : RangeInfo<char>
    {
        public CharRangeInfo() : base() { }
        
        protected CharRangeInfo(Comparison<char> comparison) : base(comparison) { }

        protected override IRange<char> CreateInternal(char minimum, char maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            new CharRange(minimum, maximum, canTakeMinimum, canTakeMaximum);

        public override char GetPrev(char value)
        {
            if (value == char.MinValue) throw new InvalidOperationException();

            return (char)(value - 1);
        }

        public override char GetNext(char value)
        {
            if (value == char.MaxValue) throw new InvalidOperationException();

            return (char)(value + 1);
        }

        public override bool IsValidInternal(char minimum, char maximum, bool canTakeMinimum, bool canTakeMaximum) =>
            (maximum - minimum > 1 || (canTakeMinimum || canTakeMinimum));

        protected override bool IsOverlapInternal(
            char firstMinimum, char firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            char secondMinimum, char secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum
        )
        {
            if (firstMaximum < secondMinimum || firstMinimum > secondMaximum) return false;

            if (firstMaximum == secondMinimum)
            {
                if (firstCanTakeMaximum && secondCanTakeMinimum) return true;
            }
            else if (firstMaximum - secondMinimum == 1)
            {
                if (firstCanTakeMaximum || secondCanTakeMinimum) return true;
            }
            else return true;

            if (secondMaximum == firstMinimum)
            {
                if (firstCanTakeMaximum && secondCanTakeMinimum) return true;
            }
            else if (secondMinimum - firstMaximum == 1)
            {
                if (firstCanTakeMaximum || secondCanTakeMinimum) return true;
            }
            else return true;

            return false;
        }

        protected override void IntersectInternal(
            char firstMinimum, char firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            char secondMinimum, char secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out char resultMinimum, out char resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (firstMinimum < secondMinimum)
            {
                resultMinimum = secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum);
                resultCanTakeMinimum = secondCanTakeMinimum;
            }
            else if (firstMinimum > secondMinimum)
            {
                resultMinimum = firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum;
            }
            else
            {
                resultMinimum = (firstCanTakeMinimum && secondCanTakeMinimum) ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum && secondCanTakeMinimum;
            }

            if (firstMaximum < secondMaximum)
            {
                resultMaximum = firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum;
            }
            else if (firstMaximum > secondMaximum)
            {
                resultMaximum = secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum);
                resultCanTakeMaximum = secondCanTakeMaximum;
            }
            else
            {
                resultMaximum = (firstCanTakeMaximum && secondCanTakeMaximum) ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum && secondCanTakeMaximum;
            }
        }

        protected override void UnionInternal(
            char firstMinimum, char firstMaximum, bool firstCanTakeMinimum, bool firstCanTakeMaximum,
            char secondMinimum, char secondMaximum, bool secondCanTakeMinimum, bool secondCanTakeMaximum,
            out char resultMinimum, out char resultMaximum, out bool resultCanTakeMinimum, out bool resultCanTakeMaximum
        )
        {
            if (firstMinimum < secondMinimum)
            {
                resultMinimum = firstCanTakeMinimum ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum;
            }
            else if (firstMinimum > secondMinimum)
            {
                resultMinimum = secondCanTakeMinimum ? secondMinimum : this.GetNext(secondMinimum);
                resultCanTakeMinimum = secondCanTakeMinimum;
            }
            else
            {
                resultMinimum = (firstCanTakeMinimum || secondCanTakeMinimum) ? firstMinimum : this.GetNext(firstMinimum);
                resultCanTakeMinimum = firstCanTakeMinimum || secondCanTakeMinimum;
            }

            if (firstMaximum < secondMaximum)
            {
                resultMaximum = secondCanTakeMaximum ? secondMaximum : this.GetPrev(secondMaximum);
                resultCanTakeMaximum = secondCanTakeMaximum;
            }
            else if (firstMaximum > secondMaximum)
            {
                resultMaximum = firstCanTakeMaximum ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum;
            }
            else
            {
                resultMaximum = (firstCanTakeMaximum || secondCanTakeMaximum) ? firstMaximum : this.GetPrev(firstMaximum);
                resultCanTakeMaximum = firstCanTakeMaximum || secondCanTakeMaximum;
            }
        }
    }
}
