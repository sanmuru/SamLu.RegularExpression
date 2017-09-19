using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    public class CustomizedAction : SamLu.StateMachine.Action
    {
        private Action<object, object[]> method;

        public Action<object, object[]> Method => this.method;

        public CustomizedAction(Action<object, object[]> method) =>
            this.method = method ?? throw new ArgumentNullException(nameof(method));

        public override void Invoke(object sender, params object[] args)
        {
            this.method(sender, args);
        }

        #region op_Addition
        public static CustomizedAction operator +(CustomizedAction left, CustomizedAction right)
        {
            if (left == null) return right;
            else if (right == null) return left;
            else
                return new CustomizedAction(left.method + right.method);
        }

        public static CustomizedAction operator +(CustomizedAction left, SamLu.StateMachine.IAction right)
        {
            if (right is CustomizedAction customizedAction)
                return left + customizedAction;
            else
            {
                if (left == null)
                {
                    if (right == null) return null;
                    else
                        return new CustomizedAction(right.Invoke);
                }
                else if (right == null) return left;
                else
                    return new CustomizedAction(left.method + right.Invoke);
            }
        }

        public static CustomizedAction operator +(SamLu.StateMachine.IAction left, CustomizedAction right) => right + left;
        #endregion

        #region op_Subtraction
        public static CustomizedAction operator -(CustomizedAction left, CustomizedAction right)
        {
            if (left == null) return null;
            else if (right == null) return left;
            else
                return new CustomizedAction(left.method - right.method);
        }

        public static CustomizedAction operator -(CustomizedAction left, SamLu.StateMachine.IAction right)
        {
            if (right is CustomizedAction customizedAction)
                return left - customizedAction;
            else
            {
                if (left == null) return null;
                else if (right == null) return left;
                else
                    return new CustomizedAction(left.method - right.Invoke);
            }
        }
        #endregion
    }
}
