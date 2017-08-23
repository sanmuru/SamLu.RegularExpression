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

        private Dictionary<object, object> globalCacheDic = new Dictionary<object, object>();

        private List<(Capture<T> capture, object id)> captures = new List<(Capture<T> capture, object id)>();
        private Stack<RegexGroup<T>> regexGroups = new Stack<RegexGroup<T>>();

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

        public virtual ICollection<Match<T>> Matches(IEnumerable<T> input) =>
            this.MatchesInternal(NodeReader.CreateReader(input)).ToList();

        public virtual ICollection<Match<T>> Matches(IEnumerable<T> input, int start, int length) =>
            this.Matches(input.Skip(start).Take(length));

        //public virtual bool IsMatch(IEnumerable<T> input) { }

        public virtual bool TryMatch(IEnumerable<T> input, out Match<T> result)
        {
            result = null;

            try { result = this.Match(input); }
            catch (Exception) { return false; }

            return true;
        }

        public virtual bool TryMatches(IEnumerable<T> input, out ICollection<Match<T>> result)
        {
            result = null;

            try { result = this.Matches(input); }
            catch (Exception) { return false; }

            return true;
        }
        #endregion

        #region MatchesInternal
        private Match<T> previewMatchCache;
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
                            new Group<T>(
                                reader.Reader,
                                group
                                    .AsEnumerable()
                                    .Distinct(new EqualityComparisonComparer<Capture<T>>((x, y) =>
                                        x.Index == y.Index && x.Length == y.Length
                                    ))
                                    .Select(capture => (capture.Index, capture.Length))
                                    .ToArray()
                            )
                        );
                    var match = new Match<T>(reader.Reader, prePosition, curPositon, groups);
                    this.previewMatchCache = match;
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
                        this.MatchesInternal(repeat.InnerRegex,reader,
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
                    this.MatchesInternal(repeat.InnerRegex,reader,
                        new RegexMatchesHandler<RegexObject<T>>(this.RegexObjectMatchesInternal),
                        out isEnd
                    )
                )
                {
                    repeatCount++;

                    if (
                        repeatCount >= (repeat.MinimumCount ?? uint.MinValue)&&
                        !(repeatCount<=preRepeatCountCache)
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

        private IEnumerable<bool> GetRegexSeriesResultEnumerable(RegexObject<T>[] series, int index, NodeReader<IEnumerable<T>,T> reader, Cache cache)
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

            bool result;
            if (anchorPoint is RegexZeroLengthObject<T> zeroLength)
                result = this.RegexZeroLengthObjectMatchesInternal(zeroLength, reader, out isEnd);
            else if (anchorPoint is RegexStartBorder<T> startBorder)
                result = this.RegexStartBorderMatchesInternal(startBorder, reader, out isEnd);
            else if (anchorPoint is RegexEndBorder<T> endBorder)
                result = this.RegexEndBorderMatchesInternal(endBorder, reader, out isEnd);
            else if (anchorPoint is RegexPreviousMatch<T> preMatch)
                result = this.RegexPreviewMatchMatchesInternal(preMatch, reader, out isEnd);
            else
                throw new NotSupportedException();

            if (reader.Position != prePosition)
                reader.Position = prePosition;

            return result;
        }

        protected virtual bool RegexZeroLengthObjectMatchesInternal(RegexZeroLengthObject<T> zeroLength, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            return this.RegexObjectMatchesInternal(zeroLength.InnerRegex, reader, out isEnd);
        }

        protected virtual bool RegexStartBorderMatchesInternal(RegexStartBorder<T> startBorder, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            isEnd = reader.IsEnd();
            return reader.Position == 0;
        }

        protected virtual bool RegexEndBorderMatchesInternal(RegexEndBorder<T> endBorder, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (!reader.IsEnd())
            {
                reader.Position++;
                if (reader.IsEnd())
                {
                    isEnd = true;
                    return true;
                }
                else
                {
                    isEnd = false;
                    reader.Rollback(1);
                    return false;
                }
            }
            else
            {
                isEnd = true;
                return false;
            }
        }

        protected virtual bool RegexPreviewMatchMatchesInternal(RegexPreviousMatch<T> preMatch, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            isEnd = false;

            if (this.previewMatchCache == null)
                return true;
            else
                return reader.Position == this.previewMatchCache.Index + this.previewMatchCache.Length;
        }

        protected virtual bool RegexGroupMatchesInternal(RegexGroup<T> group, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (group.IsCaptive) this.regexGroups.Push(group);

            int prePosition = reader.Position;
            bool f;
            if (group is RegexBalanceGroup<T> balanceGroup)
                f = this.RegexBalanceGroupMatchesInternal(balanceGroup, reader, out isEnd);
            else if (group is RegexBalanceGroupItem<T> balanceGroupItem)
                f = this.RegexBalanceGroupItemMatchesInternal(balanceGroupItem, reader, out isEnd);
            else
                f = this.RegexObjectMatchesInternal(group.InnerRegex, reader, out isEnd);
            if (f && group.IsCaptive)
            {
                this.captures.Add((new Capture<T>(reader.Reader, prePosition, reader.Position - prePosition + 1), group.ID));
            }

            if (group.IsCaptive) this.regexGroups.Pop();

            return f;
        }

        private Dictionary<RegexBalanceGroup<T>, Stack<object>> balanceGroupCache = new Dictionary<RegexBalanceGroup<T>, Stack<object>>();

        protected virtual bool RegexBalanceGroupMatchesInternal(RegexBalanceGroup<T> balanceGroup, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            bool f;
            if (f = !this.balanceGroupCache.ContainsKey(balanceGroup))
                this.balanceGroupCache.Add(balanceGroup, new Stack<object>());

            bool result = this.RegexObjectMatchesInternal(balanceGroup.InnerRegex, reader, out isEnd);

            if (f)
                this.balanceGroupCache.Remove(balanceGroup);

            return result;
        }

        private bool RegexBalanceGroup__ItemMatchesInternal(Type __itemTypeDefinition, string handlerName, RegexBalanceGroupItem<T> item, NodeReader<IEnumerable<T>, T> reader, out bool result, out bool isEnd)
        {
            Type itemType = item.GetType();
            if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == __itemTypeDefinition)
            {
                var method = itemType.GetMethod(handlerName, BindingFlags.IgnoreCase | BindingFlags.NonPublic).MakeGenericMethod(itemType.GetGenericArguments());
                object[] parameters = new object[] { item, reader, null };
                result = (bool)method.Invoke(item, parameters);
                isEnd = (bool)parameters[2];

                return true;
            }
            else
            {
                result = false;
                isEnd = false;

                return false;
            }
        }

        protected virtual bool RegexBalanceGroupItemMatchesInternal(RegexBalanceGroupItem<T> balanceGroupItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            bool result;
            if (
                this.RegexBalanceGroup__ItemMatchesInternal(typeof(RegexBalanceGroupOpenItem<,>), nameof(this.RegexBalanceGroupOpenItemMatchesInternal), balanceGroupItem, reader, out result, out isEnd) ||
                this.RegexBalanceGroup__ItemMatchesInternal(typeof(RegexBalanceGroupSubItem<,>), nameof(this.RegexBalanceGroupSubItemMatchesInternal), balanceGroupItem, reader, out result, out isEnd) ||
                this.RegexBalanceGroup__ItemMatchesInternal(typeof(RegexBalanceGroupCloseItem<,>), nameof(this.RegexBalanceGroupCloseItemMatchesInternal), balanceGroupItem, reader, out result, out isEnd)
            )
                return result;
            else
            {
                if (this.RegexObjectMatchesInternal(balanceGroupItem.InnerRegex, reader, out isEnd))
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

                        return result;
                    }
                    else return false;
                }
                return false;
            }
        }

        protected virtual bool RegexBalanceGroupOpenItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> openItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (this.RegexObjectMatchesInternal(openItem.InnerRegex, reader, out isEnd))
            {
                if (this.balanceGroupCache.ContainsKey(openItem.___balanceGroup))
                {
                    this.balanceGroupCache[openItem.___balanceGroup].Push(openItem.Method.DynamicInvoke(null));

                    return true;
                }
                else return false;
            }
            else return false;
        }

        protected virtual bool RegexBalanceGroupSubItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> subItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (this.RegexObjectMatchesInternal(subItem.InnerRegex, reader, out isEnd))
            {
                if (this.balanceGroupCache.ContainsKey(subItem.___balanceGroup))
                {
                    var stack = this.balanceGroupCache[subItem.___balanceGroup];
                    stack.Push(subItem.Method.DynamicInvoke(stack.Pop()));

                    return true;
                }
                else return false;
            }
            else return false;
        }

        protected virtual bool RegexBalanceGroupCloseItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> closeItem, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            if (this.RegexObjectMatchesInternal(closeItem.InnerRegex, reader, out isEnd))
            {
                if (this.balanceGroupCache.ContainsKey(closeItem.___balanceGroup))
                {
                    return (bool)closeItem.Method.DynamicInvoke(this.balanceGroupCache[closeItem.___balanceGroup].Pop());
                }
                else return false;
            }
            else return false;
        }

        protected virtual bool RegexGroupReferenceMatchesInternal(RegexGroupReference<T> groupReference, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            RegexGroup<T> group;
            if (groupReference.IsDetermined)
                group = groupReference.Group;
            else
            {
                var groups = this.regexGroups.Where(_group => _group.ID == groupReference.GroupID).ToArray();
                switch (groups.Length)
                {
                    case 0:
                        //throw new InvalidOperationException("未找到引用的正则组。");
                        isEnd = reader.IsEnd();
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

            return this.RegexGroupMatchesInternal(group, reader, out isEnd);
        }

        protected virtual bool RegexMultiBranchMatchesInternal(RegexMultiBranch<T> multiBranch, NodeReader<IEnumerable<T>, T> reader, Cache cache)
        {
            foreach (var pair in multiBranch.Branches)
            {
                if (this.RegexObjectMatchesInternal(pair.Key, reader, out isEnd))
                    return this.RegexObjectMatchesInternal(pair.Value, reader, out isEnd);
            }
            return this.RegexObjectMatchesInternal(multiBranch.OtherwisePattern, reader, out isEnd);
        }
        #endregion

        /// <summary>
        /// 表示 <see cref="RegexProvider{T}"/> 在进行正则表达式匹配时栈项的缓存。此类不可以被继承。
        /// </summary>
        public sealed class Cache : IDictionary<object, object>
        {
            private RegexProvider<T> provider;
            private Dictionary<object, object> localCacheDic = new Dictionary<object, object>();

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
            /// 捕获集合的缓存的键。
            /// </summary>
            internal const string CAPTURES_CACHE_KEY = "CAPTURES";

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
