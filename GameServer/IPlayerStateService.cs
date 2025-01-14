using System.Net.WebSockets;
using GameServer.Models;

namespace GameServer;

/// <summary>
/// Interface for managing player state and connections.
/// </summary>
public interface IPlayerStateService
{
    /// <summary>
    /// Checks if a player is connected based on their device ID.
    /// </summary>
    /// <param name="deviceId">The device ID of the player.</param>
    /// <returns>True if the player is connected, otherwise false.</returns>
    bool IsPlayerConnected(string deviceId);
    
    /// <summary>
    /// Connects a player and assigns a new player ID.
    /// </summary>
    /// <param name="deviceId">The device ID of the player.</param>
    /// <returns>The newly assigned player ID.</returns>
    string ConnectPlayer(string deviceId);

    /// <summary>
    /// Adds a player to the list of online players.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="socket">The WebSocket connection of the player.</param>
    void AddOnlinePlayer(string playerId, WebSocket socket);

    /// <summary>
    /// Sends a notification message to a player.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="message">The notification message.</param>
    void SendNotificationToPlayer(string playerId, string message);

    /// <summary>
    /// Checks if a player is online based on their player ID.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <returns>True if the player is online, otherwise false.</returns>
    bool IsPlayerOnline(string playerId);

    /// <summary>
    /// Updates the resources of a player.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="resourceType">The type of resource to update.</param>
    /// <param name="resourceValue">The value to update the resource by.</param>
    /// <returns>The updated resource value.</returns>
    int UpdateResources(string playerId, string resourceType, int resourceValue);
    
    Player GetPlayerById(string playerId);
}