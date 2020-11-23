using ConsoleApp2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ConsoleApp2
{
  public  class ReadXmlHelper
    {
        public List<double> tolerate { get; set; } = new List<double>();
       
        /// <summary>
        /// 提取坐标
        /// </summary>
        /// <param name="content">坐标信息</param>
        /// <returns></returns>

        public List<ArcBuilding> readBuiInfoFromXml(string filePath)
        {
            List<ArcBuilding> arcBuildings = new List<ArcBuilding>();
            XmlDocument doc = new XmlDocument();
            XmlReader reader = XmlReader.Create(filePath);
            doc.Load(reader);
            XmlNode xn = doc.SelectSingleNode("ArchModel");
            XmlNodeList xnl = xn.ChildNodes; 
            foreach (XmlNode xnode in xnl)//从根节点下开始循环
            {
                if(xnode.Name== "ArchEntitys")//判断是否为Entitys
                {
                  var  ArcEntity= xnode.ChildNodes;//默认是arcentity
                    foreach (XmlNode entity in ArcEntity)//循环entitys
                    {
                        ArcBuilding arcBuilding = new ArcBuilding();
                        Boolean isHole=false;
                        XmlElement xe = entity as XmlElement;
                        arcBuilding.ArcEntityId = xe.GetAttribute("ID").ToString();//获取id
                        arcBuilding.ArcType = xe.GetAttribute("Type").ToString();//获取Type
                        foreach (XmlNode property in xe.ChildNodes)
                        {
                            if (property.Name == "ArchProperties")//遍历到property
                            {
                                XmlNodeList arcProperty = property.ChildNodes;
                                foreach (XmlNode item in arcProperty)
                                {
                                    XmlElement xe1 = item as XmlElement;
                                    if(xe1.Name== "innerProperties")//遍历到内部属性
                                    {
                                        List<Attributes> attributes1 = new List<Attributes>();
                                        foreach (XmlNode item1 in xe1.ChildNodes)
                                        {
                                            XmlElement xe2 = item1 as XmlElement;
                                            Attributes attributes = new Attributes();
                                            //model中的property
                                            attributes.PropertyName=xe2.GetAttribute("Name");//属性名
                                            attributes.Value=xe2.InnerText;
                                            attributes1.Add(attributes);
                                        }
                                        arcBuilding.Attributes = attributes1;//获取到的属性存放
                                      
                                    }
                                         
                                }    
                            }
                            else if (property.Name == "ArchGeometries")//获取坐标信息
                            {
                                XmlNodeList arcProperty = property.ChildNodes;
                                List<string> coordinates = new List<string>();
                                List<string> LinearCoors = new List<string>();
                                foreach (XmlNode item in arcProperty)
                                {
                                    
                                    if(item.Name == "ArchInnerGeometries")
                                    {
                                        if (item.HasChildNodes)
                                        {
                                            XmlNodeList xmlElements = item.ChildNodes;
                                            foreach (XmlNode item2 in xmlElements)
                                            {
                                                if (item2.Name == "ArchGeometry"&&item2.HasChildNodes)
                                                {
                                                    XmlNodeList xmlNodeList = item2.ChildNodes;
                                                    foreach (XmlNode item3 in xmlNodeList)
                                                    {
                                                        if (item3.Name == "GeoDescription")
                                                        {
                                                            //既包含Linestring也包含Polygon
                                                            if (item3.InnerText.Contains("LINESTRING") && item.InnerText.Contains("POLYGON"))
                                                            {
                                                                coordinates = ExtractPolygonCoord(item3.InnerText);//先提取双层括号polyon；
                                                                int index = item3.InnerText.IndexOf("((");
                                                                string content = item3.InnerText.Substring(0, index);//删除双层括号提取单层括号
                                                                LinearCoors = ExtractLinearCoord(content);//
                                                               

                                                            }
                                                            else if (item3.InnerText.Contains("LINESTRING") && !item.InnerText.Contains("POLYGON"))//包含线串另作处理
                                                            {
                                                                LinearCoors = ExtractLinearCoord(item.InnerText);
                                                               
                                                            }
                                                            
                                                            else
                                                            {
                                                                coordinates = ExtractCoord(item3.InnerText, out isHole);
                                                               
                                                            }
                                                            

                                                        }
                                                    }
                                                }
                                        }

                                      }        
                                    }
                                    arcBuilding.isHole = isHole;
                                    arcBuilding.coords = coordinates;
                                    arcBuilding.LinearCoords = LinearCoors;
                                }
                            
                            }
                        }
                        arcBuildings.Add(arcBuilding);
                    }
                }

                if (xnode.Name == "ArcRelations")
                {
                    DealRealtaionShip(xnode, arcBuildings);
                }
            }
            return arcBuildings;
        }
        /// <summary>
        /// 提取关系数据
        /// </summary>
        /// <param name="xmlNode"></param>
        public void DealRealtaionShip(XmlNode xmlNode,List<ArcBuilding> arcBuildings)
        {
            XmlElement xe = xmlNode as XmlElement;
            if (xe.HasChildNodes)
            {
                foreach (XmlNode item in xe.ChildNodes)
                {
                    if(item.Name== "ArchRelation")
                    {
                        XmlElement xe1 = item as XmlElement;
                        string RealtionType = xe1.GetAttribute("Type");
                        string FirstEntity = xe1.GetAttribute("EntityFirst");
                        string SecondEntity = xe1.GetAttribute("EntitySecond");
                        foreach (ArcBuilding arcBuilding in arcBuildings)
                        {
                            if (arcBuilding.ArcEntityId == FirstEntity)
                            {
                                Relation relation = new Relation();
                                relation.Name = RealtionType;
                                relation.sub = SecondEntity;
                                arcBuilding.subEntity.Add(relation);
                            }
                        }
                    }
                }
            }

        }
        public List<string> ExtractPolygonCoord(string content)
        {
           
            string pattern = @"\(\(.*?\)\)";//匹配两层括号获取polygon
            List<string> coordnates = new List<string>();

            foreach (Match match in Regex.Matches(content, pattern))
            {
                coordnates.Add(match.Value);//正则匹配
            }
            if (coordnates.Count > 1)
            {

                return coordnates;
            }
            return coordnates;  
        }
        public List<string> ExtractLinearCoord(string content)
        {
            List<string> coordnates = new List<string>();
            string pattern = @"\([^()]+\)";

            foreach (Match match in Regex.Matches(content, pattern))
            {
                List<string> listString = new List<string>();
                List<Coordinate> coordinates = new List<Coordinate>();
                string tempStr = match.Value;
                coordinates = ConvertCoordFromString(tempStr);//将匹配的字符串转化为坐标
                listString=calBuffer(coordinates, 10, "");//将坐标串转化为n组字符串
                coordnates.AddRange(listString);
            }   
            
            return coordnates;
        }
        public List<string> ExtractLinearCoord()
        {
            List<string> coordnates = new List<string>();
            string string1 = "";
            string pattern = @"\([^()]+\)";
            foreach (Match match in Regex.Matches(string1, pattern))
            {
                List<string> listString = new List<string>();
                List<Coordinate> coordinates = new List<Coordinate>();
                string tempStr = match.Value;
                coordinates = ConvertCoordFromString(tempStr);//将匹配的字符串转化为坐标
                coordinates = deleteOppositeVector(coordinates);
                listString = calBuffer(coordinates, 10, "");//将坐标串转化为n组字符串
                coordnates.AddRange(listString);
            }

            return coordnates;
        }
        /// <summary>
        /// 匹配一般的一个内层括号
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isHole"></param>
        /// <returns></returns>
        public List<string> ExtractCoord(string content,out bool isHole)
        {      
            string pattern = @"\([^()]+\)";

            //List<LineCircle> lineCircles = new List<LineCircle>();
            if (content.Contains("(((") && content.Contains(")))"))
            {
                isHole = true;
            }
            else { isHole = false; }
            List<string> coordnates = new List<string>();

            foreach (Match match in Regex.Matches(content, pattern))
                {
                    coordnates.Add(match.Value);//正则匹配
                }
            

            if (coordnates.Count > 1)
            {
              
                return coordnates;
            }
            return coordnates;

        }      
        public string ProcessShortLine(string content, double tolerance)
        {
            List<Coordinate> coords = ConvertCoordFromString(content);
            var beforeCount = coords.Count;
            string outPutString = "";
            for (int i = coords.Count-1; i > 0; i--)//倒着删保持索引不变
            {
                double dis = calcuDistance(coords[i], coords[i-1]);
                  if (dis < tolerance&&i-1!=0) //小于容差就删掉并且不是第一个坐标就删掉
                {
                    coords.Remove(coords[i-1]);
                }  
            }
            var zero = coords[0];
            var last = coords[coords.Count - 1];
            if (zero.x != last.x||zero.y!=last.y)
            {
                throw new Exception("首尾坐标不一致");
            }
            foreach (var item in coords)
            {
                outPutString += item.x + " " + item.y;
            }
                   
            return outPutString;
        }
        /// <summary>
        /// 处理一个字符串成为坐标串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<Coordinate> ConvertCoordFromString(string content)
        {
            char[] trimChar = new char[] { '(', ')' };
            string[] splitedString = content.Trim(trimChar).Split(',');
            List<Coordinate> coordinates = new List<Coordinate>();
            foreach (var item in splitedString)
            {
                Coordinate coordinate = new Coordinate();
                string[] coordx = item.Trim().Split(' ');
                coordinate.x = Convert.ToDouble(coordx[0]);
                coordinate.y = Convert.ToDouble(coordx[1]);
                coordinates.Add(coordinate);
            }
            return coordinates;

        }
      /// <summary>
      /// 坐标串转化为字符串,前后加括号
      /// </summary>
      /// <param name="coordinates"></param>
      /// <returns></returns>
        public string ConvertCoordToString( List<Coordinate> coordinates)
        {
            string outputString = "(";
            foreach (var item in coordinates)
            {
                outputString += item.x.ToString() + " " + item.y.ToString() + ",";
            }
            return outputString = outputString.Remove(outputString.Length - 1, 1) + ")";
        }
        public List<Coordinate> deleteOppositeVector(List<Coordinate> coordinates)
        {
            List<UnitVector> savedVectors = new List<UnitVector>();
            int flag = 0;
            if (coordinates.Count < 3)
            {
                throw new Exception("坐标数量小于三，无法进行判断");
            }
            //先判断第一个向量是不是等于最后一个向量
            UnitVector fistVector = calcuUnitVector(coordinates[0], coordinates[1]);
            //计算最后一条线的向量
            UnitVector lastVector = calcuUnitVector(coordinates[coordinates.Count - 2], coordinates[coordinates.Count - 1]);
            if ((Math.Abs(fistVector.X  +lastVector.X)<0.000001) && (Math.Abs(fistVector.Y + lastVector.Y)<0.00001))//第一条线的向量等于最后一条线的向量
            {
                coordinates.RemoveAt(coordinates.Count - 1);
                coordinates.RemoveAt(0);
                coordinates.Insert(0, coordinates[coordinates.Count - 1]);//删完首尾的点还要在头部再加一个和尾部相同的点
               
            }
            //判断中间线串的中间有没有出现自相交
            for (int i = 0; i < coordinates.Count - 3; i++)
            {
                if (JudgeDirection(coordinates[i],
                   coordinates[i + 1],
                   coordinates[i + 2]))//ture，方向相反
                {
                    coordinates[i + 1].x = (coordinates[i].x + coordinates[i + 2].x) / 2;
                    coordinates[i + 1].y = (coordinates[i].y + coordinates[i + 2].y) / 2;
                }

            }
            List<Segment> segments = new List<Segment>();
            List<Coordinate> deleteCoord = new List<Coordinate>();
            int count = 0;
            if (coordinates.Count > 2)
            {
                Segment segmentfirst = new Segment() { SegmentStart = coordinates[0], SegmentEnd = coordinates[1] };
                segments.Add(segmentfirst);
                for (int i = 1; i < coordinates.Count - 1; i++)
                {
                    foreach (var item in segments)
                    {
                        if(RelationWithSegment(item, coordinates[i]))//判断线段和坐标点的关系
                        {
                            deleteCoord.Add(coordinates[i]);
                        }
                        
                    }
                    Segment segment = new Segment() { SegmentStart = coordinates[i], SegmentEnd = coordinates[i+1] };
                    segments.Add(segment);

                }
            }
            for(int i=coordinates.Count-1;i>0;i--)
            {
                foreach (var deleteitem in deleteCoord)
                {
                    if (coordinates[i].x == deleteitem.x && coordinates[i].y == deleteitem.y)
                    {
                        coordinates.Remove(deleteitem);
                    }
                }
            }
         //计算第一条线的向量

            return coordinates;
        }
        private bool RelationWithSegment(Segment segment, Coordinate coordinate) {
         
            Vector vector1 = new Vector(coordinate.x-segment.SegmentStart.x,coordinate.y-segment.SegmentStart.y);
            Vector vector2 = new Vector(coordinate.x - segment.SegmentEnd.x, coordinate.y - segment.SegmentEnd.y);
            double degree= Vector.AngleBetween(vector1, vector2);
            double tolrance =Math.Abs(Math.Abs(degree) - 180);
            if (tolrance < 0.01) return true;
            else return false;
          
        }
        private bool RelationWithSegment1(Segment segment,Coordinate coordinate)
        {
        
            UnitVector vector = new UnitVector() { X = Math.Abs(coordinate.x - segment.SegmentStart.x)<0.1?0: coordinate.x - segment.SegmentStart.x, Y=Math.Abs(coordinate.y-segment.SegmentStart.y)<0.00001?0: coordinate.y - segment.SegmentStart.y };
            UnitVector vector1 = new UnitVector() { X = Math.Abs(coordinate.x - segment.SegmentEnd.x)<0.1?0:coordinate.x-segment.SegmentStart.x, Y = Math.Abs(coordinate.y - segment.SegmentEnd.y) < 0.00001 ? 0 : coordinate.y - segment.SegmentEnd.y };
            if (vector.X == 0 && vector1.X == 0)
            {
                if (vector.Y * vector1.Y < 0)//共线
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (vector.Y == 0 && vector1.Y == 0)//共线
            {
                if (vector.X * vector1.X < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }
        /// <summary>
        /// 计算三个点的外积
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns> 向量AC与向量AB的外积。如果结果为正数，表明点C在直线AB（直线方向为从A到B）的右侧；</returns>
        double cross(Coordinate A, Coordinate B, Coordinate C)
        {
            double cross1 = (C.x - A.x) * (B.y - A.y);
            double cross2 = (C.y - A.y) * (B.x - A.x);
            return (cross1 - cross2);
        }
        private bool JudgeDirection(UnitVector unitVector1,UnitVector unitVector2)
        {
            if ((Math.Abs(unitVector1.X + unitVector2.X) < 0.0000001) && (Math.Abs(unitVector1.Y + unitVector2.Y) < 0.0000001))//向量方向刚好相反
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断三个点形成的两个线的单位向量是否相反
        /// </summary>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <param name="coordinate3"></param>
        /// <returns></returns>
        private bool JudgeDirection(Coordinate coordinate1,Coordinate coordinate2, Coordinate coordinate3)
        {
            List<Coordinate> coordinates = new List<Coordinate>();
            UnitVector vector1 = calcuUnitVector(coordinate1, coordinate2);
            UnitVector vector2 = calcuUnitVector(coordinate2, coordinate3);
            if ((Math.Abs(vector1.X +vector2.X)<0.0000001) && (Math.Abs(vector1.Y  +vector2.Y)<0.0000001))//向量方向刚好相反
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 计算方向向量
        /// </summary>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        public UnitVector calcuUnitVector(Coordinate coordinate1,Coordinate coordinate2)
        {
            UnitVector vector = new UnitVector();
            vector.X= (coordinate1.x - coordinate2.x) / calcuDistance(coordinate1, coordinate2);
            vector.Y = (coordinate1.y - coordinate2.y) / calcuDistance(coordinate1, coordinate2);
            return vector;
        }
        /// <summary>
        /// 2个点会构造出来5个点的列表(形成一组坐标字符串)，3个点会构造10个坐标（形成两组坐标字符串）
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="s">生成的宽度</param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<string> calBuffer(List<Coordinate> coordinates,double s ,string Type)
        {
            List<string> coordinateList = new List<string>();
            for (int i=0; i< coordinates.Count - 1;i++)
            {
                Coordinate coordinate = coordinates[i];
                Coordinate coordinate1= coordinates[i+1];

                string coords= ConvertCoordToString(calBufferForPoint(coordinate, coordinate1, s));//生成的5个坐标转化为字符串
                coordinateList.Add(coords);
            }
            return coordinateList;
        }
        /// <summary>
        /// 计算两个坐标周围5个坐标
        /// </summary>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <returns></returns>
        public List<Coordinate> calBufferForPoint(Coordinate coordinate1,Coordinate coordinate2,double s)
        {
            List<Coordinate> coordinates = new List<Coordinate>();
            Coordinate coordinate3 = new Coordinate();
            Coordinate coordinate4 = new Coordinate();
            Coordinate coordinate5 = new Coordinate();
            Coordinate coordinate6 = new Coordinate();
            double x1 = coordinate1.x; double y1 = coordinate1.y;
            double x2 = coordinate2.x; double y2 = coordinate2.y;
            if (y1 - y2==0) {//线段是指向东西的，斜率为∞
                coordinate3.x = x1;
                coordinate3.y = y1 + s;
                coordinates.Add(coordinate3);

                coordinate4.x = x2;
                coordinate4.y = y2 + s;
                coordinates.Add(coordinate4);

                coordinate6.x = x2;
                coordinate6.y = y2 - s;
                coordinates.Add(coordinate6);

                coordinate5.x = x1;
                coordinate5.y = y1 - s;
                coordinates.Add(coordinate5);
                coordinates.Add(coordinate3);
                
            }
            else if (x1 - x2 == 0)//线段指向南北
            {
                coordinate3.x = x1 - s;
                coordinate3.y = y1;
                coordinates.Add(coordinate3);

                coordinate4.x = x1 + s;
                coordinate4.y = y1;
                coordinates.Add(coordinate4);

                coordinate6.x = x2 + s;
                coordinate6.y = y2;
                coordinates.Add(coordinate6);

                coordinate5.x = x2 - s;
                coordinate5.y = y2;
                coordinates.Add(coordinate5);

                coordinates.Add(coordinate3);
            }
            //计算第一个点的坐标
            else
            {
                coordinate3.x = -s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2)) + x1;
                coordinate3.y = (x1 - x2) / (y2 - y1) * (s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2))) + y1;
                coordinates.Add(coordinate3);


                coordinate4.x = s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2)) + x1;
                coordinate4.y = (x1 - x2) / (y2 - y1) * (-s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2))) + y1;
                coordinates.Add(coordinate4);

                coordinate5.x = -s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2)) + x2;
                coordinate5.y = (x1 - x2) / (y2 - y1) * (s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2))) + y2;
                coordinates.Add(coordinate5);


                coordinate6.x = s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2)) + x2;
                coordinate6.y = (x1 - x2) / (y2 - y1) * (-s / Math.Sqrt(1 + Math.Pow((x1 - x2) / (y2 - y1), 2))) + y2;
                coordinates.Add(coordinate6);

                coordinates.Add(coordinate3);
            }
            

            return coordinates;
        }
        /// <summary>
        /// 计算两点之间距离
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="nextCoordinate"></param>
        /// <returns></returns>
        public double calcuDistance(Coordinate coordinate,Coordinate nextCoordinate)
        {
           double dis= Math.Sqrt(Math.Pow((nextCoordinate.x - coordinate.x), 2) + Math.Pow((nextCoordinate.y - coordinate.y), 2));
           
            return dis;
            
        }
      
    }
    
}
