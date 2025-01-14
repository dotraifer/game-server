using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GameServer.Models;
using GameServer.Requests;
using GameServer.Responses;
using Serilog;

namespace GameClient;

public static class Program
{
    private const string ServerUrl = "ws://127.0.0.1:8000/";
    public static async Task Main(string[] args)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/GameServer.logs", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Information()
            .CreateLogger();
        
        var clientWebSocket = new ClientWebSocket();
        logger.Information("Attempting to connect to the server at {ServerUrl}...", ServerUrl);
        // Connect to the server
        await clientWebSocket.ConnectAsync(new Uri(ServerUrl), CancellationToken.None);

        logger.Information("Connected to the server. Start sending messages on {ServerUrl}...", ServerUrl);
        var loginRequest = new LoginRequest
        {
            ActionType = ActionType.Login,
            DeviceId = Guid.NewGuid()
        };
        
        // Send login request
        await SendMessageAsync(clientWebSocket, loginRequest);
        logger.Information("Login request sent: {LoginRequest}", loginRequest);
        // Receive login response
        var loginResponse = await ReceiveMessageAsync<LoginResponse>(clientWebSocket, logger);
        logger.Information("Login response received: {LoginResponse}", loginResponse);
        var playerId = loginResponse.PlayerId;

        var friendLoginRequest = new LoginRequest
        {
            ActionType = ActionType.Login,
            DeviceId = Guid.NewGuid()
        };

        // Send friend login request
        logger.Information("Login request sent: {LoginRequest}", loginRequest);
        await SendMessageAsync(clientWebSocket, friendLoginRequest);
        // Receive friend login response
        var friendLoginResponse = await ReceiveMessageAsync<LoginResponse>(clientWebSocket, logger);
        logger.Information("Login response received: {LoginResponse}", loginResponse);
        var friendPlayerId = friendLoginResponse.PlayerId;

        var updateRequest = new UpdateResourceRequest
        {
            ActionType = ActionType.UpdateResources,
            PlayerId = playerId,
            ResourceType = ResourceType.Coins,
            Amount = 100
        };

        // Send update resource request
        await SendMessageAsync(clientWebSocket, updateRequest);
        logger.Information("Update resource request sent: {UpdateRequest}", updateRequest);
        // Receive update resource response
        await ReceiveMessageAsync<UpdateResourceResponse>(clientWebSocket, logger);
        logger.Information("Update resource response received.");

        var sendGiftRequest = new SendGiftRequest
        {
            FriendPlayerId = friendPlayerId,
            ActionType = ActionType.SendGift,
            PlayerId = playerId,
            ResourceType = ResourceType.Coins,
            Amount = 100
        };
        
        // Send gift request
        await SendMessageAsync(clientWebSocket, sendGiftRequest);
        logger.Information("Send gift request sent: {SendGiftRequest}", sendGiftRequest);
        // Receive gift response
        await ReceiveMessageAsync<SendGiftResponse>(clientWebSocket, logger);
        logger.Information("Send gift response received.");
        // Close the connection
        await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        logger.Information("Connection closed.");
    }
    
    /// <summary>
    /// Sends a message asynchronously to the server.
    /// </summary>
    /// <param name="clientWebSocket">The client WebSocket instance.</param>
    /// <param name="message">The message object to be sent.</param>
    private static async Task SendMessageAsync(ClientWebSocket clientWebSocket, object message)
    {
        var buffer = ObjectToBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    /// <summary>
    /// Receives a message asynchronously from the server.
    /// </summary>
    /// <typeparam name="T">The type of the message to be received.</typeparam>
    /// <param name="clientWebSocket">The client WebSocket instance.</param>
    /// <param name="logger"></param>
    /// <returns>The received message deserialized to the specified type.</returns>
    /// <exception cref="NotSupportedException">Thrown when the message cannot be deserialized.</exception>
    private static async Task<T> ReceiveMessageAsync<T>(ClientWebSocket clientWebSocket, ILogger logger)
    {
        var receiveBuffer = new byte[1024];
        var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        var receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
        logger.Information("Received message from server: {ReceivedMessage}", receivedMessage);
        return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(receiveBuffer, 0, result.Count))
               ?? throw new NotSupportedException("Failed to deserialize message.");
    }

    /// <summary>
    /// Converts an object to a byte array.
    /// </summary>
    /// <param name="obj">The object to be converted.</param>
    /// <returns>The byte array representation of the object.</returns>
    private static byte[] ObjectToBytes(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return Encoding.UTF8.GetBytes(json);
    }
}