using La_Colectiva.Models.Api;
using La_Colectiva.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace La_Colectiva.Services.Handlers
{
    public static class ErrorHandler
    {
        public static Action<ApiFatalErrorResponse> Default()
        {
            return async (fatalError) => {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    var dialog = new MessageDialog($"{fatalError.Message}", "Error");
                    await dialog.ShowAsync();
                });
            };
        }

        public static Action<ApiFatalErrorResponse> Detailed(string alternativeTitle)
        {
            return async (fatalError) => {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    // Nota: En UWP, 'ErrorWindow' debería ser un 'ContentDialog'
                    var window = new ErrorWindow(fatalError, new ErrorWindowData() { Title = alternativeTitle });
                    await window.ShowAsync();
                });
            };
        }

        public static Action<ApiFatalErrorResponse> Detailed(ErrorWindowData data)
        {
            return async (fatalError) => {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    var window = new ErrorWindow(fatalError, data);
                    await window.ShowAsync();
                });
            };
        }

        public static Action<ApiFatalErrorResponse> Detailed(HttpErrorTemplate template)
        {
            return async (fatalError) => {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    var data = fatalError.Status != null
                        ? template.With(fatalError).Get(fatalError.Status.Value)
                        : template.With(fatalError).GetLocalError();

                    var window = new ErrorWindow(fatalError, data);
                    await window.ShowAsync();
                });
            };
        }

        public static Action<ApiFatalErrorResponse> Detailed()
        {
            return Detailed(HttpErrorTemplate.Default());
        }
    }
}
