using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class Combination
	{
		/// <summary>
		/// 以特定的秩生成一组元素的组合。
		/// </summary>
		/// <remarks>
		/// <para>对给定的一组元素中任意选取特定个进行组合，并返回所有可能的组合情况。</para>
		/// <para>此方法返回的集合为惰性执行模型，在循环遍历集合时将单独逐步计算各项。</para>
		/// </remarks>
		/// <param name="source">进行组合的元素列表。</param>
		/// <param name="rank">秩，选取元素的个数。</param>
		/// <typeparam name="T">元素的类型。</typeparam>
		/// <returns>
		/// <para>包含所有可能的组合情况。</para>
		/// <para>如果 <paramref name="source" /> 中的元素个数为零，则返回一个空的列表。</para>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source" /> 为 <see langword="null"/> 。
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="rank" /> 的值为负数，或者大于 <paramref name="source" /> 中元素的个数。
		/// </exception>
		public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IEnumerable<T> source, int rank = int.MaxValue)
		{
			ArgumentNullException.ThrowIfNull(source);
			if (rank < 0) throw new ArgumentOutOfRangeException(nameof(rank), rank, "rank应不小于零。");

			IList<T> elements = new List<T>(source);
			rank = Math.Min(rank, elements.Count);

			if ((elements.Count == 0) || (rank == 0))
				return Enumerable.Empty<IEnumerable<T>>();

			IList<T> item = new List<T>(elements.Count);
			int[] indexes = new int[rank];

			return Combination.GetCombinationsInternal<T>(elements, item, indexes, 1, rank);
		}

		/// <summary>
		/// 生成一组元素在所有秩上组合的集合。
		/// </summary>
		/// <remarks>
		/// <para>对给定的一组元素从最低秩到最高秩进行组合，并返回所有可能的组合情况。</para>
		/// <para>此方法返回的集合为惰性执行模型，在循环遍历集合时将单独逐步计算各项。</para>
		/// </remarks>
		/// <param name="source">进行组合的元素列表。</param>
		/// <typeparam name="T">元素的类型。</typeparam>
		/// <returns>
		/// <para>包含所有可能的组合情况。</para>
		/// <para>如果 <paramref name="source" /> 中的元素个数为零，则返回一个空列表。</para>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source" /> 为 <see langword="null"/> 。
		/// </exception>
		public static IEnumerable<IEnumerable<T>> GetOptionalCombinations<T>(this IEnumerable<T> source)
		{
			ArgumentNullException.ThrowIfNull(source);

			IList<T> elements = new List<T>(source);
			for (int rank = 1; rank <= elements.Count; rank++)
				foreach (var _item in elements.GetCombinations(rank))
					yield return _item;
		}

		/// <summary>
		/// 生成一组元素在所有秩上组合的集合的迭代器的内部方法。
		/// </summary>
		/// <remarks>
		/// <para>支持对给定的一组元素从指定的低秩 <paramref name="currentRank" /> 到最高秩 <paramref name="rank" /> 进行组合操作。</para>
		/// <para>此方法返回的集合为惰性执行模型，在循环遍历集合时将单独逐步计算各项。</para>
		/// <para><strong>此方法为内部方法，仅供内部维护人员参阅。</strong></para>
		/// </remarks>
		/// <typeparam name="T">元素的类型。</typeparam>
		/// <param name="elements">进行组合的元素列表。</param>
		/// <param name="item">其中一种组合情况。</param>
		/// <param name="indexes">各层迭代时选择的元素索引</param>
		/// <param name="currentRank">当前秩。</param>
		/// <param name="rank">秩。</param>
		/// <returns>
		/// <para>包含所有可能的组合情况的迭代器。</para>
		/// <para>如果 <paramref name="elements" /> 中的元素个数为零，则返回一个空列表。</para>
		/// </returns>
		private static IEnumerable<IEnumerable<T>> GetCombinationsInternal<T>(IList<T> elements, IList<T> item, int[] indexes, int currentRank, int rank)
		{
			if (currentRank == rank + 1)
			{
				yield return new List<T>(item);

				yield break;
			}

			for (
				indexes[currentRank - 1] = ((currentRank == 1) ? 0 : (indexes[currentRank - 2] + 1));
				indexes[currentRank - 1] < (elements.Count + currentRank - rank);
				indexes[currentRank - 1]++
			)
			{
				item.Add(elements[indexes[currentRank - 1]]);
				foreach (var _item in Combination.GetCombinationsInternal(elements, item, indexes, currentRank + 1, rank)) yield return _item;
				item.RemoveAt(item.Count - 1);
			}
		}
	}
}
