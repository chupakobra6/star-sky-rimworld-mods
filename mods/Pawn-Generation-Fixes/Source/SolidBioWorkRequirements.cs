using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.PawnGenerationFixes
{
    public static class SolidBioWorkRequirements
    {
        public const string Owner="igor.pawngeneration.fixes";
        public static bool Installed { get; private set; }
        public static MethodInfo Target { get; private set; }
        private static FieldInfo capture,kind;
        public static void Install(Harmony harmony)
        {
            try
            {
                var patchType=AccessTools.TypeByName("AlienRace.HarmonyPatches");
                var closure=AccessTools.TypeByName("AlienRace.HarmonyPatches+<>c__DisplayClass237_1");
                var root=AccessTools.TypeByName("AlienRace.HarmonyPatches+<>c__DisplayClass237_0");
                var producer=AccessTools.Method(patchType,"TryGetRandomUnusedSolidBioForPostfix");
                Target=AccessTools.DeclaredMethod(closure,"<TryGetRandomUnusedSolidBioForPostfix>b__1",new[]{typeof(PawnBio)});
                capture=AccessTools.DeclaredField(closure,"CS$<>8__locals1");
                kind=AccessTools.DeclaredField(root,"kind");
                ValidateTarget(producer,Target,capture,kind);
                harmony.Patch(Target,postfix:new HarmonyMethod(typeof(SolidBioWorkRequirements),nameof(Filter)));
                Installed=Harmony.GetPatchInfo(Target)?.Postfixes.Any(p=>p.owner==Owner && p.PatchMethod==AccessTools.Method(typeof(SolidBioWorkRequirements),nameof(Filter)))==true;
                if(!Installed) throw new InvalidOperationException("guard was not registered");
            }
            catch(Exception error)
            {
                Installed=false;
                Log.Error("Pawn Generation Fixes: HAR solid-bio guard NOT INSTALLED; current predicate structure is unsupported: "+error.Message);
            }
        }
        public static void ValidateTarget(MethodInfo producer,MethodInfo target,FieldInfo captureField,FieldInfo kindField)
        {
            if(producer==null || target==null || captureField==null || kindField==null
                || target.IsStatic || target.IsGenericMethod || target.ReturnType!=typeof(bool)
                || target.GetParameters().Length!=1 || target.GetParameters()[0].ParameterType!=typeof(PawnBio)
                || captureField.IsStatic || kindField.IsStatic || captureField.FieldType!=kindField.DeclaringType
                || captureField.DeclaringType!=target.DeclaringType || kindField.FieldType!=typeof(PawnKindDef))
                throw new InvalidOperationException("expected one PawnBio predicate with the reviewed kind capture");
            var producerCode=PatchProcessor.GetOriginalInstructions(producer).ToArray();
            var predicateCode=PatchProcessor.GetOriginalInstructions(target).ToArray();
            if(producerCode.Count(i=>i.opcode==OpCodes.Ldftn && Equals(i.operand,target))!=1
                || !predicateCode.Any(i=>i.opcode==OpCodes.Ldfld && Equals(i.operand,captureField))
                || !predicateCode.Any(i=>i.opcode==OpCodes.Ldfld && Equals(i.operand,kindField))
                || predicateCode.Any(i=>i.opcode==OpCodes.Ldfld && Equals(i.operand,AccessTools.Field(typeof(PawnKindDef),"requiredWorkTags"))))
                throw new InvalidOperationException("HAR predicate producer or missing vanilla work-tag branch changed");
        }
        public static void Filter(object __instance,PawnBio __0,ref bool __result)
        {
            if(!__result) return;
            var pawnKind=(PawnKindDef)kind.GetValue(capture.GetValue(__instance));
            var required=pawnKind.requiredWorkTags;
            var disabled=(__0.childhood?.workDisables ?? WorkTags.None)|(__0.adulthood?.workDisables ?? WorkTags.None);
            __result=(required&disabled)==WorkTags.None;
        }
    }
}
