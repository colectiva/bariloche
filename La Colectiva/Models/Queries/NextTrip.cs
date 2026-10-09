using La_Colectiva.Models.Classic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace La_Colectiva.Models.Queries
{

    public static class TripHelper
    {
        public static string Print(OperationStatus o)
        {
            switch(o)
            {
                case OperationStatus.CONFIRMED:
                    return "Confirmado";
                case OperationStatus.CANCELLED:
                    return "Cancelado";
                case OperationStatus.DELAYED:
                    return "Demorado";
                default:
                    return "";
            }
        }

        public static Brush Paint(OperationStatus o)
        {
            var x = "";
            switch (o)
            {
                case OperationStatus.CANCELLED:
                    x = "StatusCancelledBrush";
                    break;
                case OperationStatus.CONFIRMED:
                    x = "StatusConfirmedBrush";
                    break;
                case OperationStatus.DELAYED:
                    x = "StatusWarningBrush";
                    break;
                default:
                    x = "SystemControlForegroundBaseHighBrush";
                    break;


            }
            if (App.Current.Resources.TryGetValue(x, out object brush))
            {
                return (Brush)brush;
            }

            return (Brush)App.Current.Resources["SystemControlForegroundBaseHighBrush"];
        }

        public static Brush Paint(RouteStatus o)
        {
            var x = "";
            switch (o)
            {
                case RouteStatus.HIGH_TIDE:
                    x = "StatusWarningBrush";
                    break;
                case RouteStatus.LOW_TIDE:
                    x = "StatusWarningBrush";
                    break;
                case RouteStatus.OK:
                    x = "SystemControlForegroundBaseHighBrush";
                    break;
                case RouteStatus.UNKNOWN:
                    x = "SystemControlForegroundBaseHighBrush";
                    break;
                default:
                    x = "SystemControlForegroundBaseHighBrush";
                    break;


            }
            if (App.Current.Resources.TryGetValue(x, out object brush))
            {
                return (Brush)brush;
            }

            return (Brush)App.Current.Resources["SystemControlForegroundBaseHighBrush"];
        }
    
            public static string Print(RouteStatus o)
        {
            switch(o)
            {
                case RouteStatus.HIGH_TIDE:
                    return "Sujeto a marea alta";
                case RouteStatus.LOW_TIDE:
                    return "Sujeto a marea baja";
                case RouteStatus.OK:
                    return "";
                case RouteStatus.UNKNOWN:
                    return "";
                default:
                    return "";
            }
        }
    }

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
        [JsonIgnore]
        public bool HasOriginalSchedule => OriginalSchedule != null && OriginalSchedule?.Start != null && OriginalSchedule?.End != null;
        [JsonIgnore]
        public Visibility HasOriginalScheduleVisibility => HasOriginalSchedule ? Visibility.Visible : Visibility.Collapsed;
        [JsonIgnore]
        public bool HasOperation => Operation != null;

        // Texto de Horario Habitual ya formateado
        [JsonIgnore]
        public string OriginalScheduleText => HasOriginalSchedule
            ? $"Horario habitual: {OriginalSchedule.Start.Time.ToString(@"hh\:mm")} → {OriginalSchedule.End.Time.ToString(@"hh\:mm")}"
            : string.Empty;
        [JsonIgnore]
        public Brush OperationStatusBrush => Operation != null
            ? TripHelper.Paint(Operation.ForcedStatus ?? OperationStatus.CONFIRMED)
            : (TripHelper.Paint(Status));
        [JsonIgnore]
        public string OperationStatusText => Operation != null 
            ? TripHelper.Print(Operation.ForcedStatus??OperationStatus.CONFIRMED) 
            : ((Status != null && Status != RouteStatus.OK && Status != RouteStatus.UNKNOWN)
                ? TripHelper.Print(Status)
                : "");
        [JsonIgnore]
        public string OperationMessageText => (Operation != null && Operation.ManualMessage != null && Operation.ManualMessage.Trim().Length > 0) ? $" · {Operation.ManualMessage}" : string.Empty;



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
