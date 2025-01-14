using System.Net;
using Autofac;
using Serilog;
using Serilog.Events;

namespace GameServer;

public static class Facade
{
    public static async Task Start()
    {
        var builder = new ContainerBuilder();
        
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/GameServer.logs", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Information()
            .CreateLogger();

        var httpListener = new HttpListener();
        
        var playerStateService = new PlayerStateService(logger);
        builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
        builder.RegisterInstance(httpListener).AsSelf().SingleInstance();
        builder.RegisterType<GameServer>().AsSelf().SingleInstance();
        builder.RegisterInstance(playerStateService).As<IPlayerStateService>().SingleInstance();
        
        var container = builder.Build();
        await using var scope = container.BeginLifetimeScope();
        await container.Resolve<GameServer>().StartServer("127.0.0.1", 8000);
    }
}