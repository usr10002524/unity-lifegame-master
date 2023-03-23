using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class AppLocale
{
    public static void SetLocale(LangType langType)
    {
        Locale locale = null;
        switch (langType)
        {
            default:
            case LangType.LangEn: locale = Locale.CreateLocale("en"); break;
            case LangType.LangJp: locale = Locale.CreateLocale("ja-JP"); break;
        }
        // Locale a = Locale.CreateLocale(locale);
        LocalizationSettings.SelectedLocale = locale;
    }
}

public class StringDatabase
{
    public static string GetEntry(string tableName, string target)
    {
        var entry = LocalizationSettings.StringDatabase.GetTableEntry(tableName, target).Entry;
        if (entry == null)
        {
            return "";
        }
        else
        {
            return entry.Value;
        }
    }
}
