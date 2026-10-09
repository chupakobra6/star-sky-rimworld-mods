using System;
using System.Collections.Generic;
using HarmonyLib;
using MiningOutpost.Orbital.Services;
using Verse;

namespace Igor.WorksitesRussian
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        public static bool Ready { get; private set; }
        public static int CatalogFields { get; private set; }
        static Startup() { LongEventHandler.ExecuteWhenFinished(Apply); }

        private static void Apply()
        {
            var harmony=new Harmony("igor.ru.3687071198.grammar");
            WorksiteNamePatch.Install(harmony);
            WorksiteCardPatch.Install(harmony);
            WorksiteDescriptorPatch.Install(harmony);
            WorksiteFirstQuestNamePatch.Install(harmony);
            WorksiteSenderPatch.Install(harmony);
            WorksiteWorkerPatch.Install(harmony);
            WorksiteVolunteerPatch.Install(harmony);
            WorksiteParleyPatch.Install(harmony);
            WorksiteOrbitalSocialPatch.Install(harmony);
            WorksiteAttackerNamePatch.Install(harmony);
            WorksiteDefenseTextPatch.Install(harmony);
            WorksiteOfferPanicPatch.Install(harmony);
            WorksiteCheckPatch.Install(harmony);
            WorksiteServicePatch.Install(harmony);
            WorksiteEventPatch.Install(harmony);
            WorksiteCombatPatch.Install(harmony);
            WorksitePokerPatch.Install(harmony);
            WorksiteDemandPatch.Install(harmony);
            WorksiteMenuText.Install(harmony);
            WorksiteGamblePatch.Install(harmony);
            if(LanguageDatabase.activeLanguage.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase))
            {
                var jobs=(IEnumerable<OppJob>)AccessTools.Field("MiningOutpost.Orbital.Services.OpportunityCatalog:All").GetValue(null);
                CatalogFields=WorksiteCatalogText.Apply(jobs,TranslateCatalogField);
                if(CatalogFields!=837)throw new InvalidOperationException("Worksites catalog fields changed: "+CatalogFields);
            }
            Ready=true;
            Log.Message("[Igor Worksites RU] Grammar, display tables and "+CatalogFields+" catalog fields initialized.");
        }

        private static string TranslateCatalogField(string key)
        {
            if(!key.CanTranslate())throw new InvalidOperationException("Worksites catalog translation missing: "+key);
            return key.Translate().Resolve();
        }
    }
}
