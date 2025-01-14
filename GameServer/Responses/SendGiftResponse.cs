using GameServer.Models;

namespace GameServer.Responses;

/// <summary>
/// Represents the response for sending a gift to another player.
/// </summary>
public record SendGiftResponse : IServerResponse
{
    /// <summary>
    /// Player ID of the sender.
    /// </summary>
    public required string PlayerId { get; set; }
    
    /// <summary>
    /// Player ID of the recipient.
    /// </summary>
    public required string FriendPlayerId { get; set; }
    
    /// <summary>
    /// Type of resource being sent.
    /// </summary>
    public required ResourceType ResourceType { get; set; }
    
    /// <summary>
    /// Amount of the resource being sent.
    /// </summary>
    public required int Amount { get; set; }
}