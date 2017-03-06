using System.Configuration;

namespace Sciendo.FilesAnalyser.Configuration
{
    public class AnalyserConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("collectionPaths")]
        public CollectionPathElementCollection CollectionPaths
        {
            get { return (CollectionPathElementCollection)this["collectionPaths"]; }
        }

        [ConfigurationProperty("collectionMarker", DefaultValue = "", IsRequired = true)]
        public string CollectionMarker
        {
            get { return (string)this["collectionMarker"]; }
            set { this["collectionMarker"] = value; }
        }

    }

    [ConfigurationCollection(typeof(CollectionPathElement))]
    public class CollectionPathElementCollection : ConfigurationElementCollection
    {
        public CollectionPathElement this[int index]
        {
            get { return (CollectionPathElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectionPathElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionPathElement)element).Key;
        }
    }

    public class CollectionPathElement : ConfigurationElement
    {
        public CollectionPathElement() { }

        [ConfigurationProperty("key", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", DefaultValue = "", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

    }
}
