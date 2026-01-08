using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// State of a tutorial step.
    /// </summary>
    public enum TutorialStepState
    {
        Pending,
        Active,
        Completed,
        Skipped
    }

    /// <summary>
    /// Base class for tutorial steps.
    /// Inherit from this to create custom step implementations.
    /// </summary>
    [Serializable]
    public abstract class TutorialStep
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;
        [SerializeField] private string _message;
        [SerializeField] private bool _canSkip = true;
        [SerializeField] private float _delayBefore;
        [SerializeField] private float _delayAfter;

        private TutorialStepState _state = TutorialStepState.Pending;

        /// <summary>
        /// Unique identifier for this step.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Title to display.
        /// </summary>
        public string Title => _title;

        /// <summary>
        /// Message/instructions to display.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Whether this step can be skipped.
        /// </summary>
        public bool CanSkip => _canSkip;

        /// <summary>
        /// Delay before starting the step (seconds).
        /// </summary>
        public float DelayBefore => _delayBefore;

        /// <summary>
        /// Delay after completing the step (seconds).
        /// </summary>
        public float DelayAfter => _delayAfter;

        /// <summary>
        /// Current state of the step.
        /// </summary>
        public TutorialStepState State
        {
            get => _state;
            set => _state = value;
        }

        /// <summary>
        /// Fired when the step completes.
        /// </summary>
        public event Action<TutorialStep> OnCompleted;

        protected TutorialStep() { }

        protected TutorialStep(string id, string title = null, string message = null, bool canSkip = true)
        {
            _id = id;
            _title = title;
            _message = message;
            _canSkip = canSkip;
        }

        /// <summary>
        /// Called when the step starts. Override to implement step logic.
        /// </summary>
        public abstract UniTask ExecuteAsync(ITutorialView view);

        /// <summary>
        /// Called to clean up the step. Override if needed.
        /// </summary>
        public virtual void Cleanup() { }

        /// <summary>
        /// Marks the step as completed.
        /// </summary>
        public void Complete()
        {
            State = TutorialStepState.Completed;
            OnCompleted?.Invoke(this);
        }

        /// <summary>
        /// Marks the step as skipped.
        /// </summary>
        public void Skip()
        {
            State = TutorialStepState.Skipped;
            OnCompleted?.Invoke(this);
        }
    }

    /// <summary>
    /// Simple message-only tutorial step.
    /// </summary>
    public class MessageStep : TutorialStep
    {
        private readonly bool _waitForTap;

        public MessageStep(string id, string title, string message, bool waitForTap = true)
            : base(id, title, message)
        {
            _waitForTap = waitForTap;
        }

        public override async UniTask ExecuteAsync(ITutorialView view)
        {
            view.ShowMessage(Title, Message);

            if (_waitForTap)
            {
                await view.WaitForTapAsync();
            }

            Complete();
        }
    }

    /// <summary>
    /// Step that highlights a target element.
    /// </summary>
    public class HighlightStep : TutorialStep
    {
        private readonly string _targetId;
        private readonly bool _waitForTargetClick;

        public string TargetId => _targetId;

        public HighlightStep(string id, string targetId, string title = null, string message = null, bool waitForTargetClick = true)
            : base(id, title, message)
        {
            _targetId = targetId;
            _waitForTargetClick = waitForTargetClick;
        }

        public override async UniTask ExecuteAsync(ITutorialView view)
        {
            view.ShowMessage(Title, Message);
            view.HighlightTarget(_targetId);

            if (_waitForTargetClick)
            {
                await view.WaitForTargetClickAsync(_targetId);
            }
            else
            {
                await view.WaitForTapAsync();
            }

            Complete();
        }

        public override void Cleanup()
        {
            // View will handle clearing highlight
        }
    }

    /// <summary>
    /// Step that shows a pointer/hand indicating where to tap.
    /// </summary>
    public class PointerStep : TutorialStep
    {
        private readonly Vector2 _position;
        private readonly bool _animate;

        public Vector2 Position => _position;
        public bool Animate => _animate;

        public PointerStep(string id, Vector2 position, string message = null, bool animate = true)
            : base(id, null, message)
        {
            _position = position;
            _animate = animate;
        }

        public override async UniTask ExecuteAsync(ITutorialView view)
        {
            view.ShowMessage(null, Message);
            view.ShowPointer(_position, _animate);

            await view.WaitForTapAsync();

            Complete();
        }

        public override void Cleanup()
        {
            // View will handle hiding pointer
        }
    }
}
