using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3687071198
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "MiningOutpost.Alert_OutpostTurrets:GetExplanation", new Dictionary<string,string> {
                { "1 {0}", "{0}: 1" },
                { "The outpost is defended by:", "Аванпост защищают:" },
                { "You must destroy these before you can reform the caravan.", "Чтобы заново сформировать караван, сначала нужно уничтожить эти турели." },
                { "{0} {1}s", "{1}: {0}" }
            } },
            { "MiningOutpost.Alert_OutpostTurrets:GetLabel", new Dictionary<string,string> {
                { "1 turret remaining", "Осталась 1 турель" },
                { "{0} turrets remaining", "Осталось турелей: {0}" }
            } },
            { "MiningOutpost.Alert_WorksiteDefenseIncoming:.ctor", new Dictionary<string,string> {
                { "Raids arriving", "Приближаются налётчики" }
            } },
            { "MiningOutpost.Alert_WorksiteDefenseIncoming:GetLabel", new Dictionary<string,string> {
                { "Raids arriving in ", "До прибытия налётчиков: " }
            } },
            { "MiningOutpost.Alert_WorksiteLaborersAvailable:GetExplanation", new Dictionary<string,string> {
                { " will build defenses. Right-click a laborer to request turrets or fortifications.", " Для заказа оборонительных турелей или укреплений щёлкните по рабочему правой кнопкой мыши." },
                { "1 laborer", "Доступен 1 рабочий." },
                { "{0} laborers", "Доступно рабочих: {0}." }
            } },
            { "MiningOutpost.Alert_WorksiteLaborersAvailable:GetLabel", new Dictionary<string,string> {
                { "1 laborer available to build", "Для строительства доступен 1 рабочий" },
                { "{0} laborers available to build", "Рабочих для строительства: {0}" }
            } },
            { "MiningOutpost.Alert_WorksiteMedicsAvailable:GetExplanation", new Dictionary<string,string> {
                { " will treat the wounded. Select a hurt pawn and right-click a medic to request healing.", " Можно запросить лечение: выберите раненого персонажа и щёлкните по медику правой кнопкой мыши." },
                { "1 medic", "Наготове 1 медик." },
                { "{0} medics", "Медиков наготове: {0}." }
            } },
            { "MiningOutpost.Alert_WorksiteMedicsAvailable:GetLabel", new Dictionary<string,string> {
                { "1 medic standing by", "Наготове 1 медик" },
                { "{0} medics standing by", "Медиков наготове: {0}" }
            } },
            { "MiningOutpost.Alert_WorksiteMercenariesAvailable:GetExplanation", new Dictionary<string,string> {
                { " will follow a colonist. Select a pawn and right-click a mercenary to ask.", " Наёмник может следовать за поселенцем. Для этого выберите персонажа и щёлкните по наёмнику правой кнопкой мыши." },
                { "1 mercenary", "Доступен 1 наёмник." },
                { "{0} mercenaries", "Доступно наёмников: {0}." }
            } },
            { "MiningOutpost.Alert_WorksiteMercenariesAvailable:GetLabel", new Dictionary<string,string> {
                { "1 mercenary available to command", "Под командование доступен 1 наёмник" },
                { "{0} mercenaries available to command", "Доступно наёмников под командование: {0}" }
            } },
            { "MiningOutpost.CaravanArrivalAction_AidWorksiteDefense:EnterPeacefully", new Dictionary<string,string> {
                { " arrived at ", " прибывает на помощь аванпосту «" },
                { " to help defend it.", "»." },
                { "Arrived to aid", "Прибытие на помощь" }
            } },
            { "MiningOutpost.CaravanArrivalAction_AidWorksiteDefense:get_Label", new Dictionary<string,string> {
                { "Aid", "Помочь" }
            } },
            { "MiningOutpost.CaravanArrivalAction_AidWorksiteDefense:get_ReportString", new Dictionary<string,string> {
                { "Aiding worksite defense.", "Помогает защищать рабочий участок." }
            } },
            { "MiningOutpost.Defense.Alert_WorksiteDugIn:GetExplanation", new Dictionary<string,string> {
                { "1 enemy has taken up a defensive position inside.", "Внутри занял оборону один враг." },
                { "{0} enemies have taken up defensive positions inside.", "Противников на оборонительных позициях внутри: {0}." }
            } },
            { "MiningOutpost.Defense.Alert_WorksiteDugIn:GetLabel", new Dictionary<string,string> {
                { "1 enemy remaining", "Остался 1 враг" },
                { "{0} enemies remaining", "Осталось врагов: {0}" }
            } },
            { "MiningOutpost.Defense.Designator_BuildWorksiteDefense:DesignateSingleCell", new Dictionary<string,string> {
                { "The worksite doesn't have the resources to build a ", "У аванпоста не хватает ресурсов на постройку: " }
            } },
            { "MiningOutpost.Defense.Designator_BuildWorksiteDefense:TryAutoWirePower", new Dictionary<string,string> {
                { " will be unpowered.", "." },
                { "No powered line within reach. The ", "Поблизости нет доступной линии электроснабжения. Без питания останется: " }
            } },
            { "MiningOutpost.Defense.Dialog_WorksiteDefenses:DoWindowContents", new Dictionary<string,string> {
                { "Close", "Закрыть" }
            } },
            { "MiningOutpost.Defense.Dialog_WorksiteDefenses:DrawBudgetReadout", new Dictionary<string,string> {
                { "Available:", "В наличии:" }
            } },
            { "MiningOutpost.Defense.Dialog_WorksiteDefenses:DrawCard", new Dictionary<string,string> {
                { "Not enough materials on-site to build that.", "На участке не хватает материалов для этого сооружения." }
            } },
            { "MiningOutpost.Defense.Dialog_WorksiteDefenses:get_Title", new Dictionary<string,string> {
                { "Build worksite defenses", "Построить укрепления на участке" },
                { "Build worksite turrets", "Построить турели на участке" }
            } },
            { "MiningOutpost.Defense.FloatMenuOptionProvider_WorksiteDefenseBuild+<GetOptionsFor>d__6:MoveNext", new Dictionary<string,string> {
                { "Ask to build a turret...", "Попросить построить турель…" },
                { "Ask to build defenses...", "Попросить построить укрепления…" }
            } },
            { "MiningOutpost.Defense.JobDriver_HotwireTurret:<MakeNewToils>b__2_3", new Dictionary<string,string> {
                { " hotwired ", " взламывает турель: " },
                { ". It will fight for you if the worksite turns hostile.", ". Если аванпост станет враждебным, она будет сражаться на вашей стороне." }
            } },
            { "MiningOutpost.Defense.MapComponent_TurretHotwire:ActivateAll", new Dictionary<string,string> {
                { "Your hotwired turret has switched to your side!", "Взломанная турель перешла на вашу сторону!" },
                { "Your {0} hotwired turrets have switched to your side!", "Взломанные турели перешли на вашу сторону! Всего: {0}." }
            } },
            { "MiningOutpost.Defense.MapComponent_WorksiteComms:RunnerName", new Dictionary<string,string> {
                { "the outpost", "аванпост" }
            } },
            { "MiningOutpost.Defense.MapComponent_WorksiteComms:TryDispatchRunner", new Dictionary<string,string> {
                { " is calling for help on the comms console.", " вызывает помощь по пульту связи." }
            } },
            { "MiningOutpost.Defense.MapComponent_WorksiteComms:WatchRunner", new Dictionary<string,string> {
                { " could call for help.", ")." },
                { " was downed before they could call for help.", " больше не может действовать. Вызвать помощь не удалось." },
                { "The comms console was destroyed before ", "Консоль связи уничтожена. Помощь не вызвана (" },
                { "The power was disconnected before ", "Связь обесточена. Помощь не вызвана (" }
            } },
            { "MiningOutpost.Defense.Patch_BuildingTurretGun_GetGizmos_Hotwire+<>c__DisplayClass0_0:<Postfix>b__0", new Dictionary<string,string> {
                { "No colonists available", "Нет доступных поселенцев" }
            } },
            { "MiningOutpost.Defense.Patch_BuildingTurretGun_GetGizmos_Hotwire+<>c__DisplayClass0_0:<Postfix>b__1", new Dictionary<string,string> {
                { " hotwired.", " — блокировка снята." },
                { "DevMode: ", "ОТЛАДКА: " }
            } },
            { "MiningOutpost.Defense.Patch_BuildingTurretGun_GetGizmos_Hotwire+<Postfix>d__0:MoveNext", new Dictionary<string,string> {
                { "DEV: Hotwire now", "ОТЛАДКА: мгновенный взлом" },
                { "Hotwire turret", "Взломать турель" },
                { "Instantly register this turret as hotwired.", "Мгновенно отметить турель как взломанную." },
                { "Order a colonist to splice this turret's targeting feed. Nothing changes now, but if you betray the worksite the turret fights for you.", "Приказать поселенцу вмешаться в систему наведения турели. Пока ничего не изменится, но если вы предадите владельцев участка, турель будет сражаться на вашей стороне." }
            } },
            { "MiningOutpost.Defense.TurretHotwireHelper+<GetHotwireOptionsForColonist>d__2:MoveNext", new Dictionary<string,string> {
                { " (cannot reach)", " (нельзя добраться)" },
                { " (reserved by ", " (занято: " },
                { "Hotwire turret", "Взломать турель" },
                { "someone", "кто-то" }
            } },
            { "MiningOutpost.Defense.WorksiteDefenseBuildAI:TryExpireStarvedOrder", new Dictionary<string,string> {
                { "The worksite doesn't have the resources to build a ", "У аванпоста не хватает ресурсов на постройку: " }
            } },
            { "MiningOutpost.FactionCleanupUtility:CleanupOrphanedTemporaryFactions", new Dictionary<string,string> {
                { "No orphaned temporary factions found.", "Не найдено временных фракций, оставшихся без связанных объектов." },
                { "Removed {0} orphaned temporary faction(s). Check log for details.", "Удалено временных фракций без связанных объектов: {0}. Подробности — в журнале." }
            } },
            { "MiningOutpost.FactionCleanupUtility:ListTemporaryFactions", new Dictionary<string,string> {
                { "Found {0} temporary faction(s). Check log for details.", "Найдено временных фракций: {0}. Подробности — в журнале." }
            } },
            { "MiningOutpost.FloatMenuOptionProvider_AskMedicForHealing:GetSingleOptionFor", new Dictionary<string,string> {
                { "Ask for healing", "Попросить о лечении" },
                { "Ask for healing (not injured)", "Попросить о лечении (нет ран)" },
                { "Cannot ask for healing: unreachable", "Невозможно попросить о лечении: нет пути" }
            } },
            { "MiningOutpost.FloatMenuOptionProvider_AskMercenaryToFollow+<>c__DisplayClass6_0:<GetSingleOptionFor>b__0", new Dictionary<string,string> {
                { " will stop following ", " прекращает сопровождение: " }
            } },
            { "MiningOutpost.FloatMenuOptionProvider_AskMercenaryToFollow+<>c__DisplayClass6_0:<GetSingleOptionFor>b__1", new Dictionary<string,string> {
                { " will stay close and defend ", " будет сопровождать и защищать: " }
            } },
            { "MiningOutpost.FloatMenuOptionProvider_AskMercenaryToFollow:GetSingleOptionFor", new Dictionary<string,string> {
                { "Ask to follow", "Попросить сопровождать" },
                { "Stop following ", "Прекратить сопровождение: " }
            } },
            { "MiningOutpost.FloatMenuOptionProvider_CoweringWorkerArrest+<GetOptionsFor>d__6:MoveNext", new Dictionary<string,string> {
                { "Cannot arrest: guards still present", "Невозможно арестовать: охрана ещё здесь" }
            } },
            { "MiningOutpost.FloatMenuOptionProvider_RecruitImpressedMercenary:GetSingleOptionFor", new Dictionary<string,string> {
                { "Recruit ", "Принять в колонию: " }
            } },
            { "MiningOutpost.JobGiver_MercenaryRouter:TryAidMeleedAlly", new Dictionary<string,string> {
                { "Moving to aid", "Идёт на помощь" }
            } },
            { "MiningOutpost.JobGiver_MercenaryRouter:TryGiveJob", new Dictionary<string,string> {
                { "Following ", "Сопровождает: " },
                { "to eat", "чтобы поесть" },
                { "to rest", "чтобы отдохнуть" }
            } },
            { "MiningOutpost.JobGiver_MercenaryRouter:TryHoldDefensivePosition", new Dictionary<string,string> {
                { "Defending", "Обороняется" },
                { "Falling back to cover", "Отступает в укрытие" }
            } },
            { "MiningOutpost.JobGiver_MercenaryRouter:TryTakeFrontlinePosition", new Dictionary<string,string> {
                { "Holding the line", "Удерживает рубеж" },
                { "Taking up position", "Занимает позицию" }
            } },
            { "MiningOutpost.JobGiver_MercenaryRouter:TryWildernessPatrol", new Dictionary<string,string> {
                { "Patrolling the wilderness", "Патрулирует окрестности" }
            } },
            { "MiningOutpost.Loot.FarmingOutpost_MapTracker:CheckIfOutpostConquered", new Dictionary<string,string> {
                { "{0} {1} survived the raid, you can add them to your caravan or leave them behind.", "Выжившие после нападения: {1} — {0}. Их можно добавить в караван или оставить на аванпосте." }
            } },
            { "MiningOutpost.LordJob_MiningOutpostGuard:Notify_MercenaryAttacked", new Dictionary<string,string> {
                { "You have attacked a mercenary guard! They are requesting backup...", "Вы атаковали наёмника из охраны! Идёт вызов подкрепления…" }
            } },
            { "MiningOutpost.LordJob_MiningOutpostWork:UpdateWorkerBackupCountdown", new Dictionary<string,string> {
                { "You have attacked a worker! They are calling for help...", "Вы атаковали работника аванпоста! Идёт вызов подкрепления…" }
            } },
            { "MiningOutpost.LordToil_MiningOutpostGuard:LordToilTick", new Dictionary<string,string> {
                { "The outpost is burning down! The garrison is fighting the fire and on high alert.", "Аванпост полыхает! Гарнизон тушит пожар и приведён в полную боевую готовность." }
            } },
            { "MiningOutpost.MiningOutpostMod:DoSettingsWindowContents", new Dictionary<string,string> {
                { "Both systems work independently. Ideology spawns its work sites (logging, mining, farming, hunting) and Worksites Expanded will spawns its own sites.", "Обе системы работают независимо. Ideology создаёт лесозаготовительные, горнодобывающие, фермерские и охотничьи лагеря, а Worksites Expanded — свои рабочие участки." },
                { "Changes to Ideology compatibility take effect after restarting the game.", "Изменения совместимости с Ideology вступят в силу после перезапуска игры." },
                { "Dig in chance:  {0:P0}", "Шанс занять оборону: {0:P0}" },
                { "Disable loiter timeout", "Отключить ограничение на бездействие" },
                { "Disable outpost turrets", "Отключить турели аванпостов" },
                { "Disables all outpost AI. Pawns will use standard RimWorld behaviour.", "Отключить весь особый ИИ аванпостов. Персонажи будут вести себя по обычным правилам RimWorld." },
                { "Disables the timeout where guards assault the player if they loiter at the outpost without fighting.", "Охрана не будет нападать на ваших персонажей за то, что они слишком долго находятся на аванпосте, не вступая в бой." },
                { "Enemy threat multiplier:  {0:F1}×", "Множитель силы защитников: {0:F1}×" },
                { "How often mercenaries will take up defensive positions instead of coming out to meet you. Set to 0% to turn off defensive positions.", "Как часто наёмники занимают оборонительные позиции вместо того, чтобы выходить вам навстречу. При 0% они не занимают оборонительные позиции." },
                { "How often outpost quests appear. Frequency decreases as more outpost sites exist on the world map.", "Как часто появляются задания на аванпосты. Чем больше таких участков уже есть на карте мира, тем реже появляются новые." },
                { "Ideology and Worksites Expanded", "Ideology и Worksites Expanded" },
                { "Increases the threat of outpost defenses. 1.0 is default.", "Увеличить силу защиты аванпостов. По умолчанию: 1,0." },
                { "Outpost factions", "Фракции аванпостов" },
                { "Outposts will be created by either temporary or permanent factions.", "Аванпосты могут принадлежать как временным, так и постоянным фракциям." },
                { "Outposts will be created by permanent factions only.", "Аванпосты могут принадлежать только постоянным фракциям." },
                { "Outposts will be created by temporary factions only.", "Аванпосты могут принадлежать только временным фракциям." },
                { "Permanent factions only", "Только постоянные фракции" },
                { "Prints verbose logs to the debug console during outpost generation.", "Выводить подробные записи в журнал отладки при создании аванпостов." },
                { "Quest spawn rate:  ", "Частота заданий: " },
                { "Removes all turrets from generated outposts.", "Удалять все турели при создании аванпостов." },
                { "Replace outpost turrets with Vanilla Furniture Expanded - Security equivalents and add weapon complexes to the perimeter.", "Заменять турели аванпостов аналогами из Vanilla Furniture Expanded — Security и добавлять оружейные комплексы по периметру." },
                { "Temporary and permanent factions (recommended)", "Временные и постоянные фракции (рекомендуется)" },
                { "Temporary factions only", "Только временные фракции" },
                { "The Worksites Expanded experience. Ideology work sites are removed.", "Использовать рабочие участки из Worksites Expanded. Рабочие участки из Ideology будут отключены." },
                { "Upgrade outpost security", "Усилить защиту аванпостов" },
                { "Use vanilla AI", "Использовать стандартный ИИ" },
                { "Verbose logging", "Подробный журнал" },
                { "Worksites Expanded only (recommended)", "Только Worksites Expanded (рекомендуется)" }
            } },
            { "MiningOutpost.Orbital.CaravanArrivalAction_EnterOrbitalPlatform:Arrived", new Dictionary<string,string> {
                { "The orbital platform cannot be contacted right now.", "Сейчас с орбитальной платформой нельзя связаться." },
                { "The orbital platform is currently under siege.", "Сейчас орбитальная платформа находится в осаде." }
            } },
            { "MiningOutpost.Orbital.CaravanArrivalAction_EnterOrbitalPlatform:get_Label", new Dictionary<string,string> {
                { "Enter orbital platform", "Войти на орбитальную станцию" }
            } },
            { "MiningOutpost.Orbital.CaravanArrivalAction_EnterOrbitalPlatform:get_ReportString", new Dictionary<string,string> {
                { "Entering orbital platform.", "Вход на орбитальную платформу." }
            } },
            { "MiningOutpost.Orbital.Dialog_GravshipPlatformWarning:.ctor", new Dictionary<string,string> {
                { "Abort", "Отмена" },
                { "Approaching this platform in a standard gravship is a death sentence. The station's defensive batteries are highly accurate and will tear unarmored hulls to pieces. Unless you are piloting a fully equipped warship ready for high-intensity combat, steer clear. For peaceful arrivals, use a passenger shuttle.", "Подойти к этой платформе на обычном гравикорабле — верная смерть. Её оборонительные батареи бьют очень точно и разорвут небронированный корпус на части. Держитесь подальше, если у вас нет полностью оснащённого боевого корабля, готового к ожесточённому сражению. Для мирного визита используйте пассажирский челнок." },
                { "I'm ready to die", "Смерть меня не пугает" }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:AtmosphericFor", new Dictionary<string,string> {
                { "A bar and hub for expatriates somewhere in the bowels of the vast orbital platform. It is the kind of place you ‘go for a drink’, not to actually have a drink. Known for being extremely popular among those who wish to gamble for change, or those who are looking for killers and connected fixers.", "Бар и место встреч тех, кто живёт вдали от родины, где-то в недрах огромной орбитальной платформы. Сюда заходят «выпить», но вовсе не ради выпивки. Заведение особенно популярно среди тех, кто готов сыграть с судьбой, а также тех, кто ищет наёмных убийц или решал с нужными связями." },
                { "A monitor blinks through expired listings: 'No new jobs today'. Crews are forced to seek out local fixers, take on one-time contracts, or hunt down their own leads.", "На мониторе мигают просроченные объявления: «Новой работы сегодня нет». Экипажам приходится искать местных посредников, браться за разовые контракты или самостоятельно добывать зацепки." },
                { "A sprawling, chaotic bazaar is carved straight into the platform’s highest-traffic thoroughfares. It is a completely open secret in lawless space where merchants and mercenaries filter through daily, casually browsing stalls packed with drugs, high-end military hardware and stolen ultratech. Some of the vendors are even beginning to put up neon signs.", "Огромный хаотичный базар втиснут прямо в самые оживлённые проходы платформы. В космосе, где нет закона, об этом «тайном» рынке знает каждый. Торговцы и наёмники ежедневно проходят мимо и как ни в чём не бывало разглядывают прилавки с наркотиками, передовой военной техникой и крадеными сверхтехнологиями. Некоторые продавцы уже начинают вывешивать неоновые вывески." },
                { "Fuel merchants shout prices across crowded terminals while thick transfer hoses snake into docked freighters. Steam and smoke dance around your boots and the floor vibrates faintly.", "Торговцы топливом выкрикивают цены сквозь гул переполненных терминалов, а толстые перекачивающие шланги тянутся к пришвартованным грузовым судам. Вокруг ваших сапог клубятся пар и дым; пол слегка вибрирует." },
                { "Sparks rain from the catwalks as engineers in oil-stained vacsuits work on a half-stripped light craft. The clang of impact wrenches is echoing off the bulkheads.", "С мостков сыплются искры: инженеры в замасленных скафандрах работают над наполовину разобранным лёгким судном. Грохот ударных гайковёртов отдаётся эхом от переборок." },
                { "The waiting room smells of antiseptic and ozone. Cybernetic doctors can do surgical techniques faster than you can repair a toaster and mercenaries are always running to them to patch up wounds or install new bionics. Think of it like getting your ears pierced, circa 1980.", "В приёмной пахнет антисептиком и озоном. Киберхирурги проводят операции быстрее, чем вы успеете починить тостер, и наёмники то и дело прибегают залечить раны или установить новую бионику. Примерно как проколоть уши где-нибудь в 1980-м." }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:CacheHeaderInfo", new Dictionary<string,string> {
                { "{0} is a claustrophobic labyrinth of narrow alleys beneath sweating pipes and spiderweb girders. Wet, black cables hang like vines between stalls and crowded bars. The thick fog of recycled air is warm and slightly metallic. Unlike the barren surface of the rimworld, every square meter of the rotating grid is fiercely monitored, ensuring the gears of trade never stop grinding.", "{0} — тесный лабиринт узких проходов под мокрыми от конденсата трубами и паутиной балок. Между лавками и переполненными барами, словно лианы, свисают влажные чёрные кабели. Густая пелена тёплого рециркулирующего воздуха отдаёт металлом. В отличие от пустынной поверхности окраинного мира, каждый квадратный метр этой вращающейся платформы находится под неусыпным надзором: жернова торговли не должны останавливаться." }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:DrawBlackMarket", new Dictionary<string,string> {
                { "Cheap highs for desperate street-rats.", "Дешёвая дурь для отчаявшейся уличной шпаны." },
                { "Contraband", "Контрабанда" },
                { "Exotic tech with lethal pricetags.", "Экзотические технологии по убийственным ценам." },
                { "Gear", "Снаряжение" },
                { "Rare items", "Редкие товары" },
                { "Weapons and armor of dubious provenance.", "Оружие и броня сомнительного происхождения." }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:DrawExitButton", new Dictionary<string,string> {
                { "<color=#FF5470>Abandon Job</color>", "<color=#FF5470>Бросить работу</color>" },
                { "Exit", "Выйти" }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:DrawGambleForChange", new Dictionary<string,string> {
                { "Blackjack", "Блэкджек" },
                { "Coming soon.", "Скоро появится." },
                { "Craps", "Крэпс" },
                { "Find a dark corner to shoot dice with the other miscreants of the platform.", "Найти тёмный угол и бросить кости в компании других негодяев с платформы." },
                { "Hit or stand against the dealer.", "Взять ещё карту или остановиться — сыграйте против дилера." },
                { "Hold'em against your rivals.", "Холдем против ваших соперников." },
                { "Poker", "Покер" }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:DrawLobby", new Dictionary<string,string> {
                { "A bar and hub for seeking volunteers, hiring mercenaries, and gambling for change.", "Бар, где можно найти добровольцев, нанять наёмников и сыграть с судьбой." },
                { "Black market", "Чёрный рынок" },
                { "Buy contraband.", "Купить контрабанду." },
                { "Cybernetic doctor", "Киберхирург" },
                { "Find work, leads, and rumors.", "Найти работу, зацепки и слухи." },
                { "Fueling station", "Заправочная станция" },
                { "Install cybernetic implants.", "Установить кибернетические импланты." },
                { "Leave platform", "Покинуть платформу" },
                { "Look for a fuel merchant.", "Найти торговца топливом." },
                { "Look for opportunities", "Поиск возможностей" },
                { "Request ship repairs and purchase replacement parts.", "Заказать ремонт корабля и приобрести запчасти." },
                { "Shuttle hangar", "Ангар челноков" }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:DrawRecruitmentHub", new Dictionary<string,string> {
                { "Gamble for change", "Игра с судьбой" },
                { "See who is up for recruitment.", "Узнать, кто готов присоединиться к вам." },
                { "Seek volunteers", "Поиск добровольцев" },
                { "Sit at the card table.", "Сесть за карточный стол." }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:DrawShuttleHangar", new Dictionary<string,string> {
                { "Buy spare parts, scrap and components from the orbital dockyard", "Купить запчасти, лом и компоненты на орбитальной верфи." },
                { "Hire engineers", "Нанять инженеров" },
                { "Hire engineers to perform maintenance on your gravship.", "Нанять инженеров для обслуживания гравикорабля." },
                { "Maintenance shop", "Ремонтная мастерская" }
            } },
            { "MiningOutpost.Orbital.Dialog_OrbitalServices:LocationLabel", new Dictionary<string,string> {
                { "Black market", "Чёрный рынок" },
                { "Cybernetic doctor", "Киберхирург" },
                { "Fueling station", "Заправочная станция" },
                { "Look for opportunities", "Поиск возможностей" },
                { "Shuttle hangar", "Ангар челноков" }
            } },
            { "MiningOutpost.Orbital.OrbitalDockAtInjector+<Inject>d__0:MoveNext", new Dictionary<string,string> {
                { "Attack {0}", "Атаковать: {0}" },
                { "orbital platform", "орбитальная платформа" }
            } },
            { "MiningOutpost.Orbital.OrbitalStrings:RandomRecruitmentHubName", new Dictionary<string,string> {
                { "Recruitment hub", "Вербовочный центр" }
            } },
            { "MiningOutpost.Orbital.Patch_Caravan_GetGizmos_OrbitalRelabel:Postfix", new Dictionary<string,string> {
                { "Dock at the orbital platform.", "Пристыковаться к орбитальной станции." },
                { "Enter orbital platform", "Войти на орбитальную станцию" },
                { "Parley", "Переговоры" },
                { "The orbital platform is currently under siege.", "Сейчас орбитальная станция находится в осаде." }
            } },
            { "MiningOutpost.Orbital.Patch_TilesPerDayCalculator_OrbitalShortcut:Prefix", new Dictionary<string,string> {
                { "Caravan is docked and cannot traverse.", "Караван находится у причала и не может передвигаться." }
            } },
            { "MiningOutpost.Orbital.Patch_TilesPerDayCalculator_OrbitalShortcut_LowLevel:Prefix", new Dictionary<string,string> {
                { "Tile is on a non-traversable layer.", "Эта клетка находится на слое, непригодном для передвижения." }
            } },
            { "MiningOutpost.Orbital.Services.AugmentedDebtorEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Tenement Alley", "Переулок у доходного дома" },
                { "The debtor snarls, raising an industrial-grade cyber-arm as alley muscle closes in!", "Должник рычит, поднимая промышленную киберруку, а уличные громилы смыкают кольцо!" }
            } },
            { "MiningOutpost.Orbital.Services.AugmentedDebtorEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Alley Thug ", "Уличный бандит " },
                { "Augmented Debtor", "Должник с имплантами" },
                { "fires zip gun", "стреляет из самопала" },
                { "slams hydraulic fist", "бьёт гидравлическим кулаком" }
            } },
            { "MiningOutpost.Orbital.Services.AugmentedDebtorEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Crushing blows fracture armor. Crew falls back into the rain.", "Сокрушительные удары раскалывают броню. Экипаж отступает под дождь." }
            } },
            { "MiningOutpost.Orbital.Services.AugmentedDebtorEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Cramped concrete alley, black-market cyberware whining under power surge.", "Тесный бетонный переулок; при скачке нагрузки воют купленные на чёрном рынке импланты." }
            } },
            { "MiningOutpost.Orbital.Services.AugmentedDebtorEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Hab-Block 9 Tenement", "Доходный дом жилого блока №9" }
            } },
            { "MiningOutpost.Orbital.Services.AugmentedDebtorEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The debtor falls, cyber-limbs sparking. Extraction complete.", "Должник падает, искря киберконечностями. Изъятие имплантов завершено." }
            } },
            { "MiningOutpost.Orbital.Services.BallastTurnkeyEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Ballast Bay", "Балластный отсек" },
                { "The holding pen gate slams open! Kidnappers level shotguns across the flooded grating.", "Ворота загона с грохотом распахиваются! Похитители направляют дробовики через затопленную решётку." }
            } },
            { "MiningOutpost.Orbital.Services.BallastTurnkeyEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Turnkey Guard ", "Тюремщик " },
                { "Turnkey Ring Leader", "Главарь тюремщиков" },
                { "fires sawn-off shotgun", "стреляет из обреза" },
                { "fires slug thrower", "стреляет из пулевого оружия" }
            } },
            { "MiningOutpost.Orbital.Services.BallastTurnkeyEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Slugs shatter the gangway grates. Crew breaks contact.", "Пули разбивают решётки перехода. Экипаж выходит из боя." }
            } },
            { "MiningOutpost.Orbital.Services.BallastTurnkeyEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Dripping rust-water, cold iron bulkheads, shotguns loaded with heavy shot.", "Капает ржавая вода, холодеют железные переборки; дробовики заряжены картечью." }
            } },
            { "MiningOutpost.Orbital.Services.BallastTurnkeyEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Ballast Bay Sub-Deck", "Нижняя палуба балластного отсека" }
            } },
            { "MiningOutpost.Orbital.Services.BallastTurnkeyEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The turnkey gang is broken. The hostage is secured.", "Банда тюремщика разбита. Заложник в безопасности." }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackLogWindow:DoWindowContents", new Dictionary<string,string> {
                { "(no active table)", "(нет активного стола)" },
                { "Log", "Журнал" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:.ctor", new Dictionary<string,string> {
                { "-deck shoe. Dealer stands on all 17s.", ". Дилер останавливается на любых 17 очках, включая мягкие 17." },
                { "Fresh ", "Новая пачка карт. Колод: " },
                { "You", "Игрок" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:AdvanceBotTurn", new Dictionary<string,string> {
                { "busts at ", "перебор: " },
                { "hits.", "берёт карту." },
                { "soft ", "мягких " },
                { "stands on ", "останавливается на " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:AdvanceDealerTurn", new Dictionary<string,string> {
                { "Dealer busts at ", "У дилера перебор: " },
                { "Dealer hits. The total is ", "Дилер берёт карту. Сумма очков: " },
                { "Dealer stands on ", "Дилер останавливается на " },
                { "soft ", "мягких " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:BeginDealerTurn", new Dictionary<string,string> {
                { ". The total is ", ". Сумма очков: " },
                { "Dealer reveals ", "Дилер открывает " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:FormatTotalLabel", new Dictionary<string,string> {
                { "Blackjack", "Блэкджек" },
                { "Bust", "Перебор" },
                { "s", " (мягкая)" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:LogDealer", new Dictionary<string,string> {
                { "Dealer", "Дилер" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:MakeBotName", new Dictionary<string,string> {
                { "Alex", "Алекс" },
                { "Dana", "Дана" },
                { "Drey", "Дрей" },
                { "Marl", "Марл" },
                { "Mei", "Мэй" },
                { "Pell", "Пелл" },
                { "Riku", "Рику" },
                { "Sol", "Сол" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:PlayerDouble", new Dictionary<string,string> {
                { "bust.", "перебор." },
                { "doubles down. The total is ", "удваивает ставку и берёт карту. Сумма очков: " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:PlayerHit", new Dictionary<string,string> {
                { "bust.", "перебор." },
                { "hits. The total is ", "берёт карту. Сумма очков: " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:PlayerStand", new Dictionary<string,string> {
                { "stands on ", "останавливается на " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:ReshuffleShoe", new Dictionary<string,string> {
                { " cards.", "." },
                { "Shoe shuffled. ", "Башмак перемешан. Количество карт: " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:Resolve", new Dictionary<string,string> {
                { "loses ", "проигрывает " },
                { "pushes.", "играет вничью." },
                { "wins ", "выигрывает " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:StartDeal", new Dictionary<string,string> {
                { ". Dealer shows ", ". Открытая карта дилера: " },
                { "Hand #", "Раздача №" },
                { "blackjack!", "блэкджек!" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:StartNextHand", new Dictionary<string,string> {
                { "cashes out.", "забирает деньги и покидает стол." }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:ConfirmExitTable", new Dictionary<string,string> {
                { "Exit table", "Выйти из-за стола" },
                { "Keep playing", "Продолжить игру" },
                { "Leave", "Уйти" },
                { "Leave with $", "Покинуть стол и забрать $" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawActionRow", new Dictionary<string,string> {
                { "Deal  ", "Раздать  " },
                { "Double down", "Удвоить ставку" },
                { "Doubling needs two cards and enough silver to match the bet.", "Для удвоения нужны две карты и достаточно серебра, чтобы повторить ставку." },
                { "Hit", "Ещё карту" },
                { "Place chips to set your bet.", "Положите фишки, чтобы сделать ставку." },
                { "Stand", "Хватит" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawBuyInScreen", new Dictionary<string,string> {
                { " buy-in.</color>", ".</color>" },
                { "<color=#E96C6C>Not enough silver to cover the $", "<color=#E96C6C>Не хватает серебра на вступительный взнос: $" },
                { "Blackjack", "Блэкджек" },
                { "Buy-in", "Вступительный взнос" },
                { "Caravan silver: <color=#F5C654>$", "Серебро в караване: <color=#F5C654>$" },
                { "Min chip $", "Минимальная фишка: $" },
                { "No conscious colonists available to seat at the table.", "Нет поселенцев в сознании, способных сесть за стол." },
                { "Pick a buy-in and a colonist to sit down. The shoe is six decks. The dealer must stand on all 17s.", "Выберите вступительный взнос и поселенца, который сядет за стол. В игре шесть колод. При 17 очках дилер всегда останавливается." },
                { "Seat", "Игрок" },
                { "Sit at table", "Сесть за стол" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawChipTray", new Dictionary<string,string> {
                { "Bet ", "Ставка: " },
                { "Bet 0", "Ставка: 0" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawDealerPanel", new Dictionary<string,string> {
                { "Dealer", "Дилер" },
                { "Dealer must stand on 17", "Дилер останавливается при 17 очках" },
                { "Shoe ", "Карт в башмаке: " },
                { "Showing ", "Открытая карта: " },
                { "Total ", "Сумма: " }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawEmptySeat", new Dictionary<string,string> {
                { "Empty seat", "Свободное место" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawExitRow", new Dictionary<string,string> {
                { "Exit table", "Выйти из-за стола" },
                { "You can't stand up mid-hand.", "Во время раздачи встать из-за стола нельзя." }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawPawnPicker", new Dictionary<string,string> {
                { "Swap colonist", "Сменить поселенца" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawPlayerPanel", new Dictionary<string,string> {
                { "(no bet placed)", "(ставка не сделана)" },
                { "</color> since sitting down.", "</color>." },
                { "Bet ", "Ставка: " },
                { "Betting", "Ставки" },
                { "Blackjack", "Блэкджек" },
                { "Lose", "Проигрыш" },
                { "Push", "Ничья" },
                { "Waiting", "Ожидание" },
                { "Win", "Выигрыш" },
                { "You have lost <color=#E96C6C>-", "С начала игры вы потеряли <color=#E96C6C>-" },
                { "You have made <color=#8CD96B>+", "С начала игры вы выиграли <color=#8CD96B>+" },
                { "You're breaking even since sitting down.", "С начала игры вы вышли в ноль." },
                { "Your turn", "Ваш ход" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawResetRow", new Dictionary<string,string> {
                { "Reset", "Сбросить" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:DrawTable", new Dictionary<string,string> {
                { "Blackjack", "Блэкджек" },
                { "Close", "Закрыть" },
                { "Hand #", "Раздача №" },
                { "Log", "Журнал" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:PhaseLabel", new Dictionary<string,string> {
                { "Dealer acting", "Ход дилера" },
                { "Hand over", "Раздача окончена" },
                { "Place your bet", "Сделайте ставку" },
                { "Players acting", "Ходят игроки" },
                { "Your turn", "Ваш ход" }
            } },
            { "MiningOutpost.Orbital.Services.BlackjackTable:StartSession", new Dictionary<string,string> {
                { "Not enough silver.", "Недостаточно серебра." }
            } },
            { "MiningOutpost.Orbital.Services.BuyerBodyguardEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Antechamber", "Передняя" },
                { "Focus fire on the heavily armored bodyguard before his point-blank fire cuts the squad down.", "Сосредоточьте огонь на тяжело бронированном телохранителе, прежде чем он расстреляет отряд в упор." },
                { "Resolver", "Ведение боя" },
                { "The buyer scrambles behind the blast vault as his chrome bodyguard powers up servos.", "Покупатель прячется за бронированным хранилищем, пока его телохранитель с имплантами запускает сервоприводы." }
            } },
            { "MiningOutpost.Orbital.Services.BuyerBodyguardEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Augmented Bodyguard", "Телохранитель с имплантами" },
                { "Syndicate Guard ", "Охранник синдиката " },
                { "fires autopistol", "стреляет из автоматического пистолета" },
                { "fires heavy magnum", "стреляет из тяжёлого магнума" }
            } },
            { "MiningOutpost.Orbital.Services.BuyerBodyguardEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Subdermal plating soaks the rounds. The crew is driven back.", "Подкожные бронепластины поглощают пули. Экипаж вынужден отойти." }
            } },
            { "MiningOutpost.Orbital.Services.BuyerBodyguardEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Subdermal chrome, heavy caliber revolver, monoblade in the sleeve.", "Подкожные импланты, крупнокалиберный револьвер и моноклинок в рукаве." }
            } },
            { "MiningOutpost.Orbital.Services.BuyerBodyguardEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Buyer's Vault Antechamber", "Перед хранилищем покупателя" }
            } },
            { "MiningOutpost.Orbital.Services.BuyerBodyguardEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The cybernetic bodyguard crashes to the deck plating. The vault is yours.", "Телохранитель с имплантами с грохотом падает на палубный настил. Хранилище ваше." }
            } },
            { "MiningOutpost.Orbital.Services.CasinoBouncersEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Bouncers flip the card table as the pit boss draws an automatic weapon!", "Вышибалы опрокидывают карточный стол, а пит-босс достаёт автоматическое оружие!" },
                { "Casino Floor", "Зал казино" }
            } },
            { "MiningOutpost.Orbital.Services.CasinoBouncersEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Casino Bouncer ", "Вышибала казино " },
                { "Pit Boss", "Смотритель зала" },
                { "fires machine pistol", "стреляет из автоматического пистолета" },
                { "swings shock baton", "бьёт электродубинкой" }
            } },
            { "MiningOutpost.Orbital.Services.CasinoBouncersEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Shock clubs and stun fire drop the crew onto the plush carpet.", "Электродубинки и оглушающие выстрелы валят экипаж на мягкий ковёр." }
            } },
            { "MiningOutpost.Orbital.Services.CasinoBouncersEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Upturned velvet tables, shattered glassware, security sirens wailing.", "Опрокинутые столы с бархатным покрытием, битое стекло и вой охранных сирен." }
            } },
            { "MiningOutpost.Orbital.Services.CasinoBouncersEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "High-Roller Pit", "Зал крупных ставок" }
            } },
            { "MiningOutpost.Orbital.Services.CasinoBouncersEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The casino floor is cleared. The winnings are yours.", "Зал казино зачищен. Выигрыш ваш." }
            } },
            { "MiningOutpost.Orbital.Services.CollectorBodyguardEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Corridor", "Коридор" },
                { "The loan collector draws a heavy caliber hand-cannon as his armored muscle steps up.", "Сборщик долгов выхватывает крупнокалиберную ручную пушку, а его бронированный громила выходит вперёд." }
            } },
            { "MiningOutpost.Orbital.Services.CollectorBodyguardEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Armored Enforcer", "Бронированный силовик" },
                { "Hired Muscle", "Наёмный громила" },
                { "The Loan Collector", "Сборщик долгов" },
                { "fires heavy revolver", "стреляет из тяжёлого револьвера" },
                { "fires pump shotgun", "стреляет из помпового дробовика" },
                { "fires service pistol", "стреляет из служебного пистолета" }
            } },
            { "MiningOutpost.Orbital.Services.CollectorBodyguardEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Heavy slugs smash the bulkheads. Crew forced to break off.", "Тяжёлые пули крушат переборки. Экипаж вынужден выйти из боя." }
            } },
            { "MiningOutpost.Orbital.Services.CollectorBodyguardEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Strobe emergency lighting, hydraulic hissing, no witnesses.", "Пульсирует аварийное освещение, шипит гидравлика. Свидетелей нет." }
            } },
            { "MiningOutpost.Orbital.Services.CollectorBodyguardEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Service Corridor 4B", "Служебный коридор 4B" }
            } },
            { "MiningOutpost.Orbital.Services.CollectorBodyguardEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The collector drops his ledger. The debt is permanently liquidated.", "Сборщик долгов роняет учётную книгу. Долг окончательно ликвидирован." }
            } },
            { "MiningOutpost.Orbital.Services.Combatant:get_StatusLabel", new Dictionary<string,string> {
                { "Dead", "Смерть" },
                { "Healthy", "В порядке" },
                { "Injured", "Ранения" },
                { "Unconscious", "Без сознания" }
            } },
            { "MiningOutpost.Orbital.Services.ConcourseEnforcersEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Checkpoint", "Блокпост" },
                { "Deploy squad orders and breach the checkpoint line.", "Раздайте приказы отряду и прорвите оборону блокпоста." },
                { "Resolver", "Ведение боя" },
                { "Syndicate enforcers rack combat shotguns from behind reinforced blast barriers.", "Силовики синдиката передёргивают затворы боевых дробовиков за укреплёнными взрывозащитными барьерами." }
            } },
            { "MiningOutpost.Orbital.Services.ConcourseEnforcersEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Syndicate Enforcer ", "Силовик синдиката " },
                { "fires riot shotgun", "стреляет из полицейского дробовика" }
            } },
            { "MiningOutpost.Orbital.Services.ConcourseEnforcersEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Enforcer shotguns tear through the crew. Fallback!", "Дробовики силовиков косят экипаж. Отступаем!" }
            } },
            { "MiningOutpost.Orbital.Services.ConcourseEnforcersEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Station enforcers behind barrier barricades, shotguns racked.", "Силовики станции за баррикадами, дробовики наготове." }
            } },
            { "MiningOutpost.Orbital.Services.ConcourseEnforcersEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Bazaar Security Checkpoint", "Пост охраны базара" }
            } },
            { "MiningOutpost.Orbital.Services.ConcourseEnforcersEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The checkpoint is smashed. The concourse is clear.", "Блокпост разгромлен. Проход свободен." }
            } },
            { "MiningOutpost.Orbital.Services.DefenseDronesEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Bridge Deck", "Палуба мостика" },
                { "Emergency systems detect intruders! Rotary ceiling drones drop on gimbals.", "Аварийные системы обнаружили нарушителей! С потолка на шарнирных подвесах опускаются дроны с многоствольными орудиями." }
            } },
            { "MiningOutpost.Orbital.Services.DefenseDronesEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Defense Drone ", "Оборонительный дрон " },
                { "fires rotary repeater", "стреляет из многоствольного скорострельного орудия" }
            } },
            { "MiningOutpost.Orbital.Services.DefenseDronesEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Rotary fire sweeps the bridge corridor, forcing an emergency retreat.", "Огонь многоствольных орудий прочёсывает коридор мостика, вынуждая немедленно отступить." }
            } },
            { "MiningOutpost.Orbital.Services.DefenseDronesEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Decompressed flight deck, automated point-defense drones spinning up.", "Разгерметизированная палуба управления; автоматические дроны ближней обороны запускают орудия." }
            } },
            { "MiningOutpost.Orbital.Services.DefenseDronesEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Derelict Command Bridge", "Мостик заброшенного корабля" }
            } },
            { "MiningOutpost.Orbital.Services.DefenseDronesEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The defense drones spark out and clatter to the deck.", "Оборонительные дроны, искря, гаснут и с грохотом падают на палубу." }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawCrewCard", new Dictionary<string,string> {
                { "Cannot act.", "Не может действовать." },
                { "Click the name to view wounds", "Нажмите на имя, чтобы посмотреть ранения" },
                { "Down. The crew will recover them.", "Не может встать. Экипаж организует эвакуацию." },
                { "End Turn", "Завершить ход" },
                { "Order", "Приказ" },
                { "Out of the fight.", "Больше не участвует в бою." },
                { "Patch", "Перевязать" },
                { "Ready", "Готово" },
                { "Reload", "Перезарядка" },
                { "Stunned. Loses this round.", "Оглушение. Этот раунд пропущен." },
                { "Target", "Цель" },
                { "nearest", "ближайшая" },
                { "{0}   {1}/{2} rds", "{0}   {1}/{2} патр." }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawCrewColumn", new Dictionary<string,string> {
                { "Crew", "Экипаж" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawEnemyCard", new Dictionary<string,string> {
                { "Click to view wounds", "Нажмите, чтобы посмотреть ранения" },
                { "Difficulty to hit: {0}", "Сложность попадания: {0}" },
                { "Down", "Не может встать" },
                { "Killed", "Смерть" },
                { "Operational", "В строю" },
                { "Out of the fight", "Не участвует в бою" },
                { "Wounded", "Ранения" },
                { "{0} {1}m", "{0} {1} м" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawEnemyColumn", new Dictionary<string,string> {
                { "Enemies", "Противники" },
                { "{0} up", "В строю: {0}" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawFooter", new Dictionary<string,string> {
                { "FALL BACK", "ОТСТУПИТЬ" },
                { "LEAVE THE BAY", "ПОКИНУТЬ ОТСЕК" },
                { "RESOLVE QUEUED ORDERS", "ВЫПОЛНИТЬ ПРИКАЗЫ НА ХОД" },
                { "WAITING ON {0} CREW", "ЖДУТ ПРИКАЗОВ: {0}" },
                { "Withdraw", "Отступить" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawHeader", new Dictionary<string,string> {
                { "FIREFIGHT  -  ", "ПЕРЕСТРЕЛКА — " },
                { "Over", "Бой окончен" },
                { "PHASE", "ФАЗА" },
                { "Queue Orders", "Приказы на ход" },
                { "ROUND", "РАУНД" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:DrawLogColumn", new Dictionary<string,string> {
                { "Combat Log", "Журнал боя" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:OpenOrderMenu", new Dictionary<string,string> {
                { "Aim", "Прицелиться" },
                { "Burst fire", "Стрельба очередью" },
                { "Charge into melee", "Ринуться в ближний бой" },
                { "Hold", "Удерживать позицию" },
                { "Patch wounds", "Перевязать раны" },
                { "Reload", "Перезарядка" },
                { "Single shot", "Одиночный выстрел" },
                { "Take cover", "Укрыться" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:OpenTargetMenu", new Dictionary<string,string> {
                { "(no targets)", "(нет целей)" },
                { "{0}  ({1}, DC {2})", "{0}  ({1}, сложность {2})" }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:OrderHint", new Dictionary<string,string> {
                { "Harder to hit this round.", "В этом раунде попасть сложнее." },
                { "Holds fire.", "Не открывает огонь." }
            } },
            { "MiningOutpost.Orbital.Services.Dialog_Firefight:OrderLabel", new Dictionary<string,string> {
                { "Aim", "Прицелиться" },
                { "Burst fire", "Стрельба очередью" },
                { "Charge into melee", "Ринуться в ближний бой" },
                { "Choose", "Выбрать" },
                { "Hold", "Удерживать позицию" },
                { "Patch wounds", "Перевязать раны" },
                { "Reload", "Перезарядка" },
                { "Single shot", "Одиночный выстрел" },
                { "Take cover", "Укрыться" }
            } },
            { "MiningOutpost.Orbital.Services.FirefightEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "The crew falls back, carrying their wounded.", "Экипаж отступает, унося раненых." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The bay is clear.", "Отсек зачищен." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightHealthWindow:DoWindowContents", new Dictionary<string,string> {
                { "Consciousness", "Сознание" },
                { "No health data.", "Нет данных о здоровье." },
                { "Pain", "Боль" }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:.ctor", new Dictionary<string,string> {
                { "Resolver", "Ведение боя" },
                { "Round 1. {0} hostiles, {1} crew.", "Раунд 1. Противников: {0}; членов экипажа: {1}." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:ApplyTacticalAdvantages", new Dictionary<string,string> {
                { " was caught off guard and starts stunned for Round 1!", " не успевает среагировать и начинает первый раунд с оглушением!" },
                { "Dense smoke deployed: crew begins in heavy cover.", "Развёрнута плотная дымовая завеса: экипаж начинает бой в надёжном укрытии." },
                { "Flanking vector secured: crew gains +2 REF initiative bonus.", "Фланговая позиция занята: экипаж получает +2 к броску инициативы (реакция)." },
                { "Tactical Advantage", "Тактическое преимущество" }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:ApplyWound", new Dictionary<string,string> {
                { "is grazed, nothing more.", "отделывается касательным попаданием." },
                { "is stunned and will lose the next round.", "получает оглушение и пропустит следующий раунд." },
                { "takes it on the armour. No wound.", "принимает удар на броню. Раны нет." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:CrewCharge", new Dictionary<string,string> {
                { "charges the ", "бросается в ближний бой. Цель: " },
                { "finds nothing left to close on.", "не находит противника, с которым можно сблизиться." },
                { "lands the blow ({0} to {1}).", "попадает ({0} против {1})." },
                { "swings and misses ({0} to {1}).", "замахивается и промахивается ({0} против {1})." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:CrewShoot", new Dictionary<string,string> {
                { " Hit.", " Попадание." },
                { " Miss.", " Промах." },
                { "fires on the {0} ({1}, DC {2}): {3} + REF {4} + Shooting {5}{6} = {7}.", "стреляет по цели «{0}» ({1}, сложность {2}): {3} + реакция {4} + дальний бой {5}{6} = {7}." },
                { "has no target and holds fire.", "не видит цели и не открывает огонь." },
                { "is dry. The shot never comes.", "остаётся без патронов. Выстрела не происходит." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:DoAim", new Dictionary<string,string> {
                { "sights in (+{0} to the next shot).", "прицеливается (+{0} к следующему выстрелу)." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:DoCover", new Dictionary<string,string> {
                { "drops behind cover.", "ныряет за укрытие." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:DoPatch", new Dictionary<string,string> {
                { "keeps the pressure on.", "продолжает прижимать повязку." },
                { "works a wound closed and holds pressure.", "стягивает края раны и прижимает повязку." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:DoReload", new Dictionary<string,string> {
                { "reloads. {0} rounds.", "перезаряжает оружие; патронов: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:EnemyMelee", new Dictionary<string,string> {
                { "gets a free swing at", "наносит свободный удар" },
                { "scratches at", "бьёт когтями" },
                { "{0} {1} and connects ({2} to {3}).", "{0} по цели «{1}»: попадание ({2} против {3})." },
                { "{0} {1}, dodged ({2} to {3}).", "{0} по цели «{1}»: уклонение ({2} против {3})." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:EnemyShoot", new Dictionary<string,string> {
                { "fires", "стреляет" },
                { "{0} at {1} ({2}, DC {3}): {4} vs {5}. Hit!", "{0} по цели «{1}» ({2}, сложность {3}): {4} против {5}. Попадание!" },
                { "{0} at {1} ({2}, DC {3}): {4} vs {5}. Miss.", "{0} по цели «{1}» ({2}, сложность {3}): {4} против {5}. Промах." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:Finish", new Dictionary<string,string> {
                { "Resolver", "Ведение боя" }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:ResolveCrew", new Dictionary<string,string> {
                { "is reeling and loses the round.", "пошатывается и пропускает раунд." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:ResolveEnemies", new Dictionary<string,string> {
                { " to ", " → " },
                { "closes in (", "сближается (" },
                { "is stunned and reels, unable to act.", "не может действовать из-за оглушения." },
                { "strikes in a fierce flurry.", "наносит яростную серию ударов." }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:ResolveRound", new Dictionary<string,string> {
                { "Initiative to {0} ({1} to {2}).", "Инициатива: {0} ({1} против {2})." },
                { "Resolver", "Ведение боя" },
                { "Round ", "Раунд " },
                { "Round {0}. {1} hostiles left.", "Раунд {0}. Осталось противников: {1}." },
                { "the crew", "экипаж" },
                { "the hostiles", "противники" }
            } },
            { "MiningOutpost.Orbital.Services.FirefightSim:Withdraw", new Dictionary<string,string> {
                { "The crew breaks contact and pulls back, carrying their wounded.", "Экипаж выходит из боя и отступает, унося раненых." }
            } },
            { "MiningOutpost.Orbital.Services.Fnff:RangeName", new Dictionary<string,string> {
                { "Close", "Ближняя" },
                { "Extreme", "Предельная" },
                { "Long", "Дальняя" },
                { "Medium", "Средняя" },
                { "Point Blank", "В упор" }
            } },
            { "MiningOutpost.Orbital.Services.GantrySecurityEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Gantry", "Эстакада" },
                { "Station security officers spot the crate and draw sidearms from the crane catwalk.", "Охранники станции замечают ящик и достают оружие на крановом мостке." }
            } },
            { "MiningOutpost.Orbital.Services.GantrySecurityEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Gantry Security ", "Охранник эстакады " },
                { "fires service pistol", "стреляет из служебного пистолета" }
            } },
            { "MiningOutpost.Orbital.Services.GantrySecurityEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Security officers pin the crew against the gantry rails.", "Охрана прижимает экипаж к перилам эстакады." }
            } },
            { "MiningOutpost.Orbital.Services.GantrySecurityEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Screaming atmospheric exhaust, thirty-meter drop into the lower bays.", "С рёвом выходит воздух; внизу, в тридцати метрах, — нижние отсеки." }
            } },
            { "MiningOutpost.Orbital.Services.GantrySecurityEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Craneway Gantry 12", "Крановая эстакада №12" }
            } },
            { "MiningOutpost.Orbital.Services.GantrySecurityEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The patrol is downed. You secure the pilfered cargo.", "Патруль повержен. Украденный груз под вашим контролем." }
            } },
            { "MiningOutpost.Orbital.Services.HandEval:ShowdownHandText", new Dictionary<string,string> {
                { " and ", " и " },
                { "a ", "" },
                { "a flush", "флеш" },
                { "a full house, ", "фул-хаус: " },
                { "a hand", "комбинация" },
                { "a pair of ", "пара: " },
                { "a straight", "стрит" },
                { "a straight flush", "стрит-флеш" },
                { "four of a kind, ", "каре: " },
                { "high card of ", "старшая карта — " },
                { "three of a kind, ", "тройка: " },
                { "two pair, ", "две пары: " }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:CheckRequirements", new Dictionary<string,string> {
                { "Need {0} silver (have {1})", "Нужно серебра: {0}; в наличии: {1}" },
                { "Requires combat blade in caravan", "Каравану нужно холодное оружие с лезвием" },
                { "Requires explosive / mortar shell in caravan", "Каравану нужна взрывчатка или миномётный снаряд" },
                { "Requires food rations in caravan", "Каравану нужны пищевые припасы" },
                { "Requires precision rifle in caravan", "Каравану нужна высокоточная винтовка" }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:ConfirmAbandon", new Dictionary<string,string> {
                { "Abandon this contract? The job will become permanently unavailable.", "Отказаться от контракта? После этого вернуться к заданию будет невозможно." }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:DrawEnding", new Dictionary<string,string> {
                { "Return to the board", "Вернуться к списку заданий" }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:DrawFirefightLaunch", new Dictionary<string,string> {
                { "Firefight underway...", "Идёт перестрелка…" },
                { "Friday Night Firefight", "Пятничная перестрелка" },
                { "Open fire", "Открыть огонь" },
                { "This drops into a turn-based firefight. The crew fights with their own guns and skills, and the wounds are real.", "Начнётся пошаговая перестрелка. Экипаж использует собственное оружие и навыки, а полученные раны останутся у поселенцев и после боя." }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:DrawOptions", new Dictionary<string,string> {
                { "   |   Consumes 1 ", "   |   Будет потрачен 1 предмет: " },
                { "Accept the job", "Взяться за задание" },
                { "Consumes 1 ", "Будет потрачен 1 предмет: " },
                { "Cost: {0} silver", "Стоимость: {0} серебра" },
                { "Friday Night Firefight", "Пятничная перестрелка" },
                { "Leave (no options here)", "Уйти (доступных действий нет)" },
                { "No eligible crew", "Нет подходящих членов экипажа" },
                { "no crew", "нет экипажа" },
                { "{0}   |   Cost: {1} silver", "{0}   |   Стоимость: {1} серебра" }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:DrawResolvePanel", new Dictionary<string,string> {
                { "Continue", "Продолжить" },
                { "Exploded!", "Максимум! Ещё бросок!" },
                { "Failed", "Провал" },
                { "Resolving", "Проверка" },
                { "Success", "Успех" },
                { "The die explodes: 20 + {0} = {1}", "Переброс при максимуме: 20 + {0} = {1}" }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:DrawStandaloneRoll", new Dictionary<string,string> {
                { "Attempt", "Попытаться" },
                { "No eligible crew", "Нет подходящих членов экипажа" },
                { "Open fire", "Открыть огонь" },
                { "Requirements not met", "Условия не выполнены" },
                { "Resolving", "Проверка" }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:FormatStoryText", new Dictionary<string,string> {
                { "<color=#D09B43>the operative</color>", "<color=#D09B43>оперативник</color>" },
                { "<color=#D09B43>the runner</color>", "<color=#D09B43>курьер</color>" },
                { "<color=#D09B43>the specialist</color>", "<color=#D09B43>специалист</color>" },
                { "the Oxygen Baron", "Кислородный барон" }
            } },
            { "MiningOutpost.Orbital.Services.OpportunityRunner:StartRoll", new Dictionary<string,string> {
                { "no crew", "нет экипажа" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalBlackMarket:OpenTrade", new Dictionary<string,string> {
                { "No colonist in the caravan is able to negotiate.", "В караване нет поселенца, способного вести переговоры." },
                { "The vendor has nothing right now.", "Сейчас у продавца ничего нет." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:BuyConsumable", new Dictionary<string,string> {
                { "Bought {0} {1} for {2} silver.", "Куплено: {1} × {0}. Уплачено серебра: {2}." },
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:ConfirmDestructiveInstall", new Dictionary<string,string> {
                { "Cancel", "Отмена" },
                { "Continue", "Продолжить" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:Draw", new Dictionary<string,string> {
                { "(No surgeons on shift.)", "(Сейчас хирургов нет на смене.)" },
                { "Cybernetic Doctor", "Киберхирург" },
                { "merchant", "торговец" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawBiocodeDetailPanel", new Dictionary<string,string> {
                { "BIOCODED ITEMS", "БИОКОДИРОВАННЫЕ ПРЕДМЕТЫ" },
                { "No biocoded items in the caravan.", "В караване нет биокодированных предметов." },
                { "Pick a biocoded item from the caravan inventory. The doctor will unlock the implant tags so the item can be wielded by anyone again.", "Выберите биокодированный предмет в инвентаре каравана. Врач снимет привязку к владельцу, и пользоваться предметом снова сможет кто угодно." },
                { "Remove biocoding", "Снять биокодирование" },
                { "Remove biocoding for ", "Снять биокодирование: " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawBiocodeListRow", new Dictionary<string,string> {
                { "Biocode disencryption", "Расшифровка биокода" },
                { "Remove biocoding", "Снять биокодирование" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawDetailPanel", new Dictionary<string,string> {
                { " for ", " — " },
                { " silver", " серебра" },
                { "COST", "СТОИМОСТЬ" },
                { "Install on ", "Установить: " },
                { "No colonists in this caravan to install on.", "В караване нет поселенцев, которым можно установить имплант." },
                { "Out of stock", "Нет в наличии" },
                { "RECIPIENT", "ПАЦИЕНТ" },
                { "{0} has no compatible body parts for {1}.", "{0}: нет части тела, подходящей для установки «{1}»." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawPharmacyDetailPanel", new Dictionary<string,string> {
                { "No stock today.", "Сегодня товаров нет." },
                { "Pharmacy", "Аптека" },
                { "Stims, medicine and recovery aids in limited supply.", "Стимуляторы, медикаменты и средства для восстановления. Запасы ограничены." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawPharmacyListRow", new Dictionary<string,string> {
                { "Consumables and medicine", "Расходные материалы и медикаменты" },
                { "Pharmacy", "Аптека" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawPharmacyOutOfStock", new Dictionary<string,string> {
                { "Sold out", "Распродано" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawPharmacyRow", new Dictionary<string,string> {
                { "Buy", "Купить" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawRecipeList", new Dictionary<string,string> {
                { "Out of stock", "Нет в наличии" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawRecipientRow", new Dictionary<string,string> {
                { "Change recipient", "Сменить пациента" },
                { "Examine ", "Осмотреть: " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:DrawTabBar", new Dictionary<string,string> {
                { "Organs", "Органы" },
                { "Services", "Услуги" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:Install", new Dictionary<string,string> {
                { "That colonist is no longer available.", "Этот поселенец больше недоступен." },
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." },
                { "The doctor shrugged. The recipe is unavailable.", "Врач разводит руками: эта операция недоступна." },
                { "{0} already has that implant.", "{0}: такой имплант уже установлен." },
                { "{0} no longer has that body part.", "{0}: этой части тела уже нет." },
                { "{0} returned with a {1}.", "{0}: установлен имплант «{1}»." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:RemoveBiocode", new Dictionary<string,string> {
                { "Biocoding removed from {0}. You paid {1} silver.", "Биокодирование снято: {0}. Уплачено серебра: {1}." },
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalCyberDoc:ShortTierLabel", new Dictionary<string,string> {
                { "Archotech implant", "Архотек-имплант" },
                { "Bionic implant", "Бионический имплант" },
                { "Implant", "Имплант" },
                { "Prosthetic implant", "Протез" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalFuelShop:Draw", new Dictionary<string,string> {
                { "(No fuel types available. Could not find Chemfuel nor VGE_Astrofuel.)", "(Нет доступного топлива: определения Chemfuel и VGE_Astrofuel не найдены.)" },
                { "Fuel shop", "Топливная станция" },
                { "The fuel station will restock in {0} day(s)", "До пополнения запасов топлива: {0} дн." },
                { "Use the hoses to transfer fuel, and the limiter to transfer exact amounts.", "Перекачивайте топливо с помощью шлангов. Чтобы отмерить точное количество, задайте лимит." },
                { "{0} for astrofuel", "Астротопливо: {0}" },
                { "{0} for passenger shuttle refuel", "Заправка пассажирского челнока: {0}" },
                { "{0} for stored chemfuel", "Запасы химтоплива: {0}" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalFuelShop:DrawCargoCard", new Dictionary<string,string> {
                { "Cargo", "Груз" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalFuelShop:DrawLimiter", new Dictionary<string,string> {
                { "{0} ({1} silver)", "{0} ({1} серебра)" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalFuelShop:DrawNozzleCard", new Dictionary<string,string> {
                { "{0} units in stock", "В наличии: {0} ед." },
                { "{0}/unit", "{0}/ед." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalFuelShop:DrawShuttleCard", new Dictionary<string,string> {
                { "No passenger shuttle.", "Пассажирского челнока нет." },
                { "Passenger shuttle", "Пассажирский челнок" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalFuelShop:TryCommit", new Dictionary<string,string> {
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." },
                { "You Spent {0} silver.", "Потрачено серебра: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable+<>c__DisplayClass61_1:<OpenSwapPawnMenuForAnesthetic>b__1", new Dictionary<string,string> {
                { "Not enough silver.", "Недостаточно серебра." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmAllIn", new Dictionary<string,string> {
                { "All in", "Ва-банк" },
                { "Yes", "Да" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmExitTable", new Dictionary<string,string> {
                { "Exit table", "Выйти из-за стола" },
                { "Keep playing", "Продолжить игру" },
                { "Leave", "Уйти" },
                { "Leave with $", "Покинуть стол и забрать $" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmWagerGear", new Dictionary<string,string> {
                { "Wager equipment", "Поставить снаряжение" },
                { "Yes", "Да" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmWagerOrgan", new Dictionary<string,string> {
                { "Yes", "Да" },
                { "the pawn", "Поселенец" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmWagerOrganInner", new Dictionary<string,string> {
                { "Wager body part", "Поставить часть тела" },
                { "Yes", "Да" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawBuyInScreen", new Dictionary<string,string> {
                { " buy-in.</color>", ".</color>" },
                { "; small blind ", "; малый блайнд: " },
                { "<color=#E96C6C>You cannot afford the $", "<color=#E96C6C>Недостаточно серебра для входа: $" },
                { "Big blind ", "Большой блайнд: " },
                { "Buy-in", "Вступительный взнос" },
                { "Caravan silver: <color=#F5C654>$", "Серебро в караване: <color=#F5C654>$" },
                { "No conscious colonists available to seat at the table.", "Нет поселенцев в сознании, способных сесть за стол." },
                { "Pick a buy-in and a colonist to sit down at the table.", "Выберите вступительный взнос и поселенца, который сядет за стол." },
                { "Poker", "Покер" },
                { "Seat", "Игрок" },
                { "Sit at table", "Сесть за стол" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawCommunityRow", new Dictionary<string,string> {
                { "Pot  ", "Банк: " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawDefaultActionRows", new Dictionary<string,string> {
                { "All in", "Ва-банк" },
                { "Call ", "Уравнять: " },
                { "Check", "Чек" },
                { "Exit table", "Выйти из-за стола" },
                { "Fold", "Пас" },
                { "Only one body part wager per hand.", "В каждой раздаче можно поставить только одну часть тела." },
                { "Raise using silver", "Поднять ставку серебром" },
                { "Wager body part", "Поставить часть тела" },
                { "Wager equipment", "Поставить снаряжение" },
                { "You can't stand up mid-hand. Wait for the hand to finish.", "Во время раздачи встать из-за стола нельзя. Дождитесь её окончания." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawOpponentPanel", new Dictionary<string,string> {
                { " has lost <color=#E96C6C>-", " — убыток: <color=#E96C6C>-" },
                { " has made <color=#8CD96B>+", " — прибыль: <color=#8CD96B>+" },
                { " is breaking even since you sat down.", " играет в ноль с момента вашего появления." },
                { "</color> since you sat down.", "</color> с момента вашего появления." },
                { "In ", "Внесено: " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawPawnPicker", new Dictionary<string,string> {
                { "Swap colonist", "Сменить поселенца" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawPlayerPanel", new Dictionary<string,string> {
                { "  It's your turn.", "  Ваш ход." },
                { "</color> since sitting down.", "</color> с начала игры." },
                { "In  ", "Внесено: " },
                { "You have ", "Ваша комбинация: " },
                { "You have lost <color=#E96C6C>-", "Ваш убыток: <color=#E96C6C>-" },
                { "You have made <color=#8CD96B>+", "Ваша прибыль: <color=#8CD96B>+" },
                { "You're breaking even since sitting down.", "С начала игры у вас нет ни прибыли, ни убытка." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawRaiseSliderPanel", new Dictionary<string,string> {
                { "1/2 pot", "½ банка" },
                { "1/3 pot", "⅓ банка" },
                { "2/3 pot", "⅔ банка" },
                { "All in", "Ва-банк" },
                { "Cancel", "Отмена" },
                { "Confirm", "Подтвердить" },
                { "Exit table", "Выйти из-за стола" },
                { "Min raise", "Мин. повышение" },
                { "Pot", "Банк" },
                { "Raise to:", "Поднять до:" },
                { "You can't stand up mid-hand. Wait for the hand to finish.", "Во время раздачи встать из-за стола нельзя. Дождитесь её окончания." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawTable", new Dictionary<string,string> {
                { "Close", "Закрыть" },
                { "Gamble for change", "Игра с судьбой" },
                { "Hand #", "Раздача №" },
                { "Log", "Журнал" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:FlushLossPopups", new Dictionary<string,string> {
                { "Lost", "Проигрыш" },
                { "OK", "ОК" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:FlushRivalWinPrompts", new Dictionary<string,string> {
                { "Keep", "Оставить себе" },
                { "Sell", "Продать" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:HandleAnestheticPromptIfNeeded", new Dictionary<string,string> {
                { "Body part harvested", "Часть тела изъята" },
                { "Pawn", "Поселенец" },
                { "Yes", "Да" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:OpenSwapPawnMenuForAnesthetic", new Dictionary<string,string> {
                { "  (needs $", "  (нужно $" },
                { "  rebuy $", "  повторный взнос: $" },
                { "No other conscious colonists. Cashing out.", "Других поселенцев в сознании нет. Забираем деньги и выходим." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:OpenWagerOrganMenu", new Dictionary<string,string> {
                { "Only one body part wager per hand.", "В каждой раздаче можно поставить только одну часть тела." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalGamblingTable:StartSession", new Dictionary<string,string> {
                { "Not enough silver.", "Недостаточно серебра." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers+<>c:<Draw>b__17_12", new Dictionary<string,string> {
                { "Repair to ", "Починить до " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers+<>c:<Draw>b__17_15", new Dictionary<string,string> {
                { "Maintain to ", "Обслужить до " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers:Draw", new Dictionary<string,string> {
                { "(Vanilla Gravship Expanded failed to load.)", "(Не удалось загрузить Vanilla Gravship Expanded.)" },
                { "Hire Engineers", "Нанять инженеров" },
                { "Maintenance", "Обслуживание" },
                { "Repair", "Ремонт" },
                { "The staging bays bustle with fitters, cutters, and laborers that will shuttle out to your gravship's coordinates.", "В сборочных доках кипит работа. Монтажники, резчики и разнорабочие готовы вылететь к вашему гравикораблю." },
                { "Your gravship's major components are in good shape.", "Основные узлы вашего гравикорабля в хорошем состоянии." },
                { "{0} damaged", "Повреждено: {0}" },
                { "{0} need maintenance", "Требует обслуживания: {0}" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers:DrawAllAndSlider", new Dictionary<string,string> {
                { "All", "Всё" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers:DrawCostLine", new Dictionary<string,string> {
                { "{0} silver", "{0} серебра" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers:MaintRowPreview", new Dictionary<string,string> {
                { "% maintained", "% обслужено" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers:RepairRowPreview", new Dictionary<string,string> {
                { " HP", " ед. прочности" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalHireEngineers:TryDispatch", new Dictionary<string,string> {
                { "Dock engineers repaired {0} components and completed maintenance on {1}. You paid {2} silver.", "Инженеры из доков отремонтировали узлов: {0}; обслужили: {1}. Уплачено серебра: {2}." },
                { "No engineering work selected.", "Работы не выбраны." },
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalMaintenanceShop:BuildPriceTooltip", new Dictionary<string,string> {
                { "\n\nUnit price: ", "\n\nЦена за единицу: " },
                { "\n  x ", "\n  × " },
                { " (buying)", " (покупка)" },
                { " (weapon)", " (оружие)" },
                { "Base value: ", "Базовая стоимость: " },
                { "Market value: ", "Рыночная стоимость: " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalMaintenanceShop:Draw", new Dictionary<string,string> {
                { "Maintenance shop", "Ремонтная мастерская" },
                { "maintenance shop", "магазин запчастей" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalMaintenanceShop:DrawOutOfStockRow", new Dictionary<string,string> {
                { "Out of stock", "Нет в наличии" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalMaintenanceShop:DrawPipeVariantDropdown", new Dictionary<string,string> {
                { "Hidden", "Скрытый" },
                { "Switch between visible and hidden variant", "Переключить между обычным и скрытым вариантом" },
                { "Visible", "Обычный" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalMaintenanceShop:RarityLabel", new Dictionary<string,string> {
                { "extremely rare", "крайне редко встречается" },
                { "eye-wateringly rare", "невероятная редкость" },
                { "markup", "наценка" },
                { "material", "материал" },
                { "rare", "редко встречается" },
                { "spare parts", "запчасти" },
                { "uncommon", "нечасто встречается" },
                { "very rare", "очень редко встречается" },
                { "very uncommon", "мало где встречается" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalMaintenanceShop:TryAcceptCart", new Dictionary<string,string> {
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." },
                { "You Spent {0} silver.", "Потрачено серебра: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOpportunitiesBoard:DrawList", new Dictionary<string,string> {
                { "More jobs coming soon.", "Позже появятся новые задания." },
                { "No money, no fuel, no way home. There is one last, undignified way down.", "Нет денег, топлива и пути домой. Остаётся один последний, унизительный способ спуститься." },
                { "Pay chemfuel and silver to descend your pods to a destination.", "Заплатите химтопливом и серебром, чтобы спустить капсулы к выбранному месту." },
                { "Relaunch pods", "Повторный запуск капсул" },
                { "Sell organic matter to the platform's agronomist.", "Продайте органическое сырьё агроному платформы." },
                { "Stranded?", "Застряли?" },
                { "The Soup", "Варево" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOpportunitiesBoard:DrawSheetToggle", new Dictionary<string,string> {
                { "Character sheets", "Карточки персонажей" },
                { "View squad character sheets", "Посмотреть карточки отряда" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOpportunitiesBoard:HeaderBody", new Dictionary<string,string> {
                { "The orbital platform feels a lot colder when your pockets are empty. You are down to absolute zero. You have no silver, no stock, and less than 50 chemfuel. You don't own the berth you're docked in, and you certainly don't own the air you're breathing.", "Орбитальная платформа кажется куда холоднее, когда карманы пусты. Вы на полном нуле: нет ни серебра, ни товаров, а химтоплива меньше 50 единиц. Ни причал, к которому вы пристыкованы, ни воздух, которым вы дышите, вам не принадлежат." },
                { "Thick, opaque glass tubes bubble with a murky green chemical sludge behind heavy industrial warning stripes, hydraulic crushing pistons, and steam vents releasing organic vapor. The deep, rhythmic, disturbing sound of a heavy-duty industrial blender followed by a wet, bubbling gurgle loops relentlessly.", "За толстыми мутными стёклами труб бурлит зеленоватая химическая жижа. Вокруг — широкие промышленные предупреждающие полосы, гидравлические дробящие поршни и паровые сопла, выпускающие органические испарения. Без конца повторяется тяжёлый, ритмичный и тревожный грохот промышленного блендера, сменяющийся влажным бульканьем." },
                { "Your pods sit inside a simple machine: a controlled chemfuel detonation erupts inside the sealed launch tube, building enough sudden pressure to violently spit the capsule out into the void. It’s essentially an incredibly inefficient, outrageously expensive dumbwaiter designed to blast your steel coffin face-first back into the dirt.", "Ваши капсулы помещаются в нехитрый механизм: управляемый взрыв химтоплива в герметичной пусковой трубе создаёт резкий скачок давления и с силой выплёвывает капсулу в пустоту. По сути это невероятно неэффективный и неприлично дорогой грузовой лифт, созданный, чтобы выстрелить вашим стальным гробом носом прямо обратно в землю." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOpportunitiesBoard:HeaderTitle", new Dictionary<string,string> {
                { "Opportunity", "Предложение" },
                { "Relaunch pods", "Повторный запуск капсул" },
                { "Stranded?", "Застряли?" },
                { "The Soup", "Варево" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganDemand:TagFor", new Dictionary<string,string> {
                { "High demand", "Высокий спрос" },
                { "Low demand", "Низкий спрос" },
                { "Steady demand", "Стабильный спрос" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:BuyOrgan", new Dictionary<string,string> {
                { "Bought {0} for {1} silver.", "Куплено: {0}. Уплачено серебра: {1}." },
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:DrawBuyColumn", new Dictionary<string,string> {
                { "FOR SALE", "В ПРОДАЖЕ" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:DrawBuyDetail", new Dictionary<string,string> {
                { "Buy for ", "Купить за " },
                { "COST", "СТОИМОСТЬ" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:DrawDetailShell", new Dictionary<string,string> {
                { " silver", " серебра" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:DrawSellColumn", new Dictionary<string,string> {
                { "Clinic budget: {0} silver", "Бюджет клиники: {0} серебра" },
                { "No organs in the caravan.", "В караване нет органов на продажу." },
                { "SELL YOUR ORGANS", "ПРОДАЖА ОРГАНОВ" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:DrawSellDetail", new Dictionary<string,string> {
                { "Clinic budget: {0} silver", "Бюджет клиники: {0} серебра" },
                { "OFFER", "ПРЕДЛОЖЕНИЕ" },
                { "Sell for ", "Продать за " }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalOrganShop:SellOrgan", new Dictionary<string,string> {
                { "Sold {0} for {1} silver.", "Продано: {0}. Получено серебра: {1}." },
                { "The clinic has spent its organ-buying budget for this cycle.", "Клиника уже израсходовала бюджет на закупку органов в этом цикле." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalShopUtility:DrawCartFooter", new Dictionary<string,string> {
                { "Accept", "Принять" },
                { "Reset", "Сбросить" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalShopUtility:DrawRestockTooltip", new Dictionary<string,string> {
                { "This {0} has fresh stock.", "{0}: ассортимент обновлён." },
                { "This {0} will rotate stock in {1} day(s).", "{0}: до смены ассортимента — {1} дн." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalShopUtility:DrawShopRow", new Dictionary<string,string> {
                { "Buy", "Купить" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalShopUtility:DrawTradeHeader", new Dictionary<string,string> {
                { "Silver", "Серебро" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalShuttleCargo:DepositWinnings", new Dictionary<string,string> {
                { "Convert to gold", "Обменять на золото" },
                { "Keep silver", "Оставить серебро" },
                { "Shuttle full", "Челнок заполнен" },
                { "Your passenger shuttle cannot hold any more!", "В пассажирском челноке больше нет места!" },
                { "Your passenger shuttle cannot hold any more! Would you like to convert {0} silver into {1} gold?", "В пассажирском челноке больше нет места! Обменять {0} серебра на {1} золота?" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalShuttleCargo:RejectIfOverCapacity", new Dictionary<string,string> {
                { "Your passenger shuttle cannot hold any more!", "В пассажирском челноке больше нет места!" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVirtualTrader:GiveSoldThingToPlayer", new Dictionary<string,string> {
                { "Your passenger shuttle cannot hold any more!", "В пассажирском челноке больше нет места!" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVirtualTrader:get_TraderName", new Dictionary<string,string> {
                { "Orbital Black Market", "Орбитальный чёрный рынок" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:BuildSkillTooltip", new Dictionary<string,string> {
                { " (major passion)", " (страсть)" },
                { " (passion)", " (интерес)" },
                { "(no skill data)", "(нет сведений о навыках)" },
                { "(no useful skills)", "(нет полезных навыков)" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:Draw", new Dictionary<string,string> {
                { "Seek volunteers", "Поиск добровольцев" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:DrawCandidateCard", new Dictionary<string,string> {
                { "Recruit  ({0})", "Завербовать ({0})" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:DrawConsumedCard", new Dictionary<string,string> {
                { "Already recruited.", "Вербовка завершена." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:DrawRecruitedGearCard", new Dictionary<string,string> {
                { "Buy helmet  ({0})", "Купить шлем ({0})" },
                { "Buy vacsuit  ({0})", "Купить скафандр ({0})" },
                { "The dock will fit a vacsuit and matching helmet for a fee.", "За отдельную плату в доках подберут скафандр и подходящий шлем." },
                { "{0} has joined the caravan.", "В караване новый участник: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:DrawVolunteerRestockTooltip", new Dictionary<string,string> {
                { "This hub has fresh volunteers.", "Здесь появились новые добровольцы." },
                { "This hub will have new volunteers in {0} day(s).", "Новые добровольцы появятся здесь через {0} дн." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:Recruit", new Dictionary<string,string> {
                { "The caravan cannot afford this deal.", "В караване не хватает серебра на эту сделку." },
                { "{0} has joined the caravan.", "В караване новый участник: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:ResolveRoleTitle", new Dictionary<string,string> {
                { "drifter", "бродяга" }
            } },
            { "MiningOutpost.Orbital.Services.OrbitalVolunteerHall:TryBuyGear", new Dictionary<string,string> {
                { "You cannot afford to buy the gear.", "Не хватает серебра на снаряжение." }
            } },
            { "MiningOutpost.Orbital.Services.OrganWager:get_LabelCap", new Dictionary<string,string> {
                { "Body part", "Часть тела" }
            } },
            { "MiningOutpost.Orbital.Services.Patch_OrbitalBlackMarket_PriceTooltip:Postfix", new Dictionary<string,string> {
                { "\n  x ", "\n  × " },
                { " (black market)", " (чёрный рынок)" }
            } },
            { "MiningOutpost.Orbital.Services.PodDescent:LaunchAsPods", new Dictionary<string,string> {
                { "The caravan has no one to relaunch.", "В караване некому возвращаться на капсулах." }
            } },
            { "MiningOutpost.Orbital.Services.PodDescent:LaunchPodsTo", new Dictionary<string,string> {
                { "No safe landing site could be found on the surface below.", "На поверхности внизу не найдено безопасного места посадки." },
                { "You do not have enough silver.", "Недостаточно серебра." },
                { "{0} is fired into space.", "Отправка в космос: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.PodDescent:LaunchPodsToCell", new Dictionary<string,string> {
                { "No safe landing site could be found on the surface below.", "На поверхности внизу не найдено безопасного места посадки." },
                { "You do not have enough silver.", "Недостаточно серебра." },
                { "{0} is fired into space.", "Отправка в космос: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.PodDescent:TryJettison", new Dictionary<string,string> {
                { "No safe landing site could be found on the surface below.", "На поверхности внизу не найдено безопасного места посадки." },
                { "{0} is jettisoned in a cargo pod.", "Сброшено в грузовой капсуле: {0}." }
            } },
            { "MiningOutpost.Orbital.Services.PokerLogWindow:DoWindowContents", new Dictionary<string,string> {
                { "(no active table)", "(нет активного стола)" },
                { "Log", "Журнал" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:AdvanceStreet", new Dictionary<string,string> {
                { "Flop: ", "Флоп: " },
                { "River: ", "Ривер: " },
                { "Turn: ", "Тёрн: " }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:AnnounceHeatIfChanged", new Dictionary<string,string> {
                { " yawned.", " зевает." },
                { "Table", "Стол" },
                { "That hand was boring.", "Скучная вышла раздача." },
                { "The table is cooling down.", "Азарт за столом остывает." },
                { "The table is hot!", "За столом разгорелась игра!" },
                { "The table is lukewarm.", "За столом пока без особого азарта." },
                { "The table is on fire!", "За столом страсти кипят!" },
                { "The table is warming up.", "Игра становится азартнее." }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:ApplySeatAction", new Dictionary<string,string> {
                { "checks.", "объявляет чек." },
                { "folds.", "пасует." }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:BuildDeathContextText", new Dictionary<string,string> {
                { "Colonist", "Поселенец" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:DealNewHand", new Dictionary<string,string> {
                { "joins the table with ", "садится за стол с суммой " },
                { "leaves the table.", "покидает стол." }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:EndHand", new Dictionary<string,string> {
                { "wins uncontested.", "выигрывает без вскрытия: все соперники спасовали." }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:HandCategoryReadable", new Dictionary<string,string> {
                { "a ", "" },
                { "a hand", "комбинация" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:LogDealer", new Dictionary<string,string> {
                { "Dealer", "Дилер" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:MakeFreshRival", new Dictionary<string,string> {
                { "Drey", "Дрей" },
                { "Greel", "Грил" },
                { "Husk", "Хаск" },
                { "Marl", "Марл" },
                { "Pell", "Пелл" },
                { "Sable", "Сейбл" },
                { "Vex", "Векс" },
                { "Yssa", "Исса" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:PickRandomSeatName", new Dictionary<string,string> {
                { "Someone", "Кто-то" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:PlayerHandReadable", new Dictionary<string,string> {
                { "High card (", "Старшая карта (" },
                { "Pocket pair (", "Карманная пара (" },
                { "s)", ")" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:PositionTag", new Dictionary<string,string> {
                { "Big blind", "Большой блайнд" },
                { "Dealer", "Дилер" },
                { "Small blind", "Малый блайнд" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:PostDealInit", new Dictionary<string,string> {
                { ". Dealer: ", ". Раздаёт: " },
                { "Hand #", "Раздача №" },
                { "pays big blind ", "вносит большой блайнд: " },
                { "pays small blind ", "вносит малый блайнд: " }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:PrintRelationshipFlavor", new Dictionary<string,string> {
                { "is feeling confident", "чувствует уверенность." },
                { "is feeling greedy", "жадничает." }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:ResolvePlayerWagers", new Dictionary<string,string> {
                { "Colonist", "Поселенец" },
                { "The winner", "Победитель" },
                { "body part", "часть тела" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:StreetLabel", new Dictionary<string,string> {
                { "Flop", "Флоп" },
                { "Pre-flop", "Префлоп" },
                { "River", "Ривер" },
                { "Showdown", "Вскрытие" },
                { "Turn", "Тёрн" }
            } },
            { "MiningOutpost.Orbital.Services.PokerSim:SwapPlayerPawn", new Dictionary<string,string> {
                { "takes the seat with ", "садится за стол с " }
            } },
            { "MiningOutpost.Orbital.Services.RatHuntEncounter+<Intro>d__4:MoveNext", new Dictionary<string,string> {
                { "Bay 7", "Отсек №7" },
                { "Queue an order for each of the crew, then resolve the round.", "Назначьте приказ каждому члену экипажа, затем выполните раунд." },
                { "Resolver", "Ведение боя" },
                { "The pallets shift. Something runs along the top of the stacks and drops out of sight.", "Поддоны сдвигаются. Что-то пробегает по верху штабелей и исчезает из виду." }
            } },
            { "MiningOutpost.Orbital.Services.RatHuntEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Bay rat ", "Крыса грузового отсека " },
                { "Scratch and bite", "царапается и кусается" }
            } },
            { "MiningOutpost.Orbital.Services.RatHuntEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Grain stock, killed lights, more eyes than the brief said.", "Запасы зерна, погашенный свет и больше глаз, чем обещал инструктаж." }
            } },
            { "MiningOutpost.Orbital.Services.RatHuntEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Bay 7", "Отсек №7" }
            } },
            { "MiningOutpost.Orbital.Services.SafehouseMercsEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "A thermite charge blows the rear blast door inwards! Syndicate mercenaries pour in.", "Термитный заряд выбивает заднюю бронированную дверь внутрь! В проём устремляются наёмники синдиката." },
                { "Safehouse", "Убежище" }
            } },
            { "MiningOutpost.Orbital.Services.SafehouseMercsEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Syndicate Interceptor ", "Боец отряда перехвата синдиката " },
                { "fires burst carbine", "стреляет очередью из карабина" }
            } },
            { "MiningOutpost.Orbital.Services.SafehouseMercsEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Crossfire suppresses the position. Crew retreats with casualties.", "Позиция подавлена перекрёстным огнём. Экипаж отступает с потерями." }
            } },
            { "MiningOutpost.Orbital.Services.SafehouseMercsEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Flashing red sirens, tear gas canisters rolling across metal grates.", "Мигают красные огни тревоги, по металлическим решёткам катятся баллоны со слезоточивым газом." }
            } },
            { "MiningOutpost.Orbital.Services.SafehouseMercsEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Sub-Deck Safehouse", "Убежище под палубой" }
            } },
            { "MiningOutpost.Orbital.Services.SafehouseMercsEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The interceptor squad is eliminated. The safehouse corridor is secure.", "Отряд перехвата уничтожен. Коридор убежища под контролем." }
            } },
            { "MiningOutpost.Orbital.Services.SpireBodyguardEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Penthouse", "Пентхаус" },
                { "The executive screams for security as his personal bodyguard draws a chrome pistol.", "Руководитель с криком зовёт охрану, а его личный телохранитель достаёт хромированный пистолет." }
            } },
            { "MiningOutpost.Orbital.Services.SpireBodyguardEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Elite Chrome Bodyguard", "Элитный телохранитель с имплантами" },
                { "Spire Guard ", "Охранник шпиля " },
                { "fires auto-carbine", "стреляет из автоматического карабина" },
                { "fires chrome flechette pistol", "стреляет из хромированного игольчатого пистолета" }
            } },
            { "MiningOutpost.Orbital.Services.SpireBodyguardEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "High-velocity rounds punch through cover. Crew abandons the spire.", "Высокоскоростные пули пробивают укрытия. Экипаж покидает шпиль." }
            } },
            { "MiningOutpost.Orbital.Services.SpireBodyguardEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Panoramic vista of the orbital ring, marble bar counter shattered by gunfire.", "Панорама орбитального кольца; мраморная барная стойка расколота выстрелами." }
            } },
            { "MiningOutpost.Orbital.Services.SpireBodyguardEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Penthouse Viewing Terrace", "Обзорная терраса пентхауса" }
            } },
            { "MiningOutpost.Orbital.Services.SpireBodyguardEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The elite bodyguard collapses. You secure the dataspike and slip out.", "Элитный телохранитель падает. Вы забираете накопитель с данными и ускользаете." }
            } },
            { "MiningOutpost.Orbital.Services.SquadSheetUI:Draw", new Dictionary<string,string> {
                { "(no crew on this caravan)", "(в этом караване нет экипажа)" },
                { "Colony", "Колония" },
                { "Sift through active pawn sheets, skill checks, and systemic health conditions.", "Просмотрите карточки участников, проверки навыков и состояния здоровья, влияющие на характеристики." }
            } },
            { "MiningOutpost.Orbital.Services.SquadSheetUI:DrawHealth", new Dictionary<string,string> {
                { "Consciousness", "Сознание" },
                { "Health", "Здоровье" },
                { "Pain", "Боль" }
            } },
            { "MiningOutpost.Orbital.Services.SquadSheetUI:DrawSheet", new Dictionary<string,string> {
                { "Skills", "Навыки" }
            } },
            { "MiningOutpost.Orbital.Services.SquadSheetUI:JobTitle", new Dictionary<string,string> {
                { "Crew", "Экипаж" }
            } },
            { "MiningOutpost.Orbital.Services.StatLab:Derive", new Dictionary<string,string> {
                { "Base", "Базовое значение" },
                { "Bionics", "Бионика" },
                { "Bonded animals", "Животные с привязанностью" },
                { "Heavy frame", "Тучное телосложение" },
                { "Hulk frame", "Мускулистое телосложение" },
                { "Humanity loss", "Утрата человечности" },
                { "Lifetime kills", "Убийств за всю жизнь" },
                { "Thin frame", "Худощавое телосложение" }
            } },
            { "MiningOutpost.Orbital.Services.StatLab:Explain", new Dictionary<string,string> {
                { "Chrome menace", "Угрожающий вид имплантов" },
                { "Hulk (stealth)", "Мускулистое телосложение (скрытность)" },
                { "Imposing frame", "Внушительное телосложение" },
                { "Thin (stealth)", "Худощавое телосложение (скрытность)" }
            } },
            { "MiningOutpost.Orbital.Services.Stranded+<>c__DisplayClass10_0:<Draw>b__0", new Dictionary<string,string> {
                { "No safe landing site could be found on the surface below.", "На поверхности внизу не найдено безопасного места посадки." }
            } },
            { "MiningOutpost.Orbital.Services.Stranded:Draw", new Dictionary<string,string> {
                { "Climb into the cargo pod?\n\nYour shuttle will be left behind, scrapped and destroyed. You will be jettisoned to a random spot on the surface below. This cannot be undone.", "Забраться в грузовую капсулу?\n\nВаш челнок останется здесь: его разберут на лом и уничтожат. Вас выбросит в случайное место на поверхности внизу. Отменить это будет нельзя." },
                { "Pull the manual release lever", "Дёрнуть ручной рычаг сброса" },
                { "Someone", "Кто-то" }
            } },
            { "MiningOutpost.Orbital.Services.Stranded:RollFlavor", new Dictionary<string,string> {
                { "Someone", "Кто-то" }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:BuildOutcomeText", new Dictionary<string,string> {
                { "The high-quality material will be used to grow sheets of printed tissue and synthetic skin for the platform's surgeons.", "Качественный материал пойдёт на выращивание листов печатной ткани и синтетической кожи для хирургов платформы." },
                { "The lipids will be rendered and turned into high-grade chemfuel and industrial grease to lubricate the station's heavy machinery and air scrubbers.", "Жиры вытопят и превратят в качественное химтопливо и промышленную смазку для тяжёлого оборудования станции и очистителей воздуха." },
                { "The liquid violently churns, and thick, dark sludge splashes against the glass. Because the tissue is already liquefying and contaminated with bacteria, it cannot be used for any medical or industrial grade applications. The slurry will be chemically sterilized and broken down into low-grade agricultural nitrates.", "Жидкость яростно бурлит, густая тёмная жижа бьётся о стекло. Ткани уже разлагаются и заражены бактериями, поэтому не годятся для медицинских или промышленных нужд. Массу химически обеззаразят и разложат на низкосортные нитратные удобрения." },
                { "The liquid violently churns, and thick, dark sludge splashes against the glass. It is a borderline insult to dump clean, edible proteins into a chemical liquefier. The agronomist openly thinks it's a waste that you didn't just eat it, and rather traded it for a handful of silver.", "Жидкость яростно бурлит, густая тёмная жижа бьётся о стекло. Отправлять чистый съедобный белок в химический разжижитель — почти оскорбление. Агроном не скрывает своего мнения: вам следовало съесть это, а не менять на горсть серебра." },
                { "The liquid violently churns, and thick, dark sludge splashes against the glass. Relieved, the agronomist tells you the slurry will be used for fertilizer, and that the station's hydroponic basins were beginning to look like sand.\n\n", "Жидкость яростно бурлит, густая тёмная жижа бьётся о стекло. Агроном с облегчением сообщает, что масса пойдёт на удобрения: гидропонные бассейны станции уже начали напоминать песок.\n\n\n" },
                { "The liquid violently churns, and thick, dark sludge splashes against the glass. With the yield consisting mostly of calcium and hair, the remains are processed with minimal mechanical resistance. Lacking the necessary soft tissues, lipids, and moisture the brittle remains will be pulverized into a dry phosphate powder and added to the soil.", "Жидкость яростно бурлит, густая тёмная жижа бьётся о стекло. Сырьё состоит в основном из кальция и волос, поэтому почти не сопротивляется обработке. Мягких тканей, жиров и влаги уже нет: хрупкие останки измельчат в сухой фосфатный порошок и внесут в почву." },
                { "The station refines the slurry into high-tensile bioplastics to reinforce the structural seals and failing bulkheads on the lower decks.", "На станции массу переработают в высокопрочный биопластик, чтобы укрепить силовые уплотнения и разрушающиеся переборки нижних палуб." }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:CatTag", new Dictionary<string,string> {
                { "Fresh", "Свежий труп" },
                { "Plant/Animal", "Растения / животные" },
                { "Rotting", "Гниющий труп" },
                { "Skeleton", "Скелет" }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:CorpseTip", new Dictionary<string,string> {
                { "\n{0} silver", "\n{0} серебра" },
                { "\n{0} x {1} = {2} silver", "\n{0} × {1} = {2} серебра" },
                { "Animal or plant matter. Worth 45% of its market value.", "Животное или растительное сырьё. Стоит 45% от его рыночной цены." },
                { "Dessicated humanoid remains. Worth 30% of a fresh corpse.", "Высохшие останки гуманоида. Стоят 30% от цены свежего трупа." },
                { "Fresh humanoid corpse. Full reclamation value from meat and leather yield.", "Свежий труп гуманоида. Полная стоимость переработки по выходу мяса и кожи." },
                { "Rotting humanoid corpse. Worth 60% of a fresh corpse.", "Гниющий труп гуманоида. Стоит 60% от цены свежего трупа." }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:Deliver", new Dictionary<string,string> {
                { "Add organic matter to the cart before delivering.", "Перед сдачей добавьте органическое сырьё в корзину." }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:DrawBriefing", new Dictionary<string,string> {
                { "Accept", "Принять" },
                { "The agronomist's vats are full. This opportunity is complete.", "Чаны агронома полны. Предложение выполнено." },
                { "This opportunity has expired.", "Срок действия предложения истёк." },
                { "Throwing organic matter out of an airlock is a horrific waste of precious nitrogen, carbon, and water. Everything must be reclaimed. The chief agronomist on this platform requires all organic matter be delivered here. He is particularly interested in maximizing yield and will pay a premium price for fresh humanoid corpses.", "Выбрасывать органику через шлюз — чудовищная трата драгоценного азота, углерода и воды. Всё нужно перерабатывать. Главный агроном платформы требует доставлять органическое сырьё сюда. Он особенно заинтересован в максимальном выходе продукции и готов щедро платить за свежие трупы гуманоидов." }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:DrawCart", new Dictionary<string,string> {
                { "Cart contents", "Содержимое корзины" },
                { "The agronomist's vats are full. This opportunity is complete.", "Чаны агронома полны. Предложение выполнено." },
                { "The caravan is carrying no organic matter to deliver.", "В караване нет органического сырья для сдачи." },
                { "This opportunity has expired.", "Срок действия предложения истёк." },
                { "Throwing organic matter out of an airlock is a horrific waste of precious nitrogen, carbon, and water. Everything must be reclaimed. The chief agronomist on this platform requires all organic matter be delivered here. He is particularly interested in maximizing yield and will pay a premium price for fresh humanoid corpses.", "Выбрасывать органику через шлюз — чудовищная трата драгоценного азота, углерода и воды. Всё нужно перерабатывать. Главный агроном платформы требует доставлять органическое сырьё сюда. Он особенно заинтересован в максимальном выходе продукции и готов щедро платить за свежие трупы гуманоидов." }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:DrawDeliverFooter", new Dictionary<string,string> {
                { "Deliver", "Сдать" }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:DrawResult", new Dictionary<string,string> {
                { "Ok", "Хорошо" },
                { "You were paid {0} silver.", "Вы получили {0} серебра." }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:DrawStatusTags", new Dictionary<string,string> {
                { "Offer expires in {0} day(s)", "До окончания предложения: {0} дн." },
                { "The agronomist has {0} silver left to spend", "У агронома осталось {0} серебра на закупки" }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:DrawTotalRow", new Dictionary<string,string> {
                { "Total value:", "Общая стоимость:" }
            } },
            { "MiningOutpost.Orbital.Services.TheSoup:TierTooltip", new Dictionary<string,string> {
                { "Fresh humanoid delivery: full premium (x1.1). The agronomist pays top rate for fresh, intact tissue.", "Свежий труп гуманоида: цена с надбавкой (×1,1). Агроном хорошо платит за свежие неповреждённые ткани." },
                { "Humanoid skeleton delivery: reduced (x0.8). Dry, brittle remains yield little but calcium and phosphate.", "Скелет гуманоида: сниженная цена (×0,8). Из сухих хрупких останков почти нечего извлечь, кроме кальция и фосфатов." },
                { "Plant or animal matter delivery: reduced (x0.7). A waste of clean protein the agronomist thinks you should have eaten.", "Растительное или животное сырьё: сниженная цена (×0,7). Агроном считает такую трату полноценного белка расточительством: это стоило съесть." },
                { "Rotting humanoid delivery: reduced (x0.9). Liquefying, contaminated tissue is only fit for low-grade nitrates.", "Гниющий труп гуманоида: сниженная цена (×0,9). Разлагающиеся заражённые ткани годятся лишь на низкосортные нитраты." }
            } },
            { "MiningOutpost.Orbital.Services.TransportPods+<>c__DisplayClass16_0:<BeginDeckCellTargeting>b__1", new Dictionary<string,string> {
                { "Cannot land pods here.", "Здесь нельзя посадить капсулы." },
                { "You do not have enough silver.", "Недостаточно серебра." }
            } },
            { "MiningOutpost.Orbital.Services.TransportPods:ChosePodDestination", new Dictionary<string,string> {
                { "Cannot land pods here.", "Здесь нельзя посадить капсулы." },
                { "You do not have enough silver.", "Недостаточно серебра." }
            } },
            { "MiningOutpost.Orbital.Services.TransportPods:Draw", new Dictionary<string,string> {
                { "Choose destination", "Выбрать место назначения" },
                { "You have {0} chemfuel and {1} silver.", "У вас {0} химтоплива и {1} серебра." },
                { "You must provide the chemfuel needed to relaunch your transport pods. If you don't have enough fuel, the deficit can be automatically purchased using silver. Furthermore, if you currently have more people than pods, you must pay an extra fee to provision new pods for the extra crew.", "Для повторного запуска транспортных капсул нужно химтопливо. Если его не хватает, недостающее можно автоматически купить за серебро. Кроме того, если людей теперь больше, чем капсул, придётся доплатить за новые капсулы для дополнительных пассажиров." }
            } },
            { "MiningOutpost.Orbital.Services.TransportPods:PodTargetLabel", new Dictionary<string,string> {
                { "Cannot land pods here.", "Здесь нельзя посадить капсулы." },
                { "Drop pods here.\n{0} chemfuel, {1} silver", "Сбросить капсулы сюда.\n{0} химтоплива, {1} серебра" },
                { "Land pods here.\n{0} chemfuel, {1} silver", "Посадить капсулы здесь.\n{0} химтоплива, {1} серебра" }
            } },
            { "MiningOutpost.Orbital.Services.TransportersArrivalAction_PodRelaunchDescent:Arrived", new Dictionary<string,string> {
                { "The pods slammed down.", "Капсулы с грохотом приземлились." }
            } },
            { "MiningOutpost.Orbital.Services.TransportersArrivalAction_StrandedDescent:Arrived", new Dictionary<string,string> {
                { "The cargo pod slammed down.", "Грузовая капсула с грохотом приземлилась." }
            } },
            { "MiningOutpost.Orbital.Services.TurbineSwarmEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Turbine", "Турбина" },
                { "Turbine vibrations trigger a stampede! Dozens of red eyes pour from the intake vents.", "Вибрация турбины поднимает целую орду! Из заборных вентиляционных отверстий хлынули десятки красных глаз." }
            } },
            { "MiningOutpost.Orbital.Services.TurbineSwarmEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Scratch and bite", "царапается и кусается" },
                { "Turbine Vermin ", "Турбинный вредитель " }
            } },
            { "MiningOutpost.Orbital.Services.TurbineSwarmEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "The horde swarms over the catwalk, biting through seals.", "Орда захлёстывает мосток, прогрызая уплотнения." }
            } },
            { "MiningOutpost.Orbital.Services.TurbineSwarmEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Massive rotating fan blades, red eyes swarming the intake conduits.", "Вращаются огромные лопасти вентилятора; в заборных каналах кишат красные глаза." }
            } },
            { "MiningOutpost.Orbital.Services.TurbineSwarmEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Turbine Bay Intake", "Воздухозаборник турбинного отсека" }
            } },
            { "MiningOutpost.Orbital.Services.TurbineSwarmEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "The intake turbine is cleared of vermin.", "Впускная турбина очищена от вредителей." }
            } },
            { "MiningOutpost.Orbital.Services.WagerItem:LabelShort", new Dictionary<string,string> {
                { " (apparel)", " (одежда)" },
                { " (weapon)", " (оружие)" },
                { "asset", "имущество" }
            } },
            { "MiningOutpost.Orbital.Services.WeaponProfile:For", new Dictionary<string,string> {
                { "Bare hands", "Голые руки" }
            } },
            { "MiningOutpost.Orbital.Services.ZeroGScavengersEncounter+<Intro>d__8:MoveNext", new Dictionary<string,string> {
                { "Cargo Core", "Грузовой узел" },
                { "Muzzle flashes light the dark hull! Rival scavengers maneuver across the zero-g framework.", "Вспышки выстрелов озаряют тёмный корпус! Соперники-сборщики утиля маневрируют среди конструкций в невесомости." }
            } },
            { "MiningOutpost.Orbital.Services.ZeroGScavengersEncounter:BuildEnemies", new Dictionary<string,string> {
                { "Vacuum Scavenger ", "Космический сборщик утиля " },
                { "fires flechette carbine", "стреляет из игольчатого карабина" }
            } },
            { "MiningOutpost.Orbital.Services.ZeroGScavengersEncounter:get_LoseHeadline", new Dictionary<string,string> {
                { "Suit puncture alarms howl in the void. Crew retreats to the lock.", "В пустоте воют сигналы о пробоинах скафандров. Экипаж отступает к шлюзу." }
            } },
            { "MiningOutpost.Orbital.Services.ZeroGScavengersEncounter:get_SceneTag", new Dictionary<string,string> {
                { "Silent floating scrap, tether lines, cold-gas thruster maneuvering.", "В тишине парит лом, натянуты страховочные тросы; манёвры выполняются двигателями на сжатом газе." }
            } },
            { "MiningOutpost.Orbital.Services.ZeroGScavengersEncounter:get_SceneTitle", new Dictionary<string,string> {
                { "Depressurized Cargo Core", "Разгерметизированный грузовой узел" }
            } },
            { "MiningOutpost.Orbital.Services.ZeroGScavengersEncounter:get_WinHeadline", new Dictionary<string,string> {
                { "Rival scavengers drift lifelessly into the hull recesses.", "Безжизненные тела соперников-сборщиков утиля дрейфуют в ниши корпуса." }
            } },
            { "MiningOutpost.Orbital.TransportersArrivalAction_DockAtOrbital:Arrived", new Dictionary<string,string> {
                { "Enter", "Войти" },
                { "You are dropping through the upper atmosphere at terminal velocity when the orbital platform's tractor beam finds you. Your transport pod cluster is swallowed and dragged down into a roaring bay thick with the stench of burning chemfuel and the sparkle of cutting torches. Automatic magnets slam your pods into a tight landing zone. To relaunch your pods using chemfuel or silver, you must look for opportunities.", "Вы несётесь сквозь верхние слои атмосферы на предельной скорости, когда вас находит тяговый луч орбитальной платформы. Он захватывает группу транспортных капсул и втягивает её в грохочущий отсек, полный вони горящего химтоплива и искр газовых резаков. Автоматические магниты с размаху вгоняют капсулы на тесную посадочную площадку. Чтобы снова запустить их за химтопливо или серебро, откройте «Поиск возможностей»." }
            } },
            { "MiningOutpost.Parley.AtonementGenerator:GenerateDemand", new Dictionary<string,string> {
                { "That kind of disrespect won't be tolerated", "Такое неуважение терпеть не станут" },
                { "They want you to compensate them for this insult", "Они требуют возмещения за это оскорбление" },
                { "Your behaviour has infuriated the outpost", "Ваше поведение разъярило жителей аванпоста" },
                { "Your offer was found offensive", "Ваше предложение сочли оскорбительным" },
                { "Your threat was poorly received", "Ваша угроза вызвала недовольство" }
            } },
            { "MiningOutpost.Parley.BlackguardHandler:ApplyBlackguardPenalty", new Dictionary<string,string> {
                { "Your reputation as a blackguard has spread to the other factions.", "Другие фракции узнали о вашем вероломстве." }
            } },
            { "MiningOutpost.Parley.BlackguardHandler:ShowWarningAndAttack", new Dictionary<string,string> {
                { "Blackguard warning", "Предупреждение о вероломстве" },
                { "You traded with this outpost less than {0} days ago. Attacking now will mark you as a blackguard.\n\nYour reputation will be worsened with all neutral and allied factions.\n\nAre you sure you want to proceed?", "Вы торговали с этим аванпостом менее {0} дн. назад. Если нападёте сейчас, за вами закрепится слава вероломной колонии.\n\nВаши отношения со всеми нейтральными и союзными фракциями ухудшатся.\n\nВы уверены, что хотите продолжить?" }
            } },
            { "MiningOutpost.Parley.CaravanArrivalAction_ParleyWithOutpost:Arrived", new Dictionary<string,string> {
                { "The worksite is currently under siege.", "Сейчас рабочий участок находится в осаде." },
                { "This outpost cannot be parleyed with.", "С этим аванпостом нельзя вести переговоры." },
                { "This outpost is not ready to parley for {0} more day(s).", "Аванпост снова будет готов к переговорам через {0} дн." }
            } },
            { "MiningOutpost.Parley.CaravanArrivalAction_ParleyWithOutpost:get_Label", new Dictionary<string,string> {
                { "Parley with outpost", "Переговоры с аванпостом" }
            } },
            { "MiningOutpost.Parley.CaravanArrivalAction_ParleyWithOutpost:get_ReportString", new Dictionary<string,string> {
                { "Parleying with outpost.", "Переговоры с аванпостом." }
            } },
            { "MiningOutpost.Parley.CaravanArrivalAction_TradeWithOutpost:Arrived", new Dictionary<string,string> {
                { "No caravan member has enough social skill to trade.", "Ни у кого в караване навык общения не позволяет торговать." },
                { "The worksite is currently under siege.", "Сейчас рабочий участок находится в осаде." },
                { "This outpost has nothing to trade right now.", "Сейчас этому аванпосту нечего предложить для торговли." },
                { "This outpost is not ready to trade again for {0} more day(s).", "Аванпост снова будет готов торговать через {0} дн." }
            } },
            { "MiningOutpost.Parley.CaravanArrivalAction_TradeWithOutpost:get_Label", new Dictionary<string,string> {
                { "Trade with outpost", "Торговать с аванпостом" }
            } },
            { "MiningOutpost.Parley.CaravanArrivalAction_TradeWithOutpost:get_ReportString", new Dictionary<string,string> {
                { "Trading with outpost.", "Торговля с аванпостом." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley+<>c__DisplayClass81_0:<TriggerSortieAmbush>b__2", new Dictionary<string,string> {
                { " have intercepted your caravan!", "." },
                { "Mercenaries from ", "Ваш караван перехватили наёмники! Их фракция: " },
                { "Mercenaries have intercepted your caravan!", "Наёмники перехватили ваш караван!" },
                { "Sortie", "Вылазка" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:ApplyParleyGoodwillToSiteFaction", new Dictionary<string,string> {
                { "intimidated us", "запугивание" },
                { "poached our workers", "переманивание наших работников" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:ApplyReward", new Dictionary<string,string> {
                { "The outpost workers yield to your demands.", "Работники аванпоста уступают вашим требованиям." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:ApplyRoyalTitheReward", new Dictionary<string,string> {
                { "{0} has been granted {1} royal favor from the tithe.", "{0} получает {1} ед. чести за выплату десятины." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:ApplyWorkerReward", new Dictionary<string,string> {
                { "the outpost", "аванпост" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:BuildRewardDescription", new Dictionary<string,string> {
                { "A random faction's goodwill toward you will increase by {0}.", "Отношение случайной фракции к вам улучшится на {0}." },
                { "Someone from the outpost will be handed over.", "Вам передадут одного из жителей аванпоста." },
                { "They will offer whatever they can spare from their stockpile.", "Они предложат то, что смогут выделить из своих запасов." },
                { "They'll hand over someone less experienced.", "Вам передадут менее опытного работника." },
                { "your colonist", "Ваш поселенец" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:BuildSpecificOptions", new Dictionary<string,string> {
                { "Proceed", "Продолжить" },
                { "Random", "Случайный вариант" },
                { "Random faction", "Случайная фракция" },
                { "[ally]", "[союзники]" },
                { "[hostile]", "[враги]" },
                { "[neutral]", "[нейтралы]" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:CacheHeaderInfo", new Dictionary<string,string> {
                { " Outpost", " — аванпост" },
                { "Neutral", "Нейтральный" },
                { "Outpost", "Аванпост" },
                { "Unknown Faction", "Неизвестная фракция" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:DrawCommsHeader", new Dictionary<string,string> {
                { ", Envoy", ", переговорщик" },
                { "No envoy", "Нет переговорщика" },
                { "Social skill: {0}", "Общение: {0}" },
                { "Swap active negotiator", "Сменить переговорщика" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:DrawDemandRow", new Dictionary<string,string> {
                { "Counter-offer was rejected.", "Встречное предложение отклонено." },
                { "You rejected their counter-offer for this demand.", "Вы отклонили их встречное предложение по этому требованию." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:DrawDemandSelection", new Dictionary<string,string> {
                { "Cancel", "Отмена" },
                { "Cannot attempt to parley with this outpost again for {0} days after an attempt.", "После попытки переговоров повторить её с этим аванпостом можно лишь через {0} дн." },
                { "Demand a payment of silver from the outpost.", "Потребовать от аванпоста выплату серебром." },
                { "Demand a worker from the outpost.", "Потребовать, чтобы аванпост передал вам работника." },
                { "Demand apology", "Потребовать извинения" },
                { "Demand endorsement", "Потребовать поручительство" },
                { "Demand materials from the outpost's stockpile.", "Потребовать материалы со склада аванпоста." },
                { "Demand royal tithe", "Потребовать имперскую десятину" },
                { "Demand silver", "Потребовать серебро" },
                { "Demand the outpost endorse you to another faction, improving goodwill.", "Потребовать, чтобы аванпост поручился за вас перед другой фракцией и улучшил её отношение к вам." },
                { "Demand the worksite pay their dues to the Empire to earn royal favor.", "Потребовать, чтобы рабочий участок заплатил дань Империи, за которую вы получите честь." },
                { "Demand tribute", "Потребовать дань" },
                { "Demand worker", "Потребовать работника" },
                { "Humiliate the outpost and boost your colonists' mood.", "Унизить аванпост и поднять настроение вашим поселенцам." },
                { "If they refuse, the outpost may become hostile.", "Если вам откажут, аванпост может стать враждебным." },
                { "Parleying successfully will prevent raids at this outpost for {0} days.", "После успешных переговоров этот аванпост будет защищён от рейдов на {0} дн." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:DrawOutcome", new Dictionary<string,string> {
                { "Accept", "Принять" },
                { "Accept terms", "Принять условия" },
                { "Close", "Закрыть" },
                { "Counter-Offer", "Встречное предложение" },
                { "Demand Dismissed", "Требование проигнорировано" },
                { "Demand Refused", "В требовании отказано" },
                { "Demand Rejected!", "Требование отвергнуто!" },
                { "Demand accepted", "Требование принято" },
                { "Refuse and leave", "Отказаться и уйти" },
                { "Reject", "Отклонить" },
                { "Something else", "Что-нибудь другое" },
                { "The outpost has placed a bounty on your colony. Mercenaries will come to collect.", "Аванпост назначил награду за расправу над вашей колонией. За ней придут наёмники." },
                { "They can't provide that right now.", "Сейчас они не могут этого предоставить." },
                { "Unavailable", "Недоступно" },
                { "You can make another demand.", "Вы можете выдвинуть другое требование." },
                { "You may be able to demand something else.", "Возможно, вы сможете потребовать что-нибудь другое." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:DrawRoyalTargetSelection", new Dictionary<string,string> {
                { "  (no title)", "  (без титула)" },
                { "Back", "Назад" },
                { "Cancel", "Отмена" },
                { "Choose which colonist will receive the royal favor:", "Выберите поселенца, которому будет дарована честь:" },
                { "None of your caravan pawns are royalty. You need at least one noble colonist to demand a royal tithe.", "Ни у одного поселенца в караване нет знатного титула. Для требования имперской десятины нужен хотя бы один титулованный поселенец." }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:DrawSpecificsPanel", new Dictionary<string,string> {
                { "\nFinal: {0}", "\nИтог: {0}" },
                { " (Cannot afford)", " (не хватает средств)" },
                { " (unavailable)", " (недоступно)" },
                { "Back", "Назад" },
                { "Demand: ", "Требование: " },
                { "No offers available.", "Нет доступных предложений." },
                { "Press demands", "Настаивать на требованиях" },
                { "Remove offer", "Убрать предложение" },
                { "Success chance: {0}%", "Вероятность успеха: {0}%" },
                { "Sweeten Deal", "Предложить уступку" },
                { "Sweeten deal", "Предложить уступку" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:GetAlliedDeclineText", new Dictionary<string,string> {
                { "Despite your help defending the outpost, they aren't willing to ", "Несмотря на вашу помощь в обороне аванпоста, его жители не готовы " },
                { "accept your demands", "принимать ваши требования" },
                { "bow down to you", "преклоняться перед вами" },
                { "give up their workers", "отдавать своих работников" },
                { "give you tribute", "платить вам дань" },
                { "pay imperial tithe on your behalf", "платить имперскую десятину от вашего имени" },
                { "pay you any silver", "платить вам серебром" },
                { "vouch for you", "поручиться за вас" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:GetCounterOfferLabel", new Dictionary<string,string> {
                { "+{0} goodwill instead of +{1}", "+{0} к отношениям вместо +{1}" },
                { "+{0} mood for {1} days", "+{0} к настроению на {1} дн." },
                { "a less experienced worker", "менее опытный работник" },
                { "less than requested", "меньше запрошенного" },
                { "materials", "материалы" },
                { "{0} silver", "{0} серебра" },
                { "{0}-{1} royal favor", "{0}–{1} ед. чести" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:GetDemandLabel", new Dictionary<string,string> {
                { "A Worker", "Работник" },
                { "An Apology", "Извинения" },
                { "An Endorsement", "Поручительство" },
                { "Royal Tithe", "Имперская десятина" },
                { "Silver", "Серебро" },
                { "Tribute (Materials)", "Дань материалами" },
                { "Unknown", "Неизвестно" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:GetUnavailableMessage", new Dictionary<string,string> {
                { "Your negotiator", "Ваш переговорщик" },
                { "that material", "этот материал" },
                { "that type of worker", "специалист этой профессии" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:ResolveDemand", new Dictionary<string,string> {
                { "The outpost has launched a sortie to crush you, and placed a bounty on your colony!", "Защитники аванпоста вышли на вылазку, чтобы разгромить вас, и назначили награду за расправу над вашей колонией!" },
                { "The outpost has launched a sortie, leaving their outpost to drive you away!", "Защитники аванпоста вышли на вылазку, чтобы прогнать вас!" },
                { "The outpost has placed a bounty on your colony. Mercenaries will come to collect.", "Аванпост назначил награду за расправу над вашей колонией. За ней придут наёмники." },
                { "The outpost is spreading word of your attempted treachery to the other factions.", "Аванпост рассказывает другим фракциям о вашей попытке предательства." },
                { "The outpost negotiator", "Переговорщик аванпоста" },
                { "Your negotiator", "Ваш переговорщик" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:TriggerBountyRaid", new Dictionary<string,string> {
                { "the outpost", "аванпост" }
            } },
            { "MiningOutpost.Parley.Dialog_Parley:TriggerSortieAmbush", new Dictionary<string,string> {
                { "Flee", "Бежать" },
                { "Sortie Interception", "Перехват вылазкой" },
                { "The outpost has launched a sortie to crush you, and placed a bounty on your colony!", "Защитники аванпоста вышли на вылазку, чтобы разгромить вас, и назначили награду за расправу над вашей колонией!" },
                { "The outpost has launched a sortie, leaving their outpost to drive you away!", "Защитники аванпоста вышли на вылазку, чтобы прогнать вас!" }
            } },
            { "MiningOutpost.Parley.Hediff_ParleyBetrayal:TriggerBetrayal", new Dictionary<string,string> {
                { " was secretly loyal to the outpost and has betrayed your colony!", " тайно служит аванпосту и предаёт вашу колонию!" },
                { " was secretly loyal to the outpost and is attempting to sneak away from the colony!", " тайно служит аванпосту и пытается незаметно покинуть колонию!" },
                { "Betrayal", "Предательство" }
            } },
            { "MiningOutpost.Parley.OutpostFloatMenuHelper+<AnnotateOptions>d__0:MoveNext", new Dictionary<string,string> {
                { " (HIGH ALERT)", " (БОЕВАЯ ГОТОВНОСТЬ)" },
                { " (locked out  {0} day(s) remaining)", " (недоступно ещё {0} дн.)" },
                { "Land outside outpost", "Приземлиться за пределами аванпоста" }
            } },
            { "MiningOutpost.Parley.OutpostSiteTracker:FireRetaliationRaid", new Dictionary<string,string> {
                { " has arrived to avenge your demands!", "." },
                { "A retaliatory force from ", "Прибыл карательный отряд — ответ на ваши требования! Отправивший его аванпост: " }
            } },
            { "MiningOutpost.Parley.OutpostSiteTracker:ScheduleRescueRaid", new Dictionary<string,string> {
                { " have arrived to rescue the prisoner you took during parley!", "." },
                { "Mercenaries from ", "Прибыли наёмники, чтобы освободить пленника, захваченного вами во время переговоров! Отправивший их аванпост: " }
            } },
            { "MiningOutpost.Parley.OutpostTradeGoods:GetRarityLabel", new Dictionary<string,string> {
                { "abundant", "в избытке" },
                { "common", "широко доступно" },
                { "rare", "редко встречается" },
                { "scarce", "в дефиците" },
                { "unknown", "неизвестно" }
            } },
            { "MiningOutpost.Parley.OutpostVirtualTrader:get_TraderName", new Dictionary<string,string> {
                { "Outpost", "Аванпост" }
            } },
            { "MiningOutpost.Parley.ParleyBargains:GenerateAvailableOffers", new Dictionary<string,string> {
                { "Ask nicely", "Вежливо попросить" },
                { "Offer a bribe ({0} silver)", "Предложить взятку ({0} серебра)" },
                { "Offer a hefty bribe ({0} silver)", "Предложить крупную взятку ({0} серебра)" },
                { "Spend {0} honor", "Потратить {0} ед. чести" },
                { "Trade ", "Предложить в обмен: " }
            } },
            { "MiningOutpost.Parley.ParleySortieUtility:TriggerEncounter", new Dictionary<string,string> {
                { " have intercepted your caravan!", "." },
                { "Fighters from ", "Ваш караван перехватили бойцы! Их фракция: " }
            } },
            { "MiningOutpost.Parley.ParleySuccessCalculator:CalculateWithSpecifics", new Dictionary<string,string> {
                { " and ", " и " },
                { "Aerial vehicle's descent alerted the outpost.", "Посадка воздушного судна насторожила аванпост." },
                { "Arrived at nighttime.", "Прибытие ночью." },
                { "Caravan has a centipede.", "В караване есть сентипеда." },
                { "Caravan has a diabolus.", "В караване есть дьявол." },
                { "Caravan has a doomsday rocket launcher.", "У каравана есть ракетница «Судный день»." },
                { "Caravan has a lancer.", "В караване есть улан." },
                { "Caravan has a mechanoid army.", "У каравана есть армия механоидов." },
                { "Caravan has a medic.", "В караване есть медик." },
                { "Caravan has a militor.", "В караване есть боевой дрон." },
                { "Caravan has a scyther.", "В караване есть жнец." },
                { "Caravan has a triple rocket launcher.", "У каравана есть тройная ракетница." },
                { "Caravan has a {0}.", "В караване есть: {0}." },
                { "Caravan has an orbital weapon.", "У каравана есть орбитальное оружие." },
                { "Caravan has shield belts.", "У каравана есть пояса-щиты." },
                { "Caravan has slaves in tow.", "Караван ведёт с собой рабов." },
                { "Caravan has some low quality weapons.", "В караване есть оружие низкого качества." },
                { "Caravan is carrying a mortar.", "Караван перевозит миномёт." },
                { "Caravan is wearing prestige armor.", "Участники каравана носят роскошную броню." },
                { "Caravan is well equipped.", "Караван хорошо экипирован." },
                { "Caravan morale is low.", "Низкий боевой дух каравана." },
                { "Colony", "Колония" },
                { "Combined combat strength of the caravan.", "Совокупная боевая сила каравана." },
                { "In the wake of destruction wrought by {0}, the planet is in a period of total, societal collapse.", "Разрушения, причинённые колонией «{0}», привели планету к полному распаду общества." },
                { "Injured colonists.", "Раненые поселенцы." },
                { "Is a moderate caravan.", "Караван среднего размера." },
                { "Is a small caravan.", "Небольшой караван." },
                { "Large caravan is without a capable commander.", "В большом караване нет способного командира." },
                { "Leader", "Лидер" },
                { "Lugging around incapacitated colonists.", "Караван перевозит недееспособных поселенцев." },
                { "Malnourished colonists.", "Недоедающие поселенцы." },
                { "Random demand is flexible.", "Случайное требование допускает выбор." },
                { "Shuttle's descent alerted the outpost.", "Спуск челнока насторожил аванпост." },
                { "Sick colonists.", "Больные поселенцы." },
                { "The outpost", "Аванпост" },
                { "They are a mere homestead.", "Это всего лишь усадьба." },
                { "Transport pod landing made the caravan easy to track.", "После посадки транспортных капсул караван легко выследить." },
                { "archotech implant", "архотек-имплант" },
                { "bionic implant", "бионический имплант" },
                { "{0} are a menace to the neighbourhood.", "Колония «{0}» — угроза для всей округи." },
                { "{0} are already wealthy and they know it.", "Колония «{0}» и без того богата, и на аванпосте это знают." },
                { "{0} are an extremely dangerous, organized group of brutes.", "Колония «{0}» — крайне опасная, организованная группа головорезов." },
                { "{0} are on a deadly war path of mass destruction.", "Колония «{0}» ведёт смертоносную войну и сеет массовые разрушения." },
                { "{0} base reluctance.", "{0}: исходное нежелание уступать." },
                { "{0} believe raiding is virtuous.", "{0}: идеология одобряет набеги." },
                { "{0} came to our aid.", "{0}: помощь нам в обороне." },
                { "{0} can intimidate even the most powerful people", "{0} умеет запугать даже самых влиятельных людей." },
                { "{0} can spew fire.", "{0}: может извергать огонь." },
                { "{0} can spray acid.", "{0}: может распылять кислоту." },
                { "{0} has a {1}.", "{0}: установлен имплант «{1}»." },
                { "{0} has an annoying voice.", "{0}: противный голос." },
                { "{0} has an imposing frame.", "{0}: внушительное телосложение." },
                { "{0} has brain damage.", "{0}: повреждение мозга." },
                { "{0} has consumed human flesh.", "{0}: употребление человеческого мяса." },
                { "{0} has creepy breathing.", "{0}: жуткое дыхание." },
                { "{0} has killed one hundred or more.", "{0}: не менее ста убитых." },
                { "{0} has killed ten or more.", "{0}: не менее десяти убитых." },
                { "{0} has ritual scars.", "{0}: ритуальные шрамы." },
                { "{0} have become a nuisance around worksites", "Колония «{0}» уже стала постоянной проблемой для рабочих участков." },
                { "{0} have cut out human organs.", "{0}: извлечение человеческих органов." },
                { "{0} have knocked over their first worksite.", "Колония «{0}» разграбила свой первый рабочий участок." },
                { "{0} have many allies.", "У колонии «{0}» много союзников." },
                { "{0} have never raided a worksite.", "Колония «{0}» ещё ни разу не нападала на рабочий участок." },
                { "{0} is a blusterer, known for failing to press their demands.", "{0}: пустые угрозы и известная неспособность настоять на своих требованиях." },
                { "{0} is a brawler.", "{0}: любовь к рукопашному бою." },
                { "{0} is a cannibal.", "{0}: каннибализм." },
                { "{0} is a capable enforcer", "{0}: умеет заставить подчиниться" },
                { "{0} is a hard talker of professional caliber", "{0}: ведёт жёсткие переговоры на профессиональном уровне" },
                { "{0} is a hussar.", "{0}: ксенотип «Гусар»." },
                { "{0} is a kidnapper.", "{0}: похищение людей." },
                { "{0} is a level {1} psycaster.", "{0}: пси-способности {1}-го уровня." },
                { "{0} is a masochistic freak.", "{0}: извращённая тяга к боли." },
                { "{0} is a master at extracting concessions", "{0}: мастерски добивается уступок" },
                { "{0} is a master of combat.", "{0}: мастерство в бою." },
                { "{0} is a psychopath.", "{0}: черта «Психопат»." },
                { "{0} is a sanguophage.", "{0}: ксенотип «Сангвофаг»." },
                { "{0} is a social liability", "{0}: мешает переговорам" },
                { "{0} is a wimp.", "{0}: неженка." },
                { "{0} is a {1} in the Empire.", "{0}: имперский титул — {1}." },
                { "{0} is alone.", "В караване только {0}." },
                { "{0} is an expert strong-armer", "{0}: умело добивается своего угрозами" },
                { "{0} is beautiful.", "{0}: красивая внешность." },
                { "{0} is bloodthirsty.", "{0}: кровожадность." },
                { "{0} is depressive.", "{0}: депрессивность." },
                { "{0} is drunk.", "{0}: опьянение." },
                { "{0} is genetically aggressive.", "{0}: генетическая агрессивность." },
                { "{0} is genetically delicate.", "{0}: генетически обусловленная хрупкость." },
                { "{0} is genetically hyper-aggressive.", "{0}: генетическая гиперагрессивность." },
                { "{0} is genetically incapable of violence.", "{0}: генетическая неспособность к насилию." },
                { "{0} is high on go-juice.", "{0}: действие Go-сока." },
                { "{0} is high on luciferium.", "{0}: действие люциферия." },
                { "{0} is high on smokeleaf.", "{0}: действие дымолиста." },
                { "{0} is high on wake-up.", "{0}: действие бодрина." },
                { "{0} is high on yayo.", "{0}: действие порошка психина." },
                { "{0} is incapable of violence.", "{0}: неспособность к насилию." },
                { "{0} is injured.", "{0}: ранения." },
                { "{0} is iron-willed.", "{0}: железная воля." },
                { "{0} is just a kid.", "{0}: всего лишь ребёнок." },
                { "{0} is kind.", "{0}: доброта." },
                { "{0} is lazy.", "{0}: лень." },
                { "{0} is missing {1}.", "{0}: отсутствующие части тела — {1}." },
                { "{0} is naked and a cannibal.", "{0}: каннибализм и нагота." },
                { "{0} is naked.", "{0}: нагота." },
                { "{0} is nervous.", "{0}: нервозность." },
                { "{0} is nimble.", "{0}: проворность." },
                { "{0} is old in a profession where people usually die young.", "{0}: преклонный возраст в профессии, где обычно умирают молодыми." },
                { "{0} is one of the most feared and effective coercive negotiators alive", "{0}: входит в число самых грозных и успешных мастеров принуждения" },
                { "{0} is pregnant.", "{0}: беременность." },
                { "{0} is regarded as one of the most dangerous negotiators on this rimworld", "{0}: входит в число самых опасных переговорщиков этого окраинного мира" },
                { "{0} is scarred.", "{0}: шрамы." },
                { "{0} is staggeringly ugly.", "{0}: крайне отталкивающая внешность." },
                { "{0} is steadfast.", "{0}: стойкость." },
                { "{0} is too old.", "{0}: слишком преклонный возраст." },
                { "{0} is tough.", "{0}: крепкое телосложение." },
                { "{0} is ugly.", "{0}: отталкивающая внешность." },
                { "{0} is void-touched.", "{0}: влияние Пустоты." },
                { "{0} is wearing someone else's skin.", "{0}: одежда из чужой кожи." },
                { "{0} is wielding {1}.", "{0}: оружие — {1}." },
                { "{0} is withdrawing from an addiction.", "{0}: ломка из-за зависимости." },
                { "{0}'s combat prowess.", "{0}: боевое мастерство." },
                { "{0}'s ideology forbids violence.", "{0}: идеология запрещает насилие." },
                { "{0}'s unsettles the easily rattled", "{0}: пугает лишь самых робких" },
                { "{0}'s words are occasionally taken seriously", "{0}: к словам иногда прислушиваются" },
                { "{0}'s {1} is injured.", "{0}: ранение — {1}." },
                { "{0}'s {1} is maimed.", "{0}: увечье — {1}." }
            } },
            { "MiningOutpost.Parley.ParleySuccessCalculator:GetSpecificityLabel", new Dictionary<string,string> {
                { "Specificity.", "Определённость запроса." },
                { "faction", "фракция" },
                { "item", "предмет" }
            } },
            { "MiningOutpost.Parley.ParleyVehicleFrameworkPatchApplicator:GetArrivalOptionsPostfix", new Dictionary<string,string> {
                { "Land outside outpost", "Приземлиться за пределами аванпоста" }
            } },
            { "MiningOutpost.Parley.ParleyVehicleFrameworkPatchApplicator:LandToCaravanArrivedPostfix", new Dictionary<string,string> {
                { "The noise of your aerial vehicle's descent revealed your position. The outpost is on high alert, making negotiation less effective.", "Шум спуска вашего воздушного судна выдал ваше местоположение. Аванпост приведён в полную боевую готовность, и переговоры будут менее успешными." }
            } },
            { "MiningOutpost.Parley.Patch_CaravanAttack_BlackguardCheck:Prefix", new Dictionary<string,string> {
                { "This outpost is on high alert. Defenders will be significantly stronger.", "Этот аванпост приведён в полную боевую готовность. Его защитники будут значительно сильнее." },
                { "You cannot attack this outpost for {0} more day(s). A parley agreement is in effect.", "Вы не можете нападать на этот аванпост ещё {0} дн. Действует соглашение, достигнутое на переговорах." }
            } },
            { "MiningOutpost.Parley.Patch_Caravan_GetGizmos:Postfix", new Dictionary<string,string> {
                { "\n\nWARNING: This outpost is on high alert!", "\n\nВНИМАНИЕ: аванпост в полной боевой готовности!" },
                { "Attack", "Напасть" },
                { "Cannot parley for {0} more day(s).", "Переговоры недоступны ещё {0} дн." },
                { "Cannot trade for {0} more day(s).", "Торговля недоступна ещё {0} дн." },
                { "Enter the outpost with hostile intent.", "Войти на аванпост с враждебными намерениями." },
                { "Negotiate demands with the outpost.", "Обсудить требования с аванпостом." },
                { "Open trade with this outpost.", "Начать торговлю с этим аванпостом." },
                { "Parley", "Переговоры" },
                { "Parley agreement prevents attack for {0} more day(s).", "Достигнутое на переговорах соглашение запрещает нападение ещё {0} дн." },
                { "The worksite is currently under siege.", "Сейчас аванпост находится в осаде." },
                { "Trade", "Торговля" }
            } },
            { "MiningOutpost.Parley.Patch_FormCaravan_ShuttleNoiseWarning:Postfix", new Dictionary<string,string> {
                { "The noise of your shuttle's descent revealed your position. The outpost is on high alert, making negotiation less effective.", "Шум спуска вашего челнока выдал ваше местоположение. Аванпост приведён в полную боевую готовность, и переговоры будут менее успешными." },
                { "Your transport pods left a hot trail across the sky. Your colonists are highly trackable and vulnerable. Negotiation will be less effective.", "Ваши транспортные капсулы оставили яркий след в небе. Поселенцев легко выследить, и они уязвимы. Переговоры будут менее успешными." }
            } },
            { "MiningOutpost.Parley.Patch_MapDeiniter_EjectPlayerPawns:Prefix", new Dictionary<string,string> {
                { "Your colonists were recovered from the outpost and formed a caravan.", "Ваши поселенцы выбрались с аванпоста и сформировали караван." }
            } },
            { "MiningOutpost.Parley.Patch_Site_GetFloatMenuOptions:Postfix", new Dictionary<string,string> {
                { "Aid ", "Помочь: " },
                { "Aid (Dev: instantly)", "Помочь (разраб.: мгновенно)" },
                { "Parley (Dev: instantly)", "Переговоры (разраб.: мгновенно)" },
                { "Parley with outpost", "Переговоры с аванпостом" },
                { "Trade (Dev: instantly)", "Торговля (разраб.: мгновенно)" },
                { "Trade with outpost", "Торговать с аванпостом" },
                { "Visit ", "Посетить: " },
                { "Visit (Dev: instantly)", "Посетить (разраб.: мгновенно)" }
            } },
            { "MiningOutpost.Parley.Patch_TransporterArrival_ParleyCheck:Prefix", new Dictionary<string,string> {
                { "This outpost is on high alert. Defenders will be significantly stronger.", "Этот аванпост приведён в полную боевую готовность. Его защитники будут значительно сильнее." },
                { "You cannot attack this outpost for {0} more day(s). A parley agreement is in effect.", "Вы не можете нападать на этот аванпост ещё {0} дн. Действует соглашение, достигнутое на переговорах." }
            } },
            { "MiningOutpost.Parley.PlayerOffer:GetConfirmationText", new Dictionary<string,string> {
                { "Proceed with the parley demands?", "Продолжить переговоры и выдвинуть требования?" },
                { "You are exchanging {0} honor. Continue?", "Вы отдадите {0} ед. чести. Продолжить?" },
                { "You are offering a bribe of {0} silver. Continue?", "Вы предлагаете взятку: {0} серебра. Продолжить?" }
            } },
            { "MiningOutpost.Patch_Caravan_GetGizmos_AidWorksite:Postfix", new Dictionary<string,string> {
                { "Aid", "Помочь" },
                { "Enter this outpost as an ally to defend against an upcoming attack.", "Войти в аванпост как союзник, чтобы помочь отразить грядущее нападение." },
                { "This caravan cannot enter the worksite.", "Этот караван не может войти на рабочий участок." }
            } },
            { "MiningOutpost.Patch_GenExplosion_DoExplosion:Postfix", new Dictionary<string,string> {
                { "The outpost is being bombed! The garrison is on high alert.", "Аванпост бомбят! Гарнизон приведён в полную боевую готовность." }
            } },
            { "MiningOutpost.Patch_Pawn_GetGizmos_ImpressNow+<Append>d__1:MoveNext", new Dictionary<string,string> {
                { "Dev: Impress now", "Разраб.: впечатлить немедленно" },
                { "Instantly satisfy this mercenary's impression threshold and raise the recruit offer.", "Немедленно достичь порога впечатления для этого наёмника, чтобы он предложил присоединиться к колонии." }
            } },
            { "MiningOutpost.Patch_Pawn_GetInspectString_DefenseAllyStatus:DescribeDefenseActivity", new Dictionary<string,string> {
                { "Defending", "Обороняется" },
                { "Falling back to cover", "Отступает в укрытие" },
                { "Following ", "Сопровождает: " },
                { "Guarding", "Охраняет" },
                { "Healing", "Оказывает медицинскую помощь" },
                { "Hunting enemies", "Преследует врагов" },
                { "Hunting for enemies", "Выслеживает врагов" },
                { "Manning the gun", "Обслуживает орудие" },
                { "Relaxing", "Отдыхает" },
                { "Socialising", "Общается" },
                { "Standing by", "Ждёт приказа" },
                { "Watching for targets", "Ищет цели" }
            } },
            { "MiningOutpost.Patch_Pawn_GetInspectString_WorkerStatus:Postfix", new Dictionary<string,string> {
                { "\nMental state: Fleeing", "\nПсихическое состояние: бегство" },
                { "\nMental state: Panicking (cowering)", "\nПсихическое состояние: паника (сжимается от страха)" }
            } },
            { "MiningOutpost.Patch_WorksiteDefense_RelabelTradeTooltip:Postfix", new Dictionary<string,string> {
                { "Defended this outpost", "Защита этого аванпоста" }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter+<CompGetGizmosExtra>d__55:MoveNext", new Dictionary<string,string> {
                { "DEV: Find Site Now", "ОТЛАДКА: найти участок сейчас" },
                { "Overclock", "Разгон" },
                { "Overclock mode: ", "Режим разгона: " },
                { "Overclock: ", "Разгон: " },
                { "Push the emitter beyond safe limits to scan faster.", "Превысить безопасную нагрузку на излучатель, чтобы ускорить сканирование." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:<CompGetGizmosExtra>b__55_0", new Dictionary<string,string> {
                { "Aggressive surveying: 2× speed", "Усиленное сканирование: скорость ×2" },
                { "No holds barred: 3× speed", "Без ограничений: скорость ×3" },
                { "Off", "Выкл." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:<CompGetGizmosExtra>b__55_1", new Dictionary<string,string> {
                { "Pulse emitter is surveying at normal speed.", "Импульсный излучатель сканирует с обычной скоростью." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:<CompGetGizmosExtra>b__55_2", new Dictionary<string,string> {
                { "Pulse emitter is aggressively surveying at 2× speed. Expect unforseen consequences.", "Импульсный излучатель работает в усиленном режиме: скорость ×2. Возможны непредвиденные последствия." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:<CompGetGizmosExtra>b__55_3", new Dictionary<string,string> {
                { "Pulse emitter is surveying indiscriminately at 3× speed. Expect frequent consequences.", "Импульсный излучатель сканирует без ограничений: скорость ×3. Неприятные последствия будут частыми." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:CompInspectStringExtra", new Dictionary<string,string> {
                { "\nBroken down.", "\nПоломка." },
                { "\nExisting work sites nearby are reducing scan effectiveness by {0:F0}%.", "\nРабочие участки поблизости снижают эффективность сканирования на {0:F0}%." },
                { "\nReady to scan.", "\nГотов к сканированию." },
                { "Cooling down: {0:F1}h remaining.\n{1}", "Охлаждение: осталось {0:F1} ч.\n{1}" },
                { "Not connected to power.", "Нет подключения к электросети." },
                { "Scan progress: {0:F2}%{1}", "Прогресс сканирования: {0:F2}%{1}" }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:FireQuest", new Dictionary<string,string> {
                { "Someone", "Кто-то" }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:TryComponentBurnout", new Dictionary<string,string> {
                { "The pulse emitter fried itself.", "Импульсный излучатель сгорел." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:TryDataCorruption", new Dictionary<string,string> {
                { "Files were accidentally deleted and {0}% of scan data was wiped from the pulse emitter.", "Файлы были случайно удалены: импульсный излучатель потерял {0}% данных сканирования." },
                { "The pulse emitter became corrupted. {0}% of the pulse emitter's data could not be recovered.", "Данные импульсного излучателя повреждены. Восстановить {0}% данных не удалось." },
                { "The pulse emitter lost {0}% of scan progress due to a system error.", "Из-за системной ошибки импульсный излучатель потерял {0}% прогресса сканирования." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:TryLockout", new Dictionary<string,string> {
                { "The pulse emitter failed catastrophically and needs to be restored from a backup.", "Импульсный излучатель: аварийный сбой. Восстановление из резервной копии займёт некоторое время." },
                { "The pulse emitter has suffered a critical failure and must be restored from a backup.", "Импульсный излучатель: критический сбой. Идёт восстановление из резервной копии." },
                { "The pulse emitter is no longer operational due to a severe fault and needs to load a previous save.", "Импульсный излучатель временно отключён из-за серьёзного сбоя. Он восстанавливает ранее сохранённые данные." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:TryPowerSurge", new Dictionary<string,string> {
                { "A power surge is causing the pulse emitter to use {0:F0}W", "Из-за скачка нагрузки импульсный излучатель потребляет {0:F0} Вт." }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:TryStartFire", new Dictionary<string,string> {
                { "The pulse emitter started a fire!", "Импульсный излучатель вызвал пожар!" }
            } },
            { "MiningOutpost.PulseEmitter.CompPulseEmitter:TryVentHeat", new Dictionary<string,string> {
                { "The pulse emitter is venting extreme heat!", "Импульсный излучатель выделяет огромное количество тепла!" }
            } },
            { "MiningOutpost.QuestNode_GetMapFallback:RunInt", new Dictionary<string,string> {
                { "map_definite->your current position", "map_definite->ваша текущая позиция" },
                { "map_indefinite->a nearby area", "map_indefinite->ближайшая местность" },
                { "map_label->your position", "map_label->ваша позиция" }
            } },
            { "MiningOutpost.QuestNode_GetMiningFaction:InjectFallbackDescriptionRules", new Dictionary<string,string> {
                { "questDescription(priority=-1)->A work site has been detected in the region. Locate it on the world map and raid it for resources.", "questDescription(priority=-1)->В регионе обнаружен рабочий участок. Найдите его на карте мира и совершите набег, чтобы добыть ресурсы." },
                { "questName(priority=-1)->Outpost Located", "questName(priority=-1)->Найден аванпост" },
                { "questName(priority=-1)->Work Site Detected", "questName(priority=-1)->Обнаружен рабочий участок" }
            } },
            { "MiningOutpost.QuestNode_Root_WorksiteDefense:RunInt", new Dictionary<string,string> {
                { "Expected around {0}", "Ожидаемое время: {0}" },
                { "Raiders arrive in {0}", "Налётчики прибудут через {0}" }
            } },
            { "MiningOutpost.Vehicles.HotwireFloatMenuHelper+<GetHotwireOptionsForColonist>d__2:MoveNext", new Dictionary<string,string> {
                { "Hotwire vehicle", "Завести машину без ключа" },
                { "Hotwire vehicle (cannot reach)", "Завести машину без ключа (нельзя добраться)" },
                { "Hotwire vehicle (reserved by ", "Завести машину без ключа (занято: " },
                { "someone", "кто-то" }
            } },
            { "MiningOutpost.Vehicles.JobDriver_HotwireVehicle:<MakeNewToils>b__2_3", new Dictionary<string,string> {
                { "The vehicle was hotwired!", "Машина заведена без ключа!" }
            } },
            { "MiningOutpost.Vehicles.VehiclePatchApplicator+<>c__DisplayClass2_0:<GetGizmosPostfix>b__0", new Dictionary<string,string> {
                { " (cannot reach)", " (нельзя добраться)" },
                { " (reserved by ", " (занято: " },
                { "No colonists available", "Нет доступных поселенцев" },
                { "someone", "кто-то" }
            } },
            { "MiningOutpost.Vehicles.VehiclePatchApplicator+<>c__DisplayClass2_0:<GetGizmosPostfix>b__2", new Dictionary<string,string> {
                { " hotwired.", " — блокировка снята." },
                { "DevMode: ", "ОТЛАДКА: " }
            } },
            { "MiningOutpost.Vehicles.VehiclePatchApplicator+<GetGizmosPostfix>d__2:MoveNext", new Dictionary<string,string> {
                { "DEV: Hotwire now", "ОТЛАДКА: мгновенный взлом" },
                { "Hotwire vehicle", "Завести машину без ключа" },
                { "Instantly hotwire this vehicle.", "Мгновенно завести эту машину без ключа." },
                { "Order a colonist to hotwire this vehicle.", "Приказать поселенцу завести эту машину без ключа." }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:AttackerNoun", new Dictionary<string,string> {
                { "raiders", "налётчики" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:BeginOnSiteDefense", new Dictionary<string,string> {
                { "Caravan arrived at {0}. {1} is arriving in {2} hours.", "Караван прибыл на место: {0}. Нападение группы «{1}» ожидается через {2} ч." }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:DebugRandomDefenseMission", new Dictionary<string,string> {
                { "No eligible worksite on the world map to defend.", "На карте мира нет подходящего рабочего участка для задания на оборону." }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:IssueOffer", new Dictionary<string,string> {
                { " is under attack by ", " подвергается нападению. Противник: " },
                { "the worksite", "рабочий участок" },
                { "worksite", "рабочий участок" },
                { "your colony", "ваша колония" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:NotifyFriendlyFire", new Dictionary<string,string> {
                { "Friendly fire will not be tolerated!", "Огонь по своим недопустим!" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:NotifyHotwire", new Dictionary<string,string> {
                { "Hotwiring the worksite's vehicles will not be tolerated!", "Угонять транспорт рабочего участка недопустимо!" },
                { "The worksite now considers you their enemy!", "Жители рабочего участка теперь считают вас врагами!" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:NotifyTheft", new Dictionary<string,string> {
                { "Looting the worksite will not be tolerated!", "Грабить рабочий участок недопустимо!" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:OnAbandon", new Dictionary<string,string> {
                { "You abandoned the battle!", "Вы бросили союзников в бою!" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:OnDefenseOverrun", new Dictionary<string,string> {
                { "Outpost overrun", "Аванпост захвачен" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:OnDefenseSuccess", new Dictionary<string,string> {
                { "Successful defense", "Успешная оборона" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:PendingRaidExplanation", new Dictionary<string,string> {
                { "{0}: the next wave of the {1} arrives in {2} hours.", "{0}: следующая волна группы «{1}» прибудет через {2} ч." },
                { "{0}: {1} will attack in {2} hours.", "{0}: нападение группы «{1}» ожидается через {2} ч." }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:ProcessActive", new Dictionary<string,string> {
                { " is arriving at ", "» прибывает на место: " },
                { ". Hold the line with the defenders!", ". Помогите защитникам удержать позиции!" },
                { "Another wave of the ", "Подкрепление группы «" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:Resolve", new Dictionary<string,string> {
                { "Outpost overrun", "Аванпост захвачен" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:SettleAttackerRelation", new Dictionary<string,string> {
                { "{0} is grateful you let the worksite fall. (+{1} goodwill, abandoned worksite defence)", "Фракция «{0}» благодарна вам за то, что вы позволили захватить рабочий участок. (Улучшение отношений: +{1}, отказ от защиты участка)" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:SiteDesc", new Dictionary<string,string> {
                { "worksite", "рабочий участок" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:TryStartAid", new Dictionary<string,string> {
                { "Cannot accept defense mission.", "Невозможно принять задание на оборону." },
                { "This pawn cannot accept the defense mission.", "Этот персонаж не может принять задание на оборону." }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:WName", new Dictionary<string,string> {
                { "the worksite", "рабочий участок" }
            } },
            { "MiningOutpost.WorkSiteRevengeTracker:DebugForceStartRevenge", new Dictionary<string,string> {
                { "DEBUG: Revenge sequence started vs ", "ОТЛАДКА: начата карательная кампания против " }
            } },
            { "MiningOutpost.WorkSiteRevengeTracker:FireRevengeRaid", new Dictionary<string,string> {
                { "Counter-attack", "Контратака" }
            } },
            { "MiningOutpost.WorkerNeutralizationUtility:NeutralizeAllWorkers", new Dictionary<string,string> {
                { "With their guards defeated, the workers flee the area.", "Охрана побеждена, и работники бегут с участка." }
            } },
            { "MiningOutpost.WorkerPanicUtility:AlertGarrison", new Dictionary<string,string> {
                { "The injured worker has alerted the garrison! The mercenaries are attacking immediately.", "Раненый работник поднял тревогу! Наёмники немедленно переходят в наступление." },
                { "The injured worker has alerted the garrison! They are defending the outpost immediately.", "Раненый работник поднял тревогу! Гарнизон немедленно приступает к обороне аванпоста." },
                { "The mercenaries are abandoning their posts to hunt you down!", "Наёмники бросают посты и отправляются в погоню за вами!" },
                { "The mercenaries are defending the outpost immediately.", "Наёмники немедленно приступают к обороне аванпоста." },
                { "The mercenaries are launching a coordinated assault on your position!", "Наёмники начинают скоординированную атаку на вашу позицию!" },
                { "The mercenaries are taking up defensive positions.", "Наёмники занимают оборонительные позиции." }
            } },
            { "MiningOutpost.WorkerPanicUtility:HandleWorkerAttacked", new Dictionary<string,string> {
                { "You have attacked a worker! They are calling for help...", "Вы атаковали работника аванпоста! Идёт вызов подкрепления…" }
            } },
            { "MiningOutpost.WorkerRebellionUtility:TryTriggerRebellion", new Dictionary<string,string> {
                { "Slave uprising: The enslaved workers are attacking their pirate captors!", "Восстание рабов: порабощённые работники атакуют удерживающих их пиратов!" },
                { "Worker uprising: The oppressed workers are attacking the guards!", "Восстание рабочих: угнетённые работники атакуют охрану!" }
            } },
            { "MiningOutpost.WorksiteEscortTracker:GetEscortTarget", new Dictionary<string,string> {
                { " is no longer following ", " больше не сопровождает: " }
            } },
            { "MiningOutpost.WorksiteEscortTracker:NotifyFollowInterrupted", new Dictionary<string,string> {
                { " has stopped following ", " больше не сопровождает: " }
            } },
            { "MiningOutpost.WorksiteEscortTracker:NotifyFollowing", new Dictionary<string,string> {
                { " again.", "." },
                { " is following ", " вновь сопровождает: " }
            } },
            { "MiningOutpost.WorksiteGarrisonResolveUtility:FleeingMessage", new Dictionary<string,string> {
                { " are fleeing.", "» бегут с поля боя." },
                { "The mercenaries are fleeing.", "Наёмники бегут с поля боя." },
                { "The mercenaries from ", "Наёмники фракции «" }
            } },
            { "MiningOutpost.WorksiteGarrisonResolveUtility:StandDown", new Dictionary<string,string> {
                { "The mercenaries at the outpost successfully repelled your raid.", "Наёмники аванпоста успешно отразили ваш набег." }
            } },
            { "MiningOutpost.WorksiteImpressionTracker:MakeOffer", new Dictionary<string,string> {
                { " wants to join your colony.", " хочет присоединиться к вашей колонии." },
                { "'s leadership, ", ". " },
                { "Because of ", "Под впечатлением от лидерских качеств: " },
                { "your colony", "ваша колония" }
            } },
            { "MiningOutpost.WorksiteRecruitUtility:RecruitImpressed", new Dictionary<string,string> {
                { " has joined your colony.", " присоединяется к вашей колонии." }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "MiningOutpost.Alert_WorksiteDefenseIncoming:.ctor", new string[] {  } },
            { "MiningOutpost.Orbital.Dialog_GravshipPlatformWarning:.ctor", new string[] { "System.Action" } },
            { "MiningOutpost.Orbital.Services.BlackjackSim:.ctor", new string[] { "System.Int32", "Verse.Pawn" } },
            { "MiningOutpost.Orbital.Services.FirefightSim:.ctor", new string[] { "MiningOutpost.Orbital.Services.FirefightEncounter", "RimWorld.Planet.Caravan", "MiningOutpost.Orbital.Services.OpportunityRunState" } },
            { "MiningOutpost.Parley.ParleySortieUtility:TriggerEncounter", new string[] { "RimWorld.Planet.Caravan", "RimWorld.Planet.Site", "System.Single", "System.String", "System.String", "System.Boolean" } },
            { "MiningOutpost.Parley.ParleySuccessCalculator:CalculateWithSpecifics", new string[] { "RimWorld.Planet.Caravan", "RimWorld.Planet.Site", "MiningOutpost.Parley.OutpostSiteData", "MiningOutpost.Parley.DemandSpecifics", "Verse.Pawn", "MiningOutpost.Parley.PlayerOffer" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3687071198.literals");
            foreach (var target in Methods.Keys.ToArray())
            {
                if (!IsApplicable(target)) { Methods.Remove(target); continue; }
                harmony.Patch(ResolveTarget(target),
                    transpiler: new HarmonyMethod(typeof(LiteralTranslations), nameof(Transpile)));
            }
        }
        public static bool IsApplicable(string target)
        {
            switch (target)
            {

                default: return true;
            }
        }
        public static MethodBase ResolveTarget(string target)
        {
            int split = target.LastIndexOf(':');
            var type = AccessTools.TypeByName(target.Substring(0, split));
            string name = target.Substring(split + 1);
            if (type == null) throw new MissingMethodException("Exact translation type was not found: " + target);
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static;
            MethodBase method;
            if (Parameters.TryGetValue(target, out var parameters))
            {
                var signature = parameters.Select(AccessTools.TypeByName).ToArray();
                if (signature.Any(parameter => parameter == null))
                    throw new MissingMethodException("Translation parameter type was not found: " + target);
                method = name == ".ctor"
                    ? (MethodBase)type.GetConstructor(flags, null, signature, null)
                    : type.GetMethod(name, flags, null, signature, null);
            }
            else method = name == ".ctor"
                ? (MethodBase)type.GetConstructor(flags, null, Type.EmptyTypes, null)
                : type.GetMethod(name, flags);
            if (method == null || method.DeclaringType != type)
                throw new MissingMethodException("Exact translation target was not found: " + target);
            return method;
        }
        public static string Select(string original, string russian)
        {
            return LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true
                ? russian : original;
        }
        public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> input, MethodBase __originalMethod)
        {
            var translations = Methods[__originalMethod.DeclaringType.FullName + ":" + __originalMethod.Name];
            var found = new HashSet<string>();
            var output = new List<CodeInstruction>();
            foreach (var instruction in input)
            {
                output.Add(instruction);
                if (instruction.opcode != OpCodes.Ldstr || !(instruction.operand is string text)
                    || !translations.TryGetValue(text, out var russian)) continue;
                found.Add(text);
                output.Add(new CodeInstruction(OpCodes.Ldstr, russian));
                output.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LiteralTranslations), nameof(Select))));
            }
            if (found.Count != translations.Count)
                throw new InvalidOperationException("Source strings changed in " + __originalMethod + ": "
                    + string.Join(", ", translations.Keys.Where(k => !found.Contains(k))));
            return output;
        }
    }
    [StaticConstructorOnStartup]
    internal static class DeferredLiterals
    {
        static DeferredLiterals() { LongEventHandler.ExecuteWhenFinished(LiteralTranslations.Apply); }
    }
}
