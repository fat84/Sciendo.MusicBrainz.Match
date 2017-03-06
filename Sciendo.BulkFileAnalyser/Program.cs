using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Id3;
using Sciendo.FilesAnalyser;
using Sciendo.FilesAnalyser.Configuration;
using Sciendo.IOC;

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
            Container.GetInstance().Add(new RegisteredType().For<Mp3File>().BasedOn<IMp3Stream>().With(LifeStyle.Transient).IdentifiedBy(analyser.Mp3IocKey));

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
