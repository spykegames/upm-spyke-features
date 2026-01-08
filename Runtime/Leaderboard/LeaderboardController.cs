using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Controller implementation for leaderboard operations.
    /// </summary>
    public class LeaderboardController : ILeaderboardController, IInitializable, IDisposable
    {
        [Inject] private readonly LeaderboardModel _model;
        [Inject(Optional = true)] private readonly ILeaderboardView _view;
        [Inject(Optional = true)] private readonly ILeaderboardNetworkService _network;

        private LeaderboardTab _currentTab = LeaderboardTab.Players;
        private LeaderboardScope _currentScope = LeaderboardScope.Global;
        private bool _isFetching;

        public LeaderboardTab CurrentTab => _currentTab;
        public LeaderboardScope CurrentScope => _currentScope;
        public bool IsFetching => _isFetching;

        public event Action OnLeaderboardUpdated;
        public event Action<LeaderboardTab> OnTabChanged;
        public event Action<LeaderboardScope> OnScopeChanged;
        public event Action<LeaderboardEntry> OnEntryClicked;

        public void Initialize()
        {
            _model.OnDataChanged += HandleDataChanged;
        }

        public async UniTask<bool> FetchLeaderboardAsync(LeaderboardTab tab, LeaderboardScope scope)
        {
            if (_isFetching)
            {
                Debug.LogWarning("[LeaderboardController] Already fetching leaderboard.");
                return false;
            }

            if (_network == null)
            {
                Debug.LogWarning("[LeaderboardController] No network service configured.");
                return false;
            }

            _isFetching = true;
            _view?.ShowLoading(true);

            try
            {
                var entries = await _network.FetchLeaderboardAsync(tab, scope);
                _model.SetEntries(tab, scope, entries);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LeaderboardController] Failed to fetch leaderboard: {ex.Message}");
                return false;
            }
            finally
            {
                _isFetching = false;
                _view?.ShowLoading(false);
            }
        }

        public async UniTask<bool> FetchAllLeaderboardsAsync()
        {
            var success = true;

            // Fetch current tab/scope first for faster display
            success &= await FetchLeaderboardAsync(_currentTab, _currentScope);

            // Fetch other combinations in background
            foreach (LeaderboardTab tab in Enum.GetValues(typeof(LeaderboardTab)))
            {
                foreach (LeaderboardScope scope in Enum.GetValues(typeof(LeaderboardScope)))
                {
                    if (tab == _currentTab && scope == _currentScope) continue;

                    var result = await FetchLeaderboardAsync(tab, scope);
                    success &= result;
                }
            }

            return success;
        }

        public IReadOnlyList<LeaderboardEntry> GetEntries(LeaderboardTab tab, LeaderboardScope scope)
        {
            return _model.GetEntries(tab, scope);
        }

        public LeaderboardEntry GetCurrentUserEntry()
        {
            return _model.CurrentUserEntry;
        }

        public int GetCurrentUserRank(LeaderboardTab tab, LeaderboardScope scope)
        {
            return _model.GetCurrentUserRank(tab, scope);
        }

        public void UpdateLocalScore(long score, long subScore = 0)
        {
            _model.UpdateCurrentUserScore(score, subScore);
        }

        public void SwitchTab(LeaderboardTab tab)
        {
            if (_currentTab == tab) return;

            _currentTab = tab;
            OnTabChanged?.Invoke(tab);
            RefreshView();
        }

        public void SwitchScope(LeaderboardScope scope)
        {
            if (_currentScope == scope) return;

            _currentScope = scope;
            OnScopeChanged?.Invoke(scope);
            RefreshView();
        }

        /// <summary>
        /// Called when a leaderboard entry is clicked.
        /// </summary>
        public void HandleEntryClicked(LeaderboardEntry entry)
        {
            OnEntryClicked?.Invoke(entry);
        }

        private void HandleDataChanged()
        {
            RefreshView();
            OnLeaderboardUpdated?.Invoke();
        }

        private void RefreshView()
        {
            var entries = _model.GetEntries(_currentTab, _currentScope);
            _view?.RefreshEntries(entries);
            _view?.HighlightCurrentUser(_model.CurrentUserEntry);
        }

        public void Dispose()
        {
            _model.OnDataChanged -= HandleDataChanged;
        }
    }

    /// <summary>
    /// Network service interface for leaderboard operations.
    /// Games implement this to connect to their backend.
    /// </summary>
    public interface ILeaderboardNetworkService
    {
        UniTask<IReadOnlyList<LeaderboardEntry>> FetchLeaderboardAsync(LeaderboardTab tab, LeaderboardScope scope);
    }
}
