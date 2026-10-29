# Enhanced Carrying Capacity – Save Loading Fix [Star Sky Mods]

[![Enhanced Carrying Capacity – Save Loading Fix](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/carrying-save-loading-fix.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3829381031)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3829381031) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

<!-- rimmods:readme intro begin -->
Prevent a save-loading failure caused by Enhanced Carrying Capacity loading its settings during caravan restoration
<!-- rimmods:readme intro end -->

## What it does

<!-- rimmods:readme features begin -->
The fix loads the original mod's settings during normal startup, before a save is loaded. Carrying-capacity multipliers and offsets stay unchanged, and the fix adds no data to saves

Enable the fix before loading the affected save
<!-- rimmods:readme features end -->

## Requirements and installation

<!-- rimmods:readme compatibility begin -->
RimWorld 1.6. Requires [Enhanced Carrying Capacity](https://steamcommunity.com/sharedfiles/filedetails/?id=2972540973). Load this mod after it. Harmony is not required

Can be added to an existing save. The fix can be removed without removing save data, but the original loading error may return
<!-- rimmods:readme compatibility end -->

## Feedback

<!-- rimmods:readme feedback begin -->
Found a bug? Leave a comment on the mod page or on Boosty
<!-- rimmods:readme feedback end -->

## Credits

<!-- rimmods:readme credit begin -->
Dependencies: Enhanced Carrying Capacity is by Designer225
<!-- rimmods:readme credit end -->

## Ludeon notice

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->

## License and source

The fix code uses the [MIT license](LICENSE). See [NOTICES.md](NOTICES.md) for credits

The ready-to-use DLL is in `1.6/Assemblies`. Source code is in `Source`. To build it yourself, configure references to the DLLs from your own RimWorld installation and the dependencies above. Game and third-party DLLs are not included

Package ID: `starsky.rimworld.enhancedcarryingcapacity.saveloadfix`
