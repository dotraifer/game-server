using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using GameServer.Models;

namespace GameServer;

public class PlayerStateService : IPlayerStateService
{
    private readonly ConcurrentDictionary<string, Player> Players = new();
    private readonly ConcurrentDictionary<string, WebSocket> OnlinePlayers = new();

    public bool IsPlayerConnected(string deviceId) => Players.ContainsKey(deviceId);

    public string ConnectPlayer(string deviceId)
    {
        var playerId = Guid.NewGuid().ToString();
        Players[deviceId] = new Player { PlayerId = playerId, DeviceId = deviceId };
        return playerId;
    }

    public void AddOnlinePlayer(string playerId, WebSocket socket)
    {
        OnlinePlayers[playerId] = socket;
    }

    public void RemovePlayer(string playerId)
    {
        var entry = Players.FirstOrDefault(p => p.Value.PlayerId == playerId);
        if (!string.IsNullOrEmpty(entry.Key))
        {
            Players.TryRemove(entry.Key, out _);
            OnlinePlayers.TryRemove(playerId, out _);
        }
    }

    public Player GetPlayerById(string playerId)
    {
        return Players.Values.FirstOrDefault(p => p.PlayerId == playerId);
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
        return OnlinePlayers.ContainsKey(playerId);
    }

    public void SendNotificationToPlayer(string playerId, string message)
    {
        if (OnlinePlayers.TryGetValue(playerId, out var socket) && socket.State == WebSocketState.Open)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None)
                .GetAwaiter().GetResult();
        }
    }
}