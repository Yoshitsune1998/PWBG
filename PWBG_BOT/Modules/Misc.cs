using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using PWBG_BOT.Modules;

namespace PWBG_BOT.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private static HTMLGenerator gen = new HTMLGenerator();
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

        //[Command("waifu")]
        //public async Task Waifu([Remainder]string text)
        //{
        //    string[] options = text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        //    Random r = new Random();
        //    string opt =  options[r.Next(0,options.Length)];
        //    embed = embedBuilder(opt);
        //    embed.WithTitle("Waifu from user "+Context.User.Username);
        //    string url = gen.GetImageUrl(opt);
        //    embed.WithThumbnailUrl(url);

        //    await Context.Channel.SendMessageAsync("", false, embed);
        //}




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
