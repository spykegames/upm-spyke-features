using System;
using Cysharp.Threading.Tasks;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// Controller interface for daily bonus operations.
    /// </summary>
    public interface IDailyBonusController
    {
        /// <summary>
        /// Checks if bonus is available and shows popup if so.
        /// </summary>
        UniTask CheckAndShowBonusAsync();

        /// <summary>
        /// Claims the current day's bonus.
        /// </summary>
        UniTask<DailyBonusReward> ClaimBonusAsync();

        /// <summary>
        /// Gets the reward for the current day.
        /// </summary>
        DailyBonusReward GetCurrentDayReward();

        /// <summary>
        /// Gets the reward for a specific day.
        /// </summary>
        DailyBonusReward GetRewardForDay(int day);

        /// <summary>
        /// Current streak.
        /// </summary>
        int CurrentStreak { get; }

        /// <summary>
        /// Current day in the cycle (1-based).
        /// </summary>
        int CurrentDay { get; }

        /// <summary>
        /// Whether bonus is currently available.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Time remaining until next bonus (TimeSpan).
        /// </summary>
        TimeSpan TimeRemaining { get; }

        /// <summary>
        /// Fired when bonus becomes available.
        /// </summary>
        event Action OnBonusAvailable;

        /// <summary>
        /// Fired when bonus is claimed.
        /// </summary>
        event Action<DailyBonusReward> OnBonusClaimed;

        /// <summary>
        /// Fired when streak changes.
        /// </summary>
        event Action<int> OnStreakChanged;
    }
}
