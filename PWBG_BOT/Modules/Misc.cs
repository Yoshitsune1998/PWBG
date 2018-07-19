using System;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using PWBG_BOT.Core.UserAccounts;
using PWBG_BOT.Modules;

namespace PWBG_BOT.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private EmbedBuilder embed;
        
        //Command Lines

        [Command("test")]
        public async Task Test()
        {
            await Context.Channel.SendMessageAsync(Utilities.GetText("Test"));
        }
        [Command("say")]
        public async Task Say([Remainder]string text)
        {
            string uname = Context.User.Username;
            embed = embedBuilder(text);
            embed.WithTitle(Utilities.GetFormattedText("Say_&NAME", uname));

            await Context.Channel.SendMessageAsync("",false,embed);
        }

        [Command("secret")]
        public async Task Secret()
        {
            if (!isQuizManager((SocketGuildUser)Context.User)) return;
                await Context.Channel.SendMessageAsync(Utilities.GetText("Reveal"));
        }

        [Command("load")]
        public async Task Load()
        {
            await Context.Channel.SendMessageAsync("Data has "+DataStorage.GetPairsCount() + " pairs");
            DataStorage.SavePairs("Count "+ DataStorage.GetPairsCount(), "Counter " + DataStorage.GetPairsCount());
        }

        [Command("pm")]
        public async Task PrivateMessage()
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync(Utilities.GetText("pm"));
        }

        [Command("myProfile")]
        public async Task MyProfile()
        {
            var account = UserAccounts.GetUserAccount(Context.User);
            embed = embedBuilder("XP : "+account.XP+"\nPoints : "+account.Points);
            embed.WithTitle(Context.User.Username + "'s Profile");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        //[Command("waifu")]
        //public async Task Waifu([Remainder]string text)
        //{
        //    string[] options = text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        //    Random r = new Random();
        //    string opt = options[r.Next(0, options.Length)];
        //    embed = embedBuilder(opt);
        //    embed.WithTitle("Waifu from user " + Context.User.Username);
        //    string url = HTMLGenerator.GetImageUrl(opt);
        //    embed.WithThumbnailUrl(url);

        //    await Context.Channel.SendMessageAsync("", false, embed);
        //}

        private bool isQuizManager(SocketGuildUser user)
        {
            string RoleName = "Quiz Manager";
            var res = from r in user.Guild.Roles
                      where r.Name == RoleName
                      select r.Id;
            ulong roleId = res.FirstOrDefault();
            if (roleId == 0) return false;
            var targetRole = user.Guild.GetRole(roleId);
            return user.Roles.Contains(targetRole);
        }

        public EmbedBuilder embedBuilder(string text)
        {
            var embed = new EmbedBuilder();
            
            embed.WithCurrentTimestamp();
            embed.WithDescription(text);
            embed.WithColor(0, 255, 0);

            return embed;
        }
        
    }
}
