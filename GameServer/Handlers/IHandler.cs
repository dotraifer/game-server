namespace GameServer.Handlers;

public interface IHandler
{
    Task<string> HandleAsync();
}