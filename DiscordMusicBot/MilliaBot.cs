using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord.Audio;
using Discord.Commands;
using Discord.Modules;
using Discord.WebSocket;
using DiscordMusicBot.TonyDBDataSetTableAdapters;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot
{
    class MilliaBot
    {
        #region Login Info
        public static readonly string TOKEN = "Mzg3ODIxNzQ4MjQ0MTg1MDg4.DQkCrA.kEltsSYZ-GFEg46nuVxGko9Uj0M";
        public static readonly string SERVER_ID = "0jh83Qzd7N5HqLXq";
        public static readonly string VOICE_CHANNEL_ID = "0jh83Qzd7N3xkOBT";
        #endregion

        private static DiscordSocketClient _discord;
        private static CommandService _commands;

        public static void Main(string[] args)
            => new MilliaBot().RoundStartHaircar().GetAwaiter().GetResult();

        public async Task RoundStartHaircar()
        {
            var services = new ServiceCollection().AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                MessageCacheSize = 100
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async, // Force all commands to run async
                LogLevel = LogSeverity.Debug
            }))
            .AddSingleton<DiscordEventHandlers>()
            .AddSingleton<LoggingService>()
            .AddSingleton<StartupService>()
            .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();     // Create the service provider

            provider.GetRequiredService<LoggingService>();      // Initialize the logging service, startup service, and command handler
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<DiscordEventHandlers>();

            await Task.Delay(-1);     // Prevent the application from closing
        }
    }
}
