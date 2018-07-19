using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace PWBG_BOT
{
    public static class MainStorage
    {
        private static Dictionary<string, ulong> pairs = new Dictionary<string, ulong>();

        private static string FileLocation = "SystemLang/data.json";

        static MainStorage()
        {
            if (!GetOrCreateFile(FileLocation)) return;

            string json = File.ReadAllText(FileLocation);
            pairs = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(json);
        }

        public static Dictionary<string, ulong> LoadPairs()
        {
            return pairs;
        }

        public static ulong GetValueOf(string key)
        {
            if (pairs.ContainsKey(key)) return pairs[key];
            return 0;
        }

        public static void ChangeData(string key, ulong newValue)
        {
            pairs[key] = newValue;
            SaveData();
        }

        public static void SavePairs(string data, ulong value)
        {
            pairs.Add(data, value);
            SaveData();
        }

        public static int GetPairsCount()
        {
            return pairs.Count;
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText(FileLocation, json);
        }

        private static bool GetOrCreateFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }

    }
}
