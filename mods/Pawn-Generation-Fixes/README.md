# Pawn Generation Fixes [Star Sky Mods]

[![Pawn Generation Fixes](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/pawn-generation-fixes.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065834)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065834) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

<!-- rimmods:readme intro begin -->
Fix two pawn generation problems that can leave broken family links or cause repeated attempts to create faction leaders
<!-- rimmods:readme intro end -->

## What it does

<!-- rimmods:readme features begin -->
When a generated pawn is rejected because a required skill is unavailable, the fix cleans up the rejected pawn and its family links before the next attempt

With Humanoid Alien Races active, the fix filters out premade biographies whose work restrictions conflict with the pawn kind's required work. This helps prevent repeated attempts to create a leader who cannot perform the required work
<!-- rimmods:readme features end -->

## Requirements and installation

<!-- rimmods:readme compatibility begin -->
RimWorld 1.6. Requires [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077). [Humanoid Alien Races](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762) is optional. Load this mod after Harmony and after Humanoid Alien Races if you use it
<!-- rimmods:readme compatibility end -->

## Feedback

<!-- rimmods:readme feedback begin -->
Found a bug? Leave a comment on the mod page or on Boosty
<!-- rimmods:readme feedback end -->

## Credits

<!-- rimmods:readme credit begin -->
Dependencies: Harmony is by Andreas Pardeike. Humanoid Alien Races is by erdelf
<!-- rimmods:readme credit end -->

## Ludeon notice

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->

## License and source

The fix code uses the [MIT license](LICENSE). See [NOTICES.md](NOTICES.md) for credits

The ready-to-use DLL is in `Assemblies`. Source code is in `Source`. To build it yourself, configure references to the DLLs from your own RimWorld installation and Harmony

Package ID: `starsky.rimworld.pawngeneration.fixes`
