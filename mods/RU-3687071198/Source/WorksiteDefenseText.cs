using System;using System.Globalization;
namespace Igor.WorksitesRussian
{
    public static class WorksiteDefenseText
    {
        private static string Attacker(string value)
        {
            if(value=="raiders")return "налётчики";
            if(value=="raider")return "налётчик";
            var translated=WorksiteAttackerNames.Label(value);
            if(translated==value)translated=WorksiteAttackerNames.Label("a "+value);
            return translated=="a "+value?value:translated;
        }
        private static string Role(string value)=>value=="worker"?"работник":value=="mercenary"?"наёмник":value;
        public static string Two(string a,string b)
        {
            if(a=="the ")return b;
            if(b==" The worksite now considers you their enemy!"||b==". The worksite now considers you their enemy!")
                return a+(a.EndsWith(".")?" ":". ")+"Теперь аванпост считает вас врагами!";
            return string.Concat(a,b);
        }
        public static string Three(string a,string b,string c)
        {
            if(a=="The garrison at "&&c==" was wiped out. The outpost is lost.")return "Гарнизон уничтожен, аванпост потерян. Защитники: "+b+".";
            if((a=="The "||a=="A ")&&(c==" are sacking the worksite."||c==" is sacking the worksite."))return "Аванпост грабят. Нападающие: "+Attacker(b)+".";
            if(a=="The "&&c==" have overrun the outpost. They are sacking the worksite.")return "Аванпост захвачен и подвергается разграблению. Нападающие: "+Attacker(b)+".";
            if(b==" stole a "||b==" stole the worksite's ")return "Кража на аванпосте. Виновник: "+a+". Похищено: "+c;
            if(a=="The worksite's "&&c==" was loaded for transport")return "Имущество аванпоста погружено для вывоза: "+b;
            return string.Concat(a,b,c);
        }
        public static string Four(string a,string b,string c,string d)
        {
            if(a=="the "&&c==" operated by ")return b+" (владелец — "+d+")";
            if(b==" was overrun by "&&d==". The outpost was destroyed.")return "Аванпост разгромлен и уничтожен. Защитники: "+a+". Нападающие: "+Attacker(c)+".";
            if(b==" has concluded that you conspired with the "&&d=="! The worksite now considers you their enemy.")return "Вас считают сообщниками нападающих, и аванпост стал враждебным. Защитники: "+a+". Нападающие: "+Attacker(c)+".";
            return string.Concat(a,b,c,d);
        }
        public static string Array(string[] p)
        {
            if(p.Length==5&&p[0]=="The "&&p[2]==" built by "&&p[4]==" is unpowered.")return "Постройка «"+p[1]+"» не подключена к питанию. Строитель: "+(p[3]=="the crew"?"бригада":p[3])+".";
            if(p.Length==5&&p[1]==" is attacking "&&p[3]==". Fight alongside the defenders to drive them away!")return "Началась атака на аванпост. Нападающие: "+Attacker(p[0])+". Объект: "+p[2]+". Сражайтесь вместе с защитниками, чтобы отбить нападение!"+p[4];
            if(p.Length==8&&p[0]=="You helped "&&p[2]==" defeat the "&&p[4]==". They are grateful that ")return "Вы помогли защитникам аванпоста одержать победу. Защитники: "+p[1]+". Противник: "+Attacker(p[3])+". Они благодарны вашей фракции «"+(p[5]=="your colony"?"колония":p[5])+"» за помощь.\n\n"+p[7];
            if(p.Length==5&&p[0]=="Everyone at "&&p[2]==" was killed by "&&p[4]==". The outpost was destroyed.")return "Все защитники погибли, аванпост уничтожен. Защитники: "+p[1]+". Нападающие: "+Attacker(p[3])+".";
            if(p.Length==5&&p[1]==" defeated "&&p[3]==". Their ranks were bolstered, their armories were replenished, ")return "Защитники аванпоста победили. Защитники: "+p[0]+". Противник: "+Attacker(p[2])+". Гарнизон получил подкрепление, запасы оружия пополнены. "+p[4];
            if(p.Length==5&&p[0]=="Since you are not participating in the defense, "&&p[2]==" suspects you of conspiring with the ")return "Вы не участвуете в обороне, и защитники подозревают вас в сговоре с нападающими. Защитники: "+p[1]+". Нападающие: "+Attacker(p[3])+"!";
            if(p.Length==6&&(p[1]==" attacked the worksite "||p[1]==" killed the worksite ")&&p[3]==" "&&p[5]==".")return (p[1]==" attacked the worksite "?"Нападение на сотрудника аванпоста.":"Убийство сотрудника аванпоста.")+" Виновник: "+p[0]+". Цель: "+p[4]+"; должность — "+Role(p[2])+".";
            return string.Concat(p);
        }
        public static string One(string format,object value)
        {
            if(format=="and they will be holding the site for another {0} days.")return "Они останутся на аванпосте ещё "+Days(Convert.ToInt32(value,CultureInfo.InvariantCulture))+".";
            if(format=="You will not be able to attack this outpost for {0} days. They are willing to parley and trade with you more favorably in the future.")return "Вы не сможете атаковать этот аванпост ещё "+Days(Convert.ToInt32(value,CultureInfo.InvariantCulture))+". В дальнейшем его владельцы будут благосклоннее к вам на переговорах и предложат более выгодные условия торговли.";
            return string.Format(format,value);
        }
        public static string Days(int value){int last=Math.Abs(value)%10,hundred=Math.Abs(value)%100;return value+" "+(hundred>=11&&hundred<=14?"дней":last==1?"день":last>=2&&last<=4?"дня":"дней");}
    }
}
