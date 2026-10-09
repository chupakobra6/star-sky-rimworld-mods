using System;
using System.Collections.Generic;
using System.Reflection;
using Verse.Grammar;
namespace Igor.WorksitesRussian
{
    public static class WorksiteFirstQuestNames
    {
        private static readonly FieldInfo Weight = typeof(Rule_String).GetField("weight", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingFieldException("Rule_String.weight");
        public static List<Rule> Create(bool industrial)
        {
            var result = new List<Rule>();
            if (industrial)
            {
                Add(result, "Благодетель в тени", 0.04f);
                Add(result, "Скрытый сигнал", 0.04f);
                Add(result, "Тайная наводка", 0.04f);
                Add(result, "Контакт в тени", 0.04f);
                Add(result, "Скрытая передача", 0.04f);
                Add(result, "Молчаливый благодетель", 0.04f);
                Add(result, "Безмолвный сигнал", 0.04f);
                Add(result, "Негласная наводка", 0.04f);
                Add(result, "Негласный контакт", 0.04f);
                Add(result, "Беззвучная передача", 0.04f);
                Add(result, "Благодетель из корпорации", 0.24f);
                Add(result, "Сигнал корпорации", 0.24f);
                Add(result, "Наводка из корпорации", 0.24f);
                Add(result, "Контакт из корпорации", 0.24f);
                Add(result, "Передача корпорации", 0.24f);
                Add(result, "Тайный благодетель", 0.04f);
                Add(result, "Секретный сигнал", 0.04f);
                Add(result, "Секретная наводка", 0.04f);
                Add(result, "Тайный контакт", 0.04f);
                Add(result, "Секретная передача", 0.04f);
                Add(result, "Анонимный благодетель", 0.04f);
                Add(result, "Анонимный сигнал", 0.04f);
                Add(result, "Анонимная наводка", 0.04f);
                Add(result, "Анонимный контакт", 0.04f);
                Add(result, "Анонимная передача", 0.04f);
            }
            else
            {
                Add(result, "Древнее предостережение", 0.1f);
                Add(result, "Древнее знамение", 0.1f);
                Add(result, "Древний шёпот", 0.1f);
                Add(result, "Древний слух", 0.1f);
                Add(result, "Древнее послание", 0.1f);
                Add(result, "Тревожное предостережение", 0.1f);
                Add(result, "Тревожное знамение", 0.1f);
                Add(result, "Тревожный шёпот", 0.1f);
                Add(result, "Тревожный слух", 0.1f);
                Add(result, "Тревожное послание", 0.1f);
                Add(result, "Загадочное предостережение", 0.1f);
                Add(result, "Загадочное знамение", 0.1f);
                Add(result, "Загадочный шёпот", 0.1f);
                Add(result, "Загадочный слух", 0.1f);
                Add(result, "Загадочное послание", 0.1f);
                Add(result, "Странное предостережение", 0.1f);
                Add(result, "Странное знамение", 0.1f);
                Add(result, "Странный шёпот", 0.1f);
                Add(result, "Странный слух", 0.1f);
                Add(result, "Странное послание", 0.1f);
            }
            return result;
        }
        private static void Add(List<Rule> rules, string name, float weight)
        {
            var rule = new Rule_String("questName", name);
            Weight.SetValue(rule, weight);
            rule.AddConstantConstraint("isFirstQuest", "true", "==");
            rules.Add(rule);
        }
    }
}
