using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.IO;
using SamLu.RegularExpression.Diagnostics;
using SamLu.Diagnostics;
using SamLu.Runtime;

namespace SamLu.RegularExpression.StateMachine.FunctionalTransitions
{
    [DebugInfoProxy(
        typeof(RegexFSMLastCaptureReferenceTransition<>._DebugInfo),
        new[] { TypeParameterFillin.TypeParameter_1 }
    )]
    public sealed class RegexFSMLastCaptureReferenceTransition<T> : RegexFSMFunctionalTransition<T>, IRegexFSMTransitionProxy<T>
    {
        private object idToken;
        private object id;
        private Func<RegexFSMLastCaptureReferenceTransition<T>, IReaderSource<T>, RegexFSMTransitProxyHandler<T>, object[], bool> predicate;

        [RegexFSMFunctionalTransitionMetadata]
        public object IDToken => this.idToken;

        [RegexFSMFunctionalTransitionMetadata]
        public object ID => this.id;

        [RegexFSMFunctionalTransitionMetadata]
        public Func<RegexFSMLastCaptureReferenceTransition<T>, IReaderSource<T>, RegexFSMTransitProxyHandler<T>, object[], bool> Predicate => this.predicate;

        private RegexFSMLastCaptureReferenceTransition(object idToken, object id)
        {
            this.idToken = idToken;
            this.id = id;
        }

        public RegexFSMLastCaptureReferenceTransition(object idToken, object id, Func<RegexFSMLastCaptureReferenceTransition<T>, IReaderSource<T>, RegexFSMTransitProxyHandler<T>, object[], bool> predicate) : this(idToken, id)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }

        #region IRegexFSMTransitionProxy{T} Implementation
        bool IRegexFSMTransitionProxy<T>.TransitProxy(IReaderSource<T> readerSource, RegexFSMTransitProxyHandler<T> handler, params object[] args)
        {
            return this.Predicate(this, readerSource, handler, args);
        }
        #endregion

        /// <summary>
        /// 为 <see cref="RegexFSMLastCaptureReferenceTransition{T}"/> 提供调试信息。
        /// </summary>
        internal sealed class _DebugInfo : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMLastCaptureReferenceTransition<T>>
        {
            /// <summary>
            /// 获取 <see cref="RegexFSMLastCaptureReferenceTransition{T}"/> 的显式名称。
            /// </summary>
            protected override string Name => "reference";

            /// <summary>
            /// 获取 <see cref="RegexFSMLastCaptureReferenceTransition{T}"/> 的显式参数序列。
            /// </summary>
            protected override IEnumerable<string> Parameters => new[] { $"id = {{{this.functionalTransition.ID}}}" };

            /// <summary>
            /// 使用规范参数列表初始化 <see cref="_DebugInfo"/> 类的新实例。
            /// </summary>
            /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
            /// <param name="args">获取调试信息的参数列表。</param>
            public _DebugInfo(RegexFSMLastCaptureReferenceTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
        }
    }
}
