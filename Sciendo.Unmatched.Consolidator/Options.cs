using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Sciendo.Unmatched.Consolidator
{
    public class Options
    {
        [Option('s', "source", DefaultValue = "newallunmatched.xml", Required = true, HelpText = "path to the source file. This file is the unmactched file produced by Sciendo.BulkMusicMatcher after a human intervention to identify fix suggestions.")]
        public string Source { get; set; }

        [Option('o', "out", DefaultValue = "newallunmatchedfixsuggestion.xml", Required = true, HelpText = "path to the output file. This file contains only file paths and the human edited fix suggestions.")]
        public string Output { get; set; }
        public string GetHelpText()
        {
            return CommandLine.Text.HelpText.AutoBuild(this).ToString();
        }


    }
}
