using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractImg
{
   public class Coordnate
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
        public Coordnate(double lon,double lat)
        {
            this.Lon = lon;
            this.Lat = lat;
        }
    }
}
