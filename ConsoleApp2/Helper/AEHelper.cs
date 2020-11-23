using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Model
{
   public class AEHelper
    {
           /// <summary>
            /// 临时的线转化为AE的线
            /// </summary>
            /// <param name="templateLines"></param>
            /// <returns></returns>
       static public List<PolylineWithId> CovertMyLineToPoline(List<TemplateLine> templateLines)
        {

            List<PolylineWithId> PolylineWithIds = new List<PolylineWithId>();
            foreach (TemplateLine line in templateLines)
            {
               
                PolylineWithId polyline = new PolylineWithId();
               
                foreach (var coords in line.coords)
                {
                    Point point = new Point() { X=coords.x,Y=coords.y};
                    polyline.polyline.AddPoint(point);
                    polyline.id = line.Id;
                   
                }
                PolylineWithIds.Add(polyline);
            }
            return PolylineWithIds;
        }
        /// <summary>
        ///把数据里的坐标处理为临时坐标
        /// </summary>
        /// <param name="arcBuildings"></param>
        /// <returns></returns>
      static  public List<TemplateLine> createMyTempline(List<ArcBuilding> arcBuildings)
        {

            ReadXmlHelper readXmlHelper = new ReadXmlHelper();
            List<TemplateLine> templateLines = new List<TemplateLine>();
            foreach (ArcBuilding arcBuilding in arcBuildings)
            {
                foreach (string coords in arcBuilding.coords)//遍历建筑物中的坐标串
                {
                    TemplateLine templateLine = new TemplateLine();
               
                    List<Coordinate> coordinates = new List<Coordinate>();
                    coordinates = readXmlHelper.ConvertCoordFromString(coords);//坐标字符串形成一个坐标串
                   // coordinates = readXmlHelper.deleteOppositeVector(coordinates);
                    templateLine.Id = arcBuilding.ArcEntityId;
                    templateLine.coords = (coordinates);

                    templateLines.Add(templateLine);
                }
                foreach (string coords in arcBuilding.LinearCoords)//遍历线串
                {
                    TemplateLine templateLine = new TemplateLine();

                    List<Coordinate> coordinates = new List<Coordinate>();
                    coordinates = readXmlHelper.ConvertCoordFromString(coords);//坐标字符串形成一个坐标串
                     // coordinates = readXmlHelper.deleteOppositeVector(coordinates);//删除有闭合回环的线
                    templateLine.Id = arcBuilding.ArcEntityId;
                    templateLine.coords = (coordinates);

                    templateLines.Add(templateLine);
                }
            }
            return templateLines;
        }
        /// <summary>
        /// 创建一个要素集
        /// </summary>
        /// <param name="shpfolder"></param>
        /// <param name="shpname"></param>
        /// <returns></returns>
        public static IFeatureClass CreatePolygonFeatureClass(string shpfolder, string shpname)
        {
            IWorkspaceFactory pWorkSpaceFac = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFeatureWorkSpace = pWorkSpaceFac.OpenFromFile(shpfolder, 0) as IFeatureWorkspace;

            //创建字段集2
            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;//创建必要字段
            IFields fields = ocDescription.RequiredFields;
            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
            IField field = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = field.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            //geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            //geometryDefEdit.SpatialReference_2 = spatialReference;

            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
            ISpatialReferenceFactory pSpatialRefFac = new SpatialReferenceEnvironmentClass();
            IProjectedCoordinateSystem pcsSys = pSpatialRefFac.CreateProjectedCoordinateSystem((int)esriSRProjCS4Type.esriSRProjCS_Xian1980_3_Degree_GK_Zone_39);
            geometryDefEdit.SpatialReference_2 = pcsSys;

            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null; //将传入字段 转成 validatedFields
            fieldChecker.ValidateWorkspace = (IWorkspace)pFeatureWorkSpace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            IFeatureClass featureClass = pFeatureWorkSpace.CreateFeatureClass(shpname, validatedFields, ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, fcDescription.ShapeFieldName, "");
            AddField(featureClass, "BuildID", "自带id", 50);
            return featureClass;

        }
      
        /// <summary>
        /// 向feature添加字段
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="fieldName"></param>
        /// <param name="AliasName"></param>
        /// <param name="length"></param>
        /// <returns></returns>
      static public bool AddField(IFeatureClass layer, string fieldName, string AliasName, int length)
        {
            try
            {
                ITable pTable = (ITable)layer;

                IFieldEdit pFieldEdit = new FieldClass()
                {
                    IFieldEdit_Type_2 = esriFieldType.esriFieldTypeString,
                    IFieldEdit_AliasName_2 = AliasName,
                    IFieldEdit2_Name_2 = fieldName,
                    IFieldEdit_Length_2 = 50,
                    IFieldEdit_IsNullable_2 = true
                };
                pTable.AddField(pFieldEdit);
                Console.WriteLine("添加字段成功");
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static void AddFeatureToFeatureClass(PolylineWithId feature, IFeatureClass pFeatureClass)
        {
            IFeature pFeature = pFeatureClass.CreateFeature();
            pFeature.Shape = feature.polyline as IGeometry;
            var field= pFeatureClass.Fields;
            pFeature.set_Value(3, feature.id);
            pFeature.Store();
        }

    }
}
