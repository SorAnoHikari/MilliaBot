using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using YoutubeExtractor;
using Discord.Audio;

namespace DiscordMusicBot
{
    public static class MusicService
    {
        public static async Task ExecutePlaylist(IAudioClient voiceClient, List<string> playlist, Channel jamSessionChatChannel)
        {
            while (playlist.Count > 0)
            {
                string strCmdText;
                // TODO: Make this better
                Process cmdProcess = new Process();
                cmdProcess.StartInfo.FileName = "cmd.exe";
                cmdProcess.StartInfo.UseShellExecute = false;
                cmdProcess.StartInfo.RedirectStandardOutput = true;
                cmdProcess.StartInfo.RedirectStandardInput = true;
                cmdProcess.Start();

                /* Change for local/prod */
                //cmdProcess.StandardInput.WriteLine(@"del C:\Users\Tony\Desktop\Misc\DiscordBot\DiscordMusicBot\DiscordMusicBot\bin\assets\current.mp3");
                cmdProcess.StandardInput.WriteLine(@"del C:\MilliaBot\MilliaBot\assets\current.mp3");

                //cmdProcess.StartInfo.WorkingDirectory = @"cd C:\Users\Tony\Desktop\Misc\DiscordBot\DiscordMusicBot\DiscordMusicBot\bin\Debug";
                cmdProcess.StartInfo.WorkingDirectory = @"cd C:\MilliaBot\MilliaBot\Debug";

                strCmdText =
                    "youtube-dl -o ../assets/current.mp3 --extract-audio --audio-format mp3 " +
                    playlist.First();
                cmdProcess.StandardInput.WriteLine(strCmdText);

                await Task.Delay(5555);

                string file = "../assets/current.mp3";

                await jamSessionChatChannel.SendMessage("Now playing: " + playlist.First());
                if (playlist.Count > 1)
                {
                    await jamSessionChatChannel.SendMessage("Songs left in the playlist: " + playlist.Count);
                }

                await PlayMusic(voiceClient, file);

                if (playlist.Count > 0)
                    playlist.RemoveAt(0);
            }
        }

        public static async Task PlayMusic(IAudioClient voiceClient, string file)
        {
            var ffmpegProcess = new ProcessStartInfo();

            ffmpegProcess.FileName = @"C:\FFMPEG\bin\ffmpeg.exe";
            ffmpegProcess.Arguments = $"-i {file} -f s16le -ar 48000 -ac 2 pipe:1 -loglevel quiet";
            ffmpegProcess.UseShellExecute = false;
            ffmpegProcess.RedirectStandardOutput = true;

            var p = Process.Start(ffmpegProcess);

            await Task.Delay(1000); //give it 2 seconds to get some dataz
            int blockSize = 3840; // 1920 for mono
            byte[] buffer = new byte[blockSize];
            int read;
            while (!MilliaBot.IsSkipSong)
            {
                read = p.StandardOutput.BaseStream.Read(buffer, 0, blockSize);
                if (read == 0 || MilliaBot.IsSkipSong)
                {
                    MilliaBot.IsSkipSong = false;
                    await Task.Delay(1000);
                    break; //nothing to read
                }
                voiceClient.Send(buffer, 0, read);
            }
            voiceClient.Wait();
        }
    }
}
