using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Classic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI;
using La_Colectiva.Models.Api;
using La_Colectiva.Services.Handlers;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace La_Colectiva.Views.Dialogs
{
    public sealed partial class DockSelectorDialog : ContentDialog
    {
        private readonly DockService _dockService;

        private Paginator<Dock> Paginator;

        public ObservableCollection<Dock> Docks { get; } = new ObservableCollection<Dock>();
        public Dock SelectedDock { get; private set; }
        public ObservableCollection<Dock> Favorites { get; } = new ObservableCollection<Dock>();
        private bool _favoritesLoaded = false;

        private readonly double _lat;
        private readonly double _lon;

        private string _q = "";
        public string Q {
            get { return _q; }
            set {
                _q = value;
            }
        }

        public DockSelectorDialog(double lat = -34.4258, double lon = -58.5796)
        {
            this.InitializeComponent();
            _dockService = App.Current.Services.GetRequiredService<DockService>();
            _lat = lat;
            _lon = lon;

            this.Paginator = new Paginator<Dock>()
                .PageSize(7)
                .Append()
                .Loader(async (_p, _s) => {
                    progressRing.IsActive = true;
                    btnLoadMore.IsEnabled = false;
                    var p = new Page<Dock>();
                    await _dockService
                        .Search(Q, _lat, _lon, _p, _s)
                        .Ok(page => {
                            p = page;
                        })
                        .FatalError(err => {
                            btnLoadMore.Visibility = Visibility.Collapsed;
                            ErrorHandler.Detailed()(err);
                        })
                        .RunAsync();
                    btnLoadMore.Visibility = p.Pageable.TotalPages > p.Pageable.Number ? Visibility.Visible : Visibility.Collapsed;
                    txtEmpty.Visibility = this.Paginator.Items.Count == 0 && p.Content.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                    progressRing.IsActive = false;
                    btnLoadMore.IsEnabled = true;
                    return p;
                });


            lstDocks.ItemsSource = this.Paginator.Items;
            lstFavorites.ItemsSource = this.Favorites;
            this.Loaded += async (s, e) => await this.Paginator.First();
        }

        private async void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPivot.SelectedIndex == 1 && !_favoritesLoaded)
            {
                progressRingFavorites.IsActive = true;
                progressRingFavorites.Visibility = Visibility.Visible;

                Favorites.Clear();

                // Asumiendo que exponés el nuevo método en tu DockService
                // (O podés inyectar y usar el repositorio local acá directamente)
                var dtos = await _dockService.GetBookmarks();
                foreach (var dto in dtos)
                {
                    Favorites.Add(dto);
                }

                txtEmptyFavorites.Visibility = Favorites.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                progressRingFavorites.IsActive = false;
                progressRingFavorites.Visibility = Visibility.Collapsed;
                _favoritesLoaded = true;
            }
        }

        private async Task LoadDocksAsync()
        {
            
        }

        private async void btnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            await this.Paginator.NextPage();
        }

        private void lstDocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstDocks.SelectedItem is Dock dock)
            {
                SelectedDock = dock;
                lstFavorites.SelectedItem = null; // Limpiar la otra lista
                IsPrimaryButtonEnabled = true;
            }
        }

        private void lstFavorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFavorites.SelectedItem is Dock dock)
            {
                SelectedDock = dock;
                lstDocks.SelectedItem = null; // Limpiar la otra lista
                IsPrimaryButtonEnabled = true;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Cierre por botón Aceptar (SelectedDock conserva el valor)
        }


        private System.Threading.CancellationTokenSource _cts;
        private async void QTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Q = qTxt.Text;
            
            _cts?.Cancel();
            _cts = new System.Threading.CancellationTokenSource();

            try
            {
                await Task.Delay(500, _cts.Token);
                
                this.Paginator.Clear();
                await this.Paginator.First();
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
