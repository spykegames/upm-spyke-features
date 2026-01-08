using System;
using UnityEngine;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// Represents a single day's bonus reward.
    /// </summary>
    [Serializable]
    public class DailyBonusReward
    {
        [SerializeField] private int _day;
        [SerializeField] private string _rewardType;
        [SerializeField] private int _amount;
        [SerializeField] private Sprite _icon;
        [SerializeField] private bool _isSpecial;

        /// <summary>
        /// Day number (1-based).
        /// </summary>
        public int Day => _day;

        /// <summary>
        /// Type of reward (coins, gems, item, etc.).
        /// </summary>
        public string RewardType => _rewardType;

        /// <summary>
        /// Amount of the reward.
        /// </summary>
        public int Amount => _amount;

        /// <summary>
        /// Icon to display.
        /// </summary>
        public Sprite Icon => _icon;

        /// <summary>
        /// Whether this is a special/premium reward day.
        /// </summary>
        public bool IsSpecial => _isSpecial;

        public DailyBonusReward(int day, string rewardType, int amount, Sprite icon = null, bool isSpecial = false)
        {
            _day = day;
            _rewardType = rewardType;
            _amount = amount;
            _icon = icon;
            _isSpecial = isSpecial;
        }
    }
}
