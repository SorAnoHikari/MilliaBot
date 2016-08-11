using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace DiscordMusicBot
{
    public static class WebServices
    {
        private static System.Windows.Forms.HtmlDocument DocumentHtml;
        public static string GetMatchVideo(string character1, string character2)
        {
            string youtubeUrl = "Nothing found";

            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            var keepOnRockinUrl = @"http://www.keeponrock.in?character1=" + GetCharAlias(character1);

            if (!String.IsNullOrEmpty(character2))
            {
                character2 = GetCharAlias(character2);
                keepOnRockinUrl += "&character1=" + character2;
            }

            return keepOnRockinUrl;

            Stream data = client.OpenRead(keepOnRockinUrl);

            try
            {
                // download each page and dump the content
                var task = MessageLoopWorker.Run(DoWorkAsync, keepOnRockinUrl);
                task.Wait();

                if (task.Result != null)
                {
                    string html = task.Result.ToString();

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var youtubeLink = doc.DocumentNode.SelectSingleNode("//div[@class='kor-match-table-cell kor-match-table-link']//a");
                    if (youtubeLink != null && youtubeLink.HasAttributes)
                    {
                        youtubeUrl = youtubeLink.Attributes["href"].Value;
                    }
                }
                else
                {
                    youtubeUrl = "Something went wrong, try again perhaps?";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DoWorkAsync failed: " + ex.Message);
            }

            return youtubeUrl;
        }

        static async Task<string> DoWorkAsync(object[] args)
        {
            string html = "";
            using (var wb = new WebBrowser())
            {
                wb.ScriptErrorsSuppressed = true;

                TaskCompletionSource<bool> tcs = null;
                //WebBrowserDocumentCompletedEventHandler documentCompletedHandler = (s, e) => tcs.TrySetResult(true);

                // navigate to each URL in the list
                foreach (var url in args)
                {
                    tcs = new TaskCompletionSource<bool>();
                    wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
                    try
                    {
                        wb.Navigate(url.ToString());
                        //await Task.Delay(2000);
                        while (wb.ReadyState != WebBrowserReadyState.Complete)
                        {
                            Application.DoEvents();
                        }
                        // await for DocumentCompleted
                        await tcs.Task;
                    }
                    finally
                    {
                        wb.DocumentCompleted -= wb_DocumentCompleted;
                    }

                    html = DocumentHtml.Body.InnerText;
                }
            }

            return html;
        }

        private static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            if (wb.ReadyState != WebBrowserReadyState.Complete)
                return;
            else
                DocumentHtml = wb.Document;
        }

        public static class MessageLoopWorker
        {
            public static async Task<object> Run(Func<object[], Task<string>> worker, params object[] args)
            {
                var tcs = new TaskCompletionSource<object>();

                var thread = new Thread(() =>
                {
                    EventHandler idleHandler = null;

                    idleHandler = async (s, e) =>
                    {
                        // handle Application.Idle just once
                        Application.Idle -= idleHandler;

                        // return to the message loop
                        await Task.Yield();

                        // and continue asynchronously
                        // propogate the result or exception
                        try
                        {
                            var result = await worker(args);
                            tcs.SetResult(result);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }

                        // signal to exit the message loop
                        // Application.Run will exit at this point
                        Application.ExitThread();
                    };

                    // handle Application.Idle just once
                    // to make sure we're inside the message loop
                    // and SynchronizationContext has been correctly installed
                    Application.Idle += idleHandler;
                    Application.Run();
                });

                // set STA model for the new thread
                thread.SetApartmentState(ApartmentState.STA);

                // start the thread and await for the task
                thread.Start();
                try
                {
                    return await tcs.Task;
                }
                finally
                {
                    thread.Join();
                }
            }
        }

        private static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                Console.WriteLine("Natigated to {0}", e.Url);
                Application.ExitThread();   // Stops the thread
            }
        }

        private static string GetCharAlias(string name)
        {
            string alias = "";
            switch (name)
            {
                case "chipp":
                    alias = "chp";
                    break;
                case "i-no":
                    alias = "ino";
                    break;
                case "jacko":
                case "jack-o":
                    alias = "jko";
                    break;
                case "johnny":
                    alias = "jhn";
                    break;
                case "ky":
                    alias = "kyk";
                    break;
                case "millia":
                    alias = "mll";
                    break;
                case "raven":
                    alias = "rev";
                    break;
                case "slayer":
                    alias = "sly";
                    break;
                default:
                    alias = name.Substring(0, 3);
                    break;
            }
            return alias;
        }
    }
}
