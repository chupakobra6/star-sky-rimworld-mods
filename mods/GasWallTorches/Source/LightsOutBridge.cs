using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace StarSky.GasWallTorches
{
    [StaticConstructorOnStartup]
    public static class LightsOutBridge
    {
        private static Func<Room, Pawn, bool> shouldTurnOff;
        private static Func<ThingWithComps, bool> keepOn;
        private static Action<ThingWithComps, int?> disable;
        private static Action<ThingWithComps> enable;
        private static Func<Dictionary<ThingWithComps, bool?>> buildingStatus;
        public static bool Active { get; private set; }

        static LightsOutBridge()
        {
            var harmony = new Harmony("starsky.rimworld.gaswalltorches");
            harmony.Patch(AccessTools.PropertyGetter(typeof(CompGlower), "ShouldBeLitNow"),
                postfix: new HarmonyMethod(typeof(LightsOutBridge), nameof(GlowerPostfix)));
            Type lights = AccessTools.TypeByName("LightsOut.Common.Lights");
            Type rooms = AccessTools.TypeByName("LightsOut.Common.Rooms");
            Type resources = AccessTools.TypeByName("LightsOut.Common.Resources");
            if (lights == null || rooms == null || resources == null)
                return;
            MethodInfo canBeLight = AccessTools.Method(lights, "CanBeLight", new[] { typeof(ThingWithComps) });
            MethodInfo getRoom = AccessTools.Method(rooms, "GetRoom", new[] { typeof(ThingWithComps) });
            MethodInfo setTicks = AccessTools.Method(resources, "SetTicksRemaining", new[] { typeof(ThingWithComps), typeof(int?) });
            MethodInfo should = AccessTools.Method(lights, "ShouldTurnOffAllLights", new[] { typeof(Room), typeof(Pawn) });
            MethodInfo keep = AccessTools.Method(lights, "KeepLightOn", new[] { typeof(ThingWithComps) });
            MethodInfo disableMethod = AccessTools.Method(lights, "DisableLight", new[] { typeof(ThingWithComps), typeof(int?) });
            MethodInfo enableMethod = AccessTools.Method(lights, "EnableLight", new[] { typeof(ThingWithComps) });
            MethodInfo status = AccessTools.PropertyGetter(resources, "BuildingStatus");
            if (canBeLight == null || getRoom == null || setTicks == null || should == null
                || keep == null || disableMethod == null || enableMethod == null || status == null)
            {
                Log.Error("[GasWallTorches] Unsupported LightsOut API. Gas torches retain manual control.");
                return;
            }
            shouldTurnOff = (Func<Room, Pawn, bool>)Delegate.CreateDelegate(typeof(Func<Room, Pawn, bool>), should);
            keepOn = (Func<ThingWithComps, bool>)Delegate.CreateDelegate(typeof(Func<ThingWithComps, bool>), keep);
            disable = (Action<ThingWithComps, int?>)Delegate.CreateDelegate(typeof(Action<ThingWithComps, int?>), disableMethod);
            enable = (Action<ThingWithComps>)Delegate.CreateDelegate(typeof(Action<ThingWithComps>), enableMethod);
            buildingStatus = (Func<Dictionary<ThingWithComps, bool?>>)Delegate.CreateDelegate(
                typeof(Func<Dictionary<ThingWithComps, bool?>>), status);
            harmony.Patch(canBeLight, prefix: new HarmonyMethod(typeof(LightsOutBridge), nameof(CanBeLightPrefix)));
            harmony.Patch(getRoom, postfix: new HarmonyMethod(typeof(LightsOutBridge), nameof(RoomPostfix)));
            harmony.Patch(setTicks, prefix: new HarmonyMethod(typeof(LightsOutBridge), nameof(SetTicksPrefix)));
            harmony.Patch(enableMethod, postfix: new HarmonyMethod(typeof(LightsOutBridge), nameof(EnableLightPostfix)));
            Active = true;
        }

        internal static void Synchronize(CompGasTorch torch)
        {
            if (Active)
                buildingStatus()[torch.parent] = torch.AutomaticAllowed;
        }

        public static void Reevaluate(CompGasTorch torch)
        {
            Room room = torch.LightingRoom;
            if (!Active || room == null)
                return;
            if (!room.OutdoorsForWork && !keepOn(torch.parent) && shouldTurnOff(room, null))
                disable(torch.parent, 0);
            else
                enable(torch.parent);
        }

        private static bool CanBeLightPrefix(ThingWithComps __0, ref bool __result)
        {
            if (__0?.GetComp<CompGasTorch>() == null)
                return true;
            __result = true;
            return false;
        }

        private static void RoomPostfix(ThingWithComps __0, ref Room __result)
        {
            CompGasTorch torch = __0?.GetComp<CompGasTorch>();
            if (torch != null)
                __result = torch.LightingRoom;
        }

        private static bool SetTicksPrefix(ThingWithComps __0, int? __1)
        {
            CompGasTorch torch = __0?.GetComp<CompGasTorch>();
            if (torch == null)
                return true;
            if (torch.RestoringAfterLoad)
                Synchronize(torch);
            else
                torch.SetAutomaticState(__1);
            // Our saved deadline replaces LightsOut's transient queue for this
            // torch. Otherwise a cancelled queue item can later shut it off early.
            return false;
        }

        private static void EnableLightPostfix(ThingWithComps __0)
        {
            CompGasTorch torch = __0?.GetComp<CompGasTorch>();
            // LightsOut returns early when the light is already on, including
            // while a delay is pending. A real enable must still cancel that delay.
            if (torch != null && !torch.RestoringAfterLoad)
                torch.SetAutomaticState(-1);
        }

        private static void GlowerPostfix(CompGlower __instance, ref bool __result)
        {
            if (__result && __instance.parent.GetComp<CompGasTorch>() is CompGasTorch torch)
                __result = torch.IsBurning;
        }
    }
}
