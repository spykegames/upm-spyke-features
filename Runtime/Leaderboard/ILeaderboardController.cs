using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Controller interface for leaderboard operations.
    /// </summary>
    public interface ILeaderboardController
    {
        /// <summary>
        /// Fetches leaderboard data for the specified tab and scope.
        /// </summary>
        UniTask<bool> FetchLeaderboardAsync(LeaderboardTab tab, LeaderboardScope scope);

        /// <summary>
        /// Fetches all leaderboards.
        /// </summary>
        UniTask<bool> FetchAllLeaderboardsAsync();

        /// <summary>
        /// Gets entries for a specific tab and scope.
        /// </summary>
        IReadOnlyList<LeaderboardEntry> GetEntries(LeaderboardTab tab, LeaderboardScope scope);

        /// <summary>
        /// Gets the current user's entry.
        /// </summary>
        LeaderboardEntry GetCurrentUserEntry();

        /// <summary>
        /// Gets the current user's rank for a leaderboard.
        /// </summary>
        int GetCurrentUserRank(LeaderboardTab tab, LeaderboardScope scope);

        /// <summary>
        /// Updates the local user's score.
        /// </summary>
        void UpdateLocalScore(long score, long subScore = 0);

        /// <summary>
        /// Switches to a different tab.
        /// </summary>
        void SwitchTab(LeaderboardTab tab);

        /// <summary>
        /// Switches to a different scope.
        /// </summary>
        void SwitchScope(LeaderboardScope scope);

        /// <summary>
        /// Current active tab.
        /// </summary>
        LeaderboardTab CurrentTab { get; }

        /// <summary>
        /// Current active scope.
        /// </summary>
        LeaderboardScope CurrentScope { get; }

        /// <summary>
        /// Whether data is currently being fetched.
        /// </summary>
        bool IsFetching { get; }

        /// <summary>
        /// Fired when leaderboard data is updated.
        /// </summary>
        event Action OnLeaderboardUpdated;

        /// <summary>
        /// Fired when tab changes.
        /// </summary>
        event Action<LeaderboardTab> OnTabChanged;

        /// <summary>
        /// Fired when scope changes.
        /// </summary>
        event Action<LeaderboardScope> OnScopeChanged;

        /// <summary>
        /// Fired when an entry is clicked.
        /// </summary>
        event Action<LeaderboardEntry> OnEntryClicked;
    }
}
