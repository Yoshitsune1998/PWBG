using System;
using System.Collections.Generic;
using System.Text;
using PWBG_BOT.Core.UserAccounts;
using PWBG_BOT.Core.BuffAndDebuff;
using PWBG_BOT.Core.Items;
using PWBG_BOT.Core.SurvivorInventory;
using PWBG_BOT.Core.System;
using Newtonsoft.Json;
using System.IO;

namespace PWBG_BOT.Core
{
    public static class DataStorage
    {
        /* SAVE FILE */

        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        public static void SaveInventory(IEnumerable<Inventory> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        public static void SaveItems(IEnumerable<Item> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        public static void SaveBuffs(IEnumerable<Buff> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        public static void SaveDebuffs(IEnumerable<Debuff> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        public static void SaveQuizzes(IEnumerable<Quiz> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        /* LOAD FILE */

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static IEnumerable<Inventory> LoadInventory(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Inventory>>(json);
        }

        public static IEnumerable<Item> LoadItem(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Item>>(json);
        }

        public static IEnumerable<Buff> LoadBuff(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Buff>>(json);
        }

        public static IEnumerable<Debuff> LoadDebuff(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Debuff>>(json);
        }

        public static IEnumerable<Quiz> LoadQuiz(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Quiz>>(json);
        }

        public static bool SaveExist(string filePath)
        {
            return File.Exists(filePath);
        }

    }
}
