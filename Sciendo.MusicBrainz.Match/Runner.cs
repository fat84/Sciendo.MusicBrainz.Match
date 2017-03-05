using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;
using Sciendo.Common.Serialization;
using Sciendo.IOC;

namespace Sciendo.MusicBrainz.Match
{
    public class Runner:IRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _path;
        private readonly string[] _extensions;
        private readonly IAnalyser _analyser;
        private readonly string _outputFilePath;
        private List<FileAnalysed> _filesAnalysed;
        private bool _fileSystemStopped=false;
        private bool _analyserStopped = false;
        private Container _container;

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
            _filesAnalysed=new List<FileAnalysed>();
            _container = Container.GetInstance();
        }
        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            var folders = _fileSystem.GetLeafDirectories(_path);
            if(folders!=null)
                foreach (var folder in folders)
                {
                    if (_fileSystem.StopActivity || _analyser.StopActivity)
                    {
                        Serializer.SerializeToFile(_filesAnalysed,_outputFilePath);
                        return;
                    }
                    _filesAnalysed.AddRange(RunDirectory(folder));
                }
            _filesAnalysed.AddRange(RunDirectory(_path));
            if(!string.IsNullOrEmpty(_outputFilePath))
                Serializer.SerializeToFile(_filesAnalysed,_outputFilePath);
        }

        private IEnumerable<FileAnalysed> RunDirectory(string path)
        {
            var previousArtist = string.Empty;
            var files = _fileSystem.GetFiles(path, _extensions);
            if (files == null) yield break;
            foreach (var file in files)
            {
                if (_fileSystem.StopActivity || _analyser.StopActivity)
                {
                    if(string.IsNullOrEmpty(_outputFilePath))
                        Serializer.SerializeToFile(_filesAnalysed,_outputFilePath);
                    break;
                }
                var newMp3File = _container.ResolveToNew<IMp3Stream>(_analyser.Mp3IocKey, file,Mp3Permissions.Read);
                var fileAnalysed = _analyser.AnalyseFile(newMp3File, file);
                if (!string.IsNullOrEmpty(previousArtist) && fileAnalysed.Id3TagComplete &&
                    previousArtist != fileAnalysed.Artist)
                {
                    fileAnalysed.PossiblePartOfACollection = true;
                }
                yield return fileAnalysed;
            }
        }

        public void Stop()
        {
            _fileSystem.StopActivity = true;
            _analyser.StopActivity = true;
            Serializer.SerializeToFile(_filesAnalysed, _outputFilePath);
        }

        public List<FileAnalysed> FilesAnalysed { get {return _filesAnalysed;} }
    }
}
