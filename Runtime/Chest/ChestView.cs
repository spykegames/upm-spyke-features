using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// Base implementation of IChestView using Unity UI and PrimeTween.
    /// Extend this for custom implementations.
    /// </summary>
    public class ChestView : MonoBehaviour, IChestView
    {
        [Header("Chest Display")]
        [SerializeField] private Image _chestImage;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _chestTransform;

        [Header("Reward Display")]
        [SerializeField] private Transform _rewardContainer;
        [SerializeField] private GameObject _rewardItemPrefab;

        [Header("Animation")]
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeIntensity = 10f;
        [SerializeField] private float _openScalePop = 1.3f;

        private Sequence _currentAnimation;
        private bool _skipRequested;
        private readonly List<GameObject> _spawnedRewards = new();

        public async UniTask PlayOpenAnimationAsync(ChestModel chest, ChestTypeConfig typeConfig)
        {
            _skipRequested = false;

            // Set chest image
            if (_chestImage != null && typeConfig?.ClosedIcon != null)
            {
                _chestImage.sprite = typeConfig.ClosedIcon;
            }

            // Show
            gameObject.SetActive(true);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }

            // Shake animation before opening
            if (!_skipRequested && _chestTransform != null)
            {
                await PlayShakeAnimation();
            }

            if (_skipRequested) return;

            // Scale pop on open
            if (_chestTransform != null)
            {
                _currentAnimation = Sequence.Create()
                    .Chain(Tween.Scale(_chestTransform, _openScalePop, 0.2f, Ease.OutBack))
                    .Chain(Tween.Scale(_chestTransform, 1f, 0.1f, Ease.InOutQuad));

                await _currentAnimation.ToUniTask();
            }

            // Change to open icon
            if (_chestImage != null && typeConfig?.OpenIcon != null)
            {
                _chestImage.sprite = typeConfig.OpenIcon;
            }
        }

        private async UniTask PlayShakeAnimation()
        {
            if (_chestTransform == null) return;

            var originalPos = _chestTransform.localPosition;
            var elapsed = 0f;

            while (elapsed < _shakeDuration && !_skipRequested)
            {
                var offsetX = Random.Range(-_shakeIntensity, _shakeIntensity);
                var offsetY = Random.Range(-_shakeIntensity, _shakeIntensity);
                _chestTransform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }

            _chestTransform.localPosition = originalPos;
        }

        public void ShowReward(ChestReward reward)
        {
            if (_rewardContainer == null || _rewardItemPrefab == null) return;

            var rewardObj = Instantiate(_rewardItemPrefab, _rewardContainer);
            _spawnedRewards.Add(rewardObj);

            // Configure reward item (would need a reward item component)
            var image = rewardObj.GetComponentInChildren<Image>();
            if (image != null && reward.Icon != null)
            {
                image.sprite = reward.Icon;
            }

            // Animate in
            var rectTransform = rewardObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.zero;
                Tween.Scale(rectTransform, Vector3.one, 0.3f, Ease.OutBack);
            }
        }

        public void SkipAnimation()
        {
            _skipRequested = true;
            _currentAnimation.Stop();
        }

        public void Hide()
        {
            if (_canvasGroup != null)
            {
                Tween.Alpha(_canvasGroup, 0f, 0.3f, Ease.InQuad)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            _skipRequested = false;
            _currentAnimation.Stop();

            // Clear spawned rewards
            foreach (var reward in _spawnedRewards)
            {
                if (reward != null)
                {
                    Destroy(reward);
                }
            }
            _spawnedRewards.Clear();

            // Reset transform
            if (_chestTransform != null)
            {
                _chestTransform.localScale = Vector3.one;
                _chestTransform.localPosition = Vector3.zero;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }
    }
}
