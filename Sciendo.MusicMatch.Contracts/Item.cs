using System;
using System.Xml.Serialization;

namespace Sciendo.MusicMatch.Contracts
{
    public class Item
    {
        public static string NoFixSuggestion = "No Suggestion";
        public Item()
        {
            Name = string.Empty;
            Id= Guid.Empty;
            FixSuggestion = NoFixSuggestion;
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public Guid Id { get; set; }

        public string FixSuggestion { get; set; }
    }
}