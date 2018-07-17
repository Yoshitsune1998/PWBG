using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace PWBG_BOT
{
    internal class Program
    {

        DiscordSocketClient _client;
        CommandHandler _handler;

        private static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            }
            );
            _client.Log += _client_Log;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }


        
    }

}
