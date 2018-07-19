﻿using System;
using System.Collections.Generic;
using Discord.WebSocket;
using System.Linq;

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

        public static UserAccount GetUserAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        public static void SaveAccount()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static UserAccount GetOrCreateAccount(ulong id)
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
                Points = 10
            };
            accounts.Add(newAccount);
            SaveAccount();
            return newAccount;
        }

    }
}