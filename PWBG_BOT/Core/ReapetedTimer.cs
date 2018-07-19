using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;

namespace PWBG_BOT.Core
{
    internal static class ReapetedTimer
    {
        private static Timer loopTimer;
        private static SocketTextChannel channel;

        internal static Task StartTimer()
        {
            channel = GlobalVar.Client.GetGuild(464870639153578026).GetTextChannel(464870639153578028);
            loopTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            //loopTimer.Elapsed += OnTimerTicked;


            return Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            await channel.SendMessageAsync("Ping!!!");
        }
        
    }
}
