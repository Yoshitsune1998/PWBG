using System;
using System.Collections.Generic;
using System.Text;
using PWBG_BOT.Core.UserAccounts;
using Newtonsoft.Json;
using System.IO;

namespace PWBG_BOT.Core
{
    public static class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static bool SaveExist(string filePath)
        {
            return File.Exists(filePath);
        }

    }
}
