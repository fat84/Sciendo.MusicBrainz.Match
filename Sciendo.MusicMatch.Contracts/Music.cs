using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class Music:MusicBase
    {
        public Music()
        {
            TagAnalysis=new TagAnalysis();
        }

        [XmlAttribute]
        public long Id { get; set; }
        public uint Track { get; set; }
        public MatchStatus MatchStatus { get; set; }
        public TagAnalysis TagAnalysis { get; set; }
        [XmlText]
        public string Neo4JMatchingQuery { get; set; }

        public List<Neo4jQuery> Neo4jApplyQuerries { get; set; } 
    }
}
