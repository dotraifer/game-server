namespace GameServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var facade = new Facade();
        await facade.Start();
    }
}