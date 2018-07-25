using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using PWBG_BOT.Core.UserAccounts;

namespace PWBG_BOT.Core.BuffAndDebuff
{
    class StatusAilments
    {
        public static async void Burn(UserAccount user, Debuff burn)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user,burn.Value);
            bool loss = false;
            bool die = false;
            burn.Countdown--;
            if (burn.Countdown <= 0)
            {
                user.Debuffs.Remove(burn);
                loss = !loss;
            }
            var realuser = GlobalVar.GuildSelect.GetUser(user.ID);
            if (user.HP <= 0) {
                die = !die;
                loss = true;
            } 
            UserAccounts.UserAccounts.SaveAccount();
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realuser.Mention} get BURNED, HP become {user.HP}");
            if(die) await GlobalVar.ChannelSelect.SendMessageAsync("YOU DIED!!");
            if (loss) await GlobalVar.ChannelSelect.SendMessageAsync("BURN HAS BEEN REMOVED");
        }

    }
}
