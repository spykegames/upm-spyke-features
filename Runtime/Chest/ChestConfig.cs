using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// ScriptableObject configuration for chest types.
    /// </summary>
    [CreateAssetMenu(fileName = "ChestConfig", menuName = "Spyke/Features/Chest Config")]
    public class ChestConfig : ScriptableObject
    {
        [Header("Chest Types")]
        [SerializeField] private List<ChestTypeConfig> _chestTypes = new();

        [Header("Animation Settings")]
        [SerializeField] private float _openDuration = 2f;
        [SerializeField] private float _rewardDisplayDelay = 0.5f;
        [SerializeField] private float _rewardDisplayInterval = 0.2f;

        /// <summary>
        /// Configured chest types.
        /// </summary>
        public IReadOnlyList<ChestTypeConfig> ChestTypes => _chestTypes;

        /// <summary>
        /// Duration of the open animation.
        /// </summary>
        public float OpenDuration => _openDuration;

        /// <summary>
        /// Delay before showing rewards.
        /// </summary>
        public float RewardDisplayDelay => _rewardDisplayDelay;

        /// <summary>
        /// Interval between displaying each reward.
        /// </summary>
        public float RewardDisplayInterval => _rewardDisplayInterval;

        /// <summary>
        /// Gets config for a specific chest type.
        /// </summary>
        public ChestTypeConfig GetTypeConfig(ChestType type)
        {
            foreach (var config in _chestTypes)
            {
                if (config.Type == type)
                {
                    return config;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Configuration for a specific chest type.
    /// </summary>
    [Serializable]
    public class ChestTypeConfig
    {
        [SerializeField] private ChestType _type;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _closedIcon;
        [SerializeField] private Sprite _openIcon;
        [SerializeField] private Color _glowColor = Color.white;
        [SerializeField] private GameObject _prefab;

        /// <summary>
        /// The chest type.
        /// </summary>
        public ChestType Type => _type;

        /// <summary>
        /// Display name for this chest type.
        /// </summary>
        public string DisplayName => _displayName;

        /// <summary>
        /// Icon when chest is closed.
        /// </summary>
        public Sprite ClosedIcon => _closedIcon;

        /// <summary>
        /// Icon when chest is open.
        /// </summary>
        public Sprite OpenIcon => _openIcon;

        /// <summary>
        /// Glow color for effects.
        /// </summary>
        public Color GlowColor => _glowColor;

        /// <summary>
        /// Prefab for the chest view.
        /// </summary>
        public GameObject Prefab => _prefab;
    }
}
