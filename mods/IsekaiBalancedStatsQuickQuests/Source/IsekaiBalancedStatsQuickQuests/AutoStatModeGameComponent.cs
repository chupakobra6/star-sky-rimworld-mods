using System.Collections.Generic;
using IsekaiLeveling;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    public enum AutoStatMode
    {
        Balanced = 0,
        ClassWeighted = 1
    }

    public enum PetAutoStatMode
    {
        PhysicalWeighted = 0,
        Balanced = 1
    }

    public class PawnAutoStatProgress : IExposable
    {
        public List<int> AutomaticAllocations = NewAllocationList();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref AutomaticAllocations, "automaticAllocations", LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                NormalizeAllocations();
        }

        public int[] GetAllocationSnapshot()
        {
            NormalizeAllocations();
            return AutomaticAllocations.ToArray();
        }

        public void RecordAllocation(int statIndex)
        {
            NormalizeAllocations();
            if (statIndex >= 0 && statIndex < AutomaticAllocations.Count)
                AutomaticAllocations[statIndex]++;
        }

        public void ResetAllocations()
        {
            AutomaticAllocations = NewAllocationList();
        }

        private void NormalizeAllocations()
        {
            if (AutomaticAllocations == null)
                AutomaticAllocations = NewAllocationList();

            while (AutomaticAllocations.Count < 6)
                AutomaticAllocations.Add(0);
            if (AutomaticAllocations.Count > 6)
                AutomaticAllocations.RemoveRange(6, AutomaticAllocations.Count - 6);

            for (int i = 0; i < AutomaticAllocations.Count; i++)
                if (AutomaticAllocations[i] < 0)
                    AutomaticAllocations[i] = 0;
        }

        private static List<int> NewAllocationList()
        {
            return new List<int> { 0, 0, 0, 0, 0, 0 };
        }
    }

    public class PetAutoStatState : IExposable
    {
        public bool Enabled = true;
        public PetAutoStatMode Mode = PetAutoStatMode.PhysicalWeighted;
        public List<int> AutomaticAllocations = NewAllocationList();

        public void ExposeData()
        {
            Scribe_Values.Look(ref Enabled, "enabled", true);
            Scribe_Values.Look(ref Mode, "mode", PetAutoStatMode.PhysicalWeighted);
            Scribe_Collections.Look(ref AutomaticAllocations, "automaticAllocations", LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                NormalizeAllocations();
        }

        public int[] GetAllocationSnapshot()
        {
            NormalizeAllocations();
            return AutomaticAllocations.ToArray();
        }

        public void RecordAllocation(int statIndex)
        {
            NormalizeAllocations();
            if (statIndex >= 0 && statIndex < AutomaticAllocations.Count)
                AutomaticAllocations[statIndex]++;
        }

        public void ResetAllocations()
        {
            AutomaticAllocations = NewAllocationList();
        }

        private void NormalizeAllocations()
        {
            if (AutomaticAllocations == null)
                AutomaticAllocations = NewAllocationList();

            while (AutomaticAllocations.Count < 6)
                AutomaticAllocations.Add(0);
            if (AutomaticAllocations.Count > 6)
                AutomaticAllocations.RemoveRange(6, AutomaticAllocations.Count - 6);

            for (int i = 0; i < AutomaticAllocations.Count; i++)
                if (AutomaticAllocations[i] < 0)
                    AutomaticAllocations[i] = 0;
        }

        private static List<int> NewAllocationList()
        {
            return new List<int> { 0, 0, 0, 0, 0, 0 };
        }
    }

    public class AutoStatModeGameComponent : GameComponent
    {
        private Dictionary<int, AutoStatMode> pawnModes = new Dictionary<int, AutoStatMode>();
        private List<int> pawnModeKeys;
        private List<AutoStatMode> pawnModeValues;
        private Dictionary<int, PawnAutoStatProgress> pawnProgress = new Dictionary<int, PawnAutoStatProgress>();
        private List<int> pawnProgressKeys;
        private List<PawnAutoStatProgress> pawnProgressValues;
        private Dictionary<int, PetAutoStatState> petStates = new Dictionary<int, PetAutoStatState>();
        private List<int> petStateKeys;
        private List<PetAutoStatState> petStateValues;

        public AutoStatModeGameComponent(Game game)
        {
        }

        public static AutoStatModeGameComponent Get()
        {
            return Current.Game?.GetComponent<AutoStatModeGameComponent>();
        }

        public AutoStatMode GetMode(Pawn pawn)
        {
            if (pawn != null && pawnModes.TryGetValue(pawn.thingIDNumber, out AutoStatMode mode))
                return mode;

            AutoStatMode defaultMode = IsekaiBalancedStatsQuickQuestsMod.Settings.DefaultPawnMode;
            if (defaultMode != AutoStatMode.ClassWeighted || pawn == null)
                return AutoStatMode.Balanced;

            IsekaiComponent component = pawn.GetComp<IsekaiComponent>();
            return string.IsNullOrEmpty(component?.passiveTree?.assignedTree)
                ? AutoStatMode.Balanced
                : AutoStatMode.ClassWeighted;
        }

        public void SetMode(Pawn pawn, AutoStatMode mode)
        {
            if (pawn == null)
                return;

            AutoStatMode previous = GetMode(pawn);
            pawnModes[pawn.thingIDNumber] = mode;
            if (previous != mode)
                GetPawnAutoStatProgress(pawn)?.ResetAllocations();
        }

        public PawnAutoStatProgress GetPawnAutoStatProgress(Pawn pawn)
        {
            if (pawn == null)
                return null;

            if (!pawnProgress.TryGetValue(pawn.thingIDNumber, out PawnAutoStatProgress progress)
                || progress == null)
            {
                progress = new PawnAutoStatProgress();
                pawnProgress[pawn.thingIDNumber] = progress;
            }

            return progress;
        }

        public PetAutoStatState GetPetState(Pawn pawn)
        {
            if (pawn == null)
                return null;

            if (!petStates.TryGetValue(pawn.thingIDNumber, out PetAutoStatState state) || state == null)
            {
                IsekaiFixesModSettings settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
                state = new PetAutoStatState
                {
                    Enabled = settings.PetAutoEnabledByDefault,
                    Mode = settings.DefaultPetMode
                };
                petStates[pawn.thingIDNumber] = state;
            }

            return state;
        }

        public bool GetPetAutoEnabled(Pawn pawn)
        {
            return GetPetState(pawn)?.Enabled
                   ?? IsekaiBalancedStatsQuickQuestsMod.Settings.PetAutoEnabledByDefault;
        }

        public void SetPetAutoEnabled(Pawn pawn, bool enabled)
        {
            PetAutoStatState state = GetPetState(pawn);
            if (state != null)
                state.Enabled = enabled;
        }

        public PetAutoStatMode GetPetMode(Pawn pawn)
        {
            return GetPetState(pawn)?.Mode
                   ?? IsekaiBalancedStatsQuickQuestsMod.Settings.DefaultPetMode;
        }

        public void SetPetMode(Pawn pawn, PetAutoStatMode mode)
        {
            PetAutoStatState state = GetPetState(pawn);
            if (state != null)
            {
                if (state.Mode != mode)
                    state.ResetAllocations();
                state.Mode = mode;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(
                ref pawnModes,
                "isekaiAutoStatModes",
                LookMode.Value,
                LookMode.Value,
                ref pawnModeKeys,
                ref pawnModeValues);

            Scribe_Collections.Look(
                ref pawnProgress,
                "isekaiPawnAutoStatProgress",
                LookMode.Value,
                LookMode.Deep,
                ref pawnProgressKeys,
                ref pawnProgressValues);

            Scribe_Collections.Look(
                ref petStates,
                "isekaiPetAutoStatStates",
                LookMode.Value,
                LookMode.Deep,
                ref petStateKeys,
                ref petStateValues);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (pawnModes == null)
                    pawnModes = new Dictionary<int, AutoStatMode>();
                if (pawnProgress == null)
                    pawnProgress = new Dictionary<int, PawnAutoStatProgress>();
                if (petStates == null)
                    petStates = new Dictionary<int, PetAutoStatState>();
            }
        }
    }
}
