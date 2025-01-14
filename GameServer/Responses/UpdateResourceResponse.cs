namespace GameServer.Responses;

/// <summary>
/// Represents the response for updating a player's resources.
/// </summary>
public record UpdateResourceResponse
{
    /// <summary>
    /// New balance of the resource after the update.
    /// </summary>
    public required int NewBalance { get; set; }
}