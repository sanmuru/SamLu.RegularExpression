using SamLu.RegularExpression.StateMachine.FunctionalTransitions;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 表示正则表达式构造的有限状态机。
    /// </summary>
    public class RegexFSM<T> : FSM, IRegexFSM<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的当前状态。
        /// </summary>
        new public virtual IRegexFSMState<T> CurrentState
        {
            get => (IRegexFSMState<T>)base.CurrentState;
            protected set => this.CurrentState = value;
        }

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的起始状态。
        /// </summary>
        new public IRegexFSMState<T> StartState
        {
            get => (IRegexFSMState<T>)base.StartState;
            set => base.StartState = value;
        }

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的状态集。
        /// </summary>
        new public virtual ICollection<IRegexFSMState<T>> States =>
            new ReadOnlyCollection<IRegexFSMState<T>>(
                this.StartState == null ?
                    new List<IRegexFSMState<T>>() :
                    this.StartState.RecurGetStates().Cast<IRegexFSMState<T>>().ToList()
            );

        protected IList<Match<T>> matches = new List<Match<T>>();
        public MatchCollection<T> Matches =>
            new MatchCollection<T>(this.matches);

        #region Match
        public event RegexFSMMatchEventHandler<T> Match;

#pragma warning disable 1591
        protected virtual void this_Match(object sender, RegexFSMMatchEventArgs<T> e)
        {
            this.matches.Add(e.Match);
        }
