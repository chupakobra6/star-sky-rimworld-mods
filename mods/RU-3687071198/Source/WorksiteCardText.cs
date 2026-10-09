using System;
using System.Globalization;

namespace Igor.WorksitesRussian
{
    public static class WorksiteCardText
    {
        private static readonly string[] Singular={"двойка","тройка","четвёрка","пятёрка","шестёрка","семёрка","восьмёрка","девятка","десятка","валет","дама","король","туз"};
        private static readonly string[] Plural={"двойки","тройки","четвёрки","пятёрки","шестёрки","семёрки","восьмёрки","девятки","десятки","валеты","дамы","короли","тузы"};
        public static readonly string[] Categories={"Старшая карта","Пара","Две пары","Тройка","Стрит","Флеш","Фул-хаус","Каре","Стрит-флеш"};
        public static string RankWord(int rank,bool plural)
        {
            return rank>=2&&rank<=14?(plural?Plural:Singular)[rank-2]:rank.ToString(CultureInfo.InvariantCulture);
        }
        public static string Face(byte rank)
        {
            switch(rank){case 14:return "Т";case 13:return "К";case 12:return "Д";case 11:return "В";default:return rank.ToString(CultureInfo.InvariantCulture);}
        }
        public static string Badge(string text)
        {
            if(string.IsNullOrEmpty(text)||text=="-")return text;
            switch(text){case "Fold":return "Пас";case "Check":return "Чек";
                case "Wagered body part":return "На кону часть тела";
                case "Wagered apparel":return "На кону одежда";
                case "Wagered weapon":return "На кону оружие";}
            string[] source={"Call ","Raise ","All in ","Win ","SB ","BB "};
            string[] russian={"Уравнено: ","Повышение: ","Ва-банк: ","Выигрыш: ","МБ ","ББ "};
            for(int i=0;i<source.Length;i++)if(text.StartsWith(source[i],StringComparison.Ordinal))return russian[i]+text.Substring(source[i].Length);
            return text;
        }
    }
}
