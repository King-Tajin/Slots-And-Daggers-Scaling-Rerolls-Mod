# Scaling Rerolls

A BepInEx 5 plugin that gives Slots & Daggers unlimited upgrade shop rerolls with a price that rises along a gentle quadratic curve.

## Features

- Unlimited rerolls in every upgrade shop
- The price goes up every roll and the increase grows each time, following base + acceleration x k x (k + 1) for the k-th reroll, capped at 200 or the base price if that is higher
- Upgrade price reduction still lowers the starting price
- The free, unlimited rerolls in the shop at the start of a run are unchanged
- The growth rate can be changed in the config file

## Install

1. Install BepInEx 5 into the game folder.
2. Copy `ScalingRerolls.dll` into `BepInEx/plugins/ScalingRerolls/`.
3. Launch the game.

## Build

1. Create a `libs/` folder next to the `.csproj` and copy in the Unity, `UnityEngine.UI` and `Assembly-CSharp` DLLs from your game install. BepInEx and Harmony come from the `BepInEx.Core` NuGet package.
2. Run `dotnet build -c Release`.
