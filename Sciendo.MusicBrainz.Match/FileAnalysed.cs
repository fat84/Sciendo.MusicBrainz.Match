using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;
using Id3.Id3;

namespace Sciendo.MusicBrainz.Match
{
    public class FileAnalysed
    {
        public string FilePath { get; set; }


        public string Artist { get; set; }

        public string Album { get; set; }

        public string Title { get; set; }

        public bool MarkedAsPartOfCollection { get; set; }

        public bool PossiblePartOfACollection { get; set; }

        public bool InCollectionPath { get; set; }

        public bool Id3TagComplete { get
        {
            return !string.IsNullOrEmpty(Album) || !string.IsNullOrEmpty(Artist) ||
                   !string.IsNullOrEmpty(Title);
        } }
    }
}
