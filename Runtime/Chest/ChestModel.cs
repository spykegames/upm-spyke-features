using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// Model representing a chest with its contents.
    /// </summary>
    [Serializable]
    public class ChestModel
    {
        [SerializeField] private string _chestId;
        [SerializeField] private ChestType _type;
        [SerializeField] private ChestState _state;
        [SerializeField] private List<ChestReward> _rewards = new();
        [SerializeField] private DateTime _unlockTime;

        /// <summary>
        /// Unique identifier for this chest.
        /// </summary>
        public string ChestId
        {
            get => _chestId;
            set => _chestId = value;
        }

        /// <summary>
        /// Type of chest (determines appearance and rewards).
        /// </summary>
        public ChestType Type
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// Current state of the chest.
        /// </summary>
        public ChestState State
        {
            get => _state;
            set => _state = value;
        }

        /// <summary>
        /// Rewards contained in this chest.
        /// </summary>
        public List<ChestReward> Rewards => _rewards;

        /// <summary>
        /// Time when the chest can be opened (for timed chests).
        /// </summary>
        public DateTime UnlockTime
        {
            get => _unlockTime;
            set => _unlockTime = value;
        }

        /// <summary>
        /// Whether the chest is ready to open.
        /// </summary>
        public bool CanOpen => State == ChestState.Ready ||
                              (State == ChestState.Locked && DateTime.UtcNow >= UnlockTime);

        /// <summary>
        /// Time remaining until the chest can be opened.
        /// </summary>
        public TimeSpan TimeRemaining => UnlockTime > DateTime.UtcNow
            ? UnlockTime - DateTime.UtcNow
            : TimeSpan.Zero;

        public ChestModel() { }

        public ChestModel(string chestId, ChestType type, List<ChestReward> rewards)
        {
            _chestId = chestId;
            _type = type;
            _state = ChestState.Ready;
            _rewards = rewards ?? new List<ChestReward>();
        }
    }

    /// <summary>
    /// Types of chests with different rarities.
    /// </summary>
    public enum ChestType
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Daily,
        Special
    }

    /// <summary>
    /// States a chest can be in.
    /// </summary>
    public enum ChestState
    {
        Locked,
        Ready,
        Opening,
        Opened
    }
}
