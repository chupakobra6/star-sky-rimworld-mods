# ISEKAI — Balanced Stats [Star Sky Mods]

[![ISEKAI — Balanced Stats](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/isekai-balanced.gif)](https://steamcommunity.com/sharedfiles/filedetails/?id=3789339468)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3789339468) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

Choose how ISEKAI spends your colonists’ and pets’ stat points, give new enemies a broader mix of stats, and reduce the bonuses from equipment upgrades. Each part can be configured separately. Requires RimWorld 1.6, Harmony and ISEKAI RPG LEVELING.

## What it does

- **Balanced:** divides new automatic points equally among strength (STR), dexterity (DEX), vitality (VIT), intelligence (INT), wisdom (WIS) and charisma (CHA). With no stat at its cap, six points give +1 to each stat, even if earned across several levels. It does not make existing totals equal: starting bonuses and manually spent points stay intact.
- **By class:** gives more points to the class’s main stats without neglecting the rest. With no stat capped, in each nine-point cycle, the primary stat gets 3, the secondary gets 2 and each other stat gets 1. Berserker gets 4 in STR and 1 in each other stat because both class bonuses favour STR. Choose a class first to unlock this mode.
- **Pets:** Combat gives twice as many points to each of STR, DEX and VIT as to each of INT, WIS and CHA. Balanced gives equal shares. Automatic allocation is enabled by default for player pets and skips capped stats.
- **Personal choices:** each colonist or pet keeps its own mode and allocation progress in the save. Pets also have an individual automatic-allocation switch. Changing mode starts a new cycle for future points without compensating for earlier differences. All modes skip capped stats.

Select a colonist and open its ISEKAI tab, or select an ISEKAI pet with its own creature stats tab and open that tab. Use **Mode** beside **Auto allocate** to choose a distribution. Enable Auto allocate to spend points automatically; leave it off to spend them manually. You can select Balanced before choosing a class and choose a mode while automation is off. Controls follow the current ISEKAI or vanilla interface style. Pets’ nested stat window remains available for manual allocation. Existing unspent pet points are allocated shortly after loading when that pet’s automatic allocation is enabled.

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

Enemy rolls keep their full allocated-point budget, rank and level. These weights mean that, out of nine points, a humanoid’s primary stat gets 3, its secondary gets 2 and each other stat gets 1; a creature’s STR, DEX and VIT get 2 each, and each other stat gets 1. You can choose **Equal shares** or **Original ISEKAI** separately for humanoids and creatures. Humanoids follow ISEKAI's role-generation setting; creatures change only at first initialization. Player characters, pets, neutral factions and humanoids whose stats ISEKAI marks as locked are excluded. Wild animals that become manhunters after their initial roll are not reshaped. A changed setting does not scan existing pawns.

Refinement means ISEKAI’s equipment upgrades. The strength settings scale the extra bonuses from those upgrades, not the equipment’s base stats. They affect both existing and newly upgraded equipment. Recorded upgrade levels remain unchanged, so restoring upstream balance restores their original effects. Only movement from apparel refinement is capped; other movement effects, runes, durability, mass, upgrade success chances and failure outcomes stay under ISEKAI's control.

## Settings

Open RimWorld’s **Mod Settings** and select **ISEKAI — Balanced Stats [Star Sky Mods]**. Here you can toggle improved colonist allocation and pet allocation separately, choose defaults for characters with no saved personal choice, choose whether new or unconfigured pets start with allocation enabled, and hide mode-change messages. Both features are enabled by default. Balanced is the default for colonists; Combat is the default for pets.

Turning off improved colonist allocation restores ISEKAI’s original algorithm. Pausing pet allocation preserves unspent points. Global switches and changed defaults do not overwrite personal choices stored in the colony. The scrollable settings window also provides separate enemy modes, combat and movement strengths (0–100%), a total movement cap (0–100%; 0 means uncapped), material costs (1–5×), and a refinement master switch. Each aspect can return to its upstream value independently. **Recommended balance** restores only enemy/refinement defaults; **Upstream enemy and equipment balance** disables only those additions. Neither changes colonist or pet choices.

## Installation and upgrading

Load after Harmony, ISEKAI RPG LEVELING, any Guild/Creatures add-ons and No Auto Camera Jump if used. English and Russian interface text is included. A compiled DLL is included in `1.6/Assemblies`; no build is needed to play.

This remains the [original Workshop item](https://steamcommunity.com/sharedfiles/filedetails/?id=3789339468). Since **1.2.0**, middle-click acceptance belongs to the separate [ISEKAI — Quick Quests](https://steamcommunity.com/sharedfiles/filedetails/?id=3802965084) mod. Subscribe to Quick Quests separately to keep the shortcut; Balanced Stats does not require it.

Existing stat settings, personal modes and allocation progress remain intact. The assembly identity, settings class, game-component types and save keys are retained. The folder keeps its historical name because RimWorld includes it in local mod-settings filenames.

Can be added to an existing save. Back up before removing a mod from an ongoing colony. Original ISEKAI files are neither included nor modified.

## Feedback

<!-- rimmods:readme feedback begin -->
Found a bug or an inaccuracy? Leave a comment on the mod page or on Boosty.
<!-- rimmods:readme feedback end -->

<!-- rimmods:readme credit begin -->
Preview artwork: POUPUN / ISEKAI RPG LEVELING.
<!-- rimmods:readme credit end -->

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->

## Development and license

Source is in `Source` under the included MIT license. The `.csproj` accepts `RimWorldManagedPath`, `HarmonyAssemblyPath` and `IsekaiAssemblyPath` pointing to your installed dependencies. These DLLs are not distributed here.

Unit fixtures verify allocation budgets, caps, settings and Verse serialization. The isolated native probe verifies the installed ISEKAI API, patched stat effects and settings rendering; gameplay balance remains a player-adjustable preset.
