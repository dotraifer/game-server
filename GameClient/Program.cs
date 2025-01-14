using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GameServer.Models;
using GameServer.Requests;
using GameServer.Responses;

namespace GameClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var clientWebSocket = new ClientWebSocket();
        // Connect to the server
        await clientWebSocket.ConnectAsync(new Uri("ws://127.0.0.1:8000/"), CancellationToken.None);

        Console.WriteLine("Connected to the server. Start sending messages...");

        var loginRequest = new LoginRequest
        {
            ActionType = ActionType.Login,
            DeviceId = Guid.NewGuid()
        };
        
        // Send login request
        await SendMessageAsync(clientWebSocket, loginRequest);
        // Receive login response
        var loginResponse = await ReceiveMessageAsync<LoginResponse>(clientWebSocket);
        var playerId = loginResponse.PlayerId;

        var friendLoginRequest = new LoginRequest
        {
            ActionType = ActionType.Login,
            DeviceId = Guid.NewGuid()
        };

        // Send friend login request
        await SendMessageAsync(clientWebSocket, friendLoginRequest);
        // Receive friend login response
        var friendLoginResponse = await ReceiveMessageAsync<LoginResponse>(clientWebSocket);
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
        // Receive update resource response
        await ReceiveMessageAsync<UpdateResourceResponse>(clientWebSocket);

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
        // Receive gift response
        await ReceiveMessageAsync<SendGiftResponse>(clientWebSocket);
        // Close the connection
        await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
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
    /// <returns>The received message deserialized to the specified type.</returns>
    /// <exception cref="NotSupportedException">Thrown when the message cannot be deserialized.</exception>
    private static async Task<T> ReceiveMessageAsync<T>(ClientWebSocket clientWebSocket)
    {
        var receiveBuffer = new byte[1024];
        var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        var receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
        Console.WriteLine($"Received message from server: {receivedMessage}");
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