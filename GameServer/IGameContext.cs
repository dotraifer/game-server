using Serilog;

namespace GameServer;

/// <summary>
/// Represents the game context which holds the logger and player state service.
/// </summary>
public interface IGameContext
{
    /// <summary>
    /// Logger instance.
    /// </summary>
    public ILogger Logger { get; init; }

    /// <summary>
    /// Player state service instance.
    /// </summary>
    public PlayerStateService PlayerStateService { get; set; }
}