using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace PWBG_BOT
{
    class DataStorage
    {
        private static Dictionary<string, string> pairs = new Dictionary<string, string>();

        static DataStorage()
        {
            if (!GetOrCreateFile("SystemLang/data.json")) return;

            string json = File.ReadAllText("SystemLang/data.json");
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SavePairs(string data, string value)
        {
            pairs.Add(data,value);
            SaveData();
        }

        public static int GetPairsCount()
        {
            return pairs.Count;
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText("SystemLang/data.json",json);
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
