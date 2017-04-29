using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sciendo.Common.Serialization;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.FilesAnalyser
{
    public class Runner:IRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _path;
        private readonly string[] _extensions;
        private readonly IAnalyser _analyser;
        private readonly string _outputFilePath;
        private List<Music> _filesAnalysed;
        private bool _fileSystemStopped=false;
        private bool _analyserStopped = false;
        private static long _counter;

        public Runner(IFileSystem fileSystem, string path, string[] extensions, IAnalyser analyser, string outputFilePath="")
        {
            if(fileSystem==null)
                throw new ArgumentNullException(nameof(fileSystem));
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if(extensions==null || !extensions.Any())
                throw new ArgumentNullException(nameof(extensions));
            if(analyser==null)
                throw new ArgumentNullException(nameof(analyser));
            if(!Directory.Exists(path))
                throw new ArgumentException("Folder does not exist",nameof(path));
            _fileSystem = fileSystem;
            _path = path;
            _extensions = extensions;
            _analyser = analyser;
            _outputFilePath = outputFilePath;
            _filesAnalysed=new List<Music>();
        }
        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            _counter = 0;
            var folders = _fileSystem.GetLeafDirectories(_path);
            if(folders!=null)
                foreach (var folder in folders)
                {
                    if (_fileSystem.StopActivity || _analyser.StopActivity)
                    {
                        return;
                    }
                    _filesAnalysed.AddRange(RunDirectory(folder));
                }
            _filesAnalysed.AddRange(RunDirectory(_path));
        }

        private IEnumerable<Music> RunDirectory(string path)
        {
            var previousArtist = string.Empty;
            var files = _fileSystem.GetFiles(path, _extensions);
            if (files == null) yield break;
            foreach (var file in files)
            {
                if (_fileSystem.StopActivity || _analyser.StopActivity)
                {
                    break;
                }
                var fileAnalysed = _analyser.AnalyseFile(file);
                if (!string.IsNullOrEmpty(previousArtist) && !fileAnalysed.TagAnalysis.Id3TagIncomplete &&
                    previousArtist != fileAnalysed.Artist.Name)
                {
                    fileAnalysed.TagAnalysis.PossiblePartOfACollection = true;
                }
                fileAnalysed.Id = _counter++;
                previousArtist = (fileAnalysed.TagAnalysis.Id3TagIncomplete)? string.Empty:fileAnalysed.Artist.Name;
                yield return fileAnalysed;
            }
        }

        public void Stop()
        {
            _fileSystem.StopActivity = true;
            _analyser.StopActivity = true;
            Serializer.SerializeToFile(_filesAnalysed, _outputFilePath);
        }

        public List<Music> FilesAnalysed { get {return _filesAnalysed;} }
    }
}
