using SamLu.IO;
using SamLu.RegularExpression.Extend;
using SamLu.RegularExpression.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SamLu.RegularExpression
{
    public class RegexProvider<T>
    {
        private RegexObject<T> pattern;
        private RegexOptions options;

        public RegexOptions Options => this.options;
        public bool RightToLeft => this.Options.RightToLeft;

        /// <summary>
        /// 全局缓存字典。
        /// </summary>
        private Dictionary<object, object> globalCacheDic = new Dictionary<object, object>()
        {
            { Cache.CAPTURES_CACHE_KEY, new List<(Capture<T>,object)>() },
            { Cache.PREVIEW_MATCH_CACHE_KEY, null },
            { Cache.BALANCE_GROUP_CACHE_KEY, new Dictionary<RegexBalanceGroup<T>, Stack<object>>() },
            { Cache.REGEX_GROUPS_CACHE_KEY, new Stack<RegexGroup<T>>() }
        };

        private List<(Capture<T> capture, object id)> captures = new List<(Capture<T> capture, object id)>();

        public RegexProvider(RegexObject<T> pattern) : this(pattern, RegexOptions.None) { }

        public RegexProvider(RegexObject<T> pattern, RegexOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (options == null) throw new ArgumentNullException(nameof(options));

            this.pattern = pattern;
            this.options = options;
        }

        #region Match/Matches
        public virtual Match<T> Match(IEnumerable<T> input) =>
            this.MatchesInternal(NodeReader.CreateReader(input)).FirstOrDefault() ?? Match<T>.Empty;

        public virtual Match<T> Match(IEnumerable<T> input, int start, int length) =>
            this.Match(input.Skip(start).Take(length));

        public virtual MatchCollection<T> Matches(IEnumerable<T> input) =>
            new MatchCollection<T>(this.MatchesInternal(NodeReader.CreateReader(input)));

        public virtual MatchCollection<T> Matches(IEnumerable<T> input, int start, int length) =>
            this.Matches(input.Skip(start).Take(length));

        //public virtual bool IsMatch(IEnumerable<T> input) { }

        public virtual bool TryMatch(IEnumerable<T> input, out Match<T> result)
        {
            result = null;

            try { result = this.Match(input); }
            catch (Exception) { return false; }

            return true;
        }

        public virtual bool TryMatches(IEnumerable<T> input, out MatchCollection<T> result)
        {
            result = null;

            try { result = this.Matches(input); }
            catch (Exception) { return false; }

            return true;
        }
        #endregion

        #region MatchesInternal
        protected virtual IEnumerable<Match<T>> MatchesInternal(NodeReader<IEnumerable<T>, T> reader)
        {
            while (!reader.IsEnd())
            {
                int prePosition = reader.Position;
                if (this.MatchesInternal(
                    this.pattern, reader,
                    new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                    out bool isEnd)
                )
                {
                    int curPositon = reader.Position;
                    var groups = this.captures
                        .GroupBy(
                            (capture => capture.id),
                            (capture => capture.capture),
                            new EqualityComparisonComparer<object>((x, y) =>
                            {
                                if (x == null && y == null) return false;
                                else return object.Equals(x, y);
                            })
                        )
                        .OrderBy(group =>
                            group
                                .Select(capture =>
                                    this.captures.FindIndex(item =>
                                        item.capture == capture && item.id == group.Key
                                    )
                                )
                                .Min()
                        )
                        .Select(group =>
                            (group.Key, new Group<T>(
                                reader.Reader,
                                group
                                    .AsEnumerable()
                                    .Distinct(new EqualityComparisonComparer<Capture<T>>((x, y) =>
                                        x.Index == y.Index && x.Length == y.Length
                                    ))
                                    .Select(capture => (capture.Index, capture.Length))
                                    .ToArray()
                            ))
                        );
                    var match = new Match<T>(reader.Reader, prePosition, curPositon, groups);
                    this.globalCacheDic[Cache.PREVIEW_MATCH_CACHE_KEY] = match;
                    yield return match;
                    continue;
                }
                else reader.Position++;

                if (isEnd)
                {
                    this.captures.Clear();
                    yield break;
                }
            }
        }

        public delegate bool RegexMatchesHandler<in TRegexObject>(TRegexObject regex, NodeReader<IEnumerable<T>, T> reader, Cache cache);

        private Dictionary<Type, Delegate> handlers;

        protected bool MatchesInternal<TObj>(TObj obj, NodeReader<IEnumerable<T>, T> reader, out bool isEnd) =>
            this.MatchesInternal((object)obj, reader, out isEnd);

        protected bool MatchesInternal(object obj, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            Type type = obj.GetType();
            if (this.handlers.ContainsKey(type))
                return this.MatchesInternal(obj, reader, this.handlers[type], out isEnd);
            else
                throw new NotSupportedException();
        }

        private bool MatchesInternal(object obj, NodeReader<IEnumerable<T>, T> reader, Delegate handler, out bool isEnd) =>
            this.MatchesInternal((cache => this.GetResultEnumerable(obj, reader, handler, cache)), out isEnd);

        private bool MatchesInternal(Func<Cache, IEnumerable<bool>> resultEnumerableFunc, out bool isEnd)
        {
            Cache cache = new Cache(this);
            /* 重构（提取方法）目标：获取对象是否匹配。 */
            bool result = resultEnumerableFunc(cache).LastOrDefault(_result => _result); // 获取最后一个指示匹配成功的提示值；若全为匹配失败则返回默认值（匹配不成功）。

            isEnd = cache.IsEnd;
            if (result)
            { // 当前栈项处理完毕，且结果为匹配。
              /* 将部分必要的数据从本地缓存转移到全局缓存中。 */
                cache.CopyCacheFromLocalToGlobal(
                    (localDic, globalDic) =>
                    {
                        if (localDic.ContainsKey(Cache.CAPTURES_CACHE_KEY))
                        {
                            var globalCaptures = (List<(Capture<T>, object)>)globalDic[Cache.CAPTURES_CACHE_KEY];
                            foreach (var capture in (IEnumerable<(Capture<T> capture, object id)>)localDic[Cache.CAPTURES_CACHE_KEY])
                                globalCaptures.Add(capture);
                        }
                    }
                );

                return true;
            }
            else
            { // 当前栈项处理完毕，且结果为不匹配。
                return false;
            }
        }

        private IEnumerable<bool> GetResultEnumerable(object obj, NodeReader<IEnumerable<T>, T> reader, Delegate handler, Cache cache)
        {
            bool result;
            // 在下一个栈项的处理结果为不匹配且栈项缓存指示未处理完毕的前提下——
            while (!(result = (bool)handler.DynamicInvoke(obj, reader, cache)) && !cache.Handled)
                yield return result; // 永远为 false 。
        }

        #region Register/Deregister Handler
        protected bool RegisterHandler<TRegexObject>(RegexMatchesHandler<TRegexObject> handler)
            where TRegexObject : RegexObject<T>
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            return this.RegisterHandlerInternal(typeof(TRegexObject), handler);
        }

        protected bool RegisterHandler<TObj>(Func<TObj, NodeReader<IEnumerable<T>, T>, Cache, bool> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            return this.RegisterHandlerInternal(typeof(TObj), handler);
        }

        protected bool RegisterHandler(Delegate handler)
        {
            var method = handler.Method;
            if (method.ReturnType == typeof(bool))
            {
                Type[] parameterTypes = method.GetParameters().Select(pi => pi.ParameterType).ToArray();
                if (parameterTypes.Length == 3 &&
                    parameterTypes.SequenceEqual(
                        new Type[] { typeof(object), typeof(NodeReader<IEnumerable<T>, T>), typeof(Cache) },
                        new EqualityComparisonComparer<Type>((x, y) => x.IsAssignableFrom(y))
                    )
                )
                    return this.RegisterHandlerInternal(parameterTypes[0], handler);
            }

            throw new ArgumentOutOfRangeException(nameof(handler), handler, "意外的方法签名。");
        }

        private bool RegisterHandlerInternal(Type type, Delegate handler)
        {
            if (this.handlers.ContainsKey(type))
                return false;
            else
            {
                this.handlers.Add(type, handler);
                return true;
            }
        }

        protected bool DeregisterHandler<TRegexObject>() where TRegexObject : RegexObject<T> =>
            this.DeregisterHandlerInternal(typeof(TRegexObject));

        protected bool DeregisterHandler(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return this.DeregisterHandlerInternal(type);
        }

        private bool DeregisterHandlerInternal(Type type)
        {
            if (this.handlers.ContainsKey(type))
            {
                this.handlers.Remove(type);
                return true;
            }
            else
                return false;
        }
        #endregion

        protected virtual bool RegexObjectMatchesInternal(RegexObject<T> regex, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            bool result;
            bool isEnd;

            if (regex is IRegexAnchorPoint<T> anchorPoint)
                result = this.MatchesInternal(
                    anchorPoint,
                    reader,
                    new Func<IRegexAnchorPoint<T>, NodeReader<IEnumerable<T>, T>, Cache, bool>(this.IRegexAnchorPointMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexGroup<T> group)
                result = this.MatchesInternal(
                    group,
                    reader,
                    new RegexMatchesHandler<RegexGroup<T>>(this.RegexGroupMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexGroupReference<T> groupReference)
                result = this.MatchesInternal(
                    groupReference,
                    reader,
                    new RegexMatchesHandler<RegexGroupReference<T>>(this.RegexGroupReferenceMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexMultiBranch<T> multiBranch)
                result = this.MatchesInternal(
                    multiBranch,
                    reader,
                    new RegexMatchesHandler<RegexMultiBranch<T>>(this.RegexMultiBranchMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexCondition<T> condition)
                result = this.MatchesInternal(
                    condition,
                    reader,
                    new RegexMatchesHandler<RegexCondition<T>>(this.RegexConditionMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexRepeat<T> repeat)
                result = this.MatchesInternal(
                    repeat,
                    reader,
                    new RegexMatchesHandler<RegexRepeat<T>>(this.RegexRepeatMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexNonGreedyRepeat<T> nonGreedyRepeat)
                result = this.MatchesInternal(
                    nonGreedyRepeat,
                    reader,
                    new RegexMatchesHandler<RegexNonGreedyRepeat<T>>(this.RegexNonGreedyRepeatMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexParallels<T> parallels)
                result = this.MatchesInternal(
                    parallels,
                    reader,
                    new RegexMatchesHandler<RegexParallels<T>>(this.RegexParallelsMatchesInternal),
                    out isEnd
                );
            else if (regex is RegexSeries<T> series)
                result = this.MatchesInternal(
                    series,
                    reader,
                    new RegexMatchesHandler<RegexSeries<T>>(this.RegexSeriesMatchInternal),
                    out isEnd
                );
            else
                throw new NotSupportedException();

            cache.IsEnd = isEnd;
            return result;
        }

        protected virtual bool RegexConditionMatchesInternal(RegexCondition<T> condition, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            cache.Handled = true;

            if (reader.IsEnd())
            {
                cache.IsEnd = true;
                return false;
            }
            else
            {
                var t = reader.Read();
                cache.IsEnd = reader.IsEnd();
                return condition.Condition(t);
            }
        }

        private bool Regex__RepeatMatchesInternal(RegexRepeat<T> repeat, NodeReader<IEnumerable<T>, T> reader, bool isGreedy, Cache cache)
        {
            const string REPEATCOUNT_CACHE_KEY = "REPEATCOUNT";
            uint? preRepeatCountCache;
            if (cache.ContainsKey(REPEATCOUNT_CACHE_KEY))
                preRepeatCountCache = (uint)cache[REPEATCOUNT_CACHE_KEY];
            else
                cache.Add(REPEATCOUNT_CACHE_KEY, preRepeatCountCache = null);

            int prePosition = reader.Position;
            uint repeatCount = 0;
            bool result;
            bool isEnd = reader.IsEnd();
            if (isGreedy)
            {
                if (repeat.IsInfinte)
                {
                    while (
                        !(repeatCount >= preRepeatCountCache) &&
                        this.MatchesInternal(repeat.InnerRegex, reader,
                            new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                            out isEnd
                        )
                    )
                        repeatCount++;
                }
                else
                {
                    while (
                        (
                            repeatCount < repeat.MaximumCount.Value &&
                            !(repeatCount >= preRepeatCountCache.Value)
                        ) &&
                        this.MatchesInternal(repeat.InnerRegex, reader,
                            new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                            out isEnd
                        )
                    )
                        repeatCount++;
                }

                if ((repeatCount >= (repeat.MinimumCount ?? uint.MinValue)) && !(repeatCount >= preRepeatCountCache))
                {
                    cache[REPEATCOUNT_CACHE_KEY] = repeatCount;
                    cache.Handled = repeatCount == (repeat.MinimumCount ?? uint.MinValue);
                    cache.IsEnd = isEnd;

                    return true;
                }
            }
            else
            {
                while (
                    !(repeatCount >= repeat.MaximumCount) &&
                    this.MatchesInternal(repeat.InnerRegex, reader,
                        new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                        out isEnd
                    )
                )
                {
                    repeatCount++;

                    if (
                        repeatCount >= (repeat.MinimumCount ?? uint.MinValue) &&
                        !(repeatCount <= preRepeatCountCache)
                    )
                        break;
                }

                if ((repeatCount >= (repeat.MinimumCount ?? uint.MinValue)) && !(repeatCount <= preRepeatCountCache))
                {
                    cache[REPEATCOUNT_CACHE_KEY] = repeatCount;
                    cache.Handled = !(repeatCount != repeat.MaximumCount);
                    cache.IsEnd = isEnd;

                    return true;
                }
            }

            // 不匹配情况：
            reader.Position = prePosition;
            cache.Handled = true;
            cache.IsEnd = reader.IsEnd();

            return false;
        }

        protected virtual bool RegexRepeatMatchesInternal(RegexRepeat<T> repeat, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            return this.Regex__RepeatMatchesInternal(repeat, reader, true, cache);
        }

        protected virtual bool RegexNonGreedyRepeatMatchesInternal(RegexNonGreedyRepeat<T> nonGreedyRepeat, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            return this.Regex__RepeatMatchesInternal(nonGreedyRepeat.InnerRepeat, reader, false, cache);
        }

        protected virtual bool RegexParallelsMatchesInternal(RegexParallels<T> parallels, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            const string PARALLELS_ENUMERATOR_KEY = "PARALLELS_ENUMERATOR";
            IReaderSource<RegexObject<T>> readerSource;
            if (cache.ContainsKey(PARALLELS_ENUMERATOR_KEY))
                readerSource = (IReaderSource<RegexObject<T>>)cache[PARALLELS_ENUMERATOR_KEY];
            else
                readerSource = parallels.Parallels.CreateReader();

            if (readerSource.HasNext())
            {
                var regex = readerSource.Read();

                bool result = this.MatchesInternal(regex, reader,
                    new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                    out bool isEnd
                );
                cache.Handled = false;
                cache.IsEnd = isEnd;

                return result;
            }
            else
            {
                cache.Handled = true;
                cache.IsEnd = reader.IsEnd();

                return false;
            }
        }

        protected virtual bool RegexSeriesMatchInternal(RegexSeries<T> series, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            bool result = this.MatchesInternal(
                (_cache => this.GetRegexSeriesResultEnumerable(series.Series.ToArray(), 0, reader, _cache)),
                out bool isEnd
            );
            cache.IsEnd = isEnd;
            return result;
        }

        private IEnumerable<bool> GetRegexSeriesResultEnumerable(RegexObject<T>[] series, int index, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (index < series.Length)
            {
                foreach (var result in this.GetResultEnumerable(series[index], reader, new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal), cache))
                {
                    if (result)
                    { // 串联正则的当前项匹配成功。
                        Cache _cache = new Cache(this);
                        foreach (var _result in GetRegexSeriesResultEnumerable(series, index + 1, reader, _cache))
                            yield return _result;
                    }
                    else
                        // 串联正则的当前项匹配不成功。
                        yield return false;
                }
            }
            else
                // 串联正则匹配成功到最后一项。
                yield return true;
        }

        protected virtual bool IRegexAnchorPointMatchesInternal(IRegexAnchorPoint<T> anchorPoint, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            int prePosition = reader.Position;

            bool isEnd;
            bool result;
            if (anchorPoint is RegexZeroLengthObject<T> zeroLength)
                result = this.MatchesInternal(
                    zeroLength,
                    reader,
                    new RegexMatchesHandler<RegexZeroLengthObject<T>>(this.RegexZeroLengthObjectMatchesInternal),
                    out isEnd);
            else if (anchorPoint is RegexStartBorder<T> startBorder)
                result = this.MatchesInternal(
                    startBorder,
                    reader,
                    new RegexMatchesHandler<RegexStartBorder<T>>(this.RegexStartBorderMatchesInternal),
                    out isEnd
                );
            else if (anchorPoint is RegexEndBorder<T> endBorder)
                result = this.MatchesInternal(
                    endBorder,
                    reader,
                    new RegexMatchesHandler<RegexEndBorder<T>>(this.RegexEndBorderMatchesInternal),
                    out isEnd
                );
            else if (anchorPoint is RegexPreviousMatch<T> preMatch)
                result = this.MatchesInternal(
                    preMatch,
                    reader,
                    new RegexMatchesHandler<RegexPreviousMatch<T>>(this.RegexPreviewMatchMatchesInternal),
                    out isEnd
                );
            else
                throw new NotSupportedException();

            if (reader.Position != prePosition)
                reader.Position = prePosition;

            cache.IsEnd = isEnd;
            return result;
        }

        protected virtual bool RegexZeroLengthObjectMatchesInternal(RegexZeroLengthObject<T> zeroLength, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            bool result = this.MatchesInternal(
                zeroLength.InnerRegex, reader,
                new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                out bool isEnd
            );
            cache.Handled = true;
            cache.IsEnd = isEnd;

            return result;
        }

        protected virtual bool RegexStartBorderMatchesInternal(RegexStartBorder<T> startBorder, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            cache.IsEnd = reader.IsEnd();
            cache.Handled = true;
            return reader.Position == 0;
        }

        protected virtual bool RegexEndBorderMatchesInternal(RegexEndBorder<T> endBorder, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (!reader.IsEnd())
            {
                reader.Position++;
                if (reader.IsEnd())
                {
                    cache.IsEnd = true;
                    cache.Handled = true;
                    return true;
                }
                else
                {
                    cache.IsEnd = false;
                    cache.Handled = true;
                    reader.Rollback(1);
                    return false;
                }
            }
            else
            {
                cache.IsEnd = true;
                cache.Handled = true;
                return false;
            }
        }

        protected virtual bool RegexPreviewMatchMatchesInternal(RegexPreviousMatch<T> preMatch, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            Match<T> previewMatchCache;
            if (cache.ContainsKey(Cache.PREVIEW_MATCH_CACHE_KEY))
                previewMatchCache = (Match<T>)cache[Cache.PREVIEW_MATCH_CACHE_KEY];
            else
                previewMatchCache = null;

            bool result;
            if (previewMatchCache == null)
                result = true;
            else
                result = reader.Position == previewMatchCache.Index + previewMatchCache.Length;

            cache.IsEnd = reader.IsEnd();
            cache.Handled = true;
            return result;
        }

        protected virtual bool RegexGroupMatchesInternal(RegexGroup<T> group, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            var regexGroups = (Stack<RegexGroup<T>>)cache[Cache.REGEX_GROUPS_CACHE_KEY];

            if (group.IsCaptive) regexGroups.Push(group);

            int prePosition = reader.Position;
            bool isEnd;
            bool result;
            if (group is RegexBalanceGroup<T> balanceGroup)
                result = this.MatchesInternal(
                    balanceGroup,
                    reader,
                    new RegexMatchesHandler<RegexBalanceGroup<T>>(this.RegexBalanceGroupMatchesInternal),
                    out isEnd
                );
            else if (group is RegexBalanceGroupItem<T> balanceGroupItem)
                result = this.MatchesInternal(
                    balanceGroupItem,
                    reader,
                    new RegexMatchesHandler<RegexBalanceGroupItem<T>>(this.RegexBalanceGroupItemMatchesInternal),
                    out isEnd
                );
            else
                result = this.MatchesInternal(
                    group.InnerRegex,
                    reader,
                    new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                    out isEnd
                );

            if (result && group.IsCaptive)
            {
                cache.Capture(new Capture<T>(reader.Reader, prePosition, reader.Position - prePosition + 1), group.ID);
            }

            if (group.IsCaptive) regexGroups.Pop();
            
            return result;
        }

        protected virtual bool RegexBalanceGroupMatchesInternal(RegexBalanceGroup<T> balanceGroup, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)cache[Cache.BALANCE_GROUP_CACHE_KEY];

            bool f;
            if (f = !balanceGroupCache.ContainsKey(balanceGroup))
                balanceGroupCache.Add(balanceGroup, new Stack<object>());

            bool result = this.MatchesInternal(
                balanceGroup.InnerRegex,
                reader,
                new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                out bool isEnd
            );

            if (f)
                balanceGroupCache.Remove(balanceGroup);

            cache.Handled = true;
            cache.IsEnd = isEnd;
            return result;
        }

        private bool RegexBalanceGroup__ItemMatchesInternal(Type __itemTypeDefinition, string handlerName, RegexBalanceGroupItem<T> item, NodeReader<IEnumerable<T>, T> reader, Cache cache, out bool result)
        {
            Type itemType = item.GetType();
            IEnumerable<Type> sourceTypes;
            if (__itemTypeDefinition.IsInterface)
                sourceTypes = itemType.GetInterfaces();
            else
            {
                List<Type> baseTypes = new List<Type> { itemType };
                for (Type type = itemType; type.BaseType != typeof(object); type = type.BaseType) baseTypes.Add(itemType);
                sourceTypes = baseTypes;
            }
            Type supportedType = sourceTypes.FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == __itemTypeDefinition);
            if (supportedType != null)
            { // 存在指定的类型。
                var method = itemType.GetMethod(handlerName, BindingFlags.IgnoreCase | BindingFlags.NonPublic).MakeGenericMethod(supportedType.GetGenericArguments());
                var handler = method.CreateDelegate(typeof(RegexMatchesHandler<>).MakeGenericType(supportedType), method.IsStatic ? null : this);
                result = this.MatchesInternal(item, reader, handler, out bool isEnd);

                cache.IsEnd = isEnd;
                cache.Handled = true;
                return true;
            }
            else
            { // 不存在指定的类型。
                result = false;
                cache.IsEnd = reader.IsEnd();
                cache.Handled = true;

                return false;
            }
        }

        protected virtual bool RegexBalanceGroupItemMatchesInternal(RegexBalanceGroupItem<T> balanceGroupItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            cache.Handled = true;

            bool result;
            if (
                this.RegexBalanceGroup__ItemMatchesInternal(
                    typeof(RegexBalanceGroupOpenItem<,>),
                    nameof(this.RegexBalanceGroupOpenItemMatchesInternal),
                    balanceGroupItem, reader, cache, out result
                ) ||
                this.RegexBalanceGroup__ItemMatchesInternal(
                    typeof(RegexBalanceGroupSubItem<,>),
                    nameof(this.RegexBalanceGroupSubItemMatchesInternal),
                    balanceGroupItem, reader, cache, out result
                ) ||
                this.RegexBalanceGroup__ItemMatchesInternal(
                    typeof(RegexBalanceGroupCloseItem<,>),
                    nameof(this.RegexBalanceGroupCloseItemMatchesInternal),
                    balanceGroupItem, reader, cache, out result
                )
            )
                cache.IsEnd = reader.IsEnd();
            else
            {
                bool isEnd;
                if (this.MatchesInternal(balanceGroupItem.InnerRegex,reader,
                    new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                    out isEnd
                ))
                {
                    if (balanceGroupItem.Method.Method.ReturnType == typeof(bool))
                    {
                        result = (bool)balanceGroupItem.Method.DynamicInvoke(
                            balanceGroupItem.Method.Method.GetParameters().Select(pi => pi.GetType())
                            .Select(type =>
                                (object)(type.IsValueType ? Activator.CreateInstance(type, Type.EmptyTypes) : null)
                            )
                            .ToArray()
                        );
                    }
                    else result = false;
                }
                else result = false;

                cache.IsEnd = isEnd;
            }
            
            return result;
        }

        protected virtual bool RegexBalanceGroupOpenItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> openItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)cache[Cache.BALANCE_GROUP_CACHE_KEY];
            
            bool result;
            if (this.MatchesInternal(openItem.InnerRegex ,reader,
                new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                out bool isEnd
            ))
            {
                if (balanceGroupCache.ContainsKey(openItem.___balanceGroup))
                {
                    balanceGroupCache[openItem.___balanceGroup].Push(openItem.Method.DynamicInvoke(null));

                    result = true;
                }
                else result = false;
            }
            else result = false;

            cache.IsEnd = isEnd;
            cache.Handled = true;

            return result;
        }

        protected virtual bool RegexBalanceGroupSubItemMatchesInternal<TSeed>(RegexBalanceGroupSubItem<T, TSeed> subItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)cache[Cache.BALANCE_GROUP_CACHE_KEY];

            if (balanceGroupCache.ContainsKey(subItem.___balanceGroup))
            {
                var stack = balanceGroupCache[subItem.___balanceGroup];

                if (
                    subItem.Predicate((TSeed)stack.Peek()) &&
                    this.MatchesInternal(subItem.InnerRegex, reader,
                        new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                        out bool isEnd
                    )
                )
                {
                    stack.Push(subItem.Method.DynamicInvoke(stack.Pop()));

                    return true;
                }
                else return false;
            }
            else return false;
        }

        protected virtual bool RegexBalanceGroupCloseItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> closeItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            var balanceGroupCache = (IDictionary<RegexBalanceGroup<T>, Stack<object>>)cache[Cache.BALANCE_GROUP_CACHE_KEY];

            if (this.MatchesInternal(closeItem.InnerRegex, reader,
                new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                out bool isEnd
            ))
            {
                if (balanceGroupCache.ContainsKey(closeItem.___balanceGroup))
                {
                    return (bool)closeItem.Method.DynamicInvoke(balanceGroupCache[closeItem.___balanceGroup].Pop());
                }
                else return false;
            }
            else return false;
        }

        protected virtual bool RegexGroupReferenceMatchesInternal(RegexGroupReference<T> groupReference, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            var regexGroups = (Stack<RegexGroup<T>>)cache[Cache.REGEX_GROUPS_CACHE_KEY];

            RegexGroup<T> group;
            if (groupReference.IsDetermined)
                group = groupReference.Group;
            else
            {
                var groups = regexGroups.Where(_group => _group.ID == groupReference.GroupID).ToArray();
                switch (groups.Length)
                {
                    case 0:
                        //throw new InvalidOperationException("未找到引用的正则组。");
                        cache.IsEnd = reader.IsEnd();
                        cache.Handled = true;
                        return false;
                    case 1:
                        group = groups[0];
                        break;
                    default:
                        group = new RegexGroup<T>(
                            groups.Select(_group => _group.InnerRegex).UnionMany()
                        );
                        break;
                        //throw new InvalidOperationException("找到多个重复 ID 的正则组。");
                        //return false;
                }
            }

            bool result = this.MatchesInternal(group.InnerRegex, reader,
                new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                out bool isEnd
            );
            cache.IsEnd = isEnd;
            cache.Handled = true;

            return result;
        }

        protected virtual bool RegexMultiBranchMatchesInternal(RegexMultiBranch<T> multiBranch, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            bool result = this.MatchesInternal((_cache => this.GetRegexMultiBranchResultEnumerable(multiBranch, reader, _cache)), out bool isEnd);
            cache.IsEnd = isEnd;
            cache.Handled = true;

            return result;
        }

        private IEnumerable<bool> GetRegexMultiBranchResultEnumerable(RegexMultiBranch<T> multiBranch, NodeReader<IEnumerable<T>,T>reader, Cache cache)
        {
            bool result;
            bool isEnd;
            foreach (var branch in multiBranch.Branches)
            {
                if (this.MatchesInternal(branch.Predicate, reader,
                    new RegexMatchesHandler<RegexMultiBranchBranchPredicate<T>>(this.RegexMultiBranchBranchPredicateMatchesInternal),
                    out isEnd
                ))
                {
                    result = this.MatchesInternal(branch.Pattern, reader,
                        new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                        out isEnd
                    );
                    cache.IsEnd = isEnd;
                    cache.Handled = result;
                    yield return result;
                }
            }

            result = this.MatchesInternal(multiBranch.OtherwisePattern, reader,
                new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                out isEnd
            );
            cache.IsEnd = isEnd;
            cache.Handled = true;
            yield return result;
        }

        protected virtual bool RegexMultiBranchBranchPredicateMatchesInternal(RegexMultiBranchBranchPredicate<T> predicate, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (predicate is RegexMultiBranchPatternBranchPredicate<T> patternPredicate)
                return this.RegexMultiBranchPatternBranchPredicateMatchesInternal(patternPredicate, reader, cache);
            else if (predicate is RegexMultiBranchGroupReferenceBranchPredicate<T> groupReferencePredicate)
                return this.RegexMultiBranchGroupReferenceBranchPredicateMatchesInternal(groupReferencePredicate, reader, cache);
            else
                throw new NotSupportedException();
        }

        protected virtual bool RegexMultiBranchPatternBranchPredicateMatchesInternal(RegexMultiBranchPatternBranchPredicate<T> patternPredicate, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            IRegexAnchorPoint<T> pattern;
            if (patternPredicate.Pattern is IRegexAnchorPoint<T>)
                pattern = (IRegexAnchorPoint<T>)patternPredicate.Pattern;
            else
                pattern = new RegexZeroLengthObject<T>(patternPredicate.Pattern);

            bool result = this.MatchesInternal(
                pattern,
                reader,
                new Func<IRegexAnchorPoint<T>, NodeReader<IEnumerable<T>, T>, Cache, bool>(this.IRegexAnchorPointMatchesInternal),
                out bool isEnd
            );
            cache.IsEnd = isEnd;
            cache.Handled = true;

            return result;
        }

        protected virtual bool RegexMultiBranchGroupReferenceBranchPredicateMatchesInternal(RegexMultiBranchGroupReferenceBranchPredicate<T> groupReferencePredicate, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            cache.IsEnd = reader.IsEnd();
            cache.Handled = true;

            var regexGroups = (Stack<RegexGroup<T>>)cache[Cache.REGEX_GROUPS_CACHE_KEY];

            RegexGroupReference<T> groupReference = groupReferencePredicate.GroupReference;
            if (groupReference.IsDetermined)
                return true;
            else
            {
                var groups = regexGroups.Where(_group => _group.ID == groupReference.GroupID).ToArray();
                switch (groups.Length)
                {
                    case 0:
                        //throw new InvalidOperationException("未找到引用的正则组。");
                        return false;
                    case 1:
                        return true;
                    default:
                        return true;
                        //throw new InvalidOperationException("找到多个重复 ID 的正则组。");
                        //return false;

                }
            }
        }
        #endregion

        /// <summary>
        /// 表示 <see cref="RegexProvider{T}"/> 在进行正则表达式匹配时栈项的缓存。此类不可以被继承。
        /// </summary>
        public sealed class Cache : IDictionary<object, object>
        {
            private RegexProvider<T> provider;
            /// <summary>
            /// 本地缓存字典。
            /// </summary>
            private Dictionary<object, object> localCacheDic;

            /// <summary>
            /// 获取或设置一个值，指示该栈项是否已处理。若值为 true ，则弹出顶部栈项回到上一个栈项处理；若值为 false ，则继续调用处理方法直到值为 true 。
            /// </summary>
            public bool Handled { get; set; } = false;

            /// <summary>
            /// 获取或设置一个值，指示栈项是否已经处理到字符序列的结尾处。
            /// </summary>
            public bool IsEnd { get; set; } = false;

            public object this[object key]
            {
                get
                {
                    if (this.ContainsKey(key))
                        return this.localCacheDic[key];
                    else
                        return provider.globalCacheDic[key];
                }
                set
                {
                    this.localCacheDic[key] = value;
                }
            }

            /// <summary>
            /// 初始化 <see cref="Cache"/> 类的新实例，该实例使用指定的 <see cref="RegexProvider{T}"/> 以获取全局缓存。
            /// </summary>
            /// <param name="provider">要获取全局缓存通过的 <see cref="RegexProvider{T}"/> 对象。</param>
            internal Cache(RegexProvider<T> provider) =>
                this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

            /// <summary>
            /// 捕获集合的缓存的键。其在 <see cref="RegexProvider{T}.globalCacheDic"/> 中的值的类型为项是 <see cref="ValueTuple{T1, T2}"/> （其中 T1 为 <see cref="Capture{T}"/> ； T2 为 <see cref="object"/> 。）的 <see cref="List{T}"/> 。
            /// </summary>
            internal const string CAPTURES_CACHE_KEY = "CAPTURES";
            /// <summary>
            /// 前一个匹配的缓存的键。其在 <see cref="RegexProvider{T}.globalCacheDic"/> 中的值的类型为 <see cref="Match{T}"/> 。
            /// </summary>
            internal const string PREVIEW_MATCH_CACHE_KEY = "PREVIEW_MATCH";
            
            /// <summary>
            /// 对匹配 <see cref="RegexBalanceGroup{T}"/> 时预留的缓存的键。其在 <see cref="RegexProvider{T}.globalCacheDic"/> 中的值的类型为 <see cref="Dictionary{TKey, TValue}"/> （其中 TKey 为 <see cref="RegexBalanceGroup{T}"/> ； TValue 为项为 <see cref="object"/> 的 <see cref="Stack{T}"/> ） 。
            /// </summary>
            internal const string BALANCE_GROUP_CACHE_KEY = "BALANCE_GROUP";

            /// <summary>
            /// 对匹配 <see cref="RegexGroup{T}"/> 时预留的缓存的键。其在 <see cref="RegexProvider{T}.globalCacheDic"/> 中的值的类型为项为 <see cref="RegexGroup{T}"/> 的 <see cref="Stack{T}"/> 。
            /// </summary>
            internal const string REGEX_GROUPS_CACHE_KEY = "REGEX_GROUPS";

            public void Capture(Capture<T> capture, object id)
            {
                if (capture == null) throw new ArgumentNullException(nameof(capture));

                List<(Capture<T>, object)> captures;
                if (!this.ContainsKey(CAPTURES_CACHE_KEY))
                    this.Add(CAPTURES_CACHE_KEY, (captures = new List<(Capture<T>, object)>()));
                else
                    captures = (List<(Capture<T>, object)>)this[CAPTURES_CACHE_KEY];

                captures.Add((capture, id));
            }

            #region Implementations
            ICollection<object> IDictionary<object, object>.Keys => ((IDictionary<object, object>)this.localCacheDic).Keys;

            ICollection<object> IDictionary<object, object>.Values => ((IDictionary<object, object>)this.localCacheDic).Values;

            public int Count => ((IDictionary<object, object>)this.localCacheDic).Count;

            public bool IsReadOnly => ((IDictionary<object, object>)this.localCacheDic).IsReadOnly;

            public bool ContainsKey(object key)
            {
                return ((IDictionary<object, object>)this.localCacheDic).ContainsKey(key);
            }

            public void Add(object key, object value)
            {
                ((IDictionary<object, object>)this.localCacheDic).Add(key, value);
            }

            public bool Remove(object key)
            {
                return ((IDictionary<object, object>)this.localCacheDic).Remove(key);
            }

            public bool TryGetValue(object key, out object value)
            {
                return ((IDictionary<object, object>)this.localCacheDic).TryGetValue(key, out value);
            }

            void ICollection<KeyValuePair<object, object>>.Add(KeyValuePair<object, object> item)
            {
                ((IDictionary<object, object>)this.localCacheDic).Add(item);
            }

            public void Clear()
            {
                ((IDictionary<object, object>)this.localCacheDic).Clear();
            }

            bool ICollection<KeyValuePair<object, object>>.Contains(KeyValuePair<object, object> item)
            {
                return ((IDictionary<object, object>)this.localCacheDic).Contains(item);
            }

            void ICollection<KeyValuePair<object, object>>.CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
            {
                ((IDictionary<object, object>)this.localCacheDic).CopyTo(array, arrayIndex);
            }

            bool ICollection<KeyValuePair<object, object>>.Remove(KeyValuePair<object, object> item)
            {
                return ((IDictionary<object, object>)this.localCacheDic).Remove(item);
            }

            IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
            {
                return ((IDictionary<object, object>)this.localCacheDic).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IDictionary<object, object>)this.localCacheDic).GetEnumerator();
            }
            #endregion

            internal void CopyCacheFromLocalToGlobal(Action<Dictionary<object, object>, Dictionary<object, object>> method)
            {
                if (method == null) throw new ArgumentNullException(nameof(method));

                method(this.localCacheDic, this.provider.globalCacheDic);
            }
        }
    }
}
