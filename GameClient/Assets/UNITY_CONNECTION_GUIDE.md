# Unity Game Client - Quick Setup Guide

## Prerequisites
1. **Python Server**: The GameServer must be running first
2. **Newtonsoft.Json**: Added to Unity packages (should auto-install)

## Quick Start (3 Steps)

### Step 1: Start the Python Server
```bash
cd GameServer
python server.py
```
OR double-click `start_server.bat`

### Step 2: Set Up Unity Scene
1. Open Unity and load any scene (SampleScene works fine)
2. Create an empty GameObject in the scene
3. Name it "GameManager"
4. Add the `GameManager` component to it
5. The component will automatically create and configure the server client

### Step 3: Test the Connection
1. Press Play in Unity
2. Watch the Console - you should see:
   ```
   GameManager initialized. Server client ready to connect.
   Connecting to game server at ws://localhost:8080/ws...
   Successfully connected to game server!
   [WELCOME] Connected to game server - Current tick: X
   [GAME TICK] Tick: X, Clients: 1, Timestamp: ...
   ```

## What You'll See

**Every 0.4 seconds**, you'll get a game tick log like:
```
[GAME TICK] Tick: 125, Clients: 1, Timestamp: 1674567890.523
```

This proves your Unity client is successfully receiving real-time updates from the Python server!

## Troubleshooting

**"Connection failed"**:
- Make sure Python server is running first
- Check if port 8080 is available
- Try the HTML test client (`test_client.html`) to verify server works

**"Could not load Newtonsoft.Json"**:
- Unity should auto-install this package
- If not, go to Window > Package Manager and search for "Newtonsoft Json"

**No tick messages**:
- Check Unity Console for connection logs
- Verify the server URL is `ws://localhost:8080/ws`
- Look at the GameServerClient component in inspector for status

## Testing Features

Once connected, you can:
- Send test messages to the server
- Monitor connection status in real-time
- See how many clients are connected
- View current tick count

Select the GameManager in the hierarchy to see inspector controls for testing.

## Next Steps

Now that the basic connection works, you can:
1. Send player movement data to the server
2. Receive other players' positions
3. Handle game events and state synchronization
4. Add authentication and player management

The foundation is ready - your Unity client can now communicate with the Python game server in real-time!
