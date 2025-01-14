using GameServer.Models;

namespace GameServer.Requests;

public class UpdateResourceRequest : IServerRequest
{
    public required string PlayerId { get; set; }
    public required ResourceType ResourceType { get; set; }
    public required int Amount { get; set; }
    public required ActionType ActionType { get; set; }
}