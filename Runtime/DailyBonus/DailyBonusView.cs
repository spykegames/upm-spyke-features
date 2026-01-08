using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// Base implementation of IDailyBonusView.
    /// Extend this for custom implementations.
    /// </summary>
    public class DailyBonusView : MonoBehaviour, IDailyBonusView
    {
        [Header("Container")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _daysContainer;
        [SerializeField] private GameObject _dayItemPrefab;

        [Header("Current Reward")]
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;
        [SerializeField] private TextMeshProUGUI _rewardTypeText;

        [Header("Timer")]
        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private TextMeshProUGUI _timerText;

        [Header("Streak")]
        [SerializeField] private TextMeshProUGUI _streakText;
        [SerializeField] private GameObject _streakBonusIndicator;

        [Header("Buttons")]
        [SerializeField] private Button _claimButton;
        [SerializeField] private Button _closeButton;

        [Header("Indicator")]
        [SerializeField] private GameObject _availableIndicator;

        [Header("Animation")]
        [SerializeField] private float _showDuration = 0.3f;
        [SerializeField] private Transform _rewardTransform;

        private readonly List<GameObject> _spawnedDays = new();

        protected virtual void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(Hide);
            }
        }

        public void Show(DailyBonusReward currentReward, int currentDay, int cycleLength)
        {
            gameObject.SetActive(true);

            // Show current reward
            if (currentReward != null)
            {
                if (_rewardIcon != null && currentReward.Icon != null)
                {
                    _rewardIcon.sprite = currentReward.Icon;
                }

                if (_rewardAmountText != null)
                {
                    _rewardAmountText.text = currentReward.Amount.ToString("N0");
                }

                if (_rewardTypeText != null)
                {
                    _rewardTypeText.text = currentReward.RewardType;
                }
            }

            // Show day progress
            RefreshDays(currentDay, cycleLength);

            // Hide timer when showing claim popup
            if (_timerPanel != null)
            {
                _timerPanel.SetActive(false);
            }

            // Animate in
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                Tween.Alpha(_canvasGroup, 1f, _showDuration, Ease.OutQuad);
            }

            if (_rewardTransform != null)
            {
                _rewardTransform.localScale = Vector3.zero;
                Tween.Scale(_rewardTransform, Vector3.one, _showDuration, Ease.OutBack);
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

        public void PlayClaimAnimation(DailyBonusReward reward)
        {
            if (_rewardTransform != null)
            {
                Sequence.Create()
                    .Chain(Tween.Scale(_rewardTransform, 1.3f, 0.2f, Ease.OutBack))
                    .Chain(Tween.Scale(_rewardTransform, 0f, 0.3f, Ease.InBack))
                    .OnComplete(Hide);
            }
            else
            {
                Hide();
            }
        }

        public void UpdateTimer(TimeSpan remaining)
        {
            if (_timerPanel != null)
            {
                _timerPanel.SetActive(true);
            }

            if (_timerText != null)
            {
                if (remaining.TotalHours >= 1)
                {
                    _timerText.text = $"{(int)remaining.TotalHours:00}:{remaining.Minutes:00}:{remaining.Seconds:00}";
                }
                else
                {
                    _timerText.text = $"{remaining.Minutes:00}:{remaining.Seconds:00}";
                }
            }
        }

        public void UpdateStreak(int streak)
        {
            if (_streakText != null)
            {
                _streakText.text = streak.ToString();
            }

            if (_streakBonusIndicator != null)
            {
                _streakBonusIndicator.SetActive(streak > 0);
            }
        }

        public void ShowAvailableIndicator(bool show)
        {
            if (_availableIndicator != null)
            {
                _availableIndicator.SetActive(show);
            }
        }

        private void RefreshDays(int currentDay, int cycleLength)
        {
            ClearDays();

            if (_daysContainer == null || _dayItemPrefab == null) return;

            for (var i = 1; i <= cycleLength; i++)
            {
                var dayObj = Instantiate(_dayItemPrefab, _daysContainer);
                _spawnedDays.Add(dayObj);

                var dayView = dayObj.GetComponent<DailyBonusDayView>();
                if (dayView != null)
                {
                    var state = i < currentDay ? DailyBonusDayState.Claimed
                              : i == currentDay ? DailyBonusDayState.Current
                              : DailyBonusDayState.Locked;

                    dayView.Setup(i, state);
                }
            }
        }

        private void ClearDays()
        {
            foreach (var day in _spawnedDays)
            {
                if (day != null)
                {
                    Destroy(day);
                }
            }
            _spawnedDays.Clear();
        }

        protected virtual void OnDestroy()
        {
            ClearDays();
        }
    }

    /// <summary>
    /// State of a day in the bonus calendar.
    /// </summary>
    public enum DailyBonusDayState
    {
        Locked,
        Current,
        Claimed
    }

    /// <summary>
    /// Individual day view component.
    /// Attach to day item prefab.
    /// </summary>
    public class DailyBonusDayView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _dayText;
        [SerializeField] private GameObject _lockedOverlay;
        [SerializeField] private GameObject _currentHighlight;
        [SerializeField] private GameObject _claimedCheck;
        [SerializeField] private Image _rewardIcon;

        public void Setup(int day, DailyBonusDayState state)
        {
            if (_dayText != null)
            {
                _dayText.text = $"Day {day}";
            }

            if (_lockedOverlay != null)
            {
                _lockedOverlay.SetActive(state == DailyBonusDayState.Locked);
            }

            if (_currentHighlight != null)
            {
                _currentHighlight.SetActive(state == DailyBonusDayState.Current);
            }

            if (_claimedCheck != null)
            {
                _claimedCheck.SetActive(state == DailyBonusDayState.Claimed);
            }
        }

        public void SetRewardIcon(Sprite icon)
        {
            if (_rewardIcon != null && icon != null)
            {
                _rewardIcon.sprite = icon;
            }
        }
    }
}
