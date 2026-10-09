using System;
namespace Igor.WorksitesRussian
{
    public static class WorksiteGambleText
    {
        public static string Prompt(string a,string b,string c)
        {
            if(a=="If "&&c==" loses this, they will die. Continue?")return "Проигрыш этой ставки приведёт к смерти. Участник: "+b+". Продолжить?";
            if(a=="Are you sure you want to go all in for "&&c=="?")return "Поставить все оставшиеся деньги — "+b+"?";
            if(a=="No body parts on "&&c==" can be wagered.")return "Нет частей тела, которые можно поставить на кон. Участник: "+b+".";
            if(a=="No equipped weapon or worn apparel on "&&c==".")return "Нет надетой одежды или экипированного оружия для ставки. Участник: "+b+".";
            if(a=="Couldnt materialise the "&&c==". The item was sold instead.")return "Не удалось выдать выигрыш: "+b+". Вместо предмета вам зачислена его стоимость.";
            return string.Concat(a,b,c);
        }
        public static string Title(string a,string b)
        {
            if(a=="Wager ")return "На кону: "+b;
            if(b==" is under anesthetic. Swap colonist?")return a+" находится под наркозом. Заменить участника другим поселенцем?";
            return string.Concat(a,b);
        }
        public static string Wager(string[] p)=>p.Length==5&&p[0]=="Are you sure you want to wager "&&p[2]==" for "&&p[4]=="?"
            ?"Подтвердить ставку?\nНа кону: "+p[1]+".\nРазмер ставки: "+p[3]+".":string.Concat(p);
        public static string Award(string[] p)
        {
            if(p.Length==5&&p[0]=="Took "&&p[2]==" from "&&p[4]==".")return "Вы получаете выигрыш: "+p[1]+". Соперник: "+p[3]+".";
            if(p.Length==5&&p[0]=="cashes in "&&p[2]==" for "&&p[4]==".")return "продаёт выигрыш: "+p[1]+". Получено: "+p[3]+".";
            return string.Concat(p);
        }
    }
}
