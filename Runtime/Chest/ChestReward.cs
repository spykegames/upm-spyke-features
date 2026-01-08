using System;
using UnityEngine;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// Model representing a reward from a chest.
    /// </summary>
    [Serializable]
    public class ChestReward
    {
        [SerializeField] private string _rewardId;
        [SerializeField] private RewardType _type;
        [SerializeField] private int _amount;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _displayName;

        /// <summary>
        /// Unique identifier for the reward.
        /// </summary>
        public string RewardId => _rewardId;

        /// <summary>
        /// Type of reward.
        /// </summary>
        public RewardType Type => _type;

        /// <summary>
        /// Amount of the reward.
        /// </summary>
        public int Amount => _amount;

        /// <summary>
        /// Icon sprite for display.
        /// </summary>
        public Sprite Icon => _icon;

        /// <summary>
        /// Display name for the reward.
        /// </summary>
        public string DisplayName => _displayName;

        public ChestReward() { }

        public ChestReward(string rewardId, RewardType type, int amount, Sprite icon = null, string displayName = null)
        {
            _rewardId = rewardId;
            _type = type;
            _amount = amount;
            _icon = icon;
            _displayName = displayName ?? rewardId;
        }
    }

    /// <summary>
    /// Types of rewards that can be in a chest.
    /// </summary>
    public enum RewardType
    {
        Currency,
        Item,
        Booster,
        Lives,
        Experience,
        Custom
    }
}
