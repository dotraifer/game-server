using GameServer.Models;

namespace GameServer.Requests;

/// <summary>
/// Request to log in.
/// </summary>
public record LoginRequest : IServerRequest
{
    /// <summary>
    /// The device ID of the login request
    /// </summary>
    public required Guid DeviceId { get; init; }
    
    /// <inheritdoc/>
    public required ActionType ActionType { get; init; }
}