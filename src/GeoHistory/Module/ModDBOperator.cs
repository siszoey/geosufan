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

namespace GeoHistory
{
    public static class ModDBOperator
    {
        /// <summary>
        /// �����ͼ�ڵ�
        /// </summary>
        /// <param name="nodeCol"></param>
        /// <param name="strText"></param>
        /// <param name="strName"></param>
        /// <param name="pImage"></param>
        /// <param name="bExpand"></param>
        /// <returns></returns>
    //    public static DevComponents.AdvTree.Node CreateAdvTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand)
    //    {
    //        DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
    //        node.Text = strText;
    //        node.Image = pImage;
    //        if (strName != null)
    //        {
    //            node.Name = strName;
    //        }

    //        if (bExpand == true)
    //        {
    //            node.Expand();
    //        }

    //        nodeCol.Add(node);
    //        return node;
    //    }

    //    /// <summary>
    //    /// �����ͼ�ڵ���
    //    /// </summary>
    //    /// <param name="aNode"></param>
    //    /// <param name="strText"></param>
    //    /// <param name="pImage"></param>
    //    /// <returns></returns>
    //    public static DevComponents.AdvTree.Cell CreateAdvTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage)
    //    {
    //        DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell(strText);
    //        aCell.Images.Image = pImage;
    //        aNode.Cells.Add(aCell);

    //        return aCell;
    //    }

    //    /// <summary>
    //    /// ��mapcontrol�ϻ�ȡͼ����ϱ�
    //    /// </summary>
    //    /// <param name="StrScale"></param>
    //    /// <param name="pMapcontrol"></param>
    //    /// <param name="strName"></param>
    //    /// <returns></returns>
        //added by chulili
        //�������ܣ�����PDB�����ռ�  ��������������ռ������ļ���·��  �����ռ����� ���������������ռ�
        //������Դ�����ͬ�´���
        public static IWorkspace CreatePDBWorkSpace(string path, string filename)
        {
            IWorkspaceFactory pWorkspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactoryClass();
            if (System.IO.File.Exists(filename))
            {
                if (pWorkspaceFactory.IsWorkspace(filename))
                {
                    IWorkspace pTempWks = pWorkspaceFactory.OpenFromFile(filename, 0);
                    pWorkspaceFactory = null;
                    return pTempWks;
                }
            }

            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create("" + path + "", "" + filename + "", null, 0);
            IName name = (ESRI.ArcGIS.esriSystem.IName)pWorkspaceName;
            IWorkspace PDB_workspace = (IWorkspace)name.Open();
            pWorkspaceFactory = null;
            return PDB_workspace;

        }
        public static ILayer GetMapFrameLayer(string StrScale, IMapControlDefault pMapcontrol, string strName)
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

    //    /// <summary>
    //    /// ��mapcontrol�ϻ�ȡͼ����ϱ�
    //    /// </summary>
    //    /// <param name="StrScale"></param>
    //    /// <param name="pMapcontrol"></param>
    //    /// <param name="strName"></param>
    //    /// <param name="pGroupLayer"></param>
    //    /// <returns></returns>
    //    public static ILayer GetMapFrameLayer(string StrScale, IMapControlDefault pMapcontrol, string strName, out IGroupLayer pGroupLayer)
    //    {
    //        pGroupLayer = null;
    //        for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)
    //        {
    //            ILayer pLayer = pMapcontrol.Map.get_Layer(i);
    //            pGroupLayer = pLayer as IGroupLayer;
    //            if (pGroupLayer == null) continue;
    //            if (pGroupLayer.Name != strName) continue;
    //            ICompositeLayer pCompositeLayer = pGroupLayer as ICompositeLayer;
    //            if (pCompositeLayer.Count == 0) return null;
    //            for (int j = 0; j < pCompositeLayer.Count; j++)
    //            {
    //                pLayer = pCompositeLayer.get_Layer(j);
    //                if (pLayer is IFeatureLayer)
    //                {
    //                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
    //                    IDataset pDataset = pFeatureLayer.FeatureClass as IDataset;
    //                    if (pDataset.Name.Contains(StrScale))
    //                    {
    //                        return pLayer;
    //                    }
    //                }
    //            }
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// ���ݸ���ͼ���Ż�ȡ���·�Χ
    //    /// </summary>
    //    /// <param name="pRangeFeatLay"></param>
    //    /// <returns></returns>
    //    public static IGeometry GetGeometry(IFeatureLayer pRangeFeatLay)
    //    {
    //        IFeatureSelection pFeatSel = pRangeFeatLay as IFeatureSelection;
    //        if (pFeatSel.SelectionSet.Count == 0) return null;
    //        IEnumIDs pEnumIDs = pFeatSel.SelectionSet.IDs;
    //        int ID = pEnumIDs.Next();
    //        IGeometry pUnionGeo = null;
    //        while (ID != -1)
    //        {
    //            IFeature pFeat = pRangeFeatLay.FeatureClass.GetFeature(ID);
    //            if (pUnionGeo == null)
    //            {
    //                pUnionGeo = pFeat.Shape;
    //            }
    //            else
    //            {
    //                ITopologicalOperator pTop = pUnionGeo as ITopologicalOperator;
    //                pUnionGeo = pTop.Union(pFeat.Shape);
    //            }

    //            ID = pEnumIDs.Next();
    //        }
    //        return pUnionGeo;
    //    }

    //    /// <summary>
    //    /// ��Ⱦ�������� ��Ⱦ�����ֶ� ��־��¼��.state 1-�½�,2-�޸�,3-ɾ��
    //    /// </summary>
    //    /// <param name="pFeatureLayer"></param>
    //    /// <param name="strFieldName"></param>
    //    public static void SetDataUpdateSymbol(IFeatureLayer pFeatureLayer, string strFieldName)
    //    {
    //        if (pFeatureLayer == null || strFieldName == string.Empty) return;
    //        Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();
    //        Dictionary<string, ISymbol> dicFieldSymbol = new Dictionary<string, ISymbol>();


    //        ISymbol pSymbol = null;
    //        dicFieldValue.Add("1", "�½�");
    //        pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 35, 254, 7);
    //        dicFieldSymbol.Add("1", pSymbol);

    //        dicFieldValue.Add("2", "ɾ��");
    //        pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 227, 11, 11);
    //        dicFieldSymbol.Add("2", pSymbol);

    //        dicFieldValue.Add("3", "�޸�");
    //        pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 51, 102, 255); ;
    //        dicFieldSymbol.Add("3", pSymbol);

    //        dicFieldValue.Add("<Null>", "δ�仯");
    //        pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 230, 150, 159);
    //        dicFieldSymbol.Add("<Null>", pSymbol);

    //        SysCommon.Gis.ModGisPub.SetLayerUniqueValueRenderer(pFeatureLayer, strFieldName, dicFieldValue, dicFieldSymbol, false);
    //    }

    //    private static ISymbol CreateSymbol(esriGeometryType pGeometryType, int intR, int intG, int intB)
    //    {
    //        ISymbol pSymbol = null;
    //        ISimpleLineSymbol pSimpleLineSymbol = null;
    //        IColor pColor = SysCommon.Gis.ModGisPub.GetRGBColor(intR, intG, intB);
    //        switch (pGeometryType)
    //        {
    //            case esriGeometryType.esriGeometryPolygon:
    //                pSimpleLineSymbol = new SimpleLineSymbolClass();
    //                pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
    //                pSimpleLineSymbol.Color = SysCommon.Gis.ModGisPub.GetRGBColor(156, 156, 156);
    //                pSimpleLineSymbol.Width = 0.0001;
    //                ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
    //                pSimpleFillSymbol.Outline = pSimpleLineSymbol;
    //                pSimpleFillSymbol.Color = pColor;
    //                pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
    //                pSymbol = pSimpleFillSymbol as ISymbol;
    //                break;
    //            case esriGeometryType.esriGeometryPoint:
    //                ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
    //                pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
    //                pSimpleMarkerSymbol.Color = pColor;
    //                pSimpleMarkerSymbol.Size = 1;
    //                pSymbol = pSimpleMarkerSymbol as ISymbol;
    //                break;
    //            case esriGeometryType.esriGeometryPolyline:
    //                pSimpleLineSymbol = new SimpleLineSymbolClass();
    //                pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
    //                pSimpleLineSymbol.Color = pColor;
    //                pSimpleLineSymbol.Width = 0.1;
    //                pSymbol = pSimpleLineSymbol as ISymbol;
    //                break;
    //        }

    //        return pSymbol;
    //    }

    //    /// <summary>
    //    /// ���ݹ���XML�п���ṹ�ڵ㴴������
    //    /// </summary>
    //    /// <param name="feaName"></param>
    //    /// <param name="featureDataset"></param>
    //    /// <param name="feaworkspace"></param>
    //    /// <param name="fsEditAnno"></param>
    //    /// <param name="intScale"></param>
    //    public static void createAnnoFeatureClass(string feaName, IFeatureDataset featureDataset, IFeatureWorkspace feaworkspace, IFieldsEdit fsEditAnno, string intScale)
    //    {
    //        //����ע�ǵ������ֶ�
    //        try
    //        {
    //            //ע�ǵ�workSpace
    //            IFeatureWorkspaceAnno pFWSAnno = feaworkspace as IFeatureWorkspaceAnno;

    //            IGraphicsLayerScale pGLS = new GraphicsLayerScaleClass();
    //            pGLS.Units = esriUnits.esriMeters;
    //            pGLS.ReferenceScale = Convert.ToDouble(500);//����ע�Ǳ���Ҫ���ñ�����

    //            IFormattedTextSymbol myTextSymbol = new TextSymbolClass();
    //            ISymbol pSymbol = (ISymbol)myTextSymbol;
    //            //AnnoҪ��������е�ȱʡ����
    //            ISymbolCollection2 pSymbolColl = new SymbolCollectionClass();
    //            ISymbolIdentifier2 pSymID = new SymbolIdentifierClass();
    //            pSymbolColl.AddSymbol(pSymbol, "Default", out pSymID);

    //            //AnnoҪ����ı�Ҫ����
    //            IAnnotateLayerProperties pAnnoProps = new LabelEngineLayerPropertiesClass();
    //            pAnnoProps.CreateUnplacedElements = true;
    //            pAnnoProps.CreateUnplacedElements = true;
    //            pAnnoProps.DisplayAnnotation = true;
    //            pAnnoProps.UseOutput = true;

    //            ILabelEngineLayerProperties pLELayerProps = (ILabelEngineLayerProperties)pAnnoProps;
    //            pLELayerProps.Symbol = pSymbol as ITextSymbol;
    //            pLELayerProps.SymbolID = 0;
    //            pLELayerProps.IsExpressionSimple = true;
    //            pLELayerProps.Offset = 0;
    //            pLELayerProps.SymbolID = 0;

    //            IAnnotationExpressionEngine aAnnoVBScriptEngine = new AnnotationVBScriptEngineClass();
    //            pLELayerProps.ExpressionParser = aAnnoVBScriptEngine;
    //            pLELayerProps.Expression = "[DESCRIPTION]";
    //            IAnnotateLayerTransformationProperties pATP = (IAnnotateLayerTransformationProperties)pAnnoProps;
    //            pATP.ReferenceScale = pGLS.ReferenceScale;
    //            pATP.ScaleRatio = 1;

    //            IAnnotateLayerPropertiesCollection pAnnoPropsColl = new AnnotateLayerPropertiesCollectionClass();
    //            pAnnoPropsColl.Add(pAnnoProps);

    //            IObjectClassDescription pOCDesc = new AnnotationFeatureClassDescription();
    //            IFields fields = pOCDesc.RequiredFields;
    //            IFeatureClassDescription pFDesc = pOCDesc as IFeatureClassDescription;

    //            //for (int j = 0; j < pOCDesc.RequiredFields.FieldCount; j++)
    //            //{
    //            //    fsEditAnno.AddField(pOCDesc.RequiredFields.get_Field(j));yjl20120504
    //            //}
    //            fields = fsEditAnno as IFields;
    //            pFWSAnno.CreateAnnotationClass(feaName, fields, pOCDesc.InstanceCLSID, pOCDesc.ClassExtensionCLSID, pFDesc.ShapeFieldName, "", featureDataset, null, pAnnoPropsColl, pGLS, pSymbolColl, true);
    //        }
    //        catch
    //        {

    //        }
    //    }

    //    /// <summary>
    //    /// ����Ҫ����
    //    /// </summary>
    //    /// <param name="ruleXML"></param>
    //    /// <param name="feaworkspace"></param>
    //    /// <param name="intScale"></param>
    //    /// <returns></returns>
    //    public static bool createFeatureClass(XmlDocument ruleXML, IFeatureWorkspace feaworkspace, string intScale)
    //    {
    //        try
    //        {
    //            //��ȡ��Ŀ�����ṹ�ڵ㡱
    //            XmlNodeList tempNodeList = ruleXML.GetElementsByTagName("Ŀ�����ṹ");

    //            //�����ռ�ο���ͨ����ȡxml�ļ�����ÿռ�ο���·��
    //            XmlElement spatialElement = tempNodeList.Item(0).SelectSingleNode(".//�ռ�ο�") as XmlElement;
    //            string spatialPath = spatialElement.GetAttribute("·��");
    //            spatialPath = Application.StartupPath + "\\" + spatialPath;
    //            ISpatialReferenceFactory pSpaReferenceFac = new SpatialReferenceEnvironmentClass();//�ռ�ο�����
    //            ISpatialReference pSpatialReference = null;//������ÿռ�ο�
    //            if (File.Exists(spatialPath))
    //            {
    //                pSpatialReference = pSpaReferenceFac.CreateESRISpatialReferenceFromPRJFile(spatialPath);
    //            }

    //            if (pSpatialReference == null)
    //            {
    //                pSpatialReference = new UnknownCoordinateSystemClass();
    //            }

    //            //����Ĭ�ϵ�Resolution
    //            ISpatialReferenceResolution pSpatiaprefRes = pSpatialReference as ISpatialReferenceResolution;
    //            pSpatiaprefRes.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon 
    //            pSpatiaprefRes.SetDefaultXYResolution();
    //            pSpatiaprefRes.SetDefaultZResolution();
    //            pSpatiaprefRes.SetDefaultMResolution();
    //            //����Ĭ�ϵ�Tolerence
    //            ISpatialReferenceTolerance pSpatialrefTole = pSpatiaprefRes as ISpatialReferenceTolerance;
    //            pSpatialrefTole.SetDefaultXYTolerance();
    //            pSpatialrefTole.SetDefaultZTolerance();
    //            pSpatialrefTole.SetDefaultMTolerance();

    //            //�������ݼ�
    //            IFeatureDataset pFeatureDataset = null;//�������ݼ�����װ��Ҫ����
    //            pFeatureDataset = feaworkspace.CreateFeatureDataset("DLG", pSpatialReference);

    //            #region ͨ����ȡxml�ļ������Ҫ������Ϣ
    //            tempNodeList = tempNodeList.Item(0).SelectNodes(".//Ŀ������");
    //            XmlNode newFeatureNode = null;
    //            string shapestr = "Shape";

    //            for (int i = 0; i < tempNodeList.Count; i++)
    //            {
    //                newFeatureNode = tempNodeList.Item(i);
    //                string feaName = newFeatureNode["����"].InnerText;
    //                string feaType = newFeatureNode["����"].InnerText;
    //                XmlNodeList fieldNodeList = newFeatureNode.SelectNodes(".//�ֶ�");
    //                IFields fields = new FieldsClass();
    //                IFieldsEdit fsEdit = fields as IFieldsEdit;

    //                for (int j = 0; j < fieldNodeList.Count; j++)
    //                {
    //                    //����xml�ļ������û��Զ�����ֶ�
    //                    IField newfield = new FieldClass();
    //                    IFieldEdit fieldEdit = newfield as IFieldEdit;
    //                    XmlNode fieldNode = fieldNodeList.Item(j);
    //                    fieldEdit.Name_2 = fieldNode["����"].InnerText;
    //                    fieldEdit.AliasName_2 = fieldNode["����"].InnerText;
    //                    fieldEdit.Type_2 = (esriFieldType)Enum.Parse(typeof(esriFieldType), fieldNode["����"].InnerText, true);
    //                    if (fieldNode["����"].InnerText.Trim() != "")
    //                    {
    //                        fieldEdit.Length_2 = int.Parse(fieldNode["����"].InnerText);
    //                    }
    //                    if (fieldNode["Ĭ��ֵ"].InnerText.Trim() != "")
    //                    {
    //                        fieldEdit.DefaultValue_2 = fieldNode["Ĭ��ֵ"].InnerText;
    //                    }
    //                    fieldEdit.IsNullable_2 = !bool.Parse(fieldNode["�Ƿ����"].InnerText);
    //                    newfield = fieldEdit as IField;
    //                    fsEdit.AddField(newfield);
    //                }

    //                if (feaType == "ע��")
    //                {
    //                    //ע��
    //                    createAnnoFeatureClass(feaName, pFeatureDataset, feaworkspace, fsEdit, intScale);
    //                }
    //                else
    //                {
    //                    //featureClass�������ֶ�
    //                    //���Object�ֶ�
    //                    IField newfield2 = new FieldClass();
    //                    IFieldEdit fieldEdit2 = newfield2 as IFieldEdit;
    //                    fieldEdit2.Name_2 = "OBJECTID";
    //                    fieldEdit2.Type_2 = esriFieldType.esriFieldTypeOID;
    //                    fieldEdit2.AliasName_2 = "OBJECTID";
    //                    newfield2 = fieldEdit2 as IField;
    //                    fsEdit.AddField(newfield2);

    //                    //���Geometry�ֶ�
    //                    IField newfield1 = new FieldClass();
    //                    IFieldEdit fieldEdit1 = newfield1 as IFieldEdit;
    //                    fieldEdit1.Name_2 = shapestr;
    //                    fieldEdit1.Type_2 = esriFieldType.esriFieldTypeGeometry;
    //                    IGeometryDef geoDef = new GeometryDefClass();
    //                    IGeometryDefEdit geoDefEdit = geoDef as IGeometryDefEdit;
    //                    if (feaType == "��")
    //                    {
    //                        geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
    //                        geoDefEdit.HasZ_2 = true;
    //                    }
    //                    else if (feaType == "��")
    //                    {
    //                        geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
    //                    }
    //                    else if (feaType == "��")
    //                    {
    //                        geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
    //                    }
    //                    ISpatialReference pSR = pSpatialReference;
    //                    //ISpatialReferenceResolution pSRR = pSR as ISpatialReferenceResolution;
    //                    //pSRR.SetDefaultXYResolution();
    //                    //ISpatialReferenceTolerance pSRT = (ISpatialReferenceTolerance)pSR;
    //                    //pSRT.SetDefaultXYTolerance();
    //                    geoDefEdit.SpatialReference_2 = pSR;
    //                    fieldEdit1.GeometryDef_2 = geoDefEdit as GeometryDef;
    //                    newfield1 = fieldEdit1 as IField;
    //                    fsEdit.AddField(newfield1);
    //                    fields = fsEdit as IFields;

    //                    pFeatureDataset.CreateFeatureClass(feaName, fields, null, null, esriFeatureType.esriFTSimple, shapestr, "");
    //                }

    //            }
    //            #endregion

    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    /// <summary>
    //    /// ����Ҫ����yjl20120504 overload
    //    /// �������ͼ��������ṹ������
    //    /// </summary>
    //    /// <param name="feaworkspace"></param>
    //    /// <param name="intScale"></param>
    //    /// <returns></returns>
    //    public static bool createFeatureClass(XmlDocument ruleXML, IWorkspace pSrcW, IFeatureWorkspace feaworkspace, string intScale)
    //    {
    //        try
    //        {
    //            IFeatureWorkspace pSrcFW = pSrcW as IFeatureWorkspace;
    //            //�������ݼ�
    //            IFeatureDataset pFeatureDataset = null;//�������ݼ�����װ��Ҫ����
                

    //            #region ͨ����ȡ���ͼ��������Ҫ������Ϣ
    //            IFeatureDataset pFDtdt = pSrcFW.OpenFeatureDataset("W_NJTDT");
    //            pFeatureDataset = feaworkspace.CreateFeatureDataset("DLG", (pFDtdt as IGeoDataset).SpatialReference);
    //            if (pFDtdt == null)
    //            {
    //                return false;
    //            }
    //            IEnumDataset pEnumDs = pFDtdt.Subsets;
    //            IDataset pDs = pEnumDs.Next();
    //            while (pDs != null)
    //            {
    //                IFeatureClass pSrcFC = pDs as IFeatureClass;
    //                string feaName = pDs.Name;
    //                feaName = feaName.Substring(feaName.LastIndexOf('.') + 1);
    //                IEnumFieldError pEnumFieldError = null;//�ֶμ����󼯣�Ϊ�˸�������ֶθ�ֵyjl20110804 add
    //                IFields pFixedField = null;//�ֶμ������������ֶμ������ݴ���Ѱ����������ֶ���
    //                IFieldChecker fdCheker = new FieldCheckerClass();//yjl20110804 add
    //                fdCheker.ValidateWorkspace = pSrcW;
    //                fdCheker.Validate(pSrcFC.Fields, out pEnumFieldError, out pFixedField);
    //                IFieldsEdit fsEdit = pFixedField as IFieldsEdit;
    //                if (pSrcFC.Extension != null)
    //                {
    //                    //ע��
    //                    createAnnoFeatureClass(feaName, pFeatureDataset, feaworkspace, fsEdit, intScale);
    //                }
    //                else
    //                {
    //                    string shapestr = pSrcFC.ShapeFieldName;
    //                    string[] strShapeNames = shapestr.Split('.');
    //                    shapestr = strShapeNames[strShapeNames.GetLength(0) - 1];
    //                    pFeatureDataset.CreateFeatureClass(feaName, pFixedField, null, null, esriFeatureType.esriFTSimple, shapestr, "");
    //                }
    //                pDs = pEnumDs.Next();
    //            }
    //            #endregion

    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// �����ݹ���XML�л�ȡ����XML
    //    /// </summary>
    //    /// <param name="dbProjectXML"></param>
    //    /// <param name="strRuleName"></param>
    //    /// <returns></returns>
    //    public static XmlDocument GetRuleXmlDocument(XmlDocument dbProjectXML, string strRuleName)
    //    {
    //        //��ȡ���ݹ���·��
    //        XmlElement RootElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����");
    //        string ProjectPath = RootElement.GetAttribute("·��");
    //        XmlElement aElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����//����//" + strRuleName);
    //        if (aElement == null) return null;
    //        //��ȡ�����ݹ�������·��
    //        string RulePath = aElement.GetAttribute("·��");
    //        //��ȡ���ݹ��������·��
    //        RulePath = ProjectPath + RulePath;

    //        if (!File.Exists(RulePath)) return null;

    //        try
    //        {
    //            XmlDocument outputXml = new XmlDocument();
    //            outputXml.Load(RulePath);
    //            return outputXml;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }
    //    /// <summary>
    //    /// �����ݹ���XML�л�ȡ����XML
    //    /// </summary>
    //    /// <param name="dbProjectXML"></param>
    //    /// <param name="strRuleName"></param>
    //    /// <returns></returns>
    //    public static string GetRuleXmlPath(XmlDocument dbProjectXML, string strRuleName)
    //    {
    //        //��ȡ���ݹ���·��
    //        XmlElement RootElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����");
    //        string ProjectPath = RootElement.GetAttribute("·��");
    //        XmlElement aElement = (XmlElement)dbProjectXML.SelectSingleNode(".//��Ŀ����//����//" + strRuleName);
    //        if (aElement == null) return "";
    //        //��ȡ�����ݹ�������·��
    //        string RulePath = aElement.GetAttribute("·��");
    //        //��ȡ���ݹ��������·��
    //        RulePath = ProjectPath + RulePath;

    //        if (!File.Exists(RulePath)) return "";
    //        return RulePath;
    //    }

    //    public static bool CreatExtractDB(XmlDocument ExtractXml, string intScale, string objName, out Exception eError, IWorkspace pSrcW)
    //    {
    //        bool exist;
    //        bool result = false;
    //        eError = null;
    //        if (ExtractXml == null) return false;

