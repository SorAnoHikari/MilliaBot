using Discord;
using Discord.Audio;
using System;
using System.Text;
using System.Threading.Tasks;
using DiscordMusicBot.TonyDBDataSetTableAdapters;
using WebSocketSharp;

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

        static void Main(string[] args)
        {
            var inoriClient = new DiscordClient();

            //Display all log messages in the console
            inoriClient.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            HandleMessage(inoriClient);

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            Connect(inoriClient);
        }

        private static async void HandleMessage(DiscordClient client)
        {
            client.MessageReceived += async (s, e) =>
            {
                var recievedMessage = e.Message.Text;
                // Handle the recieved message
                if (recievedMessage.ToLower().Equals("!help") || recievedMessage.ToLower().Equals("help"))
                {
                    await client.SendMessage(e.Channel, "Sample commands: !nobully !idk !reaction !addreaction");
                }
                if (recievedMessage.ToLower().Equals("nyan"))
                    await client.SendMessage(e.Channel, "nyan~☆");
                if (recievedMessage.ToLower().Equals("!nobully"))
                    await
                        client.SendMessage(e.Channel,
                            "http://36.media.tumblr.com/b9a0de59acdde512065cd345f6b14593/tumblr_nmvjvaqlq31r66h7yo1_500.jpg");
                if (recievedMessage.ToLower().Equals("!herewego"))
                    await
                        client.SendMessage(e.Channel,
                            "http://suptg.thisisnotatrueending.com/archive/32329424/images/1400964390429.jpg");
                if (recievedMessage.ToLower().Equals("!everyday"))
                    await
                        client.SendMessage(e.Channel,
                            "http://40.media.tumblr.com/5fef7876e1bfe7c744a7c5d8969ea5ba/tumblr_moly86fGRT1swsp86o2_r1_500.jpg");
                if (recievedMessage.ToLower().Equals("!idk"))
                    await
                        client.SendMessage(e.Channel,
                            "http://s.quickmeme.com/img/45/45cb7c3f84254c6aea385e88ab44149887e56bbb86ba7f7ad62ace9a91a521ef.jpg");
                if (recievedMessage.ToLower().Contains("!reaction"))
                {
                    var commandList = recievedMessage.Split(' ');
                    if (commandList.Length == 1)
                    {
                        await client.SendMessage(e.Channel, "Format is {!reaction} {description (optional)}");
                    }
                    if (commandList.Length > 1)
                    {
                        var url = ReactionUtils.GetReactionImage(commandList[1]);
                        if (url.IsNullOrEmpty())
                            url = "No images found";
                        await
                            client.SendMessage(e.Channel,
                                url);
                    }
                    else
                    {
                        var url = ReactionUtils.GetReactionImage(null);
                        if (url.IsNullOrEmpty())
                            url = "No images found";
                        await
                            client.SendMessage(e.Channel,
                                url);
                    }
                }
                if (recievedMessage.ToLower().Contains("!addreaction"))
                {
                    var commandList = recievedMessage.Split(' ');
                    if (commandList.Length > 1)
                    {
                        var adaptor = new ReactionImagesTableAdapter();
                        if (commandList.Length > 2)
                            adaptor.Insert(commandList[1], commandList[2]);
                        else
                            adaptor.Insert(commandList[1], null);
                    }
                    else
                    {
                        await client.SendMessage(e.Channel, "Format is {!addreaction} {url} {description}");
                    }
                }
                client.GetChannel(1);
            };
        }

        private static async void Connect(DiscordClient inoriClient)
        {
            inoriClient.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await inoriClient.Connect(EMAIL, PASSWORD);
                        await inoriClient.SetGame(1);
                        
                        break;
                    }
                    catch (Exception ex)
                    {
                        inoriClient.LogMessage += (s, e) => Console.WriteLine(String.Concat("Login Failed", ex));
                        await Task.Delay(inoriClient.Config.FailedReconnectDelay);
                    }
                }
            });
        }
    }
}
