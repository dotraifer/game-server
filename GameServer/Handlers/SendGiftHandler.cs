using System.Text.Json;
using GameServer.Models;
using GameServer.Requests;
using GameServer.Responses;

namespace GameServer.Handlers;

public class SendGiftHandler(SendGiftRequest request, GameContext gameContext) : IHandler
{
    public Task<string> HandleAsync()
    {
        var friendId = request.FriendPlayerId;
        var resourceType = request.ResourceType;
        var resourceValue = request.Amount;
        var playerId = request.PlayerId;

        SendGift(playerId, friendId, resourceType, resourceValue);

        var response = new SendGiftResponse
        {
            PlayerId = playerId,
            FriendPlayerId = friendId,
            Amount = resourceValue,
            ResourceType = resourceType
        };
        var json = JsonSerializer.Serialize(response);
        return Task.FromResult(json);
    }

    private void SendGift(string senderPlayerId, string recipientPlayerId, ResourceType resourceType, int resourceValue)
    {
        var sender = gameContext.PlayerStateService.GetPlayerById(senderPlayerId);
        var recipient = gameContext.PlayerStateService.GetPlayerById(recipientPlayerId);

        if (sender == null || recipient == null)
        {
            throw new Exception("Sender or recipient not found.");
        }

        if (resourceType == ResourceType.Coins && sender.Coins >= resourceValue)
        {
            sender.Coins -= resourceValue;
            recipient.Coins += resourceValue;
        }
        else if (resourceType == ResourceType.Rolls && sender.Rolls >= resourceValue)
        {
            sender.Rolls -= resourceValue;
            recipient.Rolls += resourceValue;
        }
        else
        {
            throw new Exception("Insufficient resources.");
        }

        if (gameContext.PlayerStateService.IsPlayerOnline(recipientPlayerId))
        {
            var notification = new
            {
                Type = "GiftEvent",
                From = senderPlayerId,
                ResourceType = resourceType,
                ResourceValue = resourceValue
            };

            var jsonNotification = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
            gameContext.PlayerStateService.SendNotificationToPlayer(recipientPlayerId, jsonNotification);
        }

        gameContext.Logger.Information(
            $"Gift sent from Player {senderPlayerId} to Player {recipientPlayerId}." +
            $" Resource: {resourceType}, Value: {resourceValue}."
        );
    }
}