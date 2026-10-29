using System.Collections.Generic;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace StarSky.GasWallTorches
{
    public class CompProperties_GasTorch : CompProperties_FireOverlay
    {
        public float heatPerSecond = 6f;
        public Vector3 flameOffset = new Vector3(0f, 0f, 0.203125f);
        public float flameScale = 0.6f;
        public Color flameColor = Color.white;
        public string flameGraphicPath = "Things/Special/Fire";
        public SoundDef soundAmbient;

        public CompProperties_GasTorch()
        {
            compClass = typeof(CompGasTorch);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            // The vanilla flicker renderer obtains size and offset through CompFireOverlayBase.
            fireSize = finalFireSize = flameScale;
            offset = flameOffset;
            // Graphic_Flicker adds both the building graphic offset and this
            // overlay offset. Use the wall torch's directional corrections.
            var vanilla = DefDatabase<ThingDef>.GetNamed("TorchWallLamp").GetCompProperties<CompProperties_FireOverlay>();
            offsetNorth = vanilla.offsetNorth;
            offsetSouth = vanilla.offsetSouth;
            offsetWest = vanilla.offsetWest;
            offsetEast = vanilla.offsetEast;
            if (soundAmbient == null)
                soundAmbient = SoundDefOf.FireBurning;
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var error in base.ConfigErrors(parentDef))
                yield return error;
            if (!parentDef.HasComp(typeof(CompGasTorchResourceTrader)))
                yield return "Gas torch needs a CompGasTorchResourceTrader.";
            if (heatPerSecond < 0f || flameScale <= 0f)
                yield return "Gas torch heat must be nonnegative and flame scale must be positive.";
        }
    }

    public class CompProperties_GasTorchResourceTrader : CompProperties_ResourceTrader
    {
        public CompProperties_GasTorchResourceTrader()
        {
            compClass = typeof(CompGasTorchResourceTrader);
            // The base trader marks every ResourceOn=false as a shortage.
            // Our trader distinguishes an intentional shutdown from missing gas.
            resourceOffOverlay = false;
        }
    }

    public class CompGasTorchResourceTrader : CompResourceTrader
    {
        private CompGasTorch torch;
        private Material resourceOffMaterial;
        public bool ResourceWarningActive { get; private set; }

        public override bool CanBeOn()
        {
            if (torch == null)
                torch = parent.GetComp<CompGasTorch>();
            return base.CanBeOn() && (torch == null || torch.AutomaticAllowed);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!Props.pipeNet.resource.offTexPath.NullOrEmpty())
                resourceOffMaterial = MaterialPool.MatFrom(Props.pipeNet.resource.offTexPath, ShaderDatabase.MetaOverlay);
            base.PostSpawnSetup(respawningAfterLoad);
            RefreshResourceOverlay();
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            RefreshResourceOverlay();
        }

        internal void RefreshResourceOverlay()
        {
            bool warning = parent.Spawned && !ResourceOn && CanBeOn();
            if (ResourceWarningActive == warning || pipeNetOverlayDrawer == null)
                return;
            pipeNetOverlayDrawer.TogglePulsing(parent, resourceOffMaterial, warning);
            ResourceWarningActive = warning;
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            pipeNetOverlayDrawer?.TogglePulsing(parent, resourceOffMaterial, false);
            ResourceWarningActive = false;
            base.PostDeSpawn(map, mode);
        }
    }

    public class CompGasTorch : CompFireOverlayBase
    {
        private bool automaticAllowed = true;
        private int pendingAutoOffTick = -1;
        private CompGasTorchResourceTrader trader;
        private CompFlickable flickable;
        private CompGlower glower;
        private Graphic flame;
        private Sustainer sound;
        private bool lastBurning;
        private bool restoringAfterLoad;

        public new CompProperties_GasTorch Props { get { return (CompProperties_GasTorch)props; } }
        public bool AutomaticAllowed { get { return !LightsOutBridge.Active || automaticAllowed; } }
        public bool ManualAllowed { get { return flickable == null || flickable.SwitchIsOn; } }
        public bool IsBurning
        {
            get
            {
                return parent.Spawned && AutomaticAllowed && ManualAllowed
                    && trader != null && trader.ResourceOn && trader.PipeNet != null;
            }
        }
        public bool SoundActive { get { return sound != null && !sound.Ended; } }
        public int PendingAutoOffTick { get { return pendingAutoOffTick; } }
        internal bool RestoringAfterLoad
        {
            get
            {
                // Map room/component initialization and deferred load callbacks run
                // after Building.SpawnSetup. Keep the saved state through all of them.
                // A genuine command after loading, including while paused, is accepted
                // immediately instead of waiting for this building's first tick.
                if (restoringAfterLoad && Scribe.mode == LoadSaveMode.Inactive
                    && !LongEventHandler.AnyEventNowOrWaiting)
                    restoringAfterLoad = false;
                return restoringAfterLoad;
            }
        }
        public float HeatPerSecondNow { get { return IsBurning ? Props.heatPerSecond : 0f; } }
        public Room LightingRoom
        {
            get
            {
                // RimWorld 1.6 attachments occupy the free cell; FacingCell points at the wall.
                return parent.Spawned && parent.Map.regionAndRoomUpdater.Enabled
                    ? parent.Position.GetRoom(parent.Map) : null;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            restoringAfterLoad = respawningAfterLoad && LightsOutBridge.Active;
            trader = parent.GetComp<CompGasTorchResourceTrader>();
            flickable = parent.GetComp<CompFlickable>();
            glower = parent.GetComp<CompGlower>();
            if (!LightsOutBridge.Active)
            {
                automaticAllowed = true;
                pendingAutoOffTick = -1;
            }
            RefreshState();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref automaticAllowed, "gasTorchAutomaticAllowed", true);
            Scribe_Values.Look(ref pendingAutoOffTick, "gasTorchPendingAutoOffTick", -1);
        }

        // Match LightsOut's native delay contract while retaining it across saves.
        public void SetAutomaticState(int? ticksRemaining)
        {
            if (ticksRemaining > 0)
            {
                // Room reconstruction may repeat a delayed shutoff. It must neither
                // reignite an extinguished torch nor extend an existing deadline.
                if (automaticAllowed)
                {
                    int deadline = GenTicks.TicksGame + ticksRemaining.Value;
                    pendingAutoOffTick = pendingAutoOffTick >= 0
                        ? System.Math.Min(pendingAutoOffTick, deadline) : deadline;
                }
            }
            else
            {
                automaticAllowed = ticksRemaining == -1;
                pendingAutoOffTick = -1;
            }
            RefreshAutomaticState();
        }

        internal void RefreshAutomaticState()
        {
            LightsOutBridge.Synchronize(this);
            if (trader != null && !AutomaticAllowed)
                trader.ResourceOn = false;
            if (trader != null && trader.PipeNet != null)
                trader.PipeNet.receiversDirty = true;
            trader?.RefreshResourceOverlay();
            RefreshState();
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (trader != null && !AutomaticAllowed && trader.ResourceOn)
                trader.ResourceOn = false;
            RefreshState();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (restoringAfterLoad)
            {
                restoringAfterLoad = false;
                RefreshAutomaticState();
            }
            if (LightsOutBridge.Active && pendingAutoOffTick >= 0 && GenTicks.TicksGame >= pendingAutoOffTick)
            {
                pendingAutoOffTick = -1;
                LightsOutBridge.Reevaluate(this);
            }
            bool burning = IsBurning;
            if (burning != lastBurning)
                RefreshState();
            if (!burning)
                return;
            sound?.Maintain();
            if (parent.IsHashIntervalTick(60) && Props.heatPerSecond > 0f)
                GenTemperature.PushHeat(parent.Position, parent.Map, Props.heatPerSecond);
        }

        private void RefreshState()
        {
            bool burning = IsBurning;
            lastBurning = burning;
            if (parent.Spawned)
                glower?.UpdateLit(parent.Map);
            if (burning)
            {
                if ((sound == null || sound.Ended) && !Props.soundAmbient.NullOrUndefined())
                    sound = Props.soundAmbient.TrySpawnSustainer(SoundInfo.InMap(parent, MaintenanceType.PerTick));
            }
            else
                EndSound();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!IsBurning)
                return;
            if (flame == null)
            {
                // Darklight has its own cyan texture and post-light shader. Tinting
                // the orange fire texture cyan would produce a green flame instead.
                flame = glower != null && DarklightUtility.IsDarklight(glower.Props.glowColor.ToColor)
                    ? CompDarklightOverlay.DarklightGraphic
                    : GraphicDatabase.Get<Graphic_Flicker>(Props.flameGraphicPath,
                        ShaderDatabase.Transparent, Vector2.one, Props.flameColor);
            }
            Vector3 position = parent.DrawPos;
            position.y += 0.03658537f;
            // Graphic_Flicker adds the bracket's rotational draw offset and our screen-up offset.
            flame.Draw(position, parent.Rotation, parent);
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            EndSound();
            lastBurning = false;
            restoringAfterLoad = false;
            base.PostDeSpawn(map, mode);
        }

        private void EndSound()
        {
            sound?.End();
            sound = null;
        }
    }
}
