using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public static class Regex
    {
        public static RegexConst<T> Const<T>(T t) => new RegexConst<T>(t);

        #region Series
        public static RegexSeries<T> Series<T>(IEnumerable<T> ts) => Regex.ConcatMany<T, RegexConst<T>>(ts?.Select(t => Regex.Const(t)));

        public static RegexSeries<T> Series<T>(params T[] ts) => Regex.Series<T>(ts?.AsEnumerable());
        #endregion

        #region Parallels
        public static RegexParallels<T> Parallels<T>(IEnumerable<T> ts) => Regex.UnionMany<T, RegexConst<T>>(ts?.Select(t => Regex.Const(t)));

        public static RegexParallels<T> Parallels<T>(params T[] ts) => Regex.Parallels<T>(ts?.AsEnumerable());
        #endregion

        public static RegexRange<T> Range<T>(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) => new RegexRange<T>(minimum, maximum, canTakeMinimum, canTakeMaximum);

        public static RegexRepeat<T> Optional<T>(this RegexObject<T> regex) => regex.Repeat(0, 1);

        #region NoneOrMany
        public static RegexRepeat<T> NoneOrMany<T>(T t) => Regex.Const(t).NoneOrMany();

        public static RegexRepeat<T> NoneOrMany<T>(this RegexObject<T> regex) => new RegexRepeat<T>(regex, true);
        #endregion

        #region Many
        public static RegexRepeat<T> Many<T>(T t) => Regex.Const(t).Many();

        public static RegexRepeat<T> Many<T>(this RegexObject<T> regex) => new RegexRepeat<T>(regex, 1, true);
        #endregion

        #region Repeat
        public static RegexRepeat<T> Repeat<T>(T t, ulong count) => Regex.Const(t).Repeat(count);

        public static RegexRepeat<T> Repeat<T>(this RegexObject<T> regex, ulong count) => regex * count;

        public static RegexRepeat<T> Repeat<T>(T t, ulong minimumCount, ulong maximumCount) => Regex.Const(t).Repeat(minimumCount, maximumCount);

        public static RegexRepeat<T> Repeat<T>(T t, ulong? minimumCount, ulong? maximumCount) => Regex.Const(t).Repeat(minimumCount, maximumCount);

        public static RegexRepeat<T> Repeat<T>(this RegexObject<T> regex, ulong minimumCount, ulong maximumCount) => new RegexRepeat<T>(regex, minimumCount, maximumCount);

        public static RegexRepeat<T> Repeat<T>(this RegexObject<T> regex, ulong? minimumCount, ulong? maximumCount) => new RegexRepeat<T>(regex, minimumCount, maximumCount);
        #endregion

        #region RepeatMany
        public static RegexRepeat<T> RepeatMany<T>(T t, ulong minimumCount) => Regex.Const(t).RepeatMany(minimumCount);

        public static RegexRepeat<T> RepeatMany<T>(T t, ulong? minimumCount) => Regex.Const(t).RepeatMany(minimumCount);

        public static RegexRepeat<T> RepeatMany<T>(this RegexObject<T> regex, ulong minimumCount) => new RegexRepeat<T>(regex, minimumCount, true);

        public static RegexRepeat<T> RepeatMany<T>(this RegexObject<T> regex, ulong? minimumCount) => new RegexRepeat<T>(regex, minimumCount, true);
        #endregion

        #region UnionMany
        public static RegexParallels<T> UnionMany<T>(this IEnumerable<RegexObject<T>> parallels) => new RegexParallels<T>(parallels);

        public static RegexParallels<T> UnionMany<T>(params RegexObject<T>[] parallels) => parallels?.AsEnumerable().UnionMany();

        public static RegexParallels<T> UnionMany<T, TRegexObject>(this IEnumerable<TRegexObject> parallels)
            where TRegexObject : RegexObject<T> =>
                ((IEnumerable<RegexObject<T>>)parallels).UnionMany();

        public static RegexParallels<T> UnionMany<T, TRegexObject>(params TRegexObject[] parallels)
            where TRegexObject : RegexObject<T> =>
                (parallels?.AsEnumerable() as IEnumerable<RegexObject<T>>).UnionMany();
        #endregion

        #region ConcatMany
        public static RegexSeries<T> ConcatMany<T>(this IEnumerable<RegexObject<T>> series) => new RegexSeries<T>(series);

        public static RegexSeries<T> ConcatMany<T>(params RegexObject<T>[] series) => series?.AsEnumerable().ConcatMany();

        public static RegexSeries<T> ConcatMany<T, TRegexObject>(this IEnumerable<TRegexObject> series)
            where TRegexObject : RegexObject<T> =>
                ((IEnumerable<RegexObject<T>>)series).ConcatMany();

        public static RegexSeries<T> ConcatMany<T, TRegexObject>(params TRegexObject[] series)
            where TRegexObject : RegexObject<T> =>
                (series?.AsEnumerable() as IEnumerable<RegexObject<T>>).ConcatMany();
        #endregion
    }
}
