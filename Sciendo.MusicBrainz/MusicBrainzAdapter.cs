using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;
using Sciendo.Common.Serialization;
using Sciendo.FilesAnalyser;
using Sciendo.MusicMatch.Contracts;

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
            if(filesAnalysed==null)
                throw new ArgumentNullException(nameof(filesAnalysed));

            foreach (var fileAnalysed in filesAnalysed)
            {
                var sanitizedFileAnalysed = Sanitize(fileAnalysed);
                if (!LinkOneToExisting(sanitizedFileAnalysed))
                    CreateANewOne(sanitizedFileAnalysed);
            }
        }

        private FileAnalysed Sanitize(FileAnalysed fileAnalysed)
        {
            fileAnalysed.Title = Sanitize(fileAnalysed.Title);
            fileAnalysed.Album = Sanitize(fileAnalysed.Album);
            fileAnalysed.Artist = Sanitize(fileAnalysed.Artist);

            fileAnalysed.FilePath = HttpUtility.HtmlDecode(fileAnalysed.FilePath).ToLower().Replace(@"\", "/");

            return fileAnalysed;
        }

        private string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return HttpUtility.HtmlDecode(input)
                .ToLower()
                .Replace("\"", ".?")
                .Replace(@"\", ".?")
                .Replace(@"'", ".?")
                .Replace("(", ".?")
                .Replace(")", ".?")
                .Replace("[", ".?")
                .Replace("]", ".?")
                .Replace("...", "…")
                .Replace("`", ".?");
        }

        private void CreateANewOne(FileAnalysed fileAnalysed)
        {
            //there might be a multitude of reasons for which we are here
            //Create the Artist if it doesn't exist
            //merge (a:Artist{name:"Madonna1"}) return a
            _graphClient.Cypher.Merge("(a:Artist{name:'" + fileAnalysed.Artist+ "'})").ExecuteWithoutResults();
            //Create the Album structure and link it to the artist if it doesn't exist
            //match (a:Artist{name:"Madonna1"}) 
            //merge (a)-[r:CREDITED_AS{year:""}]-(b:ArtistCredit{name:"Madonna1"})-[c:CREDITED_ON]-(d:Album{name:"True Blue1"})-[e:PART_OF]-
            //(f:Release{name:"True Blue1"})-[g:RELEASED_ON]-(h:Cd{position:"0"}) return a,r,b,c,d,e,f,g,h
            _graphClient.Cypher.Match("(a:Artist{name:'" + fileAnalysed.Artist + "'})")
                .Merge("(a)-[r:CREDITED_AS]-(b:ArtistCredit{name:'" + fileAnalysed.Artist +
                       "'})-[c:CREDITED_ON]-(d:Album{name:'" + fileAnalysed.Album + "'})-[e:PART_OF]-(f:Release{name:'" +
                       fileAnalysed.Album + "'})-[g:RELEASED_ON_MEDIUM]-(h:Cd{position:'0',name:'" + fileAnalysed.Album + "'})").ExecuteWithoutResults();
            //Create the track if it doeesn't exist
            //match (a:ArtistCredit{name:"Madonna1"})-[*..3]-(h:Cd{name:"True Blue1"})
            //merge(h) < -[p: APPEARS_ON] - (t:Track{ name: "OPEN YOUR HEART1"})
            //on create set t.mbid="myownid"
            //return t
            _graphClient.Cypher.Match("(a:ArtistCredit{name:'"+fileAnalysed.Artist+"'})-[*..3]-(h:Cd{name:'"+fileAnalysed.Album+"'})")
                .Merge("(h) <-[p: APPEARS_ON]- (t:Track{ name: '"+fileAnalysed.Title+"'})")
                .OnCreate()
                .Set("t.mbid='"+ Guid.NewGuid().ToString() + "'")
                .ExecuteWithoutResults();
            //and finally this should never not exist but just in case
            //add the localTrack
            //-[lp: HAVE_LOCAL] - (l:localTrack{ name: "mypath/mypath/myfile1.mp3"}) return h,p,t, lp, l
            _graphClient.Cypher.Match("(t:Track{name:'"+fileAnalysed.Title+"'})")
                .Merge("(t)-[lp: HAVE_LOCAL]-(l:localTrack{ name: '" + fileAnalysed.FilePath + "'})")
                .ExecuteWithoutResults();
            
        }

        //for any album
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
        private bool LinkOneToExisting(FileAnalysed fileAnalysed)
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

            var results = _graphClient.Cypher.Match(
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
                .Merge("(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:'" + fileAnalysed.FilePath + "'})")
                .Return(l => l.As<LocalTrack>())
                .Query;

            //return (results > 0);
            return true;
        }

        private void LinkToExistinginCollection(FileAnalysed fileAnalysed)
        {
            throw new NotImplementedException();
        }

        public void CreateNew(IEnumerable<FileAnalysed> fileAnalysed)
        {
            throw new NotImplementedException();
        }

        //MATCH (ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m:Cd)-[:APPEARS_ON]-(t:Track)
        //WHERE a.name=~"(?ui).*True Blue.*"
        //and t.name=~"(?ui).*Open Your Heart.*"
        //and ac.name=~"(?ui).*Madonna.*"//
        //Return t.mbid
        //limit 1
        public FileAnalysed Check(FileAnalysed fileAnalysed)
        {
            var sanitizedFileAnalysed = Sanitize(fileAnalysed);
            var query = _graphClient.Cypher.Match(
                    "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m:Cd)-[:APPEARS_ON]-(t:Track)")
                .Where("a.name=~{albumName}")
                .OrWhere("a.disambiguation=~{albumName}")
                .WithParam("albumName", "(?ui).*" + sanitizedFileAnalysed.Album + ".*")
                .AndWhere("t.name=~{title}")
                .WithParam("title", "(?ui).*" + sanitizedFileAnalysed.Title + ".*")
                .AndWhere("ac.name=~{artistName}")
                .WithParam("artistName", "(?ui).*" + sanitizedFileAnalysed.Artist + ".*")
                .Return(t => t.As<MBEntry>()).Query;
            fileAnalysed.Neo4JMatchingQuery = query.DebugQueryText;
            if (fileAnalysed.Id3TagIncomplete)
            {
                CheckProgress?.Invoke(this, new CheckProgressEventArgs(fileAnalysed.FilePath,MatchStatus.ErrorMatching));
                fileAnalysed.FixSuggestion = "Complete the ID3 Tag.";
                return fileAnalysed;
            }
            MBEntry result;
            try
            {
                result =
                    _graphClient.Cypher.Match(
                            "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m:Cd)-[:APPEARS_ON]-(t:Track)")
                        .Where("a.name=~{albumName}")
                        .WithParam("albumName", "(?ui).*" + sanitizedFileAnalysed.Album + ".*")
                        .AndWhere("t.name=~{title}")
                        .WithParam("title", "(?ui).*" + sanitizedFileAnalysed.Title + ".*")
                        .AndWhere("ac.name=~{artistName}")
                        .WithParam("artistName", "(?ui).*" + sanitizedFileAnalysed.Artist + ".*")
                        .Return(t => t.As<MBEntry>()).Results.FirstOrDefault();
                fileAnalysed.MbId = (result == null) ? Guid.Empty : new Guid(result.mbid);
                fileAnalysed.MatchStatus = (result == null) ? MatchStatus.UnMatched : MatchStatus.Matched;
                fileAnalysed.FixSuggestion = (result == null) ? "No suggestion" : "No Fix needed.";

                CheckProgress?.Invoke(this,
                result == null
                    ? new CheckProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.UnMatched)
                    : new CheckProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.Matched));

            }
            catch (Exception e)
            {
                fileAnalysed.MbId = Guid.Empty;
                fileAnalysed.MatchStatus = MatchStatus.ErrorMatching;
                CheckProgress?.Invoke(this,
                    new CheckProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.ErrorMatching));
            }
            return fileAnalysed;
        }

        public IEnumerable<FileAnalysed> CheckBulk(IEnumerable<FileAnalysed> filesAnalysed)
        {
            foreach (var fileAnalysed in filesAnalysed)
            {
                if (StopActivity)
                    break;
                yield return Check(fileAnalysed);
            }
        }

        public event EventHandler<CheckProgressEventArgs> CheckProgress;
        public bool StopActivity { get; set; }
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
