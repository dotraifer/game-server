using System.Net;
using System.Net.WebSockets;
using System.Text;
using Serilog;

namespace GameServer;

/// <summary>
/// Represents the game server that handles incoming connections and processes WebSocket requests.
/// </summary>
public class GameServer(ILogger logger, IPlayerStateService playerStateService)
{
    /// <summary>
    /// Starts the game server and listens for incoming connections.
    /// </summary>
    /// <param name="ipAddress">The IP address to bind the server to.</param>
    /// <param name="port">The port to bind the server to.</param>
    public async Task StartServer(string ipAddress, int port)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://{ipAddress}:{port}/");
        listener.Start();

        logger.Information("Server started. Waiting for connections at {IpAddress}:{Port}",
            ipAddress, port);

        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                logger.Information("WebSocket request received.");
                await ProcessWebSocketRequest(context);
            }
            else
            {
                logger.Warning("Non-WebSocket request received. Responding with 400 Bad Request.");
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    /// <summary>
    /// Processes an incoming WebSocket request.
    /// </summary>
    /// <param name="context">The HTTP listener context containing the WebSocket request.</param>
    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        logger.Information("WebSocket connection established.");
        var webSocketContext = await context.AcceptWebSocketAsync(null);
        var socket = webSocketContext.WebSocket;
        var buffer = new byte[1024];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var processedMessage = await ProcessMessageAsync(receivedMessage);
                await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(processedMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                logger.Information("WebSocket connection closed.");
            }
        }
    }

    /// <summary>
    /// Processes a received message and returns a response.
    /// </summary>
    /// <param name="message">The received message to process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the processed message response.</returns>
    private async Task<string> ProcessMessageAsync(string message)
    {
        try
        {
            var handler = HandlerFactory.CreateObject(message, logger, playerStateService);
            return await handler.HandleAsync();
        }
        catch (Exception ex)
        {
            logger.Error("Error processing message.", ex);
            return $"Error processing message. {ex.Message}";
        }
    }
}