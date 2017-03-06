using System.Collections.Generic;

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
