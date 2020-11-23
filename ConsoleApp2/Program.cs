using ConsoleApp2.Model;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            IAoInitialize m_aoinitialize = new AoInitializeClass();

            m_aoinitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine);
            
            ReadXmlHelper readXmlHelper = new ReadXmlHelper();
            List<ArcBuilding> list = readXmlHelper.readBuiInfoFromXml(@"G:\DB\S3-1.xml");//生成的建筑物列表
            //var group = list.GroupBy(item => item.ArcType).Select(x => x.Key).ToList();//统计
            //string myOut = "";
            //foreach (var item in group)
            //{
              
            //    foreach (var item1 in list)
            //    {
            //        if (!myOut.Contains(item1.ArcType))
            //        {
            //            myOut += item1.ArcType;
            //            foreach (var item2 in item1.Attributes)
            //            {
            //                myOut += item2.PropertyName+"  ";
            //            }
            //            myOut += "\n";
            //        }
            //    }
            //}
            List<TemplateLine> templateLines= AEHelper.createMyTempline(list);//生成我的临时线
           
            var features= AEHelper.CovertMyLineToPoline(templateLines);//把临时线转成AE中的线
              
            IFeatureClass featureClass= AEHelper.CreatePolygonFeatureClass( @"G:\DB", "line");

            foreach (var item in features)
            {
                if(item.id== "18958ab1-025b-4301-b8c9-897eb3ec18a1")
                    AEHelper.AddFeatureToFeatureClass(item, featureClass);
              

            }



        }
       
       
    }
}
