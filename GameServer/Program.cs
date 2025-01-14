namespace GameServer;

public static class Program
{
    public static void Main(string[] args)
    {
        var facade = new Facade();
        facade.Start();
    }
}