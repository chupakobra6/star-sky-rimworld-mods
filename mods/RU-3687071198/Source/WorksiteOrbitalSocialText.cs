using System;
using System.Collections.Generic;
using System.Linq;
namespace Igor.WorksitesRussian
{
    public static class WorksiteOrbitalSocialText
    {
        private static readonly Dictionary<string,string> Names = new Dictionary<string,string> {
            { "Dulcinea", "Дульсинея" },
            { "Alastor", "Аластор" },
            { "Leviathan", "Левиафан" },
            { "Hyperion", "Гиперион" },
            { "Elsinore", "Эльсинор" },
            { "Babel", "Вавилон" },
            { "Ozymandias", "Озимандия" },
            { "Tartarus", "Тартар" },
            { "Prydwen", "Придвен" },
            { "Golden Hind", "Золотая лань" },
            { "Pelican", "Пеликан" },
            { "Pequod", "Пекод" },
            { "Hringhorni", "Хрингхорни" },
            { "Mora", "Мора" },
            { "Reynard", "Ренар" },
            { "Ishmael", "Измаил" },
            { "Bucephalus", "Буцефал" },
            { "Bavieca", "Бабьека" },
            { "Incitatus", "Инцитат" },
            { "Hengroen", "Хенгроен" },
            { "Al-Buraq", "Аль-Бурак" },
            { "Xanthus", "Ксанф" },
            { "Warwolf", "Боевой волк" },
            { "Bad Neighbor", "Дурной сосед" },
            { "Flotsam", "Плавучие обломки" },
            { "Jetsam", "Выброшенный груз" },
            { "Illyria", "Иллирия" },
            { "Raskolnikov", "Раскольников" },
            { "Wintermute", "Винтермьют" },
            { "Meursault", "Мерсо" },
            { "Yevgenia's", "У Евгении" },
            { "The Grim Outlook", "Мрачная перспектива" },
            { "Dolores Under", "Долорес внизу" },
            { "The Usual", "Как обычно" },
            { "The Pale Occasion", "Блеклый повод" },
            { "Poor Vintage", "Паршивый урожай" },
            { "Sour Mash Republic", "Республика кислой браги" },
            { "Dog Solitude", "Собачье одиночество" },
            { "Fortunate Lapse", "Счастливая оплошность" },
            { "The Known Associate", "Известный сообщник" },
            { "The Dandy Livingstone", "Дэнди Ливингстон" },
            { "Victor Below", "Виктор внизу" },
            { "Elena Elsewhere", "Елена не здесь" },
            { "Dog's Dream", "Собачий сон" },
            { "Magic Dancer", "Волшебный танцор" },
            { "Flying Monkey", "Летающая обезьяна" },
            { "Sale Cochon", "Грязная свинья" },
            { "No Name Club", "Безымянный клуб" },
            { "El Gaucho", "Эль Гаучо" },
            { "Le Million", "Миллион" },
            { "People on Sunday", "Люди в воскресенье" },
            { "Carnival in Flanders", "Героическая кермесса" },
            { "Crime and Punishment", "Преступление и наказание" },
            { "Evening Comes", "Наступает вечер" },
            { "The Last Laugh", "Последний человек" },
            { "Bar M", "Бар «М»" },
            { "Paris Roofs", "Крыши Парижа" },
            { "Mabuse the Gambler", "Мабузе-игрок" },
            { "Bar Vathek", "Бар «Ватек»" },
            { "The Zofloya", "Зофлоя" },
            { "La Navigateur", "Навигатор" },
            { "The King Solomon", "Царь Соломон" },
            { "The Idiot", "Идиот" },
            { "The Stranger", "Посторонний" },
            { "The Fall", "Падение" }
        };
        private static readonly Dictionary<string,string> Patrons = new Dictionary<string,string> {
            { "contract killers", "наёмные убийцы" },
            { "connected fixers", "решалы с нужными связями" },
            { "data brokers", "информационные брокеры" },
            { "drug dealers", "наркоторговцы" },
            { "countermeasure experts", "специалисты по системам противодействия" },
            { "organ traders", "торговцы органами" },
            { "slave traders", "работорговцы" },
            { "bounty hunters", "охотники за головами" },
            { "space pirates", "космические пираты" },
            { "unqualified surgeons", "хирурги-самоучки" },
            { "hydroponics engineers", "инженеры гидропоники" },
            { "deep-space researchers", "исследователи дальнего космоса" },
            { "rogue traders", "вольные торговцы" },
            { "travelling medics", "странствующие медики" },
            { "refugee shipwrights", "беженцы-корабелы" },
            { "religious preachers", "религиозные проповедники" },
            { "charity workers", "сотрудники благотворительных организаций" },
            { "scholar monks", "учёные монахи" },
            { "long-haul space pilots", "пилоты дальних космических рейсов" },
            { "asteroid crackers", "дробильщики астероидов" },
            { "gravship builders", "строители гравикораблей" },
            { "mercenaries", "наёмники" },
            { "cargo auditors", "ревизоры грузов" },
            { "couriers", "курьеры" },
            { "excommunicants", "отлучённые от церкви" },
            { "lost wanderers", "заблудшие странники" },
            { "archotech worshippers", "поклонники архотеков" },
            { "insectoid wranglers", "укротители инсектоидов" },
            { "mechlords", "мехлорды" },
            { "AI peddlers", "торговцы искусственным интеллектом" },
            { "scavengers", "собиратели утиля" },
            { "anomaly tourists", "любители аномального туризма" },
            { "psychic mediums", "медиумы" },
            { "pod racers", "гонщики на капсулах" },
            { "merchant captains", "капитаны торговых судов" },
            { "analysts", "аналитики" },
            { "certified inspectors", "сертифицированные инспекторы" },
            { "customs agents", "таможенники" },
            { "charter pilots", "чартерные пилоты" },
            { "guild brokers", "брокеры гильдий" },
            { "scribes", "писцы" }
        };
        private static readonly Dictionary<string,string> Openings = new Dictionary<string,string> {
            { "a hive of scum and villainy, popular amongst", "рассадник подлости и преступности" },
            { "a beacon for honest work in orbit, frequented by", "оплот честного труда на орбите" },
            { "an unsteady dock of strange company, frequented by", "неспокойный причал, где собирается странная публика" },
            { "a strict platform, frequented by", "платформа со строгими порядками" },
            { "an orbital platform shared by scum and working folk alike, frequented by", "орбитальная платформа, где соседствуют преступники и честные работяги" }
        };

