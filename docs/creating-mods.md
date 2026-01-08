# Creating Mods for Littlewood with OpenWood

This guide explains how to create mods using the OpenWood modding API.

## Prerequisites

- .NET SDK 6.0 or later
- Visual Studio 2022, VS Code, or Rider
- BepInEx installed in your Littlewood folder

## Project Setup

### 1. Create a new Class Library project

```bash
dotnet new classlib -n MyAwesomeMod -f net472
```

### 2. Add the required references

Edit your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <PropertyGroup>
    <GamePath>C:\Path\To\Littlewood</GamePath>
    <BepInExPath>$(GamePath)\BepInEx</BepInExPath>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="BepInEx">
      <HintPath>$(BepInExPath)\core\BepInEx.dll</HintPath>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>$(BepInExPath)\core\0Harmony.dll</HintPath>
    </Reference>
    <Reference Include="OpenWood.Core">
      <HintPath>$(BepInExPath)\plugins\OpenWood\OpenWood.Core.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>$(GamePath)\Littlewood_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

### 3. Create your plugin

```csharp
using BepInEx;
using OpenWood.Core;
using OpenWood.Core.Events;

[BepInPlugin("com.yourname.mymod", "My Awesome Mod", "1.0.0")]
[BepInDependency(PluginInfo.PLUGIN_GUID)]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo("My mod is loading!");
        
        // Subscribe to events
        GameEvents.OnDayStart += day => {
            Logger.LogInfo($"A new day begins: Day {day}");
        };
    }
}
```

## Using the OpenWood API

### Events

Subscribe to game events to react to what happens in the game:

```csharp
using OpenWood.Core.Events;

// Player events
GameEvents.OnPlayerMove += (x, y) => { /* player moved */ };
GameEvents.OnPlayerLevelUp += level => { /* player leveled up */ };

// Game events
GameEvents.OnDayStart += day => { /* new day started */ };
GameEvents.OnSeasonChange += season => { /* season changed */ };

// Item events
GameEvents.OnItemPickup += (itemId, count) => { /* item picked up */ };
GameEvents.OnItemCraft += (itemId, count) => { /* item crafted */ };

// NPC events
GameEvents.OnNPCDialogue += (npcId, dialogue) => { /* NPC spoke */ };
GameEvents.OnNPCFriendshipChange += (npcId, level) => { /* friendship changed */ };
```

### Custom Items

Register new items that can appear in the game:

```csharp
using OpenWood.Core.Items;

var myItem = new ModItem
{
    Id = "mymod_magic_seed",
    Name = "Magic Seed",
    Description = "A mysterious seed that grows something special.",
    Category = "Seed",
    SellPrice = 100,
    MaxStack = 99,
    OnUse = item => {
        Logger.LogInfo("Magic seed planted!");
    }
};

ItemRegistry.Register(myItem);
```

### Custom UI

Add custom windows and UI elements:

```csharp
using OpenWood.Core.UI;
using UnityEngine;

var myWindow = new ModWindow(
    "mymod_window",
    "My Mod Settings",
    new Rect(100, 100, 250, 300)
);

myWindow.DrawContent = windowId => {
    GUILayout.Label("Welcome to my mod!");
    
    if (GUILayout.Button("Click Me"))
    {
        ModUI.ShowNotification("Button clicked!");
    }
};

ModUI.RegisterWindow(myWindow);
myWindow.Show();
```

### Loading Assets

Load textures and sprites from your mod's folder:

```csharp
using OpenWood.Core.Assets;

// Load a texture
var texture = AssetManager.LoadTexture("MyMod/sprites/item.png");

// Load a sprite
var sprite = AssetManager.LoadSprite("MyMod/sprites/item.png", pixelsPerUnit: 16);
```

### Configuration

Use BepInEx configuration for user settings:

```csharp
using BepInEx.Configuration;

public class MyMod : BaseUnityPlugin
{
    private ConfigEntry<bool> _enableFeature;
    private ConfigEntry<int> _spawnRate;
    
    private void Awake()
    {
        _enableFeature = Config.Bind(
            "General",
            "EnableFeature",
            true,
            "Enable the cool feature"
        );
        
        _spawnRate = Config.Bind(
            "General",
            "SpawnRate",
            5,
            new ConfigDescription(
                "How often things spawn",
                new AcceptableValueRange<int>(1, 100)
            )
        );
        
        if (_enableFeature.Value)
        {
            // Feature is enabled
        }
    }
}
```

### Harmony Patches

For advanced modding, use Harmony to patch game methods:

```csharp
using HarmonyLib;

[HarmonyPatch(typeof(SomeGameClass), "SomeMethod")]
public static class MyPatch
{
    // Run before the original method
    static void Prefix()
    {
        // Your code here
    }
    
    // Run after the original method
    static void Postfix(ref int __result)
    {
        // Modify the result
        __result *= 2;
    }
}
```

## Building and Installing

1. Build your project:
   ```bash
   dotnet build -c Release
   ```

2. Copy the DLL to BepInEx plugins:
   ```
   Littlewood/BepInEx/plugins/MyMod/MyAwesomeMod.dll
   ```

3. Launch the game and enjoy your mod!

## Debugging

### Enable BepInEx console

Edit `BepInEx/config/BepInEx.cfg`:

```ini
[Logging.Console]
Enabled = true
```

### View logs

Logs are saved to `BepInEx/LogOutput.log`

### Common Issues

1. **Mod not loading**: Check that all dependencies are met
2. **NullReferenceException**: Game objects might not exist yet, use null checks
3. **Harmony patches not working**: Ensure class/method names match the decompiled code

## Best Practices

1. Always check for null before accessing game objects
2. Use try-catch around risky operations
3. Clean up event subscriptions in OnDestroy
4. Test with BepInEx console enabled
5. Keep your mod focused - one feature per mod is often better

## Getting Help

- Check the decompiled game code for reference
- Look at the ExampleMod for working examples
- Report issues on the OpenWood GitHub repository
