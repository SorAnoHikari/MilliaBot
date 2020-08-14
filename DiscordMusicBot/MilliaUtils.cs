using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMusicBot.TonyDBDataSetTableAdapters;
using NHunspell;

namespace DiscordMusicBot
{
    public static class MilliaUtils
    {
        static Random RNG = new Random();

        public static List<string> EightBallResponses = new List<string>
        {
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Yes, definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy, try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };

        public static List<KeyValuePair<string, double>> ItemList = new List<KeyValuePair<string, double>>
        {
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/1/13/GGXRD_Faust_BlackHole.png", 4),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/5/59/GGXRD_Faust_Bomb.png", 18.4),
            new KeyValuePair<string, double>("http://i.imgur.com/BoIOkfd.png", 26.7),
            new KeyValuePair<string, double>("http://i.imgur.com/CKuIDy4.png", 36.3),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/6/66/GGXRD_Faust_Hammer.png", 52.9),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/9/98/GGXRD_Faust_HeliumGas.png", 55.3),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/8/89/GGXRD_Faust_Meteors.png", 60.8),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/5/50/GGXRD_Faust_ChibiFaust.png", 77.7),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/f/f6/GGXRD_Faust_DrumCan.png", 83.7),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/7/7b/GGXRD_Faust_JumpPad.png", 87.3),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/a/aa/GGXRD_Faust_Poison.png", 96.3),
            new KeyValuePair<string, double>("http://www.dustloop.com/wiki/images/8/83/GGXRD_Faust_100TonWeight.png", 100)
        };
        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            // note: creating a Random instance each call may not be correct for you,
            // consider a thread-safe static instance
            var r = new Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
        }

        public static string AddHitboxImage(string imageUrl, string characterName, string moveName)
        {
            var character = new CharactersTableAdapter().GetData().FirstOrDefault(c => c.Name.ToLower().Contains(characterName) || c.AlternateName.ToLower().Equals(characterName));
            if (character != null)
            {
                var movesAdaptor = new MovesTableAdapter();
                var existingMove = movesAdaptor.GetData().FirstOrDefault(c => c.MoveName.ToLower().Equals(moveName) && c.CharacterID == character.Id);
                if (existingMove != null)
                    return "Move already exists for this char";
                movesAdaptor.Insert(character.Id, imageUrl, moveName);
                return "Hitbox added";
            }
            return "Couldn't find " + characterName;
        }

        public static string GetHitboxImageUrl(string characterName, string moveName)
        {
            var character = new CharactersTableAdapter().GetData().FirstOrDefault(c => c.Name.ToLower().Contains(characterName) || c.AlternateName.ToLower().Equals(characterName));
            if (character != null)
            {
                var move =
                    new MovesTableAdapter().GetData()
                        .FirstOrDefault(m => m.CharacterID == character.Id && m.MoveName.ToLower().Equals(moveName));
                if (move != null)
                {
                    return move.ImageUrl;
                }
                else
                {
                    return "Move not found";
                }
            }
            return "Character not found";
        }

        public static string GetEightBallResponse()
        {
            int r = RNG.Next(EightBallResponses.Count);
            return EightBallResponses[r];
        }

        public static string GetFaustItem()
        {
            Random r = new Random();
            double itemRoll = r.NextDouble() * 100;
            double currVal = 0.0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                currVal = ItemList[i].Value;
                if (itemRoll < currVal)
                {
                    return ItemList[i].Key;
                }
            }
            return ItemList[3].Key;
        }

        public static string GetAutoCorrectedSentence(string line)
        {
            string correctedSentence = "";
            using (Hunspell hunspell = new Hunspell("en_us.aff", "en_us.dic"))
            {
                var wordTokens = line.Split(' ');
                foreach (var word in wordTokens)
                {
                    var correct = hunspell.Spell(word);
                    if (correct)
                    {
                        correctedSentence += word + " ";
                    }
                    else
                    {
                        correctedSentence += hunspell.Suggest(word).First() + " ";
                    }
                }
            }
            return correctedSentence;
        }

        public static string GetCommandLink(string commandName)
        {
            string url = "";

            var adaptor = new CommandsTableAdapter();
            var reactionImages = adaptor.GetData();

            List<TonyDBDataSet.CommandsRow> commands;
            if (!string.IsNullOrEmpty(commandName))
            {
                commands = reactionImages.Where(i => i.Name.ToLower().Equals(commandName)).ToList();
                if (!commands.Any())
                    commands = reactionImages.Where(i => i.Name.ToLower().Contains(commandName)).ToList();

                if (commands.Count > 0)
                {
                    url = commands.GetRandom().Value;
                }
            }

            return url;
        }
    }

}
