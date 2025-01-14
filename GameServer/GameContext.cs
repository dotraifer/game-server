using Serilog;

namespace GameServer;

/// <summary>
/// Represents the game context which holds the logger and player state service.
/// </summary>
public class GameContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameContext"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="playerStateService">The player state service instance.</param>
    public GameContext(ILogger logger, PlayerStateService playerStateService)
    {
        Logger = logger;
        PlayerStateService = playerStateService;
    }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    public ILogger Logger { get; init; }

    /// <summary>
    /// Gets or sets the player state service instance.
    /// </summary>
    public PlayerStateService PlayerStateService { get; set; }
}