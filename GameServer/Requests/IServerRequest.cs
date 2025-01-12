namespace GameServer;

public interface IServerRequest
{
    Type ActionType { get; set; }
}