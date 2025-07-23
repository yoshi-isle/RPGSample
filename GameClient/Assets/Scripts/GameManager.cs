using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Server Connection")]
    public GameServerClient serverClient;
    
    [Header("UI References (Optional - Connect via code or inspector)")]
    
    [Header("Auto Setup")]
    public bool createServerClientAutomatically = true;
    
    private void Start()
    {
        // Auto-create server client if not assigned
        if (serverClient == null && createServerClientAutomatically)
        {
            GameObject serverClientObj = new GameObject("GameServerClient");
            serverClient = serverClientObj.AddComponent<GameServerClient>();
            
            // Set default server URL (you can change this in the inspector)
            serverClient.serverUrl = "ws://localhost:8080/ws";
            serverClient.autoConnect = true;
        }
        
        // Subscribe to server events
        if (serverClient != null)
        {
            serverClient.OnConnected += OnServerConnected;
            serverClient.OnDisconnected += OnServerDisconnected;
            serverClient.OnMessageReceived += OnServerMessageReceived;
        }
        
        // Setup UI buttons if they exist
        SetupUI();
        
        // Update initial UI state
        UpdateUI();
        
        Debug.Log("GameManager initialized. Server client ready to connect.");
    }
    
    private void SetupUI()
    {
        Debug.Log("[GameManager] UI setup skipped - add UI components manually once UnityEngine.UI is available");
    }
    
    private void OnServerConnected()
    {
        Debug.Log("[GameManager] Connected to game server!");
        UpdateUI();
    }
    
    private void OnServerDisconnected()
    {
        Debug.Log("[GameManager] Disconnected from game server!");
        UpdateUI();
    }
    
    private void OnServerMessageReceived(ServerMessage message)
    {
        // Handle different message types
        switch (message.type)
        {
            case "tick":
                string locationInfo = "";
                if (message.unit_location != null)
                {
                    locationInfo = $" - Unit Location: ({message.unit_location.x:F2}, {message.unit_location.y:F2})";
                    if (message.position_updated)
                    {
                        locationInfo += " [UPDATED]";
                    }
                }
                Debug.Log($"[GameManager] Game Tick Received: {message.tick} (Clients: {message.clients_count}){locationInfo}");
                break;
                
            case "welcome":
                Debug.Log($"[GameManager] Welcome Message: {message.message}");
                break;
                
            case "echo":
                Debug.Log($"[GameManager] Server Echo: Received at tick {message.tick}");
                break;
                
            default:
                Debug.Log($"[GameManager] Unknown message type: {message.type}");
                break;
        }
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (serverClient == null) return;
    }
    
    private void Update()
    {
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        if (serverClient != null)
        {
            serverClient.OnConnected -= OnServerConnected;
            serverClient.OnDisconnected -= OnServerDisconnected;
            serverClient.OnMessageReceived -= OnServerMessageReceived;
        }
    }
    
    public void ConnectToServer()
    {
        serverClient?.Connect();
    }
    
    public void DisconnectFromServer()
    {
        serverClient?.Disconnect();
    }
    
    public void SendTestMessage()
    {
        serverClient?.SendTestMessage();
    }
}