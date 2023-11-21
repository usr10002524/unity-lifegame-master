using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// アプリのロケール設定
/// </summary>
public class AppLocale
{
    /// <summary>
    /// ロケールを設定する
    /// </summary>
    /// <param name="langType">言語のタイプ</param>
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

/// <summary>
/// 文字列変換データベース
/// </summary>
public class StringDatabase
{
    /// <summary>
    /// 指定されたテーブルで文字列を変換する
    /// </summary>
    /// <param name="tableName">変換テーブル名</param>
    /// <param name="target">変換元の文字列</param>
    /// <returns>変換後の文字列</returns>
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
