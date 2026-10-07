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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace La_Colectiva.Views.Dialogs
{
    public sealed partial class DockSelectorDialog : ContentDialog
    {
        private readonly DockService _dockService;
        public ObservableCollection<Dock> Docks { get; } = new ObservableCollection<Dock>();
        public Dock SelectedDock { get; private set; }

        private int _currentPage = 0;
        private const int PageSize = 10;
        private bool _hasMore = true;

        private readonly double _lat;
        private readonly double _lon;

        public DockSelectorDialog(double lat = -34.4258, double lon = -58.5796)
        {
            this.InitializeComponent();
            _dockService = App.Current.Services.GetRequiredService<DockService>();
            _lat = lat;
            _lon = lon;

            lstDocks.ItemsSource = Docks;
            this.Loaded += async (s, e) => await LoadDocksAsync();
        }

        private async Task LoadDocksAsync()
        {
            if (!_hasMore) return;

            progressRing.IsActive = true;
            btnLoadMore.IsEnabled = false;

            var request = _dockService.Search("", _lat, _lon, _currentPage, PageSize)
                .Ok(page =>
                {
                    if (page.Content != null)
                    {
                        foreach (var dock in page.Content)
                        {
                            Docks.Add(dock);
                        }
                    }

                    _hasMore = page.Pageable != null && _currentPage < page.Pageable.TotalPages - 1;
                    btnLoadMore.Visibility = _hasMore ? Visibility.Visible : Visibility.Collapsed;
                    txtEmpty.Visibility = Docks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                    _currentPage++;
                })
                .FatalError(err =>
                {
                    btnLoadMore.Visibility = Visibility.Collapsed;
                });

            await request.RunAsync();

            progressRing.IsActive = false;
            btnLoadMore.IsEnabled = true;
        }

        private async void btnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            await LoadDocksAsync();
        }

        private void lstDocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDock = lstDocks.SelectedItem as Dock;
            IsPrimaryButtonEnabled = SelectedDock != null;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Cierre por botón Aceptar (SelectedDock conserva el valor)
        }
    }
}
