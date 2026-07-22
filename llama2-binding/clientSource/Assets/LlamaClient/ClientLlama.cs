using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public static class EventType 
{
    public const string 
        generate = "generate",
        compare = "compare";
}


[Serializable]
public class LLMMessage
{
    public string event_type;
}


[Serializable]
public class ChatMessage : LLMMessage
{
    public string text;
    public float temp = 0.8f;
    public int max_tokens = 50;
    public int top_k = 40;
    public float top_p = 0.4f;
    public float repeat_penalty = 1.18f;
    public int repeat_last_n = 64;
    public int n_batch = 8;
}


public class ClientLlama : MonoBehaviour
{
    private ClientWebSocket clientWebSocket;

    [SerializeField] 
    private string serverAddress = "ws://localhost:8765";

    public static ClientLlama singletonInstance;

    private bool threadLock = false;

    async void Start()
    {
        if (singletonInstance != null)
        {
            Destroy(this);
            return;
        }

        singletonInstance = this;

        clientWebSocket = new ClientWebSocket();
        await clientWebSocket.ConnectAsync(new Uri(serverAddress), CancellationToken.None);
        print("Connected to WebSocket server.");
    }


    private async void OnApplicationQuit()
    {
        if (clientWebSocket.State == WebSocketState.Open)
        {
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    public async Task<string> SendMessageToServer(string message, int maxTokens = 20, float temperature = 0.8f)
    {
        while(threadLock) {
            print("Waiting for thread lock");
            await Task.Delay(5000);
        }

        threadLock = true;
        ChatMessage chatMessage = new()
        {
            event_type = EventType.generate,
            text = message,
            max_tokens = maxTokens,
            temp = temperature
        };

        string jsonMessage = JsonUtility.ToJson(chatMessage);

        byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        print($"Message sent: {message}");

        byte[] buffer = new byte[1024];
        var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (receiveResult.MessageType == WebSocketMessageType.Text)
        {
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            print($"Message received: {receivedMessage}");
            threadLock = false;
            return receivedMessage;
        }
        threadLock = false;
        return "No response received";
    }


    [ContextMenu("Send Message")]
    public async void SendMessage()
    {
        var message = "the capital of France is";
        var response = await SendMessageToServer(message);
        print($"¡¡¡¡¡¡ Response: {response}");
    }
}
