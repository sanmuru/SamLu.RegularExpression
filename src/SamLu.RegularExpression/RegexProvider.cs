using SamLu.IO;
using SamLu.RegularExpression.Extend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private Dictionary<Group<T>, string> namedGroups = new Dictionary<Group<T>, string>();

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

        protected virtual IEnumerable<Match<T>> MatchesInternal(NodeReader<IEnumerable<T>, T> reader)
        {
            while (!reader.IsEnd())
            {
                int prePosition = reader.Position;
                if (RegexObjectMatchesInternal(pattern, reader, out IEnumerable<Group<T>> groups, out bool isEnd))
                {
                    int curPositon = reader.Position;
                    yield return new Match<T>(reader.Reader, prePosition, curPositon, groups);
                    continue;
                }
                else reader.Position++;

                if (isEnd) yield break;
            }
        }

        protected virtual bool RegexObjectMatchesInternal(RegexObject<T> regex, NodeReader<IEnumerable<T>, T> reader, out IEnumerable<Group<T>> groups, out bool isEnd)
        {
            if (regex is RegexStartBorder<T> startBorder)
                return this.RegexStartBorderMatchesInternal(startBorder, reader, out groups, out isEnd);
            else if (regex is RegexEndBorder<T> endBorder)
                return this.RegexEndBorderMatchesInternal(endBorder, reader, out groups, out isEnd);
            else if (regex is RegexNamedGroup<T> group)
                return this.RegexGroupMatchesInternal(group, reader, out groups, out isEnd);
            else
                throw new NotSupportedException();
        }

        protected virtual bool RegexStartBorderMatchesInternal(RegexStartBorder<T> startBorder, NodeReader<IEnumerable<T>, T> reader, out IEnumerable<Group<T>> groups, out bool isEnd)
        {
            groups = Enumerable.Empty<Group<T>>();
            isEnd = reader.IsEnd();
            return reader.Position == 0;
        }

        protected virtual bool RegexEndBorderMatchesInternal(RegexEndBorder<T> endBorder, NodeReader<IEnumerable<T>, T> reader, out IEnumerable<Group<T>> groups, out bool isEnd)
        {
            groups = Enumerable.Empty<Group<T>>();
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

        protected virtual bool RegexGroupMatchesInternal(RegexNamedGroup<T> group, NodeReader<IEnumerable<T>, T> reader, out IEnumerable<Group<T>> groups, out bool isEnd)
        {
            throw new NotImplementedException();
        }

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
    }
}
