using SamLu.IO;
using SamLu.RegularExpression.StateMachine.FunctionalTransitions;
using SamLu.RegularExpression.StateMachine.ObjectModel;
using SamLu.RegularExpression.StateMachine.Service;
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
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
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

        /// <summary>
        /// 匹配列表。
        /// </summary>
        protected IList<Match<T>> matches = new List<Match<T>>();
        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的所有匹配。
        /// </summary>
        public MatchCollection<T> Matches => new MatchCollection<T>(this.matches);

        #region Match
        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的匹配事件。
        /// </summary>
        public event RegexFSMMatchEventHandler<T> Match;

#pragma warning disable 1591
        protected virtual void this_Match(object sender, RegexFSMMatchEventArgs<T> e)
        {
            this.matches.Add(e.Match);
        }
#pragma warning restore 1591

        /// <summary>
        /// 引发 <see cref="Match"/> 事件。
        /// </summary>
        /// <param name="e"><see cref="Match"/> 事件的事件参数。</param>
        protected virtual void OnMatch(RegexFSMMatchEventArgs<T> e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            this.Match?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// 初始化 <see cref="RegexFSM{T}"/> 类的新实例。
        /// </summary>
        public RegexFSM() : base()
        {
            this.Match += this.this_Match;
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
        /// <param name="transition">指定的转换。</param>
        /// <param name="state">要设为目标的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public sealed override bool SetTarget(ITransition transition, IState state) =>
            this.SetTarget((IRegexFSMTransition<T>)transition, (IRegexFSMState<T>)state);

        /// <summary>
        /// 将 <see cref="RegexFSM{T}"/> 的一个指定转换的目标设为指定状态。
        /// </summary>
        /// <param name="transition">指定的转换。</param>
        /// <param name="state">要设为目标的状态。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        public virtual bool SetTarget(IRegexFSMTransition<T> transition, IRegexFSMState<T> state) =>
            base.SetTarget(transition, state);
        #endregion

        #region 缓存数据
        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的捕获栈。
        /// </summary>
        protected CaptureStack<T> captureStack;
        /// <summary>
        /// <see cref="RegexFSM{T}"/> 的状态栈
        /// </summary>
        protected StateStack<T, IRegexFSMState<T>, IRegexFSMTransition<T>> stateStack;

        /// <summary>
        /// <see cref="RegexFSM{T}"/> 内部的 <see cref="NodeReader{TReader, TNode}"/> 。
        /// </summary>
        private NodeReader<IEnumerable<T>, T> nodeReader;

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 匹配过程的输入对象序列。
        /// </summary>
        public virtual IEnumerable<T> Inputs
        {
            get => this.nodeReader.Reader;
            protected set => this.nodeReader = value?.CreateReader();
        }

        /// <summary>
        /// 获取或（子类）设置 <see cref="RegexFSM{T}"/> 匹配过程的在 <see cref="Inputs"/> 中的当前位置。
        /// </summary>
        public virtual int Index
        {
            get => this.nodeReader.Position;
            protected set => this.nodeReader.Position = value;
        }

        private int start;
        /// <summary>
        /// 获取或（子类）设置 <see cref="RegexFSM{T}"/> 匹配过程的起始位置。子类设置其值时，将使 <see cref="Index"/> 复位至 <see cref="Start"/> 。
        /// </summary>
        public virtual int Start
        {
            get => this.start;
            protected set => this.start = this.Index = value; // 修改匹配起始位置的同时将当前位置复位至起始位置。
        }

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 下一个接受的对象。
        /// </summary>
        /// <exception cref="InvalidOperationException">内部读取器已抵达结尾。</exception>
        public virtual T NextInput => this.nodeReader.Peek();
        #endregion

        /// <summary>
        /// 记录一个匹配。
        /// </summary>
        /// <param name="captureIDToken">匹配的 ID 标志符。</param>
        /// <param name="id">捕获的 ID 。</param>
        /// <param name="start">捕获的开始位置。</param>
        /// <param name="length">捕获的长度。</param>
        public virtual void Capture(object captureIDToken, object id, int start, int length)
        {
            this.captureStack.Push(new CaptureStack<T>.Item()
            {
                StateStackCount = this.stateStack.Count,
                CaptureIDToken = captureIDToken,
                CaptureID = id,
                CaptureStart = start,
                CaptureLength = length
            });
        }

        /// <summary>
        /// 初始化状态机实例必要字段，进行匹配前准备工作。
        /// </summary>
        protected virtual void BeginMatch(IEnumerable<T> inputs)
        {
            this.Inputs = inputs;
            this.Start = 0;

            if (this.matches.Count != 0)
                this.matches.Clear();

            if (this.captureStack == null)
                this.captureStack = new CaptureStack<T>();
            else if (this.captureStack.Count != 0)
                this.captureStack.Clear();

            if (this.stateStack == null)
                this.stateStack = new StateStack<T, IRegexFSMState<T>, IRegexFSMTransition<T>>() { RelatedCaptureStack = this.captureStack };
            else if (this.stateStack.Count != 0)
                this.stateStack.Clear();
        }

        /// <summary>
        /// 回收状态机实例的缓存，进行匹配后结束工作。
        /// </summary>
        protected virtual void EndMatch()
        {
            this.nodeReader = null;
            this.start = 0;
        }

        #region Transit
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
        protected sealed override bool Transit(IState state, ITransition transition, params object[] args) =>
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
        /// <typeparam name="TInput">输入的类型。</typeparam>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <see cref="TransitMany(IEnumerable{T})"/>
        public override bool Transit<TInput>(TInput input)
        {
            if (input is T)
            {
                this.TransitMany(new[] { (T)(object)input });
                return true;
            }
            else if (input is IEnumerable<T>)
            {
                this.TransitMany((IEnumerable<T>)input);
                return true;
            }
            else
                return base.Transit(input);
        }

        /// <summary>
        /// 对 <see cref="RegexFSM{T}"/> 的 <see cref="NextInput"/> 进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="inputAccepted">获取一个值，指示 <see cref="NextInput"/> 是否被接受。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected virtual bool TransitInternal(out bool inputAccepted)
        {
            IReaderSource<IRegexFSMTransition<T>> transitionReader;
            if (this.stateStack.Count != 0 && this.stateStack.Peek().State == this.CurrentState)
                transitionReader = this.stateStack.Peek().TransitionReader;
            else
            {
                // 获取可以接受输入并进行转换的转换。
                var transitions = this.CurrentState.GetOrderedTransitions();

                transitionReader = transitions.CreateReader(); // 创建转换集的读取源。
                this.stateStack.Push(new StateStack<T, IRegexFSMState<T>, IRegexFSMTransition<T>>.Item()
                {
                    RegexFSMIndex = this.Index,
                    State = this.CurrentState,
                    TransitionReader = transitionReader
                });
            }

            if (transitionReader.HasNext())
            {
                // 获取下一个转换。
                IRegexFSMTransition<T> transition = transitionReader.Read();
                if (transition is IRegexFSMTransitionProxy<T>)
                {
                    inputAccepted = false;
                    return ((IRegexFSMTransitionProxy<T>)transition).TransitProxy(
                        this.nodeReader,
                        (_transition, _args) => this.Transit(_transition.Target, _transition, _args),
                        this
                    ) && this.Transit(transition);
                }
                else if (transition is IAcceptInputTransition<T>)
                    return (inputAccepted = !this.nodeReader.IsEnd() && ((IAcceptInputTransition<T>)transition).CanAccept(this.NextInput)) && this.Transit(transition);
                else
                {
                    inputAccepted = false;
                    return this.Transit(transition);
                }
            }
            else
            { // 当前状态的转换集已遍历结束。
                inputAccepted = false;
                return false;
            }
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

            this.TransitManyInternal();

            this.EndMatch();
        }

        /// <summary>
        /// 子类重写时，提供接受一个指定输入序列并进行一组转换动作的实现。
        /// </summary>
        protected virtual void TransitManyInternal()
        {
            while (true)
            {
                if (this.TransitInternal(out bool inputAccepted))
                { // 转换成功。
                    if (inputAccepted)
                        // 若转换接受字符，则当前位置进一位。
                        this.Index++;
                }
                else
                { // 转换未成功。
                    var stateStackItem = this.stateStack.Peek();
                    this.Index = stateStackItem.RegexFSMIndex; // 确定当前位置。

                    if (!stateStackItem.TransitionReader.HasNext())
                    { // 原因是当前状态的转换集已遍历结束。
                        if (stateStackItem.State.IsTerminal)
                        { // 当前状态是结束状态。
                            // 成功获得一个匹配。
                            this.OnMatch(new RegexFSMMatchEventArgs<T>(
                                new Match<T>(this.Inputs, this.Start, this.Index - this.Start,
                                    this.captureStack.Reverse()
                                        .GroupBy(
                                            (captureInfo =>
                                            {
                                                (object id, object idToken) groupKey = (captureInfo.CaptureID, captureInfo.CaptureIDToken);
                                                return groupKey;
                                            }),
                                            (captureInfo => (captureInfo.CaptureStart, captureInfo.CaptureLength)),
                                            new EqualityComparisonComparer<(object id, object idToken)>((x, y) =>
                                            {
                                                return object.Equals(x.id, y.id) && object.Equals(x.idToken, y.idToken);
                                            })
                                        )
                                        .Select(group =>
                                        {
                                            var captures = group.ToArray();
                                            return (group.Key.id, new Extend.Group<T>(this.Inputs, captures));
                                        })
                                )
                            ));

                            this.Start = this.Index;

                            // 清空堆栈。
                            this.stateStack.Clear();
                            this.Reset(); // 复位状态机。
                        }
                        else
                        { // 当前状态不是结束状态。
                            this.stateStack.Pop(); // 当前状态的所有转换都无法转换，退回上一个状态。

                            if (this.stateStack.Count == 0)
                            { // 包括起始状态的所有状态都被回溯。此次匹配失败，准备从下一个起始位置开始新一轮匹配。
                                // 清空堆栈。
                                this.stateStack.Clear();

                                // 复位状态机。
                                this.Reset();

                                if (this.nodeReader.IsEnd())
                                    // 若内部读取器已经到结尾。无法继续匹配。
                                    break;
                                else
                                    // 设置下一个起始位置。
                                    this.Start++;
                            }
                            else
                            {
                                // 回溯当前位置到上一个状态时的位置。
                                this.Index = this.stateStack.Peek().RegexFSMIndex;
                                this.Transit(this.stateStack.Peek().State);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 状态机服务
        #region Services
        /// <summary>
        /// 表示回溯服务。 <see cref="RegexFSM{T}"/> 可通过记录时间点、回溯到指定时间点来实现功能。
        /// </summary>
        public class BackTraceService : RegexFSMService<T, RegexFSM<T>>
        {
            /// <summary>
            /// 提供此服务的 <see cref="RegexFSM{T}"/> 。
            /// </summary>
            protected RegexFSM<T> fsm;

            /// <summary>
            /// 将 <see cref="BackTraceService"/> 服务连接到 <see cref="RegexFSM{T}"/> 。
            /// </summary>
            /// <param name="fsm">准备被连接的 <see cref="RegexFSM{T}"/> 。</param>
            /// <exception cref="ArgumentNullException"><paramref name="fsm"/> 的值为 null 。</exception>
            public override void Connect(RegexFSM<T> fsm)
            {
                if (fsm == null) throw new ArgumentNullException(nameof(fsm));

                this.fsm = fsm;
            }

            /// <summary>
            /// 获取当前的时间点。
            /// </summary>
            /// <returns>当前的时间点。</returns>
            public Timepoint GetTimepoint()
            {
                return new Timepoint(this);
            }

            /// <summary>
            /// 回溯到指定的时间点，并根据指定值判断是否移除期间的所有捕获。
            /// </summary>
            /// <param name="timepoint">指定的时间点。</param>
            /// <param name="removeCaptures">一个值，指示是否移除期间的所有捕获。</param>
            /// <exception cref="ArgumentNullException"><paramref name="timepoint"/> 的值为 null 。</exception>
            public void BackTrace(Timepoint timepoint, bool removeCaptures)
            {
                if (timepoint == null) throw new ArgumentNullException(nameof(timepoint));

                this.fsm.Index = timepoint.FSMIndex;

                if (removeCaptures)
                {
                    // 回溯状态栈。
                    while (this.fsm.stateStack.Count > timepoint.StateStackCount)
                        this.fsm.stateStack.Pop();
                }
                else
                {
                    var relatedCaptureStack = this.fsm.stateStack.RelatedCaptureStack;
                    this.fsm.stateStack.RelatedCaptureStack = null; // 暂时取消关联。
                    // 回溯状态栈。
                    while (this.fsm.stateStack.Count > timepoint.StateStackCount)
                        this.fsm.stateStack.Pop();
                    this.fsm.stateStack.RelatedCaptureStack = relatedCaptureStack;

                    // 更新捕获栈的项的必要参数。
                    foreach (var item in this.fsm.captureStack.Where(_item => _item.StateStackCount > timepoint.StateStackCount))
                        item.StateStackCount = timepoint.StateStackCount;
                }
            }

            /// <summary>
            /// 表示时间点。
            /// </summary>
            public class Timepoint
            {
                /// <summary>
                /// 记录此时间点时 <see cref="RegexFSM{T}"/> 的 <see cref="RegexFSM{T}.Index"/> 。
                /// </summary>
                public int FSMIndex { get; private set; }
                /// <summary>
                /// 记录此时间点时 <see cref="RegexFSM{T}.stateStack"/> 的 <see cref="StateStack{T, TState, TTransition}.Count"/> 。
                /// </summary>
                public int StateStackCount { get; private set; }

                /// <summary>
                /// 初始化 <see cref="Timepoint"/> 类的新实例。
                /// </summary>
                /// <param name="service">记录此时间点的服务。</param>
                /// <exception cref="ArgumentNullException"><paramref name="service"/> 的值为 null 。</exception>
                protected internal Timepoint(BackTraceService service)
                {
                    if (service == null) throw new ArgumentNullException(nameof(service));

                    this.FSMIndex = service.fsm.Index;
                    this.StateStackCount = service.fsm.stateStack.Count;
                }
            }
        }

        /// <summary>
        /// 表示进度服务。 <see cref="RegexFSM{T}"/> 可通过记录进度、回溯到指定进度来实现功能。
        /// </summary>
        public class ProgressService : RegexFSMService<T, RegexFSM<T>>
        {
            /// <summary>
            /// 提供此服务的 <see cref="RegexFSM{T}"/> 。
            /// </summary>
            protected internal RegexFSM<T> fsm;

            /// <summary>
            /// 将 <see cref="ProgressService"/> 服务连接到 <see cref="RegexFSM{T}"/> 。
            /// </summary>
            /// <param name="fsm">准备被连接的 <see cref="RegexFSM{T}"/> 。</param>
            /// <exception cref="ArgumentNullException"><paramref name="fsm"/> 的值为 null 。</exception>
            public override void Connect(RegexFSM<T> fsm)
            {
                if (fsm == null) throw new ArgumentNullException(nameof(fsm));

                this.fsm = fsm;
            }

            /// <summary>
            /// 记录当前的进度。
            /// </summary>
            /// <returns>当前的进度。</returns>
            public Progress GetProgress()
            {
                return new Progress(this);
            }

            /// <summary>
            /// 设置到指定的进度。
            /// </summary>
            /// <param name="progress">指定的进度</param>
            /// <exception cref="ArgumentNullException"><paramref name="progress"/> 的值为 null 。</exception>
            public void SetProgress(Progress progress)
            {
                if (progress == null) throw new ArgumentNullException(nameof(progress));

                this.fsm.Index = progress.Value;
            }

            /// <summary>
            /// 表示进度。
            /// </summary>
            public class Progress
            {
                /// <summary>
                /// 记录此进度时 <see cref="RegexFSM{T}"/> 的 <see cref="RegexFSM{T}.Index"/> 。
                /// </summary>
                public int Value { get; private set; }

                /// <summary>
                /// 初始化 <see cref="Progress"/> 类的新实例。
                /// </summary>
                /// <param name="service">记录此进度的服务。</param>
                /// <exception cref="ArgumentNullException"><paramref name="service"/> 的值为 null 。</exception>
                protected internal Progress(ProgressService service)
                {
                    if (service == null) throw new ArgumentNullException(nameof(service));

                    this.Value = service.fsm.Index;
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取 <see cref="RegexFSM{T}"/> 的服务。
        /// </summary>
        /// <typeparam name="TService">服务的类型。</typeparam>
        /// <returns><see cref="RegexFSM{T}"/> 的指定类型的服务。</returns>
        public virtual TService GetService<TService>()
            where TService : IRegexFSMService<T>, new()
        {
            TService service = new TService();
            service.Connect(this);

            return service;
        }
        #endregion
    }

    /// <summary>
    /// 表示正则表达式构造的有限状态机。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    /// <typeparam name="TTransition">正则构造的有限状态机的转换的类型。</typeparam>
    public class RegexFSM<T, TState, TTransition> : FSM<TState, TTransition>, IRegexFSM<T, TState, TTransition>
        where TState : IRegexFSMState<T, TTransition>
        where TTransition : IRegexFSMTransition<T, TState>
    {
        /// <summary>
        /// 匹配列表。
        /// </summary>
        protected IList<Match<T>> matches = new List<Match<T>>();
        /// <summary>
        /// <see cref="RegexFSM{T, TState, TTransition}"/> 的所有匹配。
        /// </summary>
        public MatchCollection<T> Matches => new MatchCollection<T>(this.matches);

        #region Match
        /// <summary>
        /// <see cref="RegexFSM{T, TState, TTransition}"/> 的匹配事件。
        /// </summary>
        public event RegexFSMMatchEventHandler<T> Match;

#pragma warning disable 1591
        protected virtual void this_Match(object sender, RegexFSMMatchEventArgs<T> e)
        {
            this.matches.Add(e.Match);
        }
#pragma warning restore 1591

        /// <summary>
        /// 引发 <see cref="Match"/> 事件。
        /// </summary>
        /// <param name="e"><see cref="Match"/> 事件的事件参数。</param>
        protected virtual void OnMatch(RegexFSMMatchEventArgs<T> e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            this.Match?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// 初始化 <see cref="RegexFSM{T, TState, TTransition}"/> 类的新实例。
        /// </summary>
        public RegexFSM() : base()
        {
            this.Match += this.this_Match;
        }

        #region 缓存数据
        /// <summary>
        /// <see cref="RegexFSM{T, TState, TTransition}"/> 的捕获栈。
        /// </summary>
        protected CaptureStack<T> captureStack;
        /// <summary>
        /// <see cref="RegexFSM{T, TState, TTransition}"/> 的状态栈
        /// </summary>
        protected StateStack<T, TState, TTransition> stateStack;

        /// <summary>
        /// <see cref="RegexFSM{T, TState, TTransition}"/> 内部的 <see cref="NodeReader{TReader, TNode}"/> 。
        /// </summary>
        private NodeReader<IEnumerable<T>, T> nodeReader;

        /// <summary>
        /// 获取 <see cref="RegexFSM{T, TState, TTransition}"/> 匹配过程的输入对象序列。
        /// </summary>
        public virtual IEnumerable<T> Inputs
        {
            get => this.nodeReader.Reader;
            protected set => this.nodeReader = value?.CreateReader();
        }

        /// <summary>
        /// 获取或（子类）设置 <see cref="RegexFSM{T, TState, TTransition}"/> 匹配过程的在 <see cref="Inputs"/> 中的当前位置。
        /// </summary>
        public virtual int Index
        {
            get => this.nodeReader.Position;
            protected set => this.nodeReader.Position = value;
        }

        private int start;
        /// <summary>
        /// 获取或（子类）设置 <see cref="RegexFSM{T, TState, TTransition}"/> 匹配过程的起始位置。子类设置其值时，将使 <see cref="Index"/> 复位至 <see cref="Start"/> 。
        /// </summary>
        public virtual int Start
        {
            get => this.start;
            protected set => this.start = this.Index = value; // 修改匹配起始位置的同时将当前位置复位至起始位置。
        }

        /// <summary>
        /// 获取 <see cref="RegexFSM{T, TState, TTransition}"/> 下一个接受的对象。
        /// </summary>
        /// <exception cref="InvalidOperationException">内部读取器已抵达结尾。</exception>
        public virtual T NextInput => this.nodeReader.Peek();
        #endregion

        /// <summary>
        /// 记录一个匹配。
        /// </summary>
        /// <param name="captureIDToken">匹配的 ID 标志符。</param>
        /// <param name="id">捕获的 ID 。</param>
        /// <param name="start">捕获的开始位置。</param>
        /// <param name="length">捕获的长度。</param>
        public virtual void Capture(object captureIDToken, object id, int start, int length)
        {
            this.captureStack.Push(new CaptureStack<T>.Item()
            {
                StateStackCount = this.stateStack.Count,
                CaptureIDToken = captureIDToken,
                CaptureID = id,
                CaptureStart = start,
                CaptureLength = length
            });
        }

        /// <summary>
        /// 初始化状态机实例必要字段，进行匹配前准备工作。
        /// </summary>
        protected virtual void BeginMatch(IEnumerable<T> inputs)
        {
            this.Inputs = inputs;
            this.Start = 0;

            if (this.matches.Count != 0)
                this.matches.Clear();

            if (this.captureStack == null)
                this.captureStack = new CaptureStack<T>();
            else if (this.captureStack.Count != 0)
                this.captureStack.Clear();

            if (this.stateStack == null)
                this.stateStack = new StateStack<T, TState, TTransition>() { RelatedCaptureStack = this.captureStack };
            else if (this.stateStack.Count != 0)
                this.stateStack.Clear();
        }

        /// <summary>
        /// 回收状态机实例的缓存，进行匹配后结束工作。
        /// </summary>
        protected virtual void EndMatch()
        {
            this.nodeReader = null;
            this.start = 0;
        }

        #region Transit
        /// <summary>
        /// 接受一个指定输入并进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <typeparam name="TInput">输入的类型。</typeparam>
        /// <param name="input">指定的输入。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <see cref="TransitMany(IEnumerable{T})"/>
        public override bool Transit<TInput>(TInput input)
        {
            if (input is T)
            {
                this.TransitMany(new[] { (T)(object)input });
                return true;
            }
            else if (input is IEnumerable<T>)
            {
                this.TransitMany((IEnumerable<T>)input);
                return true;
            }
            else
                return base.Transit(input);
        }

        /// <summary>
        /// 对 <see cref="RegexFSM{T, TState, TTransition}"/> 的 <see cref="NextInput"/> 进行转换。返回一个值，指示操作是否成功。
        /// </summary>
        /// <param name="inputAccepted">获取一个值，指示 <see cref="NextInput"/> 是否被接受。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        protected virtual bool TransitInternal(out bool inputAccepted)
        {
            IReaderSource<TTransition> transitionReader;
            if (this.stateStack.Count != 0 && object.Equals(this.stateStack.Peek().State, this.CurrentState))
                transitionReader = this.stateStack.Peek().TransitionReader;
            else
            {
                // 获取可以接受输入并进行转换的转换。
                var transitions = this.CurrentState.GetOrderedTransitions();

                transitionReader = transitions.CreateReader(); // 创建转换集的读取源。
                this.stateStack.Push(new StateStack<T, TState, TTransition>.Item()
                {
                    RegexFSMIndex = this.Index,
                    State = this.CurrentState,
                    TransitionReader = transitionReader
                });
            }

            if (transitionReader.HasNext())
            {
                // 获取下一个转换。
                TTransition transition = transitionReader.Read();
                if (transition is IRegexFSMTransitionProxy<T, TState>)
                {
                    inputAccepted = false;
                    return ((IRegexFSMTransitionProxy<T, TState>)transition).TransitProxy(
                        this.nodeReader,
                        (_transition, _args) => this.Transit((TState)_transition.Target, (TTransition)_transition, _args),
                        this
                    ) && this.Transit(transition);
                }
                else if (transition is IAcceptInputTransition<T>)
                    return (inputAccepted = !this.nodeReader.IsEnd() && ((IAcceptInputTransition<T>)transition).CanAccept(this.NextInput)) && this.Transit(transition);
                else
                {
                    inputAccepted = false;
                    return this.Transit(transition);
                }
            }
            else
            { // 当前状态的转换集已遍历结束。
                inputAccepted = false;
                return false;
            }
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

            this.TransitManyInternal();

            this.EndMatch();
        }

        /// <summary>
        /// 子类重写时，提供接受一个指定输入序列并进行一组转换动作的实现。
        /// </summary>
        protected virtual void TransitManyInternal()
        {
            while (true)
            {
                if (this.TransitInternal(out bool inputAccepted))
                { // 转换成功。
                    if (inputAccepted)
                        // 若转换接受字符，则当前位置进一位。
                        this.Index++;
                }
                else
                { // 转换未成功。
                    var stateStackItem = this.stateStack.Peek();
                    this.Index = stateStackItem.RegexFSMIndex; // 确定当前位置。

                    if (!stateStackItem.TransitionReader.HasNext())
                    { // 原因是当前状态的转换集已遍历结束。
                        if (stateStackItem.State.IsTerminal)
                        { // 当前状态是结束状态。
                            // 成功获得一个匹配。
                            this.OnMatch(new RegexFSMMatchEventArgs<T>(
                                new Match<T>(this.Inputs, this.Start, this.Index - this.Start,
                                    this.captureStack.Reverse()
                                        .GroupBy(
                                            (captureInfo =>
                                            {
                                                (object id, object idToken) groupKey = (captureInfo.CaptureID, captureInfo.CaptureIDToken);
                                                return groupKey;
                                            }),
                                            (captureInfo => (captureInfo.CaptureStart, captureInfo.CaptureLength)),
                                            new EqualityComparisonComparer<(object id, object idToken)>((x, y) =>
                                            {
                                                return object.Equals(x.id, y.id) && object.Equals(x.idToken, y.idToken);
                                            })
                                        )
                                        .Select(group =>
                                        {
                                            var captures = group.ToArray();
                                            return (group.Key.id, new Extend.Group<T>(this.Inputs, captures));
                                        })
                                )
                            ));

                            this.Start = this.Index;

                            // 清空堆栈。
                            this.stateStack.Clear();
                            this.Reset(); // 复位状态机。
                        }
                        else
                        { // 当前状态不是结束状态。
                            this.stateStack.Pop(); // 当前状态的所有转换都无法转换，退回上一个状态。

                            if (this.stateStack.Count == 0)
                            { // 包括起始状态的所有状态都被回溯。此次匹配失败，准备从下一个起始位置开始新一轮匹配。
                                // 清空堆栈。
                                this.stateStack.Clear();

                                // 复位状态机。
                                this.Reset();

                                if (this.nodeReader.IsEnd())
                                    // 若内部读取器已经到结尾。无法继续匹配。
                                    break;
                                else
                                    // 设置下一个起始位置。
                                    this.Start++;
                            }
                            else
                            {
                                // 回溯当前位置到上一个状态时的位置。
                                this.Index = this.stateStack.Peek().RegexFSMIndex;
                                this.Transit(this.stateStack.Peek().State);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 状态机服务
        #region Services
        /// <summary>
        /// 表示回溯服务。 <see cref="RegexFSM{T, TState, TTransition}"/> 可通过记录时间点、回溯到指定时间点来实现功能。
        /// </summary>
        public class BackTraceService : RegexFSMService<T, RegexFSM<T, TState, TTransition>>
        {
            /// <summary>
            /// 提供此服务的 <see cref="RegexFSM{T, TState, TTransition}"/> 。
            /// </summary>
            protected RegexFSM<T, TState, TTransition> fsm;

            /// <summary>
            /// 将 <see cref="BackTraceService"/> 服务连接到 <see cref="RegexFSM{T, TState, TTransition}"/> 。
            /// </summary>
            /// <param name="fsm">准备被连接的 <see cref="RegexFSM{T, TState, TTransition}"/> 。</param>
            /// <exception cref="ArgumentNullException"><paramref name="fsm"/> 的值为 null 。</exception>
            public override void Connect(RegexFSM<T, TState, TTransition> fsm)
            {
                if (fsm == null) throw new ArgumentNullException(nameof(fsm));

                this.fsm = fsm;
            }

            /// <summary>
            /// 获取当前的时间点。
            /// </summary>
            /// <returns>当前的时间点。</returns>
            public Timepoint GetTimepoint()
            {
                return new Timepoint(this);
            }

            /// <summary>
            /// 回溯到指定的时间点，并根据指定值判断是否移除期间的所有捕获。
            /// </summary>
            /// <param name="timepoint">指定的时间点。</param>
            /// <param name="removeCaptures">一个值，指示是否移除期间的所有捕获。</param>
            /// <exception cref="ArgumentNullException"><paramref name="timepoint"/> 的值为 null 。</exception>
            public void BackTrace(Timepoint timepoint, bool removeCaptures)
            {
                if (timepoint == null) throw new ArgumentNullException(nameof(timepoint));

                this.fsm.Index = timepoint.FSMIndex;

                if (removeCaptures)
                {
                    // 回溯状态栈。
                    while (this.fsm.stateStack.Count > timepoint.StateStackCount)
                        this.fsm.stateStack.Pop();
                }
                else
                {
                    var relatedCaptureStack = this.fsm.stateStack.RelatedCaptureStack;
                    this.fsm.stateStack.RelatedCaptureStack = null; // 暂时取消关联。
                    // 回溯状态栈。
                    while (this.fsm.stateStack.Count > timepoint.StateStackCount)
                        this.fsm.stateStack.Pop();
                    this.fsm.stateStack.RelatedCaptureStack = relatedCaptureStack;

                    // 更新捕获栈的项的必要参数。
                    foreach (var item in this.fsm.captureStack.Where(_item => _item.StateStackCount > timepoint.StateStackCount))
                        item.StateStackCount = timepoint.StateStackCount;
                }
            }

            /// <summary>
            /// 表示时间点。
            /// </summary>
            public class Timepoint
            {
                /// <summary>
                /// 记录此时间点时 <see cref="RegexFSM{T, TState, TTransition}"/> 的 <see cref="RegexFSM{T, TState, TTransition}.Index"/> 。
                /// </summary>
                public int FSMIndex { get; private set; }
                /// <summary>
                /// 记录此时间点时 <see cref="RegexFSM{T, TState, TTransition}.stateStack"/> 的 <see cref="StateStack{T, TState, TTransition}.Count"/> 。
                /// </summary>
                public int StateStackCount { get; private set; }

                /// <summary>
                /// 初始化 <see cref="Timepoint"/> 类的新实例。
                /// </summary>
                /// <param name="service">记录此时间点的服务。</param>
                /// <exception cref="ArgumentNullException"><paramref name="service"/> 的值为 null 。</exception>
                protected internal Timepoint(BackTraceService service)
                {
                    if (service == null) throw new ArgumentNullException(nameof(service));

                    this.FSMIndex = service.fsm.Index;
                    this.StateStackCount = service.fsm.stateStack.Count;
                }
            }
        }

        /// <summary>
        /// 表示进度服务。 <see cref="RegexFSM{T, TState, TTransition}"/> 可通过记录进度、回溯到指定进度来实现功能。
        /// </summary>
        public class ProgressService : RegexFSMService<T, RegexFSM<T, TState, TTransition>>
        {
            /// <summary>
            /// 提供此服务的 <see cref="RegexFSM{T, TState, TTransition}"/> 。
            /// </summary>
            protected internal RegexFSM<T, TState, TTransition> fsm;

            /// <summary>
            /// 将 <see cref="ProgressService"/> 服务连接到 <see cref="RegexFSM{T, TState, TTransition}"/> 。
            /// </summary>
            /// <param name="fsm">准备被连接的 <see cref="RegexFSM{T, TState, TTransition}"/> 。</param>
            /// <exception cref="ArgumentNullException"><paramref name="fsm"/> 的值为 null 。</exception>
            public override void Connect(RegexFSM<T, TState, TTransition> fsm)
            {
                if (fsm == null) throw new ArgumentNullException(nameof(fsm));

                this.fsm = fsm;
            }

            /// <summary>
            /// 记录当前的进度。
            /// </summary>
            /// <returns>当前的进度。</returns>
            public Progress GetProgress()
            {
                return new Progress(this);
            }

            /// <summary>
            /// 设置到指定的进度。
            /// </summary>
            /// <param name="progress">指定的进度</param>
            /// <exception cref="ArgumentNullException"><paramref name="progress"/> 的值为 null 。</exception>
            public void SetProgress(Progress progress)
            {
                if (progress == null) throw new ArgumentNullException(nameof(progress));

                this.fsm.Index = progress.Value;
            }

            /// <summary>
            /// 表示进度。
            /// </summary>
            public class Progress
            {
                /// <summary>
                /// 记录此进度时 <see cref="RegexFSM{T, TState, TTransition}"/> 的 <see cref="RegexFSM{T, TState, TTransition}.Index"/> 。
                /// </summary>
                public int Value { get; private set; }

                /// <summary>
                /// 初始化 <see cref="Progress"/> 类的新实例。
                /// </summary>
                /// <param name="service">记录此进度的服务。</param>
                /// <exception cref="ArgumentNullException"><paramref name="service"/> 的值为 null 。</exception>
                protected internal Progress(ProgressService service)
                {
                    if (service == null) throw new ArgumentNullException(nameof(service));

                    this.Value = service.fsm.Index;
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取 <see cref="RegexFSM{T, TState, TTransition}"/> 的服务。
        /// </summary>
        /// <typeparam name="TService">服务的类型。</typeparam>
        /// <returns><see cref="RegexFSM{T, TState, TTransition}"/> 的指定类型的服务。</returns>
        public virtual TService GetService<TService>()
            where TService : IRegexFSMService<T, TState, TTransition>, new()
        {
            TService service = new TService();
            service.Connect(this);

            return service;
        }
        #endregion

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

        TService IRegexFSM<T>.GetService<TService>()
        {
            if (typeof(IRegexFSMService<T, TState, TTransition>).IsAssignableFrom(typeof(TService)))
                return (TService)(
                    typeof(IRegexFSM<T, TState, TTransition>)
                        .GetMethod(nameof(this.GetService), Type.EmptyTypes) // 获取 IRegexFSM{T, TState, TTransition}.GetService{TService} 方法。
                        .MakeGenericMethod(typeof(TService))
                        .Invoke(this, null) // 调用方法。
                );
            else
            {
                TService service = new TService();
                service.Connect(this);

                return service;
            }
        }
        #endregion
    }
}
