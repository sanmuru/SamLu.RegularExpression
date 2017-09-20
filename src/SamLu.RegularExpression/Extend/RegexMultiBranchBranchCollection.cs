using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    /// <summary>
    /// 包含所有正则复数分支的分支的集合。
    /// </summary>
    /// <typeparam name="T">正则接受的对象的类型。</typeparam>
    public class RegexMultiBranchBranchCollection<T> : IDictionary<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>, ICollection<RegexMultiBranchBranch<T>>
    {
        private IDictionary<RegexMultiBranchBranchPredicate<T>, RegexMultiBranchBranch<T>> innerDic;

        /// <summary>
        /// 初始化 <see cref="RegexMultiBranchBranchCollection{T}"/> 类的新实例。
        /// </summary>
        public RegexMultiBranchBranchCollection() =>
            this.innerDic = new Dictionary<RegexMultiBranchBranchPredicate<T>, RegexMultiBranchBranch<T>>();

        public RegexMultiBranchBranchCollection(IEnumerable<RegexMultiBranchBranch<T>> branches) :
            this((branches ?? throw new ArgumentNullException(nameof(branches)))
                .ToDictionary(branch => branch.Predicate)
            )
        { }

        public RegexMultiBranchBranchCollection(IDictionary<RegexMultiBranchBranchPredicate<T>, RegexObject<T>> dictionary) :
            this((dictionary ?? throw new ArgumentNullException(nameof(dictionary)))
                .ToDictionary(
                    (pair => pair.Key),
                    (pair => new RegexMultiBranchBranch<T>(pair.Key, pair.Value))
                )
            )
        { }

        protected RegexMultiBranchBranchCollection(IDictionary<RegexMultiBranchBranchPredicate<T>, RegexMultiBranchBranch<T>> dictionary) =>
            this.innerDic = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        public RegexObject<T> this[RegexMultiBranchBranchPredicate<T> key]
        {
            get => this.innerDic[key].Pattern;
            set => this.innerDic[key].Pattern = value;
        }

        #region Predicates
        /// <summary>
        /// 获取 <see cref="RegexMultiBranchBranchCollection{T}"/> 的检测条件集合。
        /// </summary>
        public ICollection<RegexMultiBranchBranchPredicate<T>> Predicates => this.innerDic.Keys;

        ICollection<RegexMultiBranchBranchPredicate<T>> IDictionary<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>.Keys => this.Predicates;
        #endregion

        #region Patterns
        /// <summary>
        /// 获取 <see cref="RegexMultiBranchBranchCollection{T}"/> 的正则模式集合。
        /// </summary>
        public ICollection<RegexObject<T>> Patterns => new ReadOnlyCollection<RegexObject<T>>(this.innerDic.Values.Select(branch => branch.Pattern).ToList());

        ICollection<RegexObject<T>> IDictionary<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>.Values => this.Patterns;
        #endregion

        /// <summary>
        /// 获取 <see cref="RegexMultiBranchBranchCollection{T}"/> 包含的元素数。
        /// </summary>
        public int Count => this.innerDic.Count;

        #region IsReadOnly
        bool ICollection<RegexMultiBranchBranch<T>>.IsReadOnly => this.innerDic.IsReadOnly;

        bool ICollection<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>.IsReadOnly => this.innerDic.IsReadOnly;
        #endregion

        #region Add
        public void Add(RegexMultiBranchBranchPredicate<T> key, RegexObject<T> value) =>
            this.innerDic.Add(key, new RegexMultiBranchBranch<T>(key, value));

        public void Add(RegexMultiBranchBranch<T> branch) =>
            this.innerDic.Add(branch.Predicate, branch);

        void ICollection<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>.Add(KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>> item) =>
            this.Add(item.Key, item.Value);
        #endregion

        public void Clear() => this.innerDic.Clear();

        #region Contains
        public bool Contains(RegexMultiBranchBranch<T> branch)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));

            return this.ContainsPredicate(branch.Predicate) &&
                EqualityComparer<RegexMultiBranchBranch<T>>.Default.Equals(branch, this.innerDic[branch.Predicate]);
        }

        bool ICollection<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>.Contains(KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>> item) =>
            this.ContainsPredicate(item.Key) && EqualityComparer<RegexObject<T>>.Default.Equals(this[item.Key], item.Value);
        #endregion

        #region ContainsPredicate
        public bool ContainsPredicate(RegexMultiBranchBranchPredicate<T> predicate) =>
            this.innerDic.ContainsKey(predicate);

        bool IDictionary<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>.ContainsKey(RegexMultiBranchBranchPredicate<T> key) =>
            this.ContainsPredicate(key);
        #endregion

        #region CopyTo
        public void CopyTo(RegexMultiBranchBranch<T>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"{nameof(arrayIndex)} 的值小于 0 。");
            if (array.Length - arrayIndex < this.Count) throw new ArgumentException($"源 {typeof(RegexMultiBranchBranchPredicate<T>).FullName} 中的元素数目大于从目标 {nameof(array)} 的 {nameof(arrayIndex)} 从头到尾的可用空间。");

            var pairArray = new KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexMultiBranchBranch<T>>[array.Length];
            this.innerDic.CopyTo(pairArray, arrayIndex);

            for (int i = arrayIndex; i < array.Length; i++)
                array[i] = new RegexMultiBranchBranch<T>(pairArray[i].Key, pairArray[i].Value.Pattern);
        }

        void ICollection<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>.CopyTo(KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"{nameof(arrayIndex)} 的值小于 0 。");
            if (array.Length - arrayIndex < this.Count) throw new ArgumentException($"源 {typeof(RegexMultiBranchBranchPredicate<T>).FullName} 中的元素数目大于从目标 {nameof(array)} 的 {nameof(arrayIndex)} 从头到尾的可用空间。");

            var pairArray = new KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexMultiBranchBranch<T>>[array.Length];
            this.innerDic.CopyTo(pairArray, arrayIndex);

            for (int i = arrayIndex; i < array.Length; i++)
                array[i] = new KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>(pairArray[i].Key, pairArray[i].Value.Pattern);
        }
        #endregion

        #region GetEnumerator
        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<RegexMultiBranchBranch<T>> GetEnumerator()
        {
            var enumerator = this.innerDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return new RegexMultiBranchBranch<T>(current.Key, current.Value.Pattern);
            }
        }

        IEnumerator<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>> IEnumerable<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>.GetEnumerator()
        {
            var enumerator = this.innerDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return new KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>(current.Key, current.Value.Pattern);
            }
        }
        #endregion

        #region
        public bool Remove(RegexMultiBranchBranchPredicate<T> key) => this.innerDic.Remove(key);

        public bool Remove(RegexMultiBranchBranch<T> branch)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));

            if (this.Contains(branch))
            {
                this.Remove(branch.Predicate);
                return true;
            }
            else return false;
        }

        bool ICollection<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>.Remove(KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>> item)
        {
            if (((ICollection<KeyValuePair<RegexMultiBranchBranchPredicate<T>, RegexObject<T>>>)this).Contains(item))
            {
                this.Remove(item.Key);
                return true;
            }
            else return false;
        }
        #endregion

        public bool TryGetValue(RegexMultiBranchBranchPredicate<T> key, out RegexObject<T> value)
        {
            if (this.ContainsPredicate(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
