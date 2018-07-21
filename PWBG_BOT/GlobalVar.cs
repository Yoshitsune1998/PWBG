using Discord.WebSocket;
using PWBG_BOT.Core.System;

namespace PWBG_BOT
{
    public static class GlobalVar
    {
        public static DiscordSocketClient Client { get; set; }
        public static bool QuizHasBeenStarted = false;
        public static int Channeling { get; set; }
        public static Quiz Selected { get; set; }
        public static SocketGuild QuizGuild { get; set; }
        public static SocketTextChannel ChannelSelect { get; set; }
        public static SocketGuild GuildSelect { get; set; }
    }
}
