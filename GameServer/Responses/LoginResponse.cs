namespace GameServer.Responses;

public record LoginResponse
{
    public required string PlayerId { get; set; }
}