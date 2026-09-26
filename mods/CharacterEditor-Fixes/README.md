# Character Editor — Fixes [Star Sky Mods]

[![Character Editor — Fixes](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/character-editor-fixes.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3807830515)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3807830515) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

Character Editor — Fixes restores body-size genes in the editor and fixes two names in Character Editor's bundled name file.

## Fixes

- The body-size gene list uses gene identifiers. Translating gene labels no longer hides these genes in the editor.
- Name loading removes an initial byte-order mark and restores the separator before Kai Henriksen. Both names remain available separately.

## Installation

Install [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077) and [Character Editor](https://steamcommunity.com/sharedfiles/filedetails/?id=1874644848). Load them in this order: Harmony → Character Editor → Character Editor — Fixes. Requires RimWorld 1.6.

The fixes apply when the editor displays body-size genes and loads names. There are no settings.

## Source

The code is in `Source/Fixes.cs`, and the compiled DLL is in `Assemblies/CharacterEditorFixes.dll`. Harmony patches the installed Character Editor; this package does not include original mod files. See [LICENSE](LICENSE) for this mod's code license and [NOTICES.md](NOTICES.md) for sources and notices.

## Feedback

<!-- rimmods:readme feedback begin -->
Found a bug? Leave a comment on the mod page or on Boosty.
<!-- rimmods:readme feedback end -->

<!-- rimmods:readme credit begin -->
Original mod by void.
<!-- rimmods:readme credit end -->

## Ludeon notice

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->
