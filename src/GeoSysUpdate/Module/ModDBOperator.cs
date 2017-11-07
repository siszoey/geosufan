using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using SysCommon.Gis;
using SysCommon.Authorize;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace GeoSysUpdate
{
    public static class ModDBOperator
    {
        private static string _LogFilePath = Application.StartupPath + "\\..\\Log\\GeoSysUpdate.txt";
        public static void WriteLog(string strLog)
        {
            //�ж��ļ��Ƿ����  �����ھʹ������д��־�ĺ�����Ϊ�˲��Լ�����ʷ���ݵ�Ч��
            if (!File.Exists(_LogFilePath))
            {
                System.IO.FileStream pFileStream = File.Create(_LogFilePath);
                pFileStream.Close();
            }
            //FileStream fs = File.Open(_LogFilePath,FileMode.Append);

            //StreamReader reader = new StreamReader(fs, Encoding.GetEncoding("gb2312"));

            string strTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            string strread = strLog + "     " + strTime + "\r\n";
            StreamWriter sw = new StreamWriter(_LogFilePath, true, Encoding.GetEncoding("gb2312"));
            sw.Write(strread);
            sw.Close();
            //fs.Close();
            sw = null;
            //fs = null;
        }
        /// <summary>
        /// �����ͼ�ڵ�
        /// </summary>
        /// <param name="nodeCol"></param>
        /// <param name="strText"></param>
        /// <param name="strName"></param>
        /// <param name="pImage"></param>
        /// <param name="bExpand"></param>
        /// <returns></returns>
        ///         
        /// 

        public static DevComponents.AdvTree.Node CreateAdvTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand)
        {
            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            node.Text = strText;
            node.Image = pImage;
            if (strName != null)
            {
                node.Name = strName;
            }

            if (bExpand == true)
            {
                node.Expand();
            }

            nodeCol.Add(node);
            return node;
        }

        /// <summary>
        /// �����ͼ�ڵ���
        /// </summary>
        /// <param name="aNode"></param>
        /// <param name="strText"></param>
        /// <param name="pImage"></param>
        /// <returns></returns>
        public static DevComponents.AdvTree.Cell CreateAdvTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage)
        {
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell(strText);
            aCell.Images.Image = pImage;
            aNode.Cells.Add(aCell);

            return aCell;
        }

        //����XML�ڵ��ȡ������Ϣ
        public static object GetDBInfoByXMLNode(XmlElement dbElement, string strPath)
        {
            try
            {
                string strType = dbElement.GetAttribute("����");
                string strServer = dbElement.GetAttribute("������");
                string strInstance = dbElement.GetAttribute("������");
                string strDB = dbElement.GetAttribute("���ݿ�");
                if (strPath != "")
                {
                    strDB = strPath + strDB;
                }
                string strUser = dbElement.GetAttribute("�û�");
                string strPassword = dbElement.GetAttribute("����");
                string strVersion = dbElement.GetAttribute("�汾");

                IPropertySet pPropSet = null;
                switch (strType.Trim().ToLower())
                {
                    case "pdb":
                        pPropSet = new PropertySetClass();
                        AccessWorkspaceFactory pAccessFact = new AccessWorkspaceFactoryClass();
                        if (!File.Exists(strDB))
                        {
                            FileInfo filePDB = new FileInfo(strDB);
                            pAccessFact.Create(filePDB.DirectoryName, filePDB.Name, null, 0);
                        }
                        pPropSet.SetProperty("DATABASE", strDB);
                        IWorkspace pdbWorkspace = pAccessFact.Open(pPropSet, 0);
                        pAccessFact = null;
                        return pdbWorkspace;

                    case "gdb":
                        pPropSet = new PropertySetClass();
                        FileGDBWorkspaceFactoryClass pFileGDBFact = new FileGDBWorkspaceFactoryClass();
                        if (!Directory.Exists(strDB))
                        {
                            DirectoryInfo dirGDB = new DirectoryInfo(strDB);
                            pFileGDBFact.Create(dirGDB.Parent.FullName, dirGDB.Name, null, 0);
                        }
                        pPropSet.SetProperty("DATABASE", strDB);
                        IWorkspace gdbWorkspace = pFileGDBFact.Open(pPropSet, 0);
                        pFileGDBFact = null;
                        return gdbWorkspace;

                    case "sde":
                        pPropSet = new PropertySetClass();
                        IWorkspaceFactory pSdeFact = new SdeWorkspaceFactoryClass();
                        pPropSet.SetProperty("SERVER", strServer);
                        pPropSet.SetProperty("INSTANCE", strInstance);
                        pPropSet.SetProperty("DATABASE", strDB);
                        pPropSet.SetProperty("USER", strUser);
                        pPropSet.SetProperty("PASSWORD", strPassword);
                        pPropSet.SetProperty("VERSION", strVersion);
                        IWorkspace sdeWorkspace = pSdeFact.Open(pPropSet, 0);
                        pSdeFact = null;
                        return sdeWorkspace;

                    case "access":
                        System.Data.Common.DbConnection dbCon = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDB);
                        dbCon.Open();
                        return dbCon;

                    case "oracle":
                        string strOracle = "Data Source=" + strDB + ";Persist Security Info=True;User ID=" + strUser + ";Password=" + strPassword + ";Unicode=True";
                        System.Data.Common.DbConnection dbConoracle = new OracleConnection(strOracle);
                        dbConoracle.Open();
                        return dbConoracle;

                    case "sql":
                        string strSql = "Data Source=" + strDB + ";Initial Catalog=" + strInstance + ";User ID=" + strUser + ";Password=" + strPassword;
                        System.Data.Common.DbConnection dbConsql = new SqlConnection(strSql);
                        dbConsql.Open();
                        return dbConsql;

                    default:
                        break;
                }

                return null;
            }
            catch (Exception e)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                return null;
            }
        }
        /// <summary>
        /// ��mapcontrol�ϻ�ȡͼ����ϱ�
        /// </summary>
        /// <param name="StrScale"></param>
        /// <param name="pMapcontrol"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static ILayer GetMapFrameLayer(string StrScale,IMapControlDefault pMapcontrol, string strName)
        {
            for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)
            {
                ILayer pLayer = pMapcontrol.Map.get_Layer(i);
                IGroupLayer pGroupLayer = pLayer as IGroupLayer;
                if (pGroupLayer == null) continue;
                if (pGroupLayer.Name != strName) continue;
                ICompositeLayer pCompositeLayer = pGroupLayer as ICompositeLayer;
                if (pCompositeLayer.Count == 0) return null;
                for (int j = 0; j < pCompositeLayer.Count; j++)
                {
                    pLayer = pCompositeLayer.get_Layer(j);
                    if (pLayer is IFeatureLayer)
                    {
                        IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                        IDataset pDataset=pFeatureLayer.FeatureClass as IDataset;
                        if (pDataset.Name.Contains(StrScale))
                        {
                            return pLayer;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ��mapcontrol�ϻ�ȡͼ����ϱ�
        /// </summary>
        /// <param name="StrScale"></param>
        /// <param name="pMapcontrol"></param>
        /// <param name="strName"></param>
        /// <param name="pGroupLayer"></param>
        /// <returns></returns>
        public static ILayer GetMapFrameLayer(string StrScale, IMapControlDefault pMapcontrol, string strName, out IGroupLayer pGroupLayer)
        {
            pGroupLayer = null;
            for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)
            {
                ILayer pLayer = pMapcontrol.Map.get_Layer(i);
                pGroupLayer = pLayer as IGroupLayer;
                if (pGroupLayer == null) continue;
                if (pGroupLayer.Name != strName) continue;
                ICompositeLayer pCompositeLayer = pGroupLayer as ICompositeLayer;
                if (pCompositeLayer.Count == 0) return null;
                for (int j = 0; j < pCompositeLayer.Count; j++)
                {
                    pLayer = pCompositeLayer.get_Layer(j);
                    if (pLayer is IFeatureLayer)
                    {
                        IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                        IDataset pDataset = pFeatureLayer.FeatureClass as IDataset;
                        if (pDataset.Name.Contains(StrScale))
                        {
                            return pLayer;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ���ݸ���ͼ���Ż�ȡ���·�Χ
        /// </summary>
        /// <param name="pRangeFeatLay"></param>
        /// <returns></returns>
        public static IGeometry GetGeometry(IFeatureLayer pRangeFeatLay)
        {
            IFeatureSelection pFeatSel = pRangeFeatLay as IFeatureSelection;
            if (pFeatSel.SelectionSet.Count == 0) return null;
            IEnumIDs pEnumIDs = pFeatSel.SelectionSet.IDs;
            int ID = pEnumIDs.Next();
            IGeometry pUnionGeo = null;
            while (ID != -1)
            {
                IFeature pFeat = pRangeFeatLay.FeatureClass.GetFeature(ID);
                if (pUnionGeo == null)
                {
                    pUnionGeo = pFeat.Shape;
                }
                else
                {
                    ITopologicalOperator pTop = pUnionGeo as ITopologicalOperator;
                    pUnionGeo = pTop.Union(pFeat.Shape);
                }

                ID = pEnumIDs.Next();
            }
            return pUnionGeo;
        }

        /// <summary>
        /// ��Ⱦ�������� ��Ⱦ�����ֶ� ��־��¼��.state 1-�½�,2-�޸�,3-ɾ��
        /// </summary>
        /// <param name="pFeatureLayer"></param>
        /// <param name="strFieldName"></param>
        public static void SetDataUpdateSymbol(IFeatureLayer pFeatureLayer, string strFieldName)
        {
            if (pFeatureLayer == null || strFieldName == string.Empty) return;
            Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();
            Dictionary<string, ISymbol> dicFieldSymbol = new Dictionary<string, ISymbol>();


            ISymbol pSymbol = null;
            dicFieldValue.Add("1", "�½�");
            pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 255, 192, 203);
            dicFieldSymbol.Add("1", pSymbol);

            dicFieldValue.Add("2", "ɾ��");
            pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 255, 7, 7);
            dicFieldSymbol.Add("2", pSymbol);

            dicFieldValue.Add("3", "�޸�");
            pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 35, 254, 7); ;
            dicFieldSymbol.Add("3", pSymbol);

            dicFieldValue.Add("<Null>", "δ�仯");
            pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 245, 245, 194); ;
            dicFieldSymbol.Add("<Null>", pSymbol);

            SysCommon.Gis.ModGisPub.SetLayerUniqueValueRenderer(pFeatureLayer, strFieldName, dicFieldValue, dicFieldSymbol, false);
        }

        private static ISymbol CreateSymbol(esriGeometryType pGeometryType, int intR, int intG, int intB)
        {
            ISymbol pSymbol = null;
            ISimpleLineSymbol pSimpleLineSymbol = null;
            IColor pColor = SysCommon.Gis.ModGisPub.GetRGBColor(intR, intG, intB);
            switch (pGeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSimpleLineSymbol.Color = SysCommon.Gis.ModGisPub.GetRGBColor(156, 156, 156);
                    pSimpleLineSymbol.Width = 0.0001;
                    ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                    pSimpleFillSymbol.Outline = pSimpleLineSymbol;
                    pSimpleFillSymbol.Color = pColor;
                    pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    pSymbol = pSimpleFillSymbol as ISymbol;
                    break;
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                    pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    pSimpleMarkerSymbol.Color = pColor;
                    pSimpleMarkerSymbol.Size = 1;
                    pSymbol = pSimpleMarkerSymbol as ISymbol;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSimpleLineSymbol.Color = pColor;
                    pSimpleLineSymbol.Width = 0.1;
                    pSymbol = pSimpleLineSymbol as ISymbol;
                    break;
            }

            return pSymbol;
        }

        /// <summary>
        /// ���ݹ���XML�п���ṹ�ڵ㴴������
        /// </summary>
        /// <param name="feaName"></param>
        /// <param name="featureDataset"></param>
        /// <param name="feaworkspace"></param>
        /// <param name="fsEditAnno"></param>
        /// <param name="intScale"></param>
        public static void createAnnoFeatureClass(string feaName, IFeatureDataset featureDataset, IFeatureWorkspace feaworkspace, IFieldsEdit fsEditAnno, string intScale)
        {
            //����ע�ǵ������ֶ�
            try
            {
                //ע�ǵ�workSpace
                IFeatureWorkspaceAnno pFWSAnno = feaworkspace as IFeatureWorkspaceAnno;

                IGraphicsLayerScale pGLS = new GraphicsLayerScaleClass();
                pGLS.Units = esriUnits.esriMeters;
                pGLS.ReferenceScale = Convert.ToDouble(500);//����ע�Ǳ���Ҫ���ñ�����

                IFormattedTextSymbol myTextSymbol = new TextSymbolClass();
                ISymbol pSymbol = (ISymbol)myTextSymbol;
                //AnnoҪ��������е�ȱʡ����
                ISymbolCollection2 pSymbolColl = new SymbolCollectionClass();
                ISymbolIdentifier2 pSymID = new SymbolIdentifierClass();
                pSymbolColl.AddSymbol(pSymbol, "Default", out pSymID);

                //AnnoҪ����ı�Ҫ����
                IAnnotateLayerProperties pAnnoProps = new LabelEngineLayerPropertiesClass();
                pAnnoProps.CreateUnplacedElements = true;
                pAnnoProps.CreateUnplacedElements = true;
                pAnnoProps.DisplayAnnotation = true;
                pAnnoProps.UseOutput = true;

                ILabelEngineLayerProperties pLELayerProps = (ILabelEngineLayerProperties)pAnnoProps;
                pLELayerProps.Symbol = pSymbol as ITextSymbol;
                pLELayerProps.SymbolID = 0;
                pLELayerProps.IsExpressionSimple = true;
                pLELayerProps.Offset = 0;
                pLELayerProps.SymbolID = 0;

                IAnnotationExpressionEngine aAnnoVBScriptEngine = new AnnotationVBScriptEngineClass();
                pLELayerProps.ExpressionParser = aAnnoVBScriptEngine;
                pLELayerProps.Expression = "[DESCRIPTION]";
                IAnnotateLayerTransformationProperties pATP = (IAnnotateLayerTransformationProperties)pAnnoProps;
                pATP.ReferenceScale = pGLS.ReferenceScale;
                pATP.ScaleRatio = 1;

                IAnnotateLayerPropertiesCollection pAnnoPropsColl = new AnnotateLayerPropertiesCollectionClass();
                pAnnoPropsColl.Add(pAnnoProps);

                IObjectClassDescription pOCDesc = new AnnotationFeatureClassDescription();
                IFields fields = pOCDesc.RequiredFields;
                IFeatureClassDescription pFDesc = pOCDesc as IFeatureClassDescription;

                for (int j = 0; j < pOCDesc.RequiredFields.FieldCount; j++)
                {
                    fsEditAnno.AddField(pOCDesc.RequiredFields.get_Field(j));
                }
                fields = fsEditAnno as IFields;
                pFWSAnno.CreateAnnotationClass(feaName, fields, pOCDesc.InstanceCLSID, pOCDesc.ClassExtensionCLSID, pFDesc.ShapeFieldName, "", featureDataset, null, pAnnoPropsColl, pGLS, pSymbolColl, true);
            }
            catch
            {

            }
        }

        /// <summary>
        /// ����Ҫ����
        /// </summary>
        /// <param name="ruleXML"></param>
        /// <param name="feaworkspace"></param>
        /// <param name="intScale"></param>
        /// <returns></returns>
        public static bool createFeatureClass(XmlDocument ruleXML, IFeatureWorkspace feaworkspace, string intScale)
        {
            try
            {
                //��ȡ��Ŀ�����ṹ�ڵ㡱
                XmlNodeList tempNodeList = ruleXML.GetElementsByTagName("Ŀ�����ṹ");

                //�����ռ�ο���ͨ����ȡxml�ļ�����ÿռ�ο���·��
                XmlElement spatialElement = tempNodeList.Item(0).SelectSingleNode(".//�ռ�ο�") as XmlElement;
                string spatialPath = spatialElement.GetAttribute("·��");
                spatialPath = Application.StartupPath + "\\" + spatialPath;
                ISpatialReferenceFactory pSpaReferenceFac = new SpatialReferenceEnvironmentClass();//�ռ�ο�����
                ISpatialReference pSpatialReference = null;//������ÿռ�ο�
                if (File.Exists(spatialPath))
                {
                    pSpatialReference = pSpaReferenceFac.CreateESRISpatialReferenceFromPRJFile(spatialPath);
                }

                if (pSpatialReference == null)
                {
                    pSpatialReference = new UnknownCoordinateSystemClass();
                }

                //����Ĭ�ϵ�Resolution
                ISpatialReferenceResolution pSpatiaprefRes = pSpatialReference as ISpatialReferenceResolution;
                pSpatiaprefRes.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon 
                pSpatiaprefRes.SetDefaultXYResolution();
                pSpatiaprefRes.SetDefaultZResolution();
                pSpatiaprefRes.SetDefaultMResolution();
                //����Ĭ�ϵ�Tolerence
                ISpatialReferenceTolerance pSpatialrefTole = pSpatiaprefRes as ISpatialReferenceTolerance;
                pSpatialrefTole.SetDefaultXYTolerance();
                pSpatialrefTole.SetDefaultZTolerance();
                pSpatialrefTole.SetDefaultMTolerance();

                //�������ݼ�
                IFeatureDataset pFeatureDataset = null;//�������ݼ�����װ��Ҫ����
                pFeatureDataset = feaworkspace.CreateFeatureDataset("DLG", pSpatialReference);

                #region ͨ����ȡxml�ļ������Ҫ������Ϣ
                tempNodeList = tempNodeList.Item(0).SelectNodes(".//Ŀ������");
                XmlNode newFeatureNode = null;
                string shapestr = "Shape";

                for (int i = 0; i < tempNodeList.Count; i++)
                {
                    newFeatureNode = tempNodeList.Item(i);
                    string feaName = newFeatureNode["����"].InnerText;
                    string feaType = newFeatureNode["����"].InnerText;
                    XmlNodeList fieldNodeList = newFeatureNode.SelectNodes(".//�ֶ�");
                    IFields fields = new FieldsClass();
                    IFieldsEdit fsEdit = fields as IFieldsEdit;

                    for (int j = 0; j < fieldNodeList.Count; j++)
                    {
                        //����xml�ļ������û��Զ�����ֶ�
                        IField newfield = new FieldClass();
                        IFieldEdit fieldEdit = newfield as IFieldEdit;
                        XmlNode fieldNode = fieldNodeList.Item(j);
                        fieldEdit.Name_2 = fieldNode["����"].InnerText;
                        fieldEdit.AliasName_2 = fieldNode["����"].InnerText;
                        fieldEdit.Type_2 = (esriFieldType)Enum.Parse(typeof(esriFieldType), fieldNode["����"].InnerText, true);
                        if (fieldNode["����"].InnerText.Trim() != "")
                        {
                            fieldEdit.Length_2 = int.Parse(fieldNode["����"].InnerText);
                        }
                        if (fieldNode["Ĭ��ֵ"].InnerText.Trim() != "")
                        {
                            fieldEdit.DefaultValue_2 = fieldNode["Ĭ��ֵ"].InnerText;
                        }
                        fieldEdit.IsNullable_2 = !bool.Parse(fieldNode["�Ƿ����"].InnerText);
                        newfield = fieldEdit as IField;
                        fsEdit.AddField(newfield);
                    }

                    if (feaType == "ע��")
                    {
                        //ע��
                        createAnnoFeatureClass(feaName, pFeatureDataset, feaworkspace, fsEdit, intScale);
                    }
                    else
                    {
                        //featureClass�������ֶ�
                        //���Object�ֶ�
                        IField newfield2 = new FieldClass();
                        IFieldEdit fieldEdit2 = newfield2 as IFieldEdit;
                        fieldEdit2.Name_2 = "OBJECTID";
                        fieldEdit2.Type_2 = esriFieldType.esriFieldTypeOID;
                        fieldEdit2.AliasName_2 = "OBJECTID";
                        newfield2 = fieldEdit2 as IField;
                        fsEdit.AddField(newfield2);

                        //���Geometry�ֶ�
                        IField newfield1 = new FieldClass();
                        IFieldEdit fieldEdit1 = newfield1 as IFieldEdit;
                        fieldEdit1.Name_2 = shapestr;
                        fieldEdit1.Type_2 = esriFieldType.esriFieldTypeGeometry;
                        IGeometryDef geoDef = new GeometryDefClass();
                        IGeometryDefEdit geoDefEdit = geoDef as IGeometryDefEdit;
                        if (feaType == "��")
                        {
                            geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                            geoDefEdit.HasZ_2 = true;
                        }
                        else if (feaType == "��")
                        {
                            geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
                        }
                        else if (feaType == "��")
                        {
                            geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
                        }
                        ISpatialReference pSR = pSpatialReference;
                        //ISpatialReferenceResolution pSRR = pSR as ISpatialReferenceResolution;
                        //pSRR.SetDefaultXYResolution();
                        //ISpatialReferenceTolerance pSRT = (ISpatialReferenceTolerance)pSR;
                        //pSRT.SetDefaultXYTolerance();
                        geoDefEdit.SpatialReference_2 = pSR;
                        fieldEdit1.GeometryDef_2 = geoDefEdit as GeometryDef;
                        newfield1 = fieldEdit1 as IField;
                        fsEdit.AddField(newfield1);
                        fields = fsEdit as IFields;

                        pFeatureDataset.CreateFeatureClass(feaName, fields, null, null, esriFeatureType.esriFTSimple, shapestr, "");
                    }

                }
                #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// �����ݹ���XML�л�ȡ����XML
        /// </summary>
        /// <param name="dbProjectXML"></param>
        /// <param name="strRuleName"></param>
        /// <returns></returns>
        public static XmlDocument GetRuleXmlDocument(XmlDocument dbProjectXML, string strRuleName)
        {
            //��ȡ���ݹ���·��
            XmlElement RootElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����");
            string ProjectPath = RootElement.GetAttribute("·��");
            XmlElement aElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����//����//" + strRuleName);
            if (aElement == null) return null;
            //��ȡ�����ݹ�������·��
            string RulePath = aElement.GetAttribute("·��");
            //��ȡ���ݹ��������·��
            RulePath = ProjectPath + RulePath;

            if (!File.Exists(RulePath)) return null;

            try
            {
                XmlDocument outputXml = new XmlDocument();
                outputXml.Load(RulePath);
                return outputXml;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// �����ݹ���XML�л�ȡ����XML
        /// </summary>
        /// <param name="dbProjectXML"></param>
        /// <param name="strRuleName"></param>
        /// <returns></returns>
        public static string GetRuleXmlPath(XmlDocument dbProjectXML, string strRuleName)
        {
            //��ȡ���ݹ���·��
            XmlElement RootElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����");
            string ProjectPath = RootElement.GetAttribute("·��");
            XmlElement aElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����//����//" + strRuleName);
            if (aElement == null) return "";
            //��ȡ�����ݹ�������·��
            string RulePath = aElement.GetAttribute("·��");
            //��ȡ���ݹ��������·��
            RulePath = ProjectPath + RulePath;

            if (!File.Exists(RulePath)) return "";
            return RulePath;
        }

        public static bool CreatExtractDB(XmlDocument ExtractXml, string intScale, string objName,out Exception eError)
        {
            bool exist;
            bool result = false;
            eError = null;
            if (ExtractXml == null) return false;

            //----------��ȡ��Ŀ�����ļ�������ȡ���ݵĿ���-------------------------------------
            IWorkspace TempWorkSpace = ModDBOperator.GetWorkSpace(objName, out exist);
            if (TempWorkSpace == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ" + objName + "��������ʧ��!");
                return false;
            }
            IFeatureWorkspace pFeatureWorkSpace = TempWorkSpace as IFeatureWorkspace;
            if (!exist)
            {
                //�½����ؿ�
                result = ModDBOperator.createFeatureClass(ExtractXml, pFeatureWorkSpace, intScale);
            }
            else
            {
                result = true;
            }
            //�ͷŹ����ռ�
            if (pFeatureWorkSpace != null)
            {
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject((pFeatureWorkSpace as IWorkspace).WorkspaceFactory);
                ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pFeatureWorkSpace);
                pFeatureWorkSpace = null;
            }
            return result;
        }

        /// <summary>
        /// ������ݲ����ڵ�
        /// </summary>
        /// <param name="dbProjectXML"></param>
        /// <param name="strNodeName"></param>
        /// <param name="orgElement"></param>
        /// <param name="bOrgTemp"></param>
        /// <param name="objElement"></param>
        /// <param name="bObjTemp"></param>
        /// <param name="outputRulePath"></param>
        /// <returns></returns>
        public static bool AddDataOperatorXmlNode(XmlDocument dbProjectXML, string strNodeName, XmlElement orgElement,string orgDataSetName,bool bOrgTemp, XmlElement objElement,bool bObjTemp, string outputRulePath)
        {
            XmlNode DBNode = dbProjectXML.SelectSingleNode(".//��Ŀ����");
            XmlNode aNode = SelectNode(DBNode, strNodeName);
            //����Ѵ��ڵ��ӽڵ�
            aNode.RemoveAll();
            XmlNode sourceNode = SelectNode(aNode, "Դ��������");
            XmlElement dataElement = dbProjectXML.CreateElement("Դ����");
            XmlElement DBElement = DBNode as XmlElement;
            dataElement.SetAttribute("����", orgElement.GetAttribute("����"));
            if (!bOrgTemp)
            {
               dataElement.SetAttribute("������", orgElement.GetAttribute("������"));
            }
            else
            {
                dataElement.SetAttribute("������", DBElement.GetAttribute("·��") + orgElement.GetAttribute("������")); //���¿���ΪGDB���ҷ�����Ϊ���·��
            }
            dataElement.SetAttribute("���ݼ�", orgDataSetName);
            dataElement.SetAttribute("������", orgElement.GetAttribute("������"));
            dataElement.SetAttribute("�û�", orgElement.GetAttribute("�û�"));
            dataElement.SetAttribute("����", orgElement.GetAttribute("����"));
            dataElement.SetAttribute("�汾", orgElement.GetAttribute("�汾"));
            sourceNode.AppendChild((XmlNode)dataElement);

            XmlElement objElementTemp = SelectNode(aNode, "Ŀ����������") as XmlElement;
            objElementTemp.SetAttribute("����", objElement.GetAttribute("����"));
            if (!bObjTemp)
            {
                objElementTemp.SetAttribute("������", objElement.GetAttribute("������"));
            }
            else
            {
                objElementTemp.SetAttribute("������", DBElement.GetAttribute("·��") + objElement.GetAttribute("������")); //���¿���ΪGDB���ҷ�����Ϊ���·��
            }
            objElementTemp.SetAttribute("������", objElement.GetAttribute("������"));
            objElementTemp.SetAttribute("�û�", objElement.GetAttribute("�û�"));
            objElementTemp.SetAttribute("����", objElement.GetAttribute("����"));
            objElementTemp.SetAttribute("�汾", objElement.GetAttribute("�汾"));

            if (outputRulePath == "") return false;
            XmlNode RuleNode = SelectNode(aNode, "����");
            XmlElement RuleElement = RuleNode as XmlElement;
            RuleElement.SetAttribute("·��", outputRulePath);

            return true;
        }

        public static bool AddDataOperatorXmlNode(XmlDocument dbProjectXML, string strNodeName, string strOrgPath, string strObjPath, string outputRulePath, string strOrgType, string strObjType)
        {
            XmlNode DBNode = dbProjectXML.SelectSingleNode(".//��Ŀ����");
            XmlNode aNode = SelectNode(DBNode, strNodeName);
            //����Ѵ��ڵ��ӽڵ�
            aNode.RemoveAll();
            XmlNode sourceNode = SelectNode(aNode, "Դ��������");
            XmlElement dataElement = dbProjectXML.CreateElement("Դ����");
            XmlElement DBElement = DBNode as XmlElement;
            dataElement.SetAttribute("����", strOrgType);
            dataElement.SetAttribute("������", strOrgPath);
            dataElement.SetAttribute("������", "");
            dataElement.SetAttribute("�û�", "");
            dataElement.SetAttribute("����", "");
            dataElement.SetAttribute("�汾", "");
            sourceNode.AppendChild((XmlNode)dataElement);

            XmlElement objElement = SelectNode(aNode, "Ŀ����������") as XmlElement;
            objElement.SetAttribute("����", strObjType);
            objElement.SetAttribute("������", strObjPath);
            objElement.SetAttribute("������", "");
            objElement.SetAttribute("�û�", "");
            objElement.SetAttribute("����", "");
            objElement.SetAttribute("�汾", "");

            if (outputRulePath == "") return false;
            XmlNode RuleNode = SelectNode(aNode, "����");
            XmlElement RuleElement = RuleNode as XmlElement;
            RuleElement.SetAttribute("·��", outputRulePath);

            return true;
        }

        /// <summary>
        /// ѡ��XMLĳ�ڵ����������򴴽�
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        private static XmlNode SelectNode(XmlNode parentNode, string strName)
        {
            XmlNode aNode = parentNode.SelectSingleNode(".//" + strName);
            if (aNode == null)
            {
                aNode = parentNode.OwnerDocument.CreateElement(strName) as XmlNode;
            }

            parentNode.AppendChild(aNode);
            return aNode;
        }

        /// <summary>
        /// ����XML���ӽڵ��ȡworkspace���������򴴽�
        /// </summary>
        /// <param name="pWorkSpaceNode"></param>
        /// <returns></returns>
        public static IWorkspace GetWorkSpace(XmlNode pWorkSpaceNode,out bool bExit)
        {
            bExit = false;
            try
            {
                IWorkspace TempWorkSpace = null;
                XmlElement TempElement = pWorkSpaceNode as XmlElement;
                string strType = TempElement.GetAttribute("����");

                switch (strType.Trim().ToUpper())
                {
                    case "PDB":
                    case "GDB":
                        IWorkspaceFactory pWorkspaceFactory = null;
                        if (strType.Trim().ToUpper() == "PDB")
                        {
                            pWorkspaceFactory = new AccessWorkspaceFactoryClass();
                            bExit = File.Exists(TempElement.GetAttribute("������"));
                        }
                        else if (strType.Trim().ToUpper() == "GDB")
                        {
                            pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                            bExit = Directory.Exists(TempElement.GetAttribute("������"));
                        }


                        if (!bExit)
                        {
                            FileInfo finfo = new FileInfo(TempElement.GetAttribute("������"));
                            string outputDBPath = finfo.DirectoryName;
                            string outputDBName = finfo.Name;
                            outputDBName = outputDBName.Substring(0, outputDBName.Length - 4);
                            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(outputDBPath, outputDBName, null, 0);
                            IName pName = (IName)pWorkspaceName;
                            TempWorkSpace = (IWorkspace)pName.Open();
                        }
                        else
                        {
                            TempWorkSpace = pWorkspaceFactory.OpenFromFile(TempElement.GetAttribute("������"), 0);
                        }

                        break;
                    case "ORACLE":
                    case "SQLSERVER":
                        IWorkspaceFactory pSDEWorkspaceFactory = new SdeWorkspaceFactoryClass();
                        IPropertySet pPropSet = new PropertySetClass();
                        pPropSet.SetProperty("SERVER", TempElement.GetAttribute("������"));
                        pPropSet.SetProperty("INSTANCE", TempElement.GetAttribute("������"));
                        pPropSet.SetProperty("DATABASE", "");
                        pPropSet.SetProperty("USER", TempElement.GetAttribute("�û�"));
                        pPropSet.SetProperty("PASSWORD", TempElement.GetAttribute("����"));
                        pPropSet.SetProperty("VERSION", TempElement.GetAttribute("�汾"));

                        TempWorkSpace = pSDEWorkspaceFactory.Open(pPropSet, 0);
                        break;
                }

                return TempWorkSpace;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ����XML���ӽڵ��ȡworkspace���������򴴽�
        /// </summary>
        /// <param name="pWorkSpaceNode"></param>
        /// <returns></returns>
        public static IWorkspace GetWorkSpace(string objName, out bool bExit)
        {
            bExit = false;
            try
            {
                IWorkspace TempWorkSpace = null;

                IWorkspaceFactory pWorkspaceFactory = null;
                
                pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                bExit = Directory.Exists(objName);

                if (!bExit)
                {
                    FileInfo finfo = new FileInfo(objName);
                    string outputDBPath = finfo.DirectoryName;
                    string outputDBName = finfo.Name;
                    outputDBName = outputDBName.Substring(0, outputDBName.Length - 4);
                    IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(outputDBPath, outputDBName, null, 0);
                    IName pName = (IName)pWorkspaceName;
                    TempWorkSpace = (IWorkspace)pName.Open();
                }
                else
                {
                    TempWorkSpace = pWorkspaceFactory.OpenFromFile(objName, 0);
                }

                return TempWorkSpace;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ���ݸ���ͼ���Ż�ȡ���·�Χ
        /// </summary>
        /// <param name="MapNumArray"></param>
        /// <param name="pRangeFeatLay"></param>
        /// <returns></returns>        
        public static IGeometry GetGeometry(string[] MapNumArray, IFeatureLayer pRangeFeatLay)
        {
            if (pRangeFeatLay == null) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MapNumArray.Length; i++)
            {
                if (sb.Length != 0)
                {
                    sb.Append(",");
                }
                sb.Append("'");
                sb.Append(MapNumArray.GetValue(i).ToString());
                sb.Append("'");
            }
            IQueryFilter pQueryFilter = new QueryFilterClass();
            if (sb.Length != 0)
            {
                pQueryFilter.WhereClause = "[MAP_NEWNO] in (" + sb.ToString() + ")";
            }
            else
            {
                pQueryFilter.WhereClause = "";
            }

            IFeatureClass pFeatureClass = pRangeFeatLay.FeatureClass;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(pQueryFilter, false);

            //ѡ��ͼ��
            IFeatureSelection pFeatSel = pRangeFeatLay as IFeatureSelection;
            pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

            IGeometryBag pGeometryBag = new GeometryBagClass();
            IGeoDataset pGeoDataSet = pFeatureClass as IGeoDataset;
            pGeometryBag.SpatialReference = pGeoDataSet.SpatialReference;

            IGeometryCollection pGeometryCollection = pGeometryBag as IGeometryCollection;
            IFeature pFeature = null;
            pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                object missing = Type.Missing;
                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    pGeometryCollection.AddGeometry(pFeature.Shape, ref missing, ref missing);
                }

                pFeature = pFeatureCursor.NextFeature();
            }

            ITopologicalOperator UnionedPolygon = new PolygonClass();
            UnionedPolygon.ConstructUnion(pGeometryBag as IEnumGeometry);

            IGeometry UnionedGeometry = UnionedPolygon as IGeometry;
            return UnionedGeometry;
        }

        /// <summary>
        /// ������Ŀ�����ļ���ȡͼ����
        /// </summary>
        /// <param name="ProjectXml"></param>
        /// <returns></returns>
        public static string[] GetMapNoByXml(XmlDocument ProjectXml)
        {
            XmlNodeList MapNoList = null;
            MapNoList = ProjectXml.GetElementsByTagName("ͼ��");
            XmlNode MapNoNode = null;
            XmlElement MapNoElement = null;
            string MapNo = "";
            string[] MapNoArray = new string[MapNoList.Count];

            for (int i = 0; i < MapNoList.Count; i++)
            {
                MapNoNode = MapNoList[i];
                MapNoElement = (XmlElement)MapNoNode;
                MapNo = MapNoElement.GetAttribute("ͼ����");
                MapNoArray.SetValue(MapNo, i);
            }
            return MapNoArray;
        }
        
        /// <summary>
        /// ���ݱ����ߵ������������ط�Χ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="MapControl"></param>
        /// <param name="sysDataSet"></param>
        /// <param name="eError"></param>
        public static void AddMapLayer(string name, IQueryFilter filter,IQueryFilter foreignFilter,IMapControlDefault MapControl,SysCommon.Gis.SysGisDataSet sysDataSet,out Exception eError)
        {
            eError=null;
            bool result = false;
            bool isExistLayer=true;
            IWorkspace pWorkSpace = null;
            IFeatureWorkspace pFeatureWorkSpace;
            IFeatureDataset pFeatureDataset;
            IEnumDataset pEnumDataset;
            IDataset pDataset;
            IFeatureClass pFeatureClass;
            IFeatureClass pFeatureJoinClass;
            IFeatureLayer pFeatureLayer;
            ILayer pLayer = null;
            IGroupLayer pGroupLayer;
            string LayerName;

            try
            {
                pWorkSpace = sysDataSet.WorkSpace;
                if (pWorkSpace != null)
                {
                    pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

                    pFeatureDataset = sysDataSet.GetFeatureDataset("NJZW.assistant", out eError);
                    pDataset = pFeatureDataset as IFeatureDataset;
                    pEnumDataset = pDataset.Subsets;
                    //�о�Ҫ�������ƣ����ص��ؼ���
                    pEnumDataset.Reset();
                    pDataset = pEnumDataset.Next();
                    pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
                    if (pLayer == null && pGroupLayer == null)
                    {
                        pGroupLayer = new GroupLayerClass();
                        pGroupLayer.Name = "ʾ��ͼ";
                        isExistLayer = false;
                    }
                    while (pDataset != null)
                    {
                        if (pDataset is IFeatureClass)
                        {
                            LayerName = pDataset.Name;
                            if (LayerName.Contains(name))
                            {
                                pFeatureLayer = new FeatureLayerClass();
                                pFeatureClass = pDataset as IFeatureClass;
                                SysCommon.Gis.SysGisTable table = new SysCommon.Gis.SysGisTable(sysDataSet.WorkSpace);
                                //������ʱͼ�����Ա���ͼ�������
                                ITable pForeignTable = table.OpenTable("MAP_META",out eError);
                                ITable pForeignTableTemp = ModData.v_SysMapTable.OpenTable("MAP_META_TEMP", out eError);
                                if (pForeignTable == null || pForeignTableTemp==null)
                                {
                                    return;
                                }
                                //�����ʱ��
                                ModData.v_SysMapTable.StartTransaction(out eError);
                                pForeignTableTemp.DeleteSearchedRows(null);
                                ICursor pForeignCursor = pForeignTable.Search(foreignFilter,false);
                                if (pForeignCursor == null) return;
                                IRow pForeignRow = pForeignCursor.NextRow();
                                if (pForeignRow != null)
                                {
                                    ICursor pForTempCursor = pForeignTableTemp.Insert(true);
                                    IRowBuffer pForTempRow = pForeignTableTemp.CreateRowBuffer();
                                    int count = 0;
                                    while (pForeignRow != null)
                                    {
                                        //��������
                                        result = CopyOriFeatToDesBuffer(pForeignRow, pForTempRow);
                                        pForTempCursor.InsertRow(pForTempRow);
                                        count++;
                                        if (count > 200)
                                        {
                                            pForTempCursor.Flush();
                                            count = 0;
                                        }
                                        pForeignRow = pForeignCursor.NextRow();
                                    }
                                }
                                else
                                {
                                    result = true;
                                }
                                ModData.v_SysMapTable.EndTransaction(result, out eError);
                                //�������ӹ�ϵ���������ӱ�
                                pFeatureJoinClass = GetJionFeatureClass(pFeatureClass, "MAP_NO", (IObjectClass)pForeignTableTemp, "MAPID", filter, "STATE", out eError);
                                pFeatureLayer.FeatureClass = pFeatureJoinClass;
                                pFeatureLayer.Name = pFeatureClass.AliasName;
                                if (pLayer == null)
                                {
                                    pLayer = pFeatureLayer as ILayer;
                                    pGroupLayer.Add(pLayer);
                                }
                                else
                                {
                                    (pLayer as IFeatureLayer).FeatureClass = pFeatureJoinClass;
                                }
                                break;
                            }
                        }
                        pDataset = pEnumDataset.Next();
                    }
                    if (!isExistLayer)
                    {
                        MapControl.Map.AddLayer(pGroupLayer);
                    }

                    //��Ⱦͼ��
                    //pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "STATE");
                    //pSetMapFrameSymbol.MapFrameSymbol();
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return;
            }
        }

        /// <summary>
        /// ��ԴҪ�ؿ�����Ŀ��Ҫ����Ļ������� 
        /// </summary>
        /// <param name="OriFeature">ԴҪ��</param>
        /// <param name="pFeatureBuffer">Ŀ��Ҫ�ػ�����</param>
        public static bool CopyOriFeatToDesBuffer(IRow OriFeature, IRowBuffer pFeatureBuffer)
        {
            try
            {
                IField pDesField;
                string pDesFieldName;
                int pOriFieldIndex;
                DateTime pNow = DateTime.Now;
                for (int pDesFieldIndex = 0; pDesFieldIndex < pFeatureBuffer.Fields.FieldCount; pDesFieldIndex++)
                {
                    pDesField = pFeatureBuffer.Fields.get_Field(pDesFieldIndex);
                    if (pDesField.Editable == true && pDesField.Type != esriFieldType.esriFieldTypeOID && pDesField.Type != esriFieldType.esriFieldTypeBlob && pDesField.Type != esriFieldType.esriFieldTypeRaster)
                    {
                        pDesFieldName = pDesField.Name;
                        pOriFieldIndex = OriFeature.Fields.FindField(pDesFieldName);
                        if (pOriFieldIndex > -1)
                        {
                            if (!OriFeature.get_Value(pOriFieldIndex).Equals(string.Empty))
                            {
                                pFeatureBuffer.set_Value(pDesFieldIndex, OriFeature.get_Value(pOriFieldIndex));
                            }
                            else
                            {
                                if (OriFeature.Fields.get_Field(pOriFieldIndex).IsNullable == true)
                                {
                                    pFeatureBuffer.set_Value(pDesFieldIndex, null);
                                }
                                else
                                {
                                    if (OriFeature.Fields.get_Field(pOriFieldIndex).Type == esriFieldType.esriFieldTypeString)
                                    {
                                        pFeatureBuffer.set_Value(pDesFieldIndex, "");
                                    }
                                    else if (OriFeature.Fields.get_Field(pDesFieldIndex).Type == esriFieldType.esriFieldTypeDouble || OriFeature.Fields.get_Field(pDesFieldIndex).Type == esriFieldType.esriFieldTypeInteger || OriFeature.Fields.get_Field(pDesFieldIndex).Type == esriFieldType.esriFieldTypeSingle)
                                    {
                                        pFeatureBuffer.set_Value(pDesFieldIndex, 0);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ���ݱ����ߵ������������ط�Χ
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pQueryFilter"></param>
        /// <param name="iMapControlDefault"></param>
        /// <param name="sysGisDataSet"></param>
        /// <param name="eError"></param>
        public static void AddInterZoneLayer(string name, IQueryFilter pQueryFilter, IMapControlDefault MapControl, SysGisDataSet sysDataSet, out Exception eError)
        {
            eError = null;
            bool isExistLayer = true;
            IWorkspace pWorkSpace = null;
            IFeatureWorkspace pFeatureWorkSpace;
            IFeatureDataset pFeatureDataset;
            IEnumDataset pEnumDataset;
            IDataset pDataset;
            IFeatureClass pFeatureClass;
            IFeatureClass pFeatureJoinClass;
            IFeatureLayer pFeatureLayer;
            ILayer pLayer = null;
            IGroupLayer pGroupLayer;
            string LayerName;
            try
            {
                pWorkSpace = sysDataSet.WorkSpace;
                if (pWorkSpace != null)
                {
                    pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

                    pFeatureDataset = sysDataSet.GetFeatureDataset("NJZW.assistant", out eError);
                    pDataset = pFeatureDataset as IFeatureDataset;
                    pEnumDataset = pDataset.Subsets;
                    //�о�Ҫ�������ƣ����ص��ؼ���
                    pEnumDataset.Reset();
                    pDataset = pEnumDataset.Next();
                    pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
                    if (pLayer == null && pGroupLayer == null)
                    {
                        pGroupLayer = new GroupLayerClass();
                        pGroupLayer.Name = "ʾ��ͼ";
                        isExistLayer = false;
                    }
                    //pQueryFilter = new QueryFilterClass();
                    while (pDataset != null)
                    {
                        if (pDataset is IFeatureClass)
                        {
                            LayerName = pDataset.Name;
                            if (LayerName.Contains(name))
                            {
                                pFeatureLayer = new FeatureLayerClass();
                                pFeatureClass = pDataset as IFeatureClass;                                
                                SysCommon.Gis.SysGisTable table = new SysCommon.Gis.SysGisTable(sysDataSet.WorkSpace);
                                ITable pForeignTable = table.OpenTable("INTERZONE", out eError);
                                if (pForeignTable == null)
                                {
                                    return;
                                }
                                pFeatureJoinClass = GetJionFeatureClass(pFeatureClass, "LINEID", (IObjectClass)pForeignTable, "ZONESETID", pQueryFilter, "", out eError);
                                pFeatureLayer.FeatureClass = pFeatureJoinClass;
                                pFeatureLayer.Name = pFeatureClass.AliasName;
                                if (pLayer == null)
                                {
                                    pLayer = pFeatureLayer as ILayer;
                                    //����Χ�߲�����
                                    ICompositeLayer pClayer = pGroupLayer as ICompositeLayer;
                                    if (pClayer != null && pClayer.Count > 0)
                                    {
                                        List<ILayer> pListLayer = new List<ILayer>();
                                        for (int i = 0; i < pClayer.Count; i++)
                                        {
                                            ILayer plyr = pClayer.get_Layer(i);
                                            if (!pListLayer.Contains(plyr))
                                            {
                                                pListLayer.Add(plyr);
                                            }
                                            pGroupLayer.Delete(plyr);
                                        }
                                        pGroupLayer.Add(pLayer);
                                        foreach (ILayer pl in pListLayer)
                                        {
                                            pGroupLayer.Add(pl);
                                        }
                                    }
                                    else
                                    {
                                        pGroupLayer.Add(pLayer);
                                    }
                                }
                                else
                                {
                                    (pLayer as IFeatureLayer).FeatureClass = pFeatureJoinClass;
                                }
                                break;
                            }
                        }
                        pDataset = pEnumDataset.Next();
                    }
                    if (!isExistLayer)
                    {
                        MapControl.Map.AddLayer(pGroupLayer);
                    }

                    //��Ⱦͼ��
                    //pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "STATE");
                    //pSetMapFrameSymbol.InterZoneFrameSymbol();
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="MapControl"></param>
        /// <param name="sysDataSet"></param>
        /// <param name="eError"></param>
        public static void AddInterZoneLayer(string name,string prjId,ISpatialFilter filter, IMapControlDefault MapControl, SysCommon.Gis.SysGisDataSet sysDataSet, out Dictionary<string,string> dicList, out Exception eError)
        {
            dicList = new Dictionary<string, string>();
            string strSql = "";
            eError = null;
            bool isExistLayer = true;
            IWorkspace pWorkSpace = null;
            IFeatureWorkspace pFeatureWorkSpace;
            IFeatureDataset pFeatureDataset;
            IEnumDataset pEnumDataset;
            IDataset pDataset;
            IFeatureClass pFeatureClass;
            IFeatureClass pFeatureJoinClass;
            IFeatureLayer pFeatureLayer;
            ILayer pLayer = null;
            IGroupLayer pGroupLayer;
            string LayerName;
            IQueryFilter pQueryFilter;
            try
            {
                pWorkSpace = sysDataSet.WorkSpace;
                if (pWorkSpace != null)
                {
                    pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

                    pFeatureDataset = sysDataSet.GetFeatureDataset("NJZW.assistant", out eError);
                    pDataset = pFeatureDataset as IFeatureDataset;
                    pEnumDataset = pDataset.Subsets;
                    //�о�Ҫ�������ƣ����ص��ؼ���
                    pEnumDataset.Reset();
                    pDataset = pEnumDataset.Next();
                    pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
                    if (pLayer == null && pGroupLayer == null)
                    {
                        pGroupLayer = new GroupLayerClass();
                        pGroupLayer.Name = "ʾ��ͼ";
                        isExistLayer = false;
                    }
                    pQueryFilter = new QueryFilterClass();
                    while (pDataset != null)
                    {
                        if (pDataset is IFeatureClass)
                        {
                            LayerName = pDataset.Name;
                            if (LayerName.Contains(name))
                            {
                                pFeatureLayer = new FeatureLayerClass();
                                pFeatureClass = pDataset as IFeatureClass;
                                IFeatureCursor pCursor = pFeatureClass.Search(filter, false);
                                if (pCursor != null)
                                {
                                    IFeature pFea = pCursor.NextFeature();
                                    int index = pCursor.Fields.FindField("LINEID");
                                    while (pFea != null)
                                    {
                                        if (index != -1)
                                        {
                                            string strId = pFea.get_Value(index).ToString();
                                            if (string.IsNullOrEmpty(strId)) continue;
                                            
                                            if (string.IsNullOrEmpty(strSql))
                                            {
                                                strSql = "'" + strId + "'";
                                            }
                                            else
                                            {
                                                strSql += ",'" + strId + "'";
                                            }
                                        }
                                        pFea = pCursor.NextFeature();
                                    }
                                    //������������
                                    if (!string.IsNullOrEmpty(strSql))
                                    {
                                        pQueryFilter.WhereClause = "NJZW.bound_line.LINEID IN (" + strSql + ") AND NJZW.INTERZONE.PRJID=" + prjId;
                                    }
                                }
                                //��Ҫ���ӵı�
                                SysCommon.Gis.SysGisTable table = new SysCommon.Gis.SysGisTable(sysDataSet.WorkSpace);
                                ITable pForeignTable = table.OpenTable("INTERZONE", out eError);
                                if (pForeignTable == null)
                                {
                                    return;
                                }
                                //���ӱ�
                                pFeatureJoinClass = GetJionFeatureClass(pFeatureClass, "LINEID", (IObjectClass)pForeignTable, "ZONESETID", pQueryFilter, "", out eError);
                                pFeatureLayer.FeatureClass = pFeatureJoinClass;
                                pFeatureLayer.Name = pFeatureClass.AliasName;
                                if (pLayer == null)
                                {
                                    pLayer = pFeatureLayer as ILayer;
                                    pGroupLayer.Add(pLayer);
                                }
                                else
                                {
                                    (pLayer as IFeatureLayer).FeatureClass = pFeatureJoinClass;
                                }
                                //�����ƶ��Ķ����Χ��������������غϵģ������ƶ�������ͻ����غϵģ�Ҫ
                                //�����ӱ���ѯ����ȷ������Ҫ�ؼ�
                                pCursor = pFeatureJoinClass.Search(null, false);
                                if (pCursor != null)
                                {
                                    IFeature pFea = pCursor.NextFeature();
                                    int index = pCursor.Fields.FindField("NJZW.bound_line.LINEID");
                                    int nameIndex = pCursor.Fields.FindField("NJZW.bound_line.NAME");
                                    while (pFea != null)
                                    {
                                        if (index != -1)
                                        {
                                            string strId = pFea.get_Value(index).ToString();
                                            String strName = pFea.get_Value(nameIndex).ToString();
                                            if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(strName)) continue;
                                            dicList.Add(strId, strName);
                                        }
                                        pFea = pCursor.NextFeature();
                                    }
                                }
                                break;
                            }
                        }
                        pDataset = pEnumDataset.Next();
                    }
                    if (!isExistLayer)
                    {
                        MapControl.Map.AddLayer(pGroupLayer);
                    }

                    //��Ⱦͼ��
                    //pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "NJZW.INTERZONE.STATE");
                    //pSetMapFrameSymbol.InterZoneFrameSymbol();
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return;
            }
        }

        /// <summary>
        /// �������Ƽ���������ͼ��
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="MapControl"></param>
        /// <param name="sysDataSet"></param>
        /// <param name="eError"></param>
        public static void AddLayer(string name, IQueryFilter filter, IMapControlDefault MapControl, SysCommon.Gis.SysGisDataSet sysDataSet, out Exception eError)
        {
            eError = null;
            bool isExistLayer = true;
            IWorkspace pWorkSpace = null;
            IFeatureWorkspace pFeatureWorkSpace;
            IFeatureDataset pFeatureDataset;
            IEnumDataset pEnumDataset;
            IDataset pDataset;
            IFeatureClass pFeatureClass;
            IFeatureLayer pFeatureLayer;
            IGroupLayer pGroupLayer;
            ILayer pLayer = null;
            string LayerName;
            try
            {
                pWorkSpace = sysDataSet.WorkSpace;
                if (pWorkSpace != null)
                {
                    pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

                    pFeatureDataset = sysDataSet.GetFeatureDataset("NJZW.assistant", out eError);
                    pDataset = pFeatureDataset as IFeatureDataset;
                    pEnumDataset = pDataset.Subsets;
                    //�о�Ҫ�������ƣ����ص��ؼ���
                    pEnumDataset.Reset();
                    pDataset = pEnumDataset.Next();
                    pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
                    if (pLayer == null && pGroupLayer == null)
                    {
                        pGroupLayer = new GroupLayerClass();
                        pGroupLayer.Name = "ʾ��ͼ";
                        isExistLayer = false;
                    }
                    while (pDataset != null)
                    {
                        if (pDataset is IFeatureClass)
                        {
                            LayerName = pDataset.Name;
                            if (LayerName.Contains(name))
                            {
                                pFeatureLayer = new FeatureLayerClass();
                                pFeatureClass = pDataset as IFeatureClass;
                                pFeatureLayer.FeatureClass = pFeatureClass;
                                pFeatureLayer.Name = pFeatureClass.AliasName;
                                if (pLayer == null)
                                {
                                    (pFeatureLayer as IFeatureLayerDefinition).DefinitionExpression = filter.WhereClause;
                                    pLayer = pFeatureLayer as ILayer;
                                }
                                else
                                {
                                    (pLayer as IFeatureLayerDefinition).DefinitionExpression = filter.WhereClause;
                                    (pLayer as IFeatureLayer).FeatureClass = pFeatureClass;
                                }
                                //(pLayer as ILayerEffects).Transparency = 50;
                                pGroupLayer.Add(pLayer);
                                break;
                            }
                        }
                        pDataset = pEnumDataset.Next();
                    }
                    if (!isExistLayer)
                    {
                        MapControl.Map.AddLayer(pGroupLayer);
                    }

                    //��Ⱦͼ��
                    //pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "STATE");
                    //pSetMapFrameSymbol.ZoneFrameSymbol();
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return;
            }
        }
        
        /// <summary>
        /// ������ϵ��(�������ݺͶ�Ӧ��־�޸ı�������)
        /// </summary>
        /// <param name="pOrgFeatCls"></param>
        /// <param name="strMDB"></param>
        /// <returns></returns>
        private static IFeatureClass GetJionFeatureClass(IFeatureClass pOrgFeatCls,string pOrgFieldName, IObjectClass pForeignFeatCls,string pForeignFieldName,IQueryFilter pQueryFilter,string fieldStateName,out Exception exError)
        {
            //�������ݺ���־��¼��Ĺ����༰��
            IRelationshipClass pJionRelationshipClass = SysCommon.Gis.ModGisPub.GetRelationShipClass("JionFc", (IObjectClass)pOrgFeatCls, pOrgFieldName, (IObjectClass)pForeignFeatCls, pForeignFieldName, "", "", esriRelCardinality.esriRelCardinalityOneToOne, out exError);
            if (pJionRelationshipClass == null) return null;
            IRelQueryTable pJionRelQueryTable = SysCommon.Gis.ModGisPub.GetRelQueryTable(pJionRelationshipClass, true, pQueryFilter, fieldStateName, false, true, out exError);
            if (pJionRelQueryTable == null) return null;

            IFeatureClass pOutFeatCLs = pJionRelQueryTable as IFeatureClass;
            return pOutFeatCLs;
        }

        /// <summary>
        /// Ϊͼ����ϱ����ע��
        /// </summary>
        /// <param name="StrScale"></param>
        /// <param name="MapControl"></param>
        /// <param name="fieldName"></param>
        public static void AddLabel(string StrScale, IMapControlDefault MapControl,Dictionary<string,string> fieldNameDics)
        {
            IGeoFeatureLayer pGeoFeatLayer;
            if (MapControl.LayerCount == 0) return;
            IFeatureLayer pFeatLay = ModDBOperator.GetMapFrameLayer(StrScale,MapControl, "ʾ��ͼ") as IFeatureLayer;
            if (pFeatLay == null) return;
            pGeoFeatLayer = pFeatLay as IGeoFeatureLayer;
            Dictionary<string, string> fieldList = new Dictionary<string, string>();
            for (int i = 0; i < pGeoFeatLayer.FeatureClass.Fields.FieldCount; i++)
            {
                IField pField = pGeoFeatLayer.FeatureClass.Fields.get_Field(i);
                if (pField == null) continue;
                foreach (string fieldName in fieldNameDics.Keys)
                {
                    if (pField.Name.Contains(fieldName))
                    {
                        fieldList.Add(pField.Name, fieldNameDics[fieldName]);
                        break;
                    }
                }
            }
            string expression = "";
            if (fieldList.Count > 0)
            {
                foreach (string fieldName in fieldList.Keys)
                {
                    if (string.IsNullOrEmpty(expression))
                    {
                        expression ="\""+fieldList[fieldName]+":\" & [" + fieldName + "] & chr(10)";
                    }
                    else
                    {
                        expression += " & \"" + fieldList[fieldName] + ":\" & [" + fieldName + "] & chr(10)";
                    }
                }
            }
            //pGeoFeatLayer.DisplayField = fieldList[0].ToString();

            IAnnotateLayerPropertiesCollection pAnnoProps = null;
            pAnnoProps = pGeoFeatLayer.AnnotationProperties;

            ILineLabelPosition pPosition = null;
            pPosition = new LineLabelPositionClass();
            pPosition.Parallel = true;
            pPosition.Above = true;

            ILineLabelPlacementPriorities pPlacement = new LineLabelPlacementPrioritiesClass();
            IBasicOverposterLayerProperties4 pBasic = new BasicOverposterLayerPropertiesClass();
            pBasic.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
            pBasic.LineLabelPlacementPriorities = pPlacement;
            pBasic.LineLabelPosition = pPosition;
            pBasic.BufferRatio = 0;
            pBasic.FeatureWeight = esriBasicOverposterWeight.esriHighWeight;
            pBasic.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
            //pBasic.PlaceOnlyInsidePolygon = true;//���ڵ����ڲ���ʾ��ע  deleted by chulili s20111018 �����ϲ�û���������ã���仰Ӧע�͵����������Ǵ���

            ILabelEngineLayerProperties pLabelEngine = null;
            pLabelEngine = new LabelEngineLayerPropertiesClass();
            pLabelEngine.BasicOverposterLayerProperties = pBasic as IBasicOverposterLayerProperties;
            pLabelEngine.Expression = expression;
            pLabelEngine.Symbol.Size = 10;

            IAnnotateLayerProperties pAnnoLayerProps = null;
            pAnnoLayerProps = pLabelEngine as IAnnotateLayerProperties;
            pAnnoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
            pAnnoProps.Clear();
            pAnnoProps.Add(pAnnoLayerProps);
            pGeoFeatLayer.ScaleSymbols = false;
            pGeoFeatLayer.DisplayAnnotation = true; 
            MapControl.ActiveView.Refresh();
        }


        /// <summary>
        /// ��ͼ����ϱ����ڵײ�
        /// </summary>
        /// <param name="pMapControl"></param>
        public static void MoveMapFrameLayer(IMapControlDefault pMapControl)
        {
            ILayer pLay = ModDBOperator.GetMapFrameLayer("zone",pMapControl, "ʾ��ͼ") as ILayer;
            if (pLay == null) return;
            pMapControl.Map.MoveLayer(pLay, pMapControl.LayerCount - 1);
        }

        /// <summary>
        /// �������ַ����õ���ΧPolygon
        /// </summary>
        /// <param name="strCoor">�����ַ���,��ʽΪX@Y,�Զ��ŷָ�</param>
        /// <returns></returns>
        public static IPolygon GetPolygonByCol(string strCoor)
        {
            try
            {
                object after = Type.Missing;
                object before = Type.Missing;
                IPolygon polygon = new PolygonClass();
                IPointCollection pPointCol = (IPointCollection)polygon;
                string[] strTemp = strCoor.Split(',');
                for (int index = 0; index < strTemp.Length; index++)
                {
                    string CoorLine = strTemp[index];
                    string[] coors = CoorLine.Split('@');

                    double X = Convert.ToDouble(coors[0]);
                    double Y = Convert.ToDouble(coors[1]);

                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(X, Y);
                    pPointCol.AddPoint(pPoint, ref before, ref after);
                }

                polygon = (IPolygon)pPointCol;
                polygon.Close();

                if (IsValidateGeometry(polygon)) return polygon;
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// �ӷ�ΧPolygon�õ���Ӧ�������ַ���
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static string GetColByPolygon(IPolygon polygon)
        {
            if (polygon == null) return "";
            IPointCollection pPointCol = (IPointCollection)polygon;

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < pPointCol.PointCount; index++)
                {
                    IPoint pPoint = pPointCol.get_Point(index);

                    string X = Convert.ToString(pPoint.X);
                    string Y = Convert.ToString(pPoint.Y);

                    if (sb.Length != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(X + "@" + Y);
                }

                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// ���һ���������Ƿ�Ƿ�
        /// </summary>
        /// <param name="pgeometry"></param>
        /// <returns></returns>
        private static bool IsValidateGeometry(IGeometry pgeometry)
        {
            // ��ȡ��Geometry��ԭʼ����
            IPointCollection pOrgPointCol = (IPointCollection)pgeometry;

            // ��ȡ��Geometry��ԭʼPart��
            IGeometryCollection pOrgGeometryCol = (IGeometryCollection)pgeometry;

            // ��Ŀ����п�¡�Ͷ�Ӧ�Ĵ���
            IClone pClone = (IClone)pgeometry;
            IGeometry pGeometryTemp = (IPolygon)pClone.Clone();
            ITopologicalOperator pTopo = (ITopologicalOperator)pGeometryTemp;
            pTopo.Simplify();

            // �õ��µ�Geometry
            pGeometryTemp = (IPolygon)pTopo;

            // ��ȡ�µ�Geometry�ĵ���
            IPointCollection pObjPointCol = (IPointCollection)pGeometryTemp;

            // ��ȡ�µ�Geometry��Part��
            IGeometryCollection pObjGeometryCol = (IGeometryCollection)pGeometryTemp;

            // ���бȽ�
            if (pOrgPointCol.PointCount != pObjPointCol.PointCount) return false;

            if (pOrgGeometryCol.GeometryCount != pObjGeometryCol.GeometryCount) return false;

            return true;
        }
        
        /// <summary>
        /// ��XML��ȡ���ݿ����������ļ�
        /// </summary>
        /// <param name="strPath"></param>
        public static bool GetSettingXml(string strPath)
        {
            if (string.IsNullOrEmpty(strPath)) return false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(strPath);
                if (doc.DocumentElement != null)
                {
                    XmlElement pElement = doc.DocumentElement;
                    ModData.Server = pElement["server"].InnerText;
                    ModData.Instance = pElement["service"].InnerText;
                    ModData.Database = pElement["database"].InnerText;
                    ModData.User = pElement["user"].InnerText;
                    ModData.Password = pElement["password"].InnerText;
                    ModData.Version = pElement["version"].InnerText;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ���ֵ��ListView
        /// </summary>
        /// <param name="listViewEx"></param>
        /// <param name="pFeaDic"></param>
        public static void BandListView(DevComponents.DotNetBar.Controls.ListViewEx listViewEx, Dictionary<int,IFeature> pFeaDic,int nameIndex)
        {
            if (pFeaDic.Count > 0)
            {
                listViewEx.Items.Clear();
                foreach (int id in pFeaDic.Keys)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = (pFeaDic[id] as IFeature).get_Value(nameIndex).ToString();
                    item.Tag = pFeaDic[id];
                    item.ImageIndex = 0;
                    listViewEx.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// ���ֵ��ListView
        /// </summary>
        /// <param name="listViewEx"></param>
        /// <param name="pFeaDic"></param>
        public static void BandListView(DevComponents.DotNetBar.Controls.ListViewEx listViewEx, Dictionary<string, object[]> pFeaDic)
        {
            if (pFeaDic.Count > 0)
            {
                listViewEx.Items.Clear();
                object[] obj = null;
                foreach (string id in pFeaDic.Keys)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = pFeaDic[id][0].ToString();
                    obj = new object[] {id,pFeaDic[id]};
                    item.Tag = obj;
                    item.ImageIndex = 0;
                    listViewEx.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// ����ѯ���û�������Ϣ����ͼ
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tree"></param>
        /// <param name="gisDb"></param>
        public static void DisplayUserView(string condition,DevComponents.DotNetBar.Controls.ListViewEx listView,IWorkspace gisWorkspace, out Exception exError)
        {
            exError = null;
            User user = null;

            try
            {
                SysGisTable sysTable = new SysGisTable(gisWorkspace);
                List<Dictionary<string, object>> lstDicData = sysTable.GetRows("user_info", condition, out exError);
                Dictionary<string, User> dic = new Dictionary<string, User>();
                if (lstDicData != null)
                {
                    for (int i = 0; i < lstDicData.Count; i++)
                    {
                        user = new User();
                        user.IDStr = lstDicData[i]["USERID"].ToString();
                        user.Name = lstDicData[i]["NAME"].ToString();
                        user.TrueName = lstDicData[i]["TRUTHNAME"].ToString();
                        user.Password = lstDicData[i]["UPWD"].ToString();
                        user.SexInt = int.Parse(lstDicData[i]["USEX"].ToString());
                        user.Position = lstDicData[i]["UPOSITION"].ToString();
                        user.Remark = lstDicData[i]["UREMARK"].ToString();
                        if (lstDicData[i]["EXPORTAREA"].ToString()=="")
                        {
                            user.ExportArea = -1;
                        }
                        else
                        {
                            user.ExportArea = Convert.ToDouble(lstDicData[i]["EXPORTAREA"]);
                        }
                        dic.Add(lstDicData[i]["USERID"].ToString(), user);
                    }
                    if (dic.Count > 0)
                    {
                        listView.Items.Clear();
                        foreach (string key in dic.Keys)
                        {
                            ListViewItem item = new ListViewItem();
                            user=dic[key] as User;
                            item.Text =user.Name+"("+user.TrueName+")";
                            item.Tag = user;
                            item.ImageIndex = 0;
                            listView.Items.Add(item);
                        }
                        //tree.Nodes[0].ExpandAll();
                    }
                }
            }
            catch (Exception ex)
            {
                exError = ex;
            }
        }

        /// <summary>
        /// ����ѯ���û���������Ϣ��ComboBox
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tree"></param>
        /// <param name="gisDb"></param>
        public static void DisplayRoleComboBox(string condition, DevComponents.DotNetBar.Controls.ComboBoxEx cboBox, IWorkspace gisWorkspace, out Exception exError)
        {
            exError = null;
            Role role = null;
            ComboBoxItem item;

            try
            {
                SysGisTable sysTable = new SysGisTable(gisWorkspace);
                List<Dictionary<string, object>> lstDicData = sysTable.GetRows("role", condition, out exError);
                Dictionary<string, Role> dic = new Dictionary<string, Role>();
                if (lstDicData != null)
                {
                    for (int i = 0; i < lstDicData.Count; i++)
                    {
                        role = new Role();
                        role.IDStr = lstDicData[i]["ROLEID"].ToString();
                        role.Name = lstDicData[i]["NAME"].ToString();
                        role.Privilege = lstDicData[i]["PRIVILEGE"] as XmlDocument;
                        role.Remark = lstDicData[i]["REMARK"].ToString();
                        dic.Add(lstDicData[i]["ROLEID"].ToString(), role);
                    }
                    if (dic.Count > 0)
                    {
                        foreach (string key in dic.Keys)
                        {
                            role = dic[key] as Role;
                            item = new ComboBoxItem(role.Name, role.ID);
                            item.Tag = role;
                            cboBox.Items.Add(item);
                        }
                        //tree.Nodes[0].ExpandAll();
                    }
                }
            }
            catch (Exception ex)
            {
                exError = ex;
            }
        }

        /// <summary>
        /// ��ȡ�û���ɫ���ձ��и�����ɫ���û�ID��
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="gisDb"></param>
        /// <returns></returns>
        public static List<string> GetUserIds(string roleid, IWorkspace pWorkspace, out Exception exError)
        {
            exError = null;

            try
            {
                SysGisTable sysTable = new SysGisTable(pWorkspace);
                List<Dictionary<string, object>> lstDicData = sysTable.GetRows("user_role", "ROLEID='" + roleid+"'", out exError);
                List<string> ids = null;
                if (lstDicData != null && lstDicData.Count > 0)
                {
                    ids = new List<string>();
                    foreach (Dictionary<string, object> dic in lstDicData)
                    {
                        foreach (string key in dic.Keys)
                        {
                            if (key.Equals("USERID"))
                            {
                                ids.Add(dic[key].ToString());
                            }
                        }
                    }
                }
                return ids;
            }
            catch (Exception ex)
            {
                exError = ex;
                return null;
            }
        }

        /// <summary>
        /// �����û�ID��ȡ�û���
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetNameFromUser(object userid,IWorkspace pWorkspace, out Exception exError)
        {
            exError = null;

            try
            {
                SysGisTable sysTable = new SysGisTable(pWorkspace);
                object name= sysTable.GetFieldValue("user_info","TRUTHNAME","userid='" + userid+"'", out exError);
                if (name != null)
                {
                    return name.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                exError = ex;
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��Χ����ָ����������ĿID��
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="gisDb"></param>
        /// <returns></returns>
        public static List<int> GetProjectIds(string tableName, string userid, string field, IWorkspace pWorkspace, out Exception exError)
        {
            exError = null;

            try
            {
                SysGisDataSet sysDataSet = new SysGisDataSet(pWorkspace);
                IFeatureCursor pFeatureCursor = sysDataSet.GetFeatureCursor(tableName, field + "='" + userid+"'", out exError);
                List<int> ids = null;
                if (pFeatureCursor != null)
                {
                    ids = new List<int>();
                    IFeature pFeature = pFeatureCursor.NextFeature();
                    while (pFeature != null)
                    {
                        int index = pFeature.Fields.FindField("PRJID");
                        int tempId=int.Parse(pFeature.get_Value(index).ToString());
                        if (!ids.Contains(tempId))
                        {
                            ids.Add(tempId);
                        }
                        pFeature = pFeatureCursor.NextFeature();
                    }
                }
                return ids;
            }
            catch (Exception ex)
            {
                exError = ex;
                return null;
            }
        }

        /// <summary>
        /// ����ָ����Χ��ѯͼ����
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="pGeometry"></param>
        /// <param name="sysDataSet"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static List<string> GetMapNoByGeometry(string mapName,IGeometry pGeometry, SysCommon.Gis.SysGisDataSet sysDataSet, out Exception eError)
        {
            eError = null;
            List<string> mapNums=null;
            IFeatureClass pZoneFeatureClass=null;
            try
            {
                IFeatureDataset pFeatureDataset = sysDataSet.GetFeatureDataset("NJZW.assistant", out eError);
                IEnumDataset pEnumDataset = pFeatureDataset.Subsets;
                IDataset pDataSet = pEnumDataset.Next();
                while (pDataSet != null)
                {
                    if (pDataSet.Name.Contains(mapName))
                    {
                        pZoneFeatureClass = pDataSet as IFeatureClass;
                        break;
                    }
                    pDataSet = pEnumDataset.Next();
                }
                if (pZoneFeatureClass == null) return null;
                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = "Shape";
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                IFeatureCursor pMapNoCursor = pZoneFeatureClass.Search(pSpatialFilter, true);
                if (pMapNoCursor != null)
                {
                    mapNums = new List<string>();
                    IFeature pFeature = pMapNoCursor.NextFeature();
                    int index=pMapNoCursor.Fields.FindField("map_no");
                    while (pFeature != null)
                    {
                        if (index != -1)
                        {
                            string mapNo=pFeature.get_Value(index).ToString();
                            if (!mapNums.Contains(mapNo))
                            {
                                mapNums.Add(mapNo);
                            }
                        }
                        pFeature = pMapNoCursor.NextFeature();
                    }
                }
                return mapNums;
            }
            catch (Exception ex)
            {
                eError = ex;
                return null;
            }
        }

        /// <summary>
        /// ����һ��Ŀ¼�����ݵ���һ��Ŀ¼
        /// </summary>
        /// <param name="yanfilepath"></param>
        /// <param name="mudifilepath"></param>
        public static bool CopyFilesDirs(string yanfilepath, string mudifilepath,out Exception eError)
        {
            eError = null;
            bool result = true;
            try
            {
                string[] arrDirs = Directory.GetDirectories(yanfilepath);
                string[] arrFiles = Directory.GetFiles(yanfilepath);
                if (arrFiles.Length != 0)
                {
                    for (int i = 0; i < arrFiles.Length; i++)
                    {
                        File.Copy(yanfilepath + "\\" + System.IO.Path.GetFileName(arrFiles[i]), mudifilepath + "\\"
                        + System.IO.Path.GetFileName(arrFiles[i]), true);
                    }
                }
                if (arrDirs.Length != 0)
                {
                    for (int i = 0; i < arrDirs.Length; i++)
                    {
                        Directory.CreateDirectory(mudifilepath + "\\" + System.IO.Path.GetFileName(arrDirs[i]));
                        //�ݹ����   
                        CopyFilesDirs(yanfilepath + "\\" + System.IO.Path.GetFileName(arrDirs[i]),
                        mudifilepath + "\\" + System.IO.Path.GetFileName(arrDirs[i]), out eError);
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                eError = ex;
                return false;
            }
        }

        /// <summary>
        /// ˢ��ͼ��
        /// </summary>
        /// <param name="xmlDocument"></param>
        public static void RefreshMap(Plugin.Application.IAppGisUpdateRef _AppHk,enumTaskType currentTaskType,out Exception eError)
        {
            eError = null;
            Plugin.Application.IAppFormRef _AppForm = _AppHk as Plugin.Application.IAppFormRef;
            XmlNode DBNode = _AppHk.DBXmlDocument.SelectSingleNode(".//��Ŀ����");
            if (DBNode == null) return;
            XmlElement DBElement = DBNode as XmlElement;
            string prjid = DBElement.GetAttribute("��Ŀ���");
            string userid = _AppForm.ConnUser.ID.ToString();
            string username = _AppForm.ConnUser.Name;
            string StrScale = DBElement.GetAttribute("������").ToString();
            XmlElement tElement=null;
            //��������Ĳ�ͬѡ��ͬ��ͼ����Χ
            if (currentTaskType == enumTaskType.ZONE)
            {
                tElement = DBElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
            }
            else if (currentTaskType == enumTaskType.INTERZONE)
            {
                tElement = DBElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
            }
            else if (currentTaskType == enumTaskType.MANAGER)
            {
                tElement = DBElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
            }
            else if (currentTaskType == enumTaskType.CHECK)
            {
                tElement = DBElement.SelectSingleNode(".//�������//��Χ") as XmlElement;
            }
            if (tElement == null) return;
            string whereClause = "";
            IQueryFilter pQueryFilter = new QueryFilterClass();
            //����ͼ��
            //��¼�ֶε�ͼ���ż�
            List<StringBuilder> sbList = null;
            StringBuilder sb = null;
            //���ͼ����������100��ֶ���������
            if (tElement.ChildNodes.Count >= 1000)
            {
                sbList = new List<StringBuilder>();
                for (int n = 0; n < tElement.ChildNodes.Count / 500 + 1; n++)
                {
                    sb = new StringBuilder();
                    for (int j = n * 500; j < (n + 1) * 500 && j < tElement.ChildNodes.Count; j++)
                    {
                        //��¼�ֶε�ͼ����
                        XmlElement element = tElement.ChildNodes[j] as XmlElement;
                        string mapNo = element.GetAttribute("ͼ����");
                        if (!string.IsNullOrEmpty(mapNo))
                        {
                            if (sb.Length != 0)
                            {
                                sb.Append(",");
                            }
                            sb.Append("'" + mapNo + "'");
                        }
                    }
                    sbList.Add(sb);
                }
            }
            else
            {
                sb = new StringBuilder();
                //�������ݸ��¶Ա��б���л�ȡ����
                for (int i = 0; i < tElement.ChildNodes.Count; i++)
                {
                    //��¼�ֶε�ͼ����
                    XmlElement element = tElement.ChildNodes[i] as XmlElement;
                    string mapNo = element.GetAttribute("ͼ����");
                    if (!string.IsNullOrEmpty(mapNo))
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(",");
                        }
                        sb.Append("'" + mapNo + "'");
                    }
                }
            }
            if (sbList != null && sbList.Count > 0)
            {
                sb = new StringBuilder();
                foreach (StringBuilder sbuilder in sbList)
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                    {
                        sb.Append("MAP_NO IN(" + sbuilder.ToString() + ")");
                    }
                    else
                    {
                        sb.Append(" OR MAP_NO IN (" + sbuilder.ToString() + ")");
                    }
                }
                if (sb.Length != 0)
                {
                    if (whereClause == "")
                    {
                        whereClause += sb.ToString();
                    }
                    else
                    {
                        whereClause += " and " + sb.ToString();
                    }
                }
            }
            else
            {
                if (sb.Length != 0)
                {
                    if (whereClause == "")
                    {
                        whereClause += "MAP_NO in (" + sb.ToString() + ")";
                    }
                    else
                    {
                        whereClause += " and MAP_NO in (" + sb.ToString() + ")";
                    }
                }
            }
            pQueryFilter.WhereClause = whereClause;
            IQueryFilter pForeignFilter = new QueryFilterClass();
            if (currentTaskType == enumTaskType.MANAGER || currentTaskType == enumTaskType.CHECK)
            {
                pForeignFilter.WhereClause = "PRJID=" + prjid;
            }
            else if (currentTaskType == enumTaskType.ZONE || currentTaskType == enumTaskType.INTERZONE)
            {
                pForeignFilter.WhereClause = "PRJID=" + prjid + " AND EMPID='" + userid+"'";
            }
            AddMapLayer(StrScale, pQueryFilter, pForeignFilter, _AppHk.MapControl, ModData.v_SysDataSet, out eError);
            //AddLabel(StrScale, _AppHk.MapControl, "MAP_NO");
            //ˢ��
            _AppHk.MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        /// <summary>
        /// ˢ��ͼ��
        /// </summary>
        /// <param name="xmlDocument"></param>
        public static void RefreshInterZone(XmlDocument DBXmlDocument,MapControl mapControl,out Exception eError)
        {
            eError = null;
            XmlNode DBNode = DBXmlDocument.SelectSingleNode(".//��Ŀ����");
            if (DBNode == null) return;
            XmlElement DBElement = DBNode as XmlElement;
            string prjid = DBElement.GetAttribute("��Ŀ���");
            string StrScale = DBElement.GetAttribute("������").ToString();
            XmlElement tElement = null;
            //��������Ĳ�ͬѡ��ͬ��ͼ����Χ
            tElement = DBElement.SelectSingleNode("//��������//����") as XmlElement;
            if (tElement == null) return;
            //ˢ�·�Χ�ߵķ���״̬
            IQueryFilter pQueryFilter = new QueryFilterClass();

            //���ط�Χ��
            string whereClause = "";
            foreach (XmlElement element in tElement.ChildNodes)
            {
                string lineid = element.GetAttribute("value");
                if (string.IsNullOrEmpty(whereClause))
                {
                    whereClause = "'" + lineid + "'";
                }
                else
                {
                    whereClause += ",'" + lineid + "'";
                }
            }
            if (!string.IsNullOrEmpty(whereClause))
            {
                whereClause = "NJZW.bound_line.LINEID IN (" + whereClause + ") AND NJZW.INTERZONE.PRJID=" + prjid;
            }
            else
            {
                whereClause = "NJZW.INTERZONE.PRJID=" + prjid;
            }
            pQueryFilter.WhereClause = whereClause;
            ModDBOperator.AddInterZoneLayer("bound_line", pQueryFilter, mapControl, ModData.v_SysDataSet, out eError);
            Dictionary<string, string> fieldDic = new Dictionary<string, string>();
            fieldDic.Add("LINENAME", "��Χ��");
            fieldDic.Add("EMPNAME", "��ҵ��");
            ModDBOperator.AddLabel("bound_line", ModData.v_AppGisUpdate.MapControl, fieldDic);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        /// <summary>
        /// �ñ�����־���·������ϵ���־
        /// </summary>
        /// <param name="sysDataBase"></param>
        /// <param name="sysDataBase_2"></param>
        public static bool UpdateDataLog(Plugin.Application.IAppFormRef m_AppForm,SysCommon.DataBase.SysTable logDataBase, SysCommon.Gis.SysGisTable logGisTable, out Exception eError)
        {
            eError = null;
            bool result = false;
            try
            {
                Plugin.Application.IAppGisUpdateRef appSMPD = m_AppForm as Plugin.Application.IAppGisUpdateRef;
                if (appSMPD == null||appSMPD.DBXmlDocument==null) return false;
                XmlElement DBElement = appSMPD.DBXmlDocument.SelectSingleNode(".//��Ŀ����") as XmlElement;
                string prjid = DBElement.GetAttribute("��Ŀ���");
                string prjname = DBElement.GetAttribute("��Ŀ����");
                if (logDataBase == null || logGisTable == null) return false;
                result = logGisTable.StartTransaction(out eError);
                List<Dictionary<string, object>> valueList = logGisTable.GetRows("DATA_LOG", "", out eError);
                if (valueList == null) return false;

                //��¼Ϊɾ��
                DataTable logDelTable = logDataBase.GetSQLTable("select * from ��־��¼��", out eError);
                if (logDelTable == null) return false;
                List<Dictionary<string, object>> insertList = new List<Dictionary<string, object>>();
                List<int> delOids = new List<int>();
                List<int> updOids = new List<int>();
                foreach (DataRow row in logDelTable.Rows)
                {
                    Dictionary<string, object> insertDic = new Dictionary<string, object>();
                    StringBuilder strBuilder = new StringBuilder();
                    string feaId = row["FEAID"].ToString();
                    string state = row["STATE"].ToString();

                    if (state.Equals("2"))
                    {
                        if (valueList.Count > 0)
                        {
                            bool isExist = false;
                            foreach (Dictionary<string, object> objDic in valueList)
                            {
                                if (objDic["FEAID"].ToString().Equals(feaId))
                                {
                                    isExist = true;
                                    //��������Ϊ�½���Ҫ��
                                    if (objDic["STATE"].ToString().Equals("1"))
                                    {
                                        delOids.Add(int.Parse(objDic["OBJECTID"].ToString()));
                                    }
                                    //��������Ϊ�޸ĵ�Ҫ��
                                    else if (objDic["STATE"].ToString().Equals("3"))
                                    {
                                        updOids.Add(int.Parse(objDic["OBJECTID"].ToString()));
                                    }
                                }
                            }
                            if (!isExist)
                            {
                                insertDic.Add("CLASS", row["CLASS"]);
                                insertDic.Add("FEAID", row["FEAID"]);
                                insertDic.Add("STATE", row["STATE"]);
                                insertDic.Add("TAG", row["TAG"]);
                                insertDic.Add("PRJID", prjid);
                                insertDic.Add("PRJNAME", prjname);
                                insertList.Add(insertDic);
                            }
                        }
                        else
                        {
                            insertDic.Add("CLASS", row["CLASS"]);
                            insertDic.Add("FEAID", row["FEAID"]);
                            insertDic.Add("STATE", row["STATE"]);
                            insertDic.Add("TAG", row["TAG"]);
                            insertDic.Add("PRJID", prjid);
                            insertDic.Add("PRJNAME", prjname);
                            insertList.Add(insertDic);
                        }
                    }
                    else if (state.Equals("3"))
                    {
                        if (valueList.Count > 0)
                        {
                            bool isExist = false;
                            foreach (Dictionary<string, object> objDic in valueList)
                            {
                                if (objDic["FEAID"].ToString().Equals(feaId))
                                {
                                    isExist = true;
                                }
                            }
                            if (!isExist)
                            {
                                insertDic.Add("CLASS", row["CLASS"]);
                                insertDic.Add("FEAID", row["FEAID"]);
                                insertDic.Add("STATE", row["STATE"]);
                                insertDic.Add("TAG", row["TAG"]);
                                insertDic.Add("PRJID", prjid);
                                insertDic.Add("PRJNAME", prjname);
                                insertList.Add(insertDic);
                            }
                        }
                        else
                        {
                            insertDic.Add("CLASS", row["CLASS"]);
                            insertDic.Add("FEAID", row["FEAID"]);
                            insertDic.Add("STATE", row["STATE"]);
                            insertDic.Add("TAG", row["TAG"]);
                            insertDic.Add("PRJID", prjid);
                            insertDic.Add("PRJNAME", prjname);
                            insertList.Add(insertDic);
                        }
                    }
                    else if (state.Equals("1"))
                    {
                        insertDic.Add("CLASS", row["CLASS"]);
                        insertDic.Add("FEAID", row["FEAID"]);
                        insertDic.Add("STATE", row["STATE"]);
                        insertDic.Add("TAG", row["TAG"]);
                        insertDic.Add("PRJID", prjid);
                        insertDic.Add("PRJNAME", prjname);
                        insertList.Add(insertDic);
                    }
                }

                //���뱾��ɾ��������
                if (insertList.Count != 0)
                {
                    foreach (Dictionary<string, object> dicValues in insertList)
                    {
                        result = logGisTable.NewRow("DATA_LOG", dicValues, out eError);
                    }
                }
                //ɾ�����������½���Ҫ��
                if (result)
                {
                    string delStr = "";
                    if (delOids.Count > 0)
                    {
                        foreach (int id in delOids)
                        {
                            if (string.IsNullOrEmpty(delStr))
                            {
                                delStr = id.ToString();
                            }
                            else
                            {
                                delStr += "," + id.ToString();
                            }
                        }
                        delStr = "OBJECTID IN (" + delStr + ")";
                        result = logGisTable.DeleteRows("DATA_LOG", delStr, out eError);
                    }
                }
                //�޸ķ��������޸�Ҫ�ص�״̬
                if (result)
                {
                    string updStr = "";
                    if (updOids.Count > 0)
                    {
                        foreach (int id in updOids)
                        {
                            if (string.IsNullOrEmpty(updStr))
                            {
                                updStr = id.ToString();
                            }
                            else
                            {
                                updStr += "," + id.ToString();
                            }
                        }
                        updStr = "OBJECTID IN (" + updStr + ")";
                        Dictionary<string, object> dicValues = new Dictionary<string, object>();
                        dicValues.Add("STATE", 2);
                        result = logGisTable.UpdateRow("DATA_LOG", updStr, dicValues, out eError);
                    }
                }
                logGisTable.EndTransaction(result, out eError);
                return result;
            }
            catch (Exception ex)
            {
                eError = ex;
                logGisTable.EndTransaction(result, out eError);
                return result;
            }
        }

        
        /// <summary>
        /// չ��ͼ���ͼ���������Ҫִ��toc��Update������
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="bExpand"></param>
        public static void ExpandLegend(ILayer pLayer, bool bExpand)
        {
            ILegendInfo pLegendInfo = pLayer as ILegendInfo;

            int iLegendGroupCount = pLegendInfo.LegendGroupCount;
            ILegendGroup pLGroup;
            for (int i = 0; i < iLegendGroupCount; i++)
            {
                pLGroup = pLegendInfo.get_LegendGroup(i);
                pLGroup.Visible = bExpand;
            }
        }

        /// <summary>
        /// ������ϵ��(��Դ�����������ֶ�)�Խ��������޸�״̬����Ⱦ
        /// </summary>
        /// <param name="pOrgFeatCls"></param>
        /// <param name="strMDB"></param>
        /// <returns></returns>
        public static IRelationshipClass GetRelationshipClass(IFeatureClass pOrgFeatCls, string strMDB)
        {
            if (pOrgFeatCls == null || !File.Exists(strMDB)) return null;
            Exception exError = null;
            //��ȡ��־������
            SysCommon.Gis.SysGisTable pSysGisTable = new SysCommon.Gis.SysGisTable();
            if (pSysGisTable.SetWorkspace(strMDB, SysCommon.enumWSType.PDB, out exError) == false)
            {
                return null;
            }

            //��ȡ��־��¼��
            ITable pUpdateTable = pSysGisTable.OpenTable("��־��¼��", out exError);
            if (pUpdateTable == null) return null;

            //�������ݺ���־��¼��Ĺ����༰��
            IRelationshipClass pJionRelationshipClass = SysCommon.Gis.ModGisPub.GetRelationShipClass("JionFc", (IObjectClass)pOrgFeatCls, "FEAID", (IObjectClass)pUpdateTable, "FEAID", "", "", esriRelCardinality.esriRelCardinalityOneToOne, out exError);
            pSysGisTable.CloseWorkspace();
            return pJionRelationshipClass;
        }


        /// <summary>
        /// ��˸Ҫ��
        /// </summary>
        /// <param name="pGeometry"></param>
        /// <param name="pScreenDisplay"></param>
        /// <param name="interval"></param>
        public static void FlashFeature(IGeometry pGeometry, IScreenDisplay pScreenDisplay, int nFlash, int interval)
        {
            pScreenDisplay.StartDrawing(pScreenDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
            if (pGeometry == null) return;
            switch (pGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    FlashLine(pScreenDisplay, pGeometry, nFlash,interval);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    FlashPolygon(pScreenDisplay, pGeometry,nFlash, interval);
                    break;
                case esriGeometryType.esriGeometryPoint:
                    FlashPoint(pScreenDisplay, pGeometry,nFlash, interval);
                    break;
                default:
                    break;
            }
            pScreenDisplay.FinishDrawing();
        }

        /// <summary>
        /// ��˸��
        /// </summary>
        /// <param name="pDisplay"></param>
        /// <param name="pGeometry"></param>
        /// <param name="interval"></param>
        private static void FlashPoint(IScreenDisplay pDisplay, IGeometry pGeometry, int nFlash, int interval)
        {
            ISimpleMarkerSymbol pMarkerSymbol;
            ISymbol pSymbol;
            IRgbColor pRGBColor;

            pMarkerSymbol = new SimpleMarkerSymbolClass();
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;

            pRGBColor = new RgbColorClass();
            pRGBColor.Green = 148;
            pRGBColor.Red = 32;
            pRGBColor.Blue = 0;

            pSymbol = pMarkerSymbol as ISymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
            pDisplay.SetSymbol(pSymbol);
            for (int i = 0; i < nFlash; i++)
            {
                pDisplay.DrawPoint(pGeometry);
                System.Threading.Thread.Sleep(interval);
            }
        }

        /// <summary>
        /// ��˸��
        /// </summary>
        /// <param name="pDisplay"></param>
        /// <param name="pGeometry"></param>
        /// <param name="interval"></param>
        private static void FlashLine(IScreenDisplay pDisplay, IGeometry pGeometry, int nFlash, int interval)
        {
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            ISymbol pSymbol;
            IRgbColor pRGBColor;

            tagPOINT tagPOINT = new tagPOINT();
            WKSPoint WKSPoint = new WKSPoint();

            tagPOINT.x = (int)8;
            tagPOINT.y = (int)8;
            pDisplay.DisplayTransformation.TransformCoords(ref WKSPoint, ref tagPOINT, 1, 6);

            pLineSymbol = new SimpleLineSymbolClass();
            pLineSymbol.Width = WKSPoint.X;

            pRGBColor = new RgbColorClass();
            pRGBColor.Green = 124;
            pRGBColor.Red = 252;
            pRGBColor.Blue = 0;

            pSymbol = pLineSymbol as ISymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            pDisplay.SetSymbol(pSymbol);

            for (int i = 0; i < nFlash; i++)
            {
                pDisplay.DrawPolyline(pGeometry);
                System.Threading.Thread.Sleep(interval);
            }
        }

        /// <summary>
        /// ��˸�����
        /// </summary>
        /// <param name="pDisplay"></param>
        /// <param name="pGeometry"></param>
        /// <param name="interval"></param>
        private static void FlashPolygon(IScreenDisplay pDisplay, IGeometry pGeometry, int nFlash, int interval)
        {
            ISimpleFillSymbol pFillSymbol;
            ISymbol pSymbol;
            IRgbColor pRGBColor;

            pFillSymbol = new SimpleFillSymbolClass();
            //pFillSymbol.Outline = null;

            pRGBColor = new RgbColorClass();
            pRGBColor.Green = 148;
            pRGBColor.Red = 32;
            pRGBColor.Blue = 0;

            pSymbol = pFillSymbol as ISymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            pDisplay.SetSymbol(pSymbol);
            for (int i = 0; i < nFlash; i++)
            {
                pDisplay.DrawPolygon(pGeometry);
                System.Threading.Thread.Sleep(interval);
            }
        }

        /// <summary>
        /// ����RGB����  
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns></returns>
        public static IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = r;
            pRgbColor.Green = g;
            pRgbColor.Blue = b;
            return pRgbColor;
        }
        /// <summary>
        /// ���ָ��ͼ��ĺϲ���Χ Ϊ���μӵ�һ������
        /// </summary>
        /// <param name="strLyrName"></param>
        /// <param name="strWhere"></param>
        /// <param name="eFeatureType"></param>
        /// <param name="eGeometryType"></param>
        /// <returns></returns>
        public static IGeometry GetLyrUnionPlygon(List<IGeometry> lstGeometry)
        {   
            IGeometryBag pGeoBag = new GeometryBagClass();
            IGeometryCollection pGeoCol = pGeoBag as IGeometryCollection;
           
            if (lstGeometry.Count < 1) return null;
            if (lstGeometry[0].SpatialReference != null)
            {
                pGeoBag.SpatialReference = lstGeometry[0].SpatialReference;
            }

            object obj = System.Type.Missing;
            for (int i = 0; i < lstGeometry.Count; i++)
            {
                IGeometry pTempGeo = lstGeometry[i];
                pGeoCol.AddGeometry(pTempGeo, ref obj, ref obj);
            }

            ISpatialIndex pSI = pGeoBag as ISpatialIndex;
            pSI.AllowIndexing = true;
            pSI.Invalidate();//��������
            ITopologicalOperator pTopo = null;
            switch (lstGeometry[0].GeometryType.ToString())
            {
                case "esriGeometryPolyline":
                    pTopo = new PolylineClass();
                    break;
                case "esriGeometryPolygon":
                    pTopo = new PolygonClass();
                    break;
            }
            pTopo.ConstructUnion(pGeoBag as IEnumGeometry);
            IGeometry pGeo = pTopo as IGeometry;

            return pGeo;
        }
    }


    public class SymbolLyr
    {
        #region ���Ż�ͼ��
        XmlDocument m_vDoc = null;
        public void SymbolFeatrueLayer(IFeatureLayer pFealyr)
        {
            if (!(pFealyr is IGeoFeatureLayer)) return;

            try
            {
                string strXMLpath = System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml";

                string strLyrName = pFealyr.Name;
                if (pFealyr.FeatureClass != null)
                {
                    IDataset pDataset = pFealyr.FeatureClass as IDataset;
                    strLyrName = pDataset.Name;
                }

                strLyrName = strLyrName.Substring(strLyrName.IndexOf('.') + 1);

                if (m_vDoc == null)
                {
                    IFeatureWorkspace pFeaWks = ModData.v_SysDataSet.WorkSpace as IFeatureWorkspace;
                    ITable pTable = pFeaWks.OpenTable("SYMBOLINFO");
                    IQueryFilter pQueryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
                    pQueryFilter.WhereClause = "SYMBOLNAME='ALLSYMBOL'";

                    ICursor pCursor = pTable.Search(pQueryFilter, false);
                    IRow pRow = pCursor.NextRow();
                    if (pRow == null) return;

                    IMemoryBlobStreamVariant var = pRow.get_Value(pRow.Fields.FindField("SYMBOL")) as IMemoryBlobStreamVariant;
                    object tempObj = null;
                    if (var == null) return;

                    var.ExportToVariant(out tempObj);
                    XmlDocument doc = new XmlDocument();
                    byte[] btyes = (byte[])tempObj;
                    string xml = Encoding.Default.GetString(btyes);
                    doc.LoadXml(xml);

                    DateTime updateTime = (DateTime)pRow.get_Value(pRow.Fields.FindField("UPDATETIME"));
                    DateTime Nowtime;

                    bool blnUpdate = false;

                    //��һ����־����
                    string strTimeLog = System.Windows.Forms.Application.StartupPath + "\\..\\Template\\UpdateTime.txt";
                    if (System.IO.File.Exists(strTimeLog))
                    {
                        StreamReader sr = new StreamReader(strTimeLog);
                        string strTime = sr.ReadLine();
                        sr.Close();
                        if (!DateTime.TryParse(strTime, out Nowtime))
                        {
                            blnUpdate = true;
                        }

                        if (updateTime > Nowtime)
                        {
                            if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�������·�����Ϣ���Ƿ���Ҫ���أ�"))
                            {
                                blnUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        blnUpdate = true;
                    }

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                    pCursor = null;

                    //�����ж��Ƿ���Ҫ���ط�����Ϣ
                    if (System.IO.File.Exists(strXMLpath))
                    {
                        if (blnUpdate)
                        {
                            doc.Save(strXMLpath);
                            StreamWriter sw1 = new StreamWriter(strTimeLog);
                            sw1.Write(updateTime.ToString());
                            sw1.Close();
                        }
                    }
                    else
                    {
                        doc.Save(strXMLpath);
                        StreamWriter sw = new StreamWriter(strTimeLog);
                        sw.Write(updateTime.ToString());
                        sw.Close();
                    }

                    m_vDoc = new XmlDocument();
                    m_vDoc.Load(strXMLpath);
                }
                else
                {
                }

                XmlElement pElement = m_vDoc.SelectSingleNode("//" + strLyrName) as XmlElement;
                if (pElement == null) return;

                IFeatureRenderer pFeaRender = SysCommon.XML.XMLClass.XmlDeSerializer2(pElement.FirstChild.Value) as IFeatureRenderer;
                IGeoFeatureLayer pGeoLyr = pFealyr as IGeoFeatureLayer;
                pGeoLyr.Renderer = pFeaRender;
            }
            catch
            {

            }
        }
        #endregion

    }

    public static class ModDBOperate
    {

        /// <summary>
        /// ��ʼ����ϵͳ�����ѡ��״̬   chenyafei  add 20110215  ҳ����ת
        /// </summary>
        /// <param name="pSysName">��ϵͳname</param>
        /// <param name="pSysCaption">��ϵͳcaption</param>
        ///         
        public static string _layerTreePath = Application.StartupPath + "\\..\\res\\xml\\չʾͼ����.xml";//added by chulili 20110802 ��������ӱ���,չʾͼ����·��,ר���������ҵ�·,����,����������
        public static string _QueryConfigXmlPath = Application.StartupPath + "\\..\\Template\\QueryConfig0.Xml"; //�����ļ�·��
        public static void InitialForm(string pSysName, string pSysCaption)
        {
            if (Plugin.ModuleCommon.DicTabs == null || Plugin.ModuleCommon.AppFrm == null) return;
            //��ʼ����ǰӦ�ó��ص����ƺͱ���
            Plugin.ModuleCommon.AppFrm.CurrentSysName = pSysName;
            Plugin.ModuleCommon.AppFrm.Caption = pSysCaption;

            //��ʾѡ������ϵͳ����
            bool bEnable = false;
            bool bVisible = false;
            if (Plugin.ModuleCommon.DicControls != null)
            {
                foreach (KeyValuePair<string, Plugin.Interface.IControlRef> keyValue in Plugin.ModuleCommon.DicControls)
                {
                    bEnable = keyValue.Value.Enabled;
                    bVisible = keyValue.Value.Visible;

                    Plugin.Interface.ICommandRef pCmd = keyValue.Value as Plugin.Interface.ICommandRef;
                    if (pCmd != null)
                    {
                        if (keyValue.Key == pSysName)
                        {
                            pCmd.OnClick();
                        }
                    }
                }
            }
            //Ĭ����ʾ��ϵͳ����ĵ�һ��
            int i = 0;
            foreach (KeyValuePair<DevComponents.DotNetBar.RibbonTabItem, string> keyValue in Plugin.ModuleCommon.DicTabs)
            {
                if (keyValue.Value == pSysName)
                {
                    i = i + 1;
                    keyValue.Key.Visible = true;
                    keyValue.Key.Enabled = true;
                    if (i == 1)
                    {
                        //Ĭ��ѡ�е�һ��
                        keyValue.Key.Checked = true;
                    }
                }
                else
                {
                    keyValue.Key.Visible = false;
                    keyValue.Key.Enabled = false;
                }
            }
        }
        //added by chulili 20110802��������Ӻ���,����nodeKey��ȡ������,ֱ�Ӷ�ȡ����Դ������Ϣ,��ȡ������
        public static IFeatureClass GetFeatureClassByNodeKey(string strNodeKey)
        {
            if (strNodeKey.Equals(""))
            {
                return null;
            }
            //Ŀ¼��·������:_layerTreePath
            XmlDocument pXmldoc = new XmlDocument();
            if (!File.Exists(_layerTreePath))
            {
                SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, _layerTreePath);                
            }
            if (!File.Exists(_layerTreePath))
            {
                return null;
            }
            //��չʾͼ����,��ȡͼ��ڵ�
            pXmldoc.Load(_layerTreePath);
            string strSearch = "//Layer[@NodeKey=" + "'" + strNodeKey + "'" + "]";
            XmlNode pNode = pXmldoc.SelectSingleNode(strSearch);
            if (pNode == null)
            {
                return null;
            }
            //��ȡͼ����,����Դid
            string strFeaClassName = "";
            string strDBSourceID = "";
            try
            {
                strFeaClassName = pNode.Attributes["Code"].Value;
                strDBSourceID = pNode.Attributes["ConnectKey"].Value;
            }
            catch
            { }
            //��������Դid,��ȡ����Դ��Ϣ
            SysGisTable sysTable = new SysGisTable(Plugin.ModuleCommon.TmpWorkSpace);
            Exception eError = null;
            object objConnstr = sysTable.GetFieldValue("DATABASEMD", "CONNECTIONINFO", "ID=" + strDBSourceID, out eError);
            string conninfostr = "";
            if (objConnstr != null)
            {
                conninfostr = objConnstr.ToString();
            }
            object objType = sysTable.GetFieldValue("DATABASEMD", "DATAFORMATID", "ID=" + strDBSourceID, out eError);
            int type = -1;
            if (objType != null)
            {
                type = int.Parse(objType.ToString());
            }
            //��������Դ������Ϣ,��ȡ����Դ����
            IWorkspace pWorkspace = GetWorkSpacefromConninfo(conninfostr, type);
            if (pWorkspace == null)
            {
                return null;
            }
            //�򿪵�����
            IFeatureWorkspace pFeaWorkSpace = pWorkspace as IFeatureWorkspace;
            IFeatureClass pFeaClass = null;
            try
            {
                pFeaClass = pFeaWorkSpace.OpenFeatureClass(strFeaClassName);
            }
            catch
            { }
            return pFeaClass;

        }
        //���������ַ�����ȡ�����ռ�
        //�˴������ַ����ǹ̶���ʽ�����Ӵ� Server|Service|Database|User|Password|Version
        private static IWorkspace GetWorkSpacefromConninfo(string conninfostr, int type)
        {
            //added by chulili 20111109 ��ӱ���
            if (conninfostr == "")
            {
                return null;
            }
            if (type < 0)
            {
                return null;
            }
            //end added by chulili 20111109
            int index1 = conninfostr.IndexOf("|");
            int index2 = conninfostr.IndexOf("|", index1 + 1);
            int index3 = conninfostr.IndexOf("|", index2 + 1);
            int index4 = conninfostr.IndexOf("|", index3 + 1);
            int index5 = conninfostr.IndexOf("|", index4 + 1);
            int index6 = conninfostr.IndexOf("|", index5 + 1);
            IPropertySet pPropSet = new PropertySetClass();
            IWorkspaceFactory pWSFact = null;
            string sServer = ""; string sService = ""; string sDatabase = "";
            string sUser = ""; string sPassword = ""; string strVersion = "";
            switch (type)
            {
                case 1://mdb
                    pWSFact = new AccessWorkspaceFactoryClass();
                    sDatabase = conninfostr.Substring(index2 + 1, index3 - index2 - 1);
                    break;
                case 2://gdb
                    pWSFact = new FileGDBWorkspaceFactoryClass();
                    sDatabase = conninfostr.Substring(index2 + 1, index3 - index2 - 1);
                    break;
                case 3://sde
                    pWSFact = new SdeWorkspaceFactoryClass();
                    sServer = conninfostr.Substring(0, index1);
                    sService = conninfostr.Substring(index1 + 1, index2 - index1 - 1);
                    sDatabase = conninfostr.Substring(index2 + 1, index3 - index2 - 1);
                    sUser = conninfostr.Substring(index3 + 1, index4 - index3 - 1);
                    sPassword = conninfostr.Substring(index4 + 1, index5 - index4 - 1);
                    strVersion = conninfostr.Substring(index5 + 1, index6 - index5 - 1);
                    break;
            }

            pPropSet.SetProperty("SERVER", sServer);
            pPropSet.SetProperty("INSTANCE", sService);
            pPropSet.SetProperty("DATABASE", sDatabase);
            pPropSet.SetProperty("USER", sUser);
            pPropSet.SetProperty("PASSWORD", sPassword);
            pPropSet.SetProperty("VERSION", strVersion);
            try
            {

                IWorkspace pWorkspace = pWSFact.Open(pPropSet, 0);
                return pWorkspace;
            }
            catch
            {
                return null;
            }
        }
      



    }
}