        public static string Station(string value)
        {
            if(value=="Orbital Platform")return "Орбитальная платформа";
            if(value!=null&&value.StartsWith("The ")&&Names.TryGetValue(value.Substring(4),out var article))return article;
            if(value!=null&&value.EndsWith(" Station")&&Names.TryGetValue(value.Substring(0,value.Length-8),out var station))return "Станция «"+station+"»";
            return value;
        }
        public static string Hub(string value)
        {
            if(value=="Recruitment hub")return "Центр найма";
            return value!=null&&Names.TryGetValue(value,out var name)?name:value;
        }
        public static string Brief(string station,string opening,IReadOnlyList<string> selected)
        {
            if(string.IsNullOrEmpty(station)||station=="The station")station="Эта станция";
            if(!Openings.TryGetValue(opening,out var description))throw new ArgumentException("Unreviewed station opening: "+opening);
            var people=selected.Select(s=>Patrons.TryGetValue(s,out var name)?name:throw new ArgumentException("Unreviewed patron: "+s)).ToArray();
            var list=people.Length==0?"самые разные путешественники":people.Length==1?people[0]:string.Join(", ",people.Take(people.Length-1))+" и "+people.Last();
            return station+" — "+description+". Здесь часто бывают "+list+".";
        }
    }
}
