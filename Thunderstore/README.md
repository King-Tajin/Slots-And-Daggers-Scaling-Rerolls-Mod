# Scaling Rerolls

Removes the one reroll per shop limit in Slots & Daggers and makes the price climb the more you reroll.

## Features

- Unlimited rerolls in every upgrade shop
- The price goes up every roll but it accelerates without exploding
- Upgrade price reduction still lowers the starting price
- The free, unlimited rerolls in the shop at the start of a run are unchanged
- The growth rate can be changed in the config file

## Installation

Install with a mod manager such as r2modman or the Thunderstore Mod Manager, then launch the game.

Manual install: install BepInEx 5 into the game folder, then copy `ScalingRerolls.dll` into `BepInEx/plugins/ScalingRerolls/`.

## Configuration

Launch the game once to generate the config file at `BepInEx/config/com.king_tajin.scalingrerolls.cfg`.

| Setting | Default | Description |
| --- | --- | --- |
| Acceleration | 0.5 | How quickly the price climbs. Use 0 for a flat price, or raise it for a steeper curve |

## Source

https://github.com/King-Tajin/Slots-And-Daggers-Scaling-Rerolls-Mod

Licensed under the MIT License.
