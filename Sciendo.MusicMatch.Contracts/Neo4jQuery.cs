using System.Xml;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class Neo4jQuery
    {
        [XmlText]
        public string DebugQuery { get; set; }

        [XmlAttribute]
        public ExecutionStatus ExecutionStatus { get; set; }

        [XmlAttribute]
        public QueryType QueryType { get; set; }
    }
}