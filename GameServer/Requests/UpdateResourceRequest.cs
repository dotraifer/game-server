using GameServer.Models;

namespace GameServer.Requests;

public record UpdateResourceRequest : IServerRequest
{
    /// <summary>
    /// The player ID of the player whose resources are being updated.
    /// </summary>
    public required string PlayerId { get; init; }
    
    /// <summary>
    /// The type of resource being updated.
    /// </summary>
    public required ResourceType ResourceType { get; init; }
    
    /// <summary>
    /// The amount of the resource being updated.
    /// </summary>
    public required int Amount { get; init; }
    
    /// <inheritdoc/>
    public required ActionType ActionType { get; init; }
}