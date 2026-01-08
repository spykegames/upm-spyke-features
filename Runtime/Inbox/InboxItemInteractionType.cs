namespace Spyke.Features.Inbox
{
    /// <summary>
    /// How inbox items can be interacted with.
    /// </summary>
    public enum InboxItemInteractionType
    {
        None = 0,

        /// <summary>
        /// Item can be claimed/collected.
        /// </summary>
        Collectable = 1,

        /// <summary>
        /// Item represents something sent by the player.
        /// </summary>
        Sent = 2,

        /// <summary>
        /// Item is read-only information.
        /// </summary>
        ReadOnly = 3,

        /// <summary>
        /// Item requires a response/action.
        /// </summary>
        Actionable = 4
    }
}
