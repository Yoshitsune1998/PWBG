using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;

namespace PWBG_BOT
{
    class Utilities
    {
        private static Dictionary<string, string> _dictionary;

        static Utilities()
        {
            string json = File.ReadAllText("SystemLang/alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            _dictionary = data.ToObject<Dictionary<string,string>>();
        }

        public static string GetText(string key)
        {
            if (_dictionary.ContainsKey(key)) return _dictionary[key];
            return "";
        }

        public static string GetFormattedText(string key, object param)
        {
            return String.Format(_dictionary[key], new object[] { param });
        }

        public static string GetFormattedText(string key, params object[] param)
        {
            if (_dictionary.ContainsKey(key))
            {
                return String.Format(_dictionary[key], param);
            }
            return "";
        }

    }
}
