using System;
using System.Threading.Tasks;
using Discord;

namespace DiscordMusicBot
{
    class Program
    {
        #region Login Info
        public static readonly string EMAIL = "tony.cheng300@gmail.com";
        public static readonly string PASSWORD = "Gardevoir3";
        public static readonly string TL_SOKU_INVITE_CODE = "https://discord.gg/0jh83Qzd7N5HqLXq";
        #endregion

        static void Main(string[] args)
        {
            var inoriClient = new DiscordClient();

            //Display all log messages in the console
            inoriClient.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            //Echo back any message received, provided it didn't come from the bot itself


            //Convert our sync method to an async one and block the Main function until the bot disconnects

        }

        private async void EchoMessage(DiscordClient client)
        {
            client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                    await client.SendMessage(e.Channel, e.Message.Text);
            };
        }

        private async void Connect(DiscordClient inoriClient)
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
