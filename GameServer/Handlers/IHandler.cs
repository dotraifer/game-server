namespace GameServer.Handlers;

/// <summary>
/// Interface for handling game server requests.
/// </summary>
public interface IHandler
{
    /// <summary>
    /// Handles the request asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the JSON response.</returns>
    Task<string> HandleAsync();
}