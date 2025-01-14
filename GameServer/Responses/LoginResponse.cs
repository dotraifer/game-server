namespace GameServer.Responses;

/// <summary>
/// Represents the response for a login request.
/// </summary>
public record LoginResponse : IServerResponse
{
    /// <summary>
    /// Player ID of the logged-in player.
    /// </summary>
    public required string PlayerId { get; set; }
}