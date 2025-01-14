namespace GameServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Facade.Start();
    }
}