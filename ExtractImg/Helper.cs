using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
namespace ExtractImg
{
   public class Helper
    {
        public Tile deg2Num(double lon, double lat,double zoom)
        {
            Tile tile = new Tile();
            double lat_rad = lat / 180 * Math.PI;
            double lon_rad = lon / 180 * Math.PI;
            double n = Math.Pow(double.Parse("2"),zoom);
            tile.tileX = Convert.ToInt32(((lon + 180.0) / 360.0 * n));
            tile.tileY = Convert.ToInt32((1.0-Trig.Asinh(Trig.Tan(lat_rad))/Constants.Pi)/ 2.0 * n);
            return tile;
        }
        public Coordnate ConvertStringToCoord(string text)
        {
            string[] coordArray = text.Split(',');
            double lon= double.Parse(coordArray[0]);
            double lat = double.Parse(coordArray[1]);
            Coordnate coordnate = new Coordnate(lon, lat);
            return coordnate;
        }
    }
}
