using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.Service
{
    public class CaptureService<T> : RegexFSMService<T>
    {
        private IRegexFSM<T> fsm;

        private Dictionary<object, Stack<(object id, int start)>> dictionary = new Dictionary<object, Stack<(object, int)>>();
        
        public override void Connect(IRegexFSM<T> fsm)
        {
            if (fsm == null) throw new ArgumentNullException(nameof(fsm));

            this.fsm = fsm;
        }

        public void StartCapture(object captureIDToken, object captureID)
        {
            Stack<(object id, int start)> startStack;
            if (!this.dictionary.ContainsKey(captureIDToken))
                this.dictionary.Add(captureIDToken, startStack = new Stack<(object, int)>());
            else
                startStack = this.dictionary[captureIDToken];

            startStack.Push((captureID, this.fsm.Index));
        }

        public void EndCapture(object captureIDToken, Action<object, object, int, int> captureCallback)
        {
            if (this.dictionary.ContainsKey(captureIDToken))
                throw new ArgumentOutOfRangeException(nameof(captureIDToken), captureIDToken, "不存在此捕获标识符。");
            else
            {
                Stack<(object id, int start)> startStack = this.dictionary[captureIDToken];
                if (startStack.Count == 0)
                    throw new InvalidOperationException("未开始捕获。");
                else
                {
                    (object id, int start) = startStack.Pop();
                    captureCallback?.Invoke(captureIDToken, id, start, this.fsm.Index - start + 1);
                }
            }
        }
    }
}
