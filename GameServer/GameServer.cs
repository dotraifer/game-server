using System.Net;
using System.Net.WebSockets;

namespace GameServer;

public class GameServer(GameContext gameContext)
{
    public async Task StartServer(string ipAddress, int port)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://{ipAddress}:{port}/");
        listener.Start();

        gameContext.Logger.Information("Server started. Waiting for connections...");

        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                await ProcessWebSocketRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }
    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        var webSocketContext = await context.AcceptWebSocketAsync(null);
        var socket = webSocketContext.WebSocket;

        // Handle incoming messages
        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                var processedMessage = await ProcessMessageAsync(receivedMessage);

                // Echo back the received message
                await socket.SendAsync(
                    new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(processedMessage),
                    0, processedMessage.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
    }
    
    
    private async Task<string> ProcessMessageAsync(string message)
    {
        try
        {
            // Parse JSON message
            var handler = HandlerFactory.CreateObject(message, gameContext);
            var response  = await handler.HandleAsync();
            return response;
        }
        catch (Exception ex)
        {
            return $"Error processing message. {ex.Message}";
        }
    }
}