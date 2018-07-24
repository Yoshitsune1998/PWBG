using System;
using System.Collections.Generic;
using Discord.WebSocket;
using System.Linq;
using PWBG_BOT.Core.PlayerInventory;
using PWBG_BOT.Core.BuffAndDebuff;

namespace PWBG_BOT.Core.UserAccounts
{
    public static class UserAccounts
    {
        //DONT FORGET TO SAVE FILE BY USING UserAccounts.SaveAccount(); AFTER ADDING EXP OR SOMETHING OTHERWISE ITS NOT GONNA BE CHANGED

        private static List<UserAccount> accounts;

        private static string accountsFile = "Resources/accounts.json";

        static UserAccounts()
        {
            if (DataStorage.SaveExist(accountsFile))
            {
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccount();
            }
        }

        public static void AddingPoints(UserAccount user, int point)
        {
            user.Points += point;
            SaveAccount();
        }

        public static void AddAllPoints(UserAccount user)
        {
            user.Points += user.TempPoint;
            SaveAccount();
        }

        public static void ResetTempPoint(UserAccount user)
        {
            user.TempPoint = 0;
            SaveAccount();
        }

        public static void TempPoints(UserAccount user, int point)
        {
            if (point <= user.TempPoint) return;
            user.TempPoint = point;
            SaveAccount();
        }

        public static void DecreasingPoints(UserAccount user, int point)
        {
            user.Points -= point;
            SaveAccount();
        }

        public static void AddingKills(UserAccount user, uint kill)
        {
            user.Kills += kill;
            SaveAccount();
        }

        public static void IncreasingHealth(UserAccount user, int ammount)
        {
            user.HP += ammount;
            SaveAccount();
        }

        public static void DecreasingHealth(UserAccount user, int ammount)
        {
            user.HP -= ammount;
            SaveAccount();
        }

        public static UserAccount GetUserAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }
        
        public static UserAccount GetUserAccountByID(ulong id)
        {
            var result = from a in accounts
                         where a.ID == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) return null;
            return account;
        }
        
        public static List<UserAccount> GetAllUsers()
        {
            return accounts;
        }

        public static Inventory GetInventory(SocketUser user)
        {
            return Inventories.GetOrCreateInventory(user.Id);
        }

        public static void SaveAccount()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.ID == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                ID = id,    
                Points = 0,
                Buffs = new List<Buff>(),
                Debuffs = new List<Debuff>(),
                HP = 15,
                Inventory = Inventories.GetOrCreateInventory(id),
                Kills = 0
            };
            accounts.Add(newAccount);
            SaveAccount();
            return newAccount;
        }
        
        public static UserAccount GetRandomPlayer(SocketGuild guild)
        {
            var users = guild.Users;
            List<UserAccount> randomPlayers = new List<UserAccount>();
            var role = from r in guild.Roles
                       where r.Name.Equals("Player")
                       select r;
            var des = role.FirstOrDefault();
            foreach (var u in users)
            {
                if (u.Roles.Contains(des))
                {
                    var user = UserAccounts.GetUserAccount((SocketUser)u);
                    randomPlayers.Add(user);
                }
            }
            Random gacha = new Random();
            if (randomPlayers.Count == 0) return null;
            int luckyIndex = (int)gacha.Next(0,randomPlayers.Count);
            Console.WriteLine(luckyIndex);
            return randomPlayers[luckyIndex];
        }

        public static bool IsDead(SocketUser user)
        {
            UserAccount account = GetUserAccount(user);
            if (account.HP <= 0) return true;
            return false;
        }

    }
}
