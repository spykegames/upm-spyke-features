using System;

namespace Spyke.Features.DailyBonus
{
    /// <summary>
    /// State of daily bonus progression.
    /// </summary>
    public enum DailyBonusState
    {
        /// <summary>
        /// Bonus is available to claim.
        /// </summary>
        Available,

        /// <summary>
        /// Bonus was claimed, waiting for next day.
        /// </summary>
        Claimed,

        /// <summary>
        /// Streak was reset due to missed day.
        /// </summary>
        StreakReset
    }

    /// <summary>
    /// Model holding daily bonus state.
    /// </summary>
    public class DailyBonusModel
    {
        private int _currentStreak;
        private int _maxStreak;
        private long _lastClaimTimestamp;
        private long _nextAvailableTimestamp;
        private DailyBonusState _state;

        /// <summary>
        /// Current consecutive day streak.
        /// </summary>
        public int CurrentStreak
        {
            get => _currentStreak;
            set
            {
                if (_currentStreak != value)
                {
                    _currentStreak = value;
                    OnStreakChanged?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// Maximum streak achieved.
        /// </summary>
        public int MaxStreak
        {
            get => _maxStreak;
            set => _maxStreak = value;
        }

        /// <summary>
        /// Timestamp of last claim (Unix milliseconds).
        /// </summary>
        public long LastClaimTimestamp
        {
            get => _lastClaimTimestamp;
            set => _lastClaimTimestamp = value;
        }

        /// <summary>
        /// Timestamp when next bonus becomes available (Unix milliseconds).
        /// </summary>
        public long NextAvailableTimestamp
        {
            get => _nextAvailableTimestamp;
            set => _nextAvailableTimestamp = value;
        }

        /// <summary>
        /// Current state of daily bonus.
        /// </summary>
        public DailyBonusState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// Whether bonus is currently available to claim.
        /// </summary>
        public bool IsAvailable => State == DailyBonusState.Available;

        /// <summary>
        /// Time remaining until next bonus (in milliseconds).
        /// </summary>
        public long TimeRemaining
        {
            get
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                return Math.Max(0, _nextAvailableTimestamp - now);
            }
        }

        /// <summary>
        /// Fired when streak changes.
        /// </summary>
        public event Action<int> OnStreakChanged;

        /// <summary>
        /// Fired when state changes.
        /// </summary>
        public event Action<DailyBonusState> OnStateChanged;

        /// <summary>
        /// Resets the streak to 0.
        /// </summary>
        public void ResetStreak()
        {
            CurrentStreak = 0;
            State = DailyBonusState.StreakReset;
        }

        /// <summary>
        /// Increments the streak by 1.
        /// </summary>
        public void IncrementStreak()
        {
            CurrentStreak++;
            if (CurrentStreak > MaxStreak)
            {
                MaxStreak = CurrentStreak;
            }
        }

        /// <summary>
        /// Records a claim at the specified timestamp.
        /// </summary>
        public void RecordClaim(long timestamp, long nextAvailable)
        {
            LastClaimTimestamp = timestamp;
            NextAvailableTimestamp = nextAvailable;
            State = DailyBonusState.Claimed;
            IncrementStreak();
        }
    }
}
