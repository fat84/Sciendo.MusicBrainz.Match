using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sciendo.FilesAnalyser;

namespace Sciendo.MusicBrainz.Match.Tests
{
    [TestFixture]
    public class MusicBrainzAdapterTests
    {
        [Test]
        public void Neo4JTest()
        {
            //for a non-collection album
            //match(a:Album)-[r: PART_OF]-(b)-[r1: RELEASED_ON_MEDIUM]-(m:Cd)<-[*..3]-(ac:ArtistCredit) where a.name=~"(?i)10CC" and ac.name=~"(?i)10CC" 
            //with a, m, ac limit 1 match(m)<-[p: APPEARS_ON]-(t:Track) where t.name=~"(?i)DONNA" 
            //MERGE(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:'c:\\my path\\donna.mp3'})
            GraphClient client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "c0nc0rd3");
            client.Connect();

            MusicBrainzAdapter musicBrainzAdapter=new MusicBrainzAdapter(client);

            musicBrainzAdapter.LinkToExisting(new FileAnalysed[]
            {
                new FileAnalysed
                {
                    Artist = "MAdonna",
                    Album = "true Blue",
                    FilePath = @"myfolderpath\myfilepath.mp3",
                    Title = "Open your heart"
                }
            });

        }
    }
}
