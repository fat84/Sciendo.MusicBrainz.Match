using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
            var connectionString =
    ((MusicBrainzConfigSection)ConfigurationManager.GetSection("musicBrainz"));
            GraphClient client = new GraphClient(new Uri(connectionString.Url),connectionString.UserName,connectionString.Password);
            client.Connect();
            MusicBrainzAdapter musicBrainzAdapter= new MusicBrainzAdapter(client);
            musicBrainzAdapter.CheckProgress += MusicBrainzAdapter_CheckProgress;
            if(args.Length==3)
                musicBrainzAdapter.CheckBulkAndSplit(args[0], args[1], args[2]);
            else
            {
                musicBrainzAdapter.CheckBulk(args[0], args[1]);
            }
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
