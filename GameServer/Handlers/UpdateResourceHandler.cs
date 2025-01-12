using System.Net.WebSockets;

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
        return Task.FromResult($"{{\"NewBalance\":{newBalance}}}");
    }
}