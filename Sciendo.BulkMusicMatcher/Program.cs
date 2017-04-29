using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neo4jClient;
using Sciendo.MusicBrainz;
using Sciendo.MusicBrainz.Configuration;
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
                var connectionString =
        ((MusicBrainzConfigSection)ConfigurationManager.GetSection("musicBrainz"));
                GraphClient client = new GraphClient(new Uri(connectionString.Url), connectionString.UserName, connectionString.Password);
                client.Connect();
                MusicBrainzAdapter musicBrainzAdapter = new MusicBrainzAdapter(client);
                musicBrainzAdapter.CheckMatchingProgress += MusicBrainzAdapter_CheckProgress;
                var runner= new RunnerCollector(musicBrainzAdapter,options.Source,options.Matched, options.UnMatched,options.Matchingerrors,options.Append);
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
            switch (e.MatchStatus)
            {
                case ExecutionStatus.Found:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ExecutionStatus.FoundInCache:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case ExecutionStatus.NotFound:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
               case ExecutionStatus.ExecutionError:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine(e.File);
            Console.ForegroundColor = previous;
        }
    }
}
