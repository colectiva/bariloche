using La_Colectiva.Models.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public class ScheduleDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("time")]
        public TimeSpan Time { get; set; }
        [JsonProperty("route")]
        public RouteDto Route { get; set; }
        [JsonProperty("dock")]
        public DockDto Dock { get; set; }

        public ScheduleDto() { }
    }

    public class Schedule
    {
        public int Id { get; set; }
        public TimeSpan Time { get; set; }
        public Route Route { get; set; }
        public Dock Dock { get; set; }

        public Schedule() { }
        public Schedule(ScheduleDto data)
        {
            if (data != null)
            {
                this.Id = data.Id;
                this.Time = data.Time;
                if (data.Route != null)
                {
                    this.Route = new Route(data.Route);
                }
                if (data.Dock != null)
                {
                    this.Dock = Dock.Create(data.Dock);
                }
            }
        }
        public static Schedule Mapper(ScheduleDto data)
        {
            return new Schedule(data);
        }

    }

    public class EmbeddedSchedules : IEmbedded<ScheduleDto>
    {
        [JsonProperty("schedules")]
        public List<ScheduleDto> Items { get; set; }
    }
}
