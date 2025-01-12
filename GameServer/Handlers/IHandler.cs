

using System.Net.WebSockets;

namespace GameServer.Handlers;

public interface IHandler
{
    Task<string> HandleAsync();
}