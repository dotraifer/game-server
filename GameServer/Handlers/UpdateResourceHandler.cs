using System.Text.Json;
using GameServer.Requests;
using GameServer.Responses;
using Serilog;

namespace GameServer.Handlers;

/// <summary>
/// Handles the update resource request.
/// </summary>
/// <param name="request">The update resource request.</param>
/// <param name="logger">The logger instance for logging information.</param>
/// <param name="playerStateService">The service for managing player states and connections.</param>
public class UpdateResourceHandler(UpdateResourceRequest request,
    ILogger logger, IPlayerStateService playerStateService) : IHandler
{
    /// <inheritdoc/>
    public Task<string> HandleAsync()
    {
        logger.Information($"Handling resource update request for Player {request.PlayerId}.");
        var resourceType = request.ResourceType.ToString();
        var resourceValue = request.Amount;

        var newBalance = playerStateService.
            UpdateResources(request.PlayerId, resourceType, resourceValue);

        var response = new UpdateResourceResponse
        {
            NewBalance = newBalance
        };
        
        var json = JsonSerializer.Serialize(response);
        logger.Information($"Resource update request for Player {request.PlayerId} handled successfully.");
        
        return Task.FromResult(json);
    }
}