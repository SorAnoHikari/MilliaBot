using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord.Audio;
using Discord.Commands;
using Discord.Modules;
using DiscordMusicBot.TonyDBDataSetTableAdapters;

namespace DiscordMusicBot
{
    class MilliaBot
    {
        #region Login Info
        public static readonly string EMAIL = "tony.cheng300@gmail.com";
        public static readonly string PASSWORD = "Gardevoir3";
        public static readonly string TL_SOKU_INVITE_CODE = "https://discord.gg/0jh83Qzd7N5HqLXq";
        public static readonly string SERVER_ID = "0jh83Qzd7N5HqLXq";
        public static readonly string VOICE_CHANNEL_ID = "0jh83Qzd7N3xkOBT";
        public static readonly long TL_SOKU_SERVER_ID = 131946718471127041;
        #endregion

        private static bool IsPlayingMusic;
        private static Server OttawaAnimeCrewServer { get; set; }
        private static Server TLSokuServer { get; set; }
        private static Channel VoiceChannel { get; set; }
        private static Channel MusicChatChannel { get; set; }

        public static List<string> Playlist { get; set; }
        public static bool IsSkipSong { get; set; }

        static void Main(string[] args)
        {
            Playlist = new List<string>();
            var inoriClient = new DiscordClient(x =>
            {
                x.AppName = "MilliaBot";
                x.MessageCacheSize = 0;
                x.UsePermissionsCache = true;
                x.EnablePreUpdateEvents = true;
                x.LogLevel = LogSeverity.Debug;
            })
                .UsingCommands(x =>
                {
                    x.AllowMentionPrefix = true;
                    x.HelpMode = HelpMode.Public;
                })
                .UsingModules();

            inoriClient.AddService<AudioService>(new AudioService(new AudioServiceConfigBuilder()
            {
                Channels = 2,
                EnableEncryption = false,
                EnableMultiserver = false,
                Bitrate = 96,
            }));

            //Display all log messages in the console
            inoriClient.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            HandleMessage(inoriClient);

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            Connect(inoriClient);
        }

