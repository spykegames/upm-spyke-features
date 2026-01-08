using System;
using System.Collections.Generic;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Model holding leaderboard data for different tabs and scopes.
    /// </summary>
    public class LeaderboardModel
    {
        private readonly Dictionary<(LeaderboardTab, LeaderboardScope), List<LeaderboardEntry>> _entries = new();
        private LeaderboardEntry _currentUserEntry;

        /// <summary>
        /// The current user's entry (across any leaderboard).
        /// </summary>
        public LeaderboardEntry CurrentUserEntry => _currentUserEntry;

        /// <summary>
        /// Fired when leaderboard data changes.
        /// </summary>
        public event Action OnDataChanged;

        /// <summary>
        /// Fired when current user's entry changes.
        /// </summary>
        public event Action<LeaderboardEntry> OnCurrentUserEntryChanged;

        /// <summary>
        /// Gets entries for a specific tab and scope.
        /// </summary>
        public IReadOnlyList<LeaderboardEntry> GetEntries(LeaderboardTab tab, LeaderboardScope scope)
        {
            var key = (tab, scope);
            if (_entries.TryGetValue(key, out var entries))
            {
                return entries;
            }
            return Array.Empty<LeaderboardEntry>();
        }

        /// <summary>
        /// Sets entries for a specific tab and scope.
        /// </summary>
        public void SetEntries(LeaderboardTab tab, LeaderboardScope scope, IEnumerable<LeaderboardEntry> entries)
        {
            var key = (tab, scope);
            if (!_entries.ContainsKey(key))
            {
                _entries[key] = new List<LeaderboardEntry>();
            }
            else
            {
                _entries[key].Clear();
            }

            if (entries != null)
            {
                _entries[key].AddRange(entries);
            }

            // Update current user entry if found
            foreach (var entry in _entries[key])
            {
                if (entry.IsCurrentUser)
                {
                    UpdateCurrentUserEntry(entry);
                    break;
                }
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Updates the current user's entry.
        /// </summary>
        public void UpdateCurrentUserEntry(LeaderboardEntry entry)
        {
            _currentUserEntry = entry;
            OnCurrentUserEntryChanged?.Invoke(entry);
        }

        /// <summary>
        /// Updates the current user's score and triggers recalculation.
        /// </summary>
        public void UpdateCurrentUserScore(long score, long subScore = 0)
        {
            if (_currentUserEntry != null)
            {
                _currentUserEntry.Score = score;
                _currentUserEntry.SubScore = subScore;
                OnCurrentUserEntryChanged?.Invoke(_currentUserEntry);
            }
        }

        /// <summary>
        /// Gets the current user's rank for a specific leaderboard.
        /// </summary>
        public int GetCurrentUserRank(LeaderboardTab tab, LeaderboardScope scope)
        {
            var entries = GetEntries(tab, scope);
            foreach (var entry in entries)
            {
                if (entry.IsCurrentUser)
                {
                    return entry.Rank;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds an entry by ID.
        /// </summary>
        public LeaderboardEntry FindEntry(LeaderboardTab tab, LeaderboardScope scope, string id)
        {
            var entries = GetEntries(tab, scope);
            foreach (var entry in entries)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>
        /// Clears all entries.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
            _currentUserEntry = null;
            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Clears entries for a specific tab and scope.
        /// </summary>
        public void Clear(LeaderboardTab tab, LeaderboardScope scope)
        {
            var key = (tab, scope);
            if (_entries.ContainsKey(key))
            {
                _entries[key].Clear();
                OnDataChanged?.Invoke();
            }
        }
    }
}
