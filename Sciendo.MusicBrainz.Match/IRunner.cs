using System.Collections.Generic;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.FilesAnalyser
{
    public interface IRunner
    {
        void Initialize();
        void Start();

        void Stop();

        List<FileAnalysed> FilesAnalysed { get; }
    }
}
