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
        await clientWebSocket.ConnectAsync(new Uri("ws://127.0.0.1:8000/"), CancellationToken.None);

        Console.WriteLine("Connected to the server. Start sending messages...");

        var loginRequest = new LoginRequest
        {
            ActionType = ActionType.Login,
            DeviceId = Guid.NewGuid()
        };

        await SendMessageAsync(clientWebSocket, loginRequest);

        var loginResponse = await ReceiveMessageAsync<LoginResponse>(clientWebSocket);
        var playerId = loginResponse.PlayerId;

        loginRequest = new LoginRequest
        {
            ActionType = ActionType.Login,
            DeviceId = Guid.NewGuid()
        };

        await SendMessageAsync(clientWebSocket, loginRequest);

        var friendLoginResponse = await ReceiveMessageAsync<LoginResponse>(clientWebSocket);
        var friendPlayerId = friendLoginResponse.PlayerId;

        var updateRequest = new UpdateResourceRequest
        {
            ActionType = ActionType.UpdateResources,
            PlayerId = playerId,
            ResourceType = ResourceType.Coins,
            Amount = 100
        };

        await SendMessageAsync(clientWebSocket, updateRequest);
        
        var updateResourceResponse = await ReceiveMessageAsync<UpdateResourceResponse>(clientWebSocket);

        var sendGiftRequest = new SendGiftRequest
        {
            FriendPlayerId = friendPlayerId,
            ActionType = ActionType.SendGift,
            PlayerId = playerId,
            ResourceType = ResourceType.Coins,
            Amount = 100
        };

        await SendMessageAsync(clientWebSocket, sendGiftRequest);
        
        var sendGiftResponse = await ReceiveMessageAsync<SendGiftResponse>(clientWebSocket);
    }

    private static async Task SendMessageAsync(ClientWebSocket clientWebSocket, object message)
    {
        var buffer = ObjectToBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private static async Task<T> ReceiveMessageAsync<T>(ClientWebSocket clientWebSocket)
    {
        var receiveBuffer = new byte[1024];
        var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        var receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
        Console.WriteLine($"Received message from server: {receivedMessage}");
        return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(receiveBuffer, 0, result.Count))
               ?? throw new NotSupportedException("Failed to deserialize message.");
    }

    private static byte[] ObjectToBytes(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return Encoding.UTF8.GetBytes(json);
    }
}