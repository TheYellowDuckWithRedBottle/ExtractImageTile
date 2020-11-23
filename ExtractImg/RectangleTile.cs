using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractImg
{
   public class RectangleTile
    {
        public Tile LeftTop { get; set; }
        public Tile RightTop { get; set; }
        public Tile LeftBottom { get; set; }
        public Tile RightBottom { get; set; }
        public int Zoom { get; set; }
        public int xmin { get; set; }
        public int xmax { get; set; }
        public int ymin { get; set; }
        public int ymax { get; set; }

        public RectangleTile(List<Tile> tiles)
        {
            
            List<int> xarray = new List<int>();
            List<int> yarray = new List<int>();
            foreach (var item in tiles)
            {
                xarray.Add(item.tileX);
                yarray.Add(item.tileY);
            }
            xarray.Sort();
            yarray.Sort();
            this.xmin = xarray[0];
            this.xmax = xarray[xarray.Count - 1];
            this.ymin = yarray[0];
            this.ymax = yarray[yarray.Count - 1];
            
            LeftTop = new Tile(xarray[0],yarray[0]);
            RightTop = new Tile(xarray[xarray.Count - 1], yarray[0]);
            LeftBottom = new Tile(xarray[0], yarray[yarray.Count - 1]);
            RightBottom = new Tile(xarray[xarray.Count-1],yarray[yarray.Count-1]);
        }
        public int getXmin()
        {
            return LeftTop.tileX;
        }
        public int getXMax()
        {
            return RightBottom.tileX;
        }
        public int getYMin()
        {
            return LeftTop.tileY;
        }
        public int getYMax()
        {
            return RightBottom.tileY;
        }
       

    }
}
