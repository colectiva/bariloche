using La_Colectiva.Services.Api;
using La_Colectiva.Services.Api.Impl;
using La_Colectiva.Services.Classic;
using La_Colectiva.Services.Classic.Impl;
using La_Colectiva.Services.Local;
using La_Colectiva.Services.Local.Databases;
using La_Colectiva.Services.Local.Databases.Impl;
using La_Colectiva.Services.Local.Impl;
using La_Colectiva.Services.Queries;
using La_Colectiva.Services.Queries.Impl;
using La_Colectiva.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        public IServiceProvider Services { get; private set; }
        
        public new static App Current => (App)Application.Current;


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Services = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<HttpClient>();
            // 1. Registrar servicio de la API REST
            services.AddSingleton<ApiService, ApiServiceImpl>();
            services.AddSingleton<AuthService, AuthServiceImpl>();
            services.AddSingleton<DockService, DockServiceImpl>();
            services.AddSingleton<TripService, TripServiceImpl>();
            services.AddSingleton<TideService, TideServiceImpl>();
            services.AddSingleton<NetworkStationService, NetworkStationServiceImpl>();

            services.AddSingleton<DatabaseService, DatabaseServiceImpl>();

            services.AddSingleton<ViaRepository, ViaRepositoryImpl>();
            services.AddSingleton<DockRepository, DockRepositoryImpl>();
            services.AddSingleton<TripShortcutRepository, TripShortcutRepositoryImpl>();

            services.AddSingleton<TripReminderService, TripReminderServiceImpl>();
            services.AddSingleton<TileService, TileServiceImpl>();

            // 2. Registrar ViewModel inyectando la API
            // services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(ShellPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            Services = ConfigureServices();

            // Obtenemos un deferral para avisarle a Windows que nuestra tarea es asíncrona
            // y que no nos mate el proceso hasta que terminemos de descargar los datos.
            var deferral = args.TaskInstance.GetDeferral();

            try
            {
                if (args.TaskInstance.Task.Name == "TideTileUpdateTask")
                {
                    // NOTA: Asegurate de que App.Current.Services ya esté inicializado a esta altura.
                    // Si inicializás tus servicios en "OnLaunched", copialo también al constructor de "App()" 
                    // o a un método "ConfigureServices()" que corra siempre.

                    var tileService = App.Current.Services.GetRequiredService<TileService>() as TileServiceImpl;
                    if (tileService != null)
                    {
                        await tileService.UpdateTileFromBackgroundAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en BackgroundTask: {ex.Message}");
            }
            finally
            {
                // Le avisamos a Windows que terminamos
                deferral.Complete();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
