using La_Colectiva.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Local
{
    public interface TripReminderService
    {
        void SaveAndScheduleReminder(NextTrip trip, DateTime travelDate);
        NextTrip GetSavedTrip();
        void ClearReminder();
    }
}
