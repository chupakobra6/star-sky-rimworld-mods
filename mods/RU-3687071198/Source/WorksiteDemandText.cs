using System;
namespace Igor.WorksitesRussian
{
    public static class WorksiteDemandText
    {
        public static string Role(string value,bool accusative=false)
        {
            switch(value?.ToLowerInvariant())
            {
                case "miner":return accusative?"шахтёра":"шахтёр";
                case "farmer":return accusative?"фермера":"фермер";
                case "soldier":return accusative?"солдата":"солдат";
                case "cook":return accusative?"повара":"повар";
                case "crafter":return accusative?"ремесленника":"ремесленник";
                case "researcher":return accusative?"исследователя":"исследователь";
                case "worker":return accusative?"рабочего":"рабочий";
                default:return value;
            }
        }
        public static string Days(object days)
        {
            int value=Convert.ToInt32(days),n=Math.Abs(value)%100;
            return days+" "+(n>=11&&n<=14?"дней":n%10==1?"день":n%10>=2&&n%10<=4?"дня":"дней");
        }
        public static string RewardTwo(string f,object a,object b)
        {
            switch(f)
            {
                case "They will give you {0} {1}.":return "Вам передадут: "+b+" ×"+a+".";
                case "They can offer between {0} and {1} silver.":return "Вам могут предложить от "+a+" до "+b+" серебра.";
                case "{0}'s goodwill toward you will increase by {1}.":return "Отношение фракции «"+a+"» к вам улучшится на "+b+".";
                case "All colonists receive +{0} mood for {1} days.":return "Все поселенцы получат +"+a+" к настроению на "+Days(b)+".";
                default:return string.Format(f,a,b);
            }
        }
        public static string RewardThree(string f,object a,object b,object c)
        {
            if(f=="They can offer between {0} and {1} {2}.")return "Вам могут предложить: "+c+" — от "+a+" до "+b+".";
            if(f=="{0} gains {1}-{2} royal favor.")return a+" получит от "+b+" до "+c+" чести.";
            return string.Format(f,a,b,c);
        }
        public static string WorkerReward(string a,string b,string c)
        {
            if(a=="They may offer a "&&c==".")return "Вам могут предложить специалиста: "+Role(b)+".";
            if(a=="They'll hand over a less experienced "&&c==".")return "Вам передадут менее опытного специалиста: "+Role(b)+".";
            return string.Concat(a,b,c);
        }
        public static string Missing(string actor,string separator,string subject,string suffix)
        {
            if(separator==" says they have no "&&suffix==" at this time.")return actor+" сообщает, что сейчас на аванпосте нет запрошенного.\nЗапрошено: "+Role(subject)+".";
            return string.Concat(actor,separator,subject,suffix);
        }
        public static string RepeatMissing(string actor,string separator,string subject,string suffix)
        {
            if(separator==" already told you they don't have a "&&suffix==".")return actor+" напоминает, что на аванпосте нет запрошенного.\nЗапрошено: "+subject+".";
            return string.Concat(actor,separator,subject,suffix);
        }
        public static string MissingTwo(string actor,string suffix)=>suffix==" says they can't provide that right now."
            ?actor+" сообщает, что сейчас аванпост не может выполнить этот запрос.":string.Concat(actor,suffix);
        public static string Counter(string f,object a,object b)=>f=="{0} has countered your offer. The most they can spare is {1}."
            ?a+" делает встречное предложение. Максимум, который может выделить аванпост: "+b+".":string.Format(f,a,b);
        public static string SpecificThree(string a,string b,string c)
        {
            if(a=="Demanding a specific "&&c==".")return "Запрошена определённая профессия: "+Role(b)+".";
            if(a=="Demanding "&&c==" endorsement specifically.")return "Запрошена поддержка определённой фракции: "+b+".";
            return string.Concat(a,b,c);
        }
        public static string SpecificArray(string[] p)=>p.Length==5&&p[0]=="Demanding specific "&&p[2]==" ("&&p[4]==")."
            ?"Запрошен определённый предмет: "+p[1]+" ("+p[3]+").":string.Concat(p);
        public static string Bounty(string a,string b,string c)=>a=="The "&&c==" has placed a bounty on your colony!"
            ?"За вашу колонию назначена награда! Заказчик: "+b+".":string.Concat(a,b,c);
    }
}
