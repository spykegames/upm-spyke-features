using System.Collections.Generic;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// View interface for leaderboard UI.
    /// Implement this for your specific UI implementation.
    /// </summary>
    public interface ILeaderboardView
    {
        /// <summary>
        /// Refreshes the entry list display.
        /// </summary>
        void RefreshEntries(IReadOnlyList<LeaderboardEntry> entries);

        /// <summary>
        /// Highlights the current user's entry.
        /// </summary>
        void HighlightCurrentUser(LeaderboardEntry entry);

        /// <summary>
        /// Shows or hides loading indicator.
        /// </summary>
        void ShowLoading(bool show);

        /// <summary>
        /// Scrolls to a specific rank.
        /// </summary>
        void ScrollToRank(int rank);

        /// <summary>
        /// Scrolls to the current user's position.
        /// </summary>
        void ScrollToCurrentUser();

        /// <summary>
        /// Updates the tab UI.
        /// </summary>
        void UpdateTabUI(LeaderboardTab tab);

        /// <summary>
        /// Updates the scope UI.
        /// </summary>
        void UpdateScopeUI(LeaderboardScope scope);

        /// <summary>
        /// Shows the leaderboard panel.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the leaderboard panel.
        /// </summary>
        void Hide();
    }
}
