using System;
namespace Igor.WorksitesRussian
{
    public static class WorksitePokerText
    {
        public static string OrganSide(string side,string label)=>side=="right "?label+" (справа)":side=="left "?label+" (слева)":string.Concat(side,label);
        public static string Pot(string value)
        {
            if(value=="the pot")return "банк";
            if(value=="the main pot")return "основной банк";
            if(value!=null&&value.StartsWith("side pot ",StringComparison.Ordinal))return "побочный банк №"+value.Substring(9);
            return value;
        }
        public static string Action(string a,string b,string c)
        {
            switch(a)
            {
                case "calls ":return "уравнивает ставку: "+b+c;
                case "raises ":return "повышает ставку: "+b+c;
                case "goes all in for ":return "идёт ва-банк: "+b+c;
                default:return string.Concat(a,b,c);
            }
        }
        public static string Wager(string[] p)
        {
            if(p.Length==6&&p[1]==" worth "&&p[3]==" ("&&p[5]==").")
            {
                string item=p[0]=="wagers apparel"?"одежду":p[0]=="wagers a weapon"?"оружие":p[0]=="wagers a body part"?"часть тела":null;
                if(item!=null)return "ставит на кон "+item+" стоимостью "+p[2]+" ("+p[4]+").";
            }
            return string.Concat(p);
        }
        public static string Result(string a,string b,string c)
        {
            if(b==" wins ")return a+" выигрывает "+c;
            if(a=="wins ")return "выигрывает "+b+c;
            if(a=="shows ")return "вскрывает карты: "+b+c;
            return string.Concat(a,b,c);
        }
        public static string Bank(string[] p)
        {
            if(p.Length==5)
            {
                if(p[0]=="stakes their "&&p[2]==" worth "&&p[4]==".")return "ставит на кон: "+p[1]+". Стоимость: "+p[3]+".";
                if(p[0]=="takes back "&&p[2]==" of "&&p[4]==". The bet was uncalled.")return "забирает "+Pot(p[1])+" обратно — "+p[3]+". Никто не уравнял ставку.";
                if(p[0]=="wins "&&p[2]==" of "&&p[4]==".")return "забирает "+Pot(p[1])+" — "+p[3]+".";
                if(p[0]=="ties for "&&p[2]==" and takes "&&p[4]==".")return "делит "+Pot(p[1])+" и получает "+p[3]+".";
            }
            return string.Concat(p);
        }
        public static string WagerResult(string a,string b,string c)
        {
            if(b==" and "||b==", and ")return a+" и "+c;
            if(a=="kept the wagered "&&c==".")return "сохраняет поставленное на кон: "+b+".";
            if(a=="loses "&&c==" - harvested.")return "проигрывает часть тела: "+b+". Часть тела изъята.";
            return string.Concat(a,b,c);
        }
        public static string GearResult(string[] p)
        {
            if(p.Length==5&&p[2]=="'s "&&p[4]=="."&&(p[0]=="sold "||p[0]=="kept "))
                return (p[0]=="sold "?"продаёт выигранное снаряжение: ":"оставляет себе выигранное снаряжение: ")+p[3]+". Прежний владелец: "+p[1]+".";
            return string.Concat(p);
        }
        public static string Loss(string a,string b,string c,string d)=>b==" lost a "&&d=="."?a+": проигрыш — "+c+".":string.Concat(a,b,c,d);
        public static string Relationship(string a,string b,string c)
        {
            string target=b=="the player"?"вашего игрока":"игрока по имени "+b;
            if(a=="is suspicious of ")return "подозревает "+target+c;
            if(a=="wants revenge on ")return "хочет отомстить "+(b=="the player"?"вашему игроку":"игроку по имени "+b)+c;
            if(a=="is intimidated by ")return "опасается "+target+c;
            if(a=="admires")return "восхищается "+(b=="the player"?"вашим игроком":"игроком по имени "+b)+c;
            return string.Concat(a,b,c);
        }
        public static string DeathFour(string a,string b,string c,string d)=>b==" won the final hand at the card table with "&&d=="."
            ?"Победитель последней раздачи: "+a+". Комбинация: "+c+".":string.Concat(a,b,c,d);
        public static string DeathTwo(string a,string b)=>b==" won the final hand at the card table."?"Победитель последней раздачи: "+a+".":string.Concat(a,b);
        public static string DeathThree(string a,string b,string c)=>a=="The last moments of "&&c=="'s life at the table:"
            ?"Последние мгновения за карточным столом ("+b+"):":string.Concat(a,b,c);
    }
}
