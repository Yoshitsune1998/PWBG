using Discord.WebSocket;
using PWBG_BOT.Core.System;

namespace PWBG_BOT
{
    public static class GlobalVar
    {
        public static DiscordSocketClient Client { get; set; }
        public static bool QuizHasBeenStarted = false;
        public static int Channeling { get; set; }
        public static Quiz selected { get; set; }
    }
}
