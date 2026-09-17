# ISEKAI — Quick Quests [Star Sky Mods]

Middle-click an ISEKAI quest letter to accept it when no decision is needed.

## How it works

The shortcut accepts only available, unambiguous ISEKAI quests. Quests requiring a pawn, option or reward selection open in the normal quest tab instead, as do quests with unmet requirements or an automatic-acceptance restriction. Non-ISEKAI quests and other mouse buttons retain their normal behaviour.

The shortcut is enabled by default and can be turned off in RimWorld’s Mod Settings. English and Russian interface text is included.

## Installation

Requires **RimWorld 1.6**, **Harmony** and **ISEKAI RPG LEVELING**. Load after them and any Guild/Creatures add-ons. A compiled DLL is included in `1.6/Assemblies`; no build is needed to play.

This feature was separated from Balanced Stats & Quick Quests. Subscribe to Quick Quests separately to keep the shortcut. Update **Balanced Stats to 1.2.0 or later** before using both mods. Quick Quests works without Balanced Stats and has its own settings. Its shortcut starts enabled; a disabled quick-accept toggle from the old combined mod is not imported.

Quick Quests stores no colony save data and can be added to or removed from an existing save. Original ISEKAI files are neither included nor modified.

## Development and license

Source is in `Source` under the included MIT license. The `.csproj` accepts `RimWorldManagedPath` and `HarmonyAssemblyPath` pointing to your installed dependencies. These DLLs are not distributed here.

Report bugs with reproduction steps, mod versions and your log.
