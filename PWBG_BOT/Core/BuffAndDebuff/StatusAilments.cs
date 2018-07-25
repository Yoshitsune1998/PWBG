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
            Debuff target = new Debuff();
            foreach (var u in user.Debuffs)
            {
                if (u.Name.Equals("Burn"))
                {
                    target = u;
                }
            }
            if (target == null) return;
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
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realuser.Mention} get {target.Name}ED, HP become {user.HP}");
            if(die) await GlobalVar.ChannelSelect.SendMessageAsync("YOU DIED!!");
            if (loss) await GlobalVar.ChannelSelect.SendMessageAsync($"{target.Name} HAS BEEN REMOVED");
        }

        public static async void DecreaseDebuffCountDown(UserAccount user, Debuff debuff)
        {
            foreach (var u in user.Debuffs)
            {
                if (u == debuff)
                {
                    if (u.Countdown == -1) return;
                    user.Debuffs.Remove(debuff);
                    await GlobalVar.ChannelSelect.SendMessageAsync($"{u.Name} HAS BEEN REMOVED");
                    UserAccounts.UserAccounts.SaveAccount();
                }
            }
        }

    }
}
