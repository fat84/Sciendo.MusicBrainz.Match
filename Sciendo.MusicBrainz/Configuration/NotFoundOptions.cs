using System.Configuration;

namespace Sciendo.MusicBrainz.Configuration
{
    public class NotFoundOptions:ConfigurationElement
    {
        [ConfigurationProperty("createArtist", DefaultValue = false, IsRequired = true)]
        public bool CreateArtist
        {
            get { return (bool) this["createArtist"]; }
            set { this["createArtist"] = value; }
        }

        [ConfigurationProperty("createAlbum", DefaultValue = false, IsRequired = true)]
        public bool CreateAlbum
        {
            get { return (bool)this["createAlbum"]; }
            set { this["createAlbum"] = value; }
        }

        [ConfigurationProperty("createTitle", DefaultValue = false, IsRequired = true)]
        public bool CreateTitle
        {
            get { return (bool)this["createTitle"]; }
            set { this["createTitle"] = value; }
        }
    }
}