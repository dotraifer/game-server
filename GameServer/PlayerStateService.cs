using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using GameServer.Models;

namespace GameServer;

/// <summary>
/// Service to manage player states and connections.
/// </summary>
public class PlayerStateService : IPlayerStateService
{
    private readonly ConcurrentDictionary<string, Player> _players = new();
    private readonly ConcurrentDictionary<string, WebSocket> _onlinePlayers = new();

    /// <summary>
    /// Checks if a player is connected based on their device ID.
    /// </summary>
    /// <param name="deviceId">The device ID of the player.</param>
    /// <returns>True if the player is connected, otherwise false.</returns>
    public bool IsPlayerConnected(string deviceId) => _players.ContainsKey(deviceId);

    /// <summary>
    /// Connects a player and assigns a new player ID.
    /// </summary>
    /// <param name="deviceId">The device ID of the player.</param>
    /// <returns>The newly assigned player ID.</returns>
    public string ConnectPlayer(string deviceId)
    {
        var playerId = Guid.NewGuid().ToString();
        _players[deviceId] = new Player { PlayerId = playerId, DeviceId = deviceId };
        return playerId;
    }

    /// <summary>
    /// Adds a player to the list of online players.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="socket">The WebSocket connection of the player.</param>
    public void AddOnlinePlayer(string playerId, WebSocket socket)
    {
        _onlinePlayers[playerId] = socket;
    }

    /// <summary>
    /// Removes a player from the connected and online players list.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    public void RemovePlayer(string playerId)
    {
        var entry = _players.FirstOrDefault(p => p.Value.PlayerId == playerId);
        if (!string.IsNullOrEmpty(entry.Key))
        {
            _players.TryRemove(entry.Key, out _);
            _onlinePlayers.TryRemove(playerId, out _);
        }
    }

    /// <summary>
    /// Retrieves a player by their player ID.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <returns>The player object.</returns>
    /// <exception cref="Exception">Thrown if the player is not found.</exception>
    public Player GetPlayerById(string playerId)
    {
        return _players.Values.FirstOrDefault(p => p.PlayerId == playerId)
               ?? throw new Exception("Player not found.");
    }

    /// <summary>
    /// Updates the resources of a player.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="resourceType">The type of resource to update.</param>
    /// <param name="resourceValue">The value to update the resource by.</param>
    /// <returns>The updated resource value.</returns>
    /// <exception cref="Exception">Thrown if the player is not found.</exception>
    public int UpdateResources(string playerId, string resourceType, int resourceValue)
    {
        var player = GetPlayerById(playerId);
        if (player == null) throw new Exception("Player not found.");

        if (resourceType == "Coins") player.Coins += resourceValue;
        if (resourceType == "Rolls") player.Rolls += resourceValue;

        return resourceType == "Coins" ? player.Coins : player.Rolls;
    }

    /// <summary>
    /// Checks if a player is online based on their player ID.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <returns>True if the player is online, otherwise false.</returns>
    public bool IsPlayerOnline(string playerId)
    {
        return _onlinePlayers.ContainsKey(playerId);
    }

    /// <summary>
    /// Sends a notification message to a player.
    /// </summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="message">The notification message.</param>
    public void SendNotificationToPlayer(string playerId, string message)
    {
        if (_onlinePlayers.TryGetValue(playerId, out var socket) && socket.State == WebSocketState.Open)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None)
                .GetAwaiter().GetResult();
        }
    }
}