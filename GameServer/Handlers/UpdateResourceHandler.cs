using System.Text.Json;
using GameServer.Requests;
using GameServer.Responses;

namespace GameServer.Handlers;

public class UpdateResourceHandler(UpdateResourceRequest request,
    GameContext gameContext) : IHandler
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