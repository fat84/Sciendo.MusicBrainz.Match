using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public static class ExtensionMethods
    {
        public static MusicBase CreateNeo4JMatchingVersion(this MusicBase original)
        {
            return new MusicBase
            {
                Album = new Item {Name= Sanitize(original.Album.Name),Id=original.Album.Id,FixSuggestion = original.Album.FixSuggestion},
                AlbumArtist = new Item { Name = Sanitize(original.AlbumArtist.Name), Id = original.AlbumArtist.Id,FixSuggestion=original.AlbumArtist.FixSuggestion },
                Artist = new Item { Name = Sanitize(original.Artist.Name), Id = original.Artist.Id, FixSuggestion=original.Artist.FixSuggestion },
                FilePath = MiniSanitize(original.FilePath),
                Title = new Item { Name = Sanitize(original.Title.Name), Id = original.Title.Id, FixSuggestion=original.Title.FixSuggestion }
            };
        }

        public static MusicBase CreateNeo4JUpdatingVersion(this MusicBase original)
        {
            return new MusicBase
            {
                Album = new Item { Name = HtmlDecode(original.Album.Name), Id = original.Album.Id, FixSuggestion = original.Album.FixSuggestion },
                AlbumArtist = new Item { Name = HtmlDecode(original.AlbumArtist.Name), Id = original.AlbumArtist.Id, FixSuggestion = original.AlbumArtist.FixSuggestion },
                Artist = new Item { Name = HtmlDecode(original.Artist.Name), Id = original.Artist.Id, FixSuggestion = original.Artist.FixSuggestion },
                FilePath = MiniSanitize(original.FilePath),
                Title = new Item { Name = HtmlDecode(original.Title.Name), Id = original.Title.Id, FixSuggestion = original.Title.FixSuggestion }
            };
        }

        private static string HtmlDecode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            var output = string.Copy(input);
            return HttpUtility.HtmlDecode(output);
        }
        private static string MiniSanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            var output = string.Copy(input);
            return HttpUtility.HtmlDecode(output).ToLower().Replace(@"\", "/");
        }


        private static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            var output = string.Copy(input);
            return HttpUtility.HtmlDecode(output)
                .ToLower()
                .Replace("?", ".?")
                .Replace("\"", ".?")
                .Replace(@"\", ".?")
                .Replace(@"'", ".?")
                .Replace("(", ".?")
                .Replace(")", ".?")
                .Replace("[", ".?")
                .Replace("]", ".?")
                .Replace("...", "…")
                .Replace("`", ".?");
        }

    }
}
