# ISEKAI — No Auto Camera Jump [Star Sky Mods]

[![ISEKAI — No Auto Camera Jump](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/isekai-camera.gif)](https://steamcommunity.com/sharedfiles/filedetails/?id=3789336886)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3789336886) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

Keep your camera where you left it when a local ISEKAI hunt spawns its target.

For example, if you are planning rooms when a hunt creature appears, the view stays on your base instead of suddenly moving to the creature. You can finish what you are doing and inspect the target when you are ready.

## What it does

- Stops the forced camera jump for ordinary local ISEKAI hunts and hunt packs from the optional ISEKAI CREATURES add-on.
- Keeps letters, messages and clickable target links, so you can jump to the target when you choose.
- Leaves regular RimWorld camera jumps, world bosses, hero visits and Guild duels unchanged.

## Controls and settings

No extra action is needed during play. Both camera-jump blockers are enabled by default. The hunt still spawns its target and sends its normal notification; click the target link if you want to move the camera there yourself.

To change this, open RimWorld’s **Mod Settings** and select **ISEKAI — No Auto Camera Jump [Star Sky Mods]**. The two switches control ordinary ISEKAI hunts and ISEKAI CREATURES hunt packs separately. Turn a switch off to restore that type’s original camera jump. Restore defaults enables both again. Changes need no restart, take effect the next time a hunt target spawns, and are saved in RimWorld’s configuration. The creature-hunt switch applies only when that add-on is installed.

## Requirements and installation

Requires **RimWorld 1.6**, [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077) and [ISEKAI RPG LEVELING](https://steamcommunity.com/sharedfiles/filedetails/?id=3657580708). [ISEKAI CREATURES](https://steamcommunity.com/sharedfiles/filedetails/?id=3757560461) is optional.

Load after Harmony, ISEKAI RPG LEVELING and any Guild/Creatures add-ons. Subscribe on the [Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=3789336886), or copy `IsekaiNoAutoCameraJump` into RimWorld’s local `Mods` directory and enable it. English and Russian interface text is included. A compiled DLL is included in `1.6/Assemblies`; no build is needed to play.

This update changes the package ID from chupakobra6.isekai.noautohuntcamerajump to starsky.rimworld.isekai.noautohuntcamerajump. If the mod is no longer active after updating, re-enable it in your mod list.

Can be added to an existing save. Back up before removing a mod from an ongoing colony. Original ISEKAI files are neither included nor modified. Harmony ID, definition names and game data are unchanged.

## Development

The patch suppresses `CameraJumper.TryJump` and `CameraJumper.TryJumpAndSelect` only while these hunt-spawn methods run:

- `IsekaiLeveling.Quests.QuestPart_IsekaiLocalHunt.SpawnCreatureOnMap`
- `IsekaiCreatures.Quests.QuestPart_IsekaiCreatureQuestSpawn.SpawnPackOnMap`

Letters, messages and their clickable `LookTargets` are untouched. Source is in `Source`; see [LICENSE](LICENSE) for the code license. The `.csproj` accepts `RimWorldManagedPath` and `HarmonyAssemblyPath` pointing to your installed dependencies. Building requires C# tools; game and third-party DLLs are not distributed here. RimWorld and ISEKAI names and materials remain the property of their respective owners.

Found a bug or an inaccuracy? Leave a comment on the mod page or on Boosty.

Preview artwork: POUPUN / ISEKAI RPG LEVELING.

Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
