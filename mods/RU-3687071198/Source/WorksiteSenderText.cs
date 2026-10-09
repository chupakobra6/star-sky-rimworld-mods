using System.Collections.Generic;
namespace Igor.WorksitesRussian
{
    public static class WorksiteSenderText
    {
        private static readonly Dictionary<string,string> Words = new Dictionary<string,string>
        {
            {"mysterious benefactor", "таинственный благодетель"},
            {"corporate agent", "агент корпорации"},
            {"anonymous defector", "анонимный перебежчик"},
            {"shadow broker", "тайный посредник"},
            {"local arms dealer", "местный торговец оружием"},
            {"rogue mechanoid", "механоид-отступник"},
            {"wandering shaman", "странствующий шаман"},
            {"tribal elder", "старейшина племени"},
            {"mysterious stranger", "таинственный незнакомец"},
            {"psychic hermit", "отшельник с пси-способностями"},
            {"tribal outcast", "изгой из племени"},
            {"lone scavenger", "одинокий сборщик утиля"},
            {"using an encrypted frequency", "по зашифрованному каналу связи"},
            {"using a message tied to the leg of a well-trained bird", "с помощью записки, привязанной к лапке обученной птицы"},
        };
        public static string Translate(string key, string value)
        {
            if (key == "sender_article") return "";
            if ((key == "sender_label" || key == "contactMethod") && value != null && Words.TryGetValue(value, out var russian)) return russian;
            return value;
        }
    }
}
