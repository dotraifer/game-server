using System.Text.Json;
using GameServer.Models;
using GameServer.Requests;
using GameServer.Responses;

namespace GameServer.Handlers;

/// <summary>
/// Handles the request to send a gift from one player to another.
/// </summary>
/// <param name="request">The request containing the details of the gift to be sent.</param>
/// <param name="gameContext">The game context containing shared resources like logger and player state service.</param>
public class SendGiftHandler(SendGiftRequest request, GameContext gameContext) : IHandler
{
    public async Task<string> HandleAsync()
    {
        gameContext.Logger.Information($"Handling gift request from Player {request.PlayerId} to Player {request.FriendPlayerId}.");

        SendGift(request.PlayerId, request.FriendPlayerId, request.ResourceType, request.Amount);

        var response = new SendGiftResponse
        {
            PlayerId = request.PlayerId,
            FriendPlayerId = request.FriendPlayerId,
            Amount = request.Amount,
            ResourceType = request.ResourceType
        };

        var json = JsonSerializer.Serialize(response);
        gameContext.Logger.Information($"Gift response: {json}");

        return await Task.FromResult(json);
    }

    /// <summary>
    /// Sends a gift from one player to another.
    /// </summary>
    /// <param name="senderPlayerId">The player ID of the sender.</param>
    /// <param name="recipientPlayerId">The player ID of the recipient.</param>
    /// <param name="resourceType">The type of resource being sent.</param>
    /// <param name="resourceValue">The amount of the resource being sent.</param>
    /// <exception cref="Exception">Thrown when the sender or recipient is not found or when there are insufficient resources.</exception>
    private void SendGift(string senderPlayerId, string recipientPlayerId, ResourceType resourceType, int resourceValue)
    {
        var sender = gameContext.PlayerStateService.GetPlayerById(senderPlayerId);
        var recipient = gameContext.PlayerStateService.GetPlayerById(recipientPlayerId);

        if (sender == null || recipient == null)
        {
            gameContext.Logger.Error("Sender or recipient not found.");
            throw new Exception("Sender or recipient not found.");
        }

        if ((resourceType == ResourceType.Coins && sender.Coins < resourceValue) ||
            (resourceType == ResourceType.Rolls && sender.Rolls < resourceValue))
        {
            gameContext.Logger.Error("Insufficient resources.");
            throw new Exception("Insufficient resources.");
        }

        if (resourceType == ResourceType.Coins)
        {
            sender.Coins -= resourceValue;
            recipient.Coins += resourceValue;
        }
        else if (resourceType == ResourceType.Rolls)
        {
            sender.Rolls -= resourceValue;
            recipient.Rolls += resourceValue;
        }

        gameContext.Logger.Information(
            $"Gift sent from Player {senderPlayerId} to Player {recipientPlayerId}. Resource: {resourceType}, Value: {resourceValue}.");

        if (gameContext.PlayerStateService.IsPlayerOnline(recipientPlayerId))
        {
            var notification = new
            {
                Type = "GiftEvent",
                From = senderPlayerId,
                ResourceType = resourceType,
                ResourceValue = resourceValue
            };

            var jsonNotification = JsonSerializer.Serialize(notification);
            gameContext.PlayerStateService.SendNotificationToPlayer(recipientPlayerId, jsonNotification);
            gameContext.Logger.Information($"Notification sent to Player {recipientPlayerId}: {jsonNotification}");
        }
    }
}