    //        //----------��ȡ��Ŀ�����ļ�������ȡ���ݵĿ���-------------------------------------
    //        IWorkspace TempWorkSpace = ModDBOperator.GetWorkSpace(objName, out exist);
    //        if (TempWorkSpace == null)
    //        {
    //            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ" + objName + "��������ʧ��!");
    //            return false;
    //        }
    //        IFeatureWorkspace pFeatureWorkSpace = TempWorkSpace as IFeatureWorkspace;
    //        if (!exist)
    //        {
    //            //�½����ؿ�
    //            result = ModDBOperator.createFeatureClass(ExtractXml, pSrcW, pFeatureWorkSpace, intScale);//yjl20120504 modify ���ݹ����������������ؿ�
    //        }
    //        else
    //        {
    //            result = true;
    //        }
    //        //�ͷŹ����ռ�
    //        if (pFeatureWorkSpace != null)
    //        {
    //            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject((pFeatureWorkSpace as IWorkspace).WorkspaceFactory);
    //            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pFeatureWorkSpace);
    //            pFeatureWorkSpace = null;
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// ������ݲ����ڵ�
    //    /// </summary>
    //    /// <param name="dbProjectXML"></param>
    //    /// <param name="strNodeName"></param>
    //    /// <param name="orgElement"></param>
    //    /// <param name="bOrgTemp"></param>
    //    /// <param name="objElement"></param>
    //    /// <param name="bObjTemp"></param>
    //    /// <param name="outputRulePath"></param>
    //    /// <returns></returns>
    //    public static bool AddDataOperatorXmlNode(XmlDocument dbProjectXML, string strNodeName, XmlElement orgElement, string orgDataSetName, bool bOrgTemp, XmlElement objElement, bool bObjTemp, string outputRulePath)
    //    {
    //        XmlNode DBNode = dbProjectXML.SelectSingleNode(".//��Ŀ����");
    //        XmlNode aNode = SelectNode(DBNode, strNodeName);
    //        //����Ѵ��ڵ��ӽڵ�
    //        aNode.RemoveAll();
    //        XmlNode sourceNode = SelectNode(aNode, "Դ��������");
    //        XmlElement dataElement = dbProjectXML.CreateElement("Դ����");
    //        XmlElement DBElement = DBNode as XmlElement;
    //        dataElement.SetAttribute("����", orgElement.GetAttribute("����"));
    //        if (!bOrgTemp)
    //        {
    //            dataElement.SetAttribute("������", orgElement.GetAttribute("������"));
    //        }
    //        else
    //        {
    //            dataElement.SetAttribute("������", DBElement.GetAttribute("·��") + orgElement.GetAttribute("������")); //���¿���ΪGDB���ҷ�����Ϊ���·��
    //        }
    //        dataElement.SetAttribute("���ݼ�", orgDataSetName);
    //        dataElement.SetAttribute("������", orgElement.GetAttribute("������"));
    //        dataElement.SetAttribute("�û�", orgElement.GetAttribute("�û�"));
    //        dataElement.SetAttribute("����", orgElement.GetAttribute("����"));
    //        dataElement.SetAttribute("�汾", orgElement.GetAttribute("�汾"));
    //        sourceNode.AppendChild((XmlNode)dataElement);

    //        XmlElement objElementTemp = SelectNode(aNode, "Ŀ����������") as XmlElement;
    //        objElementTemp.SetAttribute("����", objElement.GetAttribute("����"));
    //        if (!bObjTemp)
    //        {
    //            objElementTemp.SetAttribute("������", objElement.GetAttribute("������"));
    //        }
    //        else
    //        {
    //            objElementTemp.SetAttribute("������", DBElement.GetAttribute("·��") + objElement.GetAttribute("������")); //���¿���ΪGDB���ҷ�����Ϊ���·��
    //        }
    //        objElementTemp.SetAttribute("������", objElement.GetAttribute("������"));
    //        objElementTemp.SetAttribute("�û�", objElement.GetAttribute("�û�"));
    //        objElementTemp.SetAttribute("����", objElement.GetAttribute("����"));
    //        objElementTemp.SetAttribute("�汾", objElement.GetAttribute("�汾"));

    //        if (outputRulePath == "") return false;
    //        XmlNode RuleNode = SelectNode(aNode, "����");
    //        XmlElement RuleElement = RuleNode as XmlElement;
    //        RuleElement.SetAttribute("·��", outputRulePath);

    //        return true;
    //    }

    //    public static bool AddDataOperatorXmlNode(XmlDocument dbProjectXML, string strNodeName, string strOrgPath, string strObjPath, string outputRulePath, string strOrgType, string strObjType)
    //    {
    //        XmlNode DBNode = dbProjectXML.SelectSingleNode(".//��Ŀ����");
    //        XmlNode aNode = SelectNode(DBNode, strNodeName);
    //        //����Ѵ��ڵ��ӽڵ�
    //        aNode.RemoveAll();
    //        XmlNode sourceNode = SelectNode(aNode, "Դ��������");
    //        XmlElement dataElement = dbProjectXML.CreateElement("Դ����");
    //        XmlElement DBElement = DBNode as XmlElement;
    //        dataElement.SetAttribute("����", strOrgType);
    //        dataElement.SetAttribute("������", strOrgPath);
    //        dataElement.SetAttribute("������", "");
    //        dataElement.SetAttribute("�û�", "");
    //        dataElement.SetAttribute("����", "");
    //        dataElement.SetAttribute("�汾", "");
    //        sourceNode.AppendChild((XmlNode)dataElement);

    //        XmlElement objElement = SelectNode(aNode, "Ŀ����������") as XmlElement;
    //        objElement.SetAttribute("����", strObjType);
    //        objElement.SetAttribute("������", strObjPath);
    //        objElement.SetAttribute("������", "");
    //        objElement.SetAttribute("�û�", "");
    //        objElement.SetAttribute("����", "");
    //        objElement.SetAttribute("�汾", "");

    //        if (outputRulePath == "") return false;
    //        XmlNode RuleNode = SelectNode(aNode, "����");
    //        XmlElement RuleElement = RuleNode as XmlElement;
    //        RuleElement.SetAttribute("·��", outputRulePath);

    //        return true;
    //    }

    //    /// <summary>
    //    /// ѡ��XMLĳ�ڵ����������򴴽�
    //    /// </summary>
    //    /// <param name="parentNode"></param>
    //    /// <param name="strName"></param>
    //    /// <returns></returns>
    //    private static XmlNode SelectNode(XmlNode parentNode, string strName)
    //    {
    //        XmlNode aNode = parentNode.SelectSingleNode(".//" + strName);
    //        if (aNode == null)
    //        {
    //            aNode = parentNode.OwnerDocument.CreateElement(strName) as XmlNode;
    //        }

    //        parentNode.AppendChild(aNode);
    //        return aNode;
    //    }

    //    /// <summary>
    //    /// ����XML���ӽڵ��ȡworkspace���������򴴽�
    //    /// </summary>
    //    /// <param name="pWorkSpaceNode"></param>
    //    /// <returns></returns>
    //    public static IWorkspace GetWorkSpace(XmlNode pWorkSpaceNode, out bool bExit)
    //    {
    //        bExit = false;
    //        try
    //        {
    //            IWorkspace TempWorkSpace = null;
    //            XmlElement TempElement = pWorkSpaceNode as XmlElement;
    //            string strType = TempElement.GetAttribute("����");

    //            switch (strType.Trim().ToUpper())
    //            {
    //                case "PDB":
    //                case "GDB":
    //                    IWorkspaceFactory pWorkspaceFactory = null;
    //                    if (strType.Trim().ToUpper() == "PDB")
    //                    {
    //                        pWorkspaceFactory = new AccessWorkspaceFactoryClass();
    //                        bExit = File.Exists(TempElement.GetAttribute("������"));
    //                    }
    //                    else if (strType.Trim().ToUpper() == "GDB")
    //                    {
    //                        pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
    //                        bExit = Directory.Exists(TempElement.GetAttribute("������"));
    //                    }


    //                    if (!bExit)
    //                    {
    //                        FileInfo finfo = new FileInfo(TempElement.GetAttribute("������"));
    //                        string outputDBPath = finfo.DirectoryName;
    //                        string outputDBName = finfo.Name;
    //                        outputDBName = outputDBName.Substring(0, outputDBName.Length - 4);
    //                        IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(outputDBPath, outputDBName, null, 0);
    //                        IName pName = (IName)pWorkspaceName;
    //                        TempWorkSpace = (IWorkspace)pName.Open();
    //                    }
    //                    else
    //                    {
    //                        TempWorkSpace = pWorkspaceFactory.OpenFromFile(TempElement.GetAttribute("������"), 0);
    //                    }

    //                    break;
    //                case "SDE":
    //                    IWorkspaceFactory pSDEWorkspaceFactory = new SdeWorkspaceFactoryClass();
    //                    IPropertySet pPropSet = new PropertySetClass();
    //                    pPropSet.SetProperty("SERVER", TempElement.GetAttribute("������"));
    //                    pPropSet.SetProperty("INSTANCE", TempElement.GetAttribute("������"));
    //                    pPropSet.SetProperty("DATABASE", "");
    //                    pPropSet.SetProperty("USER", TempElement.GetAttribute("�û�"));
    //                    pPropSet.SetProperty("PASSWORD", TempElement.GetAttribute("����"));
    //                    pPropSet.SetProperty("VERSION", TempElement.GetAttribute("�汾"));

    //                    TempWorkSpace = pSDEWorkspaceFactory.Open(pPropSet, 0);
    //                    break;
    //            }

    //            return TempWorkSpace;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ����XML���ӽڵ��ȡworkspace���������򴴽�
    //    /// </summary>
    //    /// <param name="pWorkSpaceNode"></param>
    //    /// <returns></returns>
    //    public static IWorkspace GetWorkSpace(string objName, out bool bExit)
    //    {
    //        bExit = false;
    //        try
    //        {
    //            IWorkspace TempWorkSpace = null;

    //            IWorkspaceFactory pWorkspaceFactory = null;

    //            pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
    //            bExit = Directory.Exists(objName);

    //            if (!bExit)
    //            {
    //                FileInfo finfo = new FileInfo(objName);
    //                string outputDBPath = finfo.DirectoryName;
    //                string outputDBName = finfo.Name;
    //                outputDBName = outputDBName.Substring(0, outputDBName.Length - 4);
    //                IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(outputDBPath, outputDBName, null, 0);
    //                IName pName = (IName)pWorkspaceName;
    //                TempWorkSpace = (IWorkspace)pName.Open();
    //            }
    //            else
    //            {
    //                TempWorkSpace = pWorkspaceFactory.OpenFromFile(objName, 0);
    //            }

    //            return TempWorkSpace;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ���ݸ���ͼ���Ż�ȡ���·�Χ
    //    /// </summary>
    //    /// <param name="MapNumArray"></param>
    //    /// <param name="pRangeFeatLay"></param>
    //    /// <returns></returns>        
    //    public static IGeometry GetGeometry(string[] MapNumArray, IFeatureLayer pRangeFeatLay)
    //    {
    //        if (pRangeFeatLay == null) return null;
    //        StringBuilder sb = new StringBuilder();
    //        for (int i = 0; i < MapNumArray.Length; i++)
    //        {
    //            if (sb.Length != 0)
    //            {
    //                sb.Append(",");
    //            }
    //            sb.Append("'");
    //            sb.Append(MapNumArray.GetValue(i).ToString());
    //            sb.Append("'");
    //        }
    //        IQueryFilter pQueryFilter = new QueryFilterClass();
    //        if (sb.Length != 0)
    //        {
    //            pQueryFilter.WhereClause = "[MAP_NEWNO] in (" + sb.ToString() + ")";
    //        }
    //        else
    //        {
    //            pQueryFilter.WhereClause = "";
    //        }

    //        IFeatureClass pFeatureClass = pRangeFeatLay.FeatureClass;
    //        IFeatureCursor pFeatureCursor = pFeatureClass.Search(pQueryFilter, false);

    //        //ѡ��ͼ��
    //        IFeatureSelection pFeatSel = pRangeFeatLay as IFeatureSelection;
    //        pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

    //        IGeometryBag pGeometryBag = new GeometryBagClass();
    //        IGeoDataset pGeoDataSet = pFeatureClass as IGeoDataset;
    //        pGeometryBag.SpatialReference = pGeoDataSet.SpatialReference;

    //        IGeometryCollection pGeometryCollection = pGeometryBag as IGeometryCollection;
    //        IFeature pFeature = null;
    //        pFeature = pFeatureCursor.NextFeature();
    //        while (pFeature != null)
    //        {
    //            object missing = Type.Missing;
    //            if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
    //            {
    //                pGeometryCollection.AddGeometry(pFeature.Shape, ref missing, ref missing);
    //            }

    //            pFeature = pFeatureCursor.NextFeature();
    //        }

    //        ITopologicalOperator UnionedPolygon = new PolygonClass();
    //        UnionedPolygon.ConstructUnion(pGeometryBag as IEnumGeometry);

    //        IGeometry UnionedGeometry = UnionedPolygon as IGeometry;
    //        return UnionedGeometry;
    //    }

    //    /// <summary>
    //    /// ������Ŀ�����ļ���ȡͼ����
    //    /// </summary>
    //    /// <param name="ProjectXml"></param>
    //    /// <returns></returns>
    //    public static string[] GetMapNoByXml(XmlDocument ProjectXml)
    //    {
    //        XmlNodeList MapNoList = null;
    //        MapNoList = ProjectXml.GetElementsByTagName("ͼ��");
    //        XmlNode MapNoNode = null;
    //        XmlElement MapNoElement = null;
    //        string MapNo = "";
    //        string[] MapNoArray = new string[MapNoList.Count];

    //        for (int i = 0; i < MapNoList.Count; i++)
    //        {
    //            MapNoNode = MapNoList[i];
    //            MapNoElement = (XmlElement)MapNoNode;
    //            MapNo = MapNoElement.GetAttribute("ͼ����");
    //            MapNoArray.SetValue(MapNo, i);
    //        }
    //        return MapNoArray;
    //    }

    //    /// <summary>
    //    /// ��ԴҪ�ؿ�����Ŀ��Ҫ����Ļ������� 
    //    /// </summary>
    //    /// <param name="OriFeature">ԴҪ��</param>
    //    /// <param name="pFeatureBuffer">Ŀ��Ҫ�ػ�����</param>
    //    public static bool CopyOriFeatToDesBuffer(IRow OriFeature, IRowBuffer pFeatureBuffer)
    //    {
    //        try
    //        {
    //            IField pDesField;
    //            string pDesFieldName;
    //            int pOriFieldIndex;
    //            DateTime pNow = DateTime.Now;
    //            for (int pDesFieldIndex = 0; pDesFieldIndex < pFeatureBuffer.Fields.FieldCount; pDesFieldIndex++)
    //            {
    //                pDesField = pFeatureBuffer.Fields.get_Field(pDesFieldIndex);
    //                if (pDesField.Editable == true && pDesField.Type != esriFieldType.esriFieldTypeOID && pDesField.Type != esriFieldType.esriFieldTypeBlob && pDesField.Type != esriFieldType.esriFieldTypeRaster)
    //                {
    //                    pDesFieldName = pDesField.Name;
    //                    pOriFieldIndex = OriFeature.Fields.FindField(pDesFieldName);
    //                    if (pOriFieldIndex > -1)
    //                    {
    //                        if (!OriFeature.get_Value(pOriFieldIndex).Equals(string.Empty))
    //                        {
    //                            pFeatureBuffer.set_Value(pDesFieldIndex, OriFeature.get_Value(pOriFieldIndex));
    //                        }
    //                        else
    //                        {
    //                            if (OriFeature.Fields.get_Field(pOriFieldIndex).IsNullable == true)
    //                            {
    //                                pFeatureBuffer.set_Value(pDesFieldIndex, null);
    //                            }
    //                            else
    //                            {
    //                                if (OriFeature.Fields.get_Field(pOriFieldIndex).Type == esriFieldType.esriFieldTypeString)
    //                                {
    //                                    pFeatureBuffer.set_Value(pDesFieldIndex, "");
    //                                }
    //                                else if (OriFeature.Fields.get_Field(pDesFieldIndex).Type == esriFieldType.esriFieldTypeDouble || OriFeature.Fields.get_Field(pDesFieldIndex).Type == esriFieldType.esriFieldTypeInteger || OriFeature.Fields.get_Field(pDesFieldIndex).Type == esriFieldType.esriFieldTypeSingle)
    //                                {
    //                                    pFeatureBuffer.set_Value(pDesFieldIndex, 0);
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// ���ݱ����ߵ������������ط�Χ
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <param name="filter"></param>
    //    /// <param name="MapControl"></param>
    //    /// <param name="sysDataSet"></param>
    //    /// <param name="eError"></param>
    //    public static void AddMapLayer(string name, IQueryFilter filter, IQueryFilter foreignFilter, IMapControlDefault MapControl, SysCommon.Gis.SysGisDataSet sysDataSet, IMapDocument pMapDoc, out Exception eError)
    //    {
    //        eError = null;
    //        bool result = false;
    //        bool isExistLayer = true;
    //        IWorkspace pWorkSpace = null;
    //        IFeatureWorkspace pFeatureWorkSpace;
    //        IFeatureDataset pFeatureDataset;
    //        IEnumDataset pEnumDataset;
    //        IDataset pDataset;
    //        IFeatureClass pFeatureClass;
    //        IFeatureClass pFeatureJoinClass;
    //        IFeatureLayer pFeatureLayer;
    //        ILayer pLayer = null;
    //        IGroupLayer pGroupLayer;
    //        string LayerName;
    //        clsSetMapFrameSymbol pSetMapFrameSymbol;
    //        try
    //        {
    //            pWorkSpace = sysDataSet.WorkSpace;
    //            if (pWorkSpace != null)
    //            {
    //                pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

    //                pFeatureDataset = sysDataSet.GetFeatureDataset("assistant", out eError);
    //                pDataset = pFeatureDataset as IFeatureDataset;
    //                pEnumDataset = pDataset.Subsets;
    //                //�о�Ҫ�������ƣ����ص��ؼ���
    //                pEnumDataset.Reset();
    //                pDataset = pEnumDataset.Next();
    //                pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
    //                if (pLayer == null && pGroupLayer == null)
    //                {
    //                    pGroupLayer = new GroupLayerClass();
    //                    pGroupLayer.Name = "ʾ��ͼ";
    //                    isExistLayer = false;
    //                }
    //                while (pDataset != null)
    //                {
    //                    if (pDataset is IFeatureClass)
    //                    {
    //                        LayerName = pDataset.Name;
    //                        if (LayerName.Contains(name))
    //                        {
    //                            pFeatureLayer = new FeatureLayerClass();
    //                            pFeatureClass = pDataset as IFeatureClass;
    //                            SysCommon.Gis.SysGisTable table = new SysCommon.Gis.SysGisTable(sysDataSet.WorkSpace);
    //                            //������ʱͼ�����Ա���ͼ�������
    //                            ITable pForeignTable = table.OpenTable("MAP_META", out eError);
    //                            ITable pForeignTableTemp = ModData.v_SysMapTable.OpenTable("MAP_META_TEMP", out eError);
    //                            if (pForeignTable == null || pForeignTableTemp == null)
    //                            {
    //                                return;
    //                            }
    //                            //�����ʱ��
    //                            ModData.v_SysMapTable.StartTransaction(out eError);
    //                            pForeignTableTemp.DeleteSearchedRows(null);
    //                            ICursor pForeignCursor = pForeignTable.Search(foreignFilter, false);
    //                            if (pForeignCursor == null) return;
    //                            IRow pForeignRow = pForeignCursor.NextRow();
    //                            if (pForeignRow != null)
    //                            {
    //                                ICursor pForTempCursor = pForeignTableTemp.Insert(true);
    //                                IRowBuffer pForTempRow = pForeignTableTemp.CreateRowBuffer();
    //                                int count = 0;
    //                                while (pForeignRow != null)
    //                                {
    //                                    //��������
    //                                    result = CopyOriFeatToDesBuffer(pForeignRow, pForTempRow);
    //                                    pForTempCursor.InsertRow(pForTempRow);
    //                                    count++;
    //                                    if (count > 200)
    //                                    {
    //                                        pForTempCursor.Flush();
    //                                        count = 0;
    //                                    }
    //                                    pForeignRow = pForeignCursor.NextRow();
    //                                }
    //                            }
    //                            else
    //                            {
    //                                result = true;
    //                            }
    //                            ModData.v_SysMapTable.EndTransaction(result, out eError);
    //                            //�������ӹ�ϵ���������ӱ�
    //                            pFeatureJoinClass = GetJionFeatureClass(pFeatureClass, "MAP_NO", (IObjectClass)pForeignTableTemp, "MAPID", filter, "STATE", out eError);
    //                            pFeatureLayer.FeatureClass = pFeatureJoinClass;
    //                            pFeatureLayer.Name = pFeatureClass.AliasName;
    //                            if (pLayer == null)
    //                            {
    //                                pLayer = pFeatureLayer as ILayer;
    //                                pGroupLayer.Add(pLayer);
    //                            }
    //                            else
    //                            {
    //                                (pLayer as IFeatureLayer).FeatureClass = pFeatureJoinClass;
    //                            }
    //                            //����ͼ�㲻�����
    //                            (pLayer as IFeatureLayer).ScaleSymbols = false;
    //                            break;
    //                        }
    //                    }
    //                    pDataset = pEnumDataset.Next();
    //                }
    //                if (!isExistLayer)
    //                {
    //                    MapControl.Map.AddLayer(pGroupLayer);
    //                }

    //                //��Ⱦͼ��
    //                if (pMapDoc == null)
    //                {
    //                    pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "STATE");
    //                    pSetMapFrameSymbol.InterZoneFrameSymbol();
    //                }
    //                else
    //                {
    //                    IMap pMap = pMapDoc.get_Map(0);
    //                    for (int i = 0; i < pMap.LayerCount; i++)
    //                    {
    //                        ILayer pLyr = pMap.get_Layer(i);
    //                        if (pLyr is IGeoFeatureLayer)
    //                        {
    //                            IGeoFeatureLayer pGeoFeaLyr = pLyr as IGeoFeatureLayer;
    //                            string strLyrName = pLyr.Name;

    //                            //���������Դ ����Ҫ���������Ϊ׼ ���û�о���ͼ����Ϊ׼
    //                            //if (pGeoFeaLyr.FeatureClass != null)
    //                            //{
    //                            //    IDataset pDataset = pGeoFeaLyr.FeatureClass as IDataset;
    //                            //    strLyrName = pDataset.Name;
    //                            //}
    //                            if (strLyrName.Equals(name) || strLyrName.Equals("�ӷ���"))
    //                            {
    //                                (pLayer as IGeoFeatureLayer).Renderer = pGeoFeaLyr.Renderer;
    //                                break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return;
    //        }
    //    }

    //    /// <summary>
    //    /// ���ݱ����ߵ������������ط�Χ
    //    /// </summary>
    //    /// <param name="p"></param>
    //    /// <param name="pQueryFilter"></param>
    //    /// <param name="iMapControlDefault"></param>
    //    /// <param name="sysGisDataSet"></param>
    //    /// <param name="eError"></param>
    //    public static void AddInterZoneLayer(string name, IQueryFilter pQueryFilter, IMapControlDefault MapControl, SysGisDataSet sysDataSet, IMapDocument pMapDoc, out Exception eError)
    //    {
    //        eError = null;
    //        bool isExistLayer = true;
    //        IWorkspace pWorkSpace = null;
    //        IFeatureWorkspace pFeatureWorkSpace;
    //        IFeatureDataset pFeatureDataset;
    //        IEnumDataset pEnumDataset;
    //        IDataset pDataset;
    //        IFeatureClass pFeatureClass;
    //        IFeatureClass pFeatureJoinClass;
    //        IFeatureLayer pFeatureLayer;
    //        ILayer pLayer = null;
    //        IGroupLayer pGroupLayer;
    //        string LayerName;
    //        clsSetMapFrameSymbol pSetMapFrameSymbol;
    //        try
    //        {
    //            pWorkSpace = sysDataSet.WorkSpace;
    //            if (pWorkSpace != null)
    //            {
    //                pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

