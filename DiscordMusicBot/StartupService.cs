using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordMusicBot
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            DiscordSocketClient discord,
            CommandService commands)
        {
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrWhiteSpace(MilliaBot.TOKEN))
                throw new Exception("No Token found");

            await _discord.LoginAsync(TokenType.Bot, MilliaBot.TOKEN);     // Login to discord
            await _discord.StartAsync();                                // Connect to the websocket

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());     // Load commands and modules into the command service
            await _discord.SetGameAsync("Guilty Gear Xrd -Revelator-");
        }
    }
}
