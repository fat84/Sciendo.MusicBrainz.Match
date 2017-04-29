using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sciendo.Common.Serialization;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    //public class RunnerMatchedApplyer
    //{
    //    private readonly string source;
    //    private readonly ApplyType _applyType;
    //    private readonly bool _testOnly;
    //    private readonly IMusicBrainzAdapter musicBrainzAdapter;

    //    public RunnerMatchedApplyer(IMusicBrainzAdapter musicBrainzAdapter, string source, ApplyType applyType, bool testOnly)
    //    {
    //        this.source = source;
    //        _applyType = applyType;
    //        _testOnly = testOnly;
    //        this.musicBrainzAdapter = musicBrainzAdapter;
    //    }
    //    public void Start()
    //    {
    //        SetOfFiles = Serializer.DeserializeFromFile<Music>(source);
    //        switch (_applyType)
    //        {
    //            case ApplyType.Matched:
    //                musicBrainzAdapter.LinkToExisting(SetOfFiles,false,_testOnly);
    //                break;
    //            case ApplyType.UnMatched:
    //                musicBrainzAdapter.CreateNew(SetOfFiles,_testOnly);
    //                break;
    //            case ApplyType.All:
    //                musicBrainzAdapter.LinkToExisting(SetOfFiles,true,_testOnly);
    //                break;
    //            default:
    //                return;
    //        }
    //    }

    //    public List<Music> SetOfFiles { get; private set; }

    //    public void Stop()
    //    {
    //        musicBrainzAdapter.StopActivity = true;
    //    }

    //    public void SaveTrace()
    //    {
    //        if (SetOfFiles != null && SetOfFiles.Any())
    //        {
    //            UpSertSave(source);
    //        }
    //    }

    //    private void UpSertSave(string file)
    //    {
    //        Serializer.SerializeToFile(SetOfFiles, file);
    //    }


    //}
}
