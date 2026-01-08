using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Base implementation of ILeaderboardView.
    /// Extend this for custom implementations.
    /// </summary>
    public class LeaderboardView : MonoBehaviour, ILeaderboardView
    {
        [Header("Container")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _entryContainer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _entryPrefab;

        [Header("Loading")]
        [SerializeField] private GameObject _loadingIndicator;

        [Header("Empty State")]
        [SerializeField] private GameObject _emptyStateObject;

        [Header("Tabs")]
        [SerializeField] private Button _playersTabButton;
        [SerializeField] private Button _teamsTabButton;
        [SerializeField] private Button _friendsTabButton;

        [Header("Scope")]
        [SerializeField] private Button _globalScopeButton;
        [SerializeField] private Button _localScopeButton;

        [Header("Current User")]
        [SerializeField] private GameObject _currentUserPanel;
        [SerializeField] private TextMeshProUGUI _currentUserRankText;
        [SerializeField] private TextMeshProUGUI _currentUserScoreText;

        [Header("Animation")]
        [SerializeField] private float _showDuration = 0.3f;
        [SerializeField] private float _entryAnimDelay = 0.02f;

        private readonly List<GameObject> _spawnedEntries = new();
        private LeaderboardEntry _currentUserEntry;

        public void RefreshEntries(IReadOnlyList<LeaderboardEntry> entries)
        {
            ClearEntries();

            if (entries == null || entries.Count == 0)
            {
                ShowEmptyState(true);
                return;
            }

            ShowEmptyState(false);

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var entryObj = SpawnEntry(entry);

                // Animate in with delay
                var rectTransform = entryObj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    var startY = rectTransform.anchoredPosition.y - 50f;
                    var endY = rectTransform.anchoredPosition.y;
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startY);

                    var canvasGroup = entryObj.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 0f;
                        Tween.Alpha(canvasGroup, 1f, 0.2f, startDelay: i * _entryAnimDelay);
                    }

                    Tween.UIAnchoredPositionY(rectTransform, endY, 0.2f, Ease.OutQuad, startDelay: i * _entryAnimDelay);
                }
            }
        }

        public void HighlightCurrentUser(LeaderboardEntry entry)
        {
            _currentUserEntry = entry;

            if (_currentUserPanel != null)
            {
                _currentUserPanel.SetActive(entry != null);
            }

            if (entry != null)
            {
                if (_currentUserRankText != null)
                {
                    _currentUserRankText.text = $"#{entry.Rank}";
                }

                if (_currentUserScoreText != null)
                {
                    _currentUserScoreText.text = entry.Score.ToString("N0");
                }
            }
        }

        public void ShowLoading(bool show)
        {
            if (_loadingIndicator != null)
            {
                _loadingIndicator.SetActive(show);
            }
        }

        public void ScrollToRank(int rank)
        {
            if (_scrollRect == null || _spawnedEntries.Count == 0) return;

            var index = rank - 1;
            if (index < 0 || index >= _spawnedEntries.Count) return;

            var normalizedPosition = 1f - (float)index / (_spawnedEntries.Count - 1);
            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }

        public void ScrollToCurrentUser()
        {
            if (_currentUserEntry != null)
            {
                ScrollToRank(_currentUserEntry.Rank);
            }
        }

        public void UpdateTabUI(LeaderboardTab tab)
        {
            SetButtonSelected(_playersTabButton, tab == LeaderboardTab.Players);
            SetButtonSelected(_teamsTabButton, tab == LeaderboardTab.Teams);
            SetButtonSelected(_friendsTabButton, tab == LeaderboardTab.Friends);
        }

        public void UpdateScopeUI(LeaderboardScope scope)
        {
            SetButtonSelected(_globalScopeButton, scope == LeaderboardScope.Global);
            SetButtonSelected(_localScopeButton, scope == LeaderboardScope.Local);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                Tween.Alpha(_canvasGroup, 1f, _showDuration, Ease.OutQuad);
            }
        }

        public void Hide()
        {
            if (_canvasGroup != null)
            {
                Tween.Alpha(_canvasGroup, 0f, _showDuration, Ease.InQuad)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual GameObject SpawnEntry(LeaderboardEntry entry)
        {
            if (_entryContainer == null || _entryPrefab == null) return null;

            var entryObj = Instantiate(_entryPrefab, _entryContainer);
            _spawnedEntries.Add(entryObj);

            var entryView = entryObj.GetComponent<LeaderboardEntryView>();
            if (entryView != null)
            {
                entryView.Setup(entry);
            }

            return entryObj;
        }

        protected void ClearEntries()
        {
            foreach (var entry in _spawnedEntries)
            {
                if (entry != null)
                {
                    Destroy(entry);
                }
            }
            _spawnedEntries.Clear();
        }

        protected void ShowEmptyState(bool show)
        {
            if (_emptyStateObject != null)
            {
                _emptyStateObject.SetActive(show);
            }

            if (_entryContainer != null)
            {
                _entryContainer.gameObject.SetActive(!show);
            }
        }

        private void SetButtonSelected(Button button, bool selected)
        {
            if (button == null) return;

            // Override in subclass for custom selection visuals
            var colors = button.colors;
            colors.normalColor = selected ? colors.selectedColor : colors.normalColor;
            button.colors = colors;
        }

        protected virtual void OnDestroy()
        {
            ClearEntries();
        }
    }

    /// <summary>
    /// Individual leaderboard entry view component.
    /// Attach to leaderboard entry prefab.
    /// </summary>
    public class LeaderboardEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _subScoreText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private GameObject _currentUserHighlight;
        [SerializeField] private Button _button;

        private LeaderboardEntry _entry;

        public LeaderboardEntry Entry => _entry;

        public void Setup(LeaderboardEntry entry)
        {
            _entry = entry;

            if (_rankText != null)
            {
                _rankText.text = $"#{entry.Rank}";
            }

            if (_nameText != null)
            {
                _nameText.text = entry.Name ?? "";
            }

            if (_scoreText != null)
            {
                _scoreText.text = entry.Score.ToString("N0");
            }

            if (_subScoreText != null && entry.SubScore > 0)
            {
                _subScoreText.text = entry.SubScore.ToString();
            }

            if (_currentUserHighlight != null)
            {
                _currentUserHighlight.SetActive(entry.IsCurrentUser);
            }

            // Avatar loading would be handled by game-specific code
        }

        public void OnClick()
        {
            // Button click handled via controller events
        }
    }
}
