using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractImg
{
    public class Tile
    {
        public int tileX { get; set; }
        public int tileY { get; set; }
        public Tile(int tileX, int tileY)
        {
            this.tileX = tileX;
            this.tileY = tileY;
        }
        public Tile()
        {

        }
    }
}
