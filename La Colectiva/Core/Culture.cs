using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Core
{
    public class Culture
    {

        public static readonly Culture Default = new Culture();

        public Culture() { }

        public static double from(string n)
        {
            return Double.Parse(n, CultureInfo.InvariantCulture);
        }

        public static string coordsToGoogleMapString(double Lat, double Lng)
        {
            return $"{Lat:F6},{Lng:F6}";
        }

        public static string prettyPrintLat(double lat)
        {
            char dir = lat >= 0 ? 'N' : 'S';
            double a = Math.Abs(lat);
            int g = (int)a;
            double rg = (a - g) * 60;
            int m = (int)rg;
            double s = Math.Round((rg - m) * 60, 2);
            return $"{g}° {m}' {s}\" {dir}";
        }
        

        public static string prettyPrintLng(double lng)
        {
            char dir = lng >= 0 ? 'E' : 'W';
            double a = Math.Abs(lng);
            int g = (int)a;
            double rg = (a - g) * 60;
            int m = (int)rg;
            double s = Math.Round((rg - m) * 60,2);
            return $"{g}° {m}' {s}\" {dir}";
        }
        /* 
        public static string prettyPrintLng(PointLatLng position)
        {
            return prettyPrintLng(position.Lng);
        }

        public static string prettyPrintCoords(PointLatLng position)
        {
            return $"{prettyPrintLat(position.Lat)} {prettyPrintLng(position.Lng)}";
        } //*/


    }
}
