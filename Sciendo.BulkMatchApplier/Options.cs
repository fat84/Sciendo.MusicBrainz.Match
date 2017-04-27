using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Sciendo.MusicBrainz;

namespace Sciendo.BulkMatchApplier
{
    public class Options
    {
        [Option('s', "source", DefaultValue = "newallmatched.xml", Required = true, HelpText = "path to the source file. This matched file produced by Sciendo.BulkMusicMatcher.")]
        public string Source { get; set; }

        [Option('a', "applytype", DefaultValue = MusicBrainz.ApplyType.Matched, Required = true, HelpText = "depends on the data in the source file. if the source file is a matched one it should be Matched, if the source file is an unmatched file it should be UnMatched, it should also accept a file produced by the bulkfile.analyser in which case it should be All.")]
        public ApplyType ApplyType { get; set; }

        public string GetHelpText()
        {
            return CommandLine.Text.HelpText.AutoBuild(this).ToString();
        }

    }
}
