namespace Sciendo.FilesAnalyser
{
    public class FileAnalysed
    {
        public string FilePath { get; set; }


        public string Artist { get; set; }

        public string Album { get; set; }

        public uint Track { get; set; }

        public string Title { get; set; }

        public bool MarkedAsPartOfCollection { get; set; }

        public bool PossiblePartOfACollection { get; set; }

        public bool InCollectionPath { get; set; }

        public bool Id3TagIncomplete { get; set; }
    }
}
