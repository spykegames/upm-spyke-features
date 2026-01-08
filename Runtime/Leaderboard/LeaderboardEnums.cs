namespace Spyke.Features.Leaderboard
{
    /// <summary>
    /// Leaderboard tabs.
    /// </summary>
    public enum LeaderboardTab
    {
        Players,
        Teams,
        Friends
    }

    /// <summary>
    /// Leaderboard scope.
    /// </summary>
    public enum LeaderboardScope
    {
        Global,
        Local,
        Weekly,
        Daily
    }

    /// <summary>
    /// Entry type in leaderboard.
    /// </summary>
    public enum LeaderboardEntryType
    {
        User,
        Team
    }
}