    //                pFeatureDataset = sysDataSet.GetFeatureDataset("assistant", out eError);
    //                pDataset = pFeatureDataset as IFeatureDataset;
    //                pEnumDataset = pDataset.Subsets;
    //                //�о�Ҫ�������ƣ����ص��ؼ���
    //                pEnumDataset.Reset();
    //                pDataset = pEnumDataset.Next();
    //                pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
    //                if (pLayer == null && pGroupLayer == null)
    //                {
    //                    pGroupLayer = new GroupLayerClass();
    //                    pGroupLayer.Name = "ʾ��ͼ";
    //                    isExistLayer = false;
    //                }
    //                //pQueryFilter = new QueryFilterClass();
    //                while (pDataset != null)
    //                {
    //                    if (pDataset is IFeatureClass)
    //                    {
    //                        LayerName = pDataset.Name;
    //                        if (LayerName.Contains(name))
    //                        {
    //                            pFeatureLayer = new FeatureLayerClass();
    //                            pFeatureClass = pDataset as IFeatureClass;
    //                            SysCommon.Gis.SysGisTable table = new SysCommon.Gis.SysGisTable(sysDataSet.WorkSpace);
    //                            ITable pForeignTable = table.OpenTable("INTERZONE", out eError);
    //                            if (pForeignTable == null)
    //                            {
    //                                return;
    //                            }
    //                            pFeatureJoinClass = GetJionFeatureClass(pFeatureClass, "LINEID", (IObjectClass)pForeignTable, "ZONESETID", pQueryFilter, "", out eError);
    //                            pFeatureLayer.FeatureClass = pFeatureJoinClass;
    //                            pFeatureLayer.Name = pFeatureClass.AliasName;
    //                            if (pLayer == null)
    //                            {
    //                                pLayer = pFeatureLayer as ILayer;
    //                                //����Χ�߲�����
    //                                ICompositeLayer pClayer = pGroupLayer as ICompositeLayer;
    //                                if (pClayer != null && pClayer.Count > 0)
    //                                {
    //                                    List<ILayer> pListLayer = new List<ILayer>();
    //                                    for (int i = 0; i < pClayer.Count; i++)
    //                                    {
    //                                        ILayer plyr = pClayer.get_Layer(i);
    //                                        if (!pListLayer.Contains(plyr))
    //                                        {
    //                                            pListLayer.Add(plyr);
    //                                        }
    //                                        pGroupLayer.Delete(plyr);
    //                                    }
    //                                    pGroupLayer.Add(pLayer);
    //                                    foreach (ILayer pl in pListLayer)
    //                                    {
    //                                        pGroupLayer.Add(pl);
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    pGroupLayer.Add(pLayer);
    //                                }
    //                            }
    //                            else
    //                            {
    //                                (pLayer as IFeatureLayer).FeatureClass = pFeatureJoinClass;
    //                            }
    //                            //����ͼ�㲻�����
    //                            (pLayer as IFeatureLayer).ScaleSymbols = false;
    //                            break;
    //                        }
    //                    }
    //                    pDataset = pEnumDataset.Next();
    //                }
    //                if (!isExistLayer)
    //                {
    //                    MapControl.Map.AddLayer(pGroupLayer);
    //                }

    //                //��Ⱦͼ��
    //                if (pMapDoc == null)
    //                {
    //                    pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "STATE");
    //                    pSetMapFrameSymbol.InterZoneFrameSymbol();
    //                }
    //                else
    //                {
    //                    IMap pMap = pMapDoc.get_Map(0);
    //                    for (int i = 0; i < pMap.LayerCount; i++)
    //                    {
    //                        ILayer pLyr = pMap.get_Layer(i);
    //                        if (pLyr is IGeoFeatureLayer)
    //                        {
    //                            IGeoFeatureLayer pGeoFeaLyr = pLyr as IGeoFeatureLayer;
    //                            string strLyrName = pLyr.Name;

    //                            //���������Դ ����Ҫ���������Ϊ׼ ���û�о���ͼ����Ϊ׼
    //                            //if (pGeoFeaLyr.FeatureClass != null)
    //                            //{
    //                            //    IDataset pDataset = pGeoFeaLyr.FeatureClass as IDataset;
    //                            //    strLyrName = pDataset.Name;
    //                            //}
    //                            if (strLyrName.Equals(name) || strLyrName.Equals("��Χ��"))
    //                            {
    //                                (pLayer as IGeoFeatureLayer).Renderer = pGeoFeaLyr.Renderer;
    //                                break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return;
    //        }
    //    }

    //    /// <summary>
    //    /// �������Ƽ���������ͼ��
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <param name="filter"></param>
    //    /// <param name="MapControl"></param>
    //    /// <param name="sysDataSet"></param>
    //    /// <param name="eError"></param>
    //    public static void AddLayer(string name, IQueryFilter filter, IMapControlDefault MapControl, SysCommon.Gis.SysGisDataSet sysDataSet, IMapDocument pMapDoc, out Exception eError)
    //    {
    //        eError = null;
    //        bool isExistLayer = true;
    //        IWorkspace pWorkSpace = null;
    //        IFeatureWorkspace pFeatureWorkSpace;
    //        IFeatureDataset pFeatureDataset;
    //        IEnumDataset pEnumDataset;
    //        IDataset pDataset;
    //        IFeatureClass pFeatureClass;
    //        IFeatureLayer pFeatureLayer;
    //        IGroupLayer pGroupLayer;
    //        ILayer pLayer = null;
    //        string LayerName;
    //        clsSetMapFrameSymbol pSetMapFrameSymbol;
    //        try
    //        {
    //            pWorkSpace = sysDataSet.WorkSpace;
    //            if (pWorkSpace != null)
    //            {
    //                pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;

    //                pFeatureDataset = sysDataSet.GetFeatureDataset("assistant", out eError);//yjl20120502 
    //                pDataset = pFeatureDataset as IFeatureDataset;
    //                pEnumDataset = pDataset.Subsets;
    //                //�о�Ҫ�������ƣ����ص��ؼ���
    //                pEnumDataset.Reset();
    //                pDataset = pEnumDataset.Next();
    //                pLayer = GetMapFrameLayer(name, MapControl, "ʾ��ͼ", out pGroupLayer);
    //                if (pLayer == null && pGroupLayer == null)
    //                {
    //                    pGroupLayer = new GroupLayerClass();
    //                    pGroupLayer.Name = "ʾ��ͼ";
    //                    isExistLayer = false;
    //                }
    //                while (pDataset != null)
    //                {
    //                    if (pDataset is IFeatureClass)
    //                    {
    //                        LayerName = pDataset.Name;
    //                        if (LayerName.Contains(name))
    //                        {
    //                            pFeatureLayer = new FeatureLayerClass();
    //                            pFeatureClass = pDataset as IFeatureClass;
    //                            pFeatureLayer.FeatureClass = pFeatureClass;
    //                            pFeatureLayer.Name = pFeatureClass.AliasName;
    //                            if (pLayer == null)
    //                            {
    //                                (pFeatureLayer as IFeatureLayerDefinition).DefinitionExpression = filter.WhereClause;
    //                                pLayer = pFeatureLayer as ILayer;
    //                            }
    //                            else
    //                            {
    //                                (pLayer as IFeatureLayerDefinition).DefinitionExpression = filter.WhereClause;
    //                                (pLayer as IFeatureLayer).FeatureClass = pFeatureClass;
    //                            }
    //                            //����ͼ�㲻�����
    //                            (pLayer as IFeatureLayer).ScaleSymbols = false;
    //                            //(pLayer as ILayerEffects).Transparency = 50;
    //                            pGroupLayer.Add(pLayer);
    //                            break;
    //                        }
    //                    }
    //                    pDataset = pEnumDataset.Next();
    //                }
    //                if (!isExistLayer)
    //                {
    //                    MapControl.Map.AddLayer(pGroupLayer);
    //                }

    //                //��Ⱦͼ��
    //                if (pMapDoc == null)
    //                {
    //                    pSetMapFrameSymbol = new clsSetMapFrameSymbol(name, MapControl, "STATE");
    //                    pSetMapFrameSymbol.InterZoneFrameSymbol();
    //                }
    //                else
    //                {
    //                    IMap pMap = pMapDoc.get_Map(0);
    //                    for (int i = 0; i < pMap.LayerCount; i++)
    //                    {
    //                        ILayer pLyr = pMap.get_Layer(i);
    //                        if (pLyr is IGeoFeatureLayer)
    //                        {
    //                            IGeoFeatureLayer pGeoFeaLyr = pLyr as IGeoFeatureLayer;
    //                            string strLyrName = pLyr.Name;

    //                            //���������Դ ����Ҫ���������Ϊ׼ ���û�о���ͼ����Ϊ׼
    //                            //if (pGeoFeaLyr.FeatureClass != null)
    //                            //{
    //                            //    IDataset pDataset = pGeoFeaLyr.FeatureClass as IDataset;
    //                            //    strLyrName = pDataset.Name;
    //                            //}
    //                            if (strLyrName.Equals(name) || strLyrName.Equals("��Χ��"))
    //                            {
    //                                (pLayer as IGeoFeatureLayer).Renderer = pGeoFeaLyr.Renderer;
    //                                break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return;
    //        }
    //    }

    //    /// <summary>
    //    /// ������ϵ��(�������ݺͶ�Ӧ��־�޸ı�������)
    //    /// </summary>
    //    /// <param name="pOrgFeatCls"></param>
    //    /// <param name="strMDB"></param>
    //    /// <returns></returns>
    //    private static IFeatureClass GetJionFeatureClass(IFeatureClass pOrgFeatCls, string pOrgFieldName, IObjectClass pForeignFeatCls, string pForeignFieldName, IQueryFilter pQueryFilter, string fieldStateName, out Exception exError)
    //    {
    //        //�������ݺ���־��¼��Ĺ����༰��
    //        IRelationshipClass pJionRelationshipClass = SysCommon.Gis.ModGisPub.GetRelationShipClass("JionFc", (IObjectClass)pOrgFeatCls, pOrgFieldName, (IObjectClass)pForeignFeatCls, pForeignFieldName, "", "", esriRelCardinality.esriRelCardinalityOneToOne, out exError);
    //        if (pJionRelationshipClass == null) return null;
    //        IRelQueryTable pJionRelQueryTable = SysCommon.Gis.ModGisPub.GetRelQueryTable(pJionRelationshipClass, true, pQueryFilter, fieldStateName, false, true, out exError);
    //        if (pJionRelQueryTable == null) return null;

    //        IFeatureClass pOutFeatCLs = pJionRelQueryTable as IFeatureClass;
    //        return pOutFeatCLs;
    //    }

    //    /// <summary>
    //    /// Ϊͼ����ϱ����ע��
    //    /// </summary>
    //    /// <param name="StrScale"></param>
    //    /// <param name="MapControl"></param>
    //    /// <param name="fieldName"></param>
    //    public static void AddLabel(string StrScale, IMapControlDefault MapControl, Dictionary<string, string> fieldNameDics)
    //    {
    //        IGeoFeatureLayer pGeoFeatLayer;
    //        if (MapControl.LayerCount == 0) return;
    //        IFeatureLayer pFeatLay = ModDBOperator.GetMapFrameLayer(StrScale, MapControl, "ʾ��ͼ") as IFeatureLayer;
    //        if (pFeatLay == null) return;
    //        pGeoFeatLayer = pFeatLay as IGeoFeatureLayer;
    //        Dictionary<string, string> fieldList = new Dictionary<string, string>();
    //        for (int i = 0; i < pGeoFeatLayer.FeatureClass.Fields.FieldCount; i++)
    //        {
    //            IField pField = pGeoFeatLayer.FeatureClass.Fields.get_Field(i);
    //            if (pField == null) continue;
    //            foreach (string fieldName in fieldNameDics.Keys)
    //            {
    //                if (pField.Name.Contains(fieldName))
    //                {
    //                    fieldList.Add(pField.Name, fieldNameDics[fieldName]);
    //                    break;
    //                }
    //            }
    //        }
    //        string expression = "";
    //        if (fieldList.Count > 0)
    //        {
    //            foreach (string fieldName in fieldList.Keys)
    //            {
    //                if (string.IsNullOrEmpty(expression))
    //                {
    //                    expression = "\"" + fieldList[fieldName] + ":\" & [" + fieldName + "] & chr(10)";
    //                }
    //                else
    //                {
    //                    expression += " & \"" + fieldList[fieldName] + ":\" & [" + fieldName + "] & chr(10)";
    //                }
    //            }
    //        }
    //        //pGeoFeatLayer.DisplayField = fieldList[0].ToString();

    //        IAnnotateLayerPropertiesCollection pAnnoProps = null;
    //        pAnnoProps = pGeoFeatLayer.AnnotationProperties;

    //        ILineLabelPosition pPosition = null;
    //        pPosition = new LineLabelPositionClass();
    //        pPosition.Parallel = true;
    //        pPosition.Above = true;

    //        ILineLabelPlacementPriorities pPlacement = new LineLabelPlacementPrioritiesClass();
    //        IBasicOverposterLayerProperties4 pBasic = new BasicOverposterLayerPropertiesClass();
    //        pBasic.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
    //        pBasic.LineLabelPlacementPriorities = pPlacement;
    //        pBasic.LineLabelPosition = pPosition;
    //        pBasic.BufferRatio = 0;
    //        pBasic.FeatureWeight = esriBasicOverposterWeight.esriHighWeight;
    //        pBasic.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
    //        pBasic.PlaceOnlyInsidePolygon = true;

    //        ILabelEngineLayerProperties pLabelEngine = null;
    //        pLabelEngine = new LabelEngineLayerPropertiesClass();
    //        pLabelEngine.BasicOverposterLayerProperties = pBasic as IBasicOverposterLayerProperties;
    //        pLabelEngine.Expression = expression;
    //        pLabelEngine.Symbol.Size = 10;

    //        IAnnotateLayerProperties pAnnoLayerProps = null;
    //        pAnnoLayerProps = pLabelEngine as IAnnotateLayerProperties;
    //        pAnnoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
    //        pAnnoProps.Clear();
    //        pAnnoProps.Add(pAnnoLayerProps);
    //        pGeoFeatLayer.ScaleSymbols = false;
    //        pGeoFeatLayer.DisplayAnnotation = true;
    //        MapControl.ActiveView.Refresh();
    //    }

    //    /// <summary>
    //    /// Ϊͼ����ϱ����ע��
    //    /// </summary>
    //    /// <param name="StrScale"></param>
    //    /// <param name="MapControl"></param>
    //    /// <param name="fieldName"></param>
    //    public static void AddLabel(IFeatureLayer pFeatLay, IMapControlDefault MapControl, string fieldName)
    //    {
    //        IGeoFeatureLayer pGeoFeatLayer;
    //        if (MapControl.LayerCount == 0) return;
    //        if (pFeatLay == null) return;
    //        pGeoFeatLayer = pFeatLay as IGeoFeatureLayer;
    //        string expression = "[" + fieldName + "]";
    //        //pGeoFeatLayer.DisplayField = fieldList[0].ToString();

    //        IAnnotateLayerPropertiesCollection pAnnoProps = null;
    //        pAnnoProps = pGeoFeatLayer.AnnotationProperties;

    //        ILineLabelPosition pPosition = null;
    //        pPosition = new LineLabelPositionClass();
    //        pPosition.Parallel = true;
    //        pPosition.Above = true;

    //        ILineLabelPlacementPriorities pPlacement = new LineLabelPlacementPrioritiesClass();
    //        IBasicOverposterLayerProperties4 pBasic = new BasicOverposterLayerPropertiesClass();
    //        pBasic.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
    //        pBasic.LineLabelPlacementPriorities = pPlacement;
    //        pBasic.LineLabelPosition = pPosition;
    //        pBasic.BufferRatio = 0;
    //        pBasic.FeatureWeight = esriBasicOverposterWeight.esriHighWeight;
    //        pBasic.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
    //        pBasic.PlaceOnlyInsidePolygon = true;

    //        ILabelEngineLayerProperties pLabelEngine = null;
    //        pLabelEngine = new LabelEngineLayerPropertiesClass();
    //        pLabelEngine.BasicOverposterLayerProperties = pBasic as IBasicOverposterLayerProperties;
    //        pLabelEngine.Expression = expression;
    //        IRgbColor pRgbColor = new RgbColorClass();
    //        pRgbColor.Red = 25;
    //        pRgbColor.Green = 25;
    //        pRgbColor.Blue = 112;
    //        pLabelEngine.Symbol.Color = pRgbColor;
    //        pLabelEngine.Symbol.Size = 15;

    //        IAnnotateLayerProperties pAnnoLayerProps = null;
    //        pAnnoLayerProps = pLabelEngine as IAnnotateLayerProperties;
    //        pAnnoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
    //        pAnnoProps.Clear();
    //        pAnnoProps.Add(pAnnoLayerProps);
    //        pGeoFeatLayer.ScaleSymbols = false;
    //        pGeoFeatLayer.DisplayAnnotation = true;
    //        MapControl.ActiveView.Refresh();
    //    }

    //    /// <summary>
    //    /// ��ͼ����ϱ����ڵײ�
    //    /// </summary>
    //    /// <param name="pMapControl"></param>
    //    public static void MoveMapFrameLayer(IMapControlDefault pMapControl)
    //    {
    //        ILayer pLay = ModDBOperator.GetMapFrameLayer("zone", pMapControl, "ʾ��ͼ") as ILayer;
    //        if (pLay == null) return;
    //        pMapControl.Map.MoveLayer(pLay, pMapControl.LayerCount - 1);
    //    }

    //    /// <summary>
    //    /// �������ַ����õ���ΧPolygon
    //    /// </summary>
    //    /// <param name="strCoor">�����ַ���,��ʽΪX@Y,�Զ��ŷָ�</param>
    //    /// <returns></returns>
    //    public static IPolygon GetPolygonByCol(string strCoor)
    //    {
    //        try
    //        {
    //            object after = Type.Missing;
    //            object before = Type.Missing;
    //            IPolygon polygon = new PolygonClass();
    //            IPointCollection pPointCol = (IPointCollection)polygon;
    //            string[] strTemp = strCoor.Split(',');
    //            for (int index = 0; index < strTemp.Length; index++)
    //            {
    //                string CoorLine = strTemp[index];
    //                string[] coors = CoorLine.Split('@');

    //                double X = Convert.ToDouble(coors[0]);
    //                double Y = Convert.ToDouble(coors[1]);

    //                IPoint pPoint = new PointClass();
    //                pPoint.PutCoords(X, Y);
    //                pPointCol.AddPoint(pPoint, ref before, ref after);
    //            }

    //            polygon = (IPolygon)pPointCol;
    //            polygon.Close();

    //            if (IsValidateGeometry(polygon)) return polygon;
    //            return null;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// �ӷ�ΧPolygon�õ���Ӧ�������ַ���
    //    /// </summary>
    //    /// <param name="polygon"></param>
    //    /// <returns></returns>
    //    public static string GetColByPolygon(IPolygon polygon)
    //    {
    //        if (polygon == null) return "";
    //        IPointCollection pPointCol = (IPointCollection)polygon;

    //        try
    //        {
    //            StringBuilder sb = new StringBuilder();
    //            for (int index = 0; index < pPointCol.PointCount; index++)
    //            {
    //                IPoint pPoint = pPointCol.get_Point(index);

    //                string X = Convert.ToString(pPoint.X);
    //                string Y = Convert.ToString(pPoint.Y);

    //                if (sb.Length != 0)
    //                {
    //                    sb.Append(",");
    //                }
    //                sb.Append(X + "@" + Y);
    //            }

    //            return sb.ToString();
    //        }
    //        catch
    //        {
    //            return "";
    //        }
    //    }

    //    /// <summary>
    //    /// ���һ���������Ƿ�Ƿ�
    //    /// </summary>
    //    /// <param name="pgeometry"></param>
    //    /// <returns></returns>
    //    private static bool IsValidateGeometry(IGeometry pgeometry)
    //    {
    //        // ��ȡ��Geometry��ԭʼ����
    //        IPointCollection pOrgPointCol = (IPointCollection)pgeometry;

    //        // ��ȡ��Geometry��ԭʼPart��
    //        IGeometryCollection pOrgGeometryCol = (IGeometryCollection)pgeometry;

    //        // ��Ŀ����п�¡�Ͷ�Ӧ�Ĵ���
    //        IClone pClone = (IClone)pgeometry;
    //        IGeometry pGeometryTemp = (IPolygon)pClone.Clone();
    //        ITopologicalOperator pTopo = (ITopologicalOperator)pGeometryTemp;
    //        pTopo.Simplify();

    //        // �õ��µ�Geometry
    //        pGeometryTemp = (IPolygon)pTopo;

    //        // ��ȡ�µ�Geometry�ĵ���
    //        IPointCollection pObjPointCol = (IPointCollection)pGeometryTemp;

    //        // ��ȡ�µ�Geometry��Part��
    //        IGeometryCollection pObjGeometryCol = (IGeometryCollection)pGeometryTemp;

    //        // ���бȽ�
    //        if (pOrgPointCol.PointCount != pObjPointCol.PointCount) return false;

    //        if (pOrgGeometryCol.GeometryCount != pObjGeometryCol.GeometryCount) return false;

    //        return true;
    //    }

    //    /// <summary>
    //    /// ��XML��ȡ���ݿ����������ļ�
    //    /// </summary>
    //    /// <param name="strPath"></param>
    //    public static bool GetSettingXml(string strPath)
    //    {
    //        if (string.IsNullOrEmpty(strPath)) return false;
    //        try
    //        {
    //            XmlDocument doc = new XmlDocument();
    //            doc.Load(strPath);
    //            if (doc.DocumentElement != null)
    //            {
    //                XmlElement pElement = doc.DocumentElement;
    //                ModData.Server = pElement["server"].InnerText;
    //                ModData.Instance = pElement["service"].InnerText;
    //                ModData.Database = pElement["database"].InnerText;
    //                ModData.User = pElement["user"].InnerText;
    //                ModData.Password = pElement["password"].InnerText;
    //                ModData.Version = pElement["version"].InnerText;
    //                return true;
    //            }
    //            return false;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// ���ֵ��ListView
    //    /// </summary>
    //    /// <param name="listViewEx"></param>
    //    /// <param name="pFeaDic"></param>
    //    public static void BandListView(DevComponents.DotNetBar.Controls.ListViewEx listViewEx, Dictionary<int, IFeature> pFeaDic, int nameIndex)
    //    {
    //        if (pFeaDic.Count > 0)
    //        {
    //            listViewEx.Items.Clear();
    //            foreach (int id in pFeaDic.Keys)
    //            {
    //                ListViewItem item = new ListViewItem();
    //                item.Text = (pFeaDic[id] as IFeature).get_Value(nameIndex).ToString();
    //                item.Tag = pFeaDic[id];
    //                item.ImageIndex = 0;
    //                listViewEx.Items.Add(item);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// ���ֵ��ListView
    //    /// </summary>
    //    /// <param name="listViewEx"></param>
    //    /// <param name="pFeaDic"></param>
    //    public static void BandListView(DevComponents.DotNetBar.Controls.ListViewEx listViewEx, Dictionary<string, object[]> pFeaDic)
    //    {
    //        if (pFeaDic.Count > 0)
    //        {
    //            listViewEx.Items.Clear();
    //            object[] obj = null;
    //            foreach (string id in pFeaDic.Keys)
    //            {
    //                ListViewItem item = new ListViewItem();
    //                item.Text = pFeaDic[id][0].ToString();
    //                obj = new object[] { id, pFeaDic[id] };
    //                item.Tag = obj;
    //                item.ImageIndex = 0;
    //                listViewEx.Items.Add(item);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// ����ѯ���û�������Ϣ����ͼ
    //    /// </summary>
    //    /// <param name="tableName"></param>
    //    /// <param name="tree"></param>
    //    /// <param name="gisDb"></param>
    //    public static void DisplayUserView(string condition, DevComponents.DotNetBar.Controls.ListViewEx listView, IWorkspace gisWorkspace, out Exception exError)
    //    {
    //        exError = null;
    //        User user = null;

    //        try
    //        {
    //            SysGisTable sysTable = new SysGisTable(gisWorkspace);
    //            List<Dictionary<string, object>> lstDicData = sysTable.GetRows("user_info", condition, out exError);
    //            Dictionary<string, User> dic = new Dictionary<string, User>();
    //            if (lstDicData != null)
    //            {
    //                for (int i = 0; i < lstDicData.Count; i++)
    //                {
    //                    user = new User();
    //                    user.ID = lstDicData[i]["USERID"].ToString();
    //                    user.Name = lstDicData[i]["NAME"].ToString();
    //                    user.TrueName = lstDicData[i]["TRUTHNAME"].ToString();
    //                    user.Password = lstDicData[i]["UPWD"].ToString();
    //                    user.Sex = int.Parse(lstDicData[i]["USEX"].ToString());
    //                    user.Position = lstDicData[i]["UPOSITION"].ToString();
    //                    user.Remark = lstDicData[i]["UREMARK"].ToString();
    //                    dic.Add(lstDicData[i]["USERID"].ToString(), user);
    //                }
    //                if (dic.Count > 0)
    //                {
    //                    listView.Items.Clear();
    //                    foreach (string key in dic.Keys)
    //                    {
    //                        ListViewItem item = new ListViewItem();
    //                        user = dic[key] as User;
    //                        item.Text = user.Name + "(" + user.TrueName + ")";
    //                        item.Tag = user;
    //                        item.ImageIndex = 0;
    //                        listView.Items.Add(item);
    //                    }
    //                    //tree.Nodes[0].ExpandAll();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //        }
    //    }

    //    /// <summary>
    //    /// ����ѯ���û���������Ϣ��ComboBox
    //    /// </summary>
    //    /// <param name="tableName"></param>
    //    /// <param name="tree"></param>
    //    /// <param name="gisDb"></param>
    //    public static void DisplayRoleComboBox(string condition, DevComponents.DotNetBar.Controls.ComboBoxEx cboBox, IWorkspace gisWorkspace, out Exception exError)
    //    {
    //        exError = null;
    //        Role role = null;
    //        ComboBoxItem item;

