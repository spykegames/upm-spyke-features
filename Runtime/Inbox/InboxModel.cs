using System;
using System.Collections.Generic;
using System.Linq;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// Model holding inbox state and items.
    /// </summary>
    public class InboxModel
    {
        private readonly List<InboxItem> _items = new();

        /// <summary>
        /// All inbox items.
        /// </summary>
        public IReadOnlyList<InboxItem> Items => _items;

        /// <summary>
        /// Count of unread items.
        /// </summary>
        public int UnreadCount => _items.Count(i => !i.IsRead);

        /// <summary>
        /// Count of claimable items.
        /// </summary>
        public int ClaimableCount => _items.Count(i => i.CanClaim);

        /// <summary>
        /// Whether there are any unread items.
        /// </summary>
        public bool HasUnread => UnreadCount > 0;

        /// <summary>
        /// Whether there are any claimable items.
        /// </summary>
        public bool HasClaimable => ClaimableCount > 0;

        /// <summary>
        /// Fired when items are updated.
        /// </summary>
        public event Action OnItemsUpdated;

        /// <summary>
        /// Fired when an item is added.
        /// </summary>
        public event Action<InboxItem> OnItemAdded;

        /// <summary>
        /// Fired when an item is removed.
        /// </summary>
        public event Action<InboxItem> OnItemRemoved;

        /// <summary>
        /// Sets all items, replacing existing ones.
        /// </summary>
        public void SetItems(IEnumerable<InboxItem> items)
        {
            _items.Clear();
            if (items != null)
            {
                _items.AddRange(items);
            }
            SortByCreatedTime();
            OnItemsUpdated?.Invoke();
        }

        /// <summary>
        /// Adds a single item.
        /// </summary>
        public void AddItem(InboxItem item)
        {
            if (item == null) return;

            _items.Add(item);
            SortByCreatedTime();
            OnItemAdded?.Invoke(item);
            OnItemsUpdated?.Invoke();
        }

        /// <summary>
        /// Removes an item by ID.
        /// </summary>
        public bool RemoveItem(string itemId)
        {
            var item = GetItem(itemId);
            if (item == null) return false;

            _items.Remove(item);
            OnItemRemoved?.Invoke(item);
            OnItemsUpdated?.Invoke();
            return true;
        }

        /// <summary>
        /// Gets an item by ID.
        /// </summary>
        public InboxItem GetItem(string itemId)
        {
            return _items.Find(i => i.Id == itemId);
        }

        /// <summary>
        /// Gets items by category.
        /// </summary>
        public IReadOnlyList<InboxItem> GetItemsByCategory(InboxItemCategory category)
        {
            return _items.Where(i => i.Category == category).ToList();
        }

        /// <summary>
        /// Gets all claimable items.
        /// </summary>
        public IReadOnlyList<InboxItem> GetClaimableItems()
        {
            return _items.Where(i => i.CanClaim).ToList();
        }

        /// <summary>
        /// Gets claimable items by category.
        /// </summary>
        public IReadOnlyList<InboxItem> GetClaimableItems(InboxItemCategory category)
        {
            return _items.Where(i => i.CanClaim && i.Category == category).ToList();
        }

        /// <summary>
        /// Removes all expired items.
        /// </summary>
        public int RemoveExpiredItems()
        {
            var expiredItems = _items.Where(i => i.IsExpired).ToList();
            foreach (var item in expiredItems)
            {
                _items.Remove(item);
                OnItemRemoved?.Invoke(item);
            }

            if (expiredItems.Count > 0)
            {
                OnItemsUpdated?.Invoke();
            }

            return expiredItems.Count;
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            OnItemsUpdated?.Invoke();
        }

        private void SortByCreatedTime()
        {
            _items.Sort((a, b) => b.CreatedTimestamp.CompareTo(a.CreatedTimestamp));
        }
    }
}
