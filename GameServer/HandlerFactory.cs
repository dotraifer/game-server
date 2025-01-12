using System.Text.Json;
using GameServer.Handlers;
using GameServer.Models;

namespace GameServer;

public static class HandlerFactory
{
    public static IHandler CreateObject(string message, GameContext gameContext)
    {
        var json = JsonDocument.Parse(message);
        var typeString = json.RootElement.GetProperty("type").GetString();
        if (typeString == null) throw new Exception("Type is required");

        // Use Enum.TryParse to handle case insensitivity
        if (!Enum.TryParse<ActionType>(typeString, true, out var type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), "Invalid action type");
        }

        switch (type)
        {
            case ActionType.Login:
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new LoginHandler(loginRequest, gameContext);
            case ActionType.UpdateResources:
                var updateResourceRequest = JsonSerializer.Deserialize<UpdateResourceRequest>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new UpdateResourceHandler(updateResourceRequest, gameContext);
            case ActionType.SendGift:
                var sendGiftRequest = JsonSerializer.Deserialize<SendGiftRequest>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new SendGiftHandler(sendGiftRequest, gameContext);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}