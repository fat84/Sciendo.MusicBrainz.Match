using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class TagAnalysis
    {
        public TagAnalysis()
        {
            MarkedAsPartOfCollection = false;
            InCollectionPath = false;
            PossiblePartOfACollection = false;
            Id3TagIncomplete = true;
        }

        [XmlAttribute]
        public bool MarkedAsPartOfCollection { get; set; }
        [XmlAttribute]
        public bool PossiblePartOfACollection { get; set; }
        [XmlAttribute]
        public bool InCollectionPath { get; set; }
        [XmlAttribute]
        public bool Id3TagIncomplete { get; set; }

    }
}
