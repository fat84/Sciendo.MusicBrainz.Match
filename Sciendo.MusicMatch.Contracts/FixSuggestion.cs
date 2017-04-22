using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class FixSuggestion
    {
        [XmlAttribute]
        public string FixAlbum { get; set; }
        [XmlAttribute]
        public string FixArtist { get; set; }
        [XmlAttribute]
        public string FixTitle { get; set; }
    }
}