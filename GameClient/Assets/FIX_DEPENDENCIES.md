# Fix Unity SignalR Dependencies - Step by Step

## The Problem
Unity is missing the required NuGet packages for SignalR. You're seeing these errors:
- `TMPro` could not be found
- `AspNetCore` does not exist
- `Json` does not exist  
- `HubConnection` could not be found

## Solution: Install NuGet Packages

### Method 1: NuGetForUnity (Recommended)

1. **Download NuGetForUnity**:
   - Go to: https://github.com/GlitchEnzo/NuGetForUnity/releases
   - Download the latest `NuGetForUnity.x.x.x.unitypackage`

2. **Import into Unity**:
   - In Unity: `Assets > Import Package > Custom Package...`
   - Select the downloaded .unitypackage file
   - Import everything

3. **Install SignalR Packages**:
   - Open `Window > NuGet Package Manager`
   - Search for and install these packages:
     ```
     Microsoft.AspNetCore.SignalR.Client
     System.Text.Json (if Unity version < 2021.2)
     ```

4. **Install TextMeshPro** (if needed):
   - Open `Window > TextMeshPro > Import TMP Essential Resources`

### Method 2: Manual DLL Installation

If NuGetForUnity doesn't work, manually download DLLs:

1. **Create Plugins folder**: `Assets/Plugins/`

2. **Download these DLLs** from nuget.org:
   - Microsoft.AspNetCore.SignalR.Client.dll
   - Microsoft.AspNetCore.SignalR.Common.dll
   - Microsoft.AspNetCore.SignalR.Protocols.Json.dll
   - Microsoft.Extensions.DependencyInjection.Abstractions.dll
   - Microsoft.Extensions.Logging.Abstractions.dll
   - Microsoft.Extensions.Options.dll
   - System.Text.Json.dll (Unity 2021.2+ includes this)

3. **Place all DLLs** in `Assets/Plugins/`

4. **Configure each DLL**:
   - Select each DLL in Project window
   - In Inspector, set "Platform Settings" to "Any Platform"
   - Check "Don't process" if available

## Test Without Dependencies First

I've created a simple test script that works without external packages:

1. **Create empty GameObject** in your scene
2. **Add `SimpleSignalRTest` component**
3. **Press Play** - you should see logs without errors
4. This will simulate ticks every 400ms

Once you see this working without errors, then install the NuGet packages and switch to the full SignalR implementation.

## Quick Fix for Current Errors

Want to test immediately? Replace the problematic scripts:

1. **Disable** `SignalRManager` and `GameTickDisplay` components
2. **Enable** `SimpleSignalRTest` component  
3. **Press Play** - should work without errors

## After Installing Packages

Once NuGet packages are installed:
1. Re-enable `SignalRManager` component
2. Configure server URL: `https://localhost:7154/gamehub`
3. Press Play and check Console for real SignalR connection

## Need Help?

If you're still getting errors:
1. Share your Unity version
2. Let me know which method you tried
3. Copy any new error messages

The goal is to see logs like:
```
[SignalR] Connected successfully!
[GameTick] Tick #123 at 14:30:25.123
```
