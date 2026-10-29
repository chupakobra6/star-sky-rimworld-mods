# Crashed Ship Mechanoid Guard Fix [Star Sky Mods]

[![Crashed Ship Mechanoid Guard Fix](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/crashed-ship-guard-fix.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3829381024)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3829381024) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

<!-- rimmods:readme intro begin -->
Fix a case where defoliator and psychic ships arrive without mechanoid guards
<!-- rimmods:readme intro end -->

## What it does

<!-- rimmods:readme features begin -->
The fix checks that a group has at least one affordable mechanoid before the game chooses it. Other eligible vanilla groups keep their usual selection weights

Only the two vanilla ship-part incidents are affected. Noncombat groups stay unchanged

Threat points, pawn costs and the individual cost limit are unchanged. The fix does not create a fallback squad or change ships that have already crashed
<!-- rimmods:readme features end -->

## Requirements and installation

<!-- rimmods:readme compatibility begin -->
RimWorld 1.6. Requires [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077). Load this mod after Harmony. No DLC is required

Can be added to or removed from an existing save
<!-- rimmods:readme compatibility end -->

## Feedback

<!-- rimmods:readme feedback begin -->
Found a bug? Leave a comment on the mod page or on Boosty
<!-- rimmods:readme feedback end -->

## Credits

<!-- rimmods:readme credit begin -->
Dependencies: Harmony is by Andreas Pardeike
<!-- rimmods:readme credit end -->

## Ludeon notice

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->

## License and source

The fix code uses the [MIT license](LICENSE). See [NOTICES.md](NOTICES.md) for credits

The ready-to-use DLL is in `1.6/Assemblies`. Source code is in `Source`. To build it yourself, configure references to the DLLs from your own RimWorld installation and the dependencies above. Game and third-party DLLs are not included

Package ID: `starsky.rimworld.crashedshipmechanoidguardfix`
