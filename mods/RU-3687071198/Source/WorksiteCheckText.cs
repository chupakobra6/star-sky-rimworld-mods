using System;
using System.Collections.Generic;

namespace Igor.WorksitesRussian
{
    public static class WorksiteCheckText
    {
        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>
        {
            {"Streetwise", "Знание улиц"}, {"Tech / Slicing", "Техника / взлом"},
            {"Stealth", "Скрытность"}, {"Electronics", "Электроника"},
            {"Smooth talk / Pressure", "Уговоры / давление"},
            {"Close Quarters / Blade", "Ближний бой / клинки"},
            {"Intimidate", "Запугивание"}, {"Ballistic Clearance", "Расчистка пути огнём"},
            {"Performance", "Лицедейство"}, {"Medical", "Медицина"},
            {"Awareness", "Наблюдательность"}, {"Melee / Blade", "Ближний бой / клинки"},
            {"Chemistry", "Химия"}, {"Demolitions", "Подрывное дело"},
            {"Animal Handling", "Обращение с животными"}, {"Athletics", "Атлетика"},
            {"Initiative", "Инициатива"}, {"Fast Talk / Persuasion", "Красноречие / убеждение"},
            {"Shooting / Rifle", "Стрельба / винтовки"}, {"Tech / Crafting", "Техника / ремесло"},
            {"Combat", "Бой"}, {"Firefight", "Перестрелка"}
        };

        public static string Name(string value, Func<string, string> skill)
        {
            if (value == null) return null;
            if (Names.TryGetValue(value, out var translated)) return translated;
            var parts = value.Split('/');
            bool translatedPart = false;
            for (int i = 0; i < parts.Length; i++)
            {
                string id = parts[i].Trim();
                parts[i] = skill(id);
                translatedPart |= parts[i] != id;
            }
            return translatedPart ? string.Join(" / ", parts) : value;
        }

        public static string Stat(string value, bool shortLabel = false)
        {
            switch (value)
            {
                case "BODY": return shortLabel ? "ТЕЛ" : "Телосложение";
                case "REF": return shortLabel ? "РЕА" : "Реакция";
                case "COOL": return shortLabel ? "САМ" : "Самообладание";
                case "INT": return shortLabel ? "ИНТ" : "Интеллект";
                case "TECH": return shortLabel ? "ТЕХ" : "Технические способности";
                case "EMP": return shortLabel ? "ЭМП" : "Эмпатия";
                default: return value;
            }
        }

        public static string Difficulty(object value)
        {
            switch (value as string)
            {
                case "Easy": return "низкая";
                case "Average": return "средняя";
                case "Difficult": return "высокая";
                case "Adapt": return "адаптация";
                case "Firefight": return "перестрелка";
                default: return Convert.ToString(value);
            }
        }

        public static string SubOne(string format, object dc) =>
            format == "Firefight (DC {0})" ? string.Format("Перестрелка (сложность {0})", dc) : string.Format(format, dc);

        public static string SubThree(string format, object name, object dc, object difficulty) =>
            format == "{0} ({1}) - {2}" ? string.Format("{0} (сложность {1}; {2})", name, dc, Difficulty(difficulty)) : string.Format(format, name, dc, difficulty);

        public static string Formula(string a, string stat, string c, string name) =>
            a == "1d20 + " && c == " + " ? "1d20 + " + Stat(stat, true) + " + " + name : string.Concat(a, stat, c, name);

        public static string Math(string format, object line, object dc, object difficulty) =>
            format == "{0}   vs DC {1} ({2})" ? string.Format("{0}   против сложности {1} ({2})", line, dc, Difficulty(difficulty)) : string.Format(format, line, dc, difficulty);

        public static string TipFactor(string format, object pawn, object name, object value) =>
            format == "{0} {1} ({2})" ? string.Format("{0}: {1} ({2})", pawn, name, value) : string.Format(format, pawn, name, value);

        public static string TipOdds(string format, object color, object chance) =>
            format == "There is a <color={0}>{1}%</color> chance to succeed." ? string.Format("Шанс успеха: <color={0}>{1}%</color>.", color, chance) : string.Format(format, color, chance);

        public static string SkillFactor(string format, object prefix, object label, object level)
        {
            if (format != "{0}{1} ({2})") return string.Format(format, prefix, label, level);
            string rating;
            switch (prefix as string)
            {
                case "Low ": rating = "низкий уровень"; break;
                case "Good ": rating = "хороший уровень"; break;
                case "Excellent ": rating = "отличный уровень"; break;
                default: return string.Format("{0} ({1})", label, level);
            }
            return string.Format("{0} ({1}; {2})", label, level, rating);
        }

        public static string Explain(string format, object name, object level, Func<string, string> skill) =>
            format == "{0} {1}÷2" ? string.Format("{0}: {1} ÷ 2", Name(name as string, skill), level) : string.Format(format, name, level);

        public static string SkillTipTwo(string format, object name, object value, Func<string, string> skill)
        {
            if (format == "{0} ({1})") return string.Format(format, Name(name as string, skill), value);
            return string.Format(format, name, value);
        }

        public static string SkillTipThree(string format, object a, object b, object c, Func<string, string> skill)
        {
            if (format == "Cooking {0} + Crafting {1}, ÷4 = +{2}")
                return string.Format("({0} {1} + {2} {3}) ÷ 4 = +{4}", skill("Cooking"), a, skill("Crafting"), b, c);
            if (format == "{0} {1} ÷2 = +{2}") return string.Format("{0} {1} ÷ 2 = +{2}", skill(a as string), b, c);
            return string.Format(format, a, b, c);
        }

        public static string Resolve(string format, object[] args, Func<string, string> skill)
        {
            if (format == "{0} {1}  +  {2} {3}  +  1d20" && args.Length == 4)
                return string.Format("{0} {1} + {2} {3} + 1d20", Stat(args[0] as string, true), args[1], Name(args[2] as string, skill), args[3]);
            if (format == "{0} + {1} + {2} = {3}   vs DC {4}" && args.Length == 5)
                return string.Format("{0} + {1} + {2} = {3}   против сложности {4}", args);
            return string.Format(format, args);
        }

        public static string Refusal(string trait, string suffix)
        {
            if (suffix == " crew refuses this" && trait == "Kind")
                return "Все поселенцы в экипаже — добряки и отказываются от этого действия.";
            return string.Concat(trait, suffix);
        }

        public static string Required(string prefix, string gear, string suffix) =>
            prefix == "Requires " && suffix == " in caravan" ? "Требуется снаряжение в караване: " + gear : string.Concat(prefix, gear, suffix);
    }
}
