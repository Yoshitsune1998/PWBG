using System;
using System.Collections.Generic;
using Discord.WebSocket;
using System.Linq;
using PWBG_BOT.Core.SurvivorInventory;
using PWBG_BOT.Core.BuffAndDebuff;
using PWBG_BOT.Core.Items;
using System.Threading.Tasks;

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

        public static async void DecreasingHealth(UserAccount user, int ammount)
        {
            if (Inventories.CheckHaveThisItem(user, "Chainmail") && user.HP - ammount <= 0)
            {
                if (CheckHaveThisBuff(user, "Reversality"))
                {
                    ItemTech.haveReversal = true;
                    IncreasingHealth(user, ammount);
                    user.Buffs.Remove(Buffs.GetSpecificBuff("Reversality"));
                    await GlobalVar.ChannelSelect.SendMessageAsync($"{user.Name} REVERSALITY BUFF HAS BEEN REMOVED");
                    return;
                }
                user.HP = 1;
                Item getto = Drops.GetSpecificItem("Chainmail");
                SocketUser realuser = GlobalVar.GuildSelect.GetUser(user.ID);
                await Inventories.DropAnyItem(realuser, getto);
            }
            else if (Inventories.CheckHaveThisItem(user, "Bulletproof Vest"))
            {
                Console.WriteLine("masuk");
                ammount = (ammount / 2) + 1;
                ItemTech.damageOutcoume = ammount;
                bool temp = false;
                if (CheckHaveThisBuff(user, "Reversality"))
                {
                    ItemTech.haveReversal = true;
                    IncreasingHealth(user, ammount);
                    user.Buffs.Remove(Buffs.GetSpecificBuff("Reversality"));
                    await GlobalVar.ChannelSelect.SendMessageAsync($"{user.Name} REVERSALITY BUFF HAS BEEN REMOVED");
                    temp = !temp;
                }
                Item getto = Drops.GetSpecificItem("Bulletproof Vest");
                SocketUser realuser = GlobalVar.GuildSelect.GetUser(user.ID);
                await Inventories.DropAnyItem(realuser, getto);
                if (temp) return;
                user.HP -= ammount;
            }
            else
            {
                if (CheckHaveThisBuff(user, "Reversality"))
                {
                    ItemTech.haveReversal = true;
                    IncreasingHealth(user, ammount);
                    user.Buffs.Remove(Buffs.GetSpecificBuff("Reversality"));
                    await GlobalVar.ChannelSelect.SendMessageAsync($"{user.Name} REVERSALITY BUFF HAS BEEN REMOVED");
                    return;
                }
                user.HP -= ammount;
            }
            if (user.HP < 0) user.HP = 0;
            SaveAccount();
        }

        public static bool CheckHaveThisBuff(UserAccount user, string name)
        {
            if (user.Buffs.Count <= 0) return false;
            foreach (var b in user.Buffs)
            {
                if (b.Name.Equals(name)) return true;
            }
            return false;
        }

        public static bool CheckHaveThisDebuff(UserAccount user, string name)
        {
            if (user.Debuffs.Count <= 0) return false;
            foreach (var b in user.Debuffs)
            {
                if (b.Name.Equals(name)) return true;
            }
            return false;
        }

        public static UserAccount GetUserAccount(SocketUser user)
        {
            return GetOrCreateAccount(user);
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

        public static List<UserAccount> GetAllAliveUsers()
        {
            List<UserAccount> alive = new List<UserAccount>();
            foreach (var a in accounts)
            {
                if (a.HP <= 0) continue;
                alive.Add(a);
            }
            return alive;
        }

        public static Inventory GetInventory(SocketUser user)
        {
            return Inventories.GetOrCreateInventory(user.Id);
        }

        public static void SaveAccount()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        private static UserAccount GetOrCreateAccount(SocketUser user)
        {
            ulong id = user.Id;
            string name = user.Username;
            var result = from a in accounts
                         where a.ID == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id,name);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id,string name)
        {
            var newAccount = new UserAccount()
            {
                ID = id,    
                Points = 0,
                Buffs = new List<Buff>(),
                Debuffs = new List<Debuff>(),
                HP = 15,
                Inventory = Inventories.GetOrCreateInventory(id),
                Kills = 0,
                Name = name
            };
            accounts.Add(newAccount);
            SaveAccount();
            return newAccount;
        }
        
        public static UserAccount GetRandomSurvivor(SocketGuild guild)
        {
            var users = guild.Users;
            List<UserAccount> randomSurvivors = new List<UserAccount>();
            var role = from r in guild.Roles
                       where r.Name.Equals("Survivor")
                       select r;
            var des = role.FirstOrDefault();
            foreach (var u in users)
            {
                if (u.Roles.Contains(des))
                {
                    var user = UserAccounts.GetUserAccount((SocketUser)u);
                    if (user.HP <= 0) continue;
                    randomSurvivors.Add(user);
                }
            }
            Random gacha = new Random();
            if (randomSurvivors.Count == 0) return null;
            int luckyIndex = (int)gacha.Next(0,randomSurvivors.Count);
            return randomSurvivors[luckyIndex];
        }

        public static Buff GetRandomBuff(UserAccount target)
        {
            if (target.Buffs.Count == 1) return target.Buffs[0];
            Random rand = new Random();
            int luckyBuff = rand.Next(0,target.Buffs.Count);
            return target.Buffs[luckyBuff];
        }

        public static async void GiveBuff(UserAccount user, Item item, SocketTextChannel channel)
        {
            foreach (var b in item.Buffs)
            {
                if (user.Buffs.Count >= 3) return;
                user.Buffs.Add(b);
                await channel.SendMessageAsync($"YOU GOT {b.Name} BUFF");
                SaveAccount();
            }
        }

        public static UserAccount GetRandomBesideMe(UserAccount me)
        {
            UserAccount target;
            if (accounts.Count<=0)
            {
                return null;
            }
            do
            { target = GetRandomSurvivor(GlobalVar.GuildSelect); }
            while (me == target);
            return target;
        }

        public static bool IsDead(SocketUser user)
        {
            UserAccount account = GetUserAccount(user);
            if (account.HP <= 0) return true;
            return false;
        }

        public static async Task StatusAilment(UserAccount user)
        {
            if (user.Debuffs.Count > 0) {
                for (int i = user.Debuffs.Count - 1; i >= 0; i--)
                {
                    switch (user.Debuffs[i].Name)
                    {
                        case "Burn":
                            await StatusAilments.Burn(user, user.Debuffs.ElementAt(i));
                            break;
                        default:
                            await StatusAilments.DecreaseDebuffCountDown(user, user.Debuffs.ElementAt(i));
                            break;
                    }
                }
            }
            if (user.Buffs.Count > 0)
            {
                for (int i = user.Buffs.Count - 1; i >= 0; i--)
                {
                    switch (user.Buffs[i].Name)
                    {
                        default:
                            await StatusAilments.DecreaseBuffCountDown(user, user.Buffs.ElementAt(i));
                            break;
                    }
                }
            }
        }

    }
}
