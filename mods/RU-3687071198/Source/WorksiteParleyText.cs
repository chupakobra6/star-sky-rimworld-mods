using System;
using System.Collections.Generic;
using System.Globalization;
namespace Igor.WorksitesRussian
{
    public static class WorksiteParleyText
    {
        private static readonly Dictionary<string,string> Moods=new Dictionary<string,string>
        { {"warmly","радушно"},{"formally","официально"},{"suspiciously","с подозрением"},{"contemptuously","презрительно"} };
        private static readonly Dictionary<string,string> Tiers=new Dictionary<string,string>
        { {"legendary","легендарное мастерство"},{"planet-leading","в числе лучших на планете"},{"planet-known","известность на всей планете"},{"region-leading","в числе лучших в регионе"},{"region-known","известность в регионе"},{"capable","уверенное владение"} };
        public static string Greeting(string leader,string envoy,string mood)
        {
            if(leader=="The outpost leader")leader="Начальник аванпоста";
            if(envoy=="your envoy")envoy="ваш представитель";
            string adverb="";
            if(mood!=null) {if(!Moods.TryGetValue(mood,out adverb))throw new InvalidOperationException("Unreviewed Worksites greeting mood: "+mood);adverb+=" ";}
            return leader+" "+adverb+"приветствует вашего представителя и спрашивает о цели визита. Переговорщик: "+envoy+".";
        }
        public static string ArrayGreeting(string[] parts)
        {
            if(parts.Length!=6||parts[1]!=" greets "||parts[3]!=" "||parts[5]!=" and asks what business you want to handle.")throw new InvalidOperationException("Worksites greeting parts changed.");
            return Greeting(parts[0],parts[2],parts[4]);
        }
        public static string FourGreeting(string a,string b,string c,string d)
        {
            if(b!=" greets "||d!=" and asks what business you want to handle.")throw new InvalidOperationException("Worksites neutral greeting changed.");
            return Greeting(a,c,null);
        }
        public static string RepeatGoodWord(string a,string b,string c,string d)
        {
            if(a!=c||b!=" recently asked nicely, they won't fall for that. Use "||d!=" to sweeten the deal.")throw new InvalidOperationException("Worksites repeated request context changed.");
            return "Вежливая просьба уже была. Переговорщик — "+a+"; повторить её может только этот же человек.";
        }
        public static string Plural(int count,string one,string few,string many)
        {
            int n=Math.Abs(count)%100;
            return n>=11&&n<=14?many:n%10==1?one:n%10>=2&&n%10<=4?few:many;
        }
        private static int Count(object value)=>Convert.ToInt32(value,CultureInfo.InvariantCulture);
        public static string One(string format,object a)
        {
            if(format!="All {0} colonists are unarmed."&&format!="All {0} colonists are unarmored.")return string.Format(format,a);
            int n=Count(a);bool armed=format=="All {0} colonists are unarmed.";
            if(n==1)return "Единственный поселенец "+(armed?"без оружия.":"без брони.");
            return "Все поселенцы ("+n+")"+(armed?" без оружия.":" без брони.");
        }
        public static string Two(string format,object a,object b)
        {
            if(format=="{0} {1} in the caravan.") {int n=Count(a);return "В караване "+n+" "+Plural(n,"поселенец","поселенца","поселенцев")+".";}
            if(format=="{0} {1} unarmed."||format=="{0} {1} unarmored.") {int n=Count(a);return (format.EndsWith("unarmed.",StringComparison.Ordinal)?"Без оружия: ":"Без брони: ")+n+" "+Plural(n,"поселенец","поселенца","поселенцев")+".";}
            if(format=="{0} war {1} in the caravan.") {int n=Count(a);return "В караване "+n+" "+Plural(n,"боевое животное","боевых животных","боевых животных")+".";}
            if(format=="{0} additional {1} in the caravan.")
            {
                int n=Count(a);string kind=(string)b;
                if(kind=="psycaster"||kind=="psycasters")return "В караване ещё "+n+" "+Plural(n,"псионик","псионика","псиоников")+".";
                if(kind=="hussar"||kind=="hussars")return "В караване ещё "+n+" "+Plural(n,"гусар","гусара","гусаров")+".";
                throw new InvalidOperationException("Worksites extra caravan member type changed: "+kind);
            }
            return string.Format(format,a,b);
        }
        public static string Three(string format,object a,object b,object c)
        {
            if(format!="{0} is a {1} {2}.")return string.Format(format,a,b,c);
            if(!Tiers.TryGetValue((string)b,out var tier))throw new InvalidOperationException("Worksites mastery tier changed: "+b);
            return a+": "+c+" — "+tier+".";
        }
    }
}
