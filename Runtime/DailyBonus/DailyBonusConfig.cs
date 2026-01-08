using System.Collections.Generic;
using UnityEngine;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// Configuration for daily bonus feature.
    /// </summary>
    [CreateAssetMenu(fileName = "DailyBonusConfig", menuName = "Spyke/Features/DailyBonusConfig")]
    public class DailyBonusConfig : ScriptableObject
    {
        [Header("Timing")]
        [SerializeField] private int _resetHourUtc = 0;
        [SerializeField] private int _gracePeriodHours = 24;
        [SerializeField] private int _streakBreakHours = 48;

        [Header("Rewards")]
        [SerializeField] private List<DailyBonusReward> _rewards = new();
        [SerializeField] private int _cycleLength = 7;

        [Header("Multipliers")]
        [SerializeField] private float _streakMultiplier = 1.1f;
        [SerializeField] private int _maxMultiplierStreak = 7;

        /// <summary>
        /// Hour of day (UTC) when the bonus resets.
        /// </summary>
        public int ResetHourUtc => _resetHourUtc;

        /// <summary>
        /// Hours after reset before streak breaks.
        /// </summary>
        public int GracePeriodHours => _gracePeriodHours;

        /// <summary>
        /// Hours after which streak resets to 0.
        /// </summary>
        public int StreakBreakHours => _streakBreakHours;

        /// <summary>
        /// List of rewards for each day.
        /// </summary>
        public IReadOnlyList<DailyBonusReward> Rewards => _rewards;

        /// <summary>
        /// Number of days before cycle repeats.
        /// </summary>
        public int CycleLength => _cycleLength;

        /// <summary>
        /// Multiplier applied per streak day.
        /// </summary>
        public float StreakMultiplier => _streakMultiplier;

        /// <summary>
        /// Maximum streak for multiplier calculation.
        /// </summary>
        public int MaxMultiplierStreak => _maxMultiplierStreak;

        /// <summary>
        /// Gets the reward for a specific day in the cycle.
        /// </summary>
        public DailyBonusReward GetRewardForDay(int day)
        {
            if (_rewards == null || _rewards.Count == 0) return null;

            var index = (day - 1) % _cycleLength;
            return index < _rewards.Count ? _rewards[index] : _rewards[_rewards.Count - 1];
        }

        /// <summary>
        /// Calculates the multiplier for a given streak.
        /// </summary>
        public float GetMultiplier(int streak)
        {
            var effectiveStreak = Mathf.Min(streak, _maxMultiplierStreak);
            return 1f + (_streakMultiplier - 1f) * effectiveStreak;
        }
    }
}
