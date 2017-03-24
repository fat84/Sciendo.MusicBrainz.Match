using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine.Text;
using Sciendo.FilesAnalyser;
using Sciendo.FilesAnalyser.Configuration;
using Sciendo.IOC;

namespace Sciendo.BulkFileAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options= new Options();
            var result = CommandLine.Parser.Default.ParseArguments(args,options);
            if (result)
            {
                var fileSystem = new FileSystem();
                var extensions =
                    ((FileSystemConfigurationSection) ConfigurationManager.GetSection("fileSystem")).Extensions
                    .Cast<ExtensionElement>().Select(e => e.Value).ToArray();
                var collectionPaths =
                    ((AnalyserConfigurationSection) ConfigurationManager.GetSection("analyser")).CollectionPaths
                    .Cast<CollectionPathElement>().Select(e => e.Value).ToArray();
                var collectionMarker =
                    ((AnalyserConfigurationSection) ConfigurationManager.GetSection("analyser")).CollectionMarker;



                var analyser = new FileAnalyser(collectionPaths, fileSystem, collectionMarker);

                fileSystem.ExtensionsRead += FileSystem_ExtensionsRead;
                fileSystem.DirectoryRead += FileSystem_DirectoryRead;
                analyser.AnalyserProgress += Analyser_AnalyserProgress;
                var runner = new Runner(fileSystem, options.Source, extensions, analyser, options.Output);
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                cancellationToken.Register(runner.Stop);
                Task runTask = new Task(runner.Start, cancellationToken);
                runTask.Start();
                Console.ReadKey();
                runner.Stop();
                return;
            }
            Console.WriteLine(options.GetHelpText());
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
