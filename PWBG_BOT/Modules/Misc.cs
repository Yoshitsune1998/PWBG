using System;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using PWBG_BOT.Core.UserAccounts;
using PWBG_BOT.Modules;
using NReco.ImageGenerator;

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
        


        [Command("stats")]
        public async Task Stats([Remainder]string args = "")
        {
            SocketUser user = null;
            var mentionUser = Context.Message.MentionedUsers.FirstOrDefault();

            user = mentionUser ?? Context.User;

            var account = UserAccounts.GetUserAccount(user);

            await Context.Channel.SendMessageAsync($"{user.Mention} you have {account.XP} XP and {account.Points} Points");
        }

        [Command("mention")]
        public async Task Mention(IGuildUser user)
        {
            var us = (SocketUser)user;
            var account = UserAccounts.GetUserAccount((SocketUser)user);
            await Context.Channel.SendMessageAsync($"{us.Mention} with id {account.ID} mentioned by {Context.User.Mention}");
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
            if (!isHaveThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
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

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        //[Command("sendnudes")]
        //public async Task SendNudes([Remainder]string args = "")
        //{
        //    string html = "<style>\n    h1{\n        color: red;\n    }\n</style>\n<h1>Hello Wordl!</h1>";
        //    var converter = new HtmlToImageConverter
        //    {
        //        Width = 200,
        //        Height = 70
        //    };
        //    var jpgBytes = converter.GenerateImage(html,NReco.ImageGenerator.ImageFormat.Jpeg);
        //    await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), "hello.jpg");
        //}

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

        //    await Context.Channel.SendMessageAsync("", embed: embed);
        //}

             /*
              
             END OF LINE
             
             */

        private bool isHaveThisRole(SocketGuildUser user, string role)
        {
            var res = from r in user.Guild.Roles
                      where r.Name == role
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
