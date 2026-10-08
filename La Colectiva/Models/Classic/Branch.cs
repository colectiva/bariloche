using La_Colectiva.Models.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public class BranchDto
    {
        [JsonProperty("availability")]
        public string Availability { get; set; }
        [JsonProperty("company")]
        public Company Company { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("deletedAt")]
        public DateTime? DeletedAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        [JsonProperty("departureDock")]
        public DockDto DepartureDock { get; set; }
        [JsonProperty("departureTime")]
        public TimeSpan DepartureTime { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("routes")]
        public List<RouteDto> Routes { get; set; }
        public BranchDto() { }
    }
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Availability Availability { get; set; }
        public Company Company { get; set; }
        public Dock DepartureDock { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<Route> Routes { get; set; }
        [JsonIgnore]
        public string PrettyPrint {
            get {
                return $"[{Id}] {Name} de las {DepartureTime} ({Company.Name})";
            }
        }
        public Branch()
        {

        }
        public Branch(BranchDto dto)
        {
            this.Id = dto.Id;
            this.Name = dto.Name;
            this.Availability = new Availability(dto.Availability);
            this.Company = dto.Company;
            this.DepartureDock = Dock.Create(dto.DepartureDock);
            this.DepartureTime = dto.DepartureTime;
            this.CreatedAt = dto.CreatedAt;
            this.UpdatedAt = dto.UpdatedAt;
            this.DeletedAt = dto.DeletedAt;
            if (dto.Routes != null)
            {
                this.Routes = new List<Route>();
                foreach (var item in dto.Routes)
                {
                    this.Routes.Add(new Route(item));
                }
            }
        }

        public static Branch Mapper(BranchDto dto)
        {
            return new Branch(dto);
        }
    }


    public class EmbeddedBranch : IEmbedded<BranchDto>
    {
        [JsonProperty("branches")]
        public List<BranchDto> Items { get; set; }
    }
}
