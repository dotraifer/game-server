using GameServer.Models;

namespace GameServer.Responses;

public record SendGiftResponse
{
    public required string PlayerId { get; set; }
    
    public required string FriendPlayerId { get; set; }
    
    public required ResourceType ResourceType { get; set; }
    
    public required int Amount { get; set; }
}