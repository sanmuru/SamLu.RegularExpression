using SamLu.StateMachine;
using SamLu.StateMachine.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class RegexTransition<T> : Transition<T, RegexState<T>>
    {
        private bool isEpsilon;
        public override bool IsEpsilon => this.isEpsilon;

        protected Func<T?, IInputSymbols<T>, bool> predicate;

        public RegexTransition() => this.isEpsilon = true;

        public RegexTransition(Func<T?, IInputSymbols<T>, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            this.isEpsilon = false;
            this.predicate = predicate;
        }

        public override bool Predicate(T? input, IInputSymbols<T> inputSymbols) => this.Predicate(input, inputSymbols);
    }
}
