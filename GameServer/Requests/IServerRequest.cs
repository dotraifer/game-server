using GameServer.Models;

namespace GameServer.Requests;

public interface IServerRequest
{
    ActionType ActionType { get; set; }
}