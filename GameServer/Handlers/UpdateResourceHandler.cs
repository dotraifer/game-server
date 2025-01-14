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
    public Task<string> HandleAsync()
    {
        var resourceType = request.ResourceType.ToString();
        var resourceValue = request.Amount;

        var newBalance = playerStateService.
            UpdateResources(request.PlayerId, resourceType, resourceValue);

        var response = new UpdateResourceResponse
        {
            NewBalance = newBalance
        };
        
        var json = JsonSerializer.Serialize(response);
        
        return Task.FromResult(json);
    }
}