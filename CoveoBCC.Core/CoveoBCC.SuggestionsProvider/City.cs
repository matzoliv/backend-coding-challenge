using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoveoBCC.Core
{
    public class City
    {
        public string Name { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public City ( string Name, double Latitude, double Longitude )
        {
            this.Name = Name;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }
    }
}
