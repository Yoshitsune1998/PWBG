using PWBG_BOT.Core.UserAccounts;
using Discord.WebSocket;

namespace PWBG_BOT.Core.Items
{
    public static class ItemTech
    {
        //ANY KIND OF ITEM USED SKILL OR WHATEVER
        public async static void UseDecreasingHPItem(Item item, UserAccount user)
        {
            user.HP -= item.Value;
            if (user.HP == 0)
            {
                SocketUser realUser = GlobalVar.GuildSelect.GetUser(user.ID);
                await GlobalVar.ChannelSelect.SendMessageAsync($"{realUser.Mention} GET {item.Value} DAMAGE!! \nYOU ARE DEAD!!");
            }
            UserAccounts.UserAccounts.SaveAccount();
        }

        public async static void UseIncreasingHPItem(Item item, UserAccount user)
        {
            user.HP += item.Value;
            UserAccounts.UserAccounts.SaveAccount();
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
