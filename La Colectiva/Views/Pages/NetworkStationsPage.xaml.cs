using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Classic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
    public sealed partial class NetworkStationsPage : Page
    {
        private readonly NetworkStationService networkStationService = App.Current.Services.GetRequiredService<NetworkStationService>();
        public Paginator<NetworkStation> Paginator = new Paginator<NetworkStation>();

        public bool HasNext => Paginator.HasNext;

        public NetworkStationsPage() {
            this.InitializeComponent();
            this.Paginator
                .Append()
                .PageSize(10)
                .Loader(async (p, s) =>
                {
                    var _page = new Page<NetworkStation>();
                    await networkStationService
                        .SearchSync(p, s)
                        .Ok(pp =>
                        {
                            _page = pp;
                        })
                        .RunAsync();
                    return _page;
                });
            lstStations.ItemsSource = this.Paginator.Items;
            Refresh();
        }

        public async void Refresh()
        {
            this.Paginator.Clear();
            await this.Paginator.First();
        }

        public async void LoadMore()
        {
            await this.Paginator.Next();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void TidePageOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (lstStations.SelectedItem is NetworkStation station)
            {
                this.Frame.Navigate(typeof(NetworkStationTidePage), station);
            }
        }

        private void LstStations_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(lstStations.SelectedItem is NetworkStation station) {
                TidePageOpenBtn.Visibility = Visibility.Visible;
            } else {
                TidePageOpenBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
