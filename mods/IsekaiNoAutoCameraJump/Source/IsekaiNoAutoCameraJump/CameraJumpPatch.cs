using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace IsekaiNoAutoCameraJump
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        private const string HarmonyId = "chupakobra6.isekai.noautohuntcamerajump";

        static Bootstrap()
        {
            try
            {
                HuntCameraJumpGuard.Apply(new Harmony(HarmonyId));
            }
            catch (Exception exception)
            {
                Log.Error($"[ISEKAI No Camera Jump] Не удалось установить патч: {exception}");
            }
        }
    }

    internal static class HuntCameraJumpGuard
    {
        [ThreadStatic]
        private static int suppressionDepth;

        private static readonly string[] HuntSpawnMethods =
        {
            "IsekaiLeveling.Quests.QuestPart_IsekaiLocalHunt:SpawnCreatureOnMap",
            "IsekaiCreatures.Quests.QuestPart_IsekaiCreatureQuestSpawn:SpawnPackOnMap"
        };

        public static void Apply(Harmony harmony)
        {
            MethodInfo enter = AccessTools.Method(typeof(HuntCameraJumpGuard), nameof(EnterHuntSpawn));
            MethodInfo exit = AccessTools.Method(typeof(HuntCameraJumpGuard), nameof(ExitHuntSpawn));
            MethodInfo allow = AccessTools.Method(typeof(HuntCameraJumpGuard), nameof(AllowCameraJump));

            int guardedSpawnMethods = 0;
            foreach (string target in HuntSpawnMethods)
            {
                int separator = target.LastIndexOf(':');
                string typeName = target.Substring(0, separator);
                string methodName = target.Substring(separator + 1);
                Type type = AccessTools.TypeByName(typeName);
                MethodInfo method = type == null ? null : AccessTools.DeclaredMethod(type, methodName);

                if (method == null)
                {
                    Log.Warning($"[ISEKAI No Camera Jump] Цель не найдена: {target}. Возможно, соответствующий аддон не установлен или обновился.");
                    continue;
                }

                harmony.Patch(
                    method,
                    prefix: new HarmonyMethod(enter),
                    finalizer: new HarmonyMethod(exit));
                guardedSpawnMethods++;
            }

            List<MethodInfo> cameraMethods = AccessTools.GetDeclaredMethods(typeof(CameraJumper))
                .Where(method => method.IsStatic
                    && (method.Name == "TryJump" || method.Name == "TryJumpAndSelect"))
                .ToList();

            foreach (MethodInfo cameraMethod in cameraMethods)
                harmony.Patch(cameraMethod, prefix: new HarmonyMethod(allow));

            if (guardedSpawnMethods == 0 || cameraMethods.Count == 0)
            {
                Log.Error($"[ISEKAI No Camera Jump] Патч неполный: целей охоты {guardedSpawnMethods}, методов камеры {cameraMethods.Count}.");
                return;
            }

            Log.Message($"[ISEKAI No Camera Jump] Активен: охраняемых методов охоты {guardedSpawnMethods}, методов камеры {cameraMethods.Count}.");
        }

        private static void EnterHuntSpawn(MethodBase __originalMethod, out bool __state)
        {
            __state = ShouldSuppress(__originalMethod);
            if (__state)
                suppressionDepth++;
        }

        private static Exception ExitHuntSpawn(bool __state, Exception __exception)
        {
            if (__state && suppressionDepth > 0)
                suppressionDepth--;

            return __exception;
        }

        private static bool ShouldSuppress(MethodBase method)
        {
            string typeName = method?.DeclaringType?.FullName;
            CameraJumpModSettings settings = IsekaiNoAutoCameraJumpMod.Settings;

            if (typeName == "IsekaiLeveling.Quests.QuestPart_IsekaiLocalHunt")
                return settings.SuppressIsekaiHunts;

            if (typeName == "IsekaiCreatures.Quests.QuestPart_IsekaiCreatureQuestSpawn")
                return settings.SuppressCreatureHunts;

            return false;
        }

        private static bool AllowCameraJump()
        {
            return suppressionDepth == 0;
        }
    }
}
