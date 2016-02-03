using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordMusicBot.TonyDBDataSetTableAdapters;

namespace DiscordMusicBot
{
    public static class MilliaUtils
    {
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
    }
}
