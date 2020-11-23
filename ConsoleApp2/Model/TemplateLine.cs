using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Model
{
    public class TemplateLine
    {
        public List<Coordinate> coords { get; set; } = new List<Coordinate>();
        public string Id { get; set; }
    }
    public class PolylineWithId
    {
        public Polyline polyline { get; set; } = new Polyline();
        public string id { get; set; }
    }
}
