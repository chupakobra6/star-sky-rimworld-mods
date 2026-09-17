# ISEKAI — Balanced Stats [Star Sky Mods]

Control automatic stat allocation, round out hostile enemy builds and tune equipment refinement. Requires RimWorld 1.6, Harmony and ISEKAI RPG LEVELING.

## What it does

- **Balanced:** shares new automatic points evenly across all six stats, keeping the cycle between levels. Existing stats, starting bonuses and manual investments stay as they are.
- **By class:** favours class stats with weights of `3 / 2 / 1 / 1 / 1 / 1`. Berserker’s overlapping affinities use `4 / 1 / 1 / 1 / 1 / 1`. Available after choosing a class.
- **Pets:** Combat mode uses `STR / DEX / VIT = 2` and `INT / WIS / CHA = 1`; Balanced uses equal weights. Automatic allocation is enabled by default for player pets and skips capped stats.
- **Personal choices:** each colonist or pet keeps its own mode. Pets also have an individual automatic-allocation switch. Changing mode starts a new cycle for future points without compensating for earlier differences.

The mode button is beside the automatic-allocation checkbox in the main ISEKAI tab. Controls follow the current ISEKAI or vanilla interface style. Pets’ nested stat window remains available for manual allocation. Existing unspent pet points are allocated shortly after loading when that pet’s automatic allocation is enabled.

## Enemy and equipment balance

Recommended balance is enabled by default, including when loading settings from an earlier version. Existing explicit settings keep their values.

| Setting | Recommended value | Effect |
| --- | --- | --- |
| Hostile humanoids | Combat role | Fresh rolls use primary/secondary/other weights 3/2/1/1/1/1. |
| Hostile creatures | Combat role | Fresh rolls use STR/DEX/VIT weights 2; other stats 1. |
| Refinement combat bonuses | 50% | Scales weapon damage, attack speed, ranged accuracy and armor bonuses. |
| Refinement movement bonus | 50% | Each +5 piece gives +6.25% movement before the total cap. |
| Total apparel movement cap | +20% | Four +5 pieces give +20% instead of the original +50%. Set 0 to remove the cap. |
| Refinement material cost | 2× | Multiplies cores, steel and components; fractional counts round up. |

Enemy rolls keep their full allocated-point budget, rank and level. Humanoids follow ISEKAI's role-generation setting; creatures change only at first initialization. Player characters, pets, neutral factions and authored forge-locked humanoids are excluded. Wild animals that become manhunters after their initial roll are not reshaped. A changed setting does not scan existing pawns.

Refinement strengths affect both existing and newly upgraded equipment. Recorded upgrade levels remain unchanged, so restoring upstream balance restores their original effects. Rune effects, durability, mass, upgrade success chances and failure outcomes stay under ISEKAI's control.

## Settings

RimWorld’s Mod Settings lets you toggle improved colonist allocation and pet allocation separately, choose defaults for characters with no saved personal choice, choose whether new or unconfigured pets start with allocation enabled, and hide mode-change messages.

Turning off improved colonist allocation restores ISEKAI’s original algorithm. Pausing pet allocation preserves unspent points. Global switches and changed defaults do not overwrite personal choices stored in the colony. The scrollable settings window also provides separate enemy modes, combat and movement strengths (0–100%), a total movement cap (0–100%; 0 means uncapped), material costs (1–5×), and a refinement master switch. Each aspect can return to its upstream value independently. **Recommended balance** restores only enemy/refinement defaults; **Upstream enemy and equipment balance** disables only those additions. Neither changes colonist or pet choices.

## Installation and upgrading

Load after Harmony, ISEKAI RPG LEVELING, any Guild/Creatures add-ons and No Auto Camera Jump if used. English and Russian interface text is included. A compiled DLL is included in `1.6/Assemblies`; no build is needed to play.

This remains the [original Workshop item](https://steamcommunity.com/sharedfiles/filedetails/?id=3789339468). Since **1.2.0**, middle-click acceptance belongs to the separate [ISEKAI — Quick Quests](https://steamcommunity.com/sharedfiles/filedetails/?id=3802965084) mod. Subscribe to Quick Quests separately to keep the shortcut; Balanced Stats does not require it.

Existing stat settings, personal modes and allocation progress remain intact. The assembly identity, settings class, game-component types and save keys are retained. The folder keeps its historical name because RimWorld includes it in local mod-settings filenames.

Can be added to an existing save. Back up before removing a mod from an ongoing colony. Original ISEKAI files are neither included nor modified.

## Development and license

Source is in `Source` under the included MIT license. The `.csproj` accepts `RimWorldManagedPath`, `HarmonyAssemblyPath` and `IsekaiAssemblyPath` pointing to your installed dependencies. These DLLs are not distributed here.

Unit fixtures verify allocation budgets, caps, settings and Verse serialization. The isolated native probe verifies the installed ISEKAI API, patched stat effects and settings rendering; gameplay balance remains a player-adjustable preset.
