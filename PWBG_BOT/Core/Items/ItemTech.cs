using PWBG_BOT.Core.UserAccounts;
using Discord.WebSocket;
using PWBG_BOT.Core.BuffAndDebuff;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PWBG_BOT.Core.Items
{
    public static class ItemTech
    {

        private static bool tagProhibited = true;

        public async static Task UseDecreasingHPItem(UserAccount me,Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, item.Value);
            if (user.HP <= 0)
            {
                UserAccounts.UserAccounts.AddingKills(me, 1);
                if (tagProhibited)
                {
                    SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                    await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {item.Value} DAMAGE!! \nYOU ARE DEAD!!");
                }
                SocketUser realMe = GlobalVar.GuildSelect.GetUser(me.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realMe.Mention} GET 1 KILL");
            }
            else
            {
                if (!tagProhibited) return;
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {item.Value} DAMAGE!!");
            }
        }

        public async static Task UseDecreasingHPItem(UserAccount me, int value, UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, value);
            if (user.HP <= 0)
            {
                UserAccounts.UserAccounts.AddingKills(me, 1);
                if (tagProhibited)
                {
                    SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                    await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {value} DAMAGE!! \nYOU ARE DEAD!!");
                }
                SocketUser realMe = GlobalVar.GuildSelect.GetUser(me.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realMe.Mention} GET 1 KILL");
            }
            else
            {
                if (!tagProhibited) return;
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {value} DAMAGE!!");
            }
        }

        public static async Task AutoKO(UserAccount me,UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, user.HP);
            UserAccounts.UserAccounts.AddingKills(me, 1);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU ARE DEAD!!");
            SocketUser realMe = GlobalVar.GuildSelect.GetUser(me.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realMe.Mention} GET 1 KILL");
        }

        public static async Task UseDecreasingHPAOE(UserAccount me,Item item, List<UserAccount> users)
        {
            tagProhibited = false;
            foreach (var u in users)
            {
                if (u == me) continue;
                await UseDecreasingHPItem(me, item, u);
            }
            tagProhibited = true;
            await GlobalVar.ChannelSelect.SendMessageAsync($"Everyone got {item.Value} Damage");
        }

        public static async Task InflictDebuff(UserAccount target , Debuff debuff)
        {
            target.Debuffs.Add(debuff);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(target.ID);
            UserAccounts.UserAccounts.SaveAccount();
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU GOT {debuff.Name}");
        }

        public static async Task UseDecreasingHPAOE(UserAccount me, int value, List<UserAccount> users)
        {
            tagProhibited = false;
            foreach (var u in users)
            {
                if (u == me) continue;
                await UseDecreasingHPItem(me, value, u);
            }
            tagProhibited = true;
            await GlobalVar.ChannelSelect.SendMessageAsync($"Everyone got {value} Damage");
        }

        public async static Task UseIncreasingHPItem(Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.IncreasingHealth(user, item.Value);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU HAVE BEEN HEALED BY {item.Value} HP");
        }

        public async static Task UseDecreasingPointItem(Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.AddingPoints(user, item.Value);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOUR POINTS HAVE BEEN DECREASED BY {item.Value}");
        }

        public async static Task RemoveRandomTargetBuff(UserAccount user)
        {
            Buff remove = UserAccounts.UserAccounts.GetRandomBuff(user);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} BUFF {remove.Name} HAS BEEN REMOVED");
            user.Buffs.Remove(remove);
            UserAccounts.UserAccounts.SaveAccount();
        }

        public async static Task ArmletOfGreed(UserAccount user,Item item)
        {
            var realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} {item.Name} ACTIVATE");
            UserAccounts.UserAccounts.AddingPoints(user,item.Value);
            await GlobalVar.ChannelSelect.SendMessageAsync("YOU GOT 2 POINT");
            UserAccounts.UserAccounts.DecreasingHealth(user,item.Value);
            await GlobalVar.ChannelSelect.SendMessageAsync("YOU GOT 2 DAMAGE");
            if (user.HP <= 0)
            {
                await GlobalVar.ChannelSelect.SendMessageAsync("YOU DIED!!!");
            }
        }

    }
}
