using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Sciendo.BulkFileAnalyser
{
    public class Options
    {
        [Option('s',"source",DefaultValue=".",HelpText="Path to the source folder.",Required=true)]
        public string Source { get; set; }

        [Option('o',"output",DefaultValue = "allnew.xml",Required=true,HelpText="Path to the outputfile.")]
        public string Output { get; set; }

        public string GetHelpText()
        {
            return CommandLine.Text.HelpText.AutoBuild(this);
        }

    }
}
