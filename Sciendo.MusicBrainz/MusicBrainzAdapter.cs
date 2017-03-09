using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.FilesAnalyser;

namespace Sciendo.MusicBrainz
{
    public class MusicBrainzAdapter:IMusicBrainzAdapter
    {
        private readonly GraphClient _graphClient;

        public MusicBrainzAdapter(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }
        public void LinkToExisting(IEnumerable<FileAnalysed> filesAnalysed)
        {

            foreach (var fileAnalysed in filesAnalysed)
            {
                if (fileAnalysed.MarkedAsPartOfCollection)
                    LinkToExistinginCollection(fileAnalysed);
                else
                {
                    LinkToExistingNotCollection(fileAnalysed);
                }
            }
        }

        //for a non-collection album
        //first check to see if the local path has not been already used and delete the old 
        //localTrack. This assumes that the collection does not contain duplicate keys
        //match(l:localTrack) where l.name="myfolderpath\\myfilepath.mp3" return l
        //if not also check to see if the current track is not linked with other localTrack
        //if found deleted
        //mmatch (a:Album)-[r:PART_OF]-(b)-[r1:RELEASED_ON_MEDIUM]-(m:Cd)<-[*..3]-(ac:ArtistCredit) 
        //where a.name=~"(?i)TRUE BLUE" and ac.name=~"(?i)Madonna" 
        //with a, m, ac limit 1 match(m)<-[p: APPEARS_ON]-(t:Track)-[lp]-(l:localTrack) 
        //where t.name=~"(?i)OPEN YOUR HEART" detach delete l
        //after that go to create another one
        //match(a:Album)-[r: PART_OF]-(b)-[r1: RELEASED_ON_MEDIUM]-(m:Cd)<-[*..3]-(ac:ArtistCredit) where a.name=~"(?i)10CC" and ac.name=~"(?i)10CC" 
        //with a, m, ac limit 1 match(m)<-[p: APPEARS_ON]-(t:Track) where t.name=~"(?i)DONNA" 
        //MERGE(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:'c:\\my path\\donna.mp3'})
        private void LinkToExistingNotCollection(FileAnalysed fileAnalysed)
        {
            _graphClient.Cypher.Match("(l:localTrack)")
                .Where("l.name={filePath}")
                .WithParam("filePath", fileAnalysed.FilePath.Replace(@"\", @"/"))
                .DetachDelete("l").ExecuteWithoutResults();

            _graphClient.Cypher.Match(
                     "(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m:Cd)<-[*..3]-(ac:ArtistCredit)")
                 .Where("a.name=~{albumName}")
                 .WithParam("albumName", "(?ui).*" + fileAnalysed.Album + ".*")
                 .AndWhere("ac.name=~{artistName}")
                 .WithParam("artistName", "(?ui).*" + fileAnalysed.Artist + ".*")
                 .With("a,m,ac")
                 .Limit(1)
                 .Match("(m)<-[:APPEARS_ON]-(t:Track)-[lp]-(l:localTrack)")
                 .Where("t.name=~{trackName}")
                 .WithParam("trackName", "(?ui).*" + fileAnalysed.Title + ".*")
                 .DetachDelete("l").ExecuteWithoutResults();

            _graphClient.Cypher.Match(
                    "(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m:Cd)<-[*..3]-(ac:ArtistCredit)")
                .Where("a.name=~{albumName}")
                .WithParam("albumName", "(?ui).*" + fileAnalysed.Album + ".*")
                .AndWhere("ac.name=~{artistName}")
                .WithParam("artistName", "(?ui).*" + fileAnalysed.Artist + ".*")
                .With("a,m,ac")
                .Limit(1)
                .Match("(m)<-[:APPEARS_ON]-(t:Track)")
                .Where("t.name=~{trackName}")
                .WithParam("trackName", "(?ui).*" + fileAnalysed.Title + ".*")
                
                //.Merge("(t)-[lr:HAVE_LOCAL]-(l:localTrack)");
                .Merge("(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:'" + fileAnalysed.FilePath.Replace(@"\",@"/") + "'})").ExecuteWithoutResults();
        }

        private void LinkToExistinginCollection(FileAnalysed fileAnalysed)
        {
            throw new NotImplementedException();
        }

        public void CreateNew(IEnumerable<FileAnalysed> fileAnalysed)
        {
            throw new NotImplementedException();
        }
    }

    internal class LocalTrack
    {
        public string name { get; set; }
//        public string id { get; set; }
    }

    public class MBRecord
    {
        public string length { get; set; }
        
        public string position { get; set; }
    }
    public class MBEntry:MBRecord
    {
        public string mbid { get; set; }

        public string disambiguation { get; set; }

        public string name { get; set; }
    }

    public class MBArtistCredit:MBRecord
    {
        public string name { get; set; }
    }
}
