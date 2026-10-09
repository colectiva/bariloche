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
    public sealed partial class DateTimePickerButton : UserControl
    {
        public DateTimePickerButton()
        {
            this.InitializeComponent();
            UpdateDisplayText();
        }

        // Propiedad Date
        public DateTimeOffset? Date {
            get { return (DateTimeOffset?)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTimeOffset?), typeof(DateTimePickerButton),
                new PropertyMetadata(null, OnValueChanged));

        // Propiedad Time
        public TimeSpan? Time {
            get { return (TimeSpan?)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(TimeSpan?), typeof(DateTimePickerButton),
                new PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePickerButton control)
            {
                control.UpdateDisplayText();
            }
        }

        private async void MainButton_Click(object sender, RoutedEventArgs e)
        {
            // Crear el contenido del diálogo
            var datePicker = new DatePicker { Date = this.Date ?? DateTimeOffset.Now, Margin = new Thickness(0, 0, 0, 15) };
            var timePicker = new TimePicker { Time = this.Time ?? DateTime.Now.TimeOfDay };

            var panel = new StackPanel();
            panel.Margin = new Thickness(0, 5, 0, 0);
            panel.Children.Add(datePicker);
            panel.Children.Add(timePicker);

            // Armar el diálogo
            var dialog = new ContentDialog
            {
                Title = "Seleccionar Fecha y Hora",
                Content = panel,
                PrimaryButtonText = "Aceptar",
                SecondaryButtonText = "Ahora"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                this.Date = datePicker.Date;
                this.Time = timePicker.Time;
            }
            else if (result == ContentDialogResult.Secondary)
            {
                this.Date = null;
                this.Time = null;
            }
        }

        private void UpdateDisplayText()
        {
            if (TxtDisplay == null) return;

            if (!Date.HasValue || !Time.HasValue)
            {
                TxtDisplay.Text = "Ahora";
                return;
            }

            var selectedDate = Date.Value.Date;
            var today = DateTime.Today;
            var timeString = Time.Value.ToString(@"hh\:mm");

            if (selectedDate == today)
            {
                TxtDisplay.Text = $"Hoy {timeString}";
            }
            else if (selectedDate == today.AddDays(1))
            {
                TxtDisplay.Text = $"Mañana {timeString}";
            }
            else
            {
                // Formato: 19 dic. 2026 12:30
                var culture = new System.Globalization.CultureInfo("es-ES");
                TxtDisplay.Text = $"{selectedDate.ToString("dd MMM. yyyy", culture)} {timeString}";
            }
        }
    }
}
