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
        public static string SaveVideoToDisk(string link)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link, false);
            
            VideoInfo video = videoInfos
                .Where(info => info.CanExtractAudio)
                .OrderByDescending(info => info.AudioBitrate)
                .First();

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the audio downloader.
             * The first argument is the video where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */
            var audioDownloader = new AudioDownloader(video, Path.Combine("../assets", video.Title + video.AudioExtension));

            // Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
            // because the download will take much longer than the audio extraction.
            audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
            audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

            /*
             * Execute the audio downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            audioDownloader.Execute();

            return video.Title;
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
            while (true)
            {
                read = p.StandardOutput.BaseStream.Read(buffer, 0, blockSize);
                if (read == 0)
                    break; //nothing to read
                voiceClient.Send(buffer, 0, read);
            }
            voiceClient.Wait();
        }
    }
}
