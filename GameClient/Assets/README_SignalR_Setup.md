# Unity SignalR Client Setup

## Required NuGet Packages

You need to install these NuGet packages for Unity:

1. **Microsoft.AspNetCore.SignalR.Client** (latest stable version)
2. **Newtonsoft.Json** (for Unity, if not already present)

## Installation Methods

### Method 1: Using NuGetForUnity (Recommended)
1. Install NuGetForUnity from the Unity Asset Store or GitHub
2. Open Window > NuGet Package Manager
3. Search for and install:
   - Microsoft.AspNetCore.SignalR.Client
   - Newtonsoft.Json (if not already installed)

### Method 2: Manual DLL Installation
1. Download the required DLLs from NuGet.org
2. Place them in Assets/Plugins/ folder
3. Configure platform settings for each DLL

### Method 3: Package Manager (Unity 2021.2+)
1. Open Window > Package Manager
2. Add package from git URL: com.unity.nuget.newtonsoft-json
3. For SignalR, you'll need to manually add the DLLs

## Server Configuration

Make sure your server URL in SignalRManager.cs matches your actual server:
- Default: "https://localhost:7154/gamehub"
- Update the `serverUrl` field in the SignalRManager component

## Usage

1. Add the SignalRManager script to a GameObject in your scene
2. Optionally add GameTickDisplay for UI visualization
3. The connection will start automatically when the scene loads
4. Check the Console for debug logs of incoming game ticks

## Troubleshooting

- Ensure your server is running (you can see it's working from the terminal output)
- Check CORS settings if connection fails
- Verify SSL certificate if using HTTPS
- Check Unity Console for detailed error messages
