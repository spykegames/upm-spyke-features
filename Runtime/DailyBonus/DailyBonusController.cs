using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// Controller implementation for daily bonus operations.
    /// </summary>
    public class DailyBonusController : IDailyBonusController, IInitializable, IDisposable, ITickable
    {
        [Inject] private readonly DailyBonusModel _model;
        [Inject(Optional = true)] private readonly DailyBonusConfig _config;
        [Inject(Optional = true)] private readonly IDailyBonusView _view;

        private float _checkTimer;
        private const float CHECK_INTERVAL = 1f;
        private bool _wasAvailable;

        public int CurrentStreak => _model.CurrentStreak;
        public int CurrentDay => (_model.CurrentStreak % (_config?.CycleLength ?? 7)) + 1;
        public bool IsAvailable => _model.IsAvailable;
        public TimeSpan TimeRemaining => TimeSpan.FromMilliseconds(_model.TimeRemaining);

        public event Action OnBonusAvailable;
        public event Action<DailyBonusReward> OnBonusClaimed;
        public event Action<int> OnStreakChanged;

        public void Initialize()
        {
            _model.OnStreakChanged += HandleStreakChanged;
            _model.OnStateChanged += HandleStateChanged;

            // Check initial state
            CheckBonusAvailability();
        }

        public void Tick()
        {
            _checkTimer += Time.deltaTime;
            if (_checkTimer >= CHECK_INTERVAL)
            {
                _checkTimer = 0f;
                CheckBonusAvailability();
                UpdateViewTimer();
            }
        }

        public async UniTask CheckAndShowBonusAsync()
        {
            CheckBonusAvailability();

            if (_model.IsAvailable)
            {
                _view?.Show(GetCurrentDayReward(), CurrentDay, _config?.CycleLength ?? 7);
            }

            await UniTask.CompletedTask;
        }

        public async UniTask<DailyBonusReward> ClaimBonusAsync()
        {
            if (!_model.IsAvailable)
            {
                Debug.LogWarning("[DailyBonusController] Bonus not available to claim.");
                return null;
            }

            var reward = GetCurrentDayReward();
            if (reward == null)
            {
                Debug.LogWarning("[DailyBonusController] No reward configured for current day.");
                return null;
            }

            // Calculate next available time
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var nextAvailable = CalculateNextAvailableTime();

            // Record the claim
            _model.RecordClaim(now, nextAvailable);

            // Play claim animation
            _view?.PlayClaimAnimation(reward);

            // Notify listeners
            OnBonusClaimed?.Invoke(reward);

            Debug.Log($"[DailyBonusController] Claimed day {CurrentDay} reward: {reward.Amount} {reward.RewardType}");

            return reward;
        }

        public DailyBonusReward GetCurrentDayReward()
        {
            return _config?.GetRewardForDay(CurrentDay);
        }

        public DailyBonusReward GetRewardForDay(int day)
        {
            return _config?.GetRewardForDay(day);
        }

        private void CheckBonusAvailability()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Check if streak should be reset
            if (_model.LastClaimTimestamp > 0)
            {
                var hoursSinceLastClaim = (now - _model.LastClaimTimestamp) / (1000.0 * 60 * 60);
                var streakBreakHours = _config?.StreakBreakHours ?? 48;

                if (hoursSinceLastClaim > streakBreakHours)
                {
                    _model.ResetStreak();
                    Debug.Log("[DailyBonusController] Streak reset due to missed day.");
                }
            }

            // Check if bonus is available
            var isAvailable = now >= _model.NextAvailableTimestamp || _model.LastClaimTimestamp == 0;

            if (isAvailable && _model.State != DailyBonusState.Available)
            {
                _model.State = DailyBonusState.Available;

                if (!_wasAvailable)
                {
                    OnBonusAvailable?.Invoke();
                }
            }

            _wasAvailable = _model.IsAvailable;
        }

        private long CalculateNextAvailableTime()
        {
            var now = DateTimeOffset.UtcNow;
            var resetHour = _config?.ResetHourUtc ?? 0;

            // Calculate next reset time
            var nextReset = new DateTimeOffset(
                now.Year, now.Month, now.Day,
                resetHour, 0, 0, TimeSpan.Zero);

            if (nextReset <= now)
            {
                nextReset = nextReset.AddDays(1);
            }

            return nextReset.ToUnixTimeMilliseconds();
        }

        private void UpdateViewTimer()
        {
            if (_view != null && !_model.IsAvailable)
            {
                _view.UpdateTimer(TimeRemaining);
            }
        }

        private void HandleStreakChanged(int streak)
        {
            OnStreakChanged?.Invoke(streak);
            _view?.UpdateStreak(streak);
        }

        private void HandleStateChanged(DailyBonusState state)
        {
            if (state == DailyBonusState.Available)
            {
                _view?.ShowAvailableIndicator(true);
            }
            else
            {
                _view?.ShowAvailableIndicator(false);
            }
        }

        public void Dispose()
        {
            _model.OnStreakChanged -= HandleStreakChanged;
            _model.OnStateChanged -= HandleStateChanged;
        }
    }
}
