using GameServer.Models;

namespace GameServer;

public class UpdateResourceRequest : IServerRequest
{
    public string PlayerId { get; set; }
    public ResourceType ResourceType { get; set; }
    public int Amount { get; set; }
    public Type ActionType { get; set; }
}