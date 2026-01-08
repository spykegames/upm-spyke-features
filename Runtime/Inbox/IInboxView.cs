using System.Collections.Generic;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// View interface for inbox UI.
    /// Implement this for your specific UI implementation.
    /// </summary>
    public interface IInboxView
    {
        /// <summary>
        /// Refreshes the item list display.
        /// </summary>
        void RefreshItems(IReadOnlyList<InboxItem> items);

        /// <summary>
        /// Shows claim animation for an item.
        /// </summary>
        void ShowClaimAnimation(InboxItem item);

        /// <summary>
        /// Shows notification for a new item.
        /// </summary>
        void ShowNewItemNotification(InboxItem item);

        /// <summary>
        /// Shows the inbox popup/panel.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the inbox popup/panel.
        /// </summary>
        void Hide();

        /// <summary>
        /// Updates the notification badge count.
        /// </summary>
        void UpdateBadgeCount(int count);
    }
}
