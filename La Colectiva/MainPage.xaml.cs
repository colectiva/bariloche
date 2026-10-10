using La_Colectiva.Core;
using La_Colectiva.Models.Classic;
using La_Colectiva.Models.Local;
using La_Colectiva.Models.Queries;
using La_Colectiva.Services.Classic;
using La_Colectiva.Services.Local;
using La_Colectiva.Services.Local.Databases;
using La_Colectiva.Services.Queries;
using La_Colectiva.Views.Dialogs;
using La_Colectiva.Views.Pages;
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

namespace La_Colectiva
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private readonly DockService _dockService;
        private readonly TripService _tripService = App.Current.Services.GetRequiredService<TripService>();
        private readonly TripReminderService _trs = App.Current.Services.GetRequiredService<TripReminderService>();
        private readonly TripShortcutRepository tripShortcutRepository = App.Current.Services.GetRequiredService<TripShortcutRepository>();


        private ObservableCollection<TripShortcut> Shortcuts { get; set; } = new ObservableCollection<TripShortcut>();

        private bool _isDialogOpen = false;

        private bool _isLoading = false;
        public bool Loading {
            get => _isLoading;
            set {
                _isLoading = value;
                if(value) {
                    this.LoadingProgress.Visibility = Visibility.Visible;
                } else {
                    this.LoadingProgress.Visibility = Visibility.Collapsed;
                }
            }
        }

        private Dock _from = null;
        public Dock From {
            get { return _from;  }
            set {
                _from = value;
                OnAnyDockChange();
            }
        }

        private Dock _to = null;
        public Dock To {
            get { return _to; }
            set {
                _to = value;
                OnAnyDockChange();
            }
        }

        public void OnAnyDockChange() {
            if(From != null && To != null)
            {
                _ = EjecutarBusqueda();
                this.AddShortcutBtn.Visibility = Visibility.Visible;
            } else
            {
                this.AddShortcutBtn.Visibility = Visibility.Collapsed;
            }
        }


        private DateTimeOffset? _date = DateTimeOffset.Now;
        public DateTimeOffset? Date {
            get => _date;
            set {
                _date = value;
                if (From != null && To != null)
                {
                    _ = EjecutarBusqueda();
                }
            }
        }

        private TimeSpan? _time = DateTime.Now.TimeOfDay;
        public TimeSpan? Time {
            get => _time;
            set {
                _time = value;
                if (From != null && To != null)
                {
                    _ = EjecutarBusqueda();
                }
            }
        }

        private void ShowShortcuts()
        {
            TripShortcutListBox.Visibility = Visibility.Visible;
            LstTrips.Visibility = Visibility.Collapsed;
        }

        private void HideShortcuts()
        {
            TripShortcutListBox.Visibility = Visibility.Collapsed;
            LstTrips.Visibility = Visibility.Visible;
        }

        private async Task LoadShortcuts() {
            var items = await tripShortcutRepository.List();
            Shortcuts.Clear();
            var processed = items.Select(TripShortcut.Mapper).ToList();
            foreach (var s in processed)
            {
                Shortcuts.Add(s);
            }
            TripShortcutListBox.ItemsSource = Shortcuts;
            return;
        }
        private async Task AddShortcut()
        {
            if (From == null || To == null) return;
            var obj = new TripShortcutDto(From.Id, To.Id);
            obj.From = From.Raw;
            obj.To = To.Raw;
            await tripShortcutRepository.Save(obj);
            await LoadShortcuts();
            

        }
        private async Task RemoveSelectedShortcut()
        {
            if(TripShortcutListBox.SelectedItem is TripShortcut selected)
            {
                await tripShortcutRepository.Delete(selected.From.Id, selected.To.Id);
                await LoadShortcuts();
            }
        }

        private async Task EjecutarBusqueda()
        {
            Loading = true;
            var selectedDate = Date?.DateTime ?? DateTime.Now.Date;
            var selectedTime = Time ?? DateTime.Now.TimeOfDay;
            var request = new NextTripRequest
            {
                OriginId = From.Id,
                DestinationId = To.Id,
                Date = selectedDate,
                Time = selectedTime,
                Weekday = GetWeekday(selectedDate.DayOfWeek)
            };

            await _tripService
                .FindNextTrip(request)
                .Ok(result =>
                {
                    HideShortcuts();
                    LstTrips.ItemsSource = result;
                    Loading = false;
                })
                .RunAsync();

            Loading = false;

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

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            _dockService = App.Current.Services.GetRequiredService<DockService>();
            Loading = false;
            _ = LoadShortcuts();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        

        private async void SearchTrips()
        {
            if (From == null || To == null)
            {
                ShowShortcuts();
                return;
            }

            await EjecutarBusqueda();
        }

        private void SearchNextTrip_Click(object sender, RoutedEventArgs e)
        {
            SearchTrips();
        }

        private void LstTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstTrips.SelectedItem is NextTrip selectedTrip)
            {
                NotifyMeBtn.Visibility = Visibility.Visible;
            } else
            {
                NotifyMeBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void SwitchDocks_Click(object sender, RoutedEventArgs e)
        {
            var __from = From;
            var __to = To;
            From = __to;
            To = __from;
            this.Bindings.Update();
            SearchTrips();
        }

        // Avisarme
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (LstTrips.SelectedItem is NextTrip selectedTrip)
            {
                _trs.SaveAndScheduleReminder(selectedTrip, Date?.DateTime ?? DateTime.Now.Date);
            }
        }

        private void AddShortcutBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = AddShortcut();
        }

        private async void DeleteShortcutBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = RemoveSelectedShortcut();
        }

        private void TripShortcutListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TripShortcutListBox.SelectedItem is TripShortcut selected)
            {
                DeleteShortcutBtn.Visibility = Visibility.Visible;
                From = selected.From;
                To = selected.To;
                this.Bindings.Update();
                SearchTrips();
            } else
            {
                DeleteShortcutBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
