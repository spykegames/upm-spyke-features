using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spyke.Features.Tutorial
{
    /// <summary>
    /// View interface for tutorial UI.
    /// Implement this for your specific UI implementation.
    /// </summary>
    public interface ITutorialView
    {
        /// <summary>
        /// Shows the tutorial overlay.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the tutorial overlay.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows a message to the user.
        /// </summary>
        void ShowMessage(string title, string message);

        /// <summary>
        /// Hides the message.
        /// </summary>
        void HideMessage();

        /// <summary>
        /// Highlights a target element by ID.
        /// </summary>
        void HighlightTarget(string targetId);

        /// <summary>
        /// Clears any active highlight.
        /// </summary>
        void ClearHighlight();

        /// <summary>
        /// Shows a pointer/hand at a position.
        /// </summary>
        void ShowPointer(Vector2 position, bool animate = true);

        /// <summary>
        /// Hides the pointer.
        /// </summary>
        void HidePointer();

        /// <summary>
        /// Sets the progress indicator.
        /// </summary>
        void SetProgress(float progress);

        /// <summary>
        /// Waits for user to tap anywhere.
        /// </summary>
        UniTask WaitForTapAsync();

        /// <summary>
        /// Waits for user to click on a specific target.
        /// </summary>
        UniTask WaitForTargetClickAsync(string targetId);
    }
}
