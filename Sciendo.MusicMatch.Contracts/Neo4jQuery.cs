using System.Xml;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class Neo4jQuery
    {
        [XmlText]
        public string DebugQuery { get; set; }

        public ApplyStatus ApplyStatus { get; set; }
    }
}