    //        try
    //        {
    //            SysGisTable sysTable = new SysGisTable(gisWorkspace);
    //            List<Dictionary<string, object>> lstDicData = sysTable.GetRows("role", condition, out exError);
    //            Dictionary<string, Role> dic = new Dictionary<string, Role>();
    //            if (lstDicData != null)
    //            {
    //                for (int i = 0; i < lstDicData.Count; i++)
    //                {
    //                    role = new Role();
    //                    role.ID = lstDicData[i]["ROLEID"].ToString();
    //                    role.Name = lstDicData[i]["NAME"].ToString();
    //                    role.Privilege = lstDicData[i]["PRIVILEGE"] as XmlDocument;
    //                    role.Remark = lstDicData[i]["REMARK"].ToString();
    //                    dic.Add(lstDicData[i]["ROLEID"].ToString(), role);
    //                }
    //                if (dic.Count > 0)
    //                {
    //                    foreach (string key in dic.Keys)
    //                    {
    //                        role = dic[key] as Role;
    //                        item = new ComboBoxItem(role.Name, role.ID);
    //                        item.Tag = role;
    //                        cboBox.Items.Add(item);
    //                    }
    //                    //tree.Nodes[0].ExpandAll();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //        }
    //    }

    //    /// <summary>
    //    /// ��ȡ�û���ɫ���ձ��и�����ɫ���û�ID��
    //    /// </summary>
    //    /// <param name="userid"></param>
    //    /// <param name="gisDb"></param>
    //    /// <returns></returns>
    //    public static List<string> GetUserIds(string roleid, IWorkspace pWorkspace, out Exception exError)
    //    {
    //        exError = null;

    //        try
    //        {
    //            SysGisTable sysTable = new SysGisTable(pWorkspace);
    //            List<Dictionary<string, object>> lstDicData = sysTable.GetRows("user_role", "ROLEID='" + roleid + "'", out exError);
    //            List<string> ids = null;
    //            if (lstDicData != null && lstDicData.Count > 0)
    //            {
    //                ids = new List<string>();
    //                foreach (Dictionary<string, object> dic in lstDicData)
    //                {
    //                    foreach (string key in dic.Keys)
    //                    {
    //                        if (key.Equals("USERID"))
    //                        {
    //                            ids.Add(dic[key].ToString());
    //                        }
    //                    }
    //                }
    //            }
    //            return ids;
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// �����û�ID��ȡ�û���
    //    /// </summary>
    //    /// <param name="p"></param>
    //    /// <returns></returns>
    //    public static string GetNameFromUser(object userid, IWorkspace pWorkspace, out Exception exError)
    //    {
    //        exError = null;

    //        try
    //        {
    //            SysGisTable sysTable = new SysGisTable(pWorkspace);
    //            object name = sysTable.GetFieldValue("user_info", "TRUTHNAME", "userid='" + userid + "'", out exError);
    //            if (name != null)
    //            {
    //                return name.ToString();
    //            }
    //            return null;
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ��ȡ��Χ����ָ����������ĿID��
    //    /// </summary>
    //    /// <param name="userid"></param>
    //    /// <param name="gisDb"></param>
    //    /// <returns></returns>
    //    public static List<int> GetProjectIds(string tableName, string userid, string field, IWorkspace pWorkspace, out Exception exError)
    //    {
    //        exError = null;

    //        try
    //        {
    //            SysGisDataSet sysDataSet = new SysGisDataSet(pWorkspace);
    //            IFeatureCursor pFeatureCursor = sysDataSet.GetFeatureCursor(tableName, field + "='" + userid + "'", out exError);
    //            List<int> ids = null;
    //            if (pFeatureCursor != null)
    //            {
    //                ids = new List<int>();
    //                IFeature pFeature = pFeatureCursor.NextFeature();
    //                while (pFeature != null)
    //                {
    //                    int index = pFeature.Fields.FindField("PRJID");
    //                    int tempId = int.Parse(pFeature.get_Value(index).ToString());
    //                    if (!ids.Contains(tempId))
    //                    {
    //                        ids.Add(tempId);
    //                    }
    //                    pFeature = pFeatureCursor.NextFeature();
    //                }
    //            }
    //            return ids;
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ��ȡ������Ϣ���е�ָ�������µ���ĿID��
    //    /// </summary>
    //    /// <param name="userid"></param>
    //    /// <param name="gisDb"></param>
    //    /// <returns></returns>
    //    public static List<int> GetTableProjectIds(string tableName, string userid, string field, IWorkspace pWorkspace, out Exception exError)
    //    {
    //        exError = null;

    //        try
    //        {
    //            SysGisTable sysTable = new SysGisTable(pWorkspace);
    //            List<object> prjIds = sysTable.GetFieldValues(tableName, "PRJID", field + "='" + userid + "'", out exError);
    //            if (prjIds == null || prjIds.Count <= 0) return null;
    //            List<int> ids = new List<int>();
    //            foreach (object obj in prjIds)
    //            {
    //                if (obj != null)
    //                {
    //                    if (!ids.Contains((int)obj))
    //                    {
    //                        ids.Add((int)obj);
    //                    }
    //                }
    //            }
    //            return ids;
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ��������Ϣ�󶨵�lstProject��
    //    /// </summary>
    //    /// <param name="p"></param>
    //    /// <param name="lstProject"></param>
    //    /// <param name="iWorkspace"></param>
    //    /// <param name="exError"></param>
    //    public static void DisplayProjectView(string condition, DevComponents.DotNetBar.Controls.ListViewEx listView, IWorkspace pWorkspace, out Exception exError)
    //    {
    //        exError = null;
    //        Project project;

    //        try
    //        {
    //            SysGisTable sysTable = new SysGisTable(pWorkspace);
    //            List<Dictionary<string, object>> lstDicData = sysTable.GetRows("PROJECT", condition, out exError);
    //            Dictionary<int, Project> dic;
    //            if (lstDicData != null)
    //            {
    //                dic = new Dictionary<int, Project>();
    //                for (int i = 0; i < lstDicData.Count; i++)
    //                {
    //                    project = new Project();
    //                    project.OID = int.Parse(lstDicData[i]["OBJECTID"].ToString());
    //                    project.Name = lstDicData[i]["NAME"].ToString();
    //                    project.Managerid = lstDicData[i]["MANAGERID"].ToString();
    //                    project.Qcid = lstDicData[i]["QCID"].ToString() == "" ? -1 : int.Parse(lstDicData[i]["QCID"].ToString());
    //                    project.Scid = lstDicData[i]["SCID"].ToString() == "" ? -1 : int.Parse(lstDicData[i]["QCID"].ToString());
    //                    project.Create_Time = lstDicData[i]["CREATE_TIME"].ToString();
    //                    project.End_Time = lstDicData[i]["END_TIME"].ToString();
    //                    project.Remark = lstDicData[i]["REMARK"].ToString();
    //                    project.DataType = lstDicData[i]["DATATYPE"].ToString();
    //                    project.Scale = lstDicData[i]["SCALE"].ToString();
    //                    project.ObjDataType = lstDicData[i]["OBJDATATYPE"].ToString();
    //                    project.Server = lstDicData[i]["SERVER"].ToString();
    //                    project.Service = lstDicData[i]["SERVICE"].ToString();
    //                    project.DataBase = lstDicData[i]["DATABASE"].ToString();
    //                    project.UserName = lstDicData[i]["USERNAME"].ToString();
    //                    project.Password = lstDicData[i]["PASSWORD"].ToString();
    //                    project.Version = lstDicData[i]["VERSION"].ToString();
    //                    dic.Add(int.Parse(lstDicData[i]["OBJECTID"].ToString()), project);
    //                }
    //                if (dic.Count > 0)
    //                {
    //                    listView.Items.Clear();
    //                    foreach (int key in dic.Keys)
    //                    {
    //                        ListViewItem item = new ListViewItem();
    //                        project = dic[key] as Project;
    //                        item.Text = project.Name;
    //                        item.Tag = project;
    //                        item.ImageIndex = 0;
    //                        listView.Items.Add(item);
    //                    }
    //                    //tree.Nodes[0].ExpandAll();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            exError = ex;
    //        }
    //    }

    //    ///// <summary>
    //    ///// �������������Ĳ�����ʼ����ͼ
    //    ///// </summary>
    //    ///// <param name="projectTree"></param>
    //    ///// <param name="taskDocument"></param>
    //    ///// <param name="taskType"></param>
    //    ///// <param name="eError"></param>
    //    ///// <returns></returns>
    //    //public static bool OpenTaskXml(DevComponents.AdvTree.AdvTree projectTree, XmlDocument taskDocument, string userId, enumTaskType taskType, out Exception eError)
    //    //{
    //    //    eError = null;
    //    //    if (taskDocument == null || projectTree == null) return false;
    //    //    XmlElement aElement = taskDocument.SelectSingleNode(".//��Ŀ����") as XmlElement;
    //    //    if (aElement == null) return false;
    //    //    string StrScale = aElement.GetAttribute("������").ToString();
    //    //    ModData.Scale = StrScale;
    //    //    string prjid = aElement.GetAttribute("��Ŀ���").ToString();

    //    //    //���Ԥ����õķ��ŵ�ͼ�ĵ�
    //    //    IMapDocument pMapDoc = null;
    //    //    string strMxdPath = Application.StartupPath + "\\״̬���ŷ���.mxd";
    //    //    if (File.Exists(strMxdPath))
    //    //    {
    //    //        pMapDoc = new MapDocumentClass();
    //    //        if (pMapDoc.get_IsMapDocument(strMxdPath))
    //    //        {
    //    //            pMapDoc.Open(strMxdPath, "");
    //    //        }
    //    //        else
    //    //        {
    //    //            pMapDoc = null;
    //    //        }
    //    //    }
    //    //    //�汾ˢ��
    //    //    (ModData.v_SysDataSet.WorkSpace as IVersion).RefreshVersion();

    //    //    //��ʼ��������ͼ�����ط�Χ
    //    //    InitialDBTree newInitialDBTree = new InitialDBTree();

    //    //    //���¹���֮ǰ�����ԭ��Χ
    //    //    ModData.v_AppGisUpdate.MapControl.ClearLayers();
    //    //    // ���Ԫ��
    //    //    ModData.v_AppGisUpdate.MapControl.ActiveView.GraphicsContainer.DeleteAllElements();

    //    //    //ͼ����Χ
    //    //    XmlElement tElement = null;
    //    //    //���ڷ�Χ
    //    //    XmlElement zElement = null;
    //    //    //���䷶Χ
    //    //    XmlElement iElement = null;
    //    //    //��¼������Ŀ��Ӧ�����ڷ�Χ�漯
    //    //    List<string> zoneList = null;
    //    //    string whereClause = "PRJID=" + prjid;
    //    //    string tempWhereClause = "";
    //    //    ModData.v_CurrentTaskType = taskType;
    //    //    try
    //    //    {
    //    //        switch (taskType)
    //    //        {
    //    //            case enumTaskType.MANAGER:
    //    //                newInitialDBTree.InitTaskTree(projectTree, taskDocument, enumTaskType.MANAGER);
    //    //                //�жϹ��������·�Χ����Ƿ�Ϊ��
    //    //                tElement = aElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
    //    //                zElement = aElement.SelectSingleNode("..//��������//����") as XmlElement;
    //    //                iElement = aElement.SelectSingleNode(".//��������//����") as XmlElement;
    //    //                break;
    //    //            case enumTaskType.SPOTCHECK:
    //    //                newInitialDBTree.InitTaskTree(projectTree, taskDocument, enumTaskType.SPOTCHECK);
    //    //                //�жϹ��������·�Χ����Ƿ�Ϊ��
    //    //                tElement = aElement.SelectSingleNode(".//�������//��Χ") as XmlElement;
    //    //                zElement = aElement.SelectSingleNode("..//�������//����") as XmlElement;
    //    //                iElement = aElement.SelectSingleNode(".//�������//����") as XmlElement;
    //    //                break;
    //    //            case enumTaskType.ZONE:
    //    //                newInitialDBTree.InitTaskTree(projectTree, taskDocument, enumTaskType.ZONE);
    //    //                //�жϹ��������·�Χ����Ƿ�Ϊ��
    //    //                tElement = aElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
    //    //                zElement = aElement.SelectSingleNode("..//��������//����") as XmlElement;
    //    //                if (zElement != null)
    //    //                {
    //    //                    //�����������Ҫ�أ�ֱ����������Ҫ����ʾ����
    //    //                    if (zElement.ChildNodes.Count != 0)
    //    //                    {
    //    //                        foreach (XmlElement element in zElement.ChildNodes)
    //    //                        {
    //    //                            string zoneid = element.GetAttribute("value");
    //    //                            if (string.IsNullOrEmpty(tempWhereClause))
    //    //                            {
    //    //                                tempWhereClause = "'" + zoneid + "'";
    //    //                            }
    //    //                            else
    //    //                            {
    //    //                                tempWhereClause += ",'" + zoneid + "'";
    //    //                            }
    //    //                        }
    //    //                    }
    //    //                }
    //    //                break;
    //    //            case enumTaskType.INTERZONE:
    //    //                newInitialDBTree.InitTaskTree(projectTree, taskDocument, enumTaskType.INTERZONE);
    //    //                //�жϹ��������·�Χ����Ƿ�Ϊ��
    //    //                tElement = aElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
    //    //                iElement = aElement.SelectSingleNode(".//��������//����") as XmlElement;
    //    //                if (iElement != null)
    //    //                {
    //    //                    //������������ȡ����Ҫ����ʾ����
    //    //                    List<string> tempIds = new List<string>();
    //    //                    foreach (XmlElement element in iElement.ChildNodes)
    //    //                    {
    //    //                        string[] strIds = element.GetAttribute("value").Split(',');
    //    //                        if (strIds.Length > 0)
    //    //                        {
    //    //                            foreach (string str in strIds)
    //    //                            {
    //    //                                if (!tempIds.Contains(str))
    //    //                                {
    //    //                                    tempIds.Add(str);
    //    //                                }
    //    //                            }
    //    //                        }
    //    //                    }
    //    //                    if (tempIds.Count > 0)
    //    //                    {
    //    //                        foreach (string strId in tempIds)
    //    //                        {
    //    //                            if (string.IsNullOrEmpty(tempWhereClause))
    //    //                            {
    //    //                                tempWhereClause = strId;
    //    //                            }
    //    //                            else
    //    //                            {
    //    //                                tempWhereClause += "," + strId;
    //    //                            }
    //    //                        }
    //    //                    }
    //    //                }
    //    //                break;
    //    //            case enumTaskType.CHECK:
    //    //                newInitialDBTree.InitTaskTree(projectTree, taskDocument, enumTaskType.CHECK);
    //    //                //�жϹ��������·�Χ����Ƿ�Ϊ��
    //    //                tElement = aElement.SelectSingleNode(".//�������//��Χ") as XmlElement;
    //    //                zElement = aElement.SelectSingleNode("..//�������//����") as XmlElement;
    //    //                if (zElement != null)
    //    //                {
    //    //                    //�����������Ҫ�أ�ֱ����������Ҫ����ʾ����
    //    //                    if (zElement.ChildNodes.Count != 0)
    //    //                    {
    //    //                        zoneList = new List<string>();
    //    //                        foreach (XmlElement element in zElement.ChildNodes)
    //    //                        {
    //    //                            string zoneid = element.GetAttribute("value");
    //    //                            if (string.IsNullOrEmpty(tempWhereClause))
    //    //                            {
    //    //                                tempWhereClause = "'" + zoneid + "'";
    //    //                            }
    //    //                            else
    //    //                            {
    //    //                                tempWhereClause += ",'" + zoneid + "'";
    //    //                            }
    //    //                            if (!zoneList.Contains(zoneid))
    //    //                            {
    //    //                                zoneList.Add(zoneid);
    //    //                            }
    //    //                        }
    //    //                    }
    //    //                }
    //    //                break;
    //    //        }
    //    //        if (!string.IsNullOrEmpty(tempWhereClause))
    //    //        {
    //    //            whereClause += " and objectid in (" + tempWhereClause + ")";
    //    //        }

    //    //        //���ط�Χ��
    //    //        IQueryFilter pQueryFilter = new QueryFilterClass();
    //    //        pQueryFilter.WhereClause = whereClause;
    //    //        ModDBOperator.AddLayer("zone", pQueryFilter, ModData.v_AppGisUpdate.MapControl, ModData.v_SysDataSet, pMapDoc, out eError);
    //    //        Dictionary<string, string> fieldDic = new Dictionary<string, string>();
    //    //        fieldDic.Add("ZONENAME", "��Χ��");
    //    //        fieldDic.Add("EMPNAME", "��ҵ��");
    //    //        fieldDic.Add("QCNAME", "�����");
    //    //        ModDBOperator.AddLabel("zone", ModData.v_AppGisUpdate.MapControl, fieldDic);

    //    //        //���ط�Χ��
    //    //        if (iElement != null)
    //    //        {
    //    //            whereClause = "";
    //    //            foreach (XmlElement element in iElement.ChildNodes)
    //    //            {
    //    //                string lineid = element.GetAttribute("value");
    //    //                if (string.IsNullOrEmpty(whereClause))
    //    //                {
    //    //                    whereClause = "'" + lineid + "'";
    //    //                }
    //    //                else
    //    //                {
    //    //                    whereClause += ",'" + lineid + "'";
    //    //                }
    //    //            }
    //    //            if (!string.IsNullOrEmpty(whereClause))
    //    //            {
    //    //                whereClause = ModData.User+"."+"bound_line.LINEID IN (" + whereClause + ") AND INTERZONE.PRJID=" + prjid;
    //    //            }
    //    //            else
    //    //            {
    //    //                whereClause = ModData.User + ".INTERZONE.PRJID=" + prjid;
    //    //            }
    //    //            pQueryFilter.WhereClause = whereClause;
    //    //            ModDBOperator.AddInterZoneLayer("bound_line", pQueryFilter, ModData.v_AppGisUpdate.MapControl, ModData.v_SysDataSet, pMapDoc, out eError);
    //    //            fieldDic = new Dictionary<string, string>();
    //    //            fieldDic.Add("LINENAME", "��Χ��");
    //    //            fieldDic.Add("EMPNAME", "��ҵ��");
    //    //            ModDBOperator.AddLabel("bound_line", ModData.v_AppGisUpdate.MapControl, fieldDic);
    //    //        }
    //    //        else
    //    //        {
    //    //            if (taskType == enumTaskType.CHECK)
    //    //            {
    //    //                whereClause = "";
    //    //                //��ȡ��Χ��ķ�Χ
    //    //                if (zoneList != null && zoneList.Count > 0)
    //    //                {
    //    //                    string strSql = "";
    //    //                    foreach (string zid in zoneList)
    //    //                    {
    //    //                        if (string.IsNullOrEmpty(strSql))
    //    //                        {
    //    //                            strSql = zid;
    //    //                        }
    //    //                        else
    //    //                        {
    //    //                            strSql += "," + zid;
    //    //                        }
    //    //                    }
    //    //                    string sql = "OBJECTID IN (" + strSql + ")";
    //    //                    IGeometry pZonesGeometry = ModDBOperator.GetZonesGeometry(sql, ModData.v_SysDataSet, out eError);
    //    //                    //���ݵ�ǰ��Χ��ķ�Χȥ��ѯ�ཻ�ķ�Χ��,Ȼ����ʾ
    //    //                    List<string> pInterZones = ModDBOperator.GetInterZones("bound_line", pZonesGeometry, ModData.v_SysDataSet, out eError);
    //    //                    if (pInterZones != null)
    //    //                    {
    //    //                        if (pInterZones.Count > 0)
    //    //                        {
    //    //                            foreach (string interId in pInterZones)
    //    //                            {
    //    //                                if (string.IsNullOrEmpty(whereClause))
    //    //                                {
    //    //                                    whereClause = "'" + interId + "'";
    //    //                                }
    //    //                                else
    //    //                                {
    //    //                                    whereClause += ",'" + interId + "'";
    //    //                                }
    //    //                            }
    //    //                            whereClause = ModData.User + "." + "bound_line.LINEID IN (" + whereClause + ") and INTERZONE.PRJID=" + prjid;
    //    //                        }
    //    //                    }
    //    //                }
    //    //                else
    //    //                {
    //    //                    //��ʾ���з�Χ��
    //    //                    whereClause = ModData.User+".INTERZONE.PRJID=" + prjid;
    //    //                }
    //    //                pQueryFilter.WhereClause = whereClause;
    //    //                ModDBOperator.AddInterZoneLayer("bound_line", pQueryFilter, ModData.v_AppGisUpdate.MapControl, ModData.v_SysDataSet, pMapDoc, out eError);
    //    //                fieldDic = new Dictionary<string, string>();
    //    //                fieldDic.Add("LINENAME", "��Χ��");
    //    //                fieldDic.Add("EMPNAME", "��ҵ��");
    //    //                ModDBOperator.AddLabel("bound_line", ModData.v_AppGisUpdate.MapControl, fieldDic);
    //    //            }
    //    //        }

    //    //        //����ͼ��
    //    //        if (tElement != null)
    //    //        {
    //    //            whereClause = "";
    //    //            pQueryFilter = new QueryFilterClass();
    //    //            //��¼�ֶε�ͼ���ż�
    //    //            List<StringBuilder> sbList = null;
    //    //            StringBuilder sb = null;
    //    //            //���ͼ����������100��ֶ���������
    //    //            if (tElement.ChildNodes.Count >= 1000)
    //    //            {
    //    //                sbList = new List<StringBuilder>();
    //    //                for (int n = 0; n < tElement.ChildNodes.Count / 500 + 1; n++)
    //    //                {
    //    //                    sb = new StringBuilder();
    //    //                    for (int j = n * 500; j < (n + 1) * 500 && j < tElement.ChildNodes.Count; j++)
    //    //                    {
    //    //                        //��¼�ֶε�ͼ����
    //    //                        XmlElement element = tElement.ChildNodes[j] as XmlElement;
    //    //                        string mapNo = element.GetAttribute("ͼ����");
    //    //                        if (!string.IsNullOrEmpty(mapNo))
    //    //                        {
    //    //                            if (sb.Length != 0)
    //    //                            {
    //    //                                sb.Append(",");
    //    //                            }
    //    //                            sb.Append("'" + mapNo + "'");
    //    //                        }
    //    //                    }
    //    //                    sbList.Add(sb);
    //    //                }
    //    //            }
    //    //            else
    //    //            {
    //    //                sb = new StringBuilder();
    //    //                //�������ݸ��¶Ա��б���л�ȡ����
    //    //                for (int i = 0; i < tElement.ChildNodes.Count; i++)
    //    //                {
    //    //                    //��¼�ֶε�ͼ����
    //    //                    XmlElement element = tElement.ChildNodes[i] as XmlElement;
    //    //                    string mapNo = element.GetAttribute("ͼ����");
    //    //                    if (!string.IsNullOrEmpty(mapNo))
    //    //                    {
    //    //                        if (sb.Length != 0)
    //    //                        {
    //    //                            sb.Append(",");
    //    //                        }
    //    //                        sb.Append("'" + mapNo + "'");
    //    //                    }
    //    //                }
    //    //            }
    //    //            if (sbList != null && sbList.Count > 0)
    //    //            {
    //    //                sb = new StringBuilder();
    //    //                foreach (StringBuilder sbuilder in sbList)
    //    //                {
    //    //                    if (string.IsNullOrEmpty(sb.ToString()))
    //    //                    {
    //    //                        sb.Append("MAP_NO IN(" + sbuilder.ToString() + ")");
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        sb.Append(" OR MAP_NO IN (" + sbuilder.ToString() + ")");
    //    //                    }
    //    //                }
    //    //                if (sb.Length != 0)
    //    //                {
    //    //                    if (whereClause == "")
    //    //                    {
    //    //                        whereClause += sb.ToString();
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        whereClause += " and " + sb.ToString();
    //    //                    }
    //    //                }
    //    //            }
    //    //            else
    //    //            {
    //    //                if (sb.Length != 0)
    //    //                {
    //    //                    if (whereClause == "")
    //    //                    {
    //    //                        whereClause += "MAP_NO in (" + sb.ToString() + ")";
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        whereClause += " and MAP_NO in (" + sb.ToString() + ")";
    //    //                    }
    //    //                }
    //    //            }
    //    //            pQueryFilter.WhereClause = whereClause;
    //    //            IQueryFilter pForeignFilter = new QueryFilterClass();
    //    //            string foreignWhereClause = "";
    //    //            if (taskType == enumTaskType.MANAGER || taskType == enumTaskType.CHECK)
    //    //            {
    //    //                foreignWhereClause = "PRJID=" + prjid;
    //    //            }
    //    //            else if (taskType == enumTaskType.ZONE || taskType == enumTaskType.INTERZONE)
    //    //            {
    //    //                foreignWhereClause = "PRJID=" + prjid + " AND EMPID='" + userId + "'";
    //    //            }
    //    //            pForeignFilter.WhereClause = foreignWhereClause;
    //    //            ModDBOperator.AddMapLayer(StrScale, pQueryFilter, pForeignFilter, ModData.v_AppGisUpdate.MapControl, ModData.v_SysDataSet, pMapDoc, out eError);
    //    //            //ModDBOperator.AddLabel(StrScale, ModData.v_AppGisUpdate.MapControl, "MAP_NO");
    //    //        }
    //    //        //ˢ��
    //    //        ModData.v_AppGisUpdate.MapControl.Map.MapUnits = esriUnits.esriMeters;
    //    //        ModData.v_AppGisUpdate.MapControl.Map.ClipGeometry = null;
    //    //        ModData.v_AppGisUpdate.MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
    //    //        ModData.v_AppGisUpdate.TOCControl.Update();
    //    //        ModData.v_AppGisUpdate.MapControl.Map.ReferenceScale = Convert.ToDouble("10000");

    //    //        //��ո���ͼ
    //    //        ModData.v_AppGisUpdate.DataTree.Nodes.Clear();
    //    //        ModData.v_AppGisUpdate.ErrTree.Nodes.Clear();
    //    //        ModData.v_AppGisUpdate.UpdateDataGrid.DataSource = null;

    //    //        //ˢ�����ݸ��¶Ա��б�
    //    //        ModData.v_RefreshUpdateDataInfo = true;
    //    //        return true;
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        eError = ex;
    //    //        return false;
    //    //    }
    //    //}

    //    /// <summary>
    //    /// �õ���ǰ��Χ�ڵ���������Ҫ��
    //    /// </summary>
    //    /// <param name="p"></param>
    //    /// <param name="pZonesGeometry"></param>
    //    /// <param name="sysGisDataSet"></param>
    //    /// <param name="eError"></param>
    //    /// <returns></returns>
    //    private static List<string> GetInterZones(string interZone, IGeometry pZonesGeometry, SysGisDataSet sysGisDataSet, out Exception eError)
    //    {
    //        List<string> interZones = null;
    //        try
    //        {
    //            IFeatureCursor pFeatureCursor = sysGisDataSet.GetFeatureCursor(interZone, "", pZonesGeometry, esriSpatialRelEnum.esriSpatialRelIntersects, out eError);
    //            if (pFeatureCursor != null)
    //            {
    //                interZones = new List<string>();
    //                IFeature pFeature = pFeatureCursor.NextFeature();
    //                int lineIdIndex = pFeatureCursor.Fields.FindField("LINEID");
    //                while (pFeature != null && lineIdIndex != -1)
    //                {
    //                    string lineId = pFeature.get_Value(lineIdIndex).ToString();
    //                    if (!interZones.Contains(lineId))
    //                    {
    //                        interZones.Add(lineId);
    //                    }
    //                    pFeature = pFeatureCursor.NextFeature();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //        }
    //        return interZones;
    //    }

    //    /// <summary>
    //    /// ����ָ�������µķ�Χ��ķ�Χ
    //    /// </summary>
    //    /// <param name="condition"></param>
    //    /// <param name="sysDataSet"></param>
    //    /// <param name="eError"></param>
    //    /// <returns></returns>
    //    public static IGeometry GetZonesGeometry(string condition, SysCommon.Gis.SysGisDataSet sysDataSet, out Exception eError)
    //    {
    //        eError = null;
    //        IGeometry pZoneGeometry = null;
    //        try
    //        {
    //            IFeatureCursor pZoneCursor = sysDataSet.GetFeatureCursor("zone", condition, out eError);
    //            if (pZoneCursor != null)
    //            {
    //                IFeature pFeature = pZoneCursor.NextFeature();
    //                while (pFeature != null)
    //                {
    //                    if (pZoneGeometry == null)
    //                    {
    //                        pZoneGeometry = pFeature.Shape;
    //                    }
    //                    else
    //                    {
    //                        pZoneGeometry = (pZoneGeometry as ITopologicalOperator).Union(pFeature.Shape);
    //                    }
    //                    pFeature = pZoneCursor.NextFeature();
    //                }
    //            }
    //            return pZoneGeometry;
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ����ָ����Χ��ѯͼ����
    //    /// </summary>
    //    /// <param name="mapName"></param>
    //    /// <param name="pGeometry"></param>
    //    /// <param name="sysDataSet"></param>
    //    /// <param name="eError"></param>
    //    /// <returns></returns>
    //    public static List<string> GetMapNoByGeometry(string mapName, IGeometry pGeometry, SysCommon.Gis.SysGisDataSet sysDataSet, out Exception eError)
    //    {
    //        eError = null;
    //        List<string> mapNums = null;
    //        IFeatureClass pZoneFeatureClass = null;
    //        try
    //        {
    //            IFeatureDataset pFeatureDataset = sysDataSet.GetFeatureDataset("assistant", out eError);
    //            IEnumDataset pEnumDataset = pFeatureDataset.Subsets;
    //            IDataset pDataSet = pEnumDataset.Next();
    //            while (pDataSet != null)
    //            {
    //                if (pDataSet.Name.Contains(mapName))
    //                {
    //                    pZoneFeatureClass = pDataSet as IFeatureClass;
    //                    break;
    //                }
    //                pDataSet = pEnumDataset.Next();
    //            }
    //            if (pZoneFeatureClass == null) return null;
    //            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
    //            pSpatialFilter.Geometry = pGeometry;
    //            pSpatialFilter.GeometryField = "Shape";
    //            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

    //            IFeatureCursor pMapNoCursor = pZoneFeatureClass.Search(pSpatialFilter, true);
    //            if (pMapNoCursor != null)
    //            {
    //                mapNums = new List<string>();
    //                IFeature pFeature = pMapNoCursor.NextFeature();
    //                int index = pMapNoCursor.Fields.FindField("map_no");
    //                while (pFeature != null)
    //                {
    //                    if (index != -1)
    //                    {
    //                        string mapNo = pFeature.get_Value(index).ToString();
    //                        if (!mapNums.Contains(mapNo))
    //                        {
    //                            mapNums.Add(mapNo);
    //                        }
    //                    }
    //                    pFeature = pMapNoCursor.NextFeature();
    //                }
    //            }
    //            return mapNums;
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ����һ��Ŀ¼�����ݵ���һ��Ŀ¼
    //    /// </summary>
    //    /// <param name="yanfilepath"></param>
    //    /// <param name="mudifilepath"></param>
    //    public static bool CopyFilesDirs(string yanfilepath, string mudifilepath, out Exception eError)
    //    {
    //        eError = null;
    //        bool result = true;
    //        try
    //        {
    //            string[] arrDirs = Directory.GetDirectories(yanfilepath);
    //            string[] arrFiles = Directory.GetFiles(yanfilepath);
    //            if (arrFiles.Length != 0)
    //            {
    //                for (int i = 0; i < arrFiles.Length; i++)
    //                {
    //                    File.Copy(yanfilepath + "\\" + System.IO.Path.GetFileName(arrFiles[i]), mudifilepath + "\\"
    //                    + System.IO.Path.GetFileName(arrFiles[i]), true);
    //                }
    //            }
    //            if (arrDirs.Length != 0)
    //            {
    //                for (int i = 0; i < arrDirs.Length; i++)
    //                {
    //                    Directory.CreateDirectory(mudifilepath + "\\" + System.IO.Path.GetFileName(arrDirs[i]));
    //                    //�ݹ����   
    //                    CopyFilesDirs(yanfilepath + "\\" + System.IO.Path.GetFileName(arrDirs[i]),
    //                    mudifilepath + "\\" + System.IO.Path.GetFileName(arrDirs[i]), out eError);
    //                }
    //            }
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// ���ɸ��¹���
    //    /// </summary>
    //    /// <param name="FormulaList"></param>
    //    /// <returns></returns>
    //    //public static XmlDocument GetUpdateXml(List<FormulaXml> FormulaList, string FormulaName, List<FormulaLayer> FormulaLayerList, List<FormulaField> FormulaFieldList)
    //    //{
    //    //    XmlDocument document = new XmlDocument();
    //    //    string strXml = "<" + FormulaName + "></" + FormulaName + ">";
    //    //    document.LoadXml(strXml);
    //    //    Dictionary<string, string> objectClassDic = new Dictionary<string, string>();
    //    //    //������Ӧ�Ĺ���
    //    //    foreach (FormulaXml xmlFormula in FormulaList)
    //    //    {
    //    //        XmlElement styleElement = document.CreateElement("���");
    //    //        document[FormulaName].AppendChild(styleElement as XmlNode);
    //    //        XmlElement styleEleName = document.CreateElement("����");
    //    //        styleEleName.InnerText = xmlFormula.Name;
    //    //        styleElement.AppendChild(styleEleName);
    //    //        XmlElement styleEle;
    //    //        if (xmlFormula.StyleList == null) return null;
    //    //        foreach (FormulaStyle style in xmlFormula.StyleList)
    //    //        {
    //    //            styleEle = document.CreateElement("����");
    //    //            styleElement.AppendChild(styleEle as XmlNode);
    //    //            XmlElement styleSubName = document.CreateElement("����");

    //    //            styleSubName.InnerText = style.Style;
    //    //            styleEle.AppendChild(styleSubName as XmlNode);
    //    //            XmlElement styleOrg = document.CreateElement("Դ����");
    //    //            styleEle.AppendChild(styleOrg as XmlNode);
    //    //            XmlElement orgName = document.CreateElement("����");
    //    //            orgName.InnerText = style.OrgTable.Name;
    //    //            styleOrg.AppendChild(orgName as XmlNode);
    //    //            if (!string.IsNullOrEmpty(style.OrgTable.ConditionType))
    //    //            {
    //    //                XmlElement conditionEle = document.CreateElement("Condition");
    //    //                conditionEle.SetAttribute("ConditionType", style.OrgTable.ConditionType);
    //    //                styleOrg.AppendChild(conditionEle as XmlNode);
    //    //                XmlElement attr = document.CreateElement("AttributeName");
    //    //                attr.InnerText = style.OrgTable.AttributeName;
    //    //                conditionEle.AppendChild(attr as XmlNode);
    //    //                XmlElement oper = document.CreateElement("Operator");
    //    //                oper.InnerText = style.OrgTable.Operator == "" ? "EQ" : style.OrgTable.Operator;
    //    //                conditionEle.AppendChild(oper as XmlNode);
    //    //                XmlElement attrValue = document.CreateElement("AttributeValue");
    //    //                attrValue.InnerText = style.OrgTable.AttributeValue;
    //    //                conditionEle.AppendChild(attrValue as XmlNode);
    //    //            }
    //    //            XmlElement styleObj = document.CreateElement("Ŀ������");
    //    //            styleEle.AppendChild(styleObj as XmlNode);
    //    //            XmlElement objName = document.CreateElement("����");
    //    //            objName.InnerText = style.ObjTable.Name;
    //    //            styleObj.AppendChild(objName as XmlNode);
    //    //            if (!objectClassDic.ContainsKey(style.ObjTable.Name))
    //    //            {
    //    //                objectClassDic.Add(style.ObjTable.Name, style.Style);
    //    //            }
    //    //            XmlElement fieldMapping = document.CreateElement("�ֶζ�Ӧ��ϵ");
    //    //            styleObj.AppendChild(fieldMapping as XmlNode);
    //    //            XmlElement fieldNameELe = document.CreateElement("����");
    //    //            fieldNameELe.InnerText = "*";
    //    //            fieldMapping.AppendChild(fieldNameELe as XmlNode);
    //    //            XmlElement value = document.CreateElement("ֵ");
    //    //            value.InnerText = "*";
    //    //            fieldMapping.AppendChild(value as XmlNode);
    //    //            if (style.ObjTable.FieldMapping.Count > 0)
    //    //            {
    //    //                foreach (string fieldName in style.ObjTable.FieldMapping.Keys)
    //    //                {
    //    //                    fieldMapping = document.CreateElement("�ֶζ�Ӧ��ϵ");
    //    //                    styleObj.AppendChild(fieldMapping as XmlNode);
    //    //                    fieldNameELe = document.CreateElement("����");
    //    //                    fieldNameELe.InnerText = style.ObjTable.FieldMapping[fieldName];
    //    //                    fieldMapping.AppendChild(fieldNameELe as XmlNode);
    //    //                    value = document.CreateElement("ֵ");
    //    //                    if (fieldName.Equals("-1"))
    //    //                    {
    //    //                        value.InnerText = "-1";
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        value.InnerText = "%" + fieldName;
    //    //                    }
    //    //                    fieldMapping.AppendChild(value as XmlNode);
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //    //����Ŀ�����ṹ
    //    //    if (FormulaLayerList != null)
    //    //    {
    //    //        XmlElement objectDataElement = document.CreateElement("Ŀ�����ṹ");
    //    //        document[FormulaName].AppendChild(objectDataElement as XmlNode);
    //    //        foreach (string objName in objectClassDic.Keys)
    //    //        {
    //    //            XmlElement objData = document.CreateElement("Ŀ������");
    //    //            objectDataElement.AppendChild(objData as XmlNode);
    //    //            XmlElement objNameElement = document.CreateElement("����");
    //    //            objNameElement.InnerText = objName;
    //    //            objData.AppendChild(objNameElement as XmlNode);
    //    //            XmlElement objType = document.CreateElement("����");
    //    //            objType.InnerText = objectClassDic[objName];
    //    //            objData.AppendChild(objType as XmlNode);
    //    //            bool existLayer = false;
    //    //            //�ж��Ƿ��������
    //    //            foreach (FormulaLayer formulaLayer in FormulaLayerList)
    //    //            {
    //    //                if (formulaLayer.Name.Equals(objName))
    //    //                {
    //    //                    foreach (FormulaField field in formulaLayer.Field)
    //    //                    {
    //    //                        XmlElement objField = document.CreateElement("�ֶ�");
    //    //                        objData.AppendChild(objField as XmlNode);
    //    //                        XmlElement objFieldElement = document.CreateElement("����");
    //    //                        objFieldElement.InnerText = field.Name;
    //    //                        objField.AppendChild(objFieldElement as XmlNode);
    //    //                        objFieldElement = document.CreateElement("����");
    //    //                        objFieldElement.InnerText = field.AliasName;
    //    //                        objField.AppendChild(objFieldElement as XmlNode);
    //    //                        objFieldElement = document.CreateElement("����");
    //    //                        objFieldElement.InnerText = field.Type;
    //    //                        objField.AppendChild(objFieldElement as XmlNode);
    //    //                        objFieldElement = document.CreateElement("����");
    //    //                        objFieldElement.InnerText = field.Length;
    //    //                        objField.AppendChild(objFieldElement as XmlNode);
    //    //                        objFieldElement = document.CreateElement("Ĭ��ֵ");
    //    //                        objFieldElement.InnerText = field.DefaultValue;
    //    //                        objField.AppendChild(objFieldElement as XmlNode);
    //    //                        objFieldElement = document.CreateElement("�Ƿ����");
    //    //                        objFieldElement.InnerText = field.IsNull;
    //    //                        objField.AppendChild(objFieldElement as XmlNode);
    //    //                    }
    //    //                    existLayer = true;
    //    //                    break;
    //    //                }
    //    //            }
    //    //            //�����ڵ�������Ĭ�ϴ���
    //    //            if (!existLayer)
    //    //            {
    //    //                foreach (FormulaField field in FormulaFieldList)
    //    //                {
    //    //                    XmlElement objField = document.CreateElement("�ֶ�");
    //    //                    objData.AppendChild(objField as XmlNode);
    //    //                    XmlElement objFieldElement = document.CreateElement("����");
    //    //                    objFieldElement.InnerText = field.Name;
    //    //                    objField.AppendChild(objFieldElement as XmlNode);
    //    //                    objFieldElement = document.CreateElement("����");
    //    //                    objFieldElement.InnerText = field.AliasName;
    //    //                    objField.AppendChild(objFieldElement as XmlNode);
    //    //                    objFieldElement = document.CreateElement("����");
    //    //                    objFieldElement.InnerText = field.Type;
    //    //                    objField.AppendChild(objFieldElement as XmlNode);
    //    //                    objFieldElement = document.CreateElement("����");
    //    //                    objFieldElement.InnerText = field.Length;
    //    //                    objField.AppendChild(objFieldElement as XmlNode);
    //    //                    objFieldElement = document.CreateElement("Ĭ��ֵ");
    //    //                    objFieldElement.InnerText = field.DefaultValue;
    //    //                    objField.AppendChild(objFieldElement as XmlNode);
    //    //                    objFieldElement = document.CreateElement("�Ƿ����");
    //    //                    objFieldElement.InnerText = field.IsNull;
    //    //                    objField.AppendChild(objFieldElement as XmlNode);
    //    //                }
    //    //            }
    //    //        }
    //    //        XmlElement objSpacial = document.CreateElement("�ռ�ο�");
    //    //        objSpacial.SetAttribute("·��", @"..\Prj\NJTDT.prj");//yjl20120503
    //    //        objectDataElement.AppendChild(objSpacial as XmlNode);
    //    //    }
    //    //    return document;
    //    //}

    //    /// <summary>
    //    /// �����ύ����
    //    /// </summary>
    //    /// <param name="pFeatureNames"></param>
    //    /// <param name="doc"></param>
    //    /// <returns></returns>
    //    public static XmlDocument GetUpdateXml(List<string> pFeatureNames, XmlDocument doc)
    //    {
    //        XmlDocument document = new XmlDocument();
    //        //document.LoadXml("<�ύ����><���></���></�ύ����>");
    //        document.LoadXml("<�ύ����></�ύ����>");
    //        foreach (string strName in pFeatureNames)
    //        {
    //            string strLayerName = "";
    //            if (string.IsNullOrEmpty(strName)) continue;
    //            int index = strName.IndexOf(".");
    //            if (index != -1)
    //            {
    //                strLayerName = strName.Split('.')[1];
    //            }
    //            else
    //            {
    //                strLayerName = strName;
    //            }
    //            if (string.IsNullOrEmpty(strLayerName)) continue;
    //            XmlNode node = doc.DocumentElement.SelectSingleNode(".//Դ����//����[child::text()='" + strLayerName + "']");
    //            if (node != null)
    //            {
    //                XmlNode typeNode = node.ParentNode.ParentNode.ParentNode;
    //                XmlNode ruleNode = node.ParentNode.ParentNode;
    //                if (typeNode == null || ruleNode == null) continue;
    //                string name = typeNode["����"].InnerText;

    //                XmlNode tempNode = document.DocumentElement.SelectSingleNode(".//���//����[child::text()='" + name + "']");
    //                XmlNode typeNodeUpdate = null;
    //                if (tempNode == null)
    //                {
    //                    XmlNode docruleNode = document["�ύ����"];
    //                    typeNodeUpdate = document.CreateElement("���");
    //                    docruleNode.AppendChild(typeNodeUpdate);
    //                    XmlNode insertNode = document.CreateElement("����");
    //                    insertNode.InnerText = name;
    //                    typeNodeUpdate.AppendChild(insertNode);
    //                }
    //                else
    //                {
    //                    typeNodeUpdate = tempNode.ParentNode;
    //                }

    //                XmlNode newRuleNode = document.ImportNode(ruleNode.Clone(), true);
    //                typeNodeUpdate.AppendChild(newRuleNode);
    //            }
    //        }
    //        return document;
    //    }

    //    /// <summary>
    //    /// ˢ��ͼ��
    //    /// </summary>
    //    /// <param name="xmlDocument"></param>
    //    //public static void RefreshMap(Plugin.Application.IAppGisUpdateRef _AppHk, enumTaskType currentTaskType, out Exception eError)
    //    //{
    //    //    eError = null;
    //    //    Plugin.Application.IAppFormRef _AppForm = _AppHk as Plugin.Application.IAppFormRef;
    //    //    XmlNode DBNode = _AppHk.DBXmlDocument.SelectSingleNode(".//��Ŀ����");
    //    //    if (DBNode == null) return;
    //    //    XmlElement DBElement = DBNode as XmlElement;
    //    //    string prjid = DBElement.GetAttribute("��Ŀ���");
    //    //    string userid = _AppForm.ConnUser.ID.ToString();
    //    //    string username = _AppForm.ConnUser.Name;
    //    //    string StrScale = DBElement.GetAttribute("������").ToString();
    //    //    XmlElement tElement = null;

    //    //    //���Ԥ����õķ��ŵ�ͼ�ĵ�
    //    //    IMapDocument pMapDoc = null;
    //    //    string strMxdPath = Application.StartupPath + "\\״̬���ŷ���.mxd";
    //    //    if (File.Exists(strMxdPath))
    //    //    {
    //    //        pMapDoc = new MapDocumentClass();
    //    //        if (pMapDoc.get_IsMapDocument(strMxdPath))
    //    //        {
    //    //            pMapDoc.Open(strMxdPath, "");
    //    //        }
    //    //        else
    //    //        {
    //    //            pMapDoc = null;
    //    //        }
    //    //    }

    //    //    //��������Ĳ�ͬѡ��ͬ��ͼ����Χ
    //    //    if (currentTaskType == enumTaskType.ZONE)
    //    //    {
    //    //        tElement = DBElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
    //    //    }
    //    //    else if (currentTaskType == enumTaskType.INTERZONE)
    //    //    {
    //    //        tElement = DBElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
    //    //    }
    //    //    else if (currentTaskType == enumTaskType.MANAGER)
    //    //    {
    //    //        tElement = DBElement.SelectSingleNode(".//��������//��Χ") as XmlElement;
    //    //    }
    //    //    else if (currentTaskType == enumTaskType.CHECK)
    //    //    {
    //    //        tElement = DBElement.SelectSingleNode(".//�������//��Χ") as XmlElement;
    //    //    }
    //    //    if (tElement == null) return;
    //    //    string whereClause = "";
    //    //    IQueryFilter pQueryFilter = new QueryFilterClass();
    //    //    //����ͼ��
    //    //    //��¼�ֶε�ͼ���ż�
    //    //    List<StringBuilder> sbList = null;
    //    //    StringBuilder sb = null;
    //    //    //���ͼ����������100��ֶ���������
    //    //    if (tElement.ChildNodes.Count >= 1000)
    //    //    {
    //    //        sbList = new List<StringBuilder>();
    //    //        for (int n = 0; n < tElement.ChildNodes.Count / 500 + 1; n++)
    //    //        {
    //    //            sb = new StringBuilder();
    //    //            for (int j = n * 500; j < (n + 1) * 500 && j < tElement.ChildNodes.Count; j++)
    //    //            {
    //    //                //��¼�ֶε�ͼ����
    //    //                XmlElement element = tElement.ChildNodes[j] as XmlElement;
    //    //                string mapNo = element.GetAttribute("ͼ����");
    //    //                if (!string.IsNullOrEmpty(mapNo))
    //    //                {
    //    //                    if (sb.Length != 0)
    //    //                    {
    //    //                        sb.Append(",");
    //    //                    }
    //    //                    sb.Append("'" + mapNo + "'");
    //    //                }
    //    //            }
    //    //            sbList.Add(sb);
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        sb = new StringBuilder();
    //    //        //�������ݸ��¶Ա��б���л�ȡ����
    //    //        for (int i = 0; i < tElement.ChildNodes.Count; i++)
    //    //        {
    //    //            //��¼�ֶε�ͼ����
    //    //            XmlElement element = tElement.ChildNodes[i] as XmlElement;
    //    //            string mapNo = element.GetAttribute("ͼ����");
    //    //            if (!string.IsNullOrEmpty(mapNo))
    //    //            {
    //    //                if (sb.Length != 0)
    //    //                {
    //    //                    sb.Append(",");
    //    //                }
    //    //                sb.Append("'" + mapNo + "'");
    //    //            }
    //    //        }
    //    //    }
    //    //    if (sbList != null && sbList.Count > 0)
    //    //    {
    //    //        sb = new StringBuilder();
    //    //        foreach (StringBuilder sbuilder in sbList)
    //    //        {
    //    //            if (string.IsNullOrEmpty(sb.ToString()))
    //    //            {
    //    //                sb.Append("MAP_NO IN(" + sbuilder.ToString() + ")");
    //    //            }
    //    //            else
    //    //            {
    //    //                sb.Append(" OR MAP_NO IN (" + sbuilder.ToString() + ")");
    //    //            }
    //    //        }
    //    //        if (sb.Length != 0)
    //    //        {
    //    //            if (whereClause == "")
    //    //            {
    //    //                whereClause += sb.ToString();
    //    //            }
    //    //            else
    //    //            {
    //    //                whereClause += " and " + sb.ToString();
    //    //            }
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        if (sb.Length != 0)
    //    //        {
    //    //            if (whereClause == "")
    //    //            {
    //    //                whereClause += "MAP_NO in (" + sb.ToString() + ")";
    //    //            }
    //    //            else
    //    //            {
    //    //                whereClause += " and MAP_NO in (" + sb.ToString() + ")";
    //    //            }
    //    //        }
    //    //    }
    //    //    pQueryFilter.WhereClause = whereClause;
    //    //    IQueryFilter pForeignFilter = new QueryFilterClass();
    //    //    if (currentTaskType == enumTaskType.MANAGER || currentTaskType == enumTaskType.CHECK)
    //    //    {
    //    //        pForeignFilter.WhereClause = "PRJID=" + prjid;
    //    //    }
    //    //    else if (currentTaskType == enumTaskType.ZONE || currentTaskType == enumTaskType.INTERZONE)
    //    //    {
    //    //        pForeignFilter.WhereClause = "PRJID=" + prjid + " AND EMPID='" + userid + "'";
    //    //    }
    //    //    AddMapLayer(StrScale, pQueryFilter, pForeignFilter, _AppHk.MapControl, ModData.v_SysDataSet, pMapDoc, out eError);
    //    //    //AddLabel(StrScale, _AppHk.MapControl, "MAP_NO");
    //    //    //ˢ��
    //    //    _AppHk.MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
    //    //}

