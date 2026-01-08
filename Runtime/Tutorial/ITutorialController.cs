using System;
using Cysharp.Threading.Tasks;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// Controller interface for tutorial operations.
    /// </summary>
    public interface ITutorialController
    {
        /// <summary>
        /// Starts a tutorial sequence.
        /// </summary>
        UniTask StartTutorialAsync(TutorialSequence sequence, int startStep = 0);

        /// <summary>
        /// Starts a tutorial by ID.
        /// </summary>
        UniTask StartTutorialAsync(string sequenceId, int startStep = 0);

        /// <summary>
        /// Skips the current step.
        /// </summary>
        void SkipCurrentStep();

        /// <summary>
        /// Skips the entire tutorial.
        /// </summary>
        void SkipAll();

        /// <summary>
        /// Pauses the tutorial.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes a paused tutorial.
        /// </summary>
        void Resume();

        /// <summary>
        /// Cancels the current tutorial.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Registers a tutorial sequence.
        /// </summary>
        void RegisterSequence(TutorialSequence sequence);

        /// <summary>
        /// Checks if a sequence has been completed.
        /// </summary>
        bool IsSequenceCompleted(string sequenceId);

        /// <summary>
        /// Whether a tutorial is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Current step index.
        /// </summary>
        int CurrentStepIndex { get; }

        /// <summary>
        /// Current sequence ID.
        /// </summary>
        string CurrentSequenceId { get; }

        /// <summary>
        /// Progress through current tutorial (0-1).
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Fired when a tutorial starts.
        /// </summary>
        event Action<TutorialSequence> OnTutorialStarted;

        /// <summary>
        /// Fired when a tutorial completes.
        /// </summary>
        event Action<string> OnTutorialCompleted;

        /// <summary>
        /// Fired when a tutorial is cancelled.
        /// </summary>
        event Action<string> OnTutorialCancelled;

        /// <summary>
        /// Fired when a step changes.
        /// </summary>
        event Action<int, TutorialStep> OnStepChanged;
    }
}
