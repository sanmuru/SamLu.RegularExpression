using SamLu.IO;
using SamLu.RegularExpression.Extend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    public class RegexProvider<T>
    {
        private RegexObject<T> pattern;
        private RegexOptions options;

        public RegexOptions Options => this.options;
        public bool RightToLeft => this.Options.RightToLeft;

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

        private Match<T> previewMatchCache;
        protected virtual IEnumerable<Match<T>> MatchesInternal(NodeReader<IEnumerable<T>, T> reader)
        {
            while (!reader.IsEnd())
            {
                int prePosition = reader.Position;
                if (RegexObjectMatchesInternal(this.pattern, reader, out bool isEnd))
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

        protected virtual bool RegexObjectMatchesInternal(RegexObject<T> regex, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
        {
            if (regex is IRegexAnchorPoint<T> anchorPoint)
                return this.MatchesInternal(anchorPoint, reader, out isEnd);
            else if (regex is RegexGroup<T> group)
                return this.RegexGroupMatchesInternal(group, reader, out isEnd);
            else if (regex is RegexGroupReference<T> groupReference)
                return this.RegexGroupReferenceMatchesInternal(groupReference, reader, out isEnd);
            else if (regex is RegexMultiBranch<T> multiBranch)
                return this.RegexMultiBranchMatchesInternal(multiBranch, reader, out isEnd);
            else
                throw new NotSupportedException();
        }

        protected virtual bool MatchesInternal(IRegexAnchorPoint<T> anchorPoint, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexZeroLengthObjectMatchesInternal(RegexZeroLengthObject<T> zeroLength, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
        {
            return this.RegexObjectMatchesInternal(zeroLength.InnerRegex, reader, out isEnd);
        }

        protected virtual bool RegexStartBorderMatchesInternal(RegexStartBorder<T> startBorder, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
        {
            isEnd = reader.IsEnd();
            return reader.Position == 0;
        }

        protected virtual bool RegexEndBorderMatchesInternal(RegexEndBorder<T> endBorder, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexPreviewMatchMatchesInternal(RegexPreviousMatch<T> preMatch, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
        {
            isEnd = false;

            if (this.previewMatchCache == null)
                return true;
            else
                return reader.Position == this.previewMatchCache.Index + this.previewMatchCache.Length;
        }

        protected virtual bool RegexGroupMatchesInternal(RegexGroup<T> group, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexBalanceGroupMatchesInternal(RegexBalanceGroup<T> balanceGroup, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexBalanceGroupItemMatchesInternal(RegexBalanceGroupItem<T> balanceGroupItem, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexBalanceGroupOpenItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> openItem, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexBalanceGroupSubItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> subItem, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexBalanceGroupCloseItemMatchesInternal<TSeed>(RegexBalanceGroupOpenItem<T, TSeed> closeItem, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexGroupReferenceMatchesInternal(RegexGroupReference<T> groupReference, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
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

        protected virtual bool RegexMultiBranchMatchesInternal(RegexMultiBranch<T> multiBranch, NodeReader<IEnumerable<T>, T> reader, out bool isEnd)
        {
            foreach (var pair in multiBranch.Branches)
            {
                if (this.RegexObjectMatchesInternal(pair.Key, reader, out isEnd))
                    return this.RegexObjectMatchesInternal(pair.Value, reader, out isEnd);
            }
            return this.RegexObjectMatchesInternal(multiBranch.OtherwisePattern, reader, out isEnd);
        }
    }
}
