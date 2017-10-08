using SamLu.RegularExpression.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class RangeSetRegexStateMachineActivationContextInfo<T> : RegexStateMachineActivationContextInfoBase<T>
    {
        protected RangeSet<T> rangeSet;
        
        public override ISet<T> AccreditedSet => this.rangeSet;
        
        public RangeSetRegexStateMachineActivationContextInfo(RangeSet<T> rangeSet)
        {
            if (rangeSet == null) throw new ArgumentNullException(nameof(rangeSet));

            this.rangeSet = rangeSet;
        }

        protected override IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexConst(RegexConst<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (this.AccreditedSet.Any(t=>regex.Condition(t)))
                return BasicRegexFATransition<T>.Adapt(new RangeSetRegexFATransition<T, BasicRegexNFAState<T>>(new RangeSet<T>(new[] { (IRange<T>)regex }, this.rangeSet.RangeInfo)));
            else
                return new BasicRegexFATransition<T>(t => false);
        }

        protected override IAcceptInputTransition<T> ActivateRegexNFATransitionFromRegexRange(RegexRange<T> regex)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            if (this.AccreditedSet.Any(t => regex.Condition(t)))
                return BasicRegexFATransition<T>.Adapt(new RangeSetRegexFATransition<T, BasicRegexNFAState<T>>(new RangeSet<T>(new[] { (IRange<T>)regex }, this.rangeSet.RangeInfo)));
            else
                return new BasicRegexFATransition<T>(t => false);
        }
    }
}
