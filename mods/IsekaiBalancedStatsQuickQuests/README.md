# ISEKAI — Balanced Stats [Star Sky Mods]

Control how ISEKAI colonists and pets spend their automatic stat points. Requires RimWorld 1.6, Harmony and ISEKAI RPG LEVELING.

## What it does

- **Balanced:** shares new automatic points evenly across all six stats, keeping the cycle between levels. Existing stats, starting bonuses and manual investments stay as they are.
- **By class:** favours class stats with weights of `3 / 2 / 1 / 1 / 1 / 1`. Berserker’s overlapping affinities use `4 / 1 / 1 / 1 / 1 / 1`. Available after choosing a class.
- **Pets:** Combat mode uses `STR / DEX / VIT = 2` and `INT / WIS / CHA = 1`; Balanced uses equal weights. Automatic allocation is enabled by default for player pets and skips capped stats.
- **Personal choices:** each colonist or pet keeps its own mode. Pets also have an individual automatic-allocation switch. Changing mode starts a new cycle for future points without compensating for earlier differences.

The mode button is beside the automatic-allocation checkbox in the main ISEKAI tab. Controls follow the current ISEKAI or vanilla interface style. Pets’ nested stat window remains available for manual allocation. Existing unspent pet points are allocated shortly after loading when that pet’s automatic allocation is enabled.

## Settings

RimWorld’s Mod Settings lets you toggle improved colonist allocation and pet allocation separately, choose defaults for characters with no saved personal choice, choose whether new or unconfigured pets start with allocation enabled, and hide mode-change messages.

Turning off improved colonist allocation restores ISEKAI’s original algorithm. Pausing pet allocation preserves unspent points. Global switches and changed defaults do not overwrite personal choices stored in the colony.

## Installation and upgrading

Load after Harmony, ISEKAI RPG LEVELING, any Guild/Creatures add-ons and No Auto Camera Jump if used. English and Russian interface text is included. A compiled DLL is included in `1.6/Assemblies`; no build is needed to play.

This remains the [original Workshop item](https://steamcommunity.com/sharedfiles/filedetails/?id=3789339468). Since **1.2.0**, middle-click acceptance belongs to the separate [ISEKAI — Quick Quests](https://steamcommunity.com/sharedfiles/filedetails/?id=3802965084) mod. Subscribe to Quick Quests separately to keep the shortcut; Balanced Stats does not require it.

Existing stat settings, personal modes and allocation progress remain intact. The assembly identity, settings class, game-component types and save keys are retained. The folder keeps its historical name because RimWorld includes it in local mod-settings filenames.

This update changes the package ID from chupakobra6.isekai.balancedstatsquickquests to starsky.rimworld.isekai.balancedstatsquickquests. If the mod is no longer active after updating, re-enable it in your mod list; existing stat settings and saved allocation progress are retained.

Can be added to an existing save. Back up before removing a mod from an ongoing colony. Original ISEKAI files are neither included nor modified.

## Development and license

Source is in `Source` under the included MIT license. The `.csproj` accepts `RimWorldManagedPath`, `HarmonyAssemblyPath` and `IsekaiAssemblyPath` pointing to your installed dependencies. These DLLs are not distributed here.

The split preserves existing stat behaviour. Gameplay testing is still required for future algorithm changes.
