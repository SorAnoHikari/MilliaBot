using System;
using System.Collections.Generic;
using System.Linq;
using DiscordMusicBot.TonyDBDataSetTableAdapters;

namespace DiscordMusicBot
{
    public static class ReactionUtils
    {
        public static string GetReactionImage(string description)
        {
            string url = "";

            var adaptor = new ReactionImagesTableAdapter();
            var reactionImages = adaptor.GetData();

            List<TonyDBDataSet.ReactionImagesRow> images;
            if (!string.IsNullOrEmpty(description))
            {
                images = reactionImages.Where(i => i.Description.ToLower().Contains(description)).ToList();
            }
            else
            {
                images = reactionImages.ToList();
            }
            if (images.Count > 0)
            {
                url = images.GetRandom().ImageUrl;
            }
            
            return url;
        }
    }
}
