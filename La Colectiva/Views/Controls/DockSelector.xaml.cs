using La_Colectiva.Core;
using La_Colectiva.Models.Classic;
using La_Colectiva.Views.Dialogs;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace La_Colectiva.Views.Controls
{
    public sealed partial class DockSelector : UserControl
    {
        private bool _isDialogOpen = false;

        public DockSelector()
        {
            this.InitializeComponent();
            UpdateUI();
            UpdateLoadingState(false);
        }

        #region Dependency Properties

        // Propiedad para la etiqueta (ej: "Desde", "Hasta")
        public string Header {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(DockSelector),
                new PropertyMetadata(string.Empty, (d, e) => ((DockSelector)d).HeaderTextBlock.Text = e.NewValue?.ToString() ?? string.Empty));

        // Propiedad para el Muelle seleccionado
        public Dock SelectedDock {
            get => (Dock)GetValue(SelectedDockProperty);
            set => SetValue(SelectedDockProperty, value);
        }
        public static readonly DependencyProperty SelectedDockProperty =
            DependencyProperty.Register(nameof(SelectedDock), typeof(Dock), typeof(DockSelector),
                new PropertyMetadata(null, (d, e) => ((DockSelector)d).UpdateUI()));


        public bool LoadingData {
            get => (bool)GetValue(LoadingDataProperty);
            set => SetValue(LoadingDataProperty, value);
        }

        public static readonly DependencyProperty LoadingDataProperty =
            DependencyProperty.Register(nameof(LoadingData), typeof(bool), typeof(DockSelector),
                new PropertyMetadata(false, (d, e) => ((DockSelector)d).UpdateUI()));

        public Visibility ProgressBarVisibility => LoadingData ? Visibility.Visible : Visibility.Collapsed;

        // Texto por defecto cuando está vacío
        public string PlaceholderText {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(DockSelector),
                new PropertyMetadata("Ningún muelle seleccionado", (d, e) => ((DockSelector)d).UpdateUI()));

        #endregion

        private async void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isDialogOpen) return;
            _isDialogOpen = true;

            try
            {
                UpdateLoadingState(true);
                double lat = -34.4258;
                double lon = -58.5796;

                // Obtiene ubicación con timeout
                var pos = await LocationHelper.GetCurrentLocationAsync(timeoutSeconds: 3);
                if (pos.HasValue)
                {
                    lat = pos.Value.Latitude;
                    lon = pos.Value.Longitude;
                }

                var dialog = new DockSelectorDialog(lat, lon);
                UpdateLoadingState(false);
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary && dialog.SelectedDock != null)
                {
                    SelectedDock = dialog.SelectedDock;
                }
            }
            finally
            {
                _isDialogOpen = false;
                SelectButton.IsEnabled = true;
            }
        }

        private void UpdateLoadingState(bool isLoading)
        {
            if (progressRing == null) return;

            progressRing.IsActive = isLoading;
            progressRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateUI()
        {
            if (DockTextBlock == null) return;

            if (SelectedDock != null)
            {
                DockTextBlock.Text = SelectedDock.Name;
                DockTextBlock.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"];
            }
            else
            {
                DockTextBlock.Text = PlaceholderText;
                DockTextBlock.Foreground = (Brush)Application.Current.Resources["SystemControlPageTextBaseMediumBrush"];
            }
        }
    }
}
