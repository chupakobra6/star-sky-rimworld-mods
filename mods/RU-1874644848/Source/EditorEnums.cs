using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.CharacterEditorRussian
{
    [StaticConstructorOnStartup]
    public static class EnumStartup
    {
        static EnumStartup() { LongEventHandler.ExecuteWhenFinished(EditorEnums.Install); }
    }

    public static class EditorEnums
    {
        static readonly MethodInfo GetName=AccessTools.Method(typeof(Enum),nameof(Enum.GetName),new[]{typeof(Type),typeof(object)});
        public static void Install()
        {
            var harmony=new Harmony("igor.ru.1874644848.enums");
            foreach(string method in new[]{
                "CharacterEditor.FLabel+<>c:<get_GeneticBodytype>b__51_0",
                "CharacterEditor.FLabel+<>c:<get_EndogeneCat>b__53_0",
                "CharacterEditor.FLabel+<>c:<get_Tradeability>b__55_0",
                "CharacterEditor.FLabel+<>c:<get_TechLevel>b__57_0",
                "CharacterEditor.FLabel+<>c:<get_GasType>b__59_0",
                "CharacterEditor.PawnxTool:GetMutantRotStage",
                "CharacterEditor.SZWidgets:GetFormattedValue",
                "CharacterEditor.FLabel:GetFormattedValue",
                "CharacterEditor.Listing_X:GetFormattedValue"})
                harmony.Patch(AccessTools.Method(method),transpiler:new HarmonyMethod(typeof(EditorEnums),nameof(TranslateEnumName)));
            foreach(string method in new[]{"CharacterEditor.DialogObjects:DrawCustomFilter","CharacterEditor.DialogObjects:DrawRangedSmall"})
                harmony.Patch(AccessTools.Method(method),transpiler:new HarmonyMethod(typeof(EditorEnums),nameof(TranslateCategory)));
        }

        public static string DisplayName(Type type,object value)
        {
            string name=Enum.GetName(type,value);
            return !EditorText.Russian || name==null ? name : EditorText.Value("Enum_"+type.FullName.Replace('.','_')+"_"+name);
        }

        public static string Category<T>(T value)
        {
            string name=value.ToString();
            if(name=="None")return (string)AccessTools.Field("CharacterEditor.Label:ALL").GetValue(null);
            return DisplayName(typeof(T),value);
        }

        public static IEnumerable<CodeInstruction> TranslateEnumName(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.Calls(GetName)) { instruction.operand=AccessTools.Method(typeof(EditorEnums),nameof(DisplayName));changed++; }
                yield return instruction;
            }
            if(changed!=1)throw new InvalidOperationException("Character Editor enum display changed: "+__originalMethod+" / "+changed);
        }

        public static IEnumerable<CodeInstruction> TranslateCategory(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                var method=instruction.operand as MethodInfo;
                if(method!=null && method.IsGenericMethod && method.DeclaringType.FullName=="CharacterEditor.FLabel" && method.Name=="EnumNameAndAll")
                {
                    instruction.operand=AccessTools.Method(typeof(EditorEnums),nameof(Category)).MakeGenericMethod(method.GetGenericArguments());
                    changed++;
                }
                yield return instruction;
            }
            if(changed!=2)throw new InvalidOperationException("Character Editor category selectors changed: "+__originalMethod+" / "+changed);
        }
    }
}
