using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sciendo.FilesAnalyser;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Match.Tests
{
    [TestFixture]
//    [Ignore("Integration tests")]
    public class MusicBrainzAdapterTests
    {
        private GraphClient _client;
        [SetUp]
        public void Setup()
        {
            _client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "c0nc0rd3");
            _client.Connect();
        }

        [Test]
        public void Neo4JTestLinkLocalToExsiting()
        {
            MusicBrainzAdapter musicBrainzAdapter=new MusicBrainzAdapter(_client);

            musicBrainzAdapter.LinkToExisting(new FileAnalysed[]
            {
                new FileAnalysed
                {
                    Artist = "65daysofstatic",
                    Album = "The Fall Of Math",
                    FilePath = @"0-9\65 days of static\The Fall Of Math\65daysofstatic-another-code-against-the-gone.mp3",
                    Title = "another code against the gone"
                }
            });

        }
        [Test]
        public void Neo4JTestCreateAndLinkToLocalNoArtistNoAlbumNoTrack()
        {
            //for a non-collection album
            //match(a:Album)-[r: PART_OF]-(b)-[r1: RELEASED_ON_MEDIUM]-(m:Cd)<-[*..3]-(ac:ArtistCredit) where a.name=~"(?i)10CC" and ac.name=~"(?i)10CC" 
            //with a, m, ac limit 1 match(m)<-[p: APPEARS_ON]-(t:Track) where t.name=~"(?i)DONNA" 
            //MERGE(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:'c:\\my path\\donna.mp3'})

            MusicBrainzAdapter musicBrainzAdapter = new MusicBrainzAdapter(_client);

            musicBrainzAdapter.LinkToExisting(new FileAnalysed[]
            {
                new FileAnalysed
                {
                    Artist = "My New Artist1",
                    Album = "My New Album1",
                    FilePath = @"mycollection\folderpath\myfilepath.mp3",
                    Title = "My New Song1"
                }
            });

        }
    }
}
