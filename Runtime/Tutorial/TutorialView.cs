using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// Base implementation of ITutorialView.
    /// Extend this for custom implementations.
    /// </summary>
    public class TutorialView : MonoBehaviour, ITutorialView
    {
        [Header("Overlay")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _overlayBackground;
        [SerializeField] private Color _overlayColor = new(0, 0, 0, 0.7f);

        [Header("Message")]
        [SerializeField] private GameObject _messagePanel;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;

        [Header("Pointer")]
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private float _pointerBobAmount = 20f;
        [SerializeField] private float _pointerBobDuration = 0.5f;

        [Header("Highlight")]
        [SerializeField] private RectTransform _highlightMask;
        [SerializeField] private Image _highlightRing;

        [Header("Progress")]
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _progressText;

        [Header("Skip")]
        [SerializeField] private Button _skipButton;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private bool _waitingForTap;
        private bool _waitingForTarget;
        private string _targetId;
        private Sequence _pointerAnimation;

        public void Show()
        {
            gameObject.SetActive(true);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                Tween.Alpha(_canvasGroup, 1f, _fadeDuration, Ease.OutQuad);
            }

            if (_overlayBackground != null)
            {
                _overlayBackground.color = _overlayColor;
            }
        }

        public void Hide()
        {
            _pointerAnimation.Stop();
            HidePointer();
            ClearHighlight();
            HideMessage();

            if (_canvasGroup != null)
            {
                Tween.Alpha(_canvasGroup, 0f, _fadeDuration, Ease.InQuad)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void ShowMessage(string title, string message)
        {
            if (_messagePanel != null)
            {
                _messagePanel.SetActive(true);
            }

            if (_titleText != null)
            {
                _titleText.text = title ?? "";
                _titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
            }

            if (_messageText != null)
            {
                _messageText.text = message ?? "";
            }
        }

        public void HideMessage()
        {
            if (_messagePanel != null)
            {
                _messagePanel.SetActive(false);
            }
        }

        public void HighlightTarget(string targetId)
        {
            // Find target by ID (implementation depends on how targets are registered)
            var target = FindTarget(targetId);
            if (target == null) return;

            if (_highlightMask != null)
            {
                _highlightMask.gameObject.SetActive(true);
                _highlightMask.position = target.position;
                _highlightMask.sizeDelta = target.sizeDelta;
            }

            if (_highlightRing != null)
            {
                _highlightRing.gameObject.SetActive(true);
                _highlightRing.rectTransform.position = target.position;

                // Pulse animation
                Tween.Scale(_highlightRing.rectTransform, 1.1f, 0.5f, Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
            }
        }

        public void ClearHighlight()
        {
            if (_highlightMask != null)
            {
                _highlightMask.gameObject.SetActive(false);
            }

            if (_highlightRing != null)
            {
                _highlightRing.gameObject.SetActive(false);
            }
        }

        public void ShowPointer(Vector2 position, bool animate = true)
        {
            if (_pointer == null) return;

            _pointer.gameObject.SetActive(true);
            _pointer.anchoredPosition = position;

            if (animate)
            {
                // Bob up and down
                _pointerAnimation = Sequence.Create(cycles: -1)
                    .Chain(Tween.UIAnchoredPositionY(_pointer, position.y + _pointerBobAmount, _pointerBobDuration, Ease.InOutSine))
                    .Chain(Tween.UIAnchoredPositionY(_pointer, position.y, _pointerBobDuration, Ease.InOutSine));
            }
        }

        public void HidePointer()
        {
            _pointerAnimation.Stop();

            if (_pointer != null)
            {
                _pointer.gameObject.SetActive(false);
            }
        }

        public void SetProgress(float progress)
        {
            if (_progressSlider != null)
            {
                _progressSlider.value = progress;
            }

            if (_progressText != null)
            {
                _progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            }
        }

        public async UniTask WaitForTapAsync()
        {
            _waitingForTap = true;

            while (_waitingForTap)
            {
                if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    _waitingForTap = false;
                }
                await UniTask.Yield();
            }
        }

        public async UniTask WaitForTargetClickAsync(string targetId)
        {
            _waitingForTarget = true;
            _targetId = targetId;

            var target = FindTarget(targetId);
            if (target == null)
            {
                Debug.LogWarning($"[TutorialView] Target not found: {targetId}");
                _waitingForTarget = false;
                return;
            }

            while (_waitingForTarget)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (IsPointerOverTarget(target))
                    {
                        _waitingForTarget = false;
                    }
                }
                await UniTask.Yield();
            }

            _targetId = null;
        }

        /// <summary>
        /// Called to notify that a target was clicked.
        /// Can be called by registered target components.
        /// </summary>
        public void NotifyTargetClicked(string targetId)
        {
            if (_waitingForTarget && targetId == _targetId)
            {
                _waitingForTarget = false;
            }
        }

        /// <summary>
        /// Called when skip button is clicked.
        /// </summary>
        public void OnSkipClicked()
        {
            _waitingForTap = false;
            _waitingForTarget = false;
        }

        protected virtual RectTransform FindTarget(string targetId)
        {
            // Override to implement target finding by ID
            // Could use a registry, tags, or other methods
            var target = GameObject.Find(targetId);
            return target?.GetComponent<RectTransform>();
        }

        private bool IsPointerOverTarget(RectTransform target)
        {
            if (target == null) return false;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target,
                Input.mousePosition,
                null,
                out localPoint);

            return target.rect.Contains(localPoint);
        }

        protected virtual void Awake()
        {
            if (_skipButton != null)
            {
                _skipButton.onClick.AddListener(OnSkipClicked);
            }
        }
    }
}
