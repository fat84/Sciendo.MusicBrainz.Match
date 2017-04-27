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

namespace Sciendo.BulkMatchApplier
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
                musicBrainzAdapter.ApplyProgress += MusicBrainzAdapter_ApplyProgress;
                var runner = new RunnerMatchedApplyer(musicBrainzAdapter, options.Source, options.ApplyType);
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                cancellationToken.Register(runner.Stop);
                Task runTask = new Task(runner.Start, cancellationToken);
                runTask.Start();
                Console.ReadKey();
                cancellationTokenSource.Cancel();
                runTask.Wait(cancellationToken);
                runner.SaveTrace();
                return;
            }
            Console.WriteLine(options.GetHelpText());
        }

        private static void MusicBrainzAdapter_ApplyProgress(object sender, ApplyProgressEventArgs e)
        {
            var previous = Console.ForegroundColor;
            switch (e.ApplyStatus)
            {
                case ApplyStatus.ErrorApplying:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ApplyStatus.None:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.WriteLine(e.FilePath);
            Console.ForegroundColor = previous;
        }
    }
}
