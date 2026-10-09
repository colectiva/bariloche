using La_Colectiva.Core;
using La_Colectiva.Models.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DockType
    {
        PUBLIC,
        PRIVATE,
        COMMUNITY,
        BUSINESS,
        REFERENCE
    }
    
    [Table("BookmarkDocks")]
    public class BookmarkDockDto
    {
        [PrimaryKey]
        public int DockId { get; set; }
        public DateTime BookmarkedAt { get; set; } = DateTime.UtcNow;

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public DockDto Dock { get; set; }
    }

    [Table("Docks")]
    public class DockDto
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(DoubleOrStringConverter))]
        public double Latitude { get; set; }
        [JsonConverter(typeof(DoubleOrStringConverter))]
        public double Longitude { get; set; }

        [JsonConverter(typeof(DoubleOrStringConverter))]
        [JsonProperty("distance")]
        public double? Distance { get; set; }

        //[JsonIgnore]
        [JsonProperty("viaId")]
        [ForeignKey(typeof(ViaDto))]
        public int ViaId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        [JsonProperty("via")]
        public ViaDto Via { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("type")]
        public DockType Type { get; set; }
        [JsonIgnore]
        public DateTime SavedAt { get; set; }
        [Ignore]
        public bool IsBookmarked { get; set; }

    }

    public class Dock
    {

        public string BuildURI(string baseUrl)
        {
            return $"{baseUrl.TrimEnd('/', '\\')}/docks/${Id}";
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Via Via { get; set; }
        public Distance Distance { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        public DockType Type { get; set; }
        public DateTime SavedAt { get; set; }
        [Ignore]
        public bool IsBookmarked { get; set; }

        public string PrettyDistance {
            get {
                if(Distance != null)
                {
                    return $"{Distance.ToString()}";
                }
                return "";
            }
        }


        public Dock() { }

        public static Dock Create(DockDto raw)
        {
            return new Dock
            {
                Id = raw.Id,
                Name = raw.Name,
                Latitude = raw.Latitude, // Culture.from(raw.Latitude),
                Longitude = raw.Longitude, // Culture.from(raw.Longitude),
                Via = new Via(raw.Via),
                Distance = new Distance(raw.Distance),
                UpdatedAt = raw.UpdatedAt,
                Type = raw.Type,
                SavedAt = raw.SavedAt,
                IsBookmarked = raw.IsBookmarked
            };
        }
    }

    public class DockCreationRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("via")]
        public string Via { get; set; }
        [JsonProperty("type")]
        public DockType Type { get; set; }

        public DockCreationRequest() { }
    }

    public class DockUpdateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("viaId")]
        public int ViaId { get; set; }
        [JsonProperty("type")]
        public DockType Type { get; set; }

        public DockUpdateRequest() { }
    }


    public class EmbeddedDocks : IEmbedded<DockDto>
    {
        [JsonProperty("docks")]
        public List<DockDto> Items { get; set; }
    }
}
