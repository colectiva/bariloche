using La_Colectiva.Core;
using La_Colectiva.Models.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public class NetworkStation
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("river")]
        public string River { get; set; }
        [JsonProperty("warningLevel")]
        public double? WarningLevel { get; set; }
        [JsonProperty("evacuationLevel")]
        public double? EvacuationLevel { get; set; }
        [JsonProperty("lowTideLevel")]
        public double? LowTideLevel { get; set; }
        [JsonProperty("source")]
        public NetworkSource Source { get; set; }
        [JsonProperty("lastTide")]
        public Tide LastTide { get; set; }


        [JsonIgnore]
        public string LowTideLevelText => LowTideLevel == null ? "—" : $"{LowTideLevel} mts";
        [JsonIgnore]
        public string WarningLevelText => WarningLevel == null ? "—" : $"{WarningLevel} mts";
        [JsonIgnore]
        public string EvacuationLevelText => EvacuationLevel == null ? "—" : $"{EvacuationLevel} mts";
        [JsonIgnore]
        public string SourceName => Source.Name;

        [JsonIgnore]
        public string Coordinates {
            get {
                return $"{Culture.prettyPrintLat(this.Latitude)} {Culture.prettyPrintLng(this.Longitude)}";
            }
        }



        [JsonIgnore]
        public string LastTideHeight =>
        (LastTide?.Value.ToString("0.00") ?? "—") + " mts";

        [JsonIgnore]
        public string LastTideTime {
            get {
                if (LastTide == null)
                    return "—";

                var date = LastTide.Start.ToLocalTime();

                if (date.Date == DateTime.Today)
                    return date.ToString("HH:mm");

                if (date.Date == DateTime.Today.AddDays(-1))
                    return $"Ayer, {date:HH:mm}";

                return date.ToString("dd/MM HH:mm");
            }
        }

        [JsonIgnore]
        public string LastTideFull => this.LastTide == null ? "—" : $"{LastTideHeight} a las {LastTideTime}";

        public NetworkStation() { }
    }

    public class EmbeddedNetworkStations : IEmbedded<NetworkStation>
    {
        [JsonProperty("networkStations")]
        public List<NetworkStation> Items { get; set; }
    }
}
