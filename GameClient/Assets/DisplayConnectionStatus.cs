using UnityEngine;
using TMPro;

public class DisplayConnectionStatus : MonoBehaviour
{
    public GameServerClient serverClient;
    public TextMeshProUGUI statusText;

    void Start()
    {
        if (statusText == null)
        {
            statusText = GetComponent<TextMeshProUGUI>();
            Debug.LogError("[DisplayConnectionStatus] Status Text is not assigned! Please assign a TextMeshProUGUI component in the inspector.");
            return;
        }

         // Try to find GameServerClient if not assigned
        if (serverClient == null)
        {
            serverClient = FindObjectOfType<GameServerClient>();
            if (serverClient == null)
            {
                Debug.LogError("[DisplayConnectionStatus] No GameServerClient found! Please assign one in the inspector or ensure one exists in the scene.");
                return;
            }
        }

        serverClient.OnConnected += OnServerConnected;
        serverClient.OnDisconnected += OnServerDisconnected;
    }

    private void OnServerConnected()
    {
        statusText.text = "Connected to server";
    }
    
    private void OnServerDisconnected()
    {
        statusText.text = "Disconnected from server";
    }
}
