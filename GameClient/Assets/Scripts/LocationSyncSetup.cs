using UnityEngine;

/// <summary>
/// Helper script to quickly set up objects that respond to server unit location updates.
/// Attach this to any GameObject to make it follow the random unit positions from the server.
/// </summary>
public class LocationSyncSetup : MonoBehaviour
{
    [Header("Setup Options")]
    [Tooltip("Automatically add EnemyLocationSync component on Start")]
    public bool autoSetup = true;
    
    [Tooltip("Movement speed for the location sync")]
    public float movementSpeed = 5f;
    
    [Tooltip("Position scale factor")]
    public float positionScale = 1f;
    
    [Tooltip("Fixed height to maintain")]
    public float fixedHeight = 0f;
    
    [Header("Visual Options")]
    [Tooltip("Add a simple visual indicator (cube) if the object doesn't have a renderer")]
    public bool addVisualIfNeeded = true;
    
    [Tooltip("Color for the visual indicator")]
    public Color visualColor = Color.red;
    
    private void Start()
    {
        if (autoSetup)
        {
            SetupLocationSync();
        }
    }
    
    public void SetupLocationSync()
    {
        // Add EnemyLocationSync component if it doesn't exist
        EnemyLocationSync locationSync = GetComponent<EnemyLocationSync>();
        if (locationSync == null)
        {
            locationSync = gameObject.AddComponent<EnemyLocationSync>();
            Debug.Log($"[LocationSyncSetup] Added EnemyLocationSync to {gameObject.name}");
        }
        
        // Configure the location sync component
        locationSync.movementSpeed = movementSpeed;
        locationSync.positionScale = positionScale;
        locationSync.fixedHeight = fixedHeight;
        
        // Try to find GameServerClient
        GameServerClient serverClient = FindObjectOfType<GameServerClient>();
        if (serverClient != null)
        {
            locationSync.serverClient = serverClient;
            Debug.Log($"[LocationSyncSetup] Connected {gameObject.name} to GameServerClient");
        }
        else
        {
            Debug.LogWarning($"[LocationSyncSetup] No GameServerClient found in scene. Please ensure one exists for {gameObject.name} to work properly.");
        }
        
        // Add visual indicator if needed
        if (addVisualIfNeeded && GetComponent<Renderer>() == null)
        {
            AddVisualIndicator();
        }
        
        Debug.Log($"[LocationSyncSetup] {gameObject.name} is now set up to follow server unit locations!");
    }
    
    private void AddVisualIndicator()
    {
        // Add a cube mesh renderer
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        // Set cube mesh
        meshFilter.mesh = GetCubeMesh();
        
        // Create and assign material
        Material material = new Material(Shader.Find("Standard"));
        material.color = visualColor;
        meshRenderer.material = material;
        
        // Scale down the cube a bit
        transform.localScale = Vector3.one * 0.5f;
        
        Debug.Log($"[LocationSyncSetup] Added visual indicator to {gameObject.name}");
    }
    
    private Mesh GetCubeMesh()
    {
        // Try to get the built-in cube mesh
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = primitive.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(primitive);
        return cubeMesh;
    }
    
    // Public method to manually trigger setup
    [ContextMenu("Setup Location Sync")]
    public void ManualSetup()
    {
        SetupLocationSync();
    }
    
    // Method to create a new tracking object at runtime
    public static GameObject CreateTrackingObject(string name = "LocationTracker")
    {
        GameObject trackingObj = new GameObject(name);
        LocationSyncSetup setup = trackingObj.AddComponent<LocationSyncSetup>();
        setup.SetupLocationSync();
        return trackingObj;
    }
}
