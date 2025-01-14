using GameServer.Models;

namespace GameServer.Requests;

/// <summary>
/// Interface for server requests.
/// </summary>
public interface IServerRequest
{
    /// <summary>
    /// The action type of the request.
    /// </summary>
    ActionType ActionType { get; init; }
}