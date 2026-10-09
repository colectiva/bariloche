using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public class Tide
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("start")]
        public DateTime Start { get; set; }
        [JsonProperty("end")]
        public DateTime End { get; set; }
        [JsonProperty("value")]
        public double Value { get; set; }
        [JsonProperty("forecast")]
        public Boolean IsForecast { get; set; }
        [JsonProperty("lastRetrieved")]
        public DateTime LastRetrieved { get; set; }
        /* [JsonProperty("station")]
        public NetworkStation? Station { get; set; } //*/
        public Tide() { }
    }

    public class EmbeddedTides
    {
        [JsonProperty("tides")]
        public List<Tide> Tides { get; set; }
    }
}