        private static async void HandleMessage(DiscordClient client)
        {
            client.MessageReceived += async (s, e) =>
            {
                var recievedMessage = e.Message.Text;

                if (!(e.Message.User.Name.ToLower().Equals("milliabot") || e.Message.User.Name.ToLower().Equals("robo-millia")))
                {

                    // Handle the recieved message
                    if (recievedMessage.ToLower().Equals("!help") || recievedMessage.ToLower().Equals("help"))
                    {
                        await
                            e.Channel.SendMessage(
                                "Sample commands: !nobully !idk !reaction !addreaction !hitbox !addhitbox");
                    }
                    else if (recievedMessage.ToLower().Equals("nyan"))
                        await e.Channel.SendMessage("nyan~☆");
                    else if (recievedMessage.ToLower().Equals("!awoo"))
                        await
                            e.Channel.SendMessage(
                                "http://i.imgur.com/oPVnqGU.png");
                    else if ((recievedMessage.ToLower().Contains("milliabot") || recievedMessage.Replace("-", "").ToLower().Contains("robomillia")) && recievedMessage.EndsWith("?"))
                        await
                            e.Channel.SendMessage(
                                MilliaUtils.GetEightBallResponse());
                    else if (recievedMessage.ToLower().Contains("!reaction"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count == 1)
                        {
                            await e.Channel.SendMessage("Format is !reaction description(optional)");
                        }
                        if (commandList.Count > 1)
                        {
                            var description = commandList.Skip(1).Aggregate((i, j) => i + " " + j).ToLower();
                            var url = ReactionUtils.GetReactionImage(description);
                            if (string.IsNullOrEmpty(url))
                                url = "No images found";
                            await
                                e.Channel.SendMessage(
                                    url);
                        }
                        else
                        {
                            var url = ReactionUtils.GetReactionImage(null);
                            if (string.IsNullOrEmpty(url))
                                url = "No images found";
                            await
                                e.Channel.SendMessage(
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
                            await e.Channel.SendMessage("Format is !addreaction url description");
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
                                e.Channel.SendMessage(
                                    url);
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !hitbox charactername movename");
                        }
                    }
                    else if (recievedMessage.ToLower().Contains("!addhitbox"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count > 3)
                        {
                            var moveName = commandList.Skip(3).Aggregate((i, j) => i + " " + j).ToLower();
                            var result = MilliaUtils.AddHitboxImage(commandList[1], commandList[2].ToLower(), moveName.ToLower());

                            await e.Channel.SendMessage(result);
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !addhitbox url (character name) (move name)");
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
                                    await e.Channel.SendMessage(charName + " added");
                                }
                                else
                                {
                                    await e.Channel.SendMessage("Failed to add character");
                                }
                            }
                            else
                            {
                                await e.Channel.SendMessage(charName + " already exists");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !addcharacter (character name)");
                        }
                    }
                    else if (recievedMessage.ToLower().Contains("!playsong"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count > 1)
                        {
                            var songName = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                            string file = "../assets/" + songName + ".mp3";
                            var voiceChannel = OttawaAnimeCrewServer.VoiceChannels.FirstOrDefault(v => v.Name.Equals("Jam Session"));

                            var VoiceClient = await voiceChannel.JoinAudio();

                            await MusicService.PlayMusic(VoiceClient, file);
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !playsong (songname)");
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
                            var voiceClient = await VoiceChannel.JoinAudio();

                            if (!IsPlayingMusic)
                            {
                                IsPlayingMusic = true;

                                Playlist.Add(youtubeUrl);

                                await MusicService.ExecutePlaylist(voiceClient, Playlist, MusicChatChannel);

                                IsPlayingMusic = false;
                            }
                            else
                            {
                                Playlist.Add(youtubeUrl);
                                await e.Channel.SendMessage("added to playlist");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !downloadsong (Youtube URL)");
                        }
                    }
                    else if (recievedMessage.ToLower().Equals("!skip"))
                    {
                        IsSkipSong = true;
                        await e.Channel.SendMessage("Song skipped");
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

                            await e.Channel.SendMessage(link);
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !matchvideo (char1) (char2)");
                        }
                    }
                    else if (recievedMessage.ToLower().StartsWith("!unbansonify"))
                    {
                        var commandList = recievedMessage.ToLower().Split(' ').ToList();
                        if (commandList.Count > 1)
                        {
                            var inputLine = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                            var correctedSentence = MilliaUtils.GetAutoCorrectedSentence(inputLine);
                            await e.Channel.SendMessage(correctedSentence);
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
                            await e.Channel.SendMessage("Format is !add (URL)");
                        }
                    }
                    else if (recievedMessage.ToLower().StartsWith("!avatar"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count > 1)
                        {
                            var name = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                            var user = e.Server.Users.FirstOrDefault(u => u.Name.ToLower().Equals(name.ToLower()));
                            if (user != null)
                            {
                                if (user.AvatarUrl != null)
                                    await e.Channel.SendMessage(user.AvatarUrl);
                            }
                            else
                            {
                                await e.Channel.SendMessage("I don't see that user.");
                            }
                        }
                    }
                    else if (recievedMessage.ToLower().StartsWith("!setgame"))
                    {
                        client.SetGame("Guilty Gear Xrd -Revelator-");
                    }
                    else if (recievedMessage.ToLower().StartsWith("!item"))
                    {
                        await e.Channel.SendMessage(MilliaUtils.GetFaustItem());
                    }
                    // Must be last
                    else if (recievedMessage.StartsWith("!"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();

                        var value = MilliaUtils.GetCommandLink(commandList[0].Replace("!", ""));
                        if (!String.IsNullOrEmpty(value))
                            await e.Channel.SendMessage(value);
                    }
                }
                client.GetChannel(1);
            };
        }

        private static async void Connect(DiscordClient inoriClient)
        {
            inoriClient.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try
                    {
                        await inoriClient.Connect(EMAIL, PASSWORD);
                        inoriClient.SetGame("Guilty Gear Xrd -Revelator-");
                        var channelName = "Jam Session";
                        var textChannelName = "jam-session";
                        OttawaAnimeCrewServer = inoriClient.Servers.FirstOrDefault(s => s.Name.Equals("Ottawa Anime Crew"));
                        TLSokuServer = inoriClient.Servers.FirstOrDefault(s => s.Name.Equals("TL Soku"));
                        VoiceChannel = OttawaAnimeCrewServer.VoiceChannels.FirstOrDefault(v => v.Name.Equals(channelName));
                        MusicChatChannel = TLSokuServer.TextChannels.FirstOrDefault(v => v.Name.Equals(textChannelName));
                        await VoiceChannel.JoinAudio();

                        break;
                    }
                    catch (Exception ex)
                    {
                        inoriClient.Log.Message += (s, e) => Console.WriteLine(String.Concat("Login Failed", ex));
                        await Task.Delay(inoriClient.Config.FailedReconnectDelay);
                    }
                }
            });
        }
    }
}
