using System.Text.Json;
using GameServer.Handlers;
using GameServer.Models;
using GameServer.Requests;

namespace GameServer;

public static class HandlerFactory
{
    public static IHandler CreateObject(string message, GameContext gameContext)
    {
        var json = JsonDocument.Parse(message);
        var typeString = json.RootElement.GetProperty("ActionType").GetInt32();

        var type = (ActionType)typeString;

        switch (type)
        {
            case ActionType.Login:
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new LoginHandler(loginRequest!, gameContext);
            case ActionType.UpdateResources:
                var updateResourceRequest = JsonSerializer.Deserialize<UpdateResourceRequest>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new UpdateResourceHandler(updateResourceRequest!, gameContext);
            case ActionType.SendGift:
                var sendGiftRequest = JsonSerializer.Deserialize<SendGiftRequest>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new SendGiftHandler(sendGiftRequest!, gameContext);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}