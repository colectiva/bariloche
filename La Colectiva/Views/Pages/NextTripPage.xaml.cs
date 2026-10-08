using La_Colectiva.Models.Classic;
using La_Colectiva.Models.Queries;
using La_Colectiva.Services.Queries;
using La_Colectiva.ViewModels.Design;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace La_Colectiva.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class NextTripPage : Page
    {
        private Dock _origin;
        private Dock _destination;
        private readonly TripService _tripService;

        // Propiedades de fecha y hora inicializadas con las actuales
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public TimeSpan Time { get; set; } = DateTime.Now.Date.TimeOfDay;
        

        public NextTripPage()
        {
            InitializeComponent();

            // Instanciar tu servicio (inyectado o manual según tu app)
            _tripService = App.Current.Services.GetRequiredService<TripService>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Suponiendo que pasas ambos muelles al navegar, ej: 
            // Frame.Navigate(typeof(NextTripPage), new Tuple<Dock, Dock>(dockFrom, dockTo));
            if (e.Parameter is Tuple<Dock, Dock> docks)
            {
                _origin = docks.Item1;
                _destination = docks.Item2;

                await EjecutarBusqueda();
            }
        }

        private async Task EjecutarBusqueda()
        {
            var request = new NextTripRequest
            {
                OriginId = _origin.Id,
                DestinationId = _destination.Id,
                Date = Date,
                Time = Time,
                Weekday = GetWeekday(Date.DayOfWeek)
            };
            
            await _tripService
                .FindNextTrip(request)
                .Ok(result =>
                {
                    LstTrips.ItemsSource = result;
                })
                .RunAsync();

        }

        private char GetWeekday(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return 'L';
                case DayOfWeek.Tuesday: return 'M';
                case DayOfWeek.Wednesday: return 'X';
                case DayOfWeek.Thursday: return 'J';
                case DayOfWeek.Friday: return 'V';
                case DayOfWeek.Saturday: return 'S';
                case DayOfWeek.Sunday: return 'D';
                default: return 'L';
            }
        }

        private void LstTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
