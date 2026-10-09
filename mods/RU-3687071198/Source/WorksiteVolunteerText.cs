using System;
using System.Collections.Generic;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteVolunteerText
    {
        private static readonly Dictionary<string,string> Templates = new Dictionary<string,string>
        {
            {"{0} needs {1} silver to settle old debts before leaving this platform.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы расплатиться со старыми долгами перед уходом с платформы."},
            {"{0} owes {1} silver to a local {2}, and is asking you to pay it off.", "{PAWN_labelShort} просит вас погасить долг перед местным {CRIMINAL_INS}: {SILVER} серебра."},
            {"{0} is asking for {1} silver up front before they join you.", "{PAWN_labelShort} просит {SILVER} серебра авансом, прежде чем присоединиться к вам."},
            {"{0} is asking for {1} silver to clear a debt with a smuggler.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы расплатиться с контрабандистом."},
            {"{0} is asking you to cover their bar tab of {1} silver before they leave the station.", "{PAWN_labelShort} просит оплатить барный счёт на {SILVER} серебра перед уходом со станции."},
            {"{0} is fleeing a {2} from a nearby rimworld. They need {1} silver to cover their tracks.", "{PAWN_labelShort} скрывается от {CRIMINAL_GEN} с соседнего окраинного мира. Чтобы замести следы, нужно {SILVER} серебра."},
            {"{0} lost everything at the poker table and will join you for {1} silver in advance.", "{PAWN_labelShort}: всё проиграно за покерным столом. За аванс в {SILVER} серебра этот человек присоединится к вам."},
            {"{0} swears their last contract paid in vacsuit air. They want {1} silver before they join you.", "{PAWN_labelShort} клянётся, что по последнему контракту расплатились воздухом для скафандра. Теперь требуется {SILVER} серебра до присоединения к вам."},
            {"{0} has a sibling on a distant mining station and wants to send them {1} silver before they leave.", "{PAWN_labelShort} хочет отправить {SILVER} серебра брату или сестре на далёкую шахтёрскую станцию, прежде чем уйти."},
            {"{0} relies on a local fixer for their contracts. They'd like {1} silver before they cut them off.", "{PAWN_labelShort} получает контракты через местного решалу и хочет {SILVER} серебра, прежде чем порвать с ним."},
            {"{0} is asking for {1} silver to pay a {2} for equipment they damaged.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы заплатить {CRIMINAL_DAT} за повреждённое оборудование."},
            {"{0} is asking for {1} silver to pay a {2} they ran with on a mission that went sour", "{PAWN_labelShort} просит {SILVER} серебра для расчёта с {CRIMINAL_INS} после их совместной миссии, которая пошла наперекосяк."},
            {"{0} needs {1} silver to pay a fine for discharging a firearm in a sealed corridor, else they get jettisoned.", "{PAWN_labelShort} просит {SILVER} серебра на штраф за стрельбу в герметичном коридоре — иначе вышвырнут в космос."},
            {"{0} says they need {1} silver or a {2} is going to feed them to the trash compactor.", "{PAWN_labelShort} говорит, что нужно {SILVER} серебра, иначе {CRIMINAL_NOM} скормит {PAWN_gender ? его : её : этого человека} мусорному прессу."},
            {"{0} was on a corvette that came back without three of its crew; {1} silver and they ship out.", "{PAWN_labelShort} — бывший член экипажа корвета, вернувшегося без троих своих людей. За {SILVER} серебра отправится с вами."},
            {"{0} says {1} silver will buy them out of a bounty that a {2} just approved for their capture.", "{PAWN_labelShort} говорит, что {SILVER} серебра хватит, чтобы снять награду за {PAWN_possessive} поимку, недавно назначенную {CRIMINAL_INS}."},
            {"{0} needs {1} to square a loan with a fixer who lended them guns on credit.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы расплатиться с решалой, который дал оружие в кредит."},
            {"{0} managed to crash a rented loader mech. They need {1} silver to appease the furious {2} who owns it.", "{PAWN_labelShort} {PAWN_gender ? ухитрился : ухитрилась : умудряется} разбить арендованного погрузочного механоида. Нужно {SILVER} серебра, чтобы умилостивить его разъярённого владельца — {CRIMINAL_GEN}."},
            {"{0} is asking for {1} silver to clear their name after a hydroponics heist went completely sideways.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы очистить своё имя после совершенно проваленного ограбления гидропонной фермы."},
            {"{0} says they need {1} silver to bribe station security, otherwise a {2} is going to have them locked in a cargo crate.", "{PAWN_labelShort} говорит, что нужно {SILVER} серебра на взятку службе безопасности станции — иначе {CRIMINAL_NOM} велит запереть {PAWN_gender ? его : её : этого человека} в грузовом ящике."},
            {"{0} needs {1} silver to clear their tab with a local {2} who has been sheltering them from the law.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы расплатиться с местным {CRIMINAL_INS}, который скрывал {PAWN_gender ? его : её : этого человека} от закона."},
            {"{0} claims they need {1} silver to pay off a {2} who is threatening to leak their location to pirates.", "{PAWN_labelShort} утверждает, что нужно {SILVER} серебра, чтобы откупиться от {CRIMINAL_GEN}, который грозится выдать {PAWN_possessive} местоположение пиратам."},
            {"{0} is asking for {1} silver to send back to their parents on a war-torn rimworld.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы отправить их родителям на охваченный войной окраинный мир."},
            {"{0} will join your crew for {1} silver.", "{PAWN_labelShort} присоединится к вашему экипажу за {SILVER} серебра."},
            {"{0} is available for {1} silver.", "{PAWN_labelShort}: цена найма — {SILVER} серебра."},
            {"{0} will work for you for {1} silver.", "{PAWN_labelShort} будет работать на вас за {SILVER} серебра."},
            {"{0} needs {1} silver to pay the local {2} for the years they spent in a cryptosleep casket in their basement.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы заплатить местному {CRIMINAL_DAT} за годы в саркофаге криптосна в его подвале."},
            {"{0} is asking for {1} silver to settle a debt with a local {2} who took a bullet for them during a bad mission.", "{PAWN_labelShort} просит {SILVER} серебра, чтобы вернуть долг местному {CRIMINAL_DAT}, который принял предназначенную {PAWN_objective} пулю на неудачном задании."},
            {"{0} will gladly join you, provided you can match their competitive rate of {1} silver upfront.", "{PAWN_labelShort} с радостью присоединится к вам, если вы согласны на «вполне конкурентную» цену: {SILVER} серебра авансом."},
            {"{0} says they'll join you if you give them {1} silver to gamble away right now.", "{PAWN_labelShort} обещает присоединиться, если вы дадите {SILVER} серебра, которые можно тут же спустить за игровым столом."},
            {"{0} says they need {1} silver to pay the medical bills of an old crewmate who is dying in a corporate ward.", "{PAWN_labelShort} говорит, что нужно {SILVER} серебра на лечение старого товарища по экипажу, который умирает в корпоративной больнице."},
            {"{0} was a navigator who lost a star chart in a bar fight. They need {1} silver to settle it.", "{PAWN_labelShort} {PAWN_gender ? служил штурманом и потерял : служила штурманом и потеряла : после службы штурманом теряет} звёздную карту в барной драке. Чтобы уладить дело, нужно {SILVER} серебра."},
        };
        private static readonly Dictionary<string,string[]> Roles = new Dictionary<string,string[]>
        {
            {"loan shark", new[]{"ростовщик", "ростовщика", "ростовщику", "ростовщиком"}},
            {"crime lord", new[]{"криминальный босс", "криминального босса", "криминальному боссу", "криминальным боссом"}},
            {"smuggler", new[]{"контрабандист", "контрабандиста", "контрабандисту", "контрабандистом"}},
            {"fence", new[]{"скупщик краденого", "скупщика краденого", "скупщику краденого", "скупщиком краденого"}},
            {"fixer", new[]{"решала", "решалы", "решале", "решалой"}},
            {"dock boss", new[]{"начальник дока", "начальника дока", "начальнику дока", "начальником дока"}},
            {"racketeer", new[]{"рэкетир", "рэкетира", "рэкетиру", "рэкетиром"}},
            {"black market dealer", new[]{"торговец чёрного рынка", "торговца чёрного рынка", "торговцу чёрного рынка", "торговцем чёрного рынка"}},
            {"arms dealer", new[]{"торговец оружием", "торговца оружием", "торговцу оружием", "торговцем оружием"}},
            {"underboss", new[]{"заместитель главаря", "заместителя главаря", "заместителю главаря", "заместителем главаря"}},
            {"water baron", new[]{"водный барон", "водного барона", "водному барону", "водным бароном"}},
            {"oxygen merchant", new[]{"торговец кислородом", "торговца кислородом", "торговцу кислородом", "торговцем кислородом"}},
            {"cargo thief", new[]{"похититель грузов", "похитителя грузов", "похитителю грузов", "похитителем грузов"}},
            {"hired gun", new[]{"наёмный стрелок", "наёмного стрелка", "наёмному стрелку", "наёмным стрелком"}},
            {"fuel hoarder", new[]{"скупщик топлива", "скупщика топлива", "скупщику топлива", "скупщиком топлива"}},
            {"corporate spy", new[]{"корпоративный шпион", "корпоративного шпиона", "корпоративному шпиону", "корпоративным шпионом"}},
            {"signal thief", new[]{"перехватчик сигналов", "перехватчика сигналов", "перехватчику сигналов", "перехватчиком сигналов"}},
            {"data broker", new[]{"информационный брокер", "информационного брокера", "информационному брокеру", "информационным брокером"}},
            {"wasteland raider", new[]{"налётчик с пустошей", "налётчика с пустошей", "налётчику с пустошей", "налётчиком с пустошей"}},
        };
        public static string Format(string source, object silver, string criminal, Pawn pawn)
        {
            if (!Templates.TryGetValue(source, out var russian)) throw new InvalidOperationException("Unreviewed Worksites volunteer story: " + source);
            if (!Roles.TryGetValue(criminal, out var forms)) throw new InvalidOperationException("Unreviewed Worksites criminal role: " + criminal);
            if (pawn == null) throw new ArgumentNullException(nameof(pawn));
            return russian.Formatted(pawn.Named("PAWN"), silver.Named("SILVER"), forms[0].Named("CRIMINAL_NOM"), forms[1].Named("CRIMINAL_GEN"), forms[2].Named("CRIMINAL_DAT"), forms[3].Named("CRIMINAL_INS")).Resolve();
        }
    }
}
