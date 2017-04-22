using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicMatch.Contracts
{
    public class FileAnalysedConsolidated
    {
        public string FilePath { get; set; }


        public string FixSuggestion { get; set; }

        public FixSuggestion FixSuggestions { get; set; }

    }
}
