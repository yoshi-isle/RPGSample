using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Server Connection")]
    public GameServerClient serverClient;
    
    [Header("UI References (Optional - Connect via code or inspector)")]
    // Note: UI components removed to avoid UnityEngine.UI dependency issues
    // You can add these back once the UI module is properly imported
    // public Text statusText;
    // public Text tickText;
    // public Text clientCountText;
    // public Button connectButton;
    // public Button disconnectButton;
    // public Button sendTestButton;
    
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
        
        // UI updates disabled until UnityEngine.UI is properly imported
        // Uncomment these lines once you have UI components properly set up:
        
        // // Update status text
        // if (statusText != null)
        // {
        //     statusText.text = serverClient.isConnected ? "Connected" : "Disconnected";
        //     statusText.color = serverClient.isConnected ? Color.green : Color.red;
        // }
        // 
        // // Update tick text
        // if (tickText != null)
        // {
        //     tickText.text = $"Tick: {serverClient.currentTick}";
        // }
        // 
        // // Update client count text
        // if (clientCountText != null)
        // {
        //     clientCountText.text = $"Clients: {serverClient.connectedClients}";
        // }
        // 
        // // Update button states
        // if (connectButton != null)
        //     connectButton.interactable = !serverClient.isConnected;
        //     
        // if (disconnectButton != null)
        //     disconnectButton.interactable = serverClient.isConnected;
        //     
        // if (sendTestButton != null)
        //     sendTestButton.interactable = serverClient.isConnected;
    }
    
    private void Update()
    {
        // Update UI every frame (you might want to optimize this)
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (serverClient != null)
        {
            serverClient.OnConnected -= OnServerConnected;
            serverClient.OnDisconnected -= OnServerDisconnected;
            serverClient.OnMessageReceived -= OnServerMessageReceived;
        }
    }
    
    // Public methods for testing or external use
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