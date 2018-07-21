#region "PACKAGES"
using System;
using System.Linq;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using PWBG_BOT.Core.System;
using PWBG_BOT.Core;
using PWBG_BOT.Core.PlayerInventory;
using PWBG_BOT.Core.Items;
using PWBG_BOT.Core.UserAccounts;

#endregion

namespace PWBG_BOT.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private EmbedBuilder embed;

        #region "STARTER PACK COMMANDS"

        [Command("join")]
        public async Task JoiningBattle()
        {
            if(IsHavingThisRole((SocketGuildUser)Context.User, "Player"))
            {
                return;
            }
            else
            {
                await ChangeRole((Context.User as SocketGuildUser), "Player");
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} joined the battle");
            }
        }

        [Command("out")]
        public async Task OutFromBattle()
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Player"))
            {
                return;
            }
            await RemoveRole((Context.User as SocketGuildUser), "Player");
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} out from the battle");
        }

        [Command("stats")]
        public async Task Stats()
        {
            var user = Context.User;
            var account = UserAccounts.GetUserAccount(user);

            embed = new EmbedBuilder();
            embed.WithTitle(user.Username + "'s Profile");
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
            for (int i = temp; i < 3; i++)
            {
                embed.AddField($"Items-{i+1} : ", "---");
            }
            
            await Context.Channel.SendMessageAsync("", embed: embed);
        }

#endregion

        #region "QUIZ COMAMANDS"

        [Command("quiz")]
        public async Task StartQuiz(ulong id,ulong time=60)
        {
            if (GlobalVar.QuizHasBeenStarted) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Quiz now = Quizzes.GetQuiz(id);
            if (now == null)
            {
                await Context.Channel.SendMessageAsync("No Quiz Found");
                return;
            }
            string formattedText = $"Quiz No-{now.ID} : \nDifficulty : {now.Difficulty}\n";
            formattedText += $"Time : {time} second(s)";
            switch (now.Type)
            {
                case "image":
                    formattedText += $"Type : Image\n";
                    break;
                case "sv":
                    formattedText += $"Type : Shadowverse Pic\n";
                    break;
                case "ost":
                    formattedText += $"Type : OST(OP/ED)\n";
                    break;
                case "bonus":
                    formattedText += $"Type : Bonus\n";
                    break;
                case "voice-sv":
                    formattedText += $"Type : Shadowverse Voice\n";
                    break;
            }
            int lenght = now.Drop.Count;
            int temp = 0;
            for (int i = 0; i < lenght; i++)
            {
                formattedText += $"Drop-{i + 1} : {now.Drop[i].Name}\n";
                temp++;
            }
            for (int i = temp; i < 3; i++)
            {
                formattedText += $"Drop-{i + 1} : --- \n";
            }
            formattedText += $"{now.URL}";

            GlobalVar.Selected = now;
            Quizzes.ResetQuiz();
            await Context.Channel.SendMessageAsync(formattedText);
            await ReapetedTimer.StartQuiz(time, Context.Guild, (SocketTextChannel)Context.Channel);
        }

        [Command("quiz cancel")]
        public async Task StopQuiz()
        {
            if (!GlobalVar.QuizHasBeenStarted) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            ReapetedTimer.StopTimer();
            GlobalVar.Selected = null;
            await Context.Channel.SendMessageAsync("Quiz Has Been Canceled");
        }

        [Command("q")]
        public async Task Answering([Remainder]string answer)
        {
            if (!GlobalVar.QuizHasBeenStarted) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User,"Player")) return;
            ulong id = GlobalVar.Selected.ID;
            UserAccount user = UserAccounts.GetUserAccount(Context.User);
            uint point = Quizzes.CheckAnswer(answer,id);
            Console.WriteLine(point);
            if (point == 0)
            {
                return;
            }
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} {point} point(s)");
            UserAccounts.TempPoints(user,point);
        }

#endregion

        #region "SHOWING COMMANDS"

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
                formattedText += $"Right Answer : {q.RightAnswer}\n";
                formattedText += $"{q.URL}\n\n";
            }
            if (formattedText == "")
            {
                await Context.Channel.SendMessageAsync("No Quiz Has Been Made, Be The First to make One");
                return;
            }
            await Context.Channel.SendMessageAsync(formattedText);
        }

        #endregion

        #region "ADDING COMMANDS"

        [Command("add quiz")]
        public async Task AddingQuiz(string type, string imageUrl, string diff, ulong dropId, [Remainder]string correct)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
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
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Item made = Drops.CreatingItem(name,type,active,value,rarity);
            if (made == null)
            {
                await Context.Channel.SendMessageAsync("Failed to Make Item");
                return;
            }
            await Context.Channel.SendMessageAsync("Item has been made");
        }
        

        [Command("add hint")]
        public async Task AddingHints(ulong id, string url1, string url2, string url3)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Quiz selected = Quizzes.GetQuiz(id);
            if (selected == null)
            {
                await Context.Channel.SendMessageAsync("NO QUIZ FOUND WITH THAT ID");
                return;
            }
            List<string> hints = new List<string>();
            hints.Add(url1);
            hints.Add(url2);
            hints.Add(url3);
            Quizzes.AddingHints(selected,hints);
            await Context.Channel.SendMessageAsync("HINT HAS BEEN ADDED");
        }

        #endregion

        #region "WAREHOUSE"

        //[Command("mention")]
        //public async Task Mention(IGuildUser user)
        //{
        //    var us = (SocketUser)user;
        //    var account = UserAccounts.GetUserAccount((SocketUser)user);
        //    await Context.Channel.SendMessageAsync($"{us.Mention} with id {account.ID} mentioned by {Context.User.Mention}");
        //}
        //[Command("say")]
        //public async Task Say([Remainder]string text)
        //{
        //    string uname = Context.User.Username;
        //    embed = new EmbedBuilder();
        //    embed.WithDescription("Greetings");
        //    embed.WithTitle(Utilities.GetFormattedText("Say_&NAME", uname));

        //    await Context.Channel.SendMessageAsync("",false,embed);
        //}

        //[Command("secret")]
        //public async Task Secret()
        //{
        //    if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
        //        await Context.Channel.SendMessageAsync(Utilities.GetText("Reveal"));
        //}

        //[Command("load")]
        //public async Task Load()
        //{
        //    await Context.Channel.SendMessageAsync("Data has " + MainStorage.GetPairsCount() + " pairs");
        //    foreach (var data in MainStorage.LoadPairs())
        //    {
        //        await Context.Channel.SendMessageAsync($"{data.Key} : {data.Value}");
        //    }
        //}

        //[Command("pm")]
        //public async Task PrivateMessage()
        //{
        //    var dm = await Context.User.GetOrCreateDMChannelAsync();
        //    await dm.SendMessageAsync(Utilities.GetText("pm"));
        //}

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

        #endregion

        #region "UTILITIES"

        private static bool IsHavingThisRole(SocketGuildUser user, string role)
        {
            var res = from r in user.Guild.Roles
                      where r.Name == role
                      select r.Id;
            ulong roleId = res.FirstOrDefault();
            if (roleId == 0) return false;
            var targetRole = user.Guild.GetRole(roleId);
            return user.Roles.Contains(targetRole);
        }

        private static async Task ChangeRole(SocketGuildUser user, string role)
        {
            var res = from r in user.Guild.Roles
                      where r.Name == role
                      select r;
            var des = res.FirstOrDefault();
            await user.AddRoleAsync(des);
        }

        private static async Task RemoveRole(SocketGuildUser user, string role)
        {
            var res = from r in user.Guild.Roles
                      where r.Name == role
                      select r;
            var des = res.FirstOrDefault();
            await user.RemoveRoleAsync(des);
        }

        #endregion

    }
}
