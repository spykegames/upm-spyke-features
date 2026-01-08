using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// Controller interface for chest operations.
    /// </summary>
    public interface IChestController
    {
        /// <summary>
        /// Opens a chest and plays the opening animation.
        /// </summary>
        /// <param name="chest">The chest to open.</param>
        /// <returns>The rewards from the chest.</returns>
        UniTask<IReadOnlyList<ChestReward>> OpenChestAsync(ChestModel chest);

        /// <summary>
        /// Opens a chest by ID.
        /// </summary>
        /// <param name="chestId">The chest ID.</param>
        /// <returns>The rewards from the chest.</returns>
        UniTask<IReadOnlyList<ChestReward>> OpenChestAsync(string chestId);

        /// <summary>
        /// Skips the opening animation if one is playing.
        /// </summary>
        void SkipAnimation();

        /// <summary>
        /// Whether a chest is currently being opened.
        /// </summary>
        bool IsOpening { get; }

        /// <summary>
        /// Event fired when chest opening starts.
        /// </summary>
        event Action<ChestModel> OnChestOpening;

        /// <summary>
        /// Event fired when chest opening completes.
        /// </summary>
        event Action<ChestModel, IReadOnlyList<ChestReward>> OnChestOpened;

        /// <summary>
        /// Event fired for each reward revealed.
        /// </summary>
        event Action<ChestReward> OnRewardRevealed;
    }
}
