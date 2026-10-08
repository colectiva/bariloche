using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperationStatus
    {
        CONFIRMED, DELAYED, CANCELLED
    }



    public class DailyOperationDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("forcedRoute")]
        public RouteDto ForcedRoute { get; set; }
        [JsonProperty("forcedStatus")]
        public OperationStatus? ForcedStatus { get; set; }
        [JsonProperty("operationDate")]
        public DateTime OperationDate { get; set; }
        [JsonProperty("manualMessage")]
        public string ManualMessage { get; set; } = "";

        public DailyOperationDto() { }
    }

    public class DailyOperation
    {
        public int Id { get; set; }
        public Route ForcedRoute { get; set; }
        public OperationStatus? ForcedStatus { get; set; }
        public DateTime OperationDate { get; set; }
        public string ManualMessage { get; set; } = "";

        public DailyOperation() { }

        public DailyOperation(DailyOperationDto dto)
        {
            if (dto != null)
            {
                this.Id = dto.Id;
                this.ForcedRoute = dto.ForcedRoute != null ? new Route(dto.ForcedRoute) : null;
                this.ForcedStatus = dto.ForcedStatus;
                this.OperationDate = dto.OperationDate;
                this.ManualMessage = dto.ManualMessage;
            }
        }

        public static DailyOperation Mapper(DailyOperationDto dto)
        {
            return new DailyOperation(dto);
        }
    }
}
