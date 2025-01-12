using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class Program
{
    public static async Task Main(string[] args)
    {
        var clientWebSocket = new ClientWebSocket();
        await clientWebSocket.ConnectAsync(new Uri("""ws://127.0.0.1:8000/"""), CancellationToken.None);

        Console.WriteLine("Connected to the server. Start sending messages...");

        // Send messages to the server
        const string message = """{"type": "Login", "DeviceId": "d4054b23-12c3-4cde-983e-9e5fb5cb28a8"}""";
        var buffer = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

        // Receive messages from the server
        var receiveBuffer = new byte[1024];
        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                Console.WriteLine($"Received message from server: {receivedMessage}");
            }
        }
    }
}

