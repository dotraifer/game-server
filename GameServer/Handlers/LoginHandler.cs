using System.Text.Json;
using GameServer.Requests;
using GameServer.Responses;
using Serilog;

namespace GameServer.Handlers;

/// <summary>
/// Handles the login request for a player.
/// </summary>
/// <param name="request">The request containing the device ID of the player.</param>
/// <param name="logger">The logger instance for logging information.</param>
/// <param name="playerStateService">The service for managing player states and connections.</param>
public class LoginHandler(LoginRequest request,
    ILogger logger, IPlayerStateService playerStateService) : IHandler
{
    public Task<string> HandleAsync()
    {
        var deviceId = request.DeviceId;
        if (playerStateService.IsPlayerConnected(deviceId.ToString()))
        {
            return Task.FromResult("Player already connected.");
        }

        var playerId = playerStateService.ConnectPlayer(deviceId.ToString());
        var response = new LoginResponse
        {
            PlayerId = playerId
        };
        
        var json = JsonSerializer.Serialize(response);
        
        return Task.FromResult(json);
    }
}