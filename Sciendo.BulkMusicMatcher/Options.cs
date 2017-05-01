using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Sciendo.BulkMusicMatcher
{
    public class Options
    {
        [Option('s',"source",DefaultValue = "newall.xml",Required = true,HelpText = "path to the source file. This file is produced by Sciendo.BulkFileAnalyser.")]
        public string Source { get; set; }

        [Option('a',"append",DefaultValue=false,Required=true,HelpText = "append to the output file(s)")]
        public bool Append { get; set; }

        public string GetHelpText()
        {
            return CommandLine.Text.HelpText.AutoBuild(this).ToString();
        }
    }
}
