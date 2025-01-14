using GameServer.Models;

namespace GameServer.Requests;

/// <summary>
/// Request to send a gift from one player to another.
/// </summary>
public record SendGiftRequest : IServerRequest
{
    /// <summary>
    /// The player ID of the friend receiving the gift.
    /// </summary>
    public required string FriendPlayerId { get; init; }

    /// <summary>
    /// The type of resource being sent.
    /// </summary>
    public required ResourceType ResourceType { get; init; }

    /// <summary>
    /// The amount of the resource being sent.
    /// </summary>
    public required int Amount { get; init; }

    /// <summary>
    /// The player ID of the sender.
    /// </summary>
    public required string PlayerId { get; init; }
    
    /// <inheritdoc/>
    public required ActionType ActionType { get; init; }
}