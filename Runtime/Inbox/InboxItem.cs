using System;
using UnityEngine;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// Represents a single inbox item/message.
    /// </summary>
    [Serializable]
    public class InboxItem
    {
        [SerializeField] private string _id;
        [SerializeField] private InboxItemCategory _category;
        [SerializeField] private InboxItemInteractionType _interactionType;
        [SerializeField] private string _senderId;
        [SerializeField] private string _senderName;
        [SerializeField] private Sprite _senderAvatar;
        [SerializeField] private string _title;
        [SerializeField] private string _message;
        [SerializeField] private int _rewardAmount;
        [SerializeField] private Sprite _rewardIcon;
        [SerializeField] private long _createdTimestamp;
        [SerializeField] private long _expirationTimestamp;
        [SerializeField] private bool _isRead;
        [SerializeField] private bool _isClaimed;

        /// <summary>
        /// Unique identifier for this inbox item.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Category of this inbox item.
        /// </summary>
        public InboxItemCategory Category => _category;

        /// <summary>
        /// How this item can be interacted with.
        /// </summary>
        public InboxItemInteractionType InteractionType => _interactionType;

        /// <summary>
        /// ID of the sender (player ID, system, etc.).
        /// </summary>
        public string SenderId => _senderId;

        /// <summary>
        /// Display name of the sender.
        /// </summary>
        public string SenderName => _senderName;

        /// <summary>
        /// Avatar image of the sender.
        /// </summary>
        public Sprite SenderAvatar => _senderAvatar;

        /// <summary>
        /// Title/subject of the item.
        /// </summary>
        public string Title => _title;

        /// <summary>
        /// Message body.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Amount of reward (if applicable).
        /// </summary>
        public int RewardAmount => _rewardAmount;

        /// <summary>
        /// Icon for the reward (if applicable).
        /// </summary>
        public Sprite RewardIcon => _rewardIcon;

        /// <summary>
        /// When this item was created (Unix timestamp).
        /// </summary>
        public long CreatedTimestamp => _createdTimestamp;

        /// <summary>
        /// When this item expires (Unix timestamp, 0 = no expiration).
        /// </summary>
        public long ExpirationTimestamp => _expirationTimestamp;

        /// <summary>
        /// Whether the item has been read.
        /// </summary>
        public bool IsRead
        {
            get => _isRead;
            set => _isRead = value;
        }

        /// <summary>
        /// Whether the item has been claimed.
        /// </summary>
        public bool IsClaimed
        {
            get => _isClaimed;
            set => _isClaimed = value;
        }

        /// <summary>
        /// Whether this item can be claimed.
        /// </summary>
        public bool CanClaim =>
            InteractionType == InboxItemInteractionType.Collectable &&
            !IsClaimed &&
            !IsExpired;

        /// <summary>
        /// Whether this item has expired.
        /// </summary>
        public bool IsExpired =>
            ExpirationTimestamp > 0 &&
            DateTimeOffset.UtcNow.ToUnixTimeSeconds() > ExpirationTimestamp;

        public InboxItem(
            string id,
            InboxItemCategory category,
            InboxItemInteractionType interactionType,
            string senderId = null,
            string senderName = null,
            Sprite senderAvatar = null,
            string title = null,
            string message = null,
            int rewardAmount = 0,
            Sprite rewardIcon = null,
            long createdTimestamp = 0,
            long expirationTimestamp = 0)
        {
            _id = id;
            _category = category;
            _interactionType = interactionType;
            _senderId = senderId;
            _senderName = senderName;
            _senderAvatar = senderAvatar;
            _title = title;
            _message = message;
            _rewardAmount = rewardAmount;
            _rewardIcon = rewardIcon;
            _createdTimestamp = createdTimestamp > 0 ? createdTimestamp : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _expirationTimestamp = expirationTimestamp;
            _isRead = false;
            _isClaimed = false;
        }

        public override string ToString()
        {
            return $"InboxItem[{Id}] Category:{Category}, CanClaim:{CanClaim}";
        }
    }
}
