using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Extend
{
    public class RegexBalanceGroup<T> : RegexGroup<T>
    {
        protected RegexBalanceGroupItem<T> openItem;
        protected IList<RegexBalanceGroupItem<T>> subItems = new List<RegexBalanceGroupItem<T>>();
        protected RegexBalanceGroupItem<T> closeItem;

        public RegexBalanceGroupItem<T> OpenItem => this.openItem;
        public ICollection<RegexBalanceGroupItem<T>> SubItems => new ReadOnlyCollection<RegexBalanceGroupItem<T>>(this.subItems);
        public RegexBalanceGroupItem<T> CloseItem => this.closeItem;

        public RegexBalanceGroup(RegexObject<T> regex, RegexObject<T> openItemInnerRegex, RegexObject<T> closeItemInnerRegex, object id = null) :
            this(
                regex,
                openItemInnerRegex,
                Enumerable.Empty<RegexObject<T>>(),
                closeItemInnerRegex,
                id
            )
        { }

        public RegexBalanceGroup(RegexObject<T> regex, RegexObject<T> openItemInnerRegex, IEnumerable<RegexObject<T>> subItemInnerRegexs, RegexObject<T> closeItemInnerRegex, object id = null) : base(regex, id, true)
        {
            if (openItemInnerRegex == null) throw new ArgumentNullException(nameof(openItemInnerRegex));
            if (subItemInnerRegexs == null) throw new ArgumentNullException(nameof(subItemInnerRegexs));
            if (closeItemInnerRegex == null) throw new ArgumentNullException(nameof(closeItemInnerRegex));


            this.RegisterOpenItem(
                new RegexBalanceGroupOpenItem<T, int>(
                    openItemInnerRegex,
                    (() => this.subItems.Count)
                )
            );
            foreach (var subItem in
                subItemInnerRegexs.Select(subItemInnerRegex =>
                      new RegexBalanceGroupSubItem<T, int>(
                          subItemInnerRegex,
                          (seed => seed--),
                          (seed => seed <= 0)
                      )
                )
            ) this.RegisterSubItem(subItem);
            this.RegisterCloseItem(
                new RegexBalanceGroupCloseItem<T, int>(
                    closeItemInnerRegex,
                    (seed => --seed == 0)
                )
            );
        }

        public RegexBalanceGroup(RegexObject<T> regex, RegexBalanceGroupItem<T> openItem, IEnumerable<RegexBalanceGroupItem<T>> subItems, RegexBalanceGroupItem<T> closeItem, object id = null) : base(regex, id, true)
        {
            if (openItem == null) throw new ArgumentNullException(nameof(openItem));
            if (subItems == null) throw new ArgumentNullException(nameof(subItems));
            if (closeItem == null) throw new ArgumentNullException(nameof(closeItem));

            this.RegisterOpenItem(openItem);
            foreach (var subItem in subItems) this.RegisterSubItem(subItem);
            this.RegisterCloseItem(closeItem);
        }

        #region Register/Deregister Item
        public void RegisterOpenItem(RegexBalanceGroupItem<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.___balanceGroup == this)
            {
                if (this.openItem == item) return;
                else
                    throw new InvalidOperationException("指定平衡组项已在此平衡组中注册，且不为开始项。");
            }
            else if (item.___balanceGroup == null)
            {
                this.openItem = item;
                this.RegisterItem(item);
            }
            else throw new InvalidOperationException("指定平衡组项已注册为另一个平衡组的项。");
        }

        public void DeregisterOpenItem(RegexBalanceGroupItem<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.___balanceGroup == this)
            {
                if (this.openItem == item)
                {
                    this.DeregisterItem(item);
                    this.openItem = null;
                }
                else
                    throw new InvalidOperationException("指定平衡组项已在此平衡组中注册，且不为开始项。");
            }
            else if (item.___balanceGroup == null)
                throw new InvalidOperationException("指定平衡组项未注册为任何一个平衡组的项。");
            else
                throw new InvalidOperationException("指定平衡组项已注册为另一个平衡组的项。");
        }

        public void RegisterCloseItem(RegexBalanceGroupItem<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.___balanceGroup == this)
            {
                if (this.closeItem == item) return;
                else
                    throw new InvalidOperationException("指定平衡组项已在此平衡组中注册，且不为结束项。");
            }
            else if (item.___balanceGroup == null)
            {
                this.closeItem = item;
                this.RegisterItem(item);
            }
            else
                throw new InvalidOperationException("指定平衡组项已注册为另一个平衡组的项。");
        }

        public void DeregisterCloseItem(RegexBalanceGroupItem<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.___balanceGroup == this)
            {
                if (this.closeItem == item)
                {
                    this.DeregisterItem(item);
                    this.closeItem = null;
                }
                else
                    throw new InvalidOperationException("指定平衡组项已在此平衡组中注册，且不为结束项。");
            }
            else if (item.___balanceGroup == null)
                throw new InvalidOperationException("指定平衡组项未注册为任何一个平衡组的项。");
            else
                throw new InvalidOperationException("指定平衡组项已注册为另一个平衡组的项。");
        }

        public void RegisterSubItem(RegexBalanceGroupItem<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.___balanceGroup == this)
            {
                if (this.openItem == item || this.closeItem == item)
                    throw new InvalidOperationException("指定平衡组项已在此平衡组中注册，且不为子项。");
                else return;
            }
            else if (item.___balanceGroup == null)
            {
                this.SubItems.Add(item);
                this.RegisterItem(item);
            }
            else
                throw new InvalidOperationException("指定平衡组项已注册为另一个平衡组的项。");
        }

        public void DeregisterSubItem(RegexBalanceGroupItem<T> item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.___balanceGroup == this)
            {
                if (this.openItem == item || this.closeItem == item)
                    throw new InvalidOperationException("指定平衡组项已在此平衡组中注册，且不为子项。");
                else
                {
                    this.DeregisterItem(item);
                    this.subItems.Remove(item);
                }
            }
            else if (item.___balanceGroup == null)
                throw new InvalidOperationException("指定平衡组项未注册为任何一个平衡组的项。");
            else
                throw new InvalidOperationException("指定平衡组项已注册为另一个平衡组的项。");
        }

        protected internal void RegisterItem(RegexBalanceGroupItem<T> item)
        {
            item.___balanceGroup = this;
        }

        protected internal void DeregisterItem(RegexBalanceGroupItem<T> item)
        {
            item.___balanceGroup = null;
        }
        #endregion

        protected internal override RegexObject<T> Clone()
        {
            return new RegexBalanceGroup<T>(base.innerRegex, this.openItem, this.subItems, this.closeItem);
        }
    }
}
