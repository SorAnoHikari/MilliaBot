using Discord;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord.Audio;
using Discord.Commands;
using Discord.Modules;
using DiscordMusicBot.TonyDBDataSetTableAdapters;

namespace DiscordMusicBot
{
    class Program
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

        static void Main(string[] args)
        {
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
                EnableMultiserver = true,
                Bitrate = 128,
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
                    if (recievedMessage.ToLower().Equals("nyan"))
                        await e.Channel.SendMessage("nyan~☆");
                    if (recievedMessage.ToLower().Equals("!nobully"))
                        await
                            e.Channel.SendMessage(
                                "http://36.media.tumblr.com/b9a0de59acdde512065cd345f6b14593/tumblr_nmvjvaqlq31r66h7yo1_500.jpg");
                    if (recievedMessage.ToLower().Equals("!bully"))
                        await
                            e.Channel.SendMessage(
                                "https://data.desustorage.org/a/image/1436/16/1436163143100.jpg");
                    if (recievedMessage.ToLower().Equals("!herewego"))
                        await
                            e.Channel.SendMessage(
                                "http://suptg.thisisnotatrueending.com/archive/32329424/images/1400964390429.jpg");
                    if (recievedMessage.ToLower().Equals("!everyday"))
                        await
                            e.Channel.SendMessage(
                                "http://40.media.tumblr.com/5fef7876e1bfe7c744a7c5d8969ea5ba/tumblr_moly86fGRT1swsp86o2_r1_500.jpg");
                    if (recievedMessage.ToLower().Equals("!idk"))
                        await
                            e.Channel.SendMessage(
                                "http://s.quickmeme.com/img/45/45cb7c3f84254c6aea385e88ab44149887e56bbb86ba7f7ad62ace9a91a521ef.jpg");
                    if (recievedMessage.ToLower().Equals("!autism"))
                        await
                            e.Channel.SendMessage(
                                "http://i.imgur.com/bv3ruu8.jpg");
                    if (recievedMessage.ToLower().Equals("!awoo"))
                        await
                            e.Channel.SendMessage(
                                "http://i.imgur.com/oPVnqGU.png");
                    if (recievedMessage.ToLower().Contains("!reaction"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count == 1)
                        {
                            await e.Channel.SendMessage( "Format is !reaction description(optional)");
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
                    if (recievedMessage.ToLower().Contains("!addreaction"))
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
                            await e.Channel.SendMessage( "Format is !addreaction url description");
                        }
                    }
                    if (recievedMessage.ToLower().Contains("!hitbox"))
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
                            await e.Channel.SendMessage( "Format is !hitbox charactername movename");
                        }
                    }
                    if (recievedMessage.ToLower().Contains("!addhitbox"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count > 3)
                        {
                            var moveName = commandList.Skip(3).Aggregate((i, j) => i + " " + j).ToLower();
                            var result = MilliaUtils.AddHitboxImage(commandList[1], commandList[2].ToLower(), moveName.ToLower());

                            await e.Channel.SendMessage( result);
                        }
                        else
                        {
                            await e.Channel.SendMessage( "Format is !addhitbox url (character name) (move name)");
                        }
                    }
                    if (recievedMessage.ToLower().Contains("!addcharacter"))
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
                                    await e.Channel.SendMessage( charName + " added");
                                }
                                else
                                {
                                    await e.Channel.SendMessage( "Failed to add character");
                                }
                            }
                            else
                            {
                                await e.Channel.SendMessage( charName + " already exists");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage( "Format is !addcharacter (character name)");
                        }
                    }
                    if (recievedMessage.ToLower().Contains("!playsong"))
                    {
                        var commandList = recievedMessage.Split(' ').ToList();
                        if (commandList.Count > 1)
                        {
                            var songName = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                            string file = "../assets/" + songName +".mp3";
                            var voiceChannel = OttawaAnimeCrewServer.VoiceChannels.FirstOrDefault(v => v.Name.Equals("Jam Session"));

                            var VoiceClient = await voiceChannel.JoinAudio();

                            await MusicService.PlayMusic(VoiceClient, file);
                        }
                        else
                        {
                            await e.Channel.SendMessage("Format is !playsong (songname)");
                        }
                    }
                    if (recievedMessage.ToLower().Contains("!downloadsong"))
                    {
                        if (!IsPlayingMusic)
                        {
                            IsPlayingMusic = true;
                            var commandList = recievedMessage.Split(' ').ToList();
                            if (commandList.Count > 1)
                            {
                                var youtubeUrl = commandList.Skip(1).Aggregate((i, j) => i + " " + j);
                                var voiceChannel =
                                    OttawaAnimeCrewServer.VoiceChannels.FirstOrDefault(v => v.Name.Equals("Jam Session"));

                                var voiceClient = await voiceChannel.JoinAudio();

                                string strCmdText;
                                // TODO: Make this better
                                Process cmdProcess = new Process();
                                cmdProcess.StartInfo.FileName = "cmd.exe";
                                cmdProcess.StartInfo.UseShellExecute = false;
                                cmdProcess.StartInfo.RedirectStandardOutput = true;
                                cmdProcess.StartInfo.RedirectStandardInput = true;
                                cmdProcess.Start();

                                //cmdProcess.StandardInput.WriteLine(@"del C:\Users\Tony\Desktop\Misc\DiscordBot\DiscordMusicBot\DiscordMusicBot\bin\assets\current.mp3");
                                cmdProcess.StandardInput.WriteLine(@"del C:\MilliaBot\MilliaBot\assets\current.mp3");

                                //cmdProcess.StartInfo.WorkingDirectory = @"cd C:\Users\Tony\Desktop\Misc\DiscordBot\DiscordMusicBot\DiscordMusicBot\bin\Debug";
                                cmdProcess.StartInfo.WorkingDirectory = @"cd C:\MilliaBot\MilliaBot\Debug";

                                strCmdText = "youtube-dl -o ../assets/current.mp3 --extract-audio --audio-format mp3 " +
                                             youtubeUrl;
                                cmdProcess.StandardInput.WriteLine(strCmdText);

                                await Task.Delay(10000);

                                string file = "../assets/current.mp3";

                                MusicService.PlayMusic(voiceClient, file);

                                IsPlayingMusic = false;
                            }
                            else
                            {
                                await e.Channel.SendMessage("Format is !downloadsong (Youtube URL)");
                            }
                        }
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
                        OttawaAnimeCrewServer = inoriClient.Servers.FirstOrDefault(s => s.Name.Equals("Ottawa Anime Crew"));
                        var jamSession = OttawaAnimeCrewServer.VoiceChannels.FirstOrDefault(v => v.Name.Equals("Jam Session"));
                        await jamSession.JoinAudio();

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
