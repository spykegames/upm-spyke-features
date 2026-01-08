using System;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// View interface for daily bonus UI.
    /// Implement this for your specific UI implementation.
    /// </summary>
    public interface IDailyBonusView
    {
        /// <summary>
        /// Shows the daily bonus popup.
        /// </summary>
        void Show(DailyBonusReward currentReward, int currentDay, int cycleLength);

        /// <summary>
        /// Hides the daily bonus popup.
        /// </summary>
        void Hide();

        /// <summary>
        /// Plays the claim animation.
        /// </summary>
        void PlayClaimAnimation(DailyBonusReward reward);

        /// <summary>
        /// Updates the countdown timer display.
        /// </summary>
        void UpdateTimer(TimeSpan remaining);

        /// <summary>
        /// Updates the streak display.
        /// </summary>
        void UpdateStreak(int streak);

        /// <summary>
        /// Shows or hides the available indicator.
        /// </summary>
        void ShowAvailableIndicator(bool show);
    }
}
