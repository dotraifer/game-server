using System.Text.Json;
using GameServer.Handlers;
using GameServer.Models;
using GameServer.Requests;
using Serilog;

namespace GameServer;

/// <summary>
/// Factory class to create handler objects based on the action type in the message.
/// </summary>
public static class HandlerFactory
{
    /// <summary>
    /// Creates an appropriate handler object based on the action type in the message.
    /// </summary>
    /// <param name="message">The JSON message containing the action type and request data.</param>
    /// <param name="gameContext">The game context containing shared resources like logger and player state service.</param>
    /// <returns>An instance of a class that implements the <see cref="IHandler"/> interface.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the action type is not recognized.</exception>
    public static IHandler CreateObject(string message, ILogger logger, IPlayerStateService playerStateService)
    {
        var json = JsonDocument.Parse(message);
        var typeString = json.RootElement.GetProperty("ActionType").GetInt32();

        var type = (ActionType)typeString;

        switch (type)
        {
            case ActionType.Login:
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new LoginHandler(loginRequest!, logger, playerStateService);
            case ActionType.UpdateResources:
                var updateResourceRequest = JsonSerializer.Deserialize<UpdateResourceRequest>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new UpdateResourceHandler(updateResourceRequest!, logger, playerStateService);
            case ActionType.SendGift:
                var sendGiftRequest = JsonSerializer.Deserialize<SendGiftRequest>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new SendGiftHandler(sendGiftRequest!, logger, playerStateService);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}