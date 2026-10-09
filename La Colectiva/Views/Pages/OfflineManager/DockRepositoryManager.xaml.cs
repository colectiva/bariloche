using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Classic;
using La_Colectiva.Services.Local.Databases;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace La_Colectiva.Views.Pages.OfflineManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DockRepositoryManager : Page
    {
        private readonly DockService dockService = App.Current.Services.GetRequiredService<DockService>();
        private readonly DatabaseService dbService = App.Current.Services.GetRequiredService<DatabaseService>();

        public Paginator<Dock> Paginator = new Paginator<Dock>();
        public string Q { get; set; } = "";

        public DockRepositoryManager() {
            this.InitializeComponent();
            this.DataContext = this;
            this.Paginator
                .Append()
                .PageSize(12)
                .Loader(async (page, size) =>
                {
                    var p = new Page<Dock>();
                    await dockService
                        .Search(this.Q, page, size)
                        .Ok(x => p = x)
                        .RunOffline();
                    return p;
                });
            this.DockListBox.ItemsSource = this.Paginator.Items;
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

        public void UpdateStarBtnVisibility(Dock dock)
        {
            if (dock.IsBookmarked)
            {
                StarBtn.Icon = new SymbolIcon(Symbol.SolidStar);
                StarBtn.Label = "Quitar de favoritos";
            }
            else
            {
                StarBtn.Icon = new SymbolIcon(Symbol.OutlineStar);
                StarBtn.Label = "Agregar a favoritos";
            }
        }

        private void DockListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DockListBox.SelectedItem is Dock dock)
            {
                StarBtn.Visibility = Visibility.Visible;
                UpdateStarBtnVisibility(dock);

            } else
            {
                StarBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnLoadMore_Click(object sender, RoutedEventArgs e)
        {
            LoadMore();
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

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void DropBtn_Click(object sender, RoutedEventArgs e)
        {
            await dbService.DropData();
        }

        private async void StarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DockListBox.SelectedItem is Dock dock)
            {
                if(dock.IsBookmarked)
                {
                    await dockService.DeleteBookmark(dock.Id);
                    dock.IsBookmarked = false;
                } else
                {
                    await dockService.Bookmark(dock.Id);
                    dock.IsBookmarked = true;
                }
                UpdateStarBtnVisibility(dock);
            }
        }
    }
}
