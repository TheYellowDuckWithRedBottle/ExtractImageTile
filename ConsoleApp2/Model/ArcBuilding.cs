using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Model
{
    public class ArcBuilding
    {
        public string ArcEntityId { get; set; }
        public string ArcType { get; set; }
        public List<Attributes> Attributes { get; set; }
        public List<string> LinearCoords { get; set; }
        public bool isHole { get; set; }
        public List<string> coords { get; set; }
        public List<Relation> subEntity { get; set; } = new List<Relation>();
        
    }
    public class Relation
    {
        public string  Name { get; set; }
        public string sub { get; set; }

    }
    public class LineCircle
    {
        public List<Coordinate> linear { get; set; } = new List<Coordinate>();
    }
    public class Coordinate
    {
        public double x { get; set; }
        public double y { get; set; }
    }
    public class Attributes
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
 
    public class UnitVector
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
    public class Segment
    {
      public  Coordinate SegmentStart { get; set; }
        public Coordinate SegmentEnd { get; set; }
    }

}
