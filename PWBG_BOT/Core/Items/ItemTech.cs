using PWBG_BOT.Core.UserAccounts;
using Discord.WebSocket;
using System;

namespace PWBG_BOT.Core.Items
{
    public static class ItemTech
    {
        //ANY KIND OF ITEM USED SKILL OR WHATEVER
        public async static void UseDecreasingHPItem(Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.DecreasingHealth(user, item.Value);
            
            if (user.HP <= 0)
            {
                user.HP = 0;
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {item.Value} DAMAGE!! \nYOU ARE DEAD!!");
            }
        }

        public async static void UseIncreasingHPItem(Item item, UserAccount user)
        {
            UserAccounts.UserAccounts.IncreasingHealth(user, item.Value);
            SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} YOU HAVE BEEN HEALED BY {item.Value} HP");
        }

        public static void UseItem()
        {
            //
        }

        //
        #region "EXAMPLE"
        //EXAMPLE OF SPECIAL ITEM
        public static void UseCursedGrail()
        {
            //USE THIS ITEM WILL CHANGE EVERYONE HP TO 1
        }
        #endregion

    }
}
