using System;
using System.Collections.Generic;
using System.Linq;
using DiscordMusicBot.TonyDBDataSetTableAdapters;
using WebSocketSharp;

namespace DiscordMusicBot
{
    public static class ReactionUtils
    {
        public static string GetReactionImage(string description)
        {
            string url = "";

            var adaptor = new ReactionImagesTableAdapter();
            var reactionImages = adaptor.GetData();

            var r = new Random();
            List<TonyDBDataSet.ReactionImagesRow> images;
            if (!description.IsNullOrEmpty())
            {
                images = reactionImages.Where(i => i.Description.Contains(description)).ToList();
            }
            else
            {
                images = reactionImages.ToList();
            }
            if (images.Count > 0)
            {

                url = images.ElementAt(r.Next(1)).ImageUrl;
            }
            
            return url;
        }
    }
}
