using GameServer.Models;

namespace GameServer.Requests;

public record LoginRequest : IServerRequest
{
    public required Guid DeviceId { get; set; }
    public required ActionType ActionType { get; set; }
}