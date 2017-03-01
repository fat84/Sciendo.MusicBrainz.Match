using System;
using System.Collections.Generic;
using System.Linq;
using Id3;

namespace Sciendo.MusicBrainz.Match
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
        }

        public FileAnalysed AnalyseFile(IMp3Stream mp3File, string filePath)
        {
            if (mp3File == null)
                throw new ArgumentNullException(nameof(mp3File));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if(!_fileSystem.FileExists(filePath))
                throw new ArgumentException("File does not exist.",nameof(filePath));
            if(!mp3File.HasTags)
                return new FileAnalysed { Id3Tag = null, FilePath = filePath };
            var availableTagVersions = mp3File.AvailableTagVersions.OrderBy(v => v.Major).LastOrDefault();
            if (availableTagVersions == null)
                return new FileAnalysed {Id3Tag = null, FilePath = filePath};
            var id3Tag = mp3File.GetTag(availableTagVersions.Major, availableTagVersions.Minor);
            bool isPartOfCollection = false;
            if (id3Tag == null || id3Tag.Band == null || id3Tag.Artists==null)
                isPartOfCollection = false;
            else
            {
                isPartOfCollection = !string.IsNullOrEmpty(id3Tag.Band.TextValue) &&
                                     id3Tag.Band.TextValue.ToLower() != id3Tag.Artists.TextValue.ToLower() &&
                                     id3Tag.Band == _collectionMarker;
            }
            return new FileAnalysed
            {
                Id3Tag = id3Tag,
                FilePath = filePath,
                ShouldBePartOfCollection = _collectionPaths.Any(c => filePath.ToLower().Contains(c.ToLower())),
                IsPartOfCollection=isPartOfCollection
            };
        }

        public string[] CollectionPaths {
            get { return _collectionPaths;}
        }

        public string CollectionMarker
        {
            get { return _collectionMarker;}
        }
    }
}
