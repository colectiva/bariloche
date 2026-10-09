using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace La_Colectiva.Core
{
    public static class LocationHelper
    {
        public static async Task<BasicGeoposition?> GetCurrentLocationAsync(uint timeoutSeconds = 10)
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus != GeolocationAccessStatus.Allowed)
                    return null;

                var geolocator = new Geolocator
                {
                    DesiredAccuracyInMeters = 100 // Menor precisión = respuesta mucho más rápida
                };

                // Usa ubicación en caché si tiene menos de 5 min, o busca por máximo 'timeoutSeconds'
                Geoposition position = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(10),
                    timeout: TimeSpan.FromSeconds(timeoutSeconds)
                );

                return position.Coordinate.Point.Position;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GPS Timeout/Error: {ex.Message}");
                return null;
            }
        }
    }
}
