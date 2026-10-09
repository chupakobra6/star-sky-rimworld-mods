using System;
namespace Igor.WorksitesRussian
{
    public static class WorksiteOfferPanicText
    {
        public static string Confirmation(string a,string name,string c)
        {
            if(c=="'s mood. Continue?")
            {
                if(a=="Asking nicely will further reduce ")return "Вежливая просьба ещё сильнее ухудшит настроение. Переговорщик: "+name+". Продолжить?";
                if(a=="Asking nicely will reduce ")return "Вежливая просьба ухудшит настроение. Переговорщик: "+name+". Продолжить?";
            }
            if(a=="You are offering "&&c==", they will be removed from the caravan. Continue?")return "Вы предлагаете передать человека. Караван покинет "+name+". Продолжить?";
            return string.Concat(a,name,c);
        }
        private static string Leader(string name)=>name=="The outpost leader"?"Начальник аванпоста":name;
        public static string DemandFour(string a,string b,string c,string d)
        {
            if(b==". "&&d==" demands you abjectly beg forgiveness before the outpost.")return a+". "+Leader(c)+" требует, чтобы вы униженно молили о прощении перед жителями аванпоста.";
            return string.Concat(a,b,c,d);
        }
        public static string DemandArray(string[] p)
        {
            if(p.Length==6&&p[1]==". "&&p[5]==".")
            {
                if(p[3]==" demands you hand over your prisoner, ")return p[0]+". "+Leader(p[2])+" требует выдать вашего пленника: "+p[4]+".";
                if(p[3]==" demands you hand over your colonist, ")return p[0]+". "+Leader(p[2])+" требует выдать вашего поселенца: "+p[4]+".";
            }
            if(p.Length==8&&p[1]==", "&&p[3]==" demands that you give him "&&p[5]=="'s "&&p[7]==" as tribute.")return p[0]+". "+Leader(p[2])+" требует передать в качестве дани предмет «"+p[6]+"». Владелец: "+p[4]+".";
            return string.Concat(p);
        }
        public static string Fine(string format,object reason,object leader,object silver)
        {
            if(format=="{0}, {1} is requesting a fine of {2} silver.")return reason+". "+Leader((string)leader)+" требует уплатить штраф: "+silver+" серебра.";
            return string.Format(format,reason,leader,silver);
        }
        public static string Stealth(string role,string suffix)
        {
            if(suffix==" eliminated before requesting backup.")return (role=="Worker"?"Работник":role=="Mercenary"?"Наёмник":role)+" устранён и не успел запросить подкрепление.";
            return string.Concat(role,suffix);
        }
        public static string Downed(string name,string b,string state,string d)
        {
            if(b==" was "&&d==" before they could call for backup.")return (name=="Worker"?"Работник":name)+" не успевает запросить подкрепление: "+(state=="killed"?"смерть наступила раньше.":state=="downed"?"больше не может действовать.":state+".");
            return string.Concat(name,b,state,d);
        }
        public static string Witness(string witness,string b,string victim,string d)
        {
            if(b==" saw "&&d==" get injured, they are requesting backup.")return witness+" замечает ранение и вызывает подкрепление. Пострадавший: "+victim+".";
            return string.Concat(witness,b,victim,d);
        }
    }
}
