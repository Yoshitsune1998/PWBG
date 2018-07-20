using System;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using PWBG_BOT.Core.System;
using PWBG_BOT.Core.PlayerInventory;
using PWBG_BOT.Core.Items;
using PWBG_BOT.Core.UserAccounts;
using NReco.ImageGenerator;

namespace PWBG_BOT.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private EmbedBuilder embed;
        
        /* STARTER PACK COMMAND */

        [Command("stats")]
        public async Task Stats()
        {
            var user = Context.User;
            var account = UserAccounts.GetUserAccount(user);

            embed = new EmbedBuilder();
            embed.WithTitle(Context.User.Username + "'s Profile");
            embed.WithColor(0, 0, 255);
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.AddInlineField("HP : " , account.HP);
            embed.AddInlineField("Points : ", account.Points);
            embed.AddInlineField("Kills : ", account.Kills);
            int lenght = account.Inventory.Items.Count;
            int temp = 0;
            embed.AddField("Inventory : ", "Items List(1-3) : ");
            for (int i = 0; i < lenght; i++)
            {
                embed.AddField($"Items-{i+1} : ", account.Inventory.Items[i].Name);
                temp++;
            }
            for (int i = temp; i < 3-lenght; i++)
            {
                embed.AddField($"Items-{i+1} : ", "---");
            }

            await Context.Channel.SendMessageAsync("", embed: embed);
        }


        /* SHOWING COMMAND */

        [Command("show quizzes")]
        public async Task ShowAllQuiz()
        {
            string formattedText = "";
            foreach (var q in Quizzes.GetQuizzes())
            {
                formattedText += $"Quiz No-{q.ID}:\nDifficulty:{q.Difficulty}\n";
                switch (q.Type)
                {
                    case "image":
                        formattedText += $"Type:Image\n";
                        break;
                    case "sv":
                        formattedText += $"Type:Shadowverse Pic\n";
                        break;
                    case "ost":
                        formattedText += $"Type:OST(OP/ED)\n";
                        break;
                    case "bonus":
                        formattedText += $"Type:Bonus\n";
                        break;
                    case "voice-sv":
                        formattedText += $"Type:Shadowverse Voice\n";
                        break;
                }
                formattedText += $"{q.URL}\n\n";
            }
            if (formattedText == "")
            {
                await Context.Channel.SendMessageAsync("No Quiz Has Been Made, Be The First to make One");
                return;
            }
            await Context.Channel.SendMessageAsync(formattedText);
        }


        /* ADDING COMMAND */

        [Command("add quiz")]
        public async Task AddingQuiz(string type, string imageUrl, string diff, ulong dropId, [Remainder]string correct)
        {
            Quiz made = Quizzes.CreatingQuiz(type, imageUrl, diff, dropId, correct);
            if (made == null)
            {
                await Context.Channel.SendMessageAsync("Failed to Make Quiz");
                return;
            }
            await Context.Channel.SendMessageAsync("Quiz has been made \nDont forget to add the hints");
        }

        [Command("add item")]
        public async Task AddingItem(string name, string type, bool active, uint value, string rarity)
        {
            Item made = Drops.CreatingItem(name,type,active,value,rarity);
            if (made == null)
            {
                await Context.Channel.SendMessageAsync("Failed to Make Item");
                return;
            }
            await Context.Channel.SendMessageAsync("Item has been made");
        }

        [Command("quiz")]
        public async Task StartQuiz(ulong id)
        {
            Quiz now = Quizzes.GetQuiz(id);
            if (now == null)
            {
                await Context.Channel.SendMessageAsync("No Quiz Found");
                return;
            }
            string formattedText = $"Quiz No-{now.ID}:\nDifficulty:{now.Difficulty}\n";
            switch (now.Type)
            {
                case "image":
                    formattedText += $"Type:Image\n";
                    break;
                case "sv":
                    formattedText += $"Type:Shadowverse Pic\n";
                    break;
                case "ost":
                    formattedText += $"Type:OST(OP/ED)\n";
                    break;
                case "bonus":
                    formattedText += $"Type:Bonus\n";
                    break;
                case "voice-sv":
                    formattedText += $"Type:Shadowverse Voice\n";
                    break;
            }
            int lenght = now.Drop.Count;
            int temp = 0;
            for (int i = 0; i < lenght; i++)
            {
                formattedText+=$"Drop-{i + 1} : {now.Drop[i].Name}\n";
                temp++;
            }
            for (int i = temp; i < 4 - lenght; i++)
            {
                formattedText += $"Drop-{i + 1} : --- \n";
            }
            formattedText += $"{now.URL}";
            await Context.Channel.SendMessageAsync(formattedText);
        }

        [Command("add hint")]
        public async Task AddingHints(ulong id, string url1, string url2, string url3)
        {

        }







        [Command("truth")]
        public async Task FindingTruth(bool x)
        {
            Console.WriteLine(x);
            if (x)
            {
                await Context.Channel.SendMessageAsync("LU GAY");
            }
            else
            {
                await Context.Channel.SendMessageAsync("TETEP GAY");
            }
            
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
            await Context.Channel.SendMessageAsync("Data has " + MainStorage.GetPairsCount() + " pairs");
            foreach (var data in MainStorage.LoadPairs())
            {
                await Context.Channel.SendMessageAsync($"{data.Key} : {data.Value}");
            }
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
            embed = embedBuilder("HP : " + account.HP + "\nKills "+account.Kills+"\nPoints : " + account.Points);
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
