using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using PWBG_BOT.Core.System;
using PWBG_BOT.Core.UserAccounts;
using Discord.WebSocket;

namespace PWBG_BOT.Core
{
    public static class ReapetedTimer
    {
        private static SocketTextChannel channel;

        private static List<Timer> timers = new List<Timer>();

        private static List<string> hints = new List<string>();

        //1st timer
        public static async Task StartQuiz(ulong time, SocketGuild e, SocketTextChannel f)
        {
            channel = f;
            GlobalVar.QuizGuild = e;

            await StrategyTime();
            await Task.Delay(30000);
            //
            await StartTimer();
            await Task.Delay(15000);

            var loopTimer = new Timer()
            {
                Interval = (time * 1000) + 5000,
                AutoReset = false,
                Enabled = true,
            };
            timers.Add(loopTimer);
            loopTimer.Elapsed += QuizEnded;
            await HintCounter(loopTimer.Interval);
        }

        //2nd timer
        public static Task StartTimer()
        {
            var loopTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            timers.Add(loopTimer);
            loopTimer.Elapsed += OnTimerTicked;

            return Task.CompletedTask;
        }

        //3rd timer
        public static Task HintCounter(double QuizTimer)
        {
            var loopTimer = new Timer()
            {
                Interval = ((QuizTimer - 15000) / 3) - 2500 ,
                AutoReset = true,
                Enabled = true
            };
            timers.Add(loopTimer);
            loopTimer.Elapsed += OnHintTicking;

            return Task.CompletedTask;
        }

        //4th timer
        public static async Task StrategyTime()
        {
            GlobalVar.CanUseItem = true;
            Console.WriteLine(GlobalVar.CanUseItem);
            var loopTimer = new Timer()
            {
                Interval = 30000,
                AutoReset = false,
                Enabled = true
            };
            timers.Add(loopTimer);
            loopTimer.Elapsed += StrategyEnded;

            await channel.SendMessageAsync("STRATEGY TIME (30 secs)");

        }

        private static async void QuizEnded(object sender, ElapsedEventArgs e)
        {
            var guild = GlobalVar.QuizGuild;
            var users = guild.Users;
            int highscores = 0;
            List<SocketUser> winner = new List<SocketUser>();
            var role = from r in guild.Roles
                       where r.Name.Equals("Survivor")
                       select r;
            var des = role.FirstOrDefault();
            List<UserAccount> pacito = UserAccounts.UserAccounts.GetAllAliveUsers();
            await Task.Delay(3000);

            await channel.SendMessageAsync($"The right answer is {GlobalVar.Selected.RightAnswer}");
            await Task.Delay(1000);

            foreach (var p in pacito)
            {
                UserAccounts.UserAccounts.StatusAilment(p);
                var realuser = GlobalVar.QuizGuild.GetUser(p.ID);
                SurvivorInventory.Inventories.CountDownItem(realuser);
            }
            string text="";
            foreach (var u in users)
            {
                var temp = (SocketGuildUser)u;
                if (temp.Roles.Contains(des))
                {
                    var user = UserAccounts.UserAccounts.GetUserAccount(temp);
                    UserAccounts.UserAccounts.AddAllPoints(user);
                    text += $"{temp.Mention}\nHP : {user.HP}\n" +
                        $"POINT : {user.Points}\nKILL : {user.Kills}\n";
                    //item
                    text += "Inventory : \n";
                    int num = 0;
                    for (int i = 0; i < user.Inventory.Items.Count; i++)
                    {
                        text += $"Item-{i+1}: {user.Inventory.Items[i].Name}\n";
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
                    if(user.TempPoint!=0)
                    {
                        Console.WriteLine($"point : {user.TempPoint}");
                        
                        if (winner.Count == 0)
                        {
                            highscores = user.TempPoint;
                            winner.Add(temp);
                        }
                        else
                        {
                            var despacito = UserAccounts.UserAccounts.GetUserAccount(winner[winner.Count - 1]);
                            Console.WriteLine($"temp-1 : {user.TempPoint} ; temp-2 : {highscores}");
                            if (user.TempPoint > highscores)
                            {
                                highscores = user.TempPoint;
                                winner.Remove(winner[winner.Count - 1]);
                                winner.Add(temp);
                            }
                            else if (user.TempPoint == highscores)
                            {
                                highscores = user.TempPoint;
                                winner.Add(temp);
                            }
                        }
                    }
                    UserAccounts.UserAccounts.ResetTempPoint(user);
                    if ((text.Length + 300) > 2048)
                    {
                        await channel.SendMessageAsync(text);
                        text = "";
                    }
                }
            }
            timers.Clear();
            await Task.Delay(1000);
            await channel.SendMessageAsync(text);
            await Task.Delay(1000);
            await Quizzes.GiveDrops(winner, channel);
            GlobalVar.QuizHasBeenStarted = false;
            GlobalVar.CanUseItem = false;
            GlobalVar.Selected = null;
            GlobalVar.QuizGuild = null;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            GlobalVar.Channeling++;
            string text = "";
            if (GlobalVar.Channeling == 1)
            {
                text += "READY!";
            }else if (GlobalVar.Channeling == 2)
            {
                text += "SET!!";
            }
            else if (GlobalVar.Channeling == 3)
            {
                text += "GOOO!!!";
                timers[0].AutoReset = false;
                timers[0].Enabled = false;
                timers.Remove(timers[0]);
                GlobalVar.Channeling = 0;
                GlobalVar.QuizHasBeenStarted = true;
                text += $"\n{GlobalVar.Selected.URL}";
            }
            await channel.SendMessageAsync(text);
        }

        private static async void OnHintTicking(object sender, ElapsedEventArgs e)
        {
            GlobalVar.Channeling++;
            string text = "";

            if (hints.Count > GlobalVar.Channeling - 1)
            {
                text += $"HINT-{GlobalVar.Channeling}\n";
            }
             
            if (GlobalVar.Channeling == 1)
            {
                hints = GlobalVar.Selected.Hints;
                if (hints.Count > GlobalVar.Channeling - 1)
                {
                    text += hints[GlobalVar.Channeling - 1];
                }
                else
                {
                    text += "";
                }
            }
            else if (GlobalVar.Channeling == 2)
            {
                if (hints.Count > GlobalVar.Channeling - 1)
                {
                    text += hints[GlobalVar.Channeling - 1];
                }
                else
                {
                    text += "";
                }
            }
            else if (GlobalVar.Channeling == 3)
            {
                if (hints.Count > GlobalVar.Channeling - 1)
                {
                    text += hints[GlobalVar.Channeling - 1];
                }
                else
                {
                    text += "";
                }
                timers[1].AutoReset = false;
                timers[1].Enabled = false;
                timers.Remove(timers[1]);
                GlobalVar.Channeling = 0;
            }
            await channel.SendMessageAsync(text);
        }

        public static async void StrategyEnded(object sender, ElapsedEventArgs e)
        {
            GlobalVar.CanUseItem = false;
            timers.Remove(timers[0]);
            await channel.SendMessageAsync("STRATEGY TIME ENDED!");
        }

        public static void StopTimer()
        {
            foreach (var time in timers)
            {
                time.Enabled = false;
                time.Stop();
            }
            timers.Clear();
            timers = new List<Timer>();
        }
        
    }
}
