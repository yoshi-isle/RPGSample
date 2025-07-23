using System.Collections;
using UnityEngine;

/// <summary>
/// Script that makes a GameObject respond to random unit locations from the game server
/// and smoothly transforms to those positions via WebSocket connection.
/// </summary>
public class EnemyLocationSync : MonoBehaviour
{
    [Header("Server Connection")]
    [Tooltip("Reference to the GameServerClient component")]
    public GameServerClient serverClient;
    
    [Header("Movement Settings")]
    [Tooltip("Speed of movement interpolation")]
    public float movementSpeed = 5f;
    
    [Tooltip("Scale factor for server coordinates (server uses -10 to 10, you might want different scale)")]
    public float positionScale = 1f;
    
    [Tooltip("Height (Y position) to maintain while moving")]
    public float fixedHeight = 0f;
    
    [Header("Debug")]
    [Tooltip("Show debug logs for position updates")]
    public bool showDebugLogs = true;
    
    [Tooltip("Show gizmos for target position")]
    public bool showGizmos = true;
    
    // Private variables
    private Vector3 currentTargetPosition;
    private Vector3 lastServerPosition;
    private bool isMoving = false;
    private bool hasReceivedFirstPosition = false;
    
    private void Start()
    {
        // Try to find GameServerClient if not assigned
        if (serverClient == null)
        {
            serverClient = FindObjectOfType<GameServerClient>();
            if (serverClient == null)
            {
                Debug.LogError("[EnemyLocationSync] No GameServerClient found! Please assign one in the inspector or ensure one exists in the scene.");
                return;
            }
        }
        
        // Subscribe to server messages
        serverClient.OnMessageReceived += OnServerMessageReceived;
        serverClient.OnConnected += OnServerConnected;
        serverClient.OnDisconnected += OnServerDisconnected;
        
        // Set initial target position to current position
        currentTargetPosition = transform.position;
        
        if (showDebugLogs)
        {
            Debug.Log($"[EnemyLocationSync] {gameObject.name} initialized and listening for unit location updates");
        }
    }
    
    private void OnServerConnected()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[EnemyLocationSync] {gameObject.name} - Server connected, ready to receive position updates");
        }
    }
    
    private void OnServerDisconnected()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[EnemyLocationSync] {gameObject.name} - Server disconnected");
        }
    }
    
    private void OnServerMessageReceived(ServerMessage message)
    {
        // Only process tick messages that contain unit_location data
        if (message.type == "tick" && message.unit_location != null)
        {
            // Convert server coordinates to Unity world position
            Vector3 serverPosition = new Vector3(
                message.unit_location.x * positionScale,
                fixedHeight,
                message.unit_location.y * positionScale  // Server Y becomes Unity Z
            );
            
            // Only update if position actually changed
            if (Vector3.Distance(serverPosition, lastServerPosition) > 0.01f)
            {
                lastServerPosition = serverPosition;
                currentTargetPosition = serverPosition;
                isMoving = true;
                hasReceivedFirstPosition = true;
                
                if (showDebugLogs)
                {
                    Debug.Log($"[EnemyLocationSync] {gameObject.name} - New target position: {currentTargetPosition} (Server: {message.unit_location.x:F2}, {message.unit_location.y:F2}) [Tick: {message.tick}]");
                }
            }
        }
    }
    
    private void Update()
    {
        // Only start moving once we've received the first position update
        if (!hasReceivedFirstPosition)
            return;
            
        // Smoothly move towards the target position
        if (isMoving)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = Vector3.MoveTowards(currentPosition, currentTargetPosition, movementSpeed * Time.deltaTime);
            
            transform.position = newPosition;
            
            // Check if we've reached the target
            if (Vector3.Distance(newPosition, currentTargetPosition) < 0.01f)
            {
                isMoving = false;
                if (showDebugLogs)
                {
                    Debug.Log($"[EnemyLocationSync] {gameObject.name} - Reached target position: {currentTargetPosition}");
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (serverClient != null)
        {
            serverClient.OnMessageReceived -= OnServerMessageReceived;
            serverClient.OnConnected -= OnServerConnected;
            serverClient.OnDisconnected -= OnServerDisconnected;
        }
    }
    
    // Gizmos for debugging
    private void OnDrawGizmos()
    {
        if (!showGizmos || !hasReceivedFirstPosition)
            return;
            
        // Draw target position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentTargetPosition, 0.3f);
        
        // Draw line from current position to target
        if (isMoving)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTargetPosition);
        }
        
        // Draw current position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
    
    // Public methods for external control
    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
    
    public void SetPositionScale(float scale)
    {
        positionScale = scale;
    }
    
    public void SetFixedHeight(float height)
    {
        fixedHeight = height;
        // Update current target to new height
        currentTargetPosition = new Vector3(currentTargetPosition.x, height, currentTargetPosition.z);
    }
    
    public Vector3 GetCurrentTarget()
    {
        return currentTargetPosition;
    }
    
    public bool IsMoving()
    {
        return isMoving;
    }
    
    public bool HasReceivedFirstPosition()
    {
        return hasReceivedFirstPosition;
    }
}
