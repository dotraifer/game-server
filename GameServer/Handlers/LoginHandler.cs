using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GameServer.Requests;
using GameServer.Responses;

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
        var response = new LoginResponse
        {
            PlayerId = playerId
        };
        
        var json = JsonSerializer.Serialize(response);
        
        return Task.FromResult(json);
    }
}