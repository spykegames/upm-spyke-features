using System;
using System.Collections.Generic;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// State of the tutorial system.
    /// </summary>
    public enum TutorialState
    {
        Idle,
        Running,
        Paused,
        Completed,
        Cancelled
    }

    /// <summary>
    /// Model holding tutorial state and completion data.
    /// </summary>
    public class TutorialModel
    {
        private TutorialState _state = TutorialState.Idle;
        private TutorialSequence _currentSequence;
        private int _currentStepIndex = -1;
        private readonly HashSet<string> _completedSequences = new();
        private readonly HashSet<string> _completedSteps = new();

        /// <summary>
        /// Current state of the tutorial system.
        /// </summary>
        public TutorialState State
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
        /// Currently running sequence.
        /// </summary>
        public TutorialSequence CurrentSequence => _currentSequence;

        /// <summary>
        /// Current step index within the sequence.
        /// </summary>
        public int CurrentStepIndex => _currentStepIndex;

        /// <summary>
        /// Current step being executed.
        /// </summary>
        public TutorialStep CurrentStep => _currentSequence?.GetStep(_currentStepIndex);

        /// <summary>
        /// Whether a tutorial is currently running.
        /// </summary>
        public bool IsRunning => _state == TutorialState.Running;

        /// <summary>
        /// Whether the tutorial is paused.
        /// </summary>
        public bool IsPaused => _state == TutorialState.Paused;

        /// <summary>
        /// Progress through current sequence (0-1).
        /// </summary>
        public float Progress
        {
            get
            {
                if (_currentSequence == null || _currentSequence.StepCount == 0)
                    return 0f;
                return (float)(_currentStepIndex + 1) / _currentSequence.StepCount;
            }
        }

        /// <summary>
        /// Fired when state changes.
        /// </summary>
        public event Action<TutorialState> OnStateChanged;

        /// <summary>
        /// Fired when step changes.
        /// </summary>
        public event Action<int, TutorialStep> OnStepChanged;

        /// <summary>
        /// Fired when a sequence is completed.
        /// </summary>
        public event Action<string> OnSequenceCompleted;

        /// <summary>
        /// Starts a new sequence.
        /// </summary>
        public void StartSequence(TutorialSequence sequence)
        {
            _currentSequence = sequence;
            _currentStepIndex = -1;
            State = TutorialState.Running;
        }

        /// <summary>
        /// Moves to the next step.
        /// </summary>
        public bool NextStep()
        {
            if (_currentSequence == null) return false;

            _currentStepIndex++;

            if (_currentStepIndex >= _currentSequence.StepCount)
            {
                CompleteCurrentSequence();
                return false;
            }

            OnStepChanged?.Invoke(_currentStepIndex, CurrentStep);
            return true;
        }

        /// <summary>
        /// Marks the current step as completed.
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (CurrentStep != null)
            {
                _completedSteps.Add(CurrentStep.Id);
            }
        }

        /// <summary>
        /// Marks the current sequence as completed.
        /// </summary>
        public void CompleteCurrentSequence()
        {
            if (_currentSequence != null)
            {
                _completedSequences.Add(_currentSequence.Id);
                OnSequenceCompleted?.Invoke(_currentSequence.Id);
            }

            _currentSequence = null;
            _currentStepIndex = -1;
            State = TutorialState.Completed;
        }

        /// <summary>
        /// Cancels the current tutorial.
        /// </summary>
        public void Cancel()
        {
            _currentSequence = null;
            _currentStepIndex = -1;
            State = TutorialState.Cancelled;
        }

        /// <summary>
        /// Pauses the tutorial.
        /// </summary>
        public void Pause()
        {
            if (IsRunning)
            {
                State = TutorialState.Paused;
            }
        }

        /// <summary>
        /// Resumes a paused tutorial.
        /// </summary>
        public void Resume()
        {
            if (IsPaused)
            {
                State = TutorialState.Running;
            }
        }

        /// <summary>
        /// Checks if a sequence has been completed.
        /// </summary>
        public bool IsSequenceCompleted(string sequenceId)
        {
            return _completedSequences.Contains(sequenceId);
        }

        /// <summary>
        /// Checks if a step has been completed.
        /// </summary>
        public bool IsStepCompleted(string stepId)
        {
            return _completedSteps.Contains(stepId);
        }

        /// <summary>
        /// Marks a sequence as completed (for persistence).
        /// </summary>
        public void MarkSequenceCompleted(string sequenceId)
        {
            _completedSequences.Add(sequenceId);
        }

        /// <summary>
        /// Gets all completed sequence IDs.
        /// </summary>
        public IReadOnlyCollection<string> GetCompletedSequences()
        {
            return _completedSequences;
        }

        /// <summary>
        /// Loads completion data.
        /// </summary>
        public void LoadCompletionData(IEnumerable<string> completedSequences)
        {
            _completedSequences.Clear();
            if (completedSequences != null)
            {
                foreach (var id in completedSequences)
                {
                    _completedSequences.Add(id);
                }
            }
        }
    }
}
