using Autofac;
using Serilog;
using Serilog.Events;

namespace GameServer;

public class Facade
{
    public async Task Start()
    {
        var builder = new ContainerBuilder();
        
        var playerStateService = new PlayerStateService();
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/GameServer.logs", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Information()
            .CreateLogger();
        
        builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
        builder.RegisterType<GameServer>().AsSelf();
        builder.RegisterInstance(playerStateService).As<IPlayerStateService>().SingleInstance();
        builder.RegisterInstance(new GameContext(logger, playerStateService)).As<IGameContext>().SingleInstance();
        
        var container = builder.Build();
        await using var scope = container.BeginLifetimeScope();
        await container.Resolve<GameServer>().StartServer("127.0.0.1", 8000);
    }
}