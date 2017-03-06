using System;
using System.Linq;
using Id3;
using Id3.Id3;

namespace Sciendo.FilesAnalyser
{
    public class FileAnalyser:IAnalyser
    {
        private readonly string[] _collectionPaths;
        private readonly IFileSystem _fileSystem;
        private readonly string _collectionMarker;

        public FileAnalyser(string[] collectionPaths, IFileSystem fileSystem, string collectionMarker)
        {
            if(collectionPaths==null)
                throw new ArgumentNullException(nameof(collectionPaths));
            if(fileSystem==null)
                throw new ArgumentNullException(nameof(fileSystem));
            if(string.IsNullOrEmpty(collectionMarker))
                throw new ArgumentNullException(nameof(collectionMarker));
            
            if(!fileSystem.DirectoriesExist(collectionPaths))
                throw new ArgumentException("Not all collections paths exist.",nameof(collectionPaths));
            _collectionPaths = collectionPaths;
            _fileSystem = fileSystem;
            _collectionMarker = collectionMarker;
            StopActivity = false;
        }

        public FileAnalysed AnalyseFile(IMp3Stream mp3File, string filePath)
        {
            if (mp3File == null)
                throw new ArgumentNullException(nameof(mp3File));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if(!_fileSystem.FileExists(filePath))
                throw new ArgumentException("File does not exist.",nameof(filePath));
            if(AnalyserProgress!=null)
                AnalyserProgress(this,new AnalyserProgressEventArgs(filePath));
            if(!mp3File.HasTags)
                return new FileAnalysed { Artist = null, Album=null,Title=null, FilePath = filePath, Id3TagIncomplete=true };
            var availableTagVersions = mp3File.AvailableTagVersions.OrderBy(v => v.Major).LastOrDefault();
            if (availableTagVersions == null)
                return new FileAnalysed { Artist = null, Album = null, Title = null, FilePath = filePath,Id3TagIncomplete=true};
            IId3Tag id3Tag = null;
            try
            {
                id3Tag = mp3File.GetTag(availableTagVersions.Major, availableTagVersions.Minor);
            }
            catch{}
            bool isPartOfCollection = false;
            if (id3Tag == null || id3Tag.Band == null || id3Tag.Artists==null)
                isPartOfCollection = false;
            else
            {
                isPartOfCollection = !string.IsNullOrEmpty(id3Tag.Band.TextValue) &&
                                     id3Tag.Band.TextValue.ToLower() != id3Tag.Artists.TextValue.ToLower() &&
                                     id3Tag.Band == _collectionMarker;
            }
            var tagComplete = ValidateTag(id3Tag);
            return new FileAnalysed
            {
                Artist = (id3Tag==null || id3Tag.Artists==null )?null: id3Tag.Artists.TextValue,
                Album=(id3Tag==null || id3Tag.Album==null) ?null :id3Tag.Album.TextValue,
                Title=(id3Tag==null || id3Tag.Title==null)?null:id3Tag.Title.TextValue,
                FilePath = filePath,
                InCollectionPath = _collectionPaths.Any(c => filePath.ToLower().Contains(c.ToLower())),
                MarkedAsPartOfCollection=isPartOfCollection,
                Id3TagIncomplete=!tagComplete
                
            };
        }

        private bool ValidateTag(IId3Tag id3Tag)
        {
            if (id3Tag == null)
                return false;
            if (id3Tag.Artists == null)
                return false;
            if (string.IsNullOrEmpty(id3Tag.Artists.TextValue))
                return false;
            if (id3Tag.Album == null)
                return false;
            if (string.IsNullOrEmpty(id3Tag.Album.TextValue))
                return false;
            if (id3Tag.Title == null)
                return false;
            if (string.IsNullOrEmpty(id3Tag.Title.TextValue))
                return false;
            return true;
        }

        public string[] CollectionPaths {
            get { return _collectionPaths;}
        }

        public string CollectionMarker
        {
            get { return _collectionMarker;}
        }

        public bool StopActivity { get; set; }
        public event EventHandler<AnalyserProgressEventArgs> AnalyserProgress;
        public string Mp3IocKey { get { return "MP3IOCKEY"; } }
    }
}
