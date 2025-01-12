using Serilog;

namespace GameServer;

public class GameContext(ILogger logger, PlayerStateService playerStateService)
{
    public ILogger Logger { get; init; } = logger;

    public PlayerStateService PlayerStateService { get; set; } = playerStateService;
}