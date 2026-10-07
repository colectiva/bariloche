using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Core
{
    public static class GeoCalculator
    {
        private const double EarthRadiusInMeters = 6371000.0;

        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (EarthRadiusInMeters * c) / 1000; // Devuelve metros
        }
        

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
