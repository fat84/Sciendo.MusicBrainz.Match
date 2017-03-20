using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicBrainz.Configuration
{
    public class MusicBrainzConfigSection:ConfigurationSection
    {
            [ConfigurationProperty("url", DefaultValue = "http://localhost:7474/db/dat", IsRequired = true)]
            public string Url
            {
                get { return (string)this["url"]; }
                set { this["url"] = value; }
            }

        [ConfigurationProperty("userName", DefaultValue = "neo4j", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("password", DefaultValue = "neo4j", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

    }
}
