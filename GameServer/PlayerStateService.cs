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
    
    public bool IsPlayerConnected(string deviceId) => _players.ContainsKey(deviceId);
    
    public string ConnectPlayer(string deviceId)
    {
        var playerId = Guid.NewGuid().ToString();
        _players[deviceId] = new Player { PlayerId = playerId, DeviceId = deviceId };
        return playerId;
    }
    
    public void AddOnlinePlayer(string playerId, WebSocket socket)
    {
        _onlinePlayers[playerId] = socket;
    }
    
    public void RemovePlayer(string playerId)
    {
        var entry = _players.FirstOrDefault(p => p.Value.PlayerId == playerId);
        if (!string.IsNullOrEmpty(entry.Key))
        {
            _players.TryRemove(entry.Key, out _);
            _onlinePlayers.TryRemove(playerId, out _);
        }
    }
    
    public Player GetPlayerById(string playerId)
    {
        return _players.Values.FirstOrDefault(p => p.PlayerId == playerId)
               ?? throw new Exception("Player not found.");
    }
    
    public int UpdateResources(string playerId, string resourceType, int resourceValue)
    {
        var player = GetPlayerById(playerId);
        if (player == null) throw new Exception("Player not found.");

        if (resourceType == "Coins") player.Coins += resourceValue;
        if (resourceType == "Rolls") player.Rolls += resourceValue;

        return resourceType == "Coins" ? player.Coins : player.Rolls;
    }
    
    public bool IsPlayerOnline(string playerId)
    {
        return _onlinePlayers.ContainsKey(playerId);
    }
    
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