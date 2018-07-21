using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using PWBG_BOT.Core.System;
using Discord.WebSocket;

namespace PWBG_BOT.Core
{
    public static class ReapetedTimer
    {
        private static SocketTextChannel channel;

        private static List<Timer> timers = new List<Timer>();

        private static List<string> hints = new List<string>();

        //2nd timer
        public static Task StartTimer(SocketTextChannel e)
        {
            channel = e;
            
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
        public static Task HintCounter(SocketTextChannel e, double QuizTimer)
        {
            channel = e;
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

        //1st timer
        public static Task StartQuiz(ulong time, SocketGuild e, SocketTextChannel f)
        {
            channel = f;
            GlobalVar.QuizGuild = e;
            var loopTimer = new Timer()
            {
                Interval = (time * 1000) + 20000,
                AutoReset = false,
                Enabled = true,
            };
            timers.Add(loopTimer);
            
            loopTimer.Elapsed += QuizEnded;
            Tasking.Sleep(5000);
            StartTimer(f);
            //
            Tasking.Sleep(timers[1].Interval * 3);
            //
            HintCounter(f, loopTimer.Interval);

            return Task.CompletedTask;
        }

        private static async void QuizEnded(object sender, ElapsedEventArgs e)
        {
            var guild = GlobalVar.QuizGuild;
            var users = guild.Users;
            uint highscores = 0;
            List<SocketUser> winner = new List<SocketUser>();
            var role = from r in guild.Roles
                       where r.Name.Equals("Player")
                       select r;
            var des = role.FirstOrDefault();

            Tasking.Sleep(3000);

            await channel.SendMessageAsync($"The right answer is {GlobalVar.Selected.RightAnswer}");

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
                }
            }
            Tasking.Sleep(1000);
            await channel.SendMessageAsync(text);
            Tasking.Sleep(1000);
            await Quizzes.GiveDrops(winner, channel);
            GlobalVar.QuizHasBeenStarted = false;
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
                timers[1].AutoReset = false;
                timers[1].Enabled = false;
                timers.Remove(timers[1]);
                GlobalVar.Channeling = 0;
                GlobalVar.QuizHasBeenStarted = true;
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
                    text += $"HINT-{GlobalVar.Channeling}\n";
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

        public static void StopTimer()
        {
            foreach (var time in timers)
            {
                time.Stop();
            }
            timers = new List<Timer>();
        }
        
    }
}
