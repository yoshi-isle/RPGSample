using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Server Connection")]
    public GameServerClient serverClient;
    
    [Header("UI References (Optional)")]
    public Text statusText;
    public Text tickText;
    public Text clientCountText;
    public Button connectButton;
    public Button disconnectButton;
    public Button sendTestButton;
    
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
        if (connectButton != null)
            connectButton.onClick.AddListener(() => serverClient?.Connect());
            
        if (disconnectButton != null)
            disconnectButton.onClick.AddListener(() => serverClient?.Disconnect());
            
        if (sendTestButton != null)
            sendTestButton.onClick.AddListener(() => serverClient?.SendTestMessage());
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
                Debug.Log($"[GameManager] Game Tick Received: {message.tick} (Clients: {message.clients_count})");
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
        
        // Update status text
        if (statusText != null)
        {
            statusText.text = serverClient.isConnected ? "Connected" : "Disconnected";
            statusText.color = serverClient.isConnected ? Color.green : Color.red;
        }
        
        // Update tick text
        if (tickText != null)
        {
            tickText.text = $"Tick: {serverClient.currentTick}";
        }
        
        // Update client count text
        if (clientCountText != null)
        {
            clientCountText.text = $"Clients: {serverClient.connectedClients}";
        }
        
        // Update button states
        if (connectButton != null)
            connectButton.interactable = !serverClient.isConnected;
            
        if (disconnectButton != null)
            disconnectButton.interactable = serverClient.isConnected;
            
        if (sendTestButton != null)
            sendTestButton.interactable = serverClient.isConnected;
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