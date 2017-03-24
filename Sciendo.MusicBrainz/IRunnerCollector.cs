using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public interface IRunnerCollector:IRunner
    {
        void CollectAndSave();
    }
}
