using System;
using System.IO;
using System.Linq;
using TagLib;

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

        public FileAnalysed AnalyseFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if(!_fileSystem.FileExists(filePath))
                throw new ArgumentException("File does not exist.",nameof(filePath));
            if(AnalyserProgress!=null)
                AnalyserProgress(this,new AnalyserProgressEventArgs(filePath));

            var tag = _fileSystem.ReadTagFromFile(filePath);
            if (tag==null || tag.IsEmpty)
                return new FileAnalysed { Artist = null, Album=null,Title=null, FilePath = filePath, Id3TagIncomplete=true };
            bool isPartOfCollection = false;
            if (tag.AlbumArtists == null || tag.Artists==null || !tag.AlbumArtists.Any() || !tag.Artists.Any())
                isPartOfCollection = false;
            else
            {
                isPartOfCollection = !string.IsNullOrEmpty(tag.AlbumArtists.FirstOrDefault()) &&
                                     tag.AlbumArtists.FirstOrDefault().ToLower() != tag.Artists.FirstOrDefault().ToLower() &&
                                     tag.AlbumArtists.FirstOrDefault() == _collectionMarker;
            }
            var tagComplete = ValidateTag(tag);
            return new FileAnalysed
            {
                Artist = (tag.Artists==null )?null: tag.Artists.FirstOrDefault(),
                Album=(tag.Album==null) ?null :tag.Album,
                Title=(tag.Title==null)?null:tag.Title,
                Track=tag.Track,
                FilePath = filePath,
                InCollectionPath = _collectionPaths.Any(c => filePath.ToLower().Contains(c.ToLower())),
                MarkedAsPartOfCollection=isPartOfCollection,
                Id3TagIncomplete=!tagComplete
                
            };
        }

        private bool ValidateTag(Tag tag)
        {
            if (tag == null)
                return false;
            if (tag.Artists == null)
                return false;
            if (string.IsNullOrEmpty(tag.Artists.FirstOrDefault()))
                return false;
            if (tag.Album == null)
                return false;
            if (string.IsNullOrEmpty(tag.Album))
                return false;
            if (tag.Title == null)
                return false;
            if (string.IsNullOrEmpty(tag.Title))
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
    }
}
