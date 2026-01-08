using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// Controller implementation for inbox operations.
    /// </summary>
    public class InboxController : IInboxController, IInitializable, IDisposable
    {
        [Inject] private readonly InboxModel _model;
        [Inject(Optional = true)] private readonly IInboxView _view;
        [Inject(Optional = true)] private readonly IInboxNetworkService _network;

        private bool _isRefreshing;

        public int UnreadCount => _model.UnreadCount;
        public int ClaimableCount => _model.ClaimableCount;
        public bool IsRefreshing => _isRefreshing;

        public event Action OnInboxUpdated;
        public event Action<InboxItem> OnItemClaimed;
        public event Action<InboxItem> OnItemReceived;

        public void Initialize()
        {
            _model.OnItemsUpdated += HandleItemsUpdated;
            _model.OnItemAdded += HandleItemAdded;
        }

        public async UniTask<bool> FetchInboxAsync()
        {
            if (_isRefreshing)
            {
                Debug.LogWarning("[InboxController] Already refreshing inbox.");
                return false;
            }

            _isRefreshing = true;

            try
            {
                if (_network != null)
                {
                    var items = await _network.FetchInboxItemsAsync();
                    _model.SetItems(items);
                    _model.RemoveExpiredItems();
                    return true;
                }

                Debug.LogWarning("[InboxController] No network service configured.");
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        public async UniTask<bool> ClaimItemAsync(string itemId)
        {
            var item = _model.GetItem(itemId);
            if (item == null)
            {
                Debug.LogWarning($"[InboxController] Item not found: {itemId}");
                return false;
            }

            if (!item.CanClaim)
            {
                Debug.LogWarning($"[InboxController] Item cannot be claimed: {itemId}");
                return false;
            }

            if (_network != null)
            {
                var success = await _network.ClaimItemAsync(itemId);
                if (!success) return false;
            }

            item.IsClaimed = true;
            _view?.ShowClaimAnimation(item);
            OnItemClaimed?.Invoke(item);

            return true;
        }

        public async UniTask<IReadOnlyList<InboxItem>> ClaimAllAsync()
        {
            var claimableItems = _model.GetClaimableItems();
            var claimedItems = new List<InboxItem>();

            foreach (var item in claimableItems)
            {
                var success = await ClaimItemAsync(item.Id);
                if (success)
                {
                    claimedItems.Add(item);
                }
            }

            return claimedItems;
        }

        public async UniTask<IReadOnlyList<InboxItem>> ClaimAllInCategoryAsync(InboxItemCategory category)
        {
            var claimableItems = _model.GetClaimableItems(category);
            var claimedItems = new List<InboxItem>();

            foreach (var item in claimableItems)
            {
                var success = await ClaimItemAsync(item.Id);
                if (success)
                {
                    claimedItems.Add(item);
                }
            }

            return claimedItems;
        }

        public void MarkAsRead(string itemId)
        {
            var item = _model.GetItem(itemId);
            if (item != null && !item.IsRead)
            {
                item.IsRead = true;
                OnInboxUpdated?.Invoke();
            }
        }

        public void MarkAllAsRead()
        {
            var hasChanges = false;
            foreach (var item in _model.Items)
            {
                if (!item.IsRead)
                {
                    item.IsRead = true;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                OnInboxUpdated?.Invoke();
            }
        }

        public async UniTask<bool> DeleteItemAsync(string itemId)
        {
            if (_network != null)
            {
                var success = await _network.DeleteItemAsync(itemId);
                if (!success) return false;
            }

            return _model.RemoveItem(itemId);
        }

        public int GetClaimableCount(InboxItemCategory category)
        {
            return _model.GetClaimableItems(category).Count;
        }

        /// <summary>
        /// Called when a new item is received (e.g., via socket).
        /// </summary>
        public void OnItemReceived(InboxItem item)
        {
            _model.AddItem(item);
            OnItemReceived?.Invoke(item);
        }

        private void HandleItemsUpdated()
        {
            _view?.RefreshItems(_model.Items);
            OnInboxUpdated?.Invoke();
        }

        private void HandleItemAdded(InboxItem item)
        {
            _view?.ShowNewItemNotification(item);
        }

        public void Dispose()
        {
            _model.OnItemsUpdated -= HandleItemsUpdated;
            _model.OnItemAdded -= HandleItemAdded;
        }
    }

    /// <summary>
    /// Network service interface for inbox operations.
    /// Games implement this to connect to their backend.
    /// </summary>
    public interface IInboxNetworkService
    {
        UniTask<IReadOnlyList<InboxItem>> FetchInboxItemsAsync();
        UniTask<bool> ClaimItemAsync(string itemId);
        UniTask<bool> DeleteItemAsync(string itemId);
    }
}