    //    /// <summary>
    //    /// ˢ��ͼ��
    //    /// </summary>
    //    /// <param name="xmlDocument"></param>
    //    public static void RefreshInterZone(XmlDocument DBXmlDocument, MapControl mapControl, out Exception eError)
    //    {
    //        eError = null;
    //        XmlNode DBNode = DBXmlDocument.SelectSingleNode(".//��Ŀ����");
    //        if (DBNode == null) return;
    //        XmlElement DBElement = DBNode as XmlElement;
    //        string prjid = DBElement.GetAttribute("��Ŀ���");
    //        string StrScale = DBElement.GetAttribute("������").ToString();
    //        XmlElement tElement = null;
    //        //��������Ĳ�ͬѡ��ͬ��ͼ����Χ
    //        tElement = DBElement.SelectSingleNode("//��������//����") as XmlElement;
    //        if (tElement == null) return;

    //        //���Ԥ����õķ��ŵ�ͼ�ĵ�
    //        IMapDocument pMapDoc = null;
    //        string strMxdPath = Application.StartupPath + "\\״̬���ŷ���.mxd";
    //        if (File.Exists(strMxdPath))
    //        {
    //            pMapDoc = new MapDocumentClass();
    //            if (pMapDoc.get_IsMapDocument(strMxdPath))
    //            {
    //                pMapDoc.Open(strMxdPath, "");
    //            }
    //            else
    //            {
    //                pMapDoc = null;
    //            }
    //        }

    //        //ˢ�·�Χ�ߵķ���״̬
    //        IQueryFilter pQueryFilter = new QueryFilterClass();

    //        //���ط�Χ��
    //        string whereClause = "";
    //        foreach (XmlElement element in tElement.ChildNodes)
    //        {
    //            string lineid = element.GetAttribute("value");
    //            if (string.IsNullOrEmpty(whereClause))
    //            {
    //                whereClause = "'" + lineid + "'";
    //            }
    //            else
    //            {
    //                whereClause += ",'" + lineid + "'";
    //            }
    //        }
    //        if (!string.IsNullOrEmpty(whereClause))
    //        {
    //            whereClause = ModData.User + "." + "bound_line.LINEID IN (" + whereClause + ") AND " + ModData.User + ".INTERZONE.PRJID=" + prjid;
    //        }
    //        else
    //        {
    //            whereClause = ModData.User + ".INTERZONE.PRJID=" + prjid;
    //        }
    //        pQueryFilter.WhereClause = whereClause;
    //        ModDBOperator.AddInterZoneLayer("bound_line", pQueryFilter, mapControl, ModData.v_SysDataSet, pMapDoc, out eError);
    //        Dictionary<string, string> fieldDic = new Dictionary<string, string>();
    //        fieldDic.Add("LINENAME", "��Χ��");
    //        fieldDic.Add("EMPNAME", "��ҵ��");
    //        ModDBOperator.AddLabel("bound_line", ModData.v_AppGisUpdate.MapControl, fieldDic);
    //        mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
    //    }

    //    /// <summary>
    //    /// �ñ�����־���·������ϵ���־
    //    /// </summary>
    //    /// <param name="sysDataBase"></param>
    //    /// <param name="sysDataBase_2"></param>
    //    public static bool UpdateDataLog(Plugin.Application.IAppFormRef m_AppForm, SysCommon.DataBase.SysTable logDataBase, SysCommon.Gis.SysGisTable logGisTable, out Exception eError)
    //    {
    //        eError = null;
    //        bool result = true;
    //        try
    //        {
    //            Plugin.Application.IAppGisUpdateRef appSMPD = m_AppForm as Plugin.Application.IAppGisUpdateRef;
    //            if (appSMPD == null || appSMPD.DBXmlDocument == null) return false;
    //            XmlElement DBElement = appSMPD.DBXmlDocument.SelectSingleNode(".//��Ŀ����") as XmlElement;
    //            string prjid = DBElement.GetAttribute("��Ŀ���");
    //            string prjname = DBElement.GetAttribute("��Ŀ����");
    //            if (logDataBase == null || logGisTable == null) return false;
    //            Dictionary<string, object[]> valueList = logGisTable.GetEspRows("DATA_LOG", "prjid=" + prjid, out eError);
    //            Dictionary<string, object[]> valueListTime = logGisTable.GetEspRows("DATA_LOG_TIME", "prjid=" + prjid, out eError);
    //            if (valueList == null || valueListTime == null) return false;

    //            //��¼Ϊɾ��
    //            DataTable logDelTable = logDataBase.GetSQLTable("select * from ��־��¼��", out eError);
    //            if (logDelTable == null) return false;
    //            List<Dictionary<string, object>> insertList = new List<Dictionary<string, object>>();
    //            List<Dictionary<string, object>> insertListTime = new List<Dictionary<string, object>>();
    //            List<int> delOids = new List<int>();
    //            List<int> delOidsTime = new List<int>();
    //            Dictionary<int, string> updOids = new Dictionary<int, string>();
    //            Dictionary<int, string> updOidsTime = new Dictionary<int, string>();
    //            foreach (DataRow row in logDelTable.Rows)
    //            {
    //                Dictionary<string, object> insertDic = new Dictionary<string, object>();
    //                Dictionary<string, object> insertDicTime = new Dictionary<string, object>();
    //                StringBuilder strBuilder = new StringBuilder();
    //                string feaId = row["FEAID"].ToString();
    //                string state = row["STATE"].ToString();
    //                string updatetime = row["UPDATATIME"].ToString();

    //                if (state.Equals("2"))
    //                {
    //                    if (valueList.Keys.Count > 0)
    //                    {
    //                        bool isExist = false;
    //                        if (valueList.ContainsKey(feaId))
    //                        {
    //                            isExist = true;
    //                            object[] objDic = valueList[feaId];
    //                            object[] objDicTime = valueListTime[feaId];
    //                            if (objDic != null && objDic.Length == 2)
    //                            {
    //                                //��������Ϊ�½���Ҫ��
    //                                if (objDic[1].ToString().Equals("1"))
    //                                {
    //                                    delOids.Add(int.Parse(objDic[0].ToString()));
    //                                    delOidsTime.Add(int.Parse(objDicTime[0].ToString()));
    //                                }
    //                                //��������Ϊ�޸ĵ�Ҫ��
    //                                else if (objDic[1].ToString().Equals("3"))
    //                                {
    //                                    updOids.Add(int.Parse(objDic[0].ToString()), updatetime);
    //                                    updOidsTime.Add(int.Parse(objDicTime[0].ToString()), updatetime);
    //                                }
    //                            }
    //                        }
    //                        if (!isExist)
    //                        {
    //                            insertDic.Add("CLASS", row["CLASS"]);
    //                            insertDic.Add("FEAID", row["FEAID"]);
    //                            insertDic.Add("STATE", row["STATE"]);
    //                            insertDic.Add("TAG", row["TAG"]);
    //                            insertDic.Add("PRJID", prjid);
    //                            insertDic.Add("PRJNAME", prjname);
    //                            insertList.Add(insertDic);

    //                            insertDicTime.Add("CLASS", row["CLASS"]);
    //                            insertDicTime.Add("FEAID", row["FEAID"]);
    //                            insertDicTime.Add("STATE", row["STATE"]);
    //                            insertDicTime.Add("TAG", row["TAG"]);
    //                            insertDicTime.Add("PRJID", prjid);
    //                            insertDicTime.Add("PRJNAME", prjname);
    //                            insertDicTime.Add("UPDATATIME", updatetime);
    //                            insertListTime.Add(insertDicTime);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        insertDic.Add("CLASS", row["CLASS"]);
    //                        insertDic.Add("FEAID", row["FEAID"]);
    //                        insertDic.Add("STATE", row["STATE"]);
    //                        insertDic.Add("TAG", row["TAG"]);
    //                        insertDic.Add("PRJID", prjid);
    //                        insertDic.Add("PRJNAME", prjname);
    //                        insertList.Add(insertDic);

    //                        insertDicTime.Add("CLASS", row["CLASS"]);
    //                        insertDicTime.Add("FEAID", row["FEAID"]);
    //                        insertDicTime.Add("STATE", row["STATE"]);
    //                        insertDicTime.Add("TAG", row["TAG"]);
    //                        insertDicTime.Add("PRJID", prjid);
    //                        insertDicTime.Add("PRJNAME", prjname);
    //                        insertDicTime.Add("UPDATATIME", updatetime);
    //                        insertListTime.Add(insertDicTime);
    //                    }
    //                }
    //                else if (state.Equals("3") || state.Equals("1"))
    //                {
    //                    if (valueList.Keys.Count > 0)
    //                    {
    //                        bool isExist = false;
    //                        if (valueList.ContainsKey(feaId))
    //                        {
    //                            isExist = true;
    //                        }
    //                        if (!isExist)
    //                        {
    //                            insertDic.Add("CLASS", row["CLASS"]);
    //                            insertDic.Add("FEAID", row["FEAID"]);
    //                            insertDic.Add("STATE", row["STATE"]);
    //                            insertDic.Add("TAG", row["TAG"]);
    //                            insertDic.Add("PRJID", prjid);
    //                            insertDic.Add("PRJNAME", prjname);
    //                            insertList.Add(insertDic);

    //                            insertDicTime.Add("CLASS", row["CLASS"]);
    //                            insertDicTime.Add("FEAID", row["FEAID"]);
    //                            insertDicTime.Add("STATE", row["STATE"]);
    //                            insertDicTime.Add("TAG", row["TAG"]);
    //                            insertDicTime.Add("PRJID", prjid);
    //                            insertDicTime.Add("PRJNAME", prjname);
    //                            insertDicTime.Add("UPDATATIME", updatetime);
    //                            insertListTime.Add(insertDicTime);
    //                        }
    //                        else
    //                        {
    //                            object[] objDicTime = valueListTime[feaId];
    //                            string oid = objDicTime[0].ToString();
    //                            string updStrTime = "OBJECTID=" + oid.ToString();
    //                            Dictionary<string, object> dicValues = new Dictionary<string, object>();
    //                            dicValues.Add("UPDATATIME", updatetime);
    //                            result = logGisTable.UpdateRow("DATA_LOG_TIME", updStrTime, dicValues, out eError);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        insertDic.Add("CLASS", row["CLASS"]);
    //                        insertDic.Add("FEAID", row["FEAID"]);
    //                        insertDic.Add("STATE", row["STATE"]);
    //                        insertDic.Add("TAG", row["TAG"]);
    //                        insertDic.Add("PRJID", prjid);
    //                        insertDic.Add("PRJNAME", prjname);
    //                        insertList.Add(insertDic);

    //                        insertDicTime.Add("CLASS", row["CLASS"]);
    //                        insertDicTime.Add("FEAID", row["FEAID"]);
    //                        insertDicTime.Add("STATE", row["STATE"]);
    //                        insertDicTime.Add("TAG", row["TAG"]);
    //                        insertDicTime.Add("PRJID", prjid);
    //                        insertDicTime.Add("PRJNAME", prjname);
    //                        insertDicTime.Add("UPDATATIME", updatetime);
    //                        insertListTime.Add(insertDicTime);
    //                    }
    //                }
    //            }

    //            //���뱾��ɾ��������
    //            if (insertList.Count != 0)
    //            {
    //                foreach (Dictionary<string, object> dicValues in insertList)
    //                {
    //                    result = logGisTable.NewRow("DATA_LOG", dicValues, out eError);
    //                }

    //                foreach (Dictionary<string, object> dicValues in insertListTime)
    //                {
    //                    result = result && logGisTable.NewRow("DATA_LOG_TIME", dicValues, out eError);
    //                }
    //            }
    //            //ɾ�����������½���Ҫ��
    //            if (result)
    //            {
    //                string delStr = "";
    //                if (delOids.Count > 0)
    //                {
    //                    foreach (int id in delOids)
    //                    {
    //                        if (string.IsNullOrEmpty(delStr))
    //                        {
    //                            delStr = id.ToString();
    //                        }
    //                        else
    //                        {
    //                            delStr += "," + id.ToString();
    //                        }
    //                    }
    //                    delStr = "OBJECTID IN (" + delStr + ")";
    //                    result = logGisTable.DeleteRows("DATA_LOG", delStr, out eError);
    //                }

    //                string delStrTime = "";
    //                if (delOidsTime.Count > 0)
    //                {
    //                    foreach (int id in delOidsTime)
    //                    {
    //                        if (string.IsNullOrEmpty(delStrTime))
    //                        {
    //                            delStrTime = id.ToString();
    //                        }
    //                        else
    //                        {
    //                            delStrTime += "," + id.ToString();
    //                        }
    //                    }
    //                    delStrTime = "OBJECTID IN (" + delStrTime + ")";
    //                    result = logGisTable.DeleteRows("DATA_LOG_TIME", delStrTime, out eError);
    //                }
    //            }
    //            //�޸ķ��������޸�Ҫ�ص�״̬
    //            if (result)
    //            {
    //                string updStr = "";
    //                if (updOids.Count > 0)
    //                {
    //                    foreach (int id in updOids.Keys)
    //                    {
    //                        updStr = "OBJECTID=" + id.ToString();

    //                        Dictionary<string, object> dicValues = new Dictionary<string, object>();
    //                        dicValues.Add("STATE", 2);
    //                        result = logGisTable.UpdateRow("DATA_LOG", updStr, dicValues, out eError);
    //                    }
    //                }

    //                string updStrTime = "";
    //                if (updOidsTime.Count > 0)
    //                {
    //                    foreach (int id in updOidsTime.Keys)
    //                    {
    //                        updStrTime = "OBJECTID=" + id.ToString();

    //                        Dictionary<string, object> dicValues = new Dictionary<string, object>();
    //                        dicValues.Add("STATE", 2);
    //                        dicValues.Add("UPDATATIME", updOidsTime[id]);
    //                        result = logGisTable.UpdateRow("DATA_LOG_TIME", updStrTime, dicValues, out eError);
    //                    }
    //                }
    //            }
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            eError = ex;
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// ���ͼ��ӳ���ϵ
    //    /// </summary>
    //    /// <param name="strDBpath"></param>
    //    /// <returns></returns>
    //    //public static List<FormulaXml> GetLayersMapping(string strDBpath)
    //    //{
    //    //    List<FormulaXml> lstTempXml = new List<FormulaXml>();

    //    //    SysCommon.DataBase.SysTable vtable = new SysCommon.DataBase.SysTable();
    //    //    Exception Err;
    //    //    vtable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDBpath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out Err);

    //    //    System.Data.DataTable dt = vtable.GetSQLTable("select distinct ���ͼͼ�� from ���ݼ��_����", out Err);

    //    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    //    {
    //    //        string strStandName = dt.Rows[i]["���ͼͼ��"].ToString();

    //    //        FormulaXml vTemp = GetLyrMapping(strStandName, vtable);
    //    //        if (vTemp.StyleList.Count < 1) continue;

    //    //        lstTempXml.Add(vTemp);
    //    //    }

    //    //    return lstTempXml;
    //    //}

    //    //private static FormulaXml GetLyrMapping(string strStandName, SysCommon.DataBase.SysTable vTable)
    //    //{
    //    //    FormulaXml vMapping = new FormulaXml();
    //    //    Exception Err;

    //    //    List<string> lstTempWorkName = new List<string>();
    //    //    List<string> lstTempCodes = null;

    //    //    string strWorkName = "";
    //    //    string strCode = "";
    //    //    string strStyle = "";
    //    //    FormulaStyle vTempStyle = null;

    //    //    string strOldName = "";

    //    //    List<FormulaStyle> vTempStyles = new List<FormulaStyle>();

    //    //    System.Data.DataTable dt = vTable.GetSQLTable("select *  from ���ݼ��_���� where ���ͼͼ��='" + strStandName + "' order by ��������", out Err);
    //    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    //    {
    //    //        strWorkName = dt.Rows[i]["���ͼͼ��"].ToString();
    //    //        strCode = dt.Rows[i]["��������"].ToString();
    //    //        strStyle = dt.Rows[i]["�����Ҫ������"].ToString();

    //    //        if (i == 0) strOldName = strWorkName;

    //    //        if (lstTempCodes == null)
    //    //        {
    //    //            lstTempCodes = new List<string>();
    //    //        }

    //    //        if (lstTempWorkName.Contains(strWorkName))
    //    //        {
    //    //            if (lstTempCodes.Contains(strCode)) continue;
    //    //            lstTempCodes.Add(strCode);
    //    //            continue;
    //    //        }
    //    //        else
    //    //        {
    //    //            if (vTempStyle != null)
    //    //            {
    //    //                //���code��ļ���
    //    //                vTempStyle = new FormulaStyle();
    //    //                vTempStyle.Style = strStyle;
    //    //                vTempStyle.OrgTable.Name = strStandName;
    //    //                vTempStyle.OrgTable.AttributeName = "��������";//д����
    //    //                vTempStyle.OrgTable.AttributeValue = GetCodesByList(lstTempCodes);
    //    //                if (lstTempCodes.Count > 1)
    //    //                {
    //    //                    vTempStyle.OrgTable.Operator = "IN";
    //    //                }
    //    //                vTempStyle.OrgTable.ConditionType = "GDC_WHERE";

    //    //                vTempStyle.ObjTable.Name = strOldName;
    //    //                if (vTempStyle.ObjTable.FieldMapping.Count < 1)
    //    //                {
    //    //                    //vTempStyle.ObjTable.FieldMapping.Add("*", "*");
    //    //                }

    //    //                //���
    //    //                vTempStyles.Add(vTempStyle);
    //    //                lstTempCodes.Clear();
    //    //                strOldName = strWorkName;
    //    //            }
    //    //            else
    //    //            {
    //    //                vTempStyle = new FormulaStyle();
    //    //            }

    //    //            if (!lstTempCodes.Contains(strCode))
    //    //            {
    //    //                lstTempCodes.Add(strCode);
    //    //            }

    //    //            //
    //    //            lstTempWorkName.Add(strWorkName);
    //    //        }
    //    //    }

    //    //    if (vTempStyle != null)
    //    //    {
    //    //        //���code��ļ���
    //    //        vTempStyle = new FormulaStyle();
    //    //        vTempStyle.Style = strStyle;
    //    //        vTempStyle.OrgTable.Name = strStandName;
    //    //        //vTempStyle.OrgTable.AttributeName = "��������";//д����
    //    //        //vTempStyle.OrgTable.AttributeValue = GetCodesByList(lstTempCodes);
    //    //        //if (lstTempCodes.Count > 1)
    //    //        //{
    //    //        //    vTempStyle.OrgTable.Operator = "IN";
    //    //        //}
    //    //        //vTempStyle.OrgTable.ConditionType = "GDC_WHERE";

    //    //        vTempStyle.ObjTable.Name = strOldName;
    //    //        if (vTempStyle.ObjTable.FieldMapping.Count < 1)
    //    //        {
    //    //            //vTempStyle.ObjTable.FieldMapping.Add("*", "*");
    //    //        }

    //    //        //���
    //    //        vTempStyles.Add(vTempStyle);
    //    //        lstTempCodes.Clear();
    //    //    }

    //    //    //
    //    //    vMapping.Name = strStandName;
    //    //    vMapping.StyleList = vTempStyles;
    //    //    return vMapping;
    //    //}
    //    /// <summary>
    //    /// ���ͼ��ӳ���ϵyjl20120504 overload 
    //    /// </summary>
    //    /// <param name="strDBpath"></param>
    //    /// <returns></returns>
    //    //public static List<FormulaXml> GetLayersMapping(IWorkspace pTdtW)
    //    //{
    //    //    List<FormulaXml> lstTempXml = new List<FormulaXml>();
    //    //    IFeatureWorkspace pSrcFW = pTdtW as IFeatureWorkspace;
    //    //    Exception Err;
    //    //    #region ͨ����ȡ���ͼ��������Ҫ������Ϣ
    //    //    IFeatureDataset pFDtdt = pSrcFW.OpenFeatureDataset("w_njtdt");
    //    //    if (pFDtdt == null)
    //    //    {
    //    //        return null;
    //    //    }
    //    //    IEnumDataset pEnumDs = pFDtdt.Subsets;
    //    //    IDataset pDs = pEnumDs.Next();
    //    //    while (pDs != null)
    //    //    {
    //    //        string feaName = pDs.Name;
    //    //        feaName = feaName.Substring(feaName.LastIndexOf('.') + 1);
    //    //        FormulaXml vTemp = GetLyrMapping(feaName, pDs as IFeatureClass);
    //    //        if (vTemp.StyleList.Count < 1) continue;

    //    //        lstTempXml.Add(vTemp);
    //    //        pDs = pEnumDs.Next();
    //    //    }
    //    //    #endregion

    //    //    return lstTempXml;
    //    //}
    //    //yjl20120504 overload 
    //    //private static FormulaXml GetLyrMapping(string strStandName, IFeatureClass pTdtWFc)
    //    //{
    //    //    FormulaXml vMapping = new FormulaXml();
    //    //    Exception Err;
    //    //    string strStyle = "";
    //    //    FormulaStyle vTempStyle = null;
    //    //    List<FormulaStyle> vTempStyles = new List<FormulaStyle>();
    //    //    if (pTdtWFc.Extension != null)
    //    //    {
    //    //        strStyle = "ע��";
    //    //    }
    //    //    else
    //    //    {

    //    //        switch (pTdtWFc.ShapeType)
    //    //        {
    //    //            case esriGeometryType.esriGeometryPoint:
    //    //                strStyle = "��";
    //    //                break;
    //    //            case esriGeometryType.esriGeometryPolyline:
    //    //                strStyle = "��";
    //    //                break;
    //    //            case esriGeometryType.esriGeometryPolygon:
    //    //                strStyle = "��";
    //    //                break;
    //    //            default:
    //    //                strStyle = "δ֪";
    //    //                break;
    //    //        }
    //    //    }
    //    //    vTempStyle = new FormulaStyle();
    //    //    vTempStyle.Style = strStyle;
    //    //    vTempStyle.OrgTable.Name = strStandName;
    //    //    vTempStyle.ObjTable.Name = strStandName;
    //    //    vTempStyles.Add(vTempStyle);
    //    //    //
    //    //    vMapping.Name = strStandName;
    //    //    vMapping.StyleList = vTempStyles;
    //    //    return vMapping;
    //    //}
    //    //public static List<FormulaXml> GetOldLayersMapping(string strDBpath)
    //    //{
    //    //    List<FormulaXml> lstTempXml = new List<FormulaXml>();

    //    //    SysCommon.DataBase.SysTable vtable = new SysCommon.DataBase.SysTable();
    //    //    Exception Err;
    //    //    vtable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDBpath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out Err);

    //    //    System.Data.DataTable dt = vtable.GetSQLTable("select distinct ���ͼͼ�� from ���ݼ��_����", out Err);

    //    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    //    {
    //    //        string strStandName = dt.Rows[i]["���ͼͼ��"].ToString();

    //    //        FormulaXml vTemp = GetOldLyrMapping(strStandName, vtable);
    //    //        if (vTemp.StyleList.Count < 1) continue;

    //    //        lstTempXml.Add(vTemp);
    //    //    }

    //    //    return lstTempXml;
    //    //}

