using System;
using System.Collections.Generic;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteWorkerText
    {
        private static readonly Dictionary<string,string> Templates = new Dictionary<string,string>
        {
            {"{0} has volunteered to join you.", "{PAWN_labelShort} {PAWN_gender ? добровольно вызвался : добровольно вызвалась : добровольно соглашается} присоединиться к вам."},
            {"{0} seems to admire your bravery, and has defected.", "{PAWN_labelShort}, похоже, восхищается вашей храбростью и {PAWN_gender ? перешёл : перешла : переходит} на вашу сторону."},
            {"{0} left their crew behind.", "{PAWN_labelShort} {PAWN_gender ? покинул : покинула : покидает} своих товарищей."},
            {"{0} chose to leave.", "{PAWN_labelShort} {PAWN_gender ? решил : решила : решает} уйти."},
            {"{0} has been disposed of by their faction because of their uselessness.", "{PAWN_labelShort} больше не приносит пользы своей фракции, и та избавляется от {PAWN_gender ? него : неё : этого человека}."},
            {"{0} is offered as tribute.", "{PAWN_labelShort} {PAWN_gender ? предложен : предложена : предлагается} в качестве дани."},
            {"{0} was caught and handed over.", "{PAWN_labelShort} {PAWN_gender ? был пойман и передан : была поймана и передана : попадает в плен и передаётся} вам."},
            {"{0} was dragged from the outpost.", "{PAWN_labelShort} {PAWN_gender ? был выволочен : была выволочена : силой выдворяется} с аванпоста."},
            {"{0} reluctantly agreed to join you.", "{PAWN_labelShort} неохотно {PAWN_gender ? согласился : согласилась : соглашается} присоединиться к вам."},
            {"{0} was convinced to leave.", "{PAWN_labelShort} {PAWN_gender ? поддался : поддалась : поддаётся} уговорам уйти."},
            {"{0} surrendered.", "{PAWN_labelShort} {PAWN_gender ? сдался : сдалась : сдаётся}."},
            {"{0} will begrudgingly become your hostage.", "{PAWN_labelShort} против воли станет {PAWN_gender ? вашим заложником : вашей заложницей : заложником вашей колонии}."},
            {"{0} will become your prisoner.", "{PAWN_labelShort} станет {PAWN_gender ? вашим пленником : вашей пленницей : пленником вашей колонии}."},
            {"{0} will be handed over in bonds.", "{PAWN_labelShort} {PAWN_gender ? будет передан : будет передана : передаётся} вам в оковах."},
            {"{0} was offered as a hostage.", "{PAWN_labelShort} {PAWN_gender ? предложен в качестве заложника : предложена в качестве заложницы : предлагается в заложники}."},
        };
        public static string Format(string source, Pawn pawn)
        {
            if (!Templates.TryGetValue(source, out var russian)) throw new InvalidOperationException("Unreviewed Worksites worker story: " + source);
            if (pawn == null) throw new ArgumentNullException(nameof(pawn));
            return russian.Formatted(pawn.Named("PAWN")).Resolve();
        }
    }
}
