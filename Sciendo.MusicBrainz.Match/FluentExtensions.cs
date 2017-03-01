using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;

namespace Sciendo.MusicBrainz.Match   
{
    public static class FluentExtensions
    {
        public static IEnumerable<FileAnalysed> GetTags(this IEnumerable<string> files, IAnalyser tagLooper)
        {
            foreach (var file in files)
            {
                yield return tagLooper.AnalyseFile(new Mp3File(file),file);
            }
        }
    }
}
