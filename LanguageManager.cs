using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace mywpf
{
    public static class LanguageManager
    {
        public static string CurrentLanguage { get; private set; } = "ru";
        private static Dictionary<string, dynamic> _currentTranslations;

        public static void ChangeLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            LoadTranslations();
        }
        public static string GetSortOptionTranslation(string key)
        {
            return GetTranslation($"UI.SortOptions.{key}");
        }

        private static void LoadTranslations()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                         $"Translations/translations.{CurrentLanguage}.json");

                Debug.WriteLine($"Loading translations from: {path}");

                if (!File.Exists(path))
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                      "Translations/translations.ru.json");

                string json = File.ReadAllText(path);
                _currentTranslations = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading translations: {ex.Message}");
                _currentTranslations = new Dictionary<string, dynamic>();
            }
        }

        public static string GetTranslation(string key)
        {
            try
            {
                if (_currentTranslations == null)
                    LoadTranslations();

                var parts = key.Split('.');
                dynamic current = _currentTranslations;

                foreach (var part in parts)
                {
                    current = current[part];
                    if (current == null)
                    {
                        Debug.WriteLine($"Translation key not found: {key}");
                        return $"[{key}]";
                    }
                }

                return current.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting translation: {ex.Message}");
                return $"[{key}]";
            }
        }
    }
}