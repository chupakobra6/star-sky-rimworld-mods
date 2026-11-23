# Tribal Outfits [Star Sky Mods]

[![Tribal Outfits](https://raw.githubusercontent.com/chupakobra6/star-sky-assets/main/assets/tribal-outfits.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=3840985633)

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3840985633) · [All categories](../../README.md) · [Sources & notices](NOTICES.md)

<!-- rimmods:readme intro begin -->
Dress a RimWorld tribe for the desert, winter, a hunt or a fight. Tribal Outfits adds seven craftable sets with 26 pieces of apparel and a jade-tipped shaman staff

Made at the request of svg
<!-- rimmods:readme intro end -->

Install the `TribalOutfits` folder under RimWorld's `Mods` folder, enable Harmony and then enable Tribal Outfits in the Mods menu. With Alpha Animals or Vanilla Factions Expanded – Insectoids 2 enabled, load Tribal Outfits after them.

## Crafting

Open a Crafting Spot and add a bill. The same outfit recipes also appear at a tailoring bench or smithy, depending on the item. Each item is made separately. Research is unnecessary for the recipes, but a colony still needs to obtain the ingredients and meet the Crafting requirement.

| Outfit | Crafting | Contents and materials |
| --- | ---: | --- |
| Everyday leather | 0 | Hood (25 leather) and moccasins (30 leather). The hood gives some protection from cold and injury. |
| Desert | 0 | Wrap (25 fabric), thobe (60 fabric), sandals (15 fabric or leather and 8 wood). The pieces raise heat tolerance. They give no armor or cold insulation. Wool is excluded. |
| Winter | 2 | Coat (140 wool), hat (50), mittens (30) and felt boots (60). The coat is warmer than a vanilla parka made of the same wool at the same quality. |
| Barbarian | 3 | Cloak (120 leather), helmet (50), tunic (60), hand wraps (30) and foot wraps (30). The cloak, helmet and tunic raise the pain shock threshold, up to the game's cap. Hand wraps give a 10% bow rate bonus and five percentage points to melee hit chance. Foot wraps give ten points to hunting stealth. |
| Shaman | 4 | Devilstrand and jade helmet (60 + 50), cloak (120 + 50), gloves and boots (50 + 30 each). The staff costs 100 wood and 70 jade. At Normal quality the apparel gives +25 percentage points of psychic sensitivity in total, and the staff adds another +25. Better quality raises the bonuses. Royalty enables neural heat recovery bonuses. |
| Hive | 4 | Helmet (60), armor (180), gloves and boots (40 each). With Alpha Animals or Insectoids 2 the recipes use only their chitin. With neither installed they use leather. The body piece has 60% sharp and 25% blunt armor at Normal quality. Material choices affect insulation, durability and crafting, with zero armor against heat. |
| Steel Monsters | 5 | Helmet, armor, gloves and boots. They cost respectively 1/3/1/1 mechanoid slag chunks, 40/100/10/10 plasteel and 20/40/10/10 leather. The helmet and body armor use vanilla recon armor protection and graphics. |

Each pair of footwear adds **0.10 cells/s**. The pieces share a layer and cannot be stacked with each other. Gloves and footwear have inventory icons and their stats apply while worn. The vanilla pawn renderer does not draw them as separate equipment on the extremities.

Steel Monsters recipes use **mechanoid slag chunks** from the base game. Get them from destroyed mechanoids and ruins. You can still smelt them for steel. Regular steel slag chunks are a separate item. On loading a 1.0.3 save, old plating is exchanged for mechanoid slag chunks and old plating bills are removed.

The pack was made from the commission's item list and balance notes. It uses original item and directional apparel art, with in-game references to the installed recon armor graphics. See [NOTICES](NOTICES.md).

## Replace textures in a local copy

Copy the Workshop mod to RimWorld's `Mods` folder and enable the local copy by itself. Copying it lets you keep your changes when Steam updates the subscribed files.

Open `Textures/Things/Apparel/TribalOutfits`. Each item has its own folder. Replace PNGs in that folder and keep their names and transparent backgrounds. `Item.png` is the icon. Worn textures end in `_south`, `_north` or `_east`. Body apparel includes the body type before the direction, as in `HiveArmor_Female_east.png`. RimWorld can mirror the east view for the west view.

Steel Monsters use vanilla recon graphics until you copy `Templates/SteelMonstersTextures.xml` into `Patches`. Put your artwork in the `MechArmor` and `MechHelmet` folders in place of the white templates, then restart RimWorld. For later edits, just replace the PNGs.

To keep the shaman helmet's horns their original color when dyed, add a mask next to each worn texture. Use names such as `ShamanHelmet_southm.png`, with red for fabric that takes the dye and black for horns. Match the size and alignment of each base texture. Copy `Templates/ShamanHelmetMask.xml` into `Patches` once the masks are ready.

## Feedback and licenses

<!-- rimmods:readme feedback begin -->
Found a bug? Leave a comment here or on Boosty
<!-- rimmods:readme feedback end -->

<!-- rimmods:readme credit begin -->
Preview artwork: in-game RimWorld portraits on an illustrated landscape
<!-- rimmods:readme credit end -->

<!-- rimmods:readme ludeon begin -->
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.
<!-- rimmods:readme ludeon end -->
