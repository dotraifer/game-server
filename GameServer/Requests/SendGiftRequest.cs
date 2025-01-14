using GameServer.Models;

namespace GameServer.Requests;

public class SendGiftRequest : IServerRequest
{
    public required string FriendPlayerId { get; set; }
    public required ResourceType ResourceType { get; set; }
    public required int Amount { get; set; }
    
    public required string PlayerId { get; set; }

    public required ActionType ActionType { get; set; }
}