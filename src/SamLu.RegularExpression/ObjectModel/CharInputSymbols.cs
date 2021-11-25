using SamLu.StateMachine.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.ObjectModel
{
    public class CharInputSymbols : InputSymbols<char>
    {
        public override bool HasNext(char value) => value != char.MaxValue;

        public override bool HasPrevious(char value) => value != char.MinValue;

        public override char Previous(char value) => (char)(value - 1);

        public override char Next(char value) => (char)(value + 1);

        public override bool NextTo(char x, char y) => Math.Abs(x - y) == 1;
    }
}
