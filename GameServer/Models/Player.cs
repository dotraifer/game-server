namespace GameServer.Models;

public class Player
{
    public required string PlayerId { get; set; }
    public required string DeviceId { get; set; }
    public int Coins { get; set; } = 0;
    public int Rolls { get; set; } = 0;
}