namespace GameServer;

public class LoginRequest : IServerRequest
{
    public Guid DeviceId { get; set; }
    public Type ActionType { get; set; }
}