# Biomes Caverns Fixes [Star Sky Mods]

[![Biomes Caverns Fixes](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/biomes-caverns-fixes.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065838)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065838) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

<!-- rimmods:readme intro begin -->
Fix a missing fermentation message and Fungi lite compatibility errors in Biomes! Caverns. A one-time setup on macOS also restores sounds that fail to load
<!-- rimmods:readme intro end -->

## What it does

<!-- rimmods:readme features begin -->
The fermentation barrel uses the correct message when mushroom must is missing

With Fungi lite active, the fix updates the plant component, the missing building-hit sound reference and stale spore references in fuel and ingredient filters
<!-- rimmods:readme features end -->

## Setup

<!-- rimmods:readme how-to-use begin -->
Enable this fix for the gameplay corrections. For audio on macOS, close RimWorld, open Terminal in the fix's folder under Steam's workshop/content/294100 directory and run the command below.

bash Tools/prepare-audio.sh

This creates 87 WAV files on your Mac from your installed copy of Biomes! Caverns and takes about 900 MiB of disk space. If you keep the original in another Steam library, pass its folder as the first argument. Run the command again if Steam clears the local cache. If the script reports different source files, check for a compatible update to the fix
<!-- rimmods:readme how-to-use end -->

## Requirements and load order

<!-- rimmods:readme compatibility begin -->
RimWorld 1.6. Requires [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077), [Biomes! Caverns](https://steamcommunity.com/sharedfiles/filedetails/?id=2969748433) and [MP3 Audio Bootstrap](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065836). Load Bootstrap after Harmony and before Core. Load this fix after Biomes! Caverns and after Fungi lite if you use it
<!-- rimmods:readme compatibility end -->

## Feedback

<!-- rimmods:readme feedback begin -->
Found a bug? Leave a comment on the mod page or on Boosty
<!-- rimmods:readme feedback end -->

## Credits

<!-- rimmods:readme credit begin -->
Original mod by The Biomes Mod Team. The original sounds and music are credited to Dendroid. Harmony is by Andreas Pardeike
<!-- rimmods:readme credit end -->

## Ludeon notice

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->

## License and source

The fix code, XML patches and local setup script use the [MIT license](LICENSE). See [NOTICES.md](NOTICES.md) for credits

For a custom Steam library or a manually installed fix, run `bash Tools/prepare-audio.sh "/path/to/steamapps/workshop/content/294100/2969748433"` from the fix folder. The script checks the installed MP3 files against the manifest before preparing the local cache. The WAV copies stay on your computer
