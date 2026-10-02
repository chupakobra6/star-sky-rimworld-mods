// Generated into both independent packages. Canonical source: compatibility/house-van-illa.
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace StarSky.HouseVanIllaFixes
{
    public sealed class HouseVanIllaBootstrap : Mod
    {
        public HouseVanIllaBootstrap(ModContentPack content) : base(content)
        {
            var original = LoadedModManager.RunningMods.FirstOrDefault(m => m.PackageId.Equals("addie.housevanilla", StringComparison.OrdinalIgnoreCase));
            if (original == null || !RepairFolders(original.ModMetaData.loadFolders)) return;
            // Mod constructors run after initial assets, but before XML loading.
            // XML needs the corrected folders immediately. Unity resources must
            // wait until the asynchronous loading event finishes on the main thread.
            RefreshFolders(
                () => AccessTools.Method(typeof(ModContentPack), "InitLoadFolders").Invoke(original, null),
                () => (List<string>)AccessTools.Field(typeof(ModContentPack), "foldersToLoadDescendingOrder").GetValue(original),
                LongEventHandler.ExecuteWhenFinished,
                () => original.GetContentHolder<Texture2D>().ReloadAll(false));
        }

        internal static void RefreshFolders(Action initialize, Func<IEnumerable<string>> roots, Action<Action> defer, Action reloadTextures)
        {
            var before = roots().ToArray();
            initialize();
            if (!before.SequenceEqual(roots())) defer(reloadTextures);
        }

        internal static bool RepairFolders(ModLoadFolders metadata)
        {
            if (metadata == null) return false;
            bool changed = false;
            var folders = metadata.FoldersForVersion("1.6");
            for (int i = 0; i < folders.Count; i++)
            {
                var folder = folders[i];
                var field = AccessTools.Field(typeof(LoadFolder), "requiredAnyOfPackageIds");
                var required = (List<string>)field.GetValue(folder);
                if (folder.folderName != "1.6/Mods/Ideology" || required == null || required.Count != 1
                    || !required[0].Equals("ludeon.rimworld.royalty", StringComparison.OrdinalIgnoreCase)) continue;
                // LoadFolder caches its hash in the constructor. Replace it rather
                // than mutate the gate and leave that immutable hash inconsistent.
                folders[i] = new LoadFolder(folder.folderName, new List<string> { "ludeon.rimworld.ideology" },
                    (List<string>)AccessTools.Field(typeof(LoadFolder), "requiredAllOfPackageIds").GetValue(folder),
                    (List<string>)AccessTools.Field(typeof(LoadFolder), "disallowedAnyOfPackageIds").GetValue(folder));
                changed = true;
            }
            return changed;
        }
    }
}
