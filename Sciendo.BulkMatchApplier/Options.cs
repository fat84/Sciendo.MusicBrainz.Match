using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Sciendo.BulkMatchApplier
{
    public class Options
    {
        [Option('s', "source", DefaultValue = "newallmatched.xml", Required = true, HelpText = "path to the source file. This matched file produced by Sciendo.BulkMusicMatcher.")]
        public string Source { get; set; }

        public string GetHelpText()
        {
            return CommandLine.Text.HelpText.AutoBuild(this).ToString();
        }

    }
}
