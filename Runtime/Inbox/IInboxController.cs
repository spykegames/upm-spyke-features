using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// Controller interface for inbox operations.
    /// </summary>
    public interface IInboxController
    {
        /// <summary>
        /// Fetches inbox items from the server.
        /// </summary>
        UniTask<bool> FetchInboxAsync();

        /// <summary>
        /// Claims a specific item.
        /// </summary>
        UniTask<bool> ClaimItemAsync(string itemId);

        /// <summary>
        /// Claims all claimable items.
        /// </summary>
        UniTask<IReadOnlyList<InboxItem>> ClaimAllAsync();

        /// <summary>
        /// Claims all items in a specific category.
        /// </summary>
        UniTask<IReadOnlyList<InboxItem>> ClaimAllInCategoryAsync(InboxItemCategory category);

        /// <summary>
        /// Marks an item as read.
        /// </summary>
        void MarkAsRead(string itemId);

        /// <summary>
        /// Marks all items as read.
        /// </summary>
        void MarkAllAsRead();

        /// <summary>
        /// Deletes an item.
        /// </summary>
        UniTask<bool> DeleteItemAsync(string itemId);

        /// <summary>
        /// Gets the count of unread items.
        /// </summary>
        int UnreadCount { get; }

        /// <summary>
        /// Gets the count of claimable items.
        /// </summary>
        int ClaimableCount { get; }

        /// <summary>
        /// Gets the count of claimable items in a category.
        /// </summary>
        int GetClaimableCount(InboxItemCategory category);

        /// <summary>
        /// Whether inbox is currently being refreshed.
        /// </summary>
        bool IsRefreshing { get; }

        /// <summary>
        /// Fired when inbox is updated.
        /// </summary>
        event Action OnInboxUpdated;

        /// <summary>
        /// Fired when an item is claimed.
        /// </summary>
        event Action<InboxItem> OnItemClaimed;

        /// <summary>
        /// Fired when an item is received.
        /// </summary>
        event Action<InboxItem> OnItemReceived;
    }
}
