using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neo4jClient;
using Sciendo.MusicBrainz;
using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicBrainz.Configuration;
using Sciendo.MusicBrainz.Loaders;
using Sciendo.MusicBrainz.Queries;
using Sciendo.MusicBrainz.Queries.Matching;
using Sciendo.MusicBrainz.Queries.Merging;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.BulkMusicMatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var configSection =
        ((MusicBrainzConfigSection)ConfigurationManager.GetSection("musicBrainz"));
                GraphClient client = new GraphClient(new Uri(configSection.Url), configSection.UserName, configSection.Password);
                client.Connect();
                MusicBrainzAdapter musicBrainzAdapter = new MusicBrainzAdapter(client, new MatchingQueryFactory(),new MergingQueryFactory(), 
                    new LoaderFactory(new ItemMemoryCache(), new ItemMemoryCache()), configSection.NotFoundOptions);
                musicBrainzAdapter.CheckMatchingProgress += MusicBrainzAdapter_CheckProgress;
                var runner= new RunnerCollector(musicBrainzAdapter,options.Source,options.Append);
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                cancellationToken.Register(runner.Stop);
                Task runTask = new Task(runner.Start,cancellationToken);
                runTask.Start();
                Console.ReadKey();
                    cancellationTokenSource.Cancel();
                    runTask.Wait(cancellationToken);
                    runner.CollectAndSave();
                return;
            }
            Console.WriteLine(options.GetHelpText());
        }

        private static void MusicBrainzAdapter_CheckProgress(object sender, CheckMatchingProgressEventArgs e)
        {
            var previous = Console.ForegroundColor;
            switch (e.ExecutionStatus)
            {
                case ExecutionStatus.Found:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ExecutionStatus.NotFound:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
               case ExecutionStatus.ExecutionError:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine("{0} - {1}",e.File,e.QueryType);
            Console.ForegroundColor = previous;
        }
    }
}
