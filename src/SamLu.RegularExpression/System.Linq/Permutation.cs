using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class Permutation
	{
		/// <summary>
		/// 	以特定的秩生成一组元素的排列。
		/// </summary>
		/// <remarks>
		/// 	对给定的一组元素中任意选取特定个进行排列，并返回所有可能的排列情况。
		/// </remarks>
		/// <param name="source">进行排列的元素列表</param>
		/// <param name="rank">秩，选取元素的个数</param>
		/// <typeparam name="T">元素的类型</typeparam>
		/// <returns>
		/// 	<para>包含所有可能的排列情况。</para>
		/// 	<para>如果 <paramref name="source" /> 中的元素个数为零，则返回一个空列表。</para>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// 	<paramref name="source" /> 的值为 <see langword="null"/> 。
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	<paramref name="rank" /> 的值为负数。
		/// </exception>
		public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> source, int rank = int.MaxValue)
		{
			ArgumentNullException.ThrowIfNull(source);
			if (rank < 0) throw new ArgumentOutOfRangeException(nameof(rank), rank, "rank应不小于零。");

			IList<T> elements = new List<T>(source);
			rank = Math.Min(rank, elements.Count);

			if ((elements.Count == 0) || (rank == 0))
				return Enumerable.Empty<IList<T>>();

			IList<T> item = new List<T>(elements.Count);

			return Permutation.GetPermutationsInternal(elements, item, 1, rank);
		}

		/// <summary>
		/// 	生成一组元素在所有秩上排列的集合。
		/// </summary>
		/// <remarks>
		/// 	对给定的一组元素从最低秩到最高秩进行排列，并返回所有可能的排列情况。
		/// </remarks>
		/// <param name="source">进行排列的元素列表</param>
		/// <typeparam name="T">元素的类型</typeparam>
		/// <returns>
		/// 	<para>包含所有可能的排列情况。</para>
		/// 	<para>如果 <paramref name="source" /> 中的元素个数为零，则返回一个空列表。</para>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// 	<paramref name="source" /> 的值为 <see langword="null"/> 。
		/// </exception>
		public static IEnumerable<IEnumerable<T>> GetOptionalPermutations<T>(this IEnumerable<T> source)
		{
			ArgumentNullException.ThrowIfNull(source);

			IList<T> elements = new List<T>(source);
			for (int rank = 1; rank <= elements.Count; rank++)
				foreach (var _item in elements.GetPermutations(rank))
					yield return _item;
		}

		/// <summary>
		/// 生成一组元素在所有秩上排列的集合的迭代器的内部方法。
		/// </summary>
		/// <remarks>
		/// <para>支持对给定的一组元素从指定的低秩 <paramref name="currentRank" /> 到最高秩 <paramref name="rank" /> 进行排列操作。</para>
		/// <para>此方法返回的集合为惰性执行模型，在循环遍历集合时将单独逐步计算各项。</para>
		/// <para><strong>此方法为内部方法，仅供内部维护人员参阅。</strong></para>
		/// </remarks>
		/// <typeparam name="T">元素的类型。</typeparam>
		/// <param name="elements">进行排列的元素列表。</param>
		/// <param name="item">其中一种排列情况。</param>
		/// <param name="currentRank">当前秩。</param>
		/// <param name="rank">秩。</param>
		/// <returns>
		/// <para>包含所有可能的排列情况的迭代器。</para>
		/// <para>如果 <paramref name="elements" /> 中的元素个数为零，则返回一个空列表。</para>
		/// </returns>
		internal static IEnumerable<IEnumerable<T>> GetPermutationsInternal<T>(IList<T> elements, IList<T> item, int currentRank, int rank)
		{
			if (currentRank == rank + 1)
				yield return new List<T>(item);

			for (int index = 0; index < elements.Count; index++)
			{
				item.Add(elements[index]);
				elements.RemoveAt(index);
				foreach (var _item in GetPermutationsInternal(elements, item, currentRank + 1, rank)) yield return _item;
				elements.Insert(index, item[item.Count - 1]);
				item.RemoveAt(item.Count - 1);
			}
		}
	}
}
