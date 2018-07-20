using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using Discord.WebSocket;

namespace PWBG_BOT.Core
{
    public static class ReapetedTimer
    {
        private static SocketTextChannel channel;

        private static List<Timer> timers = new List<Timer>();

        //2nd timer
        public static Task StartTimer()
        {
            channel = GlobalVar.Client.GetGuild(464870639153578026).GetTextChannel(464870639153578028);
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

        //1st timer
        public static Task StartQuiz()
        {
            channel = GlobalVar.Client.GetGuild(464870639153578026).GetTextChannel(464870639153578028);
            var loopTimer = new Timer()
            {
                Interval = 60000,
                AutoReset = false,
                Enabled = true,
            };
            timers.Add(loopTimer);
            StartTimer();
            loopTimer.Elapsed += QuizEnded;

            return Task.CompletedTask;
        }

        private static async void QuizEnded(object sender, ElapsedEventArgs e)
        {
            var guild = GlobalVar.Client.GetGuild(464870639153578026);
            var users = guild.Users;
            
            var role = from r in guild.Roles
                       where r.Name.Equals("Player")
                       select r;
            var des = role.FirstOrDefault();
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
                    
                    user.TempPoint = 0;
                }
            }
            await channel.SendMessageAsync(text);
            GlobalVar.QuizHasBeenStarted = false;
            GlobalVar.selected = null;
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

        public static void StopTimer()
        {
            foreach (var time in timers)
            {
                time.Stop();
            }
        }
        
    }
}
