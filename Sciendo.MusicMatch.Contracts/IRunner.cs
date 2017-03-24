using System.Collections.Generic;

namespace Sciendo.MusicMatch.Contracts
{
    public interface IRunner
    {
        void Initialize();
        void Start();

        void Stop();
    }
}
