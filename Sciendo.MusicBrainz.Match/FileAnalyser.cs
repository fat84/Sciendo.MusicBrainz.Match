using System;
using System.IO;
using System.Linq;
using Sciendo.MusicMatch.Contracts;
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

        public Music AnalyseFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if(!_fileSystem.FileExists(filePath))
                throw new ArgumentException("File does not exist.",nameof(filePath));
            if(AnalyserProgress!=null)
                AnalyserProgress(this,new AnalyserProgressEventArgs(filePath));

            var tag = _fileSystem.ReadTagFromFile(filePath);
            if (tag == null || tag.IsEmpty)
                return new Music
                {
                    Artist = null,
                    Album = null,
                    Title = null,
                    FilePath = filePath,
                    TagAnalysis = new TagAnalysis {Id3TagIncomplete = true}
                };
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
            return new Music
            {
                Artist = (tag.Performers==null )?new Item(): new Item {Name= tag.Performers.FirstOrDefault(p=>p!=_collectionMarker).CleanString()},
                Album=(tag.Album==null) ?new Item() : new Item {Name= tag.Album.CleanString()},
                Title=(tag.Title==null)?new Item(): new Item {Name= tag.Title.CleanString()},
                Track=tag.Track,
                FilePath = filePath,
                TagAnalysis = new TagAnalysis { 
                InCollectionPath = _collectionPaths.Any(c => filePath.ToLower().Contains(c.ToLower())),
                MarkedAsPartOfCollection=isPartOfCollection,
                Id3TagIncomplete=!tagComplete},
                AlbumArtist= (isPartOfCollection)?new Item {Name= _collectionMarker}:new Item()
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
