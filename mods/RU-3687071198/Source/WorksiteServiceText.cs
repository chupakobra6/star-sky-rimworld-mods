using System;
using System.Collections.Generic;

namespace Igor.WorksitesRussian
{
    public static partial class WorksiteServiceText
    {
        public static string Laborer(string prefix,string role)
        {
            if(prefix!="outpost ")return string.Concat(prefix,role);
            switch(role)
            {
                case "carwasher": role="мойщик машин";break;
                case "technician": role="техник";break;
                case "mechanic": role="механик";break;
                case "lackey": role="подручный";break;
                case "squire": role="оруженосец";break;
                case "smuggler": role="контрабандист";break;
            }
            return role+" аванпоста";
        }
        public static string CyberParts(string prefix,string labels,string suffix)
        {
            if(prefix=="all of these existing parts: "&&suffix==".")
                return "Будут безвозвратно уничтожены все следующие установленные части: "+labels+".";
            if(prefix=="the existing "&&suffix==".")
                return "Будет безвозвратно уничтожена установленная часть: "+labels+".";
            return string.Concat(prefix,labels,suffix);
        }

        public static string CyberPrompt(string[] p)
        {
            if(p.Length==9&&p[0]=="Installing "&&p[2]==" on "&&p[4]=="'s "&&p[6]==" will permanently destroy ")
                return "Установка «"+p[1]+"».\nПациент: "+p[3]+".\nМесто установки: "+p[5]+".\n\n"+p[7]+" Отменить эти изменения будет нельзя.\n\nПродолжить?";
            return string.Concat(p);
        }

        public static string WinTitle(string prefix,string rival,string possessive,string asset) =>
            prefix=="You won "&&possessive=="'s "?"Выигрыш: «"+asset+"» (соперник: "+rival+")":string.Concat(prefix,rival,possessive,asset);

        public static string WinPrompt(string[] p) =>
            p.Length==5&&p[0]=="Do you want to keep the "&&p[2]==" or cash it in for $"&&p[4]=="?"
                ?"Оставить себе «"+p[1]+"» или продать за $"+p[3]+"?":string.Concat(p);

        public static string Stranded(string format,object pawn,object role)
        {
            if(format!=StrandedSource)return string.Format(format,pawn,role);
            string label=role as string;
            if(label!=null&&WorkerRoles.TryGetValue(label,out var translated))label=translated;
            return string.Format(StrandedRussian,pawn,label);
        }
    }
}
