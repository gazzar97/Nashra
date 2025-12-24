namespace SportsData.Modules.Competitions.Domain
{
    /// <summary>
    /// Represents the current status of a football match.
    /// Stored as integer in database for performance, serialized as string in API responses.
    /// </summary>
    public enum MatchStatus
    {
        /// <summary>Fixture announced, not started</summary>
        Scheduled = 0,
        
        /// <summary>Match in progress</summary>
        Live = 1,
        
        /// <summary>Final result available</summary>
        Finished = 2,
        
        /// <summary>Match delayed</summary>
        Postponed = 3,
        
        /// <summary>Match void</summary>
        Cancelled = 4
    }
}
