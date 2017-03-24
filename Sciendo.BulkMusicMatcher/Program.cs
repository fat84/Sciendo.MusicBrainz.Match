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
                musicBrainzAdapter.CheckProgress += MusicBrainzAdapter_CheckProgress;
                var runner= new Runner(musicBrainzAdapter,options.Source,options.Matched, options.UnMatched,options.Append);
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                cancellationToken.Register(runner.Stop);
                Task runTask = new Task(runner.Start,cancellationToken);
                runTask.Start();
                Console.ReadKey();
                try
                {
                    cancellationTokenSource.Cancel();
                    runTask.Wait(cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    runner.CollectAndSave();
                }
                return;
            }
            Console.WriteLine(options.GetHelpText());
        }

        private static void MusicBrainzAdapter_CheckProgress(object sender, CheckProgressEventArgs e)
        {
            var previous = Console.ForegroundColor;
            if (e.Matched)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(e.File);
            Console.ForegroundColor = previous;
        }
    }
}
