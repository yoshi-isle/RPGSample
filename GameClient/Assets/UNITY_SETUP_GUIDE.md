# Unity SignalR Setup Guide

## Step 1: Install Required Dependencies

### Option A: Using NuGetForUnity (Recommended)
1. Download NuGetForUnity from: https://github.com/GlitchEnzo/NuGetForUnity
2. Import the .unitypackage into your project
3. Open `Window > NuGet Package Manager`
4. Install these packages:
   - `Microsoft.AspNetCore.SignalR.Client` (latest version)
   - `System.Text.Json` (if not included with Unity)

### Option B: Manual Installation
1. Download the required DLL files:
   - Microsoft.AspNetCore.SignalR.Client.dll
   - Microsoft.AspNetCore.SignalR.Common.dll
   - Microsoft.AspNetCore.SignalR.Protocols.Json.dll
   - Microsoft.Extensions.DependencyInjection.Abstractions.dll
   - Microsoft.Extensions.Logging.Abstractions.dll
   - Microsoft.Extensions.Options.dll
   - System.Text.Json.dll (Unity 2021.2+ includes this)

2. Place all DLLs in `Assets/Plugins/` folder
3. For each DLL, set Platform settings to "Any Platform" in Inspector

## Step 2: Setup in Unity Scene

### Method 1: Using GameClientManager (Easy Setup)
1. In your Unity scene, create an empty GameObject
2. Name it "Game Client Manager"
3. Add the `GameClientManager` component to it
4. In the inspector, configure:
   - **Server URL**: Your SignalR server URL (e.g., "https://localhost:7154/gamehub")
   - **Player Name**: Choose a unique player name
   - **Log Game Ticks**: Check this to see tick logs
   - **Log Player Events**: Check this to see player join/leave events

### Method 2: Manual Setup
1. Create an empty GameObject named "SignalR Manager"
2. Add the `SignalRManager` component
3. Configure the same settings as above

## Step 3: Test the Connection

1. Make sure your RPG server is running (you can see it's working from your terminal)
2. Press Play in Unity
3. Check the Console for connection logs:
   ```
   [SignalR] Attempting to connect to https://localhost:7154/gamehub
   [SignalR] Connected successfully!
   [SignalR] Joined game as 'UnityPlayer'
   [GameTick] Tick #123 at 14:30:25.123 (ServerTime: 1234567890)
   ```

## Step 4: Troubleshooting

### Common Issues:

1. **"System.Text.Json not found"**
   - Unity 2021.2+: Should be included by default
   - Older Unity: Download System.Text.Json.dll and place in Assets/Plugins/

2. **Connection Failed**
   - Check server is running
   - Verify URL is correct
   - Check CORS settings on server
   - For HTTPS: Ensure SSL certificate is valid

3. **No Game Ticks Received**
   - Verify connection is successful
   - Check if you're in the "GamePlayers" group
   - Look for error messages in Console

### Expected Behavior:
- Connection establishes automatically when scene starts
- You should see game ticks every 400ms in the Console
- Player join/leave events will be logged
- Automatic reconnection if connection drops

## Step 5: Next Steps

Once you're receiving game ticks, you can:
- Subscribe to `SignalRManager.OnGameTick` event in your game scripts
- Use the tick data to synchronize game state
- Implement player movement, animations, etc.

Example subscription:
```csharp
void Start()
{
    SignalRManager.OnGameTick += HandleGameTick;
}

void HandleGameTick(GameTickData tickData)
{
    // Your game logic here
    Debug.Log($"Processing tick #{tickData.TickNumber}");
}
```
