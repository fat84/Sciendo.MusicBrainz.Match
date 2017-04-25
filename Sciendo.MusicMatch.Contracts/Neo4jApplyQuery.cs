using System.Xml;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class Neo4jApplyQuery
    {
        [XmlText]
        public string DebugQuery { get; set; }

        public ApplyStatus ApplyStatus { get; set; }
    }
}