# MP3 Audio Bootstrap [Star Sky Mods]

[![MP3 Audio Bootstrap](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/mp3-audio-bootstrap.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065836)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3834065836) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

<!-- rimmods:readme intro begin -->
Let compatible audio fixes on macOS load local WAV copies before RimWorld reads the original MP3 files
<!-- rimmods:readme intro end -->

## What it does

<!-- rimmods:readme features begin -->
The loader redirects audio only when the original files match the fix's manifest and the local WAV files are present. If either check fails, the game uses the original audio files
<!-- rimmods:readme features end -->

## Setup

<!-- rimmods:readme how-to-use begin -->
Install a compatible audio fix such as Biomes Caverns Fixes and follow its local audio setup instructions
<!-- rimmods:readme how-to-use end -->

## Requirements and load order

<!-- rimmods:readme compatibility begin -->
RimWorld 1.6. Requires [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077). Load Harmony first, then MP3 Audio Bootstrap, then Core. If you use Faster Game Loading, put it after MP3 Audio Bootstrap
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

The loader's code is available under the [MIT license](LICENSE). The compiled DLL is in `Assemblies`. The public GitHub repository includes the C# source. This package contains the loader. Each supported fix supplies its own cache setup and manifest
