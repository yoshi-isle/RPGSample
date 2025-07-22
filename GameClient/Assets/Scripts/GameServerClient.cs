using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class ServerMessage
{
    public string type;
    public string message;
    public int tick;
    public float timestamp;
    public int clients_count;
    public object original;
}

public class GameServerClient : MonoBehaviour
{
    [Header("Server Settings")]
    public string serverUrl = "ws://localhost:8080/ws";
    public bool autoConnect = true;
    
    [Header("Status")]
    public bool isConnected = false;
    public int currentTick = 0;
    public int connectedClients = 0;
    
    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    private bool isConnecting = false;
    
    // Events for other scripts to subscribe to
    public event Action<ServerMessage> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;
    
    private async void Start()
    {
        if (autoConnect)
        {
            await ConnectToServer();
        }
    }
    
    public async Task ConnectToServer()
    {
        if (isConnecting || isConnected)
        {
            Debug.LogWarning("Already connected or connecting to server");
            return;
        }
        
        isConnecting = true;
        
        try
        {
            webSocket = new ClientWebSocket();
            cancellationTokenSource = new CancellationTokenSource();
            
            Debug.Log($"Connecting to game server at {serverUrl}...");
            
            Uri serverUri = new Uri(serverUrl);
            await webSocket.ConnectAsync(serverUri, cancellationTokenSource.Token);
            
            isConnected = true;
            isConnecting = false;
            
            Debug.Log("Successfully connected to game server!");
            OnConnected?.Invoke();
            
            // Start listening for messages
            _ = Task.Run(ListenForMessages);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to connect to server: {ex.Message}");
            isConnecting = false;
            isConnected = false;
        }
    }
    
    public async Task DisconnectFromServer()
    {
        if (!isConnected || webSocket == null)
            return;
            
        try
        {
            cancellationTokenSource?.Cancel();
            
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            }
            
            isConnected = false;
            Debug.Log("Disconnected from game server");
            OnDisconnected?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error disconnecting: {ex.Message}");
        }
        finally
        {
            webSocket?.Dispose();
            cancellationTokenSource?.Dispose();
        }
    }
    
    private async Task ListenForMessages()
    {
        var buffer = new byte[1024 * 4];
        
        try
        {
            while (webSocket.State == WebSocketState.Open && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    
                    // Process message on main thread
                    UnityMainThreadDispatcher.Instance.Enqueue(() => ProcessMessage(message));
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error receiving message: {ex.Message}");
        }
        finally
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() => {
                isConnected = false;
                OnDisconnected?.Invoke();
            });
        }
    }
    
    private void ProcessMessage(string messageJson)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<ServerMessage>(messageJson);
            
            // Update status based on message type
            if (message.type == "tick")
            {
                currentTick = message.tick;
                connectedClients = message.clients_count;
                Debug.Log($"[GAME TICK] Tick: {message.tick}, Clients: {message.clients_count}, Timestamp: {message.timestamp}");
            }
            else if (message.type == "welcome")
            {
                Debug.Log($"[WELCOME] {message.message} - Current tick: {message.tick}");
            }
            else if (message.type == "echo")
            {
                Debug.Log($"[ECHO] Server echoed message at tick {message.tick}");
            }
            
            // Notify other scripts
            OnMessageReceived?.Invoke(message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing message: {ex.Message}");
        }
    }
    
    public async Task SendMessage(object data)
    {
        if (!isConnected || webSocket?.State != WebSocketState.Open)
        {
            Debug.LogWarning("Cannot send message - not connected to server");
            return;
        }
        
        try
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
            Debug.Log($"Sent message to server: {json}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending message: {ex.Message}");
        }
    }
    
    private async void OnDestroy()
    {
        await DisconnectFromServer();
    }
    
    private async void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            await DisconnectFromServer();
        }
        else if (autoConnect)
        {
            await ConnectToServer();
        }
    }
    
    private async void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            await DisconnectFromServer();
        }
        else if (autoConnect)
        {
            await ConnectToServer();
        }
    }
    
    // Public methods for UI or other scripts
    public void Connect()
    {
        _ = ConnectToServer();
    }
    
    public void Disconnect()
    {
        _ = DisconnectFromServer();
    }
    
    public void SendTestMessage()
    {
        var testData = new { 
            type = "test", 
            message = "Hello from Unity!", 
            timestamp = Time.time 
        };
        _ = SendMessage(testData);
    }
}