#pragma warning restore 1591

        protected virtual void OnMatch(RegexFSMMatchEventArgs<T> e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            this.Match(this, e);
        }
        #endregion

        /// <summary>
        /// 初始化 <see cref="RegexFSM{T}"/> 类的新实例。
        /// </summary>
        public RegexFSM() : base()
        {
            this.Match += this.this_Match;
        }

        #region 栈项类型
        public class CaptureStackItem
        {
            public int StateStackCount { get; set; }
            public object ID { get; set; }
            public int CaptureStart { get; set; }
            public int CaptureLength { get; set; }
        }

        public class StateStackItem
        {
            public int CommandStackCount { get; set; }
            public int Start { get; set; }
            public IRegexFSMState<T> State { get; set; }
            public IEnumerator<IRegexFSMTransition<T>> TransitionEnumerator { get; set; }
        }
        #endregion

        /// <summary>
        /// 捕获栈。
        /// </summary>
        protected Stack<CaptureStackItem> captureStack = new Stack<CaptureStackItem>();
        /// <summary>
        /// 命令栈
        /// </summary>
        protected Stack<(IRegexFSMTransition<T> functionalTransition, object arg, int preCommandStackCount, int thisStart)> commandStack = new Stack<(IRegexFSMTransition<T>, object, int, int)>();
        /// <summary>
        /// 状态栈
        /// </summary>
        protected Stack<StateStackItem> stateStack = new Stack<StateStackItem>();

        public virtual void BeginCapture(object id)
        {
            throw new NotImplementedException();
        }

        public virtual void EndCapture(object id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化状态机实例必要字段，进行匹配前准备工作。
        /// </summary>
        protected virtual void BeginMatch(IEnumerable<T> inputs)
        {
            this.Inputs = inputs;

            if (this.captureStack == null)
                this.captureStack = new Stack<CaptureStackItem>();
            else if (this.captureStack.Count != 0)
                this.captureStack.Clear();

            if (this.commandStack == null)
                this.commandStack = new Stack<(IRegexFSMTransition<T>, object, int, int)>();
            else if (this.commandStack.Count != 0)
                this.commandStack.Clear();

            if (this.stateStack == null)
                this.stateStack = new Stack<StateStackItem>();
            else if (this.stateStack.Count != 0)
                this.stateStack.Clear();
        }

#warning
        public IEnumerable<T> Inputs { get; protected set; }
        public int Index { get; protected set; }
        private int start;
        private int top;
        public virtual void EndMatch()
        {
            while (this.stateStack.Count != 0 && !this.stateStack.Peek().State.IsTerminal)
                this.stateStack.Pop();
            if (this.stateStack.Count == 0)
            {
                this.captureStack.Clear();
                this.commandStack.Clear();
                this.stateStack.Clear();
            }
            else
            {
                while (this.captureStack.Count != 0 && !(this.captureStack.Peek().StateStackCount < this.stateStack.Count))
                    this.captureStack.Pop();
                if (this.captureStack.Count == 0) return;
                else
                {
                    this.OnMatch(new RegexFSMMatchEventArgs<T>(
                        new Match<T>(this.Inputs, this.start, this.top - this.start + 1,
                            this.captureStack.Reverse()
                                .GroupBy(
                                    (captureInfo => captureInfo.ID),
                                    (captureInfo => (captureInfo.CaptureStart, captureInfo.CaptureLength)),
                                    new EqualityComparisonComparer<object>((x, y) =>
                                    {
                                        if (x == null && y == null) return false;
                                        else return object.Equals(x, y);
                                    })
                                )
                                .Select(group =>
                                {
                                    var captures = group.ToArray();
                                    return new Extend.Group<T>(this.Inputs, captures);
                                })
                        )
                    ));
                }
            }
        }

        #region AttachTransition
        /// <summary>
        /// 为 <see cref="RegexFSM{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool AttachTransition(IState state, ITransition transition) =>
            this.AttachTransition((IRegexFSMState<T>)state, (IRegexFSMTransition<T>)transition);

        /// <summary>
        /// 为 <see cref="RegexFSM{T}"/> 的一个指定状态添加指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            base.AttachTransition(state, transition);
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 从 <see cref="RegexFSM{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool RemoveTransition(IState state, ITransition transition) =>
            this.RemoveTransition((IRegexFSMState<T>)state, (IRegexFSMTransition<T>)transition);

        /// <summary>
        /// 从 <see cref="RegexFSM{T}"/> 的一个指定状态移除指定转换。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            base.RemoveTransition(state, transition);
        #endregion

        #region SetTarget
        /// <summary>
        /// 将 <see cref="RegexFSM{T}"/> 的一个指定转换的目标设为指定状态。
        /// </summary>
        /// <param name="transition">指定的目标。</param>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool SetTarget(ITransition transition, IState state) =>
            this.SetTarget((IRegexFSMTransition<T>)transition, (IRegexFSMState<T>)state);

        /// <summary>
        /// 将 <see cref="RegexFSM{T}"/> 的一个指定转换的目标设为指定状态。
        /// </summary>
        /// <param name="transition">指定的目标。</param>
        /// <param name="state">指定的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state) =>
            base.SetTarget(transition, state);
        #endregion

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的转换操作。此操作沿指定的转换进行。（默认的参数为此状态机本身）。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected bool Transit(IRegexFSMTransition<T> transition) => this.Transit(transition, this);

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的转换操作。此操作沿指定的转换进行，接受指定的参数。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <param name="args">指定的参数。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool Transit(ITransition transition, params object[] args) =>
            base.Transit(transition, args);

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的转换操作。此操作沿指定的转换进行，接受指定的参数。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <param name="args">指定的参数。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool Transit(IRegexFSMTransition<T> transition, params object[] args) =>
            base.Transit(transition, args);

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的转换操作。此操作将使有限状态机模型的 <see cref="CurrentState"/> 转换为指定的状态。（默认的参数为此状态机本身）。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">指定的转换。若转换操作非正常逻辑转换，应设为 null 。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected bool Transit(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) => this.Transit(state, transition, this);

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的转换操作。此操作将使有限状态机模型的 <see cref="CurrentState"/> 转换为指定的状态，接受指定的参数。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">指定的转换。若转换操作非正常逻辑转换，应设为 null 。</param>
        /// <param name="args">指定的参数。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected sealed override bool Transit(IState state, ITransition transition, params object[] args)=>
            base.Transit(state, transition, args);

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的转换操作。此操作将使有限状态机模型的 <see cref="CurrentState"/> 转换为指定的状态，接受指定的参数。
        /// </summary>
        /// <param name="state">指定的状态。</param>
        /// <param name="transition">指定的转换。若转换操作非正常逻辑转换，应设为 null 。</param>
        /// <param name="args">指定的参数。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected virtual bool Transit(IRegexFSMState<T> state, IRegexFSMTransition<T> transition, params object[] args) =>
            base.Transit(state, transition, args);

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected internal virtual bool Transit(T input)
        {
            IEnumerator<IRegexFSMTransition<T>> enumerator;
            if (this.stateStack.Peek().State != this.CurrentState)
            {
                // 获取可以接受输入并进行转换的转换。
                var transitions = this.CurrentState.GetOrderedTransitions();
                if (transitions.Any())
                {
                    enumerator = transitions.GetEnumerator();
                    this.stateStack.Push(new StateStackItem()
                    {
                        CommandStackCount = this.commandStack.Count,
                        Start = this.start,
                        State = this.CurrentState,
                        TransitionEnumerator = enumerator
                    });
                }
                else return false;
            }
            else
                enumerator = this.stateStack.Peek().TransitionEnumerator;

            if (enumerator.MoveNext())
            {
                IRegexFSMTransition<T> transition = enumerator.Current;
                if (transition is IRegexFSMTransitionProxy<T>)
                    return ((IRegexFSMTransitionProxy<T>)transition).TransitProxy(
                        input,
                        (_transition, _args) => this.Transit(_transition.Target, _transition, _args),
                        this
                    ) && this.Transit(transition);
                else if (transition is IAcceptInputTransition<T>)
                    return ((IAcceptInputTransition<T>)transition).CanAccept(input) && this.Transit(transition);
                else
                    return this.Transit(transition) && this.Transit(input);
            }
            else return false;
        }

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <typeparam name="TInput">输入的类型。</typeparam>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <see cref="Transit(T)"/>
        /// <see cref="TransitManyInternal(IEnumerable{T})"/>
        public override bool Transit<TInput>(TInput input)
        {
            if (input is T)
                return this.Transit((T)(object)input);
            else if (input is IEnumerable<T>)
                return this.TransitManyInternal((IEnumerable<T>)input);
            else
                return base.Transit(input);
        }

        /// <summary>
        /// 接受一个指定输入序列并进行一组转换动作。
        /// </summary>
        /// <param name="inputs">指定的输入序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputs"/> 的值为 null 。</exception>
        public virtual void TransitMany(IEnumerable<T> inputs)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));

            this.BeginMatch(inputs);

            this.TransitManyInternal(inputs);

            this.EndMatch();
        }

        /// <summary>
        /// 子类重写时，提供接受一个指定输入序列并进行一组转换动作的实现。
        /// </summary>
        /// <param name="inputs">指定的输入序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputs"/> 的值为 null 。</exception>
        protected virtual bool TransitManyInternal(IEnumerable<T> inputs)
        {
            foreach (var input in inputs)
            {
                if (!this.Transit(input))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 表示正则表达式构造的有限状态机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">有限状态机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">有限状态机的转换的类型。</typeparam>
    public class RegexFSM<T, TState, TTransition> : FSM<TState, TTransition>, IRegexFSM<T, TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
        protected IList<Match<T>> matches = new List<Match<T>>();
        public MatchCollection<T> Matches =>
            new MatchCollection<T>(this.matches);

        #region Match
        public event RegexFSMMatchEventHandler<T> Match;

#pragma warning disable 1591
        protected virtual void this_Match(object sender, RegexFSMMatchEventArgs<T> e)
        {
            this.matches.Add(e.Match);
        }
#pragma warning restore 1591

        protected virtual void OnMatch(RegexFSMMatchEventArgs<T> e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            this.Match(this, e);
        }
        #endregion

        public IEnumerable<T> Inputs { get; protected set; }
        public int Index { get; protected set; }

        public virtual void BeginCapture(object id)
        {
            throw new NotImplementedException();
        }

        public virtual void EndCapture(object id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化状态机实例必要字段，进行匹配前准备工作。
        /// </summary>
        protected virtual void BeginMatch(IEnumerable<T> inputs)
        {
            this.Inputs = inputs;
        }

        public virtual void EndMatch()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected internal virtual bool Transit(T input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <typeparam name="TInput">输入的类型。</typeparam>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <see cref="Transit(T)"/>
        /// <see cref="TransitManyInternal(IEnumerable{T})"/>
        public override bool Transit<TInput>(TInput input)
        {
            if (input is T)
                return this.Transit((T)(object)input);
            else if (input is IEnumerable<T>)
                return this.TransitManyInternal((IEnumerable<T>)input);
            else
                return base.Transit(input);
        }

        /// <summary>
        /// 接受一个指定输入序列并进行一组转换动作。
        /// </summary>
        /// <param name="inputs">指定的输入序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputs"/> 的值为 null 。</exception>
        public virtual void TransitMany(IEnumerable<T> inputs)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));

            this.BeginMatch(inputs);

            this.TransitManyInternal(inputs);

            this.EndMatch();
        }

        /// <summary>
        /// 子类重写时，提供接受一个指定输入序列并进行一组转换动作的实现。
        /// </summary>
        /// <param name="inputs">指定的输入序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputs"/> 的值为 null 。</exception>
        protected virtual bool TransitManyInternal(IEnumerable<T> inputs)
        {
            foreach (var input in inputs)
            {
                if (!this.Transit(input))
                    return false;
            }

            return true;
        }

        #region IRegexFSM{T} Implementation
        IRegexFSMState<T> IRegexFSM<T>.CurrentState => base.CurrentState;

        IRegexFSMState<T> IRegexFSM<T>.StartState { get => this.StartState; set => this.StartState = (TState)value; }

        ICollection<IRegexFSMState<T>> IRegexFSM<T>.States =>
            new ReadOnlyCollection<IRegexFSMState<T>>(
                this.StartState == null ?
                    new List<IRegexFSMState<T>>() :
                    ((IEnumerable<IRegexFSMState<T>>)this.StartState.RecurGetStates<TState, TTransition>()).ToList()
            );

        bool IRegexFSM<T>.AttachTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            this.AttachTransition((TState)state, (TTransition)transition);

        bool IRegexFSM<T>.RemoveTransition(IRegexFSMState<T> state, IRegexFSMTransition<T> transition) =>
            this.RemoveTransition((TState)state, (TTransition)transition);

        bool IRegexFSM<T>.SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state) =>
            this.SetTarget((TTransition)transition, (TState)state);
        #endregion
    }
}
