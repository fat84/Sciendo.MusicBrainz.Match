using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sciendo.MusicBrainz.Match;
using Sciendo.MusicBrainz.Match.Configuration;

namespace Sciendo.BulkFileAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileSystem = new FileSystem();
            var extensions =
                ((FileSystemConfigurationSection) ConfigurationManager.GetSection("fileSystem")).Extensions
                .Cast<ExtensionElement>().Select(e => e.Value).ToArray();
            var collectionPaths = ((AnalyserConfigurationSection)ConfigurationManager.GetSection("analyser")).CollectionPaths
                .Cast<CollectionPathElement>().Select(e => e.Value).ToArray();
            var collectionMarker =
                ((AnalyserConfigurationSection) ConfigurationManager.GetSection("analyser")).CollectionMarker;

            var analyser = new FileAnalyser(collectionPaths, fileSystem, collectionMarker);
            fileSystem.ExtensionsRead += FileSystem_ExtensionsRead;
            fileSystem.DirectoryRead += FileSystem_DirectoryRead;
            analyser.AnalyserProgress += Analyser_AnalyserProgress;
            var runner= new Runner(fileSystem,args[0],extensions,analyser,args[1]);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            cancellationToken.Register(runner.Stop);
            Task runTask = new Task(runner.Start, cancellationToken);
            runTask.Start();
            Console.ReadKey();
            runner.Stop();

        }

        private static void FileSystem_DirectoryRead(object sender, DirectoryReadEventArgs e)
        {
            Console.WriteLine("{0}", e.Directory);
        }

        private static void Analyser_AnalyserProgress(object sender, AnalyserProgressEventArgs e)
        {
            Console.WriteLine("{0}",e.FilePath);
        }

        private static void FileSystem_ExtensionsRead(object sender, ExtensionsReadEventArgs e)
        {
            Console.WriteLine("Starting processing extension {0}",e.Extension);
        }
    }
}
