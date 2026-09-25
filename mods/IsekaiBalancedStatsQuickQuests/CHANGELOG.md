# Changelog

## 1.4.0 — High-level movement balance — 2026-09-25

### Added
- Adjustable ISEKAI level movement bonus: 0–100% of the original +0.005 cells/s per level. The recommended 50% setting adds +0.0025 cells/s per level, including for pawns already in a save.
- Independent optional cap on the final movement speed of living ISEKAI pawns. It starts enabled at 12 cells/s and can be set from 5 to 30 cells/s or switched off. DEX, gear, genes and other speed effects contribute before this cap; pawns without ISEKAI stats are unaffected.

### Changes
- Recommended balance now includes both movement controls. Original rules restore the full level bonus and turn off the final cap without changing personal allocation choices or equipment levels.
- The movement stat explanation uses the selected level bonus rate and shows the final speed limit. Settings are saved and restored with existing balance settings.

## 1.3.0 — Enemy builds and equipment balance — 2026-09-17

### Added
- **More rounded enemies.** Fresh hostile humanoid and creature stat rolls can use combat-role or balanced distribution, with separate controls for each. Their full point budget, rank and level stay intact; existing allocated stats are not rerolled.
- **Adjustable refinement.** Recommended balance halves combat and movement bonuses, caps total apparel movement at +20%, and doubles upgrade materials. Tune each aspect or restore the original values without changing recorded upgrade levels.
- **Expanded settings.** A scrollable English/Russian settings window includes units, explanations and buttons for recommended or original enemy/equipment balance.

### Changes
- **Balance starts enabled.** Missing new settings receive the recommended values. Existing colonist and pet settings, personal modes and allocation progress remain intact.
- **Original equipment rules remain available.** Rune effects, durability, mass, success chances and failure outcomes are unchanged. Refinement strength applies to equipment already in a save as well as new upgrades.

### Presentation
- Refreshed the cover and section banners with clearer, heavier lettering.

## 1.2.0 — Balanced Stats, now on its own — 2026-09-17

### Changes
- **Quick Quests is a separate mod.** Middle-click quest acceptance and its settings have moved out of Balanced Stats. Subscribe to Quick Quests separately to keep the shortcut; either mod works without the other.
- **Your stat setup stays with you.** Colonist and pet settings, personal modes and allocation progress are retained. Stat allocation itself is unchanged.

### After updating
- **Re-enable the mod if needed.** The package ID changed from chupakobra6.isekai.balancedstatsquickquests to starsky.rimworld.isekai.balancedstatsquickquests. If RimWorld leaves the old entry inactive, select Balanced Stats again in your mod list.

### Presentation
- Refreshed the English description and cover, and removed decorative banners from the screenshot gallery.

## 2026-09-17 — Workshop page

- Refreshed the cover using the original ISEKAI artwork with a Star Sky Mods frame.
- Expanded feature and installation details in English and Russian; added illustrated sections and links to mods, updates and support.
- Unified the author credit as chupakobra6 | Star Sky Mods. This page update did not change the downloaded mod files.

## 1.1.1 — 2026-08-24

- Moved the package ID and Harmony ID into the stable chupakobra6 namespace used at that release.
- Updated load order for the camera add-on’s package ID.
- Kept the existing Steam Workshop item, 3789339468.
- Added an explicit mod version to About.xml.

## 1.1.0 — 2026-08-24

- Added RimWorld’s standard mod-settings menu.
- Added independent toggles for improved colonist allocation, pet allocation and quick quest acceptance.
- Added default allocation modes for colonists and pets without personal settings, plus the default automatic-allocation switch for pets.
- Made mode-change confirmation messages optional.
- Kept personal colonist and pet state when a global feature was disabled.

## 1.0.0 — 2026-08-24

- First public release for RimWorld 1.6.
- Added Balanced and Class modes for automatic colonist stat allocation.
- Added Combat and Balanced modes for automatic pet stat allocation.
- Integrated controls into the ISEKAI interface with matching visual styles.
- Added middle-click acceptance for unambiguous ISEKAI quests.
- Added English and Russian interface text.
