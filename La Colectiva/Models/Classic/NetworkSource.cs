using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public class NetworkSource
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class EmbeddedNetworkSource
    {
        [JsonProperty("networkSources")]
        public List<NetworkSource> NetworkSources { get; set; }
    }
}
