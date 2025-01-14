namespace GameServer.Responses;

public record UpdateResourceResponse
{
    public required int NewBalance { get; set; }
}