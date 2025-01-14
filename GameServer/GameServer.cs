using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            }
        }
    }

    private async Task<string> ProcessMessageAsync(string message)
    {
        try
        {
            var handler = HandlerFactory.CreateObject(message, gameContext);
            return await handler.HandleAsync();
        }
        catch (Exception ex)
        {
            return $"Error processing message. {ex.Message}";
        }
    }
}