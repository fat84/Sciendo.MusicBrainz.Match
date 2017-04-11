using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sciendo.Common.Serialization;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.Unmatched.Consolidator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var filesAnalysed = Serializer.DeserializeFromFile<FileAnalysed>(options.Source);

                var suggestions =
                    filesAnalysed.Where(f => f.FixSuggestion != "No Suggestion")
                        .Select(f => new FileAnalysedConsolidated { FilePath = f.FilePath, FixSuggestion = f.FixSuggestion }).ToList();
                Serializer.SerializeToFile(suggestions, options.Output);
                return;
            }
            Console.WriteLine(options.GetHelpText());


        }
    }
}
