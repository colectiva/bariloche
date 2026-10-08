using La_Colectiva.Models.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Classic
{
    public class Availability : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyAll()
        {
            OnPropertyChanged(nameof(Raw));
            OnPropertyChanged(nameof(Monday));
            OnPropertyChanged(nameof(Tuesday));
            OnPropertyChanged(nameof(Wednesday));
            OnPropertyChanged(nameof(Thursday));
            OnPropertyChanged(nameof(Friday));
            OnPropertyChanged(nameof(Saturday));
            OnPropertyChanged(nameof(Sunday));
            OnPropertyChanged(nameof(Holiday));
            OnPropertyChanged(nameof(WorkingDays));
            OnPropertyChanged(nameof(Weekend));
            OnPropertyChanged(nameof(WeekendsAndHolidays));
        }

        private string _raw = "";
        public string Raw {
            get => _raw;
            set {
                if (_raw != value)
                {
                    _raw = value ?? "";
                    NotifyAll();
                }
            }
        }

        private void add(char c)
        {
            if (Raw == null) Raw = "";
            Raw = Raw.Contains(c) ? Raw : Raw + c;
        }

        private void remove(char c)
        {
            if (Raw == null) Raw = "";
            Raw = Raw.Replace(c.ToString(), "");
        }

        private static char D = 'D';
        private static char L = 'L';
        private static char M = 'M';
        private static char X = 'X';
        private static char J = 'J';
        private static char V = 'V';
        private static char S = 'S';
        private static char F = 'F';

        public bool Sunday { get { return Raw.Contains(D); } set { if (value) add(D); else remove(D); } }
        public bool Monday { get { return Raw.Contains(L); } set { if (value) add(L); else remove(L); } }
        public bool Tuesday { get { return Raw.Contains(M); } set { if (value) add(M); else remove(M); } }
        public bool Wednesday { get { return Raw.Contains(X); } set { if (value) add(X); else remove(X); } }
        public bool Thursday { get { return Raw.Contains(J); } set { if (value) add(J); else remove(J); } }
        public bool Friday { get { return Raw.Contains(V); } set { if (value) add(V); else remove(V); } }
        public bool Saturday { get { return Raw.Contains(S); } set { if (value) add(S); else remove(S); } }
        public bool Holiday { get { return Raw.Contains(F); } set { if (value) add(F); else remove(F); } }

        public bool WorkingDays {
            get { return Monday && Tuesday && Wednesday && Thursday && Friday; }
            set { Monday = value; Tuesday = value; Wednesday = value; Thursday = value; Friday = value; }
        }

        public bool Weekend {
            get { return Saturday && Sunday; }
            set { Saturday = value; Sunday = value; }
        }

        public bool WeekendsAndHolidays {
            get { return Weekend && Holiday; }
            set { Weekend = value; Holiday = value; }
        }

        public Availability()
        {

        }

        public Availability(string raw)
        {
            Raw = raw;
        }

        public string ToString()
        {
            return Raw;
        }


    }
    public class RouteDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonIgnore]
        public string Availability { get; set; }
        [JsonProperty("branch")]
        public BranchDto Branch { get; set; }
        [JsonIgnore]
        public Company Company { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("original")]
        public Boolean Original { get; set; }

        public RouteDto() { }

    }

    public class Route
    {
        [JsonIgnore]
        public Uri Self => new Uri($"/api/routes/{Id}", UriKind.Relative);
        public Uri GetAbsoluteUri(Uri baseUri)
        {
            return new Uri(baseUri, Self);
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Branch Branch { get; set; }
        [Obsolete]
        [JsonIgnore]
        public Availability Availability { get; set; }
        [Obsolete]
        [JsonIgnore]
        public Company Company { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("original")]
        public Boolean Original { get; set; }

        public Route() { }

        public Route(RouteDto data)
        {
            Id = data.Id;
            Description = data.Description;
            //this.Availability = new Availability(data.Availability);
            // Company = data.Company;
            if (data.Branch != null) Branch = new Branch(data.Branch);
            CreatedAt = data.CreatedAt;
            UpdatedAt = data.UpdatedAt;
            if (data.Original != null)
            {
                this.Original = data.Original;
            }
            Name = data.Name;
        }

        public static Route Mapper(RouteDto data)
        {
            return new Route(data);
        }

    }

    public class EmbeddedRoute : IEmbedded<RouteDto>
    {
        [JsonProperty("routes")]
        public List<RouteDto> Items { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RouteStatus
    {
        OK,
        LOW_TIDE,
        HIGH_TIDE,
        UNKNOWN
    }
}
