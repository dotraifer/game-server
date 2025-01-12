using System.Net.WebSockets;

namespace GameServer;

public interface IPlayerStateService
{
    bool IsPlayerConnected(string deviceId);
    
    string ConnectPlayer(string deviceId);

    void AddOnlinePlayer(string playerId, WebSocket socket);


}