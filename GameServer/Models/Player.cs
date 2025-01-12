namespace GameServer.Models;

public class Player
{
    public string PlayerId { get; set; }
    public string DeviceId { get; set; }
    public int Coins { get; set; }
    public int Rolls { get; set; }
}