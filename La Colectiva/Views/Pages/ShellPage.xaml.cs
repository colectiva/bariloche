using La_Colectiva.Views.Pages.OfflineManager;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace La_Colectiva.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(MainPage));
            ContentFrame.Navigated += ContentFrame_Navigated;
            
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += ShellPage_BackRequested;

            ContentFrame.Navigate(typeof(MainPage));
        }

        private void ShellPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                e.Handled = true; 
                ContentFrame.GoBack();
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ContentFrame.CanGoBack
                    ? Windows.UI.Core.AppViewBackButtonVisibility.Visible
                    : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
        }



        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
        
        private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Tag?.ToString())
                {
                    case "MainPage":
                        ContentFrame.Navigate(typeof(MainPage));
                        break;
                    case "NetworkStationsPage":
                        ContentFrame.Navigate(typeof(NetworkStationsPage));
                        break;
                    case "OfflineDocksPage":
                        ContentFrame.Navigate(typeof(DockRepositoryManager));
                        break;
                }
                
                MySplitView.IsPaneOpen = false;
            }
        }
    }
}
