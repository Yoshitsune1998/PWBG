using PWBG_BOT.Core.UserAccounts;
using Discord.WebSocket;
using PWBG_BOT.Core.BuffAndDebuff;
using System.Collections.Generic;

namespace PWBG_BOT.Core.Items
{
    public static class ItemTech
    {
        public async static void UseDecreasingHPItem(UserAccount me,Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, item.Value);
            if (user.HP <= 0)
            {
                user.HP = 0;
                UserAccounts.UserAccounts.AddingKills(me, 1);
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {item.Value} DAMAGE!! \nYOU ARE DEAD!!");
                SocketUser realMe = GlobalVar.GuildSelect.GetUser(me.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realMe.Mention} GET 1 KILL");
            }
            else
            {
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {item.Value} DAMAGE!!");
            }
        }

        public async static void UseDecreasingHPItem(UserAccount me, int value, UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, value);
            if (user.HP <= 0)
            {
                user.HP = 0;
                UserAccounts.UserAccounts.AddingKills(me, 1);
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {value} DAMAGE!! \nYOU ARE DEAD!!");
                SocketUser realMe = GlobalVar.GuildSelect.GetUser(me.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realMe.Mention} GET 1 KILL");
            }
            else
            {
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {value} DAMAGE!!");
            }
        }

        public static async void AutoKO(UserAccount me,UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, user.HP);
            UserAccounts.UserAccounts.AddingKills(me, 1);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU ARE DEAD!!");
            SocketUser realMe = GlobalVar.GuildSelect.GetUser(me.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realMe.Mention} GET 1 KILL");
        }

        public static void UseDecreasingHPAOE(UserAccount me,Item item, List<UserAccount> users)
        {
            foreach (var u in users)
            {
                if (u == me) continue;
                UseDecreasingHPItem(me, item, u);
            }
        }

        public static async void InflictDebuff(UserAccount target , Debuff debuff)
        {
            target.Debuffs.Add(debuff);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(target.ID);
            UserAccounts.UserAccounts.SaveAccount();
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU GOT {debuff.Name}");
        }

        public static void UseDecreasingHPAOE(UserAccount me, int value, List<UserAccount> users)
        {
            foreach (var u in users)
            {
                if (u == me) continue;
                UseDecreasingHPItem(me, value, u);
            }
        }

        public async static void UseIncreasingHPItem(Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.IncreasingHealth(user, item.Value);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU HAVE BEEN HEALED BY {item.Value} HP");
        }

        public async static void UseDecreasingPointItem(Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.AddingPoints(user, item.Value);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOUR POINTS HAVE BEEN DECREASED BY {item.Value}");
        }

    }
}