    //    ///// <summary>
    //    ///// �ӾɵĹ����⵽�µĹ�����
    //    ///// </summary>
    //    ///// <param name="strStandName"></param>
    //    ///// <param name="vTable"></param>
    //    ///// <returns></returns>
    //    //private static FormulaXml GetOldLyrMapping(string strStandName, SysCommon.DataBase.SysTable vTable)
    //    //{
    //    //    FormulaXml vMapping = new FormulaXml();
    //    //    Exception Err;

    //    //    List<string> lstTempWorkName = new List<string>();

    //    //    string strWorkName = "";
    //    //    string strStyle = "";
    //    //    FormulaStyle vTempStyle = null;


    //    //    List<FormulaStyle> vTempStyles = new List<FormulaStyle>();

    //    //    System.Data.DataTable dt = vTable.GetSQLTable("select *  from ���ݼ��_���� where ���ͼͼ��='" + strStandName + "' order by ����湤��ͼ��", out Err);
    //    //    for (int i = 0; i < 1; i++)//dt.Rows.Count
    //    //    {
    //    //        strWorkName = dt.Rows[i]["����湤��ͼ��"].ToString();
    //    //        strStyle = dt.Rows[i]["�����Ҫ������"].ToString();

    //    //        if (strWorkName == "") continue;

    //    //        string[] strSubNames = strWorkName.Split(',');

    //    //        for (int intSub = 0; intSub < strSubNames.GetLength(0); intSub++)
    //    //        {
    //    //            string strSubName = strSubNames[intSub];

    //    //            Dictionary<string, string> lstFields = GetFieldMapping(strStandName, vTable);

    //    //            vTempStyle = new FormulaStyle();
    //    //            vTempStyle.Style = strStyle;
    //    //            vTempStyle.OrgTable.Name = strSubName;

    //    //            vTempStyle.ObjTable.Name = strStandName;
    //    //            vTempStyle.ObjTable.FieldMapping = lstFields;

    //    //            vTempStyles.Add(vTempStyle);
    //    //        }
    //    //    }

    //    //    //
    //    //    vMapping.Name = strStandName;
    //    //    vMapping.StyleList = vTempStyles;
    //    //    return vMapping;
    //    //}

    //    private static Dictionary<string, string> GetFieldMapping(string strWorkName, SysCommon.DataBase.SysTable vTable)
    //    {
    //        Dictionary<string, string> strTempFields = new Dictionary<string, string>();

    //        Exception Err;
    //        System.Data.DataTable dt = vTable.GetSQLTable("select *  from ���������Ա� where ������ͼ������='" + strWorkName + "' order by ������ͼ������", out Err);
    //        for (int i = 0; i < dt.Rows.Count; i++)
    //        {
    //            string strFieldName = dt.Rows[i]["����������"].ToString();
    //            string strOldFieldName = dt.Rows[i]["������������"].ToString();

    //            if (strOldFieldName == "")
    //            {
    //                if (strFieldName.ToLower().Equals("feaid"))
    //                {
    //                    strOldFieldName = "-1";
    //                }
    //                else
    //                {
    //                    continue;
    //                }
    //            }
    //            if (!strTempFields.ContainsKey(strOldFieldName))
    //            {
    //                strTempFields.Add(strOldFieldName, strFieldName);
    //            }
    //        }

    //        return strTempFields;
    //    }

    //    private static string GetCodesByList(List<string> lstcode)
    //    {
    //        if (lstcode == null) return "";
    //        if (lstcode.Count < 1) return "";

    //        string strTemp = "";
    //        for (int i = 0; i < lstcode.Count; i++)
    //        {
    //            if (strTemp == "")
    //            {
    //                strTemp = lstcode[i];
    //            }
    //            else
    //            {
    //                strTemp = strTemp + "," + lstcode[i];
    //            }
    //        }

    //        return strTemp;
    //    }

    //    /// <summary>
    //    /// չ��ͼ���ͼ���������Ҫִ��toc��Update������
    //    /// </summary>
    //    /// <param name="pLayer"></param>
    //    /// <param name="bExpand"></param>
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

    //    /// <summary>
    //    /// ���������ĵ��е�ͼ��
    //    /// </summary>
    //    /// <param name="enumTaskType"></param>
    //    /// <param name="ProjectXml"></param>
    //    /// <param name="eError"></param>
    //    public static bool UpdateTaskXml(Plugin.Application.IAppGisUpdateRef _AppHk, enumTaskType eTaskType, ref XmlDocument ProjectXml, out Exception eError)
    //    {
    //        eError = null;
    //        //��÷�Χ��ķ�Χ
    //        ILayer pRangeFeatLay = GetMapFrameLayer("zone", _AppHk.MapControl, "ʾ��ͼ") as IFeatureLayer;
    //        if (pRangeFeatLay == null)
    //        {
    //            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��Χ��ͼ��ʧ��,����أ�");
    //            return false;
    //        }
    //        IGeometry pZoneGeometry = null;
    //        IFeatureCursor pZoneCursor = (pRangeFeatLay as IFeatureLayer).Search(null, false);
    //        if (pZoneCursor != null)
    //        {
    //            IFeature pFeature = pZoneCursor.NextFeature();
    //            while (pFeature != null)
    //            {
    //                if (pZoneGeometry == null)
    //                {
    //                    pZoneGeometry = pFeature.Shape;
    //                }
    //                else
    //                {
    //                    pZoneGeometry = (pZoneGeometry as ITopologicalOperator).Union(pFeature.Shape);
    //                }
    //                pFeature = pZoneCursor.NextFeature();
    //            }
    //        }
    //        if (pZoneGeometry == null)
    //        {
    //            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��Χ�淶Χʧ�ܣ�");
    //            return false;
    //        }
    //        string taskType = "";
    //        if (eTaskType == enumTaskType.MANAGER)
    //        {
    //            taskType = "��������";
    //        }
    //        else if (eTaskType == enumTaskType.ZONE)
    //        {
    //            taskType = "��������";
    //        }
    //        else if (eTaskType == enumTaskType.INTERZONE)
    //        {
    //            taskType = "��������";
    //        }
    //        else if (eTaskType == enumTaskType.CHECK)
    //        {
    //            taskType = "�������";
    //        }
    //        else if (eTaskType == enumTaskType.SPOTCHECK)
    //        {
    //            taskType = "�������";
    //        }
    //        //��XML�����ͼ����Χ
    //        XmlElement DestDB = (XmlElement)ProjectXml.SelectSingleNode(".//" + taskType + "//��Χ");
    //        //���ͼ����Χ
    //        DestDB.RemoveAll();
    //        List<string> mapNums = GetMapNoByGeometry(ModData.Scale, pZoneGeometry, ModData.v_SysDataSet, out eError);
    //        if (eError != null)
    //        {
    //            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
    //            return false;
    //        }
    //        if (mapNums != null)
    //        {
    //            foreach (string mapNo in mapNums)
    //            {
    //                XmlElement objElement = null;
    //                objElement = ProjectXml.CreateElement("ͼ��");
    //                objElement.SetAttribute("ͼ����", mapNo);
    //                DestDB.AppendChild(objElement);
    //            }
    //        }
    //        return true;
    //    }

    //    /// <summary>
    //    /// ����֮ǰ�������û��������ĵ�
    //    /// </summary>
    //    /// <param name="pFeature"></param>
    //    public static bool UpdateZoneXml(IFeature pFeature, string pValue, SysGisTable sysGisTable, string empId, string cnameid, string prjid, string taskType, string strTask, string strSubTask, string currentId, ref XmlDocument projectXml, ref XmlDocument ZoneXml, out Exception eError)
    //    {
    //        bool result = true;
    //        eError = null;
    //        if (!empId.Equals(cnameid))
    //        {
    //            //����ǵ�ǰ��½������ֱ�Ӹ����ص�XML��֮���ѯ�õ�XML
    //            if (empId.Equals(currentId))
    //            {
    //                ZoneXml = projectXml;
    //            }
    //            else
    //            {
    //                if (ZoneXml == null)
    //                {
    //                    ZoneXml = sysGisTable.GetFieldValue("taskmanager", "taskinfo", "userid='" + empId + "' and prjid=" + prjid, out eError) as XmlDocument;
    //                }
    //            }
    //            if (ZoneXml != null)
    //            {
    //                string strSql = "";
    //                XmlElement DestDB = (XmlElement)ZoneXml.SelectSingleNode(".//" + taskType + "//" + strTask);
    //                XmlNodeList DestNodeList = DestDB.SelectNodes(".//" + strSubTask);
    //                if (DestNodeList != null && DestNodeList.Count > 0)
    //                {
    //                    foreach (XmlNode pNode in DestNodeList)
    //                    {
    //                        string nodeValue = pNode.Attributes["value"].Value;
    //                        if (!string.IsNullOrEmpty(nodeValue))
    //                        {
    //                            if (nodeValue.Equals(pValue))
    //                            {
    //                                pNode.ParentNode.RemoveChild(pNode);
    //                            }
    //                            else
    //                            {
    //                                if (string.IsNullOrEmpty(strSql))
    //                                {
    //                                    strSql = nodeValue;
    //                                }
    //                                else
    //                                {
    //                                    strSql += "," + nodeValue;
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    return true;
    //                }
    //                //��XML�����ͼ����Χ
    //                DestDB = (XmlElement)ZoneXml.SelectSingleNode(".//" + taskType + "//��Χ");
    //                //���ͼ����Χ
    //                DestDB.RemoveAll();
    //                if (!string.IsNullOrEmpty(strSql))
    //                {
    //                    strSql = "OBJECTID IN (" + strSql + ")";
    //                    IGeometry pZonesGeometry = GetZonesGeometry(strSql, ModData.v_SysDataSet, out eError);
    //                    if (pZonesGeometry != null)
    //                    {
    //                        List<string> mapNums = GetMapNoByGeometry(ModData.Scale, pZonesGeometry, ModData.v_SysDataSet, out eError);
    //                        if (mapNums != null)
    //                        {
    //                            foreach (string mapNo in mapNums)
    //                            {
    //                                XmlElement objElement = null;
    //                                objElement = ZoneXml.CreateElement("ͼ��");
    //                                objElement.SetAttribute("ͼ����", mapNo);
    //                                DestDB.AppendChild(objElement);
    //                            }
    //                        }
    //                        else
    //                        {
    //                            return false;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        return false;
    //                    }
    //                }
    //                //������ҵ���񱣴�
    //                Dictionary<string, object> dicValues = new Dictionary<string, object>();
    //                IMemoryBlobStream pBlobStream = new MemoryBlobStreamClass();
    //                byte[] bytes = Encoding.Default.GetBytes(ZoneXml.OuterXml);
    //                pBlobStream.ImportFromMemory(ref bytes[0], (uint)bytes.GetLength(0));
    //                dicValues.Add("taskinfo", pBlobStream);
    //                result = sysGisTable.UpdateRow("taskmanager", "userid='" + empId + "' and prjid=" + prjid, dicValues, out eError);
    //            }
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// ����֮ǰ�������û��������ĵ�
    //    /// </summary>
    //    /// <param name="pFeature"></param>
    //    public static void UpdateTaskXml(string pValue, string taskType, string strTask, string strSubTask, ref XmlDocument ZoneXml, out Exception eError)
    //    {
    //        eError = null;
    //        if (ZoneXml != null)
    //        {
    //            string strSql = "";
    //            XmlElement DestDB = (XmlElement)ZoneXml.SelectSingleNode(".//" + taskType + "//" + strTask);
    //            XmlNodeList DestNodeList = DestDB.SelectNodes(".//" + strSubTask);
    //            if (DestNodeList != null && DestNodeList.Count > 0)
    //            {
    //                foreach (XmlNode pNode in DestNodeList)
    //                {
    //                    string nodeValue = pNode.Attributes["value"].Value;
    //                    if (!string.IsNullOrEmpty(nodeValue))
    //                    {
    //                        if (nodeValue.Equals(pValue))
    //                        {
    //                            pNode.ParentNode.RemoveChild(pNode);
    //                        }
    //                        else
    //                        {
    //                            if (string.IsNullOrEmpty(strSql))
    //                            {
    //                                strSql = nodeValue;
    //                            }
    //                            else
    //                            {
    //                                strSql += "," + nodeValue;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            if (taskType != "��������")
    //            {
    //                //��XML�����ͼ����Χ
    //                DestDB = (XmlElement)ZoneXml.SelectSingleNode(".//" + taskType + "//��Χ");
    //                //���ͼ����Χ
    //                DestDB.RemoveAll();
    //                if (!string.IsNullOrEmpty(strSql))
    //                {
    //                    strSql = "OBJECTID IN (" + strSql + ")";
    //                    IGeometry pZonesGeometry = GetZonesGeometry(strSql, ModData.v_SysDataSet, out eError);
    //                    if (pZonesGeometry != null)
    //                    {
    //                        List<string> mapNums = GetMapNoByGeometry(ModData.Scale, pZonesGeometry, ModData.v_SysDataSet, out eError);
    //                        if (mapNums != null)
    //                        {
    //                            foreach (string mapNo in mapNums)
    //                            {
    //                                XmlElement objElement = null;
    //                                objElement = ZoneXml.CreateElement("ͼ��");
    //                                objElement.SetAttribute("ͼ����", mapNo);
    //                                DestDB.AppendChild(objElement);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// ����֮ǰ�������û��������ĵ�
    //    /// </summary>
    //    /// <param name="pInterFeatureLayer"></param>
    //    /// <param name="p"></param>
    //    public static void updateInterZoneTaskXml(string lineId, string taskType, ref XmlDocument InterZoneXml, out Exception eError)
    //    {
    //        eError = null;
    //        if (InterZoneXml != null)
    //        {
    //            string strSql = "";
    //            List<string> zoneIds = new List<string>();
    //            XmlElement DestDB = (XmlElement)InterZoneXml.SelectSingleNode(".//" + taskType + "//����");
    //            XmlNodeList DestNodeList = DestDB.SelectNodes(".//��Χ��");
    //            if (DestNodeList != null && DestNodeList.Count > 0)
    //            {
    //                foreach (XmlNode pNode in DestNodeList)
    //                {
    //                    string nodeValue = pNode.Attributes["value"].Value;
    //                    if (!string.IsNullOrEmpty(nodeValue))
    //                    {
    //                        if (lineId.Contains(nodeValue))
    //                        {
    //                            pNode.ParentNode.RemoveChild(pNode);
    //                        }
    //                    }
    //                    string[] zIds = nodeValue.Split(',');
    //                    foreach (string zoneId in zIds)
    //                    {
    //                        if (!zoneIds.Contains(zoneId))
    //                        {
    //                            zoneIds.Add(zoneId);
    //                        }
    //                    }
    //                }
    //            }

    //            if (taskType != "��������")
    //            {
    //                //��XML�����ͼ����Χ
    //                DestDB = (XmlElement)InterZoneXml.SelectSingleNode(".//" + taskType + "//��Χ");
    //                //���ͼ����Χ
    //                DestDB.RemoveAll();
    //                strSql = "";
    //                foreach (string zoneId in zoneIds)
    //                {
    //                    if (string.IsNullOrEmpty(strSql))
    //                    {
    //                        strSql = "'" + zoneId + "'";
    //                    }
    //                    else
    //                    {
    //                        strSql += ",'" + zoneId + "'";
    //                    }
    //                }

    //                if (!string.IsNullOrEmpty(strSql))
    //                {
    //                    //��XML�����ͼ����Χ
    //                    strSql = "OBJECTID IN (" + strSql + ")";
    //                    IGeometry pZonesGeometry = GetZonesGeometry(strSql, ModData.v_SysDataSet, out eError);
    //                    if (pZonesGeometry != null)
    //                    {
    //                        List<string> mapNums = GetMapNoByGeometry(ModData.Scale, pZonesGeometry, ModData.v_SysDataSet, out eError);
    //                        if (mapNums != null)
    //                        {
    //                            foreach (string mapNo in mapNums)
    //                            {
    //                                XmlElement objElement = InterZoneXml.CreateElement("ͼ��");
    //                                objElement.SetAttribute("ͼ����", mapNo);
    //                                DestDB.AppendChild(objElement);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// �����Ѿ�����������ĵ�
    //    /// </summary>
    //    /// <param name="pInterFeatureLayer"></param>
    //    /// <param name="p"></param>
    //    public static bool updateInterZoneXml(IFeatureLayer pInterFeatureLayer, string lineId, SysGisTable sysGisTable, string cnameid, string prjid, string currentId, ref XmlDocument projectXml, out Exception eError)
    //    {
    //        eError = null;
    //        bool result = true;
    //        //�������ҳ���Ӧ�ķ�Χ��
    //        string strSql = ModData.User + "." + "bound_line.LINEID IN (" + lineId + ")";//ModData.User
    //        IQueryFilter pQueryFilter = new QueryFilterClass();
    //        pQueryFilter.WhereClause = strSql;
    //        IFeatureCursor pFeatureCursor = pInterFeatureLayer.FeatureClass.Search(pQueryFilter, true);
    //        if (pFeatureCursor != null)
    //        {
    //            IFeature pFeature = pFeatureCursor.NextFeature();
    //            if (pFeature != null)
    //            {
    //                int empindex = pInterFeatureLayer.FeatureClass.Fields.FindField(ModData.User+".INTERZONE.EMPID");
    //                if (empindex != -1)
    //                {
    //                    string empid = pFeature.get_Value(empindex).ToString();
    //                    if (!empid.Equals(cnameid))
    //                    {
    //                        XmlDocument InterZoneXml = null;
    //                        if (empid.Equals(currentId))
    //                        {
    //                            InterZoneXml = projectXml;
    //                        }
    //                        else
    //                        {
    //                            InterZoneXml = sysGisTable.GetFieldValue("taskmanager", "taskinfo", "userid='" + empid + "' and prjid=" + prjid, out eError) as XmlDocument;
    //                        }
    //                        if (InterZoneXml != null)
    //                        {
    //                            strSql = "";
    //                            List<string> zoneIds = new List<string>();
    //                            XmlElement DestDB = (XmlElement)InterZoneXml.SelectSingleNode(".//��������//����");
    //                            XmlNodeList DestNodeList = DestDB.SelectNodes(".//��Χ��");
    //                            if (DestNodeList != null && DestNodeList.Count > 0)
    //                            {
    //                                foreach (XmlNode pNode in DestNodeList)
    //                                {
    //                                    string nodeValue = pNode.Attributes["value"].Value;
    //                                    if (!string.IsNullOrEmpty(nodeValue))
    //                                    {
    //                                        if (lineId.Contains(nodeValue))
    //                                        {
    //                                            pNode.ParentNode.RemoveChild(pNode);
    //                                        }
    //                                    }
    //                                    string[] zIds = nodeValue.Split(',');
    //                                    foreach (string zoneId in zIds)
    //                                    {
    //                                        if (!zoneIds.Contains(zoneId))
    //                                        {
    //                                            zoneIds.Add(zoneId);
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                            else
    //                            {
    //                                return true;
    //                            }
    //                            //��XML�����ͼ����Χ
    //                            DestDB = (XmlElement)InterZoneXml.SelectSingleNode(".//��������//��Χ");
    //                            //���ͼ����Χ
    //                            DestDB.RemoveAll();
    //                            strSql = "";
    //                            foreach (string zoneId in zoneIds)
    //                            {
    //                                if (string.IsNullOrEmpty(strSql))
    //                                {
    //                                    strSql = "'" + zoneId + "'";
    //                                }
    //                                else
    //                                {
    //                                    strSql += ",'" + zoneId + "'";
    //                                }
    //                            }

    //                            if (!string.IsNullOrEmpty(strSql))
    //                            {
    //                                //��XML�����ͼ����Χ
    //                                strSql = "OBJECTID IN (" + strSql + ")";
    //                                IGeometry pZonesGeometry = GetZonesGeometry(strSql, ModData.v_SysDataSet, out eError);
    //                                if (pZonesGeometry != null)
    //                                {
    //                                    List<string> mapNums = GetMapNoByGeometry(ModData.Scale, pZonesGeometry, ModData.v_SysDataSet, out eError);
    //                                    if (mapNums != null)
    //                                    {
    //                                        foreach (string mapNo in mapNums)
    //                                        {
    //                                            XmlElement objElement = InterZoneXml.CreateElement("ͼ��");
    //                                            objElement.SetAttribute("ͼ����", mapNo);
    //                                            DestDB.AppendChild(objElement);
    //                                        }
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    return false;
    //                                }
    //                            }
    //                            Dictionary<string, object> dicValues = new Dictionary<string, object>();
    //                            IMemoryBlobStream pBlobStream = new MemoryBlobStreamClass();
    //                            byte[] bytes = Encoding.Default.GetBytes(InterZoneXml.OuterXml);
    //                            pBlobStream.ImportFromMemory(ref bytes[0], (uint)bytes.GetLength(0));
    //                            dicValues.Add("taskinfo", pBlobStream);
    //                            result = sysGisTable.UpdateRow("taskmanager", "userid='" + empid + "' and prjid=" + prjid, dicValues, out eError);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// ������ϵ��(��Դ�����������ֶ�)�Խ��������޸�״̬����Ⱦ
    //    /// </summary>
    //    /// <param name="pOrgFeatCls"></param>
    //    /// <param name="strMDB"></param>
    //    /// <returns></returns>
    //    public static IRelationshipClass GetRelationshipClass(IFeatureClass pOrgFeatCls, string strMDB)
    //    {
    //        if (pOrgFeatCls == null || !File.Exists(strMDB)) return null;
    //        Exception exError = null;
    //        //��ȡ��־������
    //        SysCommon.Gis.SysGisTable pSysGisTable = new SysCommon.Gis.SysGisTable();
    //        if (pSysGisTable.SetWorkspace(strMDB, SysCommon.enumWSType.PDB, out exError) == false)
    //        {
    //            return null;
    //        }

    //        //��ȡ��־��¼��
    //        ITable pUpdateTable = pSysGisTable.OpenTable("��־��¼��", out exError);
    //        if (pUpdateTable == null) return null;

    //        //�������ݺ���־��¼��Ĺ����༰��
    //        IRelationshipClass pJionRelationshipClass = SysCommon.Gis.ModGisPub.GetRelationShipClass("JionFc", (IObjectClass)pOrgFeatCls, "FEAID", (IObjectClass)pUpdateTable, "FEAID", "", "", esriRelCardinality.esriRelCardinalityOneToOne, out exError);
    //        pSysGisTable.CloseWorkspace();
    //        return pJionRelationshipClass;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="geoPath"></param>
    //    /// <returns></returns>
    //    public static List<FormulaLayer> GetWorkDBfields(string geoPath)
    //    {
    //        string strSettingPath = geoPath;

    //        List<FormulaLayer> lstTempFields = new List<FormulaLayer>();

    //        SysCommon.DataBase.SysTable vtable = new SysCommon.DataBase.SysTable();
    //        Exception Err;
    //        vtable.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strSettingPath, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out Err);

    //        System.Data.DataTable dt = vtable.GetSQLTable("select * from ���������Ա� order by ������ͼ������", out Err);

    //        string strTemp = "";
    //        FormulaLayer lyaerformu = null;
    //        List<FormulaField> fields = null;
    //        FormulaField newfield = null;

    //        for (int i = 0; i < dt.Rows.Count; i++)
    //        {
    //            string strStandName = dt.Rows[i]["������ͼ������"].ToString();
    //            if (strStandName == "") continue;

    //            //�ֶ�����
    //            string strFieldName = dt.Rows[i]["����������"].ToString();
    //            string strFieldAl = dt.Rows[i]["˵��"].ToString();
    //            string strFieldType = dt.Rows[i]["��������"].ToString();
    //            if (strFieldType.ToLower() == "long")
    //            {
    //                strFieldType = esriFieldType.esriFieldTypeInteger.ToString();
    //            }
    //            else if (strFieldType.ToLower() == "float")
    //            {
    //                strFieldType = esriFieldType.esriFieldTypeDouble.ToString();
    //            }
    //            else
    //            {
    //                strFieldType = esriFieldType.esriFieldTypeString.ToString();
    //            }
    //            bool blnFieldIsNull = (bool)dt.Rows[i]["�Ƿ��������"];
    //            int intLength = 0;
    //            int.TryParse(dt.Rows[i]["���"].ToString(), out intLength);

    //            //
    //            if (strTemp != strStandName)
    //            {
    //                if (lyaerformu != null)
    //                {
    //                    //newfield = new FormulaField();
    //                    //newfield.AliasName = strFieldAl;
    //                    //newfield.Name = strFieldName;
    //                    //newfield.Type = strFieldType;
    //                    //newfield.IsNull = blnFieldIsNull.ToString();
    //                    //newfield.Length = intLength.ToString();
    //                    //fields.Add(newfield);
    //                    lyaerformu.Name = strTemp;
    //                    lyaerformu.Field = fields;
    //                    lstTempFields.Add(lyaerformu);
    //                }

    //                //��ʼ��
    //                lyaerformu = new FormulaLayer();
    //                fields = new List<FormulaField>();
    //            }
    //            strTemp = strStandName;

