using System.Text.Json;
using GameServer.Requests;
using GameServer.Responses;

namespace GameServer.Handlers;

/// <summary>
/// Handles the update resource request.
/// </summary>
/// <param name="request">The update resource request.</param>
/// <param name="gameContext">The game context containing shared resources like logger and player state service.</param>
public class UpdateResourceHandler(UpdateResourceRequest request,
    IGameContext gameContext) : IHandler
{
    public Task<string> HandleAsync()
    {
        var resourceType = request.ResourceType.ToString();
        var resourceValue = request.Amount;

        var newBalance = gameContext.PlayerStateService.
            UpdateResources(request.PlayerId, resourceType, resourceValue);

        var response = new UpdateResourceResponse()
        {
            NewBalance = newBalance
        };
        
        var json = JsonSerializer.Serialize(response);
        
        return Task.FromResult(json);
    }
}