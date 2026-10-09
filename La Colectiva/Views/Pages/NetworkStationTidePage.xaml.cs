using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Classic;
using La_Colectiva.Services.Local;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace La_Colectiva.Views.Pages
{
    public sealed partial class NetworkStationTidePage : Page
    {
        private readonly TideService tideService = App.Current.Services.GetRequiredService<TideService>();
        private readonly TileService tileService = App.Current.Services.GetRequiredService<TileService>();


        private DateTime _start = DateTime.Now;
        public DateTime Start {
            get { return _start;  }
            set {
                _start = value;
            }
        }

        private DateTime _end = DateTime.Now.AddDays(2);
        public DateTime End {
            get { return _end; }
            set {
                _end = value;
            }
        }

        public NetworkStation Station { get; set; }

        public ObservableCollection<Tide> ObservedData = new ObservableCollection<Tide>();
        public ObservableCollection<Tide> Forecast = new ObservableCollection<Tide>();

        public NetworkStationTidePage() {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            

            if (e.Parameter is NetworkStation station) {
                Station = station;
                this.DataContext = this;
                LoadObservedData();
                LoadForecast();
            }
        }


        public async void LoadObservedData() {
            if (Station == null) return;
            await tideService
                .TopData(Station.Id)
                .Ok(data => {
                    ObservedData.Clear();
                    foreach (var tide in data.OrderBy(x => x.Start)) {
                        ObservedData.Add(tide);
                    }
                })
                .RunAsync();
        }

        public async void LoadForecast() {
            if (Station == null) return;
            await tideService
                .Forecast(Station.Id, Start.AddHours(-6), End)
                .Ok(data => {
                    Forecast.Clear();
                    foreach (var tide in data.OrderBy(x => x.Start)) {
                        Forecast.Add(tide);
                    }
                })
                .RunAsync();
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Station == null) return;
            LoadObservedData();
            LoadForecast();
        }

        private void ShowInStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Station == null) return;
            tileService.UpdatePrimaryTile(Station);
        }
    }
}
