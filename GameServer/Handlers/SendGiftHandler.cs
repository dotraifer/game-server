using System.Text.Json;
using GameServer.Models;
using GameServer.Requests;
using GameServer.Responses;
using Serilog;

namespace GameServer.Handlers;

/// <summary>
/// Handles the request to send a gift from one player to another.
/// </summary>
/// <param name="request">The request containing the details of the gift to be sent.</param>
/// <param name="logger">The logger instance for logging information.</param>
/// <param name="playerStateService">The service for managing player states and connections.</param>
public class SendGiftHandler(SendGiftRequest request, ILogger logger, 
    IPlayerStateService playerStateService) : IHandler
{
    /// <inheritdoc/>
    public async Task<string> HandleAsync()
    {
        logger.Information($"Handling gift request from Player {request.PlayerId} to Player {request.FriendPlayerId}.");

        SendGift(request.PlayerId, request.FriendPlayerId, request.ResourceType, request.Amount);
        return await Task.FromResult(string.Empty);
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
        var sender = playerStateService.GetPlayerById(senderPlayerId);
        var recipient = playerStateService.GetPlayerById(recipientPlayerId);

        if (sender == null || recipient == null)
        {
            logger.Error("Sender or recipient not found.");
            throw new Exception("Sender or recipient not found.");
        }

        if ((resourceType == ResourceType.Coins && sender.Coins < resourceValue) ||
            (resourceType == ResourceType.Rolls && sender.Rolls < resourceValue))
        {
            logger.Error("Insufficient resources.");
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

        logger.Information(
            $"Gift sent from Player {senderPlayerId} to Player {recipientPlayerId}. Resource: {resourceType}, Value: {resourceValue}.");

        if (playerStateService.IsPlayerOnline(recipientPlayerId))
        {
            var notification = new SendGiftResponse
            {
                PlayerId = senderPlayerId,
                FriendPlayerId = recipientPlayerId,
                Amount = resourceValue,
                ResourceType = resourceType
            };

            var jsonNotification = JsonSerializer.Serialize(notification);
            playerStateService.SendNotificationToPlayer(recipientPlayerId, jsonNotification);
            logger.Information($"Notification sent to Player {recipientPlayerId}: {jsonNotification}");
        }
    }
}