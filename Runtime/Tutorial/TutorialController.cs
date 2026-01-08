using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// Controller implementation for tutorial operations.
    /// </summary>
    public class TutorialController : ITutorialController, IInitializable, IDisposable
    {
        [Inject] private readonly TutorialModel _model;
        [Inject(Optional = true)] private readonly ITutorialView _view;

        private readonly Dictionary<string, TutorialSequence> _registeredSequences = new();
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning => _model.IsRunning;
        public int CurrentStepIndex => _model.CurrentStepIndex;
        public string CurrentSequenceId => _model.CurrentSequence?.Id;
        public float Progress => _model.Progress;

        public event Action<TutorialSequence> OnTutorialStarted;
        public event Action<string> OnTutorialCompleted;
        public event Action<string> OnTutorialCancelled;
        public event Action<int, TutorialStep> OnStepChanged;

        public void Initialize()
        {
            _model.OnStepChanged += HandleStepChanged;
            _model.OnSequenceCompleted += HandleSequenceCompleted;
            _model.OnStateChanged += HandleStateChanged;
        }

        public async UniTask StartTutorialAsync(TutorialSequence sequence, int startStep = 0)
        {
            if (sequence == null)
            {
                Debug.LogWarning("[TutorialController] Cannot start null sequence.");
                return;
            }

            if (_model.IsRunning)
            {
                Debug.LogWarning("[TutorialController] Tutorial already running.");
                return;
            }

            if (_model.IsSequenceCompleted(sequence.Id))
            {
                Debug.Log($"[TutorialController] Sequence {sequence.Id} already completed.");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            _model.StartSequence(sequence);
            OnTutorialStarted?.Invoke(sequence);

            _view?.Show();
            _view?.SetProgress(0f);

            // Skip to start step if needed
            for (var i = 0; i < startStep; i++)
            {
                _model.NextStep();
            }

            // Execute steps
            await ExecuteStepsAsync(_cancellationTokenSource.Token);
        }

        public async UniTask StartTutorialAsync(string sequenceId, int startStep = 0)
        {
            if (_registeredSequences.TryGetValue(sequenceId, out var sequence))
            {
                await StartTutorialAsync(sequence, startStep);
            }
            else
            {
                Debug.LogWarning($"[TutorialController] Sequence not found: {sequenceId}");
            }
        }

        private async UniTask ExecuteStepsAsync(CancellationToken ct)
        {
            while (_model.NextStep())
            {
                if (ct.IsCancellationRequested) break;

                var step = _model.CurrentStep;
                if (step == null) continue;

                step.State = TutorialStepState.Active;

                try
                {
                    // Delay before
                    if (step.DelayBefore > 0)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(step.DelayBefore), cancellationToken: ct);
                    }

                    // Wait while paused
                    while (_model.IsPaused && !ct.IsCancellationRequested)
                    {
                        await UniTask.Yield(ct);
                    }

                    // Execute step
                    if (_view != null)
                    {
                        await step.ExecuteAsync(_view);
                    }

                    // Mark complete
                    _model.CompleteCurrentStep();

                    // Update progress
                    _view?.SetProgress(_model.Progress);

                    // Delay after
                    if (step.DelayAfter > 0)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(step.DelayAfter), cancellationToken: ct);
                    }

                    // Cleanup
                    step.Cleanup();
                    _view?.ClearHighlight();
                    _view?.HidePointer();
                }
                catch (OperationCanceledException)
                {
                    step.Cleanup();
                    break;
                }
            }

            // Tutorial completed
            if (!ct.IsCancellationRequested)
            {
                _model.CompleteCurrentSequence();
            }

            _view?.Hide();
        }

        public void SkipCurrentStep()
        {
            var step = _model.CurrentStep;
            if (step != null && step.CanSkip)
            {
                step.Skip();
            }
        }

        public void SkipAll()
        {
            if (_model.CurrentSequence?.CanSkipAll == true)
            {
                Cancel();
                _model.CompleteCurrentSequence();
            }
        }

        public void Pause()
        {
            _model.Pause();
        }

        public void Resume()
        {
            _model.Resume();
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            _model.CurrentStep?.Cleanup();
            _model.Cancel();

            var sequenceId = _model.CurrentSequence?.Id;
            OnTutorialCancelled?.Invoke(sequenceId);

            _view?.Hide();
        }

        public void RegisterSequence(TutorialSequence sequence)
        {
            if (sequence != null)
            {
                _registeredSequences[sequence.Id] = sequence;
            }
        }

        public bool IsSequenceCompleted(string sequenceId)
        {
            return _model.IsSequenceCompleted(sequenceId);
        }

        private void HandleStepChanged(int index, TutorialStep step)
        {
            OnStepChanged?.Invoke(index, step);
        }

        private void HandleSequenceCompleted(string sequenceId)
        {
            OnTutorialCompleted?.Invoke(sequenceId);
        }

        private void HandleStateChanged(TutorialState state)
        {
            // Additional state handling if needed
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _model.OnStepChanged -= HandleStepChanged;
            _model.OnSequenceCompleted -= HandleSequenceCompleted;
            _model.OnStateChanged -= HandleStateChanged;
        }
    }
}
