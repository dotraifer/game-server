using System.Net.WebSockets;

namespace GameServer.Handlers;

public class LoginHandler(LoginRequest request,
    GameContext gameContext) : IHandler
{
    public Task<string> HandleAsync()
    {
        var deviceId = request.DeviceId;
        if (gameContext.PlayerStateService.IsPlayerConnected(deviceId.ToString()))
        {
            return Task.FromResult("Player already connected.");
        }

        var playerId = gameContext.PlayerStateService.ConnectPlayer(deviceId.ToString());
        return Task.FromResult($"{{\"PlayerId\":\"{playerId}\"}}");
    }
}