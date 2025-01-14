using System.Text.Json;
using GameServer.Requests;
using GameServer.Responses;

namespace GameServer.Handlers;

/// <summary>
/// Handles the request to send a gift from one player to another.
/// </summary>
/// <param name="request">The request containing the details of the gift to be sent.</param>
/// <param name="gameContext">The game context containing shared resources like logger and player state service.</param>
public class LoginHandler(LoginRequest request,
    IGameContext gameContext) : IHandler
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