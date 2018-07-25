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
using PWBG_BOT.Core.BuffAndDebuff;
#endregion

namespace PWBG_BOT.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private EmbedBuilder embed;

        #region "STARTER PACK COMMANDS"

        [Command("help")]
        public async Task Help()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("--------------HELP--------------");

            string x = "-join (for joining the quiz)\n\n-out (for going out from the quiz)" +
                "\n\n-stats (for showing your stats)\n\n-q + text (for answering the quiz)" +
                "\n\n-inv show (for showing your inventory more detail then -stats)" +
                "\n\n-use item [1-3] (use item from inventory based on which number you select)" +
                "\n\n-inv drop [1-3] (drop item from inventory based on which number you select)" +
                "\n\n-show items (for showing all items)" +
                "\n\n-find item name (find with that name)";

            embed.WithDescription(x);
            embed.WithColor(52,152,219);

            await Context.Channel.SendMessageAsync("",embed : embed);
        }

        [Command("help secret")]
        public async Task HelpForDeveloper()
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer") 
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager"))return;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("--------------HELP--------------");

            string x = "-join (for joining the quiz)\n\n-out (for going out from the quiz)" +
                "\n\n-stats (for showing your stats)\n\n-q + text (for answering the quiz)" +
                "\n\n-inv show (for showing your inventory more detail then -stats)" +
                "\n\n-use item [1-3] (use item from inventory based on which number you select)" +
                "\n\n-inv drop [1-3] (drop item from inventory based on which number you select)" +
                "\n\n-show items (for showing all items)" +
                "\n\n-find item name (find with that name)" +
                "\n\n-quiz number-quiz number-of-time[opsional, default = 60s] (for showing the quiz based on the number)" +
                "\n\n-quiz cancel (for canceling quiz that has been started)" +
                "\n\n-show quizzes (show all quiz with the hints and right answer)" +
                "\n\n-show players (show all user with Player Role)" +
                "\n\n-add quiz type(image / sv / ost / bonus / voice - sv) url(embedded location like imgur / etc) " +
                "\nurl-fullimage (if nothing then type x)" +
                "\ndiff(ez / med / hard / ext / imm) drop(item number) correctAnswer(the correct answer from your quiz) " +
                "\n[Hint added in different command]" +
                "\n\n-add hint quiznumber(number of quiz that you wanted to insert hint) hint" +
                "\n\n-add item name(item name) type(target / self / random / passive) active(true / false) " +
                "\n\n-add drop quiznumber itemnumber" +
                "\n\nvalue(like ammount of damage or heal) rarity(comm / uncomm / etc) description (description of the item)" +
                "\n\n-give item item-number (adding item to your inventory)";

            embed.WithDescription(x);
            embed.WithColor(52, 152, 219);

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

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
                embed.AddInlineField($"Items-{i+1} : ", account.Inventory.Items[i].Name);
                temp++;
            }
            for (int i = temp; i < 3; i++)
            {
                embed.AddInlineField($"Items-{i+1} : ", "---");
            }
            lenght = account.Buffs.Count;
            temp = 0;
            for (int i = 0; i < lenght; i++)
            {
                embed.AddInlineField($"Buffs-{i + 1} : ", account.Buffs[i].Name);
                temp++;
            }
            for (int i = temp; i < 3; i++)
            {
                embed.AddInlineField($"Buffs-{i + 1} : ", "---");
            }
            lenght = account.Debuffs.Count;
            temp = 0;
            for (int i = 0; i < lenght; i++)
            {
                embed.AddInlineField($"Debuffs-{i + 1} : ", account.Debuffs[i].Name);
                temp++;
            }
            for (int i = temp; i < 3; i++)
            {
                embed.AddInlineField($"Debuffs-{i + 1} : ", "---");
            }

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

#endregion

        #region "QUIZ COMMANDS"

        [Command("quiz", RunMode = RunMode.Async)]
        public async Task StartQuiz(ulong id,ulong time=60)
        {
            if (GlobalVar.QuizHasBeenStarted) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Quiz now = Quizzes.GetQuiz(id);
            if (now == null)
            {
                await Context.Channel.SendMessageAsync("`No Quiz Found`");
                return;
            }
            string formattedText = $"Quiz No-{now.ID} : \nDifficulty : {now.Difficulty}\n";
            formattedText += $"Time : {time} second(s)\n";
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

            GlobalVar.Selected = now;
            Quizzes.ResetQuiz();
            GlobalVar.GuildSelect = Context.Guild;
            GlobalVar.ChannelSelect = (SocketTextChannel)Context.Channel;
            await Context.Channel.SendMessageAsync($"{formattedText}");
            await ReapetedTimer.StartQuiz(time, Context.Guild, (SocketTextChannel)Context.Channel);
        }

        [Command("quiz cancel")]
        public async Task StopQuiz()
        {
            if (!GlobalVar.QuizHasBeenStarted) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            ReapetedTimer.StopTimer();
            GlobalVar.Selected = null;
            await Context.Channel.SendMessageAsync("`Quiz Has Been Canceled`");
        }

        [Command("q")]
        public async Task Answering([Remainder]string answer)
        {
            if (!GlobalVar.QuizHasBeenStarted) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User,"Player")) return;
            if (UserAccounts.IsDead(Context.User)) return;
            UserAccount user = UserAccounts.GetUserAccount(Context.User);
            ulong id = GlobalVar.Selected.ID;
            int point = Quizzes.CheckAnswer(answer,id);
            Console.WriteLine(point);
            if (point == 0)
            {
                return;
            }
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} {point} point(s)");
            UserAccounts.TempPoints(user,point);
        }

        #endregion

        #region "ITEM COMMANDS"

        [Command("inv show")]
        public async Task ShowInventory()
        {
            Inventory inv = Inventories.GetInventory(Context.User);
            string text = "";
            if (inv.Items.Count <= 0)
            {
                await Context.Channel.SendMessageAsync("`You dont have any items`");
                return;
            }
            int i = 0;
            foreach (var item in inv.Items)
            {
                string act = (item.Active) ? "Active" : "Passive";
                text += $"Item-{++i}\nName : {item.Name}\nActive : {act}\nType : {item.Type}\nRarity : {item.Rarity}\n";
                text += "\n";
            }
            await Context.Channel.SendMessageAsync($"`{text}`");
        }

        [Command("use item", RunMode = RunMode.Async)]
        public async void UsingItem(int index, IGuildUser taggedUser = null, int optional = 0)
        {
            if (!GlobalVar.CanUseItem) return;
            if (UserAccounts.CheckHaveThisBuff(UserAccounts.GetUserAccount(Context.User), "Panic")) return;
            if (UserAccounts.IsDead(Context.User)) return;
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Player")) return;
            GlobalVar.GuildSelect = Context.Guild;
            GlobalVar.ChannelSelect = (SocketTextChannel)Context.Channel;
            await Context.Channel.SendMessageAsync("");
        }

        [Command("inv drop")]
        public void DropItemFromInventory(int id)
        {
            Inventories.DropItem(Context.User,id);
        }

        #endregion

        #region "SHOWING AND FINDING COMMANDS"

        [Command("show quizzes")]
        public async Task ShowAllQuiz()
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            string formattedText = "";
            if (Quizzes.GetQuizzes().Count == 0)
            {
                await Context.Channel.SendMessageAsync("`No Quiz Has Been Made, Be The First to make One`");
                return;
            }
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
                if(formattedText.Length + 200 > 2048)
                {
                    await Context.Channel.SendMessageAsync($"{formattedText}");
                    formattedText = "";
                }
            }
            await Context.Channel.SendMessageAsync($"{formattedText}");
        }

        [Command("show players")]
        public async Task ShowAllPlayers()
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            var users = Context.Guild.Users;
            EmbedBuilder embed = new EmbedBuilder();
            string text = "";
            foreach (var u in users)
            {
                if (IsHavingThisRole(u,"Player"))
                {
                    var user = UserAccounts.GetUserAccount((SocketUser)u);
                    text += $"{u.Username}\nHP : {user.HP}\n" +
                        $"POINT : {user.Points}\nKILL : {user.Kills}\n";
                    //item
                    text += "Inventory : \n";
                    int num = 0;
                    for (int i = 0; i < user.Inventory.Items.Count; i++)
                    {
                        text += $"Item-{i + 1}: {user.Inventory.Items[i].Name}\n";
                        num++;
                    }
                    for (int i = num; i < 3; i++)
                    {
                        text += $"Item-{i + 1}: ---\n";
                    }

                    //buff
                    text += "Buffs : \n";
                    num = 0;
                    for (int i = 0; i < user.Buffs.Count; i++)
                    {
                        text += $"Buff-{i + 1}: {user.Buffs[i].Name}\n";
                        num++;
                    }
                    for (int i = num; i < 3; i++)
                    {
                        text += $"Buff-{i + 1}: ---\n";
                    }

                    //debuff
                    text += "Debuffs : \n";
                    num = 0;
                    for (int i = 0; i < user.Debuffs.Count; i++)
                    {
                        text += $"Debuff-{i + 1}: {user.Debuffs[i].Name}\n";
                        num++;
                    }
                    for (int i = num; i < 3; i++)
                    {
                        text += $"Debuff-{i + 1}: ---\n";
                    }
                    text += "\n\n";
                }
                if ((text.Length + 300) > 2048)
                {
                    embed.WithDescription(text);
                    await Context.Channel.SendMessageAsync("", embed: embed);
                    text = "";
                }
            }
            embed.WithDescription(text);
            await Context.Channel.SendMessageAsync("",embed : embed);
        }

        [Command("show items")]
        public async Task ShowAllItems()
        {
            var temp = Drops.GetItems();
            string text = "";
            foreach (var t in temp)
            {
                string act = (t.Active) ? "Active":"Passive" ;
                text += $"Item-{t.ID}\nName : {t.Name}\nUsage : {act}\nType : {t.Type}\nRarity : {t.Rarity}\n";
                text += "\n";
                if ((text.Length + 300) > 2048)
                {
                    await Context.Channel.SendMessageAsync($"`{text}`");
                    text = "";
                }
            }
            if (text.Equals(""))
            {
                await Context.Channel.SendMessageAsync("`NO ITEM HAS BEEN MADE BE THE FIRST TO MAKE ONE!`");
                return;
            }
            await Context.Channel.SendMessageAsync($"`{text}`");
        }

        [Command("find item")]
        public async Task GetItem([Remainder]string name)
        {
            List<Item> find = Drops.WordFind(name);
            Console.WriteLine(find.Count);
            if (find.Count <= 0)
            {
                await Context.Channel.SendMessageAsync($"`NO ITEM FOUND WITH THAT NAME`");
                return;
            }
            foreach (var select in find)
            {
                string text = "";
                string act = (select.Active) ? "Active" : "Passive";
                text += $"Item-{select.ID}\nName : {select.Name}\nUsage : {act}\nType : {select.Type}\nRarity : {select.Rarity}\n";
                foreach (var item in select.Buffs)
                {
                    text += $"Buff : {item.Name}, description : {item.Tech}\n";
                }
                foreach (var item in select.Debuffs)
                {
                    text += $"Debuff : {item.Name}, description : {item.Tech}\n";
                }
                text += $"Description : {select.Description}";
                await Context.Channel.SendMessageAsync($"`{text}`");
            }
        }

        [Command("find buff")]
        public async Task GetBuff([Remainder]string name)
        {
            Buff find = Buffs.GetSpecificBuff(name);
            if (find == null)
            {
                await Context.Channel.SendMessageAsync($"`No Buff Found`");
                return;
            }
            string text = "";
            text += $"Name : {find.Name}\nCountdown : {find.Countdown}\nDescription : {find.Tech}";
            await Context.Channel.SendMessageAsync($"`{text}`");
        }

        [Command("find debuff")]
        public async Task GetDebuff([Remainder]string name)
        {
            Debuff find = Debuffs.GetSpecificDebuff(name);
            if (find == null)
            {
                await Context.Channel.SendMessageAsync($"`No Debuff Found`");
                return;
            }
            string text = "";
            text += $"Name : {find.Name}\nCountdown : {find.Countdown}\nDescription : {find.Tech}";
            await Context.Channel.SendMessageAsync($"`{text}`");
        }

        #endregion

        #region "ADDING COMMANDS"

        [Command("add quiz")]
        public async Task AddingQuiz(string type, string imageUrl, string fullImage,string diff, [Remainder]string correct)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Quiz made = Quizzes.CreatingQuiz(type, imageUrl, fullImage,diff, correct);
            if (made == null)
            {
                await Context.Channel.SendMessageAsync("`Failed to Make Quiz`");
                return;
            }
            await Context.Channel.SendMessageAsync("`Quiz has been made \nDont forget to add the hints`");
        }
        
        [Command("add item")]
        public async Task AddingItem(string name, string type, bool active, int value, string rarity,int countdown ,[Remainder]string description = "")
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Item made = Drops.CreatingItem(name,type,active,value,rarity, countdown,description);
            if (made == null)
            {
                await Context.Channel.SendMessageAsync("`Failed to Make Item`");
                return;
            }
            await Context.Channel.SendMessageAsync("`Item has been made`");
        }
        
        [Command("add item buff")]
        public async Task AddingBuffToItem(ulong id, string buffname)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Item select = Drops.GetSpecificItem(id);
            if (select == null) return;
            Buff buff = Buffs.GetSpecificBuff(buffname);
            if (buff == null) return;
            select.Buffs.Add(buff);
            await Context.Channel.SendMessageAsync("ADDING BUFF SUCCESS");
        }

        [Command("add item debuff")]
        public async Task AddingDebuffToItem(ulong id, string debuffname)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Item select = Drops.GetSpecificItem(id);
            if (select == null) return;
            Debuff debuff = Debuffs.GetSpecificDebuff(debuffname);
            if (debuff == null) return;
            select.Debuffs.Add(debuff);
            await Context.Channel.SendMessageAsync("ADDING BUFF SUCCESS");
        }

        [Command("add drop")]
        public async Task AddingDrops(ulong id, ulong idItem)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Quiz selected = Quizzes.GetQuiz(id);
            if (selected == null)
            {
                await Context.Channel.SendMessageAsync("`NO QUIZ FOUND WITH THAT ID`");
                return;
            }
            if (selected.Drop.Count >= 3)
            {
                await Context.Channel.SendMessageAsync("`CAN'T ADD MORE DROPS`");
                return;
            }
            Item drop = Drops.GetSpecificItem(idItem);
            if(drop == null)
            {
                await Context.Channel.SendMessageAsync("`NO ITEM FOUND WITH THAT ID`");
                return;
            }
            Quizzes.AddingDrops(selected,drop);
            await Context.Channel.SendMessageAsync("`DROP HAS BEEN ADDED`");
        }

        [Command("add hint")]
        public async Task AddingHints(ulong id, [Remainder]string url1)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Quiz selected = Quizzes.GetQuiz(id);
            if (selected == null)
            {
                await Context.Channel.SendMessageAsync("`NO QUIZ FOUND WITH THAT ID`");
                return;
            }
            if (selected.Hints.Count >= 3)
            {
                await Context.Channel.SendMessageAsync("`CAN'T ADD MORE HINT`");
                return;
            }
            
            Quizzes.AddingHints(selected,url1);
            await Context.Channel.SendMessageAsync("`HINT HAS BEEN ADDED`");
        }

        [Command("add buff")]
        public async Task AddingBuffs(string name, int value, int countdown, [Remainder]string desc)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Buff buff = Buffs.CreatingBuff(name,desc,value,countdown);
            if(buff == null)
            {
                await Context.Channel.SendMessageAsync("FAILED TO MAKE BUFF");
                return;
            }
            await Context.Channel.SendMessageAsync("BUFF HAS BEEN MADE");
        }

        [Command("add debuff")]
        public async Task AddingDebuffs(string name, int value, int countdown, [Remainder]string desc)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Debuff debuff = Debuffs.CreatingDebuff(name, desc, value, countdown);
            if (debuff == null)
            {
                await Context.Channel.SendMessageAsync("FAILED TO MAKE DEBUFF");
                return;
            }
            await Context.Channel.SendMessageAsync("DEBUFF HAS BEEN MADE");
        }

        #endregion

        #region "GOD COMMANDS"

        [Command("give me")]
        public async Task GiveItem(ulong index, IGuildUser guildUser=null)
        {
            if (!IsHavingThisRole((SocketGuildUser)Context.User, "Developer")
                && !IsHavingThisRole((SocketGuildUser)Context.User, "Quiz Manager")) return;
            Item item = Drops.GetSpecificItem(index);
            if (guildUser!=null)
            {
                await Inventories.GiveItem((SocketUser)guildUser, item, (SocketTextChannel)Context.Channel);
                return;
            }
            await Inventories.GiveItem(Context.User, item, (SocketTextChannel)Context.Channel);
        }

        [Command("heal me")]
        public async Task Heal(IGuildUser guildUser = null)
        {
            UserAccount me = new UserAccount();
            if (guildUser != null)
            {
                me = UserAccounts.GetUserAccount((SocketUser)guildUser);
            }
            else
            {
                me = UserAccounts.GetUserAccount(Context.User);
            }
            UserAccounts.IncreasingHealth(me,15);
            await Context.Channel.SendMessageAsync("YOU HAVE BEEN HEALED");
        }

        #endregion  

        //end of line

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