    //            //����ֶ�
    //            newfield = new FormulaField();
    //            newfield.AliasName = strFieldAl;
    //            newfield.Name = strFieldName;
    //            newfield.Type = strFieldType;
    //            newfield.IsNull = blnFieldIsNull.ToString();
    //            newfield.Length = intLength.ToString();
    //            fields.Add(newfield);

    //            if (i == dt.Rows.Count - 1)
    //            {
    //                lyaerformu.Name = strTemp;
    //                lyaerformu.Field = fields;
    //                lstTempFields.Add(lyaerformu);
    //            }
    //        }

    //        return lstTempFields;
    //    }

    //    /// <summary>
    //    /// ��˸Ҫ��
    //    /// </summary>
    //    /// <param name="pGeometry"></param>
    //    /// <param name="pScreenDisplay"></param>
    //    /// <param name="interval"></param>
    //    public static void FlashFeature(IGeometry pGeometry, IScreenDisplay pScreenDisplay, int nFlash, int interval)
    //    {
    //        pScreenDisplay.StartDrawing(pScreenDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
    //        if (pGeometry == null) return;
    //        switch (pGeometry.GeometryType)
    //        {
    //            case esriGeometryType.esriGeometryPolyline:
    //            case esriGeometryType.esriGeometryLine:
    //                FlashLine(pScreenDisplay, pGeometry, nFlash, interval);
    //                break;
    //            case esriGeometryType.esriGeometryPolygon:
    //                FlashPolygon(pScreenDisplay, pGeometry, nFlash, interval);
    //                break;
    //            case esriGeometryType.esriGeometryPoint:
    //                FlashPoint(pScreenDisplay, pGeometry, nFlash, interval);
    //                break;
    //            default:
    //                break;
    //        }
    //        pScreenDisplay.FinishDrawing();
    //    }

    //    /// <summary>
    //    /// ��˸��
    //    /// </summary>
    //    /// <param name="pDisplay"></param>
    //    /// <param name="pGeometry"></param>
    //    /// <param name="interval"></param>
    //    private static void FlashPoint(IScreenDisplay pDisplay, IGeometry pGeometry, int nFlash, int interval)
    //    {
    //        ISimpleMarkerSymbol pMarkerSymbol;
    //        ISymbol pSymbol;
    //        IRgbColor pRGBColor;

    //        pMarkerSymbol = new SimpleMarkerSymbolClass();
    //        pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;

    //        pRGBColor = new RgbColorClass();
    //        pRGBColor.Green = 148;
    //        pRGBColor.Red = 32;
    //        pRGBColor.Blue = 0;

    //        pSymbol = pMarkerSymbol as ISymbol;
    //        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
    //        pDisplay.SetSymbol(pSymbol);
    //        for (int i = 0; i < nFlash; i++)
    //        {
    //            pDisplay.DrawPoint(pGeometry);
    //            System.Threading.Thread.Sleep(interval);
    //        }
    //    }

    //    /// <summary>
    //    /// ��˸��
    //    /// </summary>
    //    /// <param name="pDisplay"></param>
    //    /// <param name="pGeometry"></param>
    //    /// <param name="interval"></param>
    //    private static void FlashLine(IScreenDisplay pDisplay, IGeometry pGeometry, int nFlash, int interval)
    //    {
    //        ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
    //        ISymbol pSymbol;
    //        IRgbColor pRGBColor;

    //        tagPOINT tagPOINT = new tagPOINT();
    //        WKSPoint WKSPoint = new WKSPoint();

    //        tagPOINT.x = (int)8;
    //        tagPOINT.y = (int)8;
    //        pDisplay.DisplayTransformation.TransformCoords(ref WKSPoint, ref tagPOINT, 1, 6);

    //        pLineSymbol = new SimpleLineSymbolClass();
    //        pLineSymbol.Width = WKSPoint.X;

    //        pRGBColor = new RgbColorClass();
    //        pRGBColor.Green = 124;
    //        pRGBColor.Red = 252;
    //        pRGBColor.Blue = 0;

    //        pSymbol = pLineSymbol as ISymbol;
    //        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

    //        pDisplay.SetSymbol(pSymbol);

    //        for (int i = 0; i < nFlash; i++)
    //        {
    //            pDisplay.DrawPolyline(pGeometry);
    //            System.Threading.Thread.Sleep(interval);
    //        }
    //    }

    //    /// <summary>
    //    /// ��˸�����
    //    /// </summary>
    //    /// <param name="pDisplay"></param>
    //    /// <param name="pGeometry"></param>
    //    /// <param name="interval"></param>
    //    private static void FlashPolygon(IScreenDisplay pDisplay, IGeometry pGeometry, int nFlash, int interval)
    //    {
    //        ISimpleFillSymbol pFillSymbol;
    //        ISymbol pSymbol;
    //        IRgbColor pRGBColor;

    //        pFillSymbol = new SimpleFillSymbolClass();
    //        //pFillSymbol.Outline = null;

    //        pRGBColor = new RgbColorClass();
    //        pRGBColor.Green = 148;
    //        pRGBColor.Red = 32;
    //        pRGBColor.Blue = 0;

    //        pSymbol = pFillSymbol as ISymbol;
    //        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

    //        pDisplay.SetSymbol(pSymbol);
    //        for (int i = 0; i < nFlash; i++)
    //        {
    //            pDisplay.DrawPolygon(pGeometry);
    //            System.Threading.Thread.Sleep(interval);
    //        }
    //    }

    //    /// <summary>   
    //    /// �����߸���Χ������ǰ��Χ��ͼ��   
    //    /// </summary>   
    //    /// <param name="pFeatureLayer">Դ����ͼ��</param>   
    //    /// <param name="pGeometry">��������Χ</param>   
    //    /// <param name="bXZQ">ͼ���Ƿ�Ϊ������</param>   
    //    /// <returns>�´�����ͼ��</returns>   
    //    public static IFeatureLayer GetSelectionLayer(IFeatureLayer pFeatureLayer, IGeometry pGeometry)
    //    {
    //        try
    //        {
    //            if (pFeatureLayer != null && pGeometry != null)
    //            {
    //                IQueryFilter pQueryFilter;
    //                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
    //                IFeatureSelection pFeatureSelection = pFeatureLayer as IFeatureSelection;
    //                pSpatialFilter.GeometryField = pFeatureLayer.FeatureClass.ShapeFieldName;
    //                pFeatureSelection.Clear();

    //                pSpatialFilter.Geometry = pGeometry;
    //                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
    //                pQueryFilter = pSpatialFilter;
    //                pFeatureSelection.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

    //                IFeatureLayerDefinition pFLDefinition = pFeatureLayer as IFeatureLayerDefinition;
    //                IFeatureLayer pNewFeatureLayer = pFLDefinition.CreateSelectionLayer(pFeatureLayer.Name, true, null, null);
    //                pNewFeatureLayer.MaximumScale = pFeatureLayer.MaximumScale;
    //                pNewFeatureLayer.MinimumScale = pFeatureLayer.MinimumScale;
    //                pNewFeatureLayer.Selectable = pFeatureLayer.Selectable;
    //                pNewFeatureLayer.Visible = pFeatureLayer.Visible;
    //                pNewFeatureLayer.ScaleSymbols = pFeatureLayer.ScaleSymbols;
    //                return pNewFeatureLayer;
    //            }
    //            else
    //            {
    //                return null;
    //            }
    //        }
    //        catch (Exception Err)
    //        {
    //            SysCommon.Error.ErrorHandle.ShowFrmError("��ʾ", "��ȡSelectionLayerʧ�ܣ�" + Err.Message);
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// ������Ҫ�ع�������������
    //    /// </summary>
    //    /// <param name="pFea"></param>
    //    /// <param name="classname"></param>
    //    /// <param name="userid"></param>
    //    /// <param name="prjid"></param>
    //    /// <returns></returns>
    //    public static bool WriteZoneFeaManager(IFeature pFea, string classname, string userid, string prjid)
    //    {
    //        Exception eError;
    //        bool result = true;
    //        SysCommon.Gis.SysGisTable sysGisTable = null;
    //        try
    //        {
    //            if (ModData.v_SysDataSet == null || ModData.v_SysDataSet.WorkSpace == null) return false;
    //            sysGisTable = new SysCommon.Gis.SysGisTable(ModData.v_SysDataSet.WorkSpace);
    //            Dictionary<string, object> dicValues = new Dictionary<string, object>();
    //            int feaIndex = pFea.Fields.FindField("FEAID");
    //            if (feaIndex == -1) return false;
    //            string feaid = pFea.get_Value(feaIndex).ToString();
    //            dicValues.Add("FEAID", feaid);
    //            dicValues.Add("CLASSNAME", classname);
    //            dicValues.Add("USERID", userid);
    //            dicValues.Add("PRJID", prjid);
    //            sysGisTable.StartWorkspaceEdit(false, out eError);
    //            if (!sysGisTable.ExistData("ZONEFEATURES", "FEAID='" + feaid + "'" + "AND USERID=" + userid + " AND PRJID=" + prjid))
    //            {
    //                result = sysGisTable.NewRow("ZONEFEATURES", dicValues, out eError);
    //            }
    //            sysGisTable.EndWorkspaceEdit(result, out eError);
    //        }
    //        catch
    //        {
    //            result = false;
    //            sysGisTable.EndWorkspaceEdit(result, out eError);
    //        }
    //        return result;
    //    }

    //    /// <summary>
    //    /// ��ȡһ�����Ա��DataTable
    //    /// </summary>
    //    /// <param name="projectTable"></param>
    //    /// <param name="strSql"></param>
    //    /// <returns></returns>
        //public static DataTable GetTable(ITable projectTable, string strSql, List<string> fieldList, enumMetaType type)
        //{
        //    DataTable pDataTable = new DataTable();
        //    DataColumn pDataColumn = null;
        //    for (int i = 0; i < projectTable.Fields.FieldCount; i++)
        //    {
        //        //���趨���ֶι���
        //        IField pField = projectTable.Fields.get_Field(i);
        //        if (fieldList.Contains(pField.Name))
        //        {
        //            continue;
        //        }
        //        pDataColumn = new DataColumn(pField.Name);
        //        pDataTable.Columns.Add(pDataColumn);
        //        pDataColumn = null;
        //    }
        //    ICursor pCursor = null;
        //    IRow pRow = null;
        //    IQueryFilter pQueryFilter = new QueryFilterClass();
        //    pQueryFilter.WhereClause = strSql;
        //    pCursor = projectTable.Search(pQueryFilter, false);
        //    pRow = pCursor.NextRow();
        //    DataRow pDataRow = null;
        //    while (pRow != null)
        //    {
        //        pDataRow = pDataTable.NewRow();
        //        for (int j = 0; j < projectTable.Fields.FieldCount; j++)
        //        {
        //            //���趨���ֶι���
        //            IField pField = projectTable.Fields.get_Field(j);
        //            if (fieldList.Contains(pField.Name))
        //            {
        //                continue;
        //            }
        //            if (pDataRow[pField.Name] == null) continue;
        //            if (pField.Type == esriFieldType.esriFieldTypeBlob || pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeOID)
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                //��״ֵ̬ת����������ʾ
        //                object obj = pRow.get_Value(j);
        //                if (pField.Name.ToLower().Contains("state"))
        //                {
        //                    if (obj != null)
        //                    {
        //                        int intObj = StrIsInt(obj.ToString()) ? int.Parse(obj.ToString()) : 0;
        //                        if (intObj != 0)
        //                        {
        //                            switch (type)
        //                            {
        //                                case enumMetaType.ProjectInfo:
        //                                    obj = ((enumProjectState)intObj).ToString();
        //                                    break;
        //                                case enumMetaType.Zone:
        //                                    obj = ((enumZoneState)intObj).ToString();
        //                                    break;
        //                                case enumMetaType.InterZone:
        //                                    obj = ((enumInterZoneState)intObj).ToString();
        //                                    break;
        //                                case enumMetaType.Map:
        //                                    obj = ((enumMapState)intObj).ToString();
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        obj = "��״̬";
        //                    }
        //                }
        //                if (pField.Type == esriFieldType.esriFieldTypeDouble)
        //                {
        //                    string dub = "";
        //                    if (obj != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(obj.ToString()))
        //                        {
        //                            dub = double.Parse(obj.ToString()).ToString("0.000");
        //                        }
        //                    }
        //                    pDataRow[pField.Name] = dub;
        //                }
        //                else
        //                {
        //                    pDataRow[pField.Name] = obj;
        //                }
        //            }
        //        }
        //        pDataTable.Rows.Add(pDataRow);
        //        pRow = pCursor.NextRow();
        //    }
        //    return pDataTable;
        //}
    //    //yjl20120520 add for history 
        public static DataTable GetTable(ITable projectTable, string strSql, List<string> fieldList, enumMetaType type, bool bHis)
        {
            DataTable pDataTable = new DataTable();
            DataColumn pDataColumn = null;
            for (int i = 0; i < projectTable.Fields.FieldCount; i++)
            {
                //���趨���ֶι���
                IField pField = projectTable.Fields.get_Field(i);
                if (fieldList.Contains(pField.Name))
                {
                    continue;
                }
                pDataColumn = new DataColumn(pField.Name);
                pDataTable.Columns.Add(pDataColumn);
                pDataColumn = null;
            }
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = strSql;
            pCursor = projectTable.Search(pQueryFilter, false);
            pRow = pCursor.NextRow();
            DataRow pDataRow = null;
            while (pRow != null)
            {
                pDataRow = pDataTable.NewRow();
                for (int j = 0; j < projectTable.Fields.FieldCount; j++)
                {
                    //���趨���ֶι���
                    IField pField = projectTable.Fields.get_Field(j);
                    if (fieldList.Contains(pField.Name))
                    {
                        continue;
                    }
                    if (pDataRow[pField.Name] == null) continue;
                    if (pField.Type == esriFieldType.esriFieldTypeBlob || pField.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }
                    else
                    {
                        //��״ֵ̬ת����������ʾ
                        object obj = pRow.get_Value(j);
                        if (pField.Name.ToLower().Contains("state"))
                        {
                            if (obj != null)
                            {
                                int intObj = StrIsInt(obj.ToString()) ? int.Parse(obj.ToString()) : 0;
                                if (intObj != 0)
                                {
                                    switch (type)
                                    {
                                        case enumMetaType.ProjectInfo:
                                            obj = ((enumProjectState)intObj).ToString();
                                            break;
                                        case enumMetaType.Zone:
                                            obj = ((enumZoneState)intObj).ToString();
                                            break;
                                        case enumMetaType.InterZone:
                                            obj = ((enumInterZoneState)intObj).ToString();
                                            break;
                                        case enumMetaType.Map:
                                            obj = ((enumMapState)intObj).ToString();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                obj = "��״̬";
                            }
                        }
                        if (pField.Type == esriFieldType.esriFieldTypeDouble)
                        {
                            string dub = "";
                            if (obj != null)
                            {
                                if (!string.IsNullOrEmpty(obj.ToString()))
                                {
                                    dub = double.Parse(obj.ToString()).ToString("0.000");
                                }
                            }
                            pDataRow[pField.Name] = dub;
                        }
                        else
                        {
                            pDataRow[pField.Name] = obj;
                        }
                    }
                }
                pDataTable.Rows.Add(pDataRow);
                pRow = pCursor.NextRow();
            }
            return pDataTable;
        }

    //    /// <summary>
    //    /// ��ȡҪ�ر��DataTable
    //    /// </summary>
    //    /// <param name="pInterZoneFeatureJoinClass"></param>
    //    /// <returns></returns>
    //    public static DataTable GetTable(IFeatureClass pFeatureClass, string strSql, List<string> fieldList, enumMetaType type)
    //    {
    //        DataTable pDataTable = new DataTable();
    //        DataColumn pDataColumn = null;
    //        for (int i = 0; i < pFeatureClass.Fields.FieldCount; i++)
    //        {
    //            //���趨���ֶι���
    //            IField pField = pFeatureClass.Fields.get_Field(i);
    //            if (fieldList.Contains(pField.Name))
    //            {
    //                continue;
    //            }
    //            pDataColumn = new DataColumn(pField.Name);
    //            pDataTable.Columns.Add(pDataColumn);
    //            pDataColumn = null;
    //        }
    //        IFeatureCursor pFeatureCursor = null;
    //        IFeature pFeature = null;
    //        IQueryFilter pQueryFilter = new QueryFilterClass();
    //        pQueryFilter.WhereClause = strSql;
    //        pFeatureCursor = pFeatureClass.Search(pQueryFilter, false);
    //        pFeature = pFeatureCursor.NextFeature();
    //        DataRow pDataRow = null;
    //        while (pFeature != null)
    //        {
    //            pDataRow = pDataTable.NewRow();
    //            for (int j = 0; j < pFeatureClass.Fields.FieldCount; j++)
    //            {
    //                //���趨���ֶι���
    //                IField pField = pFeatureClass.Fields.get_Field(j);
    //                if (fieldList.Contains(pField.Name))
    //                {
    //                    continue;
    //                }
    //                if (pDataRow[pField.Name] == null) continue;
    //                if (pField.Type == esriFieldType.esriFieldTypeBlob || pField.Type == esriFieldType.esriFieldTypeGeometry)
    //                {
    //                    continue;
    //                }
    //                else
    //                {
    //                    //��״ֵ̬ת����������ʾ
    //                    object obj = pFeature.get_Value(j);
    //                    if (pField.Name.ToLower().Contains("state"))
    //                    {
    //                        if (obj != null)
    //                        {
    //                            int intObj = StrIsInt(obj.ToString()) ? int.Parse(obj.ToString()) : 0;
    //                            if (intObj != 0)
    //                            {
    //                                switch (type)
    //                                {
    //                                    case enumMetaType.ProjectInfo:
    //                                        obj = ((enumProjectState)intObj).ToString();
    //                                        break;
    //                                    case enumMetaType.Zone:
    //                                        obj = ((enumZoneState)intObj).ToString();
    //                                        break;
    //                                    case enumMetaType.InterZone:
    //                                        obj = ((enumInterZoneState)intObj).ToString();
    //                                        break;
    //                                    case enumMetaType.Map:
    //                                        obj = ((enumMapState)intObj).ToString();
    //                                        break;
    //                                    default:
    //                                        break;
    //                                }
    //                            }
    //                        }
    //                        else
    //                        {
    //                            obj = "��״̬";
    //                        }
    //                    }
    //                    if (pField.Type == esriFieldType.esriFieldTypeDouble)
    //                    {
    //                        string dub = "";
    //                        if (obj != null)
    //                        {
    //                            if (!string.IsNullOrEmpty(obj.ToString()))
    //                            {
    //                                dub = double.Parse(obj.ToString()).ToString("0.000");
    //                            }
    //                        }
    //                        pDataRow[pField.Name] = dub;
    //                    }
    //                    else
    //                    {
    //                        pDataRow[pField.Name] = obj;
    //                    }
    //                }
    //            }
    //            pDataTable.Rows.Add(pDataRow);
    //            pFeature = pFeatureCursor.NextFeature();
    //        }
    //        return pDataTable;
    //    }

    //    /// <summary>
    //    /// �ж��ַ����Ƿ�Ϊ����
    //    /// </summary>
    //    /// <param name="Str"></param>
    //    /// <returns></returns>
        public static bool StrIsInt(string Str)
        {
            bool flag = true;
            if (Str != "")
            {
                for (int i = 0; i < Str.Length; i++)
                {
                    if (!Char.IsNumber(Str, i))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            else
            {
                flag = false;
            }
            return flag;
        }

    //    /// <summary>
    //    /// �ж��ַ����Ƿ�Ϊdouble
    //    /// </summary>
    //    /// <param name="Str"></param>
    //    /// <returns></returns>
    //    public static bool StrIsDouble(string Str)
    //    {
    //        try
    //        {
    //            double db = double.Parse(Str);
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //}

    ///// <summary>
    ///// ���Ż�
    ///// </summary>
    //public class SymbolLyr
    //{
    //    #region ���Ż�ͼ��
    //    XmlDocument m_vDoc = null;
    //    public void SymbolFeatrueLayer(IFeatureLayer pFealyr)
    //    {
    //        if (!(pFealyr is IGeoFeatureLayer)) return;

    //        try
    //        {
    //            string strXMLpath = System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml";

    //            string strLyrName = pFealyr.Name;
    //            if (pFealyr.FeatureClass != null)
    //            {
    //                IDataset pDataset = pFealyr.FeatureClass as IDataset;
    //                strLyrName = pDataset.Name;
    //            }

    //            strLyrName = strLyrName.Substring(strLyrName.IndexOf('.') + 1);

    //            if (m_vDoc == null)
    //            {
    //                IFeatureWorkspace pFeaWks = ModData.v_SysDataSet.WorkSpace as IFeatureWorkspace;
    //                ITable pTable = pFeaWks.OpenTable("SYMBOLINFO");
    //                IQueryFilter pQueryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
    //                pQueryFilter.WhereClause = "SYMBOLNAME='ALLSYMBOL'";

    //                ICursor pCursor = pTable.Search(pQueryFilter, false);
    //                IRow pRow = pCursor.NextRow();
    //                if (pRow == null) return;

    //                IMemoryBlobStreamVariant var = pRow.get_Value(pRow.Fields.FindField("SYMBOL")) as IMemoryBlobStreamVariant;
    //                object tempObj = null;
    //                if (var == null) return;

    //                var.ExportToVariant(out tempObj);
    //                XmlDocument doc = new XmlDocument();
    //                byte[] btyes = (byte[])tempObj;
    //                string xml = Encoding.Default.GetString(btyes);
    //                doc.LoadXml(xml);

    //                DateTime updateTime = (DateTime)pRow.get_Value(pRow.Fields.FindField("UPDATETIME"));
    //                DateTime Nowtime;

    //                bool blnUpdate = false;

    //                //��һ����־����
    //                string strTimeLog = System.Windows.Forms.Application.StartupPath + "\\..\\Template\\UpdateTime.txt";
    //                if (System.IO.File.Exists(strTimeLog))
    //                {
    //                    StreamReader sr = new StreamReader(strTimeLog);
    //                    string strTime = sr.ReadLine();
    //                    sr.Close();
    //                    if (!DateTime.TryParse(strTime, out Nowtime))
    //                    {
    //                        blnUpdate = true;
    //                    }

    //                    if (updateTime > Nowtime)
    //                    {
    //                        if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�������·�����Ϣ���Ƿ���Ҫ���أ�"))
    //                        {
    //                            blnUpdate = true;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    blnUpdate = true;
    //                }

    //                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
    //                pCursor = null;

    //                //�����ж��Ƿ���Ҫ���ط�����Ϣ
    //                if (System.IO.File.Exists(strXMLpath))
    //                {
    //                    if (blnUpdate)
    //                    {
    //                        doc.Save(strXMLpath);
    //                        StreamWriter sw1 = new StreamWriter(strTimeLog);
    //                        sw1.Write(updateTime.ToString());
    //                        sw1.Close();
    //                    }
    //                }
    //                else
    //                {
    //                    doc.Save(strXMLpath);
    //                    StreamWriter sw = new StreamWriter(strTimeLog);
    //                    sw.Write(updateTime.ToString());
    //                    sw.Close();
    //                }

    //                m_vDoc = new XmlDocument();
    //                m_vDoc.Load(strXMLpath);
    //            }
    //            else
    //            {
    //            }

    //            XmlElement pElement = m_vDoc.SelectSingleNode("//" + strLyrName) as XmlElement;
    //            if (pElement == null) return;

    //            IFeatureRenderer pFeaRender = SysCommon.XML.XMLClass.XmlDeSerializer2(pElement.FirstChild.Value) as IFeatureRenderer;
    //            IGeoFeatureLayer pGeoLyr = pFealyr as IGeoFeatureLayer;
    //            pGeoLyr.Renderer = pFeaRender;
    //        }
    //        catch
    //        {

    //        }
    //    }
    //    #endregion
        public enum enumMetaType
        {
            ProjectInfo,
            Zone,
            InterZone,
            Map,
            FeatureUpdate
        }
        public enum enumProjectState
        {
            δ��� = 0,
            ��� = 1
        }
        public enum enumZoneState
        {
            �� = 0,
            �����ѷ��� = 1,
            ���������� = 2,
            ���ύ�������� = 3,
            ���δͨ�� = 4,
            ���ͨ�� = 5,
            �Ѿ��ύ�����ƿ� = 6
        }
        public enum enumInterZoneState
        {
            �� = 0,
            �����ѷ��� = 1,
            �����ޱ� = 2,
            ���δͨ�� = 3,
            ����ޱ� = 4
        }
        public enum enumMapState
        {
            �� = 0,
            �����ޱ� = 1,
            ���δͨ�� = 2,
            ����ޱ� = 3
        }
    }
}