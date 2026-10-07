using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : value.ToString();
        }
    }

    public enum ViaCategory
    {
        [Description("Río")] RIVER,
        [Description("Canal")] CANAL,
        [Description("Arroyo")] STREAM,
        [Description("Camino de sirga")] WALKWAY,
        [Description("Puente")] BRIDGE
    }

    public class ViaDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ViaCategory Category { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("deletedAt")]
        public DateTime? DeletedAt { get; set; }

        public ViaDto() { }

    }

    public class Via
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ViaCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Via(ViaDto dto)
        {
            if (dto != null)
            {
                this.Id = dto.Id;
                this.Name = dto.Name;
                this.Category = dto.Category;
                this.CreatedAt = dto.CreatedAt;
                this.UpdatedAt = dto.UpdatedAt;
                this.DeletedAt = dto.DeletedAt;
            }
        }

        [JsonIgnore]
        public bool IsNavigable {
            get {
                return
                    this.Category.Equals(ViaCategory.RIVER) ||
                    this.Category.Equals(ViaCategory.CANAL) ||
                    this.Category.Equals(ViaCategory.STREAM);
            }
        }

        [JsonIgnore]
        public bool IsWalkable {
            get {
                return
                    this.Category.Equals(ViaCategory.WALKWAY) ||
                    this.Category.Equals(ViaCategory.BRIDGE);
            }
        }

        public string PrettyName {
            get {
                return $"{Category.GetDescription()} {Name}";
            }
        }

        public string PrettyCategory {
            get {
                return $"{Category.GetDescription()}";
            }
        }

    }

    public class ViaCreationRequest
    {
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ViaCategory Category { get; set; }
    }

    public class EmbeddedVias
    {
        public List<ViaDto> Vias { get; set; }
    }
}
