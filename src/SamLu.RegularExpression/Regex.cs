using SamLu.RegularExpression.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression
{
    /// <summary>
    /// 提供了一套 static 的用于快速创建正则对象、使用正则对象进行匹配的方法。
    /// </summary>
    public static class Regex
    {
        #region 创建正则对象
        /// <summary>
        /// 创建常量正则，指定内部常量对象。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的内部常量对象。</param>
        /// <returns>内部常量为指定对象的常量正则。</returns>
        public static RegexConst<T> Const<T>(T t) => new RegexConst<T>(t);

        #region Series
        /// <summary>
        /// 创建串联正则，指定内部正则对象列表为常量正则列表。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="ts">常量对象列表。</param>
        /// <returns>内部正则对象列表为指定常量对象集合构造的常量正则列表的串联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ts"/> 的值为 null 。</exception>
        public static RegexSeries<T> Series<T>(IEnumerable<T> ts) =>
            Regex.ConcatMany<T, RegexConst<T>>(
                (ts ?? throw new ArgumentNullException(nameof(ts)))
                    .Select(t => Regex.Const(t))
            );

        /// <summary>
        /// 创建串联正则，指定内部正则对象列表为常量正则列表。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="ts">常量对象参数数组。</param>
        /// <returns>内部正则对象列表为指定常量对象参数数组构造的常量正则列表的串联正则。</returns>
        public static RegexSeries<T> Series<T>(params T[] ts) =>
            Regex.Series<T>((ts ?? throw new ArgumentNullException(nameof(ts))).AsEnumerable());

        public static RegexSeries<S> Series<T, S>(IEnumerable<T> ts, Func<T, S> selector) =>
            Regex.Series(
                (ts ?? throw new ArgumentNullException(nameof(ts)))
                    .Select(selector ?? throw new ArgumentNullException(nameof(selector)))
            );
        #endregion

        #region Parallels
        /// <summary>
        /// 创建并联正则，指定内部正则对象列表为常量正则列表。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="ts">常量对象列表。</param>
        /// <returns>内部正则对象列表为指定常量对象集合构造的常量正则列表的并联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ts"/> 的值为 null 。</exception>
        public static RegexParallels<T> Parallels<T>(IEnumerable<T> ts) =>
            Regex.UnionMany<T, RegexConst<T>>(
                (ts ?? throw new ArgumentNullException(nameof(ts)))
                    .Select(t => Regex.Const(t))
            );

        /// <summary>
        /// 创建并联正则，指定内部正则对象列表为常量正则列表。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="ts">常量对象参数数组。</param>
        /// <returns>内部正则对象列表为指定常量对象参数数组构造的常量正则列表的并联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ts"/> 的值为 null 。</exception>
        public static RegexParallels<T> Parallels<T>(params T[] ts) =>
            Regex.Parallels<T>((ts ?? throw new ArgumentNullException(nameof(ts))).AsEnumerable());

        public static RegexParallels<S> Parallels<T, S>(IEnumerable<T> ts, Func<T, S> selector) =>
            Regex.Parallels(
                (ts ?? throw new ArgumentNullException(nameof(ts)))
                    .Select(selector ?? throw new ArgumentNullException(nameof(selector)))
            );
        #endregion

        /// <summary>
        /// 创建范围正则，指定范围的最小值、最大值，是否能取到最小值、最大值以及。
        /// </summary>
        /// <typeparam name="T">最小值、最大值对象的类型。</typeparam>
        /// <param name="minimum">指定的范围的最小值。</param>
        /// <param name="maximum">指定的范围的最大值。</param>
        /// <param name="canTakeMinimum">一个值，指示内部范围是否取到最小值。默认为 true 。</param>
        /// <param name="canTakeMaximum">一个值，指示内部范围是否取到最大值。默认为 true 。</param>
        /// <returns>内部范围为指定最小值、最大值，是否能取到最小值、最大值以及指定值大小比较方法的范围正则。</returns>
        public static RegexRange<T> Range<T>(T minimum, T maximum, bool canTakeMinimum = true, bool canTakeMaximum = true) => new RegexRange<T>(minimum, maximum, canTakeMinimum, canTakeMaximum);

        /// <summary>
        /// 创建指定正则对象的可选的重复正则。与重复 0 或 1 次的重复正则等价。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <returns>指定正则对象的可选的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> Optional<T>(this RegexObject<T> regex) =>
            (regex ?? throw new ArgumentNullException(nameof(regex))).Repeat(0, 1);

        #region NoneOrMany
        /// <summary>
        /// 创建指定常量对象的可选或无限重复的重复正则。与重复 0 - ∞ 次的重复正则等价。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <returns>指定常量对象的可选或无限重复的重复正则。</returns>
        public static RegexRepeat<T> NoneOrMany<T>(T t) => Regex.Const(t).NoneOrMany();

        /// <summary>
        /// 创建指定正则对象的可选或无限重复的重复正则。与重复 0 - ∞ 次的重复正则等价。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <returns>指定正则对象的可选或无限重复的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> NoneOrMany<T>(this RegexObject<T> regex) => new RegexRepeat<T>(regex ?? throw new ArgumentNullException(nameof(regex)), true);
        #endregion

        #region Many
        /// <summary>
        /// 创建指定常量对象的无限重复的重复正则。与重复 0 - ∞ 次的重复正则等价。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <returns>指定常量对象的无限重复的重复正则。</returns>
        public static RegexRepeat<T> Many<T>(T t) => Regex.Const(t).Many();

        /// <summary>
        /// 创建指定正则对象的无限重复的重复正则。与重复 0 - ∞ 次的重复正则等价。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的内部正则对象。</param>
        /// <returns>指定正则对象的无限重复的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> Many<T>(this RegexObject<T> regex) => new RegexRepeat<T>(regex ?? throw new ArgumentNullException(nameof(regex)), 1, true);
        #endregion

        #region Repeat
        /// <summary>
        /// 创建指定常量对象的指定重复次数的重复正则。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <param name="count">指定的重复次数。</param>
        /// <returns>指定常量对象的指定重复次数的重复正则。</returns>
        public static RegexRepeat<T> Repeat<T>(T t, ulong count) => Regex.Const(t).Repeat(count);

        /// <summary>
        /// 创建指定正则对象的指定重复次数的重复正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="count">指定的重复次数。</param>
        /// <returns>指定正则对象的指定重复次数的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> Repeat<T>(this RegexObject<T> regex, ulong count) => (regex ?? throw new ArgumentNullException(nameof(regex))) * count;

        /// <summary>
        /// 创建指定常量对象的重复正则。指定重复次数的最小值、最大值。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="maximumCount">最大重复次数。</param>
        /// <returns>指定常量对象以及重复次数的最小值、最大值的重复正则。</returns>
        public static RegexRepeat<T> Repeat<T>(T t, ulong minimumCount, ulong maximumCount) => Regex.Const(t).Repeat(minimumCount, maximumCount);

        /// <summary>
        /// 创建指定常量对象的重复正则。指定重复次数的最小值、最大值。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="maximumCount">最大重复次数。</param>
        /// <returns>指定常量对象以及重复次数的最小值、最大值的重复正则。</returns>
        public static RegexRepeat<T> Repeat<T>(T t, ulong? minimumCount, ulong? maximumCount) => Regex.Const(t).Repeat(minimumCount, maximumCount);

        /// <summary>
        /// 创建指定正则对象的重复正则。指定重复次数的最小值、最大值。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="maximumCount">最大重复次数。</param>
        /// <returns>指定正则对象以及重复次数的最小值、最大值的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> Repeat<T>(this RegexObject<T> regex, ulong minimumCount, ulong maximumCount) =>
            new RegexRepeat<T>(regex ?? throw new ArgumentNullException(nameof(regex)), minimumCount, maximumCount);

        /// <summary>
        /// 创建指定正则对象的重复正则。指定重复次数的最小值、最大值。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <param name="maximumCount">最大重复次数。</param>
        /// <returns>指定正则对象以及重复次数的最小值、最大值的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> Repeat<T>(this RegexObject<T> regex, ulong? minimumCount, ulong? maximumCount) =>
            new RegexRepeat<T>(regex ?? throw new ArgumentNullException(nameof(regex)), minimumCount, maximumCount);
        #endregion

        #region RepeatMany
        /// <summary>
        /// 创建指定常量对象的无限重复的重复正则。指定重复次数的最小值。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <returns>指定常量对象以及重复次数的最小值的无限重复的重复正则。</returns>
        public static RegexRepeat<T> RepeatMany<T>(T t, ulong minimumCount) => Regex.Const(t).RepeatMany(minimumCount);

        /// <summary>
        /// 创建指定常量对象的无限重复的重复正则。指定重复次数的最小值。
        /// </summary>
        /// <typeparam name="T">常量对象的类型。</typeparam>
        /// <param name="t">指定的常量对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <returns>指定常量对象以及重复次数的最小值的无限重复的重复正则。</returns>
        public static RegexRepeat<T> RepeatMany<T>(T t, ulong? minimumCount) => Regex.Const(t).RepeatMany(minimumCount);

        /// <summary>
        /// 创建指定正则对象的无限重复的重复正则。指定重复次数的最小值。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <returns>指定正则对象以及重复次数的最小值的无限重复的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> RepeatMany<T>(this RegexObject<T> regex, ulong minimumCount) =>
            new RegexRepeat<T>(regex ?? throw new ArgumentNullException(nameof(regex)), minimumCount, true);

        /// <summary>
        /// 创建指定正则对象的无限重复的重复正则。指定重复次数的最小值。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="minimumCount">最小重复次数。</param>
        /// <returns>指定正则对象以及重复次数的最小值的无限重复的重复正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexRepeat<T> RepeatMany<T>(this RegexObject<T> regex, ulong? minimumCount) =>
            new RegexRepeat<T>(regex ?? throw new ArgumentNullException(nameof(regex)), minimumCount, true);
        #endregion

        /// <summary>
        /// 创建指定正则重复的非贪婪匹配模式重复。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="repeat">指定的正则重复。</param>
        /// <returns>指定正则重复的非贪婪匹配模式重复。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="repeat"/> 的值为 null 。</exception>
        public static RegexNonGreedyRepeat<T> NonGreedy<T>(this RegexRepeat<T> repeat) =>
            new RegexNonGreedyRepeat<T>(repeat ?? throw new ArgumentNullException(nameof(repeat)));

        #region Group
        /// <summary>
        /// 创建以指定正则对象为匹配模式的正则组，并确定是否捕获此正则组。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="isCaptive">一个值，指示此正则组是否捕获。</param>
        /// <returns>以指定正则对象为匹配模式的正则组。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexGroup<T> Group<T>(this RegexObject<T> regex, bool isCaptive = true) =>
            new RegexGroup<T>(regex ?? throw new ArgumentNullException(nameof(regex)), isCaptive);

        /// <summary>
        /// 创建以指定正则对象为匹配模式的正则组，指定此正则组的 ID 并确定是否捕获。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="regex">指定的正则对象。</param>
        /// <param name="id">指定的正则组的 ID 。</param>
        /// <param name="isCaptive">一个值，指示此正则组是否捕获。</param>
        /// <returns>以指定正则对象为匹配模式的正则组。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/> 的值为 null 。</exception>
        public static RegexGroup<T> Group<T>(this RegexObject<T> regex, object id, bool isCaptive) =>
            new RegexGroup<T>(regex ?? throw new ArgumentNullException(nameof(regex)), id, isCaptive);
        #endregion

        #region GroupReference
        /// <summary>
        /// 使用指定的正则组 ID 创建正则组引用。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="id">指定的正则组 ID 。</param>
        /// <returns>指定的正则组 ID 创建正则组引用。</returns>
        public static RegexGroupReference<T> GroupReference<T>(object id) =>
            new RegexGroupReference<T>(id);

        /// <summary>
        /// 由指定正则组创建引用其的正则组引用。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="group">要获得引用的正则组。</param>
        /// <param name="groupReference">引用 <paramref name="group"/> 的正则组引用。</param>
        /// <returns>引用指定正则组的正则组引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="group"/> 的值为 null 。</exception>
        public static RegexGroup<T> GroupReference<T>(this RegexGroup<T> group, out RegexGroupReference<T> groupReference)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            groupReference = new RegexGroupReference<T>(group);
            return group;
        }
        #endregion

        #region ConcatMany
        /// <summary>
        /// 创建指定正则对象集合作为内部正则对象列表的串联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="series">指定的正则对象集合。</param>
        /// <returns>指定正则对象集合作为内部正则对象列表的串联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> 的值为 null 。</exception>
        public static RegexSeries<T> ConcatMany<T>(this IEnumerable<RegexObject<T>> series) => new RegexSeries<T>(series ?? throw new ArgumentNullException(nameof(series)));

        /// <summary>
        /// 创建指定正则对象参数数组作为内部正则对象列表的串联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="series">指定的正则对象参数数组。</param>
        /// <returns>指定正则对象参数数组作为内部正则对象列表的串联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> 的值为 null 。</exception>
        public static RegexSeries<T> ConcatMany<T>(params RegexObject<T>[] series) => (series ?? throw new ArgumentNullException(nameof(series))).AsEnumerable().ConcatMany();

        /// <summary>
        /// 创建指定正则对象集合作为内部正则对象列表的串联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <typeparam name="TRegexObject">正则对象的类型。</typeparam>
        /// <param name="series">指定的正则对象集合。</param>
        /// <returns>指定正则对象集合作为内部正则对象列表的串联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> 的值为 null 。</exception>
        public static RegexSeries<T> ConcatMany<T, TRegexObject>(this IEnumerable<TRegexObject> series)
            where TRegexObject : RegexObject<T> =>
                ((IEnumerable<RegexObject<T>>)(series ?? throw new ArgumentNullException(nameof(series)))).ConcatMany();

        /// <summary>
        /// 创建指定正则对象参数数组作为内部正则对象列表的串联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <typeparam name="TRegexObject">正则对象的类型。</typeparam>
        /// <param name="series">指定的正则对象参数数组。</param>
        /// <returns>指定正则对象参数数组作为内部正则对象列表的串联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="series"/> 的值为 null 。</exception>
        public static RegexSeries<T> ConcatMany<T, TRegexObject>(params TRegexObject[] series)
            where TRegexObject : RegexObject<T> =>
                ((series ?? throw new ArgumentNullException(nameof(series))).AsEnumerable() as IEnumerable<RegexObject<T>>).ConcatMany();
        #endregion

        #region UnionMany
        /// <summary>
        /// 创建指定正则对象集合作为内部正则对象列表的并联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="parallels">指定的正则对象集合。</param>
        /// <returns>指定正则对象集合作为内部正则对象列表的并联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>
        public static RegexParallels<T> UnionMany<T>(this IEnumerable<RegexObject<T>> parallels) => new RegexParallels<T>(parallels ?? throw new ArgumentNullException(nameof(parallels)));

        /// <summary>
        /// 创建指定正则对象参数数组作为内部正则对象列表的并联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <param name="parallels">指定的正则对象参数数组。</param>
        /// <returns>指定正则对象参数数组作为内部正则对象列表的并联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>
        public static RegexParallels<T> UnionMany<T>(params RegexObject<T>[] parallels) => (parallels ?? throw new ArgumentNullException(nameof(parallels))).AsEnumerable().UnionMany();

        /// <summary>
        /// 创建指定正则对象集合作为内部正则对象列表的并联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <typeparam name="TRegexObject">正则对象的类型。</typeparam>
        /// <param name="parallels">指定的正则对象集合。</param>
        /// <returns>指定正则对象集合作为内部正则对象列表的并联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>
        public static RegexParallels<T> UnionMany<T, TRegexObject>(this IEnumerable<TRegexObject> parallels)
            where TRegexObject : RegexObject<T> =>
                ((IEnumerable<RegexObject<T>>)(parallels ?? throw new ArgumentNullException(nameof(parallels)))).UnionMany();

        /// <summary>
        /// 创建指定正则对象参数数组作为内部正则对象列表的并联正则。
        /// </summary>
        /// <typeparam name="T">正则接受的对象的类型。</typeparam>
        /// <typeparam name="TRegexObject">正则对象的类型。</typeparam>
        /// <param name="parallels">指定的正则对象参数数组。</param>
        /// <returns>指定正则对象参数数组作为内部正则对象列表的并联正则。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parallels"/> 的值为 null 。</exception>
        public static RegexParallels<T> UnionMany<T, TRegexObject>(params TRegexObject[] parallels)
            where TRegexObject : RegexObject<T> =>
                ((parallels ?? throw new ArgumentNullException(nameof(parallels))).AsEnumerable() as IEnumerable<RegexObject<T>>).UnionMany();
        #endregion
        #endregion

        #region 使用正则对象进行匹配
        #region Match
        public static Match<T> Match<T>(IEnumerable<T> input, RegexObject<T> pattern) =>
            Regex.Match(input, pattern, RegexOptions.None);

        public static Match<T> Match<T>(IEnumerable<T> input, RegexObject<T> pattern, RegexOptions options) =>
            new RegexProvider<T>(pattern, options).Match(input);

        public static Match<T> Match<T, TRegex>(IEnumerable<T> input, RegexObject<T> pattern, RegexOptions options, Func<RegexObject<T>, RegexOptions, TRegex> activator)
            where TRegex : RegexProvider<T>
        {
            if (activator == null) throw new ArgumentNullException(nameof(activator));

            return activator(pattern, options).Match(input);
        }
        #endregion

        #region Matches
        public static MatchCollection<T> Matches<T>(IEnumerable<T> input, RegexObject<T> pattern) =>
            Regex.Matches(input, pattern, RegexOptions.None);

        public static MatchCollection<T> Matches<T>(IEnumerable<T> input, RegexObject<T> pattern, RegexOptions options) =>
            new RegexProvider<T>(pattern, options).Matches(input);

        public static MatchCollection<T> Matches<T, TRegexProvider>(IEnumerable<T> input, RegexObject<T> pattern, RegexOptions options, Func<RegexObject<T>, RegexOptions, TRegexProvider> activator)
            where TRegexProvider : RegexProvider<T>
        {
            if (activator == null) throw new ArgumentNullException(nameof(activator));

            return activator(pattern, options).Matches(input);
        }
        #endregion
        #endregion
    }
}
