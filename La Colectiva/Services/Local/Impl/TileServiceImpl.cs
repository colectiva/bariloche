using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Classic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.Storage;

namespace La_Colectiva.Services.Local.Impl
{
    public class TileServiceImpl : TileService
    {
        private readonly TideService tideService;
        private const string TASK_NAME = "TideTileUpdateTask";

        public TileServiceImpl()
        {
            tideService = App.Current.Services.GetRequiredService<TideService>();
        }


        public void UpdatePrimaryTile(NetworkStation station)
        {
            _ = RegisterAndRunTileTaskAsync(station);
        }
        
        public async Task RegisterAndRunTileTaskAsync(NetworkStation station)
        {
            if (station == null) return;

            // 1. Guardar la estación seleccionada en LocalSettings para el proceso en segundo plano
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["TileStationId"] = station.Id;
            settings.Values["TileStationName"] = station.Name;

            // 2. Ejecutar la primera actualización manual al momento
            await UpdateTileFromBackgroundAsync();

            // 3. Pedir permiso a Windows para correr en segundo plano
            var accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (accessStatus == BackgroundAccessStatus.DeniedByUser ||
                accessStatus == BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                return; // El usuario desactivó las tareas en segundo plano
            }

            // 4. Limpiar tareas anteriores para evitar duplicados
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == TASK_NAME)
                {
                    task.Value.Unregister(true);
                }
            }

            // 5. Registrar el Trigger (UWP permite un mínimo de 15 minutos, usamos 60)
            var builder = new BackgroundTaskBuilder
            {
                Name = TASK_NAME
            };

            builder.SetTrigger(new TimeTrigger(60, false)); // Cada 60 minutos
            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable)); // Solo si hay red
            builder.Register();
        }
        
        public async Task UpdateTileFromBackgroundAsync()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey("TileStationId")) return;

            long stationId = (long)settings.Values["TileStationId"];
            string stationName = (string)settings.Values["TileStationName"];

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            await tideService.TodayForecast(stationId).Ok(tides =>
            {
                var ultimasMareas = tides
                    .Where(t => t.Start <= DateTime.Now)
                    .OrderByDescending(t => t.Start)
                    .Take(5)
                    .ToList();

                foreach (var tide in ultimasMareas)
                {
                    string valor = $"{tide.Value:0.00}m";
                    string hora = tide.Start.ToString("HH:mm");
                    string estado = tide.IsForecast ? "Pronóstico" : "Observado";

                    string xml = $@"
                    <tile>
                        <visual version=""2"">
                            <binding template=""TileMedium"">
                                <text hint-style=""caption"" hint-wrap=""true"">{stationName}</text>
                                <text hint-style=""header"">{valor}</text>
                                <text hint-style=""captionSubtle"">{hora} - {estado}</text>
                            </binding>
                            <binding template=""TileWide"">
                                <text hint-style=""header"">{valor}</text>
                                <text hint-style=""caption"">{stationName}</text>
                                <text hint-style=""captionSubtle"">{estado} a las {hora}</text>
                            </binding>
                        </visual>
                    </tile>";

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    var notification = new TileNotification(doc)
                    {
                        ExpirationTime = DateTimeOffset.Now.AddHours(2)
                    };

                    updater.Update(notification);
                }
            }).RunAsync();
        }

    }

}
