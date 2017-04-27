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
        public static FileAnalysed CreateNeo4JMatchingVersion(this FileAnalysed original)
        {
            return new FileAnalysed
            {
                Album = Sanitize(original.Album),
                AlbumArtist = Sanitize(original.AlbumArtist),
                Artist = Sanitize(original.Artist),
                FilePath = MiniSanitize(original.FilePath),
                FixSuggestion = original.FixSuggestion,
                FixSuggestions = original.FixSuggestions,
                Id = original.Id,
                Id3TagIncomplete = original.Id3TagIncomplete,
                InCollectionPath = original.InCollectionPath,
                MarkedAsPartOfCollection = original.MarkedAsPartOfCollection,
                Title = Sanitize(original.Title),
                Track = original.Track
            };
        }

        public static FileAnalysed CreateNeo4JUpdatingVersion(this FileAnalysed original)
        {
            return new FileAnalysed
            {
                Album = HtmlDecode(original.Album),
                AlbumArtist = HtmlDecode(original.AlbumArtist),
                Artist = HtmlDecode(original.Artist),
                FilePath = MiniSanitize(original.FilePath),
                Title = HtmlDecode(original.Title),
                Track = original.Track
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
