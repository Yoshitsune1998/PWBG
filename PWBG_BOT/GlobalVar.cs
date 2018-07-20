using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;

namespace PWBG_BOT
{
    internal static class GlobalVar
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static bool QuizHasBeenStarted { get; set; }
    }
}
