using SamLu.Diagnostics;
using SamLu.IO;
using SamLu.RegularExpression.Diagnostics;
using SamLu.Runtime;
using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    /// <summary>
    /// 表示正则构造的有限状态机的条件功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMPredicateTransitionDebugInfo<>),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public class RegexFSMPredicateTransition<T> : RegexFSMFunctionalTransition<T>, IRegexFSMTransitionProxy<T>
    {
        protected Func<object, object[], bool> predicate;

        [RegexFSMFunctionalTransitionMetadata]
        public Func<object, object[], bool> Predicate => this.predicate;

        /// <summary>
        /// 初始化 <see cref="RegexFSMPredicateTransition{T}"/> 类的新实例。子类默认调用此构造函数。
        /// </summary>
        protected RegexFSMPredicateTransition() { }

        public RegexFSMPredicateTransition(Func<object, object[], bool> predicate) : this() =>
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        #region IRegexFSMTransitionProxy{T} Implementation
        bool IRegexFSMTransitionProxy<T>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T> handler, params object[] args) => this.Predicate(this, args);
        #endregion
    }

    /// <summary>
    /// 表示正则构造的有限状态机的条件功能转换。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    [DebugInfoProxy(
        typeof(RegexFSMPredicateTransitionDebugInfo<,>),
        new[] { TypeParameterFillin.TypeParameter_1, TypeParameterFillin.TypeParameter_2 }
    )]
    public class RegexFSMPredicateTransition<T, TState> : RegexFSMFunctionalTransition<T, TState>, IRegexFSMTransitionProxy<T, TState>
        where TState : IRegexFSMState<T>
    {
        protected Func<object, object[], bool> predicate;

        [RegexFSMFunctionalTransitionMetadata]
        public Func<object, object[], bool> Predicate => this.predicate;

        /// <summary>
        /// 初始化 <see cref="RegexFSMPredicateTransition{T, TState}"/> 类的新实例。子类默认调用此构造函数。
        /// </summary>
        protected RegexFSMPredicateTransition() { }

        public RegexFSMPredicateTransition(Func<object, object[], bool> predicate) : this() =>
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

        #region IRegexFSMTransitionProxy{T}/IRegexFSMTransitionProxy{T, TState} Implementation
        bool IRegexFSMTransitionProxy<T>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T> handler, params object[] args) => this.Predicate(this, args);

        bool IRegexFSMTransitionProxy<T, TState>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T, TState> handler, params object[] args) => this.Predicate(this, args);
        #endregion
    }
}
