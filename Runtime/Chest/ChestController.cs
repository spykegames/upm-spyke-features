using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// Controller implementation for chest operations.
    /// </summary>
    public class ChestController : IChestController, IInitializable, IDisposable
    {
        [Inject(Optional = true)] private readonly ChestConfig _config;
        [Inject(Optional = true)] private readonly IChestView _view;

        private ChestModel _currentChest;
        private bool _isOpening;
        private bool _skipRequested;

        public bool IsOpening => _isOpening;

        public event Action<ChestModel> OnChestOpening;
        public event Action<ChestModel, IReadOnlyList<ChestReward>> OnChestOpened;
        public event Action<ChestReward> OnRewardRevealed;

        public void Initialize()
        {
            // Initialize if needed
        }

        public async UniTask<IReadOnlyList<ChestReward>> OpenChestAsync(ChestModel chest)
        {
            if (chest == null)
            {
                Debug.LogWarning("[ChestController] Cannot open null chest.");
                return Array.Empty<ChestReward>();
            }

            if (_isOpening)
            {
                Debug.LogWarning("[ChestController] Already opening a chest.");
                return Array.Empty<ChestReward>();
            }

            if (!chest.CanOpen)
            {
                Debug.LogWarning($"[ChestController] Chest {chest.ChestId} cannot be opened yet.");
                return Array.Empty<ChestReward>();
            }

            _isOpening = true;
            _skipRequested = false;
            _currentChest = chest;
            chest.State = ChestState.Opening;

            OnChestOpening?.Invoke(chest);

            try
            {
                // Play open animation if view is available
                if (_view != null)
                {
                    var typeConfig = _config?.GetTypeConfig(chest.Type);
                    await _view.PlayOpenAnimationAsync(chest, typeConfig);
                }
                else
                {
                    // Default delay if no view
                    await UniTask.Delay(TimeSpan.FromSeconds(_config?.OpenDuration ?? 1f));
                }

                // Reveal rewards
                var rewardDelay = _config?.RewardDisplayDelay ?? 0.5f;
                var rewardInterval = _config?.RewardDisplayInterval ?? 0.2f;

                await UniTask.Delay(TimeSpan.FromSeconds(rewardDelay));

                foreach (var reward in chest.Rewards)
                {
                    if (_skipRequested) break;

                    OnRewardRevealed?.Invoke(reward);
                    _view?.ShowReward(reward);

                    await UniTask.Delay(TimeSpan.FromSeconds(rewardInterval));
                }

                chest.State = ChestState.Opened;
                OnChestOpened?.Invoke(chest, chest.Rewards);

                return chest.Rewards;
            }
            finally
            {
                _isOpening = false;
                _currentChest = null;
            }
        }

        public async UniTask<IReadOnlyList<ChestReward>> OpenChestAsync(string chestId)
        {
            // This would typically load from a data source
            // For now, create a simple chest model
            var chest = new ChestModel(chestId, ChestType.Common, new List<ChestReward>());
            return await OpenChestAsync(chest);
        }

        public void SkipAnimation()
        {
            _skipRequested = true;
            _view?.SkipAnimation();
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
