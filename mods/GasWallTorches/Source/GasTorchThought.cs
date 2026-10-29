using RimWorld;
using Verse;

namespace StarSky.GasWallTorches
{
    public class GasTorchThoughtExtension : DefModExtension
    {
        public ThingDef torch;
        public GeneDef gene;
        public float radius = 7f;
    }

    public class ThoughtWorker_GasTorch : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            var settings = def.GetModExtension<GasTorchThoughtExtension>();
            if (settings == null || settings.torch == null || settings.gene == null
                || !pawn.Spawned || pawn.genes == null || !pawn.genes.HasActiveGene(settings.gene))
                return false;
            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, settings.radius, true))
            {
                if (thing.def == settings.torch && thing.TryGetComp<CompGasTorch>()?.IsBurning == true)
                    return true;
            }
            return false;
        }
    }
}
