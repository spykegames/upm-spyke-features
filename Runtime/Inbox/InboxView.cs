using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace Spyke.Features.Inbox
{
    /// <summary>
    /// Base implementation of IInboxView.
    /// Extend this for custom implementations.
    /// </summary>
    public class InboxView : MonoBehaviour, IInboxView
    {
        [Header("Container")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private GameObject _itemPrefab;

        [Header("Badge")]
        [SerializeField] private GameObject _badgeObject;
        [SerializeField] private TextMeshProUGUI _badgeText;

        [Header("Empty State")]
        [SerializeField] private GameObject _emptyStateObject;

        [Header("Buttons")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _claimAllButton;

        [Header("Animation")]
        [SerializeField] private float _showDuration = 0.3f;
        [SerializeField] private float _itemAnimDelay = 0.05f;

        private readonly List<GameObject> _spawnedItems = new();

        protected virtual void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(Hide);
            }
        }

        public void RefreshItems(IReadOnlyList<InboxItem> items)
        {
            ClearItems();

            if (items == null || items.Count == 0)
            {
                ShowEmptyState(true);
                return;
            }

            ShowEmptyState(false);

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var itemObj = SpawnItem(item);

                // Animate in with delay
                var rectTransform = itemObj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.zero;
                    Tween.Scale(rectTransform, Vector3.one, 0.2f, Ease.OutBack, startDelay: i * _itemAnimDelay);
                }
            }
        }

        public void ShowClaimAnimation(InboxItem item)
        {
            // Find the item view and animate
            // Override in subclass for custom animation
        }

        public void ShowNewItemNotification(InboxItem item)
        {
            // Show notification toast
            // Override in subclass for custom notification
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                Tween.Alpha(_canvasGroup, 1f, _showDuration, Ease.OutQuad);
            }
        }

        public void Hide()
        {
            if (_canvasGroup != null)
            {
                Tween.Alpha(_canvasGroup, 0f, _showDuration, Ease.InQuad)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void UpdateBadgeCount(int count)
        {
            if (_badgeObject != null)
            {
                _badgeObject.SetActive(count > 0);
            }

            if (_badgeText != null)
            {
                _badgeText.text = count > 99 ? "99+" : count.ToString();
            }
        }

        protected virtual GameObject SpawnItem(InboxItem item)
        {
            if (_itemContainer == null || _itemPrefab == null) return null;

            var itemObj = Instantiate(_itemPrefab, _itemContainer);
            _spawnedItems.Add(itemObj);

            // Configure item view
            var itemView = itemObj.GetComponent<InboxItemView>();
            if (itemView != null)
            {
                itemView.Setup(item);
            }

            return itemObj;
        }

        protected void ClearItems()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _spawnedItems.Clear();
        }

        protected void ShowEmptyState(bool show)
        {
            if (_emptyStateObject != null)
            {
                _emptyStateObject.SetActive(show);
            }

            if (_itemContainer != null)
            {
                _itemContainer.gameObject.SetActive(!show);
            }
        }

        protected virtual void OnDestroy()
        {
            ClearItems();
        }
    }

    /// <summary>
    /// Individual inbox item view component.
    /// Attach to inbox item prefab.
    /// </summary>
    public class InboxItemView : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _senderNameText;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;
        [SerializeField] private Button _claimButton;
        [SerializeField] private GameObject _claimedOverlay;
        [SerializeField] private GameObject _unreadIndicator;

        private InboxItem _item;

        public InboxItem Item => _item;

        public void Setup(InboxItem item)
        {
            _item = item;

            if (_avatarImage != null && item.SenderAvatar != null)
            {
                _avatarImage.sprite = item.SenderAvatar;
            }

            if (_senderNameText != null)
            {
                _senderNameText.text = item.SenderName ?? "";
            }

            if (_titleText != null)
            {
                _titleText.text = item.Title ?? "";
            }

            if (_messageText != null)
            {
                _messageText.text = item.Message ?? "";
            }

            if (_rewardIcon != null && item.RewardIcon != null)
            {
                _rewardIcon.sprite = item.RewardIcon;
                _rewardIcon.gameObject.SetActive(true);
            }

            if (_rewardAmountText != null && item.RewardAmount > 0)
            {
                _rewardAmountText.text = item.RewardAmount.ToString();
            }

            if (_claimButton != null)
            {
                _claimButton.gameObject.SetActive(item.CanClaim);
            }

            if (_claimedOverlay != null)
            {
                _claimedOverlay.SetActive(item.IsClaimed);
            }

            if (_unreadIndicator != null)
            {
                _unreadIndicator.SetActive(!item.IsRead);
            }
        }

        public void OnClaimClicked()
        {
            // Called by button - controller should handle via events
        }
    }
}
