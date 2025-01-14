﻿using Autofac;
using Serilog;
using Serilog.Events;

namespace GameServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new ContainerBuilder();
        
        var playerStateService = new PlayerStateService();
        ILogger logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/GameServer.logs",
                rollingInterval: RollingInterval.Day)
            .MinimumLevel.Is(LogEventLevel.Information)
            .CreateLogger();
        
        builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
        builder.RegisterType<global::GameServer.GameServer>().AsSelf();
        builder.RegisterInstance(playerStateService).
            As<IPlayerStateService>().SingleInstance();
        builder.RegisterInstance(new GameContext(logger, playerStateService)).AsSelf().SingleInstance();
        var container = builder.Build();
        await using var scope = container.BeginLifetimeScope();
        await container.Resolve<global::GameServer.GameServer>().StartServer("127.0.0.1", 8000);
    }
}