using La_Colectiva.Models.Queries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.UI.Notifications;

namespace La_Colectiva.Services.Local.Impl
{
    class TripReminderServiceImpl: TripReminderService
    {
        private const string KeySavedTrip = "SavedNextTrip";
        private const string ToastTag = "TRT";

        /// <summary>
        /// Guarda el viaje en LocalSettings y programa la notificación local.
        /// </summary>
        public void SaveAndScheduleReminder(NextTrip trip, DateTime travelDate)
        {
            if (trip?.Start == null) return;
            
            string json = JsonConvert.SerializeObject(trip);
            ApplicationData.Current.LocalSettings.Values[KeySavedTrip] = json;
            
            DateTime departureTime = travelDate.Date.Add(trip.Start.Time);
            DateTime notificationTime = departureTime.AddMinutes(-10);
            
            if (notificationTime > DateTime.Now)
            {
                ScheduleToast(trip, notificationTime);
            }
        }

        /// <summary>
        /// Obtiene el viaje guardado al abrir la app.
        /// </summary>
        public NextTrip GetSavedTrip()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(KeySavedTrip, out object val) && val is string json)
            {
                return JsonConvert.DeserializeObject<NextTrip>(json);
            }
            return null;
        }

        /// <summary>
        /// Elimina el viaje guardado y cancela la alarma.
        /// </summary>
        public void ClearReminder()
        {
            ApplicationData.Current.LocalSettings.Values.Remove(KeySavedTrip);
            RemoveScheduledToasts();
        }

        private void ScheduleToast(NextTrip trip, DateTime notificationTime)
        {
            RemoveScheduledToasts();

            string title = $"Lancha a las {trip.Start.Time:hh\\:mm}";
            string body = $"Pasa por {trip.Start.Dock.Name}, llega {trip.End.Time:hh\\:mm} a {trip.End.Dock.Name}";

            string xml = $@"
        <toast>
            <visual>
                <binding template=""ToastGeneric"">
                    <text>{title}</text>
                    <text>{body}</text>
                </binding>
            </visual>
        </toast>";

            Windows.Data.Xml.Dom.XmlDocument doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(xml);

            var scheduledToast = new ScheduledToastNotification(doc, notificationTime)
            {
                Id = ToastTag
            };

            var toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
        }

        private void RemoveScheduledToasts()
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            foreach (var item in notifier.GetScheduledToastNotifications())
            {
                if (item.Id == ToastTag)
                {
                    notifier.RemoveFromSchedule(item);
                }
            }
        }

    }
}
