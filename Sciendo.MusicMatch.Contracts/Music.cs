using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class Music:MusicBase
    {
        public Music()
        {
            TagAnalysis=new TagAnalysis();
            Neo4jQuerries=new List<Neo4jQuery>();
        }

        [XmlAttribute]
        public long Id { get; set; }
        public uint Track { get; set; }
        public TagAnalysis TagAnalysis { get; set; }
        public List<Neo4jQuery> Neo4jQuerries { get; set; }

        public bool AllItemsMatched()
        {
            return Neo4jQuerries.All(q => q.ExecutionStatus == ExecutionStatus.Found || q.ExecutionStatus==ExecutionStatus.FoundInCache);
        }

        public bool AnyItemsWithErrors()
        {
            return Neo4jQuerries.Any(q => q.ExecutionStatus == ExecutionStatus.ExecutionError);
        }

        public bool AnyItemsWithUnMatched(QueryType queryType)
        {
            return Neo4jQuerries.Any(q => q.ExecutionStatus == ExecutionStatus.NotFound && q.QueryType==queryType);
        }
    }
}
