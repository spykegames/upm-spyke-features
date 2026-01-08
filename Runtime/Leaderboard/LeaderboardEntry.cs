using System;
using UnityEngine;

namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Represents a single entry in the leaderboard.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private LeaderboardEntryType _type;
        [SerializeField] private int _rank;
        [SerializeField] private long _score;
        [SerializeField] private long _subScore;
        [SerializeField] private string _name;
        [SerializeField] private int _avatarId;
        [SerializeField] private string _avatarUrl;
        [SerializeField] private int _frameId;
        [SerializeField] private bool _isCurrentUser;

        /// <summary>
        /// Unique identifier (user ID or team ID).
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Type of entry (User or Team).
        /// </summary>
        public LeaderboardEntryType Type => _type;

        /// <summary>
        /// Rank position (1-based).
        /// </summary>
        public int Rank
        {
            get => _rank;
            set => _rank = value;
        }

        /// <summary>
        /// Primary score value.
        /// </summary>
        public long Score
        {
            get => _score;
            set => _score = value;
        }

        /// <summary>
        /// Secondary score (rounds, sub-score, etc.).
        /// </summary>
        public long SubScore
        {
            get => _subScore;
            set => _subScore = value;
        }

        /// <summary>
        /// Display name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Avatar ID for local avatars.
        /// </summary>
        public int AvatarId
        {
            get => _avatarId;
            set => _avatarId = value;
        }

        /// <summary>
        /// URL for remote avatar (e.g., Facebook).
        /// </summary>
        public string AvatarUrl
        {
            get => _avatarUrl;
            set => _avatarUrl = value;
        }

        /// <summary>
        /// Frame ID for avatar decoration.
        /// </summary>
        public int FrameId
        {
            get => _frameId;
            set => _frameId = value;
        }

        /// <summary>
        /// Whether this entry represents the current user.
        /// </summary>
        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set => _isCurrentUser = value;
        }

        /// <summary>
        /// Whether this entry has valid data.
        /// </summary>
        public bool IsEmpty =>
            string.IsNullOrEmpty(_id) &&
            _rank == 0 &&
            _score == 0 &&
            string.IsNullOrEmpty(_name);

        public LeaderboardEntry(
            string id,
            LeaderboardEntryType type,
            int rank,
            long score,
            string name,
            long subScore = 0,
            int avatarId = 0,
            string avatarUrl = null,
            int frameId = 0,
            bool isCurrentUser = false)
        {
            _id = id;
            _type = type;
            _rank = rank;
            _score = score;
            _name = name;
            _subScore = subScore;
            _avatarId = avatarId;
            _avatarUrl = avatarUrl;
            _frameId = frameId;
            _isCurrentUser = isCurrentUser;
        }

        public override string ToString()
        {
            return $"#{Rank} {Name}: {Score}";
        }
    }
}
