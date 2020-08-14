using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using DiscordMusicBot.TonyDBDataSetTableAdapters;
using Microsoft.Extensions.Configuration;

namespace DiscordMusicBot
{
    public class DiscordEventHandlers
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;

        public DiscordEventHandlers(DiscordSocketClient discord,
            CommandService commands,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.ReactionAdded += HandleReactionAddedAsync;
            _discord.ReactionRemoved += HandleReactionRemovedAsync;
            _discord.UserJoined += HandleUserEvents;
        }

        private static bool IsPlayingMusic;
        public static bool IsSkipSong { get; set; }

        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();

            Console.WriteLine($"{reaction.User.Value} just added a reaction '{reaction.Emote}' " +
                                  $"to {message.Author}'s message ({message.Id}).");
            if (message != null && reaction.User.IsSpecified)
            {
                if (message.Channel.Name != null && message.Channel.Name.Contains("role-assignment"))
                {
                    var roleName = String.Concat(message.Content.Trim().ToLower(), "-", reaction.Emote.Name.ToLower());
                    switch (message.Content.Trim().ToLower())
                    {
                        case "sf5":
                            roleName = "sf5";
                            break;
                        case "acpr":
                            roleName = "acpr";
                            break;
                        default:
                            break;
                    }
                    var ottawaServer = _discord.Guilds.ToList().FirstOrDefault(r=>r.Name == "Ottawa FGC");
                    var roleObj =
                            ottawaServer.Roles.ToList().FirstOrDefault(r => r.Name == roleName);
                    var user = ottawaServer.Users.ToList().FirstOrDefault(u => u.Id == reaction.UserId);
                    await user.AddRoleAsync(roleObj);
                }
            }
        }

        public async Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();

            Console.WriteLine($"{reaction.User.Value} just added a reaction '{reaction.Emote}' " +
                                  $"to {message.Author}'s message ({message.Id}).");
            if (message != null && reaction.User.IsSpecified)
            {
                if (message.Channel.Name != null && message.Channel.Name.Contains("role-assignment"))
                {
                    var roleName = String.Concat(message.Content.Trim().ToLower(), "-", reaction.Emote.Name.ToLower());
                    switch (message.Content.Trim().ToLower()) {
                        case "sf5":
                            roleName = "sf5";
                            break;
                        case "acpr":
                            roleName = "acpr";
                            break;
                        default:
                            break;
                    }
  
                    var ottawaServer = _discord.Guilds.ToList().FirstOrDefault(r => r.Name == "Ottawa FGC");
                    var roleObj =
                            ottawaServer.Roles.ToList().FirstOrDefault(r => r.Name == roleName);
                    var user = ottawaServer.Users.ToList().FirstOrDefault(u => u.Id == reaction.UserId);
                    await user.RemoveRoleAsync(roleObj);
                }
            }
        }

        public async Task OnMessageReceivedAsync(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;

            if (message == null) return;
            if (message.Author.Id == _discord.CurrentUser.Id) return;

            var context = new SocketCommandContext(_discord, message);

            var recievedMessage = message.Content;

            if (message.Channel.Name != null && recievedMessage.ToLower().Contains("!giverole"))
            {
                if (message.Channel.Name.Contains("secret-clubs"))
                {
                    if (recievedMessage.ToLower().Contains("litclub"))
                    {
                        var litClubRole =
                            context.Guild.Roles.ToList().FirstOrDefault(r => r.Name == "Literature Club Member");
                        await (context.User as IGuildUser).AddRoleAsync(litClubRole);
                        await context.Channel.SendMessageAsync(
                            "Welcome to the club good sir");
                    }
                    else if (recievedMessage.ToLower().Contains("exdegen"))
                    {
                        var exDegenRole = context.Guild.Roles.ToList()
                            .FirstOrDefault(r => r.Name == "EX-Degenerate");
                        await (context.User as IGuildUser).AddRoleAsync(exDegenRole);
                        await context.Channel.SendMessageAsync(
                            "You've maxed out your degen levels");
                    }
                }
                else if (message.Channel.Name.Contains("setplay-corner"))
                {
                    if (recievedMessage.ToLower().Contains("amq"))
                    {
                        var litClubRole =
                            context.Guild.Roles.ToList().FirstOrDefault(r => r.Name == "AMQ");
                        await (context.User as IGuildUser).AddRoleAsync(litClubRole);
                        await context.Channel.SendMessageAsync(
                            "Let's pop together");
                    }
                }
                else if (message.Channel.Name.Contains("roles"))
                {
                    if (recievedMessage.ToLower().Contains("sfv"))
                    {
                        var litClubRole =
                            context.Guild.Roles.ToList().FirstOrDefault(r => r.Name == "SFV");
                        await (context.User as IGuildUser).AddRoleAsync(litClubRole);
                        await context.Channel.SendMessageAsync(
                            "SFV role shimmied in there");
                    }
                    else if (recievedMessage.ToLower().Contains("dbfz"))
                    {
                        var litClubRole =
                            context.Guild.Roles.ToList().FirstOrDefault(r => r.Name == "DBFZ");
                        await (context.User as IGuildUser).AddRoleAsync(litClubRole);
                        await context.Channel.SendMessageAsync(
                            "DBFZ role added (I couldn't think of anything clever)");
                    }
                    else if(recievedMessage.ToLower().Contains("tekken"))
                    {
                        var litClubRole =
                            context.Guild.Roles.ToList().FirstOrDefault(r => r.Name == "SFV");
                        await (context.User as IGuildUser).AddRoleAsync(litClubRole);
                        await context.Channel.SendMessageAsync(
                            "Ready to tekken a nap");
                    }
                    else if (recievedMessage.ToLower().Contains("mvci"))
                    {
                        var litClubRole =
                            context.Guild.Roles.ToList().FirstOrDefault(r => r.Name == "MVCI");
                        await (context.User as IGuildUser).AddRoleAsync(litClubRole);
                        await context.Channel.SendMessageAsync(
                            "It's time for mahvel baby");
                    }
                }
            }
            // Handle the recieved message
            else if (recievedMessage.ToLower().Equals("!help") || recievedMessage.ToLower().Equals("help"))
            {
                if (context.Channel.Name.Contains("secret-clubs"))
                {
                    await
                        context.Channel.SendMessageAsync(
                            "Use !giverole (role) to give yourself a role\n Available roles: LitClub, EXDegen");
                }
                else
                {
                    await
                        context.Channel.SendMessageAsync(
                            "Sample commands: !add !reaction !addreaction !hitbox !addsong");
                }
            }
            else if (recievedMessage.ToLower().Equals("nyan"))
                await context.Channel.SendMessageAsync("nyan~☆");
            else if (recievedMessage.ToLower().Equals("!awoo"))
                await
                    context.Channel.SendMessageAsync(
                        "http://i.imgur.com/oPVnqGU.png");
            else if ((recievedMessage.ToLower().Contains("milliabot") || recievedMessage.Replace("-", "").ToLower().Contains("robomillia")) && recievedMessage.EndsWith("?"))
                await
                    context.Channel.SendMessageAsync(
                        MilliaUtils.GetEightBallResponse());
            else if (recievedMessage.ToLower().Contains("!reaction"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count == 1)
                {
                    await context.Channel.SendMessageAsync("Format is !reaction description(optional)");
                }
                if (commandList.Count > 1)
                {
                    var description = commandList.Skip(1).Aggregate((i, j) => i + " " + j).ToLower();
                    var url = ReactionUtils.GetReactionImage(description);
                    if (string.IsNullOrEmpty(url))
                        url = "No images found";
                    await
                        context.Channel.SendMessageAsync(
                            url);
                }
                else
                {
                    var url = ReactionUtils.GetReactionImage(null);
                    if (string.IsNullOrEmpty(url))
                        url = "No images found";
                    await
                        context.Channel.SendMessageAsync(
                            url);
                }
            }
            else if (recievedMessage.ToLower().Contains("!addreaction"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var adaptor = new ReactionImagesTableAdapter();

                    if (commandList.Count > 2)
                    {
                        var description = commandList.Skip(2).Aggregate((i, j) => i + " " + j);
                        adaptor.Insert(commandList[1], description);
                    }
                    else
                        adaptor.Insert(commandList[1], null);
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !addreaction url description");
                }
            }
            else if (recievedMessage.ToLower().Contains("!hitbox"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 2)
                {
                    var moveName = commandList.Skip(2).Aggregate((i, j) => i + " " + j).ToLower();
                    var url = MilliaUtils.GetHitboxImageUrl(commandList[1].ToLower(), moveName);
                    await
                        context.Channel.SendMessageAsync(
                            url);
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !hitbox charactername movename");
                }
            }
            else if (recievedMessage.ToLower().Contains("!addhitbox"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 3)
                {
                    var moveName = commandList.Skip(3).Aggregate((i, j) => i + " " + j).ToLower();
                    var result = MilliaUtils.AddHitboxImage(commandList[1], commandList[2].ToLower(), moveName.ToLower());

                    await context.Channel.SendMessageAsync(result);
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !addhitbox url (character name) (move name)");
                }
            }
            else if (recievedMessage.ToLower().Contains("!addcharacter"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var charName = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                    var characterAdaptor = new CharactersTableAdapter();
                    var existingCharacter =
                        characterAdaptor.GetData().FirstOrDefault(c => c.Name.ToLower().Equals(charName));
                    if (existingCharacter == null)
                    {
                        characterAdaptor.Insert(charName, "");
                        if (characterAdaptor.GetData().FirstOrDefault(c => c.Name.Equals(charName)) != null)
                        {
                            await context.Channel.SendMessageAsync(charName + " added");
                        }
                        else
                        {
                            await context.Channel.SendMessageAsync("Failed to add character");
                        }
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(charName + " already exists");
                    }
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !addcharacter (character name)");
                }
            }
            else if (recievedMessage.ToLower().Contains("!joinvoice"))
            {
                var OACGuild = _discord.Guilds.ToList().FirstOrDefault(g => g.Name.Equals("Ottawa Anime Crew"));
                if (OACGuild != null)
                    await OACGuild.VoiceChannels.ToList().FirstOrDefault(v => v.Name == "Jam Session").ConnectAsync();
            }
            /*else if (recievedMessage.ToLower().Contains("!playsong"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var songName = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                    string file = "../assets/" + songName + ".mp3";
                    var voiceChannel = context.Guild.VoiceChannels.ToList().FirstOrDefault(v => v.Name.Equals("Jam Session"));

                    var VoiceClient = await voiceChannel.JoinAudio();

                    await MusicService.PlayMusic(VoiceClient, file);
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !playsong (songname)");
                }
            }
            else if (recievedMessage.ToLower().Contains("!addsong"))
            {

                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var youtubeUrl = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                    var channelName = "Koromo's Room";
                    var koromos_room = "twitch_chat";
                    // Change for servers
                    var voiceClient = await MilliaBot.OttawaAnimeCrewServer.VoiceChannels.FirstOrDefault(v => v.Name.Equals("Jam Session")).JoinAudio();

                    if (!IsPlayingMusic)
                    {
                        IsPlayingMusic = true;

                        playlist.Add(youtubeUrl);

                        await MusicService.ExecutePlaylist(voiceClient, playlist, MilliaBot.MusicChatChannel);

                        IsPlayingMusic = false;
                    }
                    else
                    {
                        playlist.Add(youtubeUrl);
                        await context.Channel.SendMessageAsync("added to playlist");
                    }
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !addsong (Youtube URL)");
                }
            }*/
            else if (recievedMessage.ToLower().Equals("!skip"))
            {
                IsSkipSong = true;
                await context.Channel.SendMessageAsync("Song skipped");
            }
            else if (recievedMessage.ToLower().Contains("!matchvideo"))
            {
                var commandList = recievedMessage.ToLower().Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    string char1 = commandList[1];
                    string char2 = "";
                    if (commandList.Count > 2)
                        char2 = commandList[2];

                    var link = WebServices.GetMatchVideo(char1, char2);

                    await context.Channel.SendMessageAsync(link);
                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !matchvideo (char1) (char2)");
                }
            }
            else if (recievedMessage.ToLower().StartsWith("!unbansonify"))
            {
                var commandList = recievedMessage.ToLower().Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var inputLine = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                    var correctedSentence = MilliaUtils.GetAutoCorrectedSentence(inputLine);
                    await context.Channel.SendMessageAsync(correctedSentence);
                }
            }
            else if (recievedMessage.ToLower().StartsWith("!add"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var command = commandList[0].Replace("!add", "");
                    var adaptor = new CommandsTableAdapter();
                    var link = commandList.Skip(1).Aggregate((i, j) => i + " " + j);

                    adaptor.Insert(command, link);

                }
                else
                {
                    await context.Channel.SendMessageAsync("Format is !add (URL)");
                }
            }
            else if (recievedMessage.ToLower().StartsWith("!avatar"))
            {
                var commandList = recievedMessage.Split(' ').ToList();
                if (commandList.Count > 1)
                {
                    var name = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                    var user =
                        context.Guild.Users.ToList()
                            .FirstOrDefault(
                                u =>
                                    u.Username.ToLower().Equals(name.ToLower()) ||
                                    u.Nickname.ToLower().Equals(name.ToLower()));
                    if (user != null)
                    {
                        await context.Channel.SendMessageAsync(user.GetAvatarUrl());
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync("I don't see that user.");
                    }
                }
            }
            else if (recievedMessage.ToLower().StartsWith("!item"))
            {
                await context.Channel.SendMessageAsync(MilliaUtils.GetFaustItem());
            }
            // Must be last
            else if (recievedMessage.Contains("!"))
            {
                var commandList = recievedMessage.Split(' ').ToList();

                var command = commandList.FirstOrDefault(msg => msg.StartsWith("!"));

                if (command != null)
                {
                    command = MilliaUtils.GetCommandLink(command.Replace("!", ""));
                    if (!String.IsNullOrEmpty(command))
                        await context.Channel.SendMessageAsync(command);
                }
            }
        }

        public async Task HandleUserEvents(SocketGuildUser newUser)
        {
            if (newUser.Guild.Name.ToLower().Contains("ottawa"))
            {
                var greetingsChannel =
                    newUser.Guild.TextChannels.ToList().FirstOrDefault(c => c.Name.ToLower().Contains("greetings"));
                if (greetingsChannel != null)
                {
                    await greetingsChannel.SendMessageAsync("Welcome to the crew, " + newUser.Mention);
                }
            }
            else if (newUser.Guild.Name.ToLower().Equals("testserver"))
            {
                var greetingsChannel =
                    newUser.Guild.TextChannels.ToList().FirstOrDefault(c => c.Name.ToLower().Contains("general"));
                if (greetingsChannel != null)
                {
                    await greetingsChannel.SendMessageAsync(@"Who bitch dis is http://i.giphy.com/khIfCXAE9Gog0.gif");
                    await greetingsChannel.SendMessageAsync("Welcome to the crew, " + newUser.Mention);
                }
            }
        }
    }
}
