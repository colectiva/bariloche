using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Core
{
    public class Distance
    {

        private double? _km { get; set; }

        public Distance(double? km)
        {
            this._km = km;
        }

        public Distance(string distance)
        {
            if (distance == null)
            {
                this._km = null;
                return;
            }
            this._km = Culture.from(distance);
        }

        public double? Meters { get { return _km * 1000; } }
        public double? Kilometers { get { return _km; } }

        public override string ToString()
        {
            if (_km == null) return "N/A";
            if (_km < 1)
            {
                return $"{Math.Round(this.Meters.Value, 2)} m";
            }
            else
            {
                return $"{Math.Round(this.Kilometers.Value, 2)} km";
            }
        }


    }
}
