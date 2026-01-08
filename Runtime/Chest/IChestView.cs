using Cysharp.Threading.Tasks;

namespace Spyke.Features.Chest
{
    /// <summary>
    /// View interface for chest UI and animations.
    /// Implement this for your specific animation system (Unity Animation, Spine, etc.).
    /// </summary>
    public interface IChestView
    {
        /// <summary>
        /// Plays the chest opening animation.
        /// </summary>
        /// <param name="chest">The chest being opened.</param>
        /// <param name="typeConfig">Configuration for this chest type.</param>
        UniTask PlayOpenAnimationAsync(ChestModel chest, ChestTypeConfig typeConfig);

        /// <summary>
        /// Shows a revealed reward.
        /// </summary>
        /// <param name="reward">The reward to display.</param>
        void ShowReward(ChestReward reward);

        /// <summary>
        /// Skips any currently playing animation.
        /// </summary>
        void SkipAnimation();

        /// <summary>
        /// Hides the chest view.
        /// </summary>
        void Hide();

        /// <summary>
        /// Prepares the view for reuse.
        /// </summary>
        void Reset();
    }
}
