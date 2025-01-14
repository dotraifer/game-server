using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using GameServer.Models;
using Serilog;

namespace GameServer;

/// <summary>
/// Service to manage player states and connections.
/// </summary>
public class PlayerStateService(ILogger logger) : IPlayerStateService
{
    private readonly ConcurrentDictionary<string, Player> _players = new();
    private readonly ConcurrentDictionary<string, WebSocket> _onlinePlayers = new();
    
    public bool IsPlayerConnected(string deviceId) => _players.ContainsKey(deviceId);
    
    public string ConnectPlayer(string deviceId)
    {
        var playerId = Guid.NewGuid().ToString();
        _players[deviceId] = new Player { PlayerId = playerId, DeviceId = deviceId };
        logger.Information("Player connected. DeviceId: {DeviceId}, PlayerId: {PlayerId}", deviceId, playerId);
        return playerId;
    }
    
    public void AddOnlinePlayer(string playerId, WebSocket socket)
    {
        _onlinePlayers[playerId] = socket;
    }
    
    public Player GetPlayerById(string playerId)
    {
        logger.Information("Fetching player with ID {PlayerId}", playerId);
        var player = _players.Values.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null)
        {
            logger.Error("Player with ID {PlayerId} not found", playerId);
            throw new Exception("Player not found.");
        }
        logger.Information("Player with ID {PlayerId} found", playerId);
        return player;
    }
    
    public int UpdateResources(string playerId, string resourceType, int resourceValue)
    {
        var player = GetPlayerById(playerId);
        if (player == null) throw new Exception("Player not found.");

        logger.Information("Updating resources for player {PlayerId}. ResourceType: {ResourceType}, ResourceValue: {ResourceValue}", playerId, resourceType, resourceValue);

        if (resourceType == "Coins") player.Coins += resourceValue;
        if (resourceType == "Rolls") player.Rolls += resourceValue;

        var updatedValue = resourceType == "Coins" ? player.Coins : player.Rolls;
        logger.Information("Updated resources for player {PlayerId}. ResourceType: {ResourceType}, UpdatedValue: {UpdatedValue}", playerId, resourceType, updatedValue);

        return updatedValue;
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
            try
            {
                socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None)
                    .GetAwaiter().GetResult();
                logger.Information("Notification sent to player {PlayerId}: {Message}", playerId, message);
            }
            catch (Exception ex)
            {
                logger.Error("Error sending notification to player {PlayerId}: {Message}. Exception: {Exception}", playerId, message, ex);
            }
        }
        else
        {
            logger.Warning("Failed to send notification. Player {PlayerId} is not online or WebSocket is not open.", playerId);
        }
    }
}