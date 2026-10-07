using La_Colectiva.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace La_Colectiva.Views.Dialogs
{
    public sealed partial class ErrorWindow : ContentDialog
    {
        public ApiFatalErrorResponse Error { get; set; }

        public ErrorWindow(ApiFatalErrorResponse error, ErrorWindowData template)
        {
            this.InitializeComponent();
            this.Error = error;

            if (!string.IsNullOrEmpty(template.Icon))
                this.txtIcon.Text = template.Icon;

            if (!string.IsNullOrEmpty(template.IconColor))
                this.txtIcon.Foreground = ConvertHexToBrush(template.IconColor);

            this.txtTitle.Text = !string.IsNullOrEmpty(template.Title) ? template.Title : "Error";
            this.txtSubtitle.Text = !string.IsNullOrEmpty(template.Subtitle) ? template.Subtitle : (error.Message ?? "");

            this.txtFullError.Text = error.PrintFullError();
        }

        private void chkDetails_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (scrollDetails != null)
            {
                scrollDetails.Visibility = chkDetails.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private SolidColorBrush ConvertHexToBrush(string hex)
        {
            try
            {
                hex = hex.Replace("#", "");
                byte a = 255;
                byte r = 0, g = 0, b = 0;

                if (hex.Length == 8)
                {
                    a = Convert.ToByte(hex.Substring(0, 2), 16);
                    r = Convert.ToByte(hex.Substring(2, 2), 16);
                    g = Convert.ToByte(hex.Substring(4, 2), 16);
                    b = Convert.ToByte(hex.Substring(6, 2), 16);
                }
                else if (hex.Length == 6)
                {
                    r = Convert.ToByte(hex.Substring(0, 2), 16);
                    g = Convert.ToByte(hex.Substring(2, 2), 16);
                    b = Convert.ToByte(hex.Substring(4, 2), 16);
                }

                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            catch
            {
                return new SolidColorBrush(Colors.Orange);
            }
        }
    }
}
