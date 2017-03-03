using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicBrainz.Match
{
    public interface IRunner
    {
        void Initialize();
        void Start();

        void Stop();

        List<FileAnalysed> FilesAnalysed { get; }
    }
}
