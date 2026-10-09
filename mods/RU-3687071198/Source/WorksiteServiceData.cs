using System.Collections.Generic;
namespace Igor.WorksitesRussian { public static partial class WorksiteServiceText {
public const string StrandedSource="Desperate for an exit before station security fines you for vagrancy, {0} tracks down an overworked {1} in the maintenance hub. They won't give you a loan, but for a fraction of your shuttle's scrap value, they can stuff you into a stripped-down, unguided cargo pod and pull the manual release lever. \n\nIt's going to be a rough, freezing hour in the dark of space, tumbling through the void with nothing but the timer counting down. But if the heat shields on your cargo pod haven't already been looted and the atmosphere doesn't burn you to ash, you will slam right back down into the dirt of the rimworld below. It's dangerous and it's undignified, but it's better than rotting in a cell or waiting for them to cut off your oxygen.";
public const string StrandedRussian="Нужно выбраться отсюда, пока охрана станции не выписала штраф за бродяжничество. В техническом узле {0} находит заваленного работой {1}. Кредита вам не дадут, зато за часть выручки от разборки вашего челнока готовы запихнуть вас в ободранную неуправляемую грузовую капсулу и дёрнуть ручной рычаг сброса.\n\nВпереди — тяжёлый, ледяной час в космической темноте. Вы будете кувыркаться в пустоте, глядя лишь на обратный отсчёт таймера. Но если теплозащиту капсулы ещё не растащили и атмосфера не сожжёт вас дотла, вы с размаху врежетесь в грунт пограничного мира внизу. Опасно и унизительно — но лучше, чем гнить в камере или ждать, пока вам перекроют кислород.";
private static readonly Dictionary<string,string> WorkerRoles=new Dictionary<string,string> {
{"worker","рабочего"},
{"dockworker","докера"},
{"deckhand","палубного рабочего"},
{"scrapper","сборщика утиля"},
{"pipe-fitter","трубопроводчика"},
{"electrician","электрика"},
{"mechanic","механика"},
{"foreman","бригадира"},
{"supervisor","начальника смены"},
{"technician","техника"}
};
}}
