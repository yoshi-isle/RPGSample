# Unity GameClient - WebSocket Connection Setup

This Unity project connects to the Python GameServer via WebSocket to receive game ticks.

## Setup Instructions

### 1. Package Dependencies
The project requires Newtonsoft.Json package which has been added to the manifest.json. Unity should automatically resolve this dependency when you open the project.

### 2. Scene Setup
To connect to the game server:

1. Open any scene (e.g., SampleScene)
2. Create an empty GameObject and name it "GameManager"
3. Add the `GameManager` component to this GameObject
4. The GameManager will automatically create and configure the `GameServerClient`

### 3. Manual Setup (Alternative)
If you prefer manual setup:

1. Create an empty GameObject named "GameServerClient"
2. Add the `GameServerClient` component to it
3. Set the Server URL to: `ws://localhost:8080/ws`
4. Check "Auto Connect" if you want automatic connection on start

### 4. Running the Connection

1. **Start the Python Server first:**
   - Navigate to the GameServer folder
   - Run: `python server.py`
   - Server will start on `http://localhost:8080`

2. **Run Unity:**
   - Press Play in Unity
   - Check the Console for connection logs
   - You should see game tick messages every 0.4 seconds

### 5. Expected Console Output

When successfully connected, you'll see logs like:
```
[GameManager] Connected to game server!
[WELCOME] Connected to game server - Current tick: 123
[GAME TICK] Tick: 124, Clients: 1, Timestamp: 1674567890.123
[GAME TICK] Tick: 125, Clients: 1, Timestamp: 1674567890.523
```

### 6. Testing Features

The GameManager provides several testing methods:
- **Connect/Disconnect**: Control connection manually
- **Send Test Message**: Send a test message to the server
- **View Status**: Monitor connection status and tick count

### 7. Inspector Controls

When the GameManager is selected in the hierarchy, you can:
- View real-time connection status
- See current tick count and connected clients
- Use buttons to connect/disconnect/send test messages (during Play mode)

### 8. Troubleshooting

**Connection Failed:**
- Ensure Python server is running on port 8080
- Check Windows Firewall settings
- Verify the server URL in GameServerClient component

**No Tick Messages:**
- Check if server is broadcasting (Python console should show tick logs)
- Verify WebSocket connection is established
- Look for error messages in Unity Console

**JSON Errors:**
- Ensure Newtonsoft.Json package is properly installed
- Check Package Manager for any missing dependencies

## Script Overview

### GameServerClient.cs
- Handles WebSocket connection to Python server
- Receives and processes server messages
- Manages connection lifecycle
- Provides events for other scripts to subscribe to

### GameManager.cs
- Demonstrates usage of GameServerClient
- Provides UI integration points
- Handles server events and logging
- Offers inspector controls for testing

### UnityMainThreadDispatcher.cs
- Utility class for executing code on Unity's main thread
- Required for WebSocket callbacks that update Unity objects
- Automatically created as singleton

## Server Message Types

The client handles these message types from the server:

1. **welcome**: Initial connection confirmation
2. **tick**: Regular game tick updates (every 0.4 seconds)
3. **echo**: Server response to client messages

## Extending the System

To add new functionality:

1. Subscribe to `GameServerClient.OnMessageReceived` event
2. Handle specific message types in your scripts
3. Use `GameServerClient.SendMessage()` to send data to server
4. Add new message types to the `ServerMessage` class as needed
