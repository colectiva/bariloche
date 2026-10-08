using La_Colectiva.Models.Classic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Queries
{
    public class NextTripRequest
    {
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public TimeSpan Time { get; set; }
        public char Weekday { get; set; }
        public DateTime Date { get; set; }
    }


    public class TripPairDto
    {
        [JsonProperty("start")]
        public ScheduleDto Start { get; set; }
        [JsonProperty("end")]
        public ScheduleDto End { get; set; }

        public TripPairDto() { }
    }

    public class TripPair
    {
        public Schedule Start { get; set; }
        public Schedule End { get; set; }
        public TripPair() { }
        public TripPair(TripPairDto data) : this()
        {
            if (data != null)
            {
                this.Start = new Schedule(data.Start);
                this.End = new Schedule(data.End);
            }
        }
        public static TripPair Mapper(TripPairDto x)
        {
            return new TripPair(x);
        }
    }

    public class NextTripDto
    {
        [JsonProperty("start")]
        public ScheduleDto Start { get; set; }
        [JsonProperty("end")]
        public ScheduleDto End { get; set; }
        [JsonProperty("route")]
        public RouteDto Route { get; set; }
        [JsonProperty("status")]
        public RouteStatus Status { get; set; }
        [JsonProperty("operation")]
        public DailyOperationDto Operation { get; set; }
        [JsonProperty("originalSchedule")]
        public TripPairDto OriginalSchedule { get; set; }
        [JsonProperty("branch")]
        public BranchDto Branch { get; set; }

        public NextTripDto() { }
    }
    public class NextTrip
    {
        public Schedule Start { get; set; }
        public Schedule End { get; set; }
        public Route Route { get; set; }
        public RouteStatus Status { get; set; }
        public DailyOperation Operation { get; set; }
        public TripPair OriginalSchedule { get; set; }
        public Branch Branch { get; set; }
        public bool HasOriginalSchedule => OriginalSchedule != null;
        public bool HasOperation => Operation != null;
        public NextTrip() { }
        public NextTrip(NextTripDto data) : this()
        {
            if (data != null)
            {
                this.Start = new Schedule(data.Start);
                this.End = new Schedule(data.End);
                if (data.Route != null)
                {
                    this.Route = new Route(data.Route);
                }
                this.Status = data.Status;
                if (data.Operation != null)
                {
                    this.Operation = DailyOperation.Mapper(data.Operation);
                }
                if (data.OriginalSchedule != null)
                {
                    this.OriginalSchedule = new TripPair(data.OriginalSchedule);
                }
                if (data.Branch != null)
                {
                    this.Branch = new Branch(data.Branch);
                }
            }
        }

        public static NextTrip Mapper(NextTripDto x)
        {
            return new NextTrip(x);
        }

    }
}
