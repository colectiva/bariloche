using La_Colectiva.Core;
using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Classic;
using La_Colectiva.Views.Dialogs;
using La_Colectiva.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace La_Colectiva
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly DockService _dockService;
        private bool _isDialogOpen = false;

        private Dock _from = null;
        public Dock From {
            get { return _from;  }
            set {
                _from = value;
                FromDockTextBox.Text = value == null ? "Ningún muelle seleccionado" : value.Name;
            }
        }

        private Dock _to = null;
        public Dock To {
            get { return _to; }
            set {
                _to = value;
                ToDockTextBox.Text = value == null ? "Ningún muelle seleccionado" : value.Name;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            
            _dockService = App.Current.Services.GetRequiredService<DockService>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async Task OpenDockSelectorAsync(Action<Dock> onSelected)
        {
            if (_isDialogOpen) return;
            _isDialogOpen = true;

            try
            {
                double lat = -34.4258;
                double lon = -58.5796;

                // Obtiene ubicación con máximo 3 segundos de espera
                var pos = await LocationHelper.GetCurrentLocationAsync(timeoutSeconds: 3);
                if (pos.HasValue)
                {
                    lat = pos.Value.Latitude;
                    lon = pos.Value.Longitude;
                }

                var dialog = new DockSelectorDialog(lat, lon);
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary && dialog.SelectedDock != null)
                {
                    onSelected(dialog.SelectedDock);
                }
            }
            finally
            {
                _isDialogOpen = false; // Libera el bloqueo al cerrar
            }
        }

        private async void ToDockBtn_Click(object sender, RoutedEventArgs e)
        {
            await OpenDockSelectorAsync(dock => To = dock);
        }

        private async void FromDockBtn_Click(object sender, RoutedEventArgs e)
        {
            await OpenDockSelectorAsync(dock => From = dock);
        }

        private void SearchTrips()
        {
            if (From == null || To == null)
            {
                return;
            }
            
            Frame.Navigate(typeof(NextTripPage), Tuple.Create(From, To));
        }

        private void SearchNextTrip_Click(object sender, RoutedEventArgs e)
        {
            SearchTrips();
        }
    }
}
