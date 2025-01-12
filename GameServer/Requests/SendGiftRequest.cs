using GameServer.Models;

namespace GameServer;

public class SendGiftRequest
{
    public string FriendPlayerId { get; set; }
    public ResourceType ResourceType { get; set; }
    public int Amount { get; set; }
    
    public string PlayerId { get; set; }
}