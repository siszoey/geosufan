using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using SysCommon.Gis;
using SysCommon.Error;
using System.Windows.Forms;
using ESRI.ArcGIS.GISClient;
namespace SysCommon
{
    public static class ModuleMap
    {
        //public static void ChangeLableEngine2MapLex(IMapControlDefault pMapControl)
        //{
        //    pMapControl.Map.AnnotationEngine =new ESRI.ArcGIS.Maplex.MaplexAnnotateMapClass() ;
        //}
        //added by chulili 20110714 ��֪�ڵ����ƣ����ҽڵ㣨����Ψһ��
        public static DevComponents.AdvTree.Node SearchLayerNodebyName(string nodename,DevComponents.AdvTree.AdvTree pTree)
        {
            if (pTree.Nodes.Count == 0)
                return null;
            DevComponents.AdvTree.Node pSearchNode = null;
            for (int i = 0; i < pTree.Nodes.Count; i++)
            {
                DevComponents.AdvTree.Node pNode = pTree.Nodes[i];
                if (pNode.Name == nodename)
                    return pNode;
                //���õݹ麯�������ҽڵ�
                pSearchNode = SearchLayerNodebyName(pNode, nodename);
                if (pSearchNode != null)
                    return pSearchNode;

            }
            return null;
        }//added by chulili 20110714 ��֪�ڵ����ƣ����ҽڵ㣨����Ψһ�����ݹ����
        public static DevComponents.AdvTree.Node SearchLayerNodebyName(DevComponents.AdvTree.Node pNode, string nodename)
        {
            if (pNode.Name == nodename)
                return pNode;
            if (pNode.Nodes.Count == 0)
                return null;
            DevComponents.AdvTree.Node pSearchNode = null;
            //�����ӽڵ�
            for (int i = 0; i < pNode.Nodes.Count; i++)
            {
                DevComponents.AdvTree.Node ptmpNode = pNode.Nodes[i];
                if (ptmpNode.Name == nodename)
                    return ptmpNode;
                //�ݹ���ò���
                pSearchNode = SearchLayerNodebyName(ptmpNode, nodename);
                //������ҵ����򷵻�
                if (pSearchNode != null)
                    return pSearchNode;

            }
            return null;
        }

        public static ILayer GetLayerByNodeKey(IWorkspace pTmpWorkSpace, IMap pMap, string strNodeKey,XmlDocument LayerTreeXmlDoc)
        {
            return GetLayerByNodeKey(pTmpWorkSpace,pMap, strNodeKey,LayerTreeXmlDoc, true);
        }
        //��ȡͼ���NodeKey��Ϣ
        public static string GetNodeKeyOfLayer(ILayer pLayer)
        {
            string strNodeKey = "";
            ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
            if (pLayerGenPro != null)
            {
                //��ȡͼ�������
                string strNodeXml = pLayerGenPro.LayerDescription;
                if (strNodeXml != "")
                {
                    XmlDocument pXmlDoc = new XmlDocument();
                    pXmlDoc.LoadXml(strNodeXml);
                    //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
                    string strSearch = "//Layer";
                    XmlNode pNode = pXmlDoc.SelectSingleNode(strSearch);
                    if (pNode != null)
                    {
                        try
                        {
                            strNodeKey = (pNode as XmlElement).GetAttribute("NodeKey");
                        }
                        catch
                        { }
                    }
                    pXmlDoc = null;
                }
            }
            return strNodeKey;
        }
        //added by chulili 20110730 ����nodeKey����ͼ��c
        //bOnlyFromMap�������壺�Ƿ������Map�в���  true����map�в��� false������Դ�в��ң�����xml����layer        
        public static ILayer GetLayerByNodeKey(IWorkspace pTmpWorkSpace, IMap pMap, string strNodeKey, XmlDocument LayerTreeXmlDoc, bool bOnlyFromMap)
        {
            if (pMap == null) return null;
            if (strNodeKey.Equals(string.Empty)) return null;

            ILayer pSearchLayer = null;
            //ѭ���ӽڵ㣬�ȶ�NodeKey
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                ILayer pLayer = pMap.get_Layer(i);
                if (pLayer != null)
                {   //���ò���ͼ��ĺ���
                    pSearchLayer = GetLayerByNodeKey(pMap, pLayer, strNodeKey);
                    if (pSearchLayer != null)
                    {
                        return pSearchLayer;
                    }
                }

            }
            if (!bOnlyFromMap)  //�����������ͼ�л�ȡ����Ҫ�������ļ��л�ȡ������layer
            {
                Exception err = null;
                if (LayerTreeXmlDoc != null && pTmpWorkSpace!=null)
                {
                    XmlNode pLayerNode = LayerTreeXmlDoc.SelectSingleNode("//Layer[@NodeKey='" + strNodeKey + "']");
                    if (pLayerNode != null)
                    {
                        ILayer pLayer = AddLayerFromXml(_DicDataLibWks, pLayerNode, pTmpWorkSpace, "", null, out err);
                        if (pLayer != null)
                        {
                            return pLayer;
                        }
                    }
                }
            }
            return null;
        }
        //added by chulili 20110730����NodeKey����ͼ�� �ݹ����
        private static ILayer GetLayerByNodeKey(IMap pMap, ILayer pLayer, string strNodeKey)
        {
            //if (pMap == null)
            //    return null;
            if (pLayer == null)
                return null;
            if (strNodeKey.Equals(string.Empty)) return null;
            ILayer pSearchLayer = null;
            //�����ӽڵ�
            IGroupLayer pGrouplayer = pLayer as IGroupLayer;
            if (pGrouplayer != null)
            {
                ICompositeLayer pComLayer = pGrouplayer as ICompositeLayer;
                for (int i = 0; i < pComLayer.Count; i++)
                {
                    ILayer pTmpLayer = pComLayer.get_Layer(i);
                    pSearchLayer = GetLayerByNodeKey(pMap, pTmpLayer, strNodeKey);
                    if (pSearchLayer != null)
                    {
                        return pSearchLayer;
                    }
                }
            }
            else
            {
                ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                //��ȡͼ�������
                string strNodeXml = pLayerGenPro.LayerDescription;
                if (strNodeKey != "" && strNodeXml!="")
                {
                    XmlDocument pXmlDoc = new XmlDocument();
                    pXmlDoc.LoadXml(strNodeXml);
                    //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
                    string strSearch = "//Layer[@NodeKey=" + "'" + strNodeKey + "'" + "]";
                    XmlNode pNode = pXmlDoc.SelectSingleNode(strSearch);
                    if (pNode != null)
                    {
                        pXmlDoc = null;
                        return pLayer;
                    }
                }
            }
            return null;
        }
        //added by chulili 20110730����NodeKey����ͼ�� �ݹ����
        public static ILayer GetLayerByNodeKey(IList<ILayer> pLayerList, string strNodeKey)
        {
            if (pLayerList == null)
                return null;
            if (pLayerList.Count==0)
                return null;
            if (strNodeKey.Equals(string.Empty)) return null;
            ILayer pSearchLayer = null;
            for (int i = 0; i < pLayerList.Count; i++)
            {
                ILayer ptmpLayer = pLayerList[i];
                pSearchLayer = GetLayerByNodeKey(null, ptmpLayer, strNodeKey);
                if (pSearchLayer != null)
                {
                    return pSearchLayer;
                }
            }
            return null;
        }
        public static void SetDataKey(DevComponents.AdvTree.Node pNode,XmlNode pXmlnode)
        {
            while (pNode != null && pXmlnode!=null)
            {
                pNode.DataKey = pXmlnode as object;
                pNode = pNode.Parent;
                pXmlnode = pXmlnode.ParentNode;
            }
        }
        
        public static bool IsLayerTreeXmlRight(string xmlpath)
        {
            if (xmlpath.Equals(string.Empty))
                return false;
            if (!File.Exists(xmlpath))
                return false;
            //��xml�ļ�
            XmlDocument pXmldoc = new XmlDocument();
            pXmldoc.Load(xmlpath );
            //�ж������ӽڵ�
            if (pXmldoc.ChildNodes.Count > 0)
            {
                for (int i = 0; i < pXmldoc.ChildNodes.Count; i++)
                {
                    XmlNode pNode = pXmldoc.ChildNodes[i];
                    if (!IsLayerTreeXmlRight(pNode))//���κ�һ���ӽڵ㲻�����������򷵻ؼ�
                    {
                        return false;
                    }
                }
            }
            else//û���κ��ӽڵ㣬Ҳ����������
            {
                return false;
            }
            return true;
        }
        //���ݽڵ������ж� �ýڵ��Ƿ����ͼ��Ŀ¼�淶 �ݹ���ú���added by chulili 20110729
        public static bool IsLayerTreeXmlRight(XmlNode pNode)
        {
            if (pNode == null) return false;
            if (pNode.NodeType != XmlNodeType.Element)
            {
                return true;
            }
            switch (pNode.Name)
            {
                case "Root":
                case "DIR":
                case "DataDIR":
                    break;
                case "ConfigInfo":
                case "Layer":
                    return true;//�жϵ�Layer�ڵ�Ϊֹ�������ж��ӽڵ�
                    break;
                default :
                    return false;
                    break;
            }
            //�ж������ӽڵ㣬ֻҪ��һ��������Ҫ���򷵻�false
            if (pNode.ChildNodes.Count > 0)
            {
                for (int i = 0; i < pNode.ChildNodes.Count; i++)
                {
                    XmlNode pTmpnode = pNode.ChildNodes[i];
                    if (!IsLayerTreeXmlRight(pTmpnode))
                        return false;
                }
            }
            return true;
        }
        //���������ݿⱣ��ͼ��Ŀ¼
        public static bool SaveLayerTree(IWorkspace pWorkspace,string xmlpath)
        {
            //�жϸ��������Ƿ���Ч
            if (pWorkspace == null)
            {
                return false ;
            }
            if (!System.IO.File.Exists(xmlpath)) return false ;
            Exception exError = null;
            ITransactions pTransactions = null;
            //����ͼ�������ɱ��������ݿⱣ�棩
            try
            {
                XmlDocument pXmlDoc = new XmlDocument();
                pXmlDoc.Load(xmlpath );
                IMemoryBlobStream pBlobStream = new MemoryBlobStreamClass();
                byte[] bytes = Encoding.Default.GetBytes(pXmlDoc.OuterXml);                
                pBlobStream.ImportFromMemory(ref bytes[0], (uint)bytes.GetLength(0));

                //��������
                pTransactions = (ITransactions)pWorkspace;
                if (!pTransactions.InTransaction) pTransactions.StartTransaction();
                SysGisTable sysTable = new SysGisTable(pWorkspace);
                Dictionary<string, object> dicData = new Dictionary<string, object>();
                dicData.Add("LAYERTREE", pBlobStream);
                dicData.Add("NAME", "LAYERTREE");
                //�ж��Ǹ��»������
                //����������ӣ��Ѵ��������
                if (!sysTable.ExistData("LAYERTREE_XML", "NAME='LAYERTREE'"))
                {
                    if (!sysTable.NewRow("LAYERTREE_XML", dicData, out exError))
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ʧ�ܣ�");
                        return false ;
                    }
                }
                else
                {
                    if (!sysTable.UpdateRow("LAYERTREE_XML", "NAME='LAYERTREE'", dicData, out exError))
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ʧ�ܣ�");
                        return false ;
                    }
                }
                //�ύ����
                if (pTransactions.InTransaction) pTransactions.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                //����������ύ
                if (pTransactions.InTransaction) pTransactions.AbortTransaction();
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ʧ�ܣ�");
                return false;
            }
        }

        //����mxd����ģ�壬�洢�����������棬�洢�ı���ΪRender
        public static bool ReadmxdToDataBase(string mxdpath, string password, IWorkspace pWKS,bool iscover)
        {
            //��mxd�ļ�
            IMapDocument pMdoc = new MapDocumentClass();
            pMdoc.Open(mxdpath, password);
            if (pMdoc == null) return false;
            int mapcnt = pMdoc.MapCount;
            //����SysGisTable������������Render��д����
            SysGisTable sysTable = new SysGisTable(pWKS);

            for (int i = 0; i < mapcnt; i++)
            {
                IMap pMap = pMdoc.get_Map(i);
                int layercnt = pMap.LayerCount;
                for (int j = 0; j < layercnt; j++)
                {
                    //��ȡmxd�ļ���ͼ��ķ��Ż���Ϣ
                    IFeatureLayer pFeaLayer = pMap.get_Layer(j) as IFeatureLayer;
                    IGeoFeatureLayer pGeoLayer = pMap.get_Layer(j) as IGeoFeatureLayer;
                    ILayer pLayer = pMap.get_Layer(j) as ILayer;
                    IFeatureLayerDefinition pLayerDefine = pMap.get_Layer(j) as IFeatureLayerDefinition;
                    IFeatureRenderer pRender = pGeoLayer.Renderer;
                    IDataset pDataset = pFeaLayer.FeatureClass as IDataset;

                    byte[] rendererValue = null;
                    string RendererType = "";
                    
                    //��ȡrender��Ϣ��д�ɶ�������
                    WriteRendererTobyte(ref rendererValue, ref RendererType, pRender, "", pFeaLayer as ILayer, "");
                    
                    Dictionary<string, object> dicData = new Dictionary<string, object>();
                    IMemoryBlobStream pBlobStream = new MemoryBlobStreamClass();
                    pBlobStream.ImportFromMemory(ref rendererValue[0], (uint)rendererValue.GetLength(0));
                    //render������д���ֵ����棬�Ժ�洢��������
                    dicData.Add("ID", System.Guid.NewGuid().ToString());
                    dicData.Add("Render", pBlobStream);
                    dicData.Add("Layer", pFeaLayer.Name);
                    dicData.Add("RenderType", RendererType);
                    //�����С������
                    //xisheng 20110812
                  //  dicData.Add("MaxScale", pLayer.MaximumScale.ToString());
                 //   dicData.Add("MinScale", pLayer.MinimumScale.ToString());
                    //������ʾ
                    dicData.Add("DefineExpression", pLayerDefine.DefinitionExpression);
                    if (pDataset != null)
                    {
                        dicData.Add("FeatureClass", pDataset.Name);
                    }
                    Exception exError = null;

                    ITransactions pTransactions = (ITransactions)pWKS;
                    if (!pTransactions.InTransaction) pTransactions.StartTransaction();
                    //��render�����һ���¼�¼����¼��ǰͼ���render
                    bool result=false;
                    if(sysTable.ExistData("Render","Layer='"+pFeaLayer.Name +"'") && iscover)
                    {
                        dicData.Remove("ID");
                        result = sysTable.UpdateRow("Render", "Layer='" + pFeaLayer.Name + "'", dicData, out exError);
                    }
                    else
                    {
                     result= sysTable.NewRow("Render", dicData, out exError);
                    }
                    if (pTransactions.InTransaction) pTransactions.CommitTransaction();
                    if (!result) return result;
                }
            }
            return true;

        }
        
       
        public static void ReadLayerToDataBaseEx(ILayer pLayer, IWorkspace pWKS, bool iscover,SysCommon.CProgress vProg)
        {
            if (pLayer == null)
                return;
            if(pWKS==null)
                return;
            //��ȡrender��Ϣ
            byte[] rendererValue = null;
            string RendererType = "";
            //��ȡmxd�ļ���ͼ��ķ��Ż���Ϣ shduan 20110720 ����DEM�ֲ���ɫ����
            string strLayerName = "";
            string strFeaClsName = "";
            //����SysGisTable������������Render��д����
            SysGisTable sysTable = new SysGisTable(pWKS);
            strLayerName = pLayer.Name;
            if (vProg != null)
            {
                vProg.SetProgress("����ͼ�㡮"+strLayerName+"��");
            }
            if (pLayer is IFeatureLayer)
            {
                IFeatureLayer pFeaLayer = pLayer as IFeatureLayer;
                
                IGeoFeatureLayer pGeoLayer = pLayer as IGeoFeatureLayer;
                IFeatureRenderer pFRender = null;
                if (pGeoLayer != null)
                {
                    pFRender = pGeoLayer.Renderer;
                }

                IDataset pDataset = pFeaLayer.FeatureClass as IDataset;
                if (pDataset != null) strFeaClsName = pDataset.Name;
                //д�ɶ�������
                WriteRendererTobyte(ref rendererValue, ref RendererType, pFRender, "", pLayer, "");
            }
            else if (pLayer is IRasterLayer)
            {
                IRasterLayer pRL = pLayer as IRasterLayer;
                strLayerName = pRL.Name;
                IRasterRenderer pRasterRenderer = pRL.Renderer;

                //д�ɶ�������
                WriteRasterRendererTobyte(ref rendererValue, ref RendererType, pRasterRenderer, "", pLayer, "");
            }
            else 
            {
                return;
            }
            //��ȡͼ������
            IFeatureLayerDefinition pLayerDefine = pLayer as IFeatureLayerDefinition;
            byte[] LayerValue = null;
            string LayerType = "";
            //д�ɶ�������
            WriteLayerConfigTobyte(ref LayerValue, ref LayerType, pLayer);
            Dictionary<string, object> dicData = new Dictionary<string, object>();
            IMemoryBlobStream pBlobStream = new MemoryBlobStreamClass();
            pBlobStream.ImportFromMemory(ref LayerValue[0], (uint)LayerValue.GetLength(0));

            IMemoryBlobStream pBlobStreamRender = null;
            if (rendererValue != null)
            {
                pBlobStreamRender = new MemoryBlobStreamClass();
                pBlobStreamRender.ImportFromMemory(ref rendererValue[0], (uint)rendererValue.GetLength(0));
            }

            //Layer����д���ֵ����棬�Ժ�洢��������
            dicData.Add("ID", System.Guid.NewGuid().ToString());
            dicData.Add("LayerConfig", pBlobStream);
            dicData.Add("Layer", strLayerName);
            dicData.Add("LayerType", LayerType);
            if (pBlobStreamRender != null)
            {
                dicData.Add("Render", pBlobStreamRender);
            }
            dicData.Add("RenderType", RendererType);
            //�����С������ shduan 20110717add
            //xisheng 20110812
            // dicData.Add("MaxScale", pLayer.MaximumScale.ToString());
            // dicData.Add("MinScale", pLayer.MinimumScale.ToString());
            dicData.Add("FeatureClass", strFeaClsName);
            //������ʾ shduan 20110717add
            if (pLayerDefine != null)
            {
                dicData.Add("DefineExpression", pLayerDefine.DefinitionExpression);
            }
            else
            {
                dicData.Add("DefineExpression", "");
            }
            Exception exError = null;

            ITransactions pTransactions = (ITransactions)pWKS;
            if (!pTransactions.InTransaction) pTransactions.StartTransaction();
            //��render�����һ���¼�¼����¼��ǰͼ���render
            bool result = false;
            if (sysTable.ExistData("Render", "Layer='" + strLayerName + "'") && iscover)
            {
                dicData.Remove("ID");
                result = sysTable.UpdateRow("Render", "Layer='" + strLayerName + "'", dicData, out exError);
            }
            else
             {
                result = sysTable.NewRow("Render", dicData, out exError);
            }
            if (pTransactions.InTransaction) pTransactions.CommitTransaction();
            return;
        }
        public static void ReadGroupLayerToDataBaseEx(IGroupLayer pGroupLayer,IWorkspace pWKS, bool iscover,SysCommon.CProgress vProg)
        {
            ICompositeLayer pComLayer = pGroupLayer as ICompositeLayer;
            if (pComLayer != null)
            {
                for (int i = 0; i < pComLayer.Count; i++)
                {
                    ILayer pLayer = pComLayer.get_Layer(i);
                    if (pLayer is IGroupLayer)
                    {
                        ReadGroupLayerToDataBaseEx(pLayer as IGroupLayer, pWKS, iscover, vProg);
                    }
                    else
                    {
                        ReadLayerToDataBaseEx(pLayer, pWKS, iscover, vProg);
                    }
                }
            }
        }
        //added by chulili 20110711 ��Ϊֱ�Ӱ�layer����Ϊ�ֶ�ֵ������mxd����ģ�壬�洢�����������棬�洢�ı���ΪRender
        public static bool ReadmxdToDataBaseEx(string mxdpath, string password, IWorkspace pWKS, bool iscover,SysCommon.CProgress vProgress)
        {
            try
            {
                //��mxd�ļ�
                IMapDocument pMdoc = new MapDocumentClass();
                pMdoc.Open(mxdpath, password);
                if (pMdoc == null) return false;
                int mapcnt = pMdoc.MapCount;
                

                for (int i = 0; i < mapcnt; i++)
                {
                    IMap pMap = pMdoc.get_Map(i);
                    int layercnt = pMap.LayerCount;
                    for (int j = 0; j < layercnt; j++)
                    {
                        //��ȡrender��Ϣ
                        //byte[] rendererValue = null;
                        //string RendererType = "";
                        //;
                        ////��ȡmxd�ļ���ͼ��ķ��Ż���Ϣ shduan 20110720 ����DEM�ֲ���ɫ����
                        //string strLayerName = "";
                        //string strFeaClsName = "";
                        ILayer pLayer = pMap.get_Layer(j) as ILayer;
                        if (pLayer is IGroupLayer)
                        {
                            ReadGroupLayerToDataBaseEx(pLayer as IGroupLayer, pWKS, iscover, vProgress);
                        }
                        else
                        {
                            ReadLayerToDataBaseEx(pLayer, pWKS, iscover, vProgress);
                        }

                    }
                }
                return true;
            }
            catch 
            {
                return false;
            }
            return true;
        }
        //��ͼ�����л�
        private static void WriteLayerConfigTobyte(ref byte[] LayerByte, ref string LayerType, ILayer pLayer)
        {
            if (pLayer is IFDOGraphicsLayer)
            {
                LayerType="FDOGraphicsLayer";
            }
            else if(pLayer is IDimensionLayer )
            {
                LayerType = "DimensionLayer";
            }
            else if (pLayer is IGdbRasterCatalogLayer)
            {
                LayerType = "GdbRasterCatalogLayer";
            }
            else if (pLayer is IRasterLayer)
            {
                LayerType = "RasterLayer";
            }
            else
            {
                LayerType = "FeatureLayer";
            }
            IPersistStream pPersistStream = pLayer as IPersistStream;


            IStream pStream = new XMLStreamClass();
            pPersistStream.Save(pStream, 0);

            IXMLStream pXMLStream = pStream as IXMLStream;
            LayerByte = pXMLStream.SaveToBytes();
        }
        // �����õ�Renderer���л�д�����ݿ�       
        private static void WriteRendererTobyte(ref byte[] RenderByte, ref string RendererType, IFeatureRenderer pFeaRenderer, string sRgbColor, ILayer pLayer, string sKeyInfo)
        {
            if (pFeaRenderer == null) return;
            if (pLayer is IFDOGraphicsLayer && sKeyInfo != "")
            {
                RendererType = "AnnoColor";
                RenderByte = (byte[])Encoding.Default.GetBytes(sRgbColor);
            }
            else
            {
                //��redererinfo����д����ص�renderer��Ϣ
                RendererType = "";
                if (pFeaRenderer is ISimpleRenderer)
                {
                    RendererType = "SimpleRenderer";
                }
                else if (pFeaRenderer is IUniqueValueRenderer)
                {
                    RendererType = "UniqueValueRenderer";
                }
                else if (pFeaRenderer is IClassBreaksRenderer)
                {
                    RendererType = "ClassBreaksRenderer";
                }
                else if (pFeaRenderer is IProportionalSymbolRenderer)
                {
                    RendererType = "ProportionalSymbolRenderer";
                }
                else if (pFeaRenderer is IChartRenderer)
                {
                    RendererType = "ChartRenderer";
                }

                IPersistStream pPersistStream = pFeaRenderer as IPersistStream;

                //IMemoryBlobStream pMemoryStream = new MemoryBlobStream();
                IStream pStream = new XMLStreamClass();
                pPersistStream.Save(pStream, 0);

                IXMLStream pXMLStream = pStream as IXMLStream;
                RenderByte = pXMLStream.SaveToBytes();
            }
        }
        // ������RasterDataset��Renderer���л�д�����ݿ�     shduan 20110721    
        private static void WriteRasterRendererTobyte(ref byte[] RenderByte, ref string RendererType, IRasterRenderer  pRasRenderer, string sRgbColor, ILayer pLayer, string sKeyInfo)
        {
            
                //��redererinfo����д����ص�renderer��Ϣ
                RendererType = "";
                if (pRasRenderer is IRasterClassifyColorRampRenderer)
                {
                    RendererType = "RasterClassifyColorRampRenderer";
                }
                else if (pRasRenderer is IRasterDiscreteColorRenderer)
                {
                    RendererType = "RasterDiscreteColorRenderer";
                }
                else if (pRasRenderer is IRasterRGBRenderer)
                {
                    RendererType = "RasterRGBRenderer";
                }
                else if (pRasRenderer is IRasterStretchColorRampRenderer)
                {
                    RendererType = "RasterStretchColorRampRenderer";
                }
                else if (pRasRenderer is IRasterUniqueValueRenderer)
                {
                    RendererType = "RasterUniqueValueRenderer";
                }
                //else if (pRasRenderer is IRasterColormapRenderer)
                //{
                //    RendererType = "RasterColormapRenderer";
                //}

                IPersistStream pPersistStream = pRasRenderer as IPersistStream;

                //IMemoryBlobStream pMemoryStream = new MemoryBlobStream();
                IStream pStream = new XMLStreamClass();
                pPersistStream.Save(pStream, 0);

                IXMLStream pXMLStream = pStream as IXMLStream;
                RenderByte = pXMLStream.SaveToBytes();
        }
        //��ʼ������Դ�����ռ��ֵ�
        public static void InitDBSourceDic(IWorkspace pConfigWks, IDictionary<string, IWorkspace> DicDataLibWks,IDictionary<string,List<IDataset>> DicDataset)
        {
            Exception exError;
            DicDataLibWks.Clear();
            SysCommon.Gis.SysGisTable  sysTable = new SysCommon.Gis.SysGisTable(pConfigWks);
            List<Dictionary<string, object>> lstDicData = sysTable.GetRows("DATABASEMD", "", out exError);
            if (lstDicData == null)
                return;
            if (lstDicData.Count > 0)
            {
                for (int i = 0; i < lstDicData.Count; i++)
                {
                    string connstr = "";
                    string dbsourceid="";
                    int itype=-1;
                    if (lstDicData[i]["CONNECTIONINFO"]!=null) 
                        connstr = lstDicData[i]["CONNECTIONINFO"].ToString();                    
                    if(lstDicData[i]["ID"]!=null)
                        dbsourceid= lstDicData[i]["ID"].ToString();
                    if (lstDicData[i]["DATAFORMATID"]!=null) 
                        itype = int.Parse(lstDicData[i]["DATAFORMATID"].ToString());
                    if ((!connstr.Equals("")) && (!dbsourceid.Equals("")))
                    {
                        IWorkspace pWorkspace = GetWorkSpacefromConninfo(connstr, itype);
                        DicDataLibWks.Add(dbsourceid, pWorkspace);
                        if (DicDataset != null)
                        {
                            int index6 = connstr.LastIndexOf("|");
                            string strDatasets = connstr.Substring(index6 + 1);
                            string[] strTemp = strDatasets.Split(new char[] { ',' });
                            IFeatureWorkspace pFeaWorkSpace = pWorkspace as IFeatureWorkspace;
                            if (pFeaWorkSpace != null)
                            {
                                List<IDataset> pTmpListdataset = new List<IDataset>();
                                for (int k = 0; k < strTemp.Length; k++)
                                {
                                    IDataset pTmpdataset = pFeaWorkSpace.OpenFeatureDataset(strTemp[k]) as IDataset;
                                    if (pTmpdataset != null)
                                    {
                                        pTmpListdataset.Add(pTmpdataset);
                                    }
                                }
                                DicDataset.Add(dbsourceid, pTmpListdataset);
                            }
                        }
                    }
                }
            }

        }
        //��ʼ������Դ�����ռ��ֵ�
        public static void InitDBSourceDic(IWorkspace pConfigWks, IDictionary<string, IWorkspace> DicDataLibWks)
        {
            InitDBSourceDic(pConfigWks, DicDataLibWks, null);
        }

        ////��ʼ��ͼ�����������ֵ䣨Ӣ����ӳ����������key:Ӣ��  value:����   ZQ 2011020  modify ��ֲ��sysCommon
        //public static void InitLayerNameDic(IWorkspace pConfigWks, IDictionary<string, string > DicLayername)
        //{
        //    Exception exError=null;
        //    DicLayername.Clear();
        //    SysCommon.Gis.SysGisTable sysTable = new SysCommon.Gis.SysGisTable(pConfigWks);
        //    List<Dictionary<string, object>> lstDicData = sysTable.GetRows("��׼ͼ������", "", out exError);
        //    if (lstDicData == null)
        //        return;
        //    try
        //    {
        //        if (lstDicData.Count > 0)
        //        {
        //            for (int i = 0; i < lstDicData.Count; i++)
        //            {
        //                string strName = "";
        //                string strAliasName = "";
        //                if (lstDicData[i]["CODE"] != null)
        //                    strName = lstDicData[i]["CODE"].ToString();
        //                if (lstDicData[i]["NAME"] != null)
        //                    strAliasName = lstDicData[i]["NAME"].ToString();
        //                //��ͼ������������ӵ��ֵ���
        //                if ((!strName.Equals("")) && (!strAliasName.Equals("")))
        //                {
        //                    if (!DicLayername.Keys.Contains(strName))
        //                    {
        //                        DicLayername.Add(strName, strAliasName);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    { }
        //}
        //���������ַ�����ȡ�����ռ�
        //�˴������ַ����ǹ̶���ʽ�����Ӵ� Server|Service|Database|User|Password|Version
        public static IWorkspace GetWorkSpacefromConninfo(string conninfostr, int type)
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
                    pWSFact = new ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactoryClass();
                    sDatabase = conninfostr.Substring(index2 + 1, index3 - index2 - 1);
                    break;
                case 2://gdb
                    pWSFact = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                    sDatabase = conninfostr.Substring(index2 + 1, index3 - index2 - 1);
                    break;
                case 3://sde
                    pWSFact = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactoryClass();
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
        /// <summary>
        /// �жϷ����Ƿ������ҵ���Ӧ�ķ���
        /// </summary>
        /// <param name="pHostOrUrl">�����ַ</param>
        /// <param name="pServiceName">����</param>
        /// <param name="pIsLAN">����</param>
        /// <returns></returns>
        public static bool OpenConn(string pHostOrUrl, string pServiceName, bool bLAN)
        {
            bool bIsConn = false;
            try
            {

                IAGSServerConnectionFactory pConnF = new AGSServerConnectionFactory();
                IAGSServerConnection pAGSServerConnection = new AGSServerConnectionClass();
                IPropertySet pProSet = new PropertySet();
                if (bLAN)
                    pProSet.SetProperty("machine", pHostOrUrl);
                else
                    pProSet.SetProperty("url", pHostOrUrl);
                ///���ӷ���
                pAGSServerConnection = pConnF.Open(pProSet, 0);
                if(pAGSServerConnection != null)
                {
                    ///��ȡ���еķ�������
                    pAGSServerConnection.ServerObjectNames.Reset();
                    IAGSEnumServerObjectName pEnumServerObjectNames = pAGSServerConnection.ServerObjectNames;
                    pEnumServerObjectNames.Reset();
                    IAGSServerObjectName pServerObjectName = pEnumServerObjectNames.Next();
                    while (pServerObjectName != null)
                    {
                        ///�Ҷ�Ӧ�ķ���
                        if (pServerObjectName.Name.ToLower() == pServiceName.ToLower())
                        {
                            return bIsConn = true;
                        }
                        pServerObjectName = pEnumServerObjectNames.Next();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnumServerObjectNames);
                }
                else
                {
                    return bIsConn =false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pAGSServerConnection);
                return bIsConn = false;
            }

            catch 
            {
                MessageBox.Show("����ʧ�ܣ���������Ƿ���","��ʾ��");
                return bIsConn = false; ;
            }
        }
        //pHostOrUrl   �����ַ
        //pServiceName ��������
        public static IAGSServerObjectName GetMapServer(string pHostOrUrl, string pServiceName, bool pIsLAN)
        {


            //������������
            IPropertySet pPropertySet = new PropertySetClass();
            if (pIsLAN)
                pPropertySet.SetProperty("machine", pHostOrUrl);
            else
                pPropertySet.SetProperty("url", pHostOrUrl);

            //������

            IAGSServerConnectionFactory pFactory = new AGSServerConnectionFactory();
            //Type factoryType = Type.GetTypeFromProgID(
            //    "esriGISClient.AGSServerConnectionFactory");
            //IAGSServerConnectionFactory agsFactory = (IAGSServerConnectionFactory)
            //    Activator.CreateInstance(factoryType);
            IAGSServerConnection pConnection = pFactory.Open(pPropertySet, 0);

            //Get the image server.
            IAGSEnumServerObjectName pServerObjectNames = pConnection.ServerObjectNames;
            pServerObjectNames.Reset();
            IAGSServerObjectName ServerObjectName = pServerObjectNames.Next();
            while (ServerObjectName != null)
            {
                if ((ServerObjectName.Name.ToLower() == pServiceName.ToLower()) &&
                    (ServerObjectName.Type == "MapServer"))
                {

                    break;
                }
                ServerObjectName = pServerObjectNames.Next();
            }

            //���ض���
            return ServerObjectName;
        }
        //added by chulili 20111119 ��ȡ��ǰ�������Ƿ�ͼ��Ŀɼ�������
        public static bool GetScaleVisibleOfLayer(double dCurScale, ILayer pLayer)
        {
            bool bRes = false;
            if (pLayer == null) //�ų�NULLֵ
            {
                return bRes;
            }
            //if (!pLayer.Visible)    //ͼ�㱾�����ɼ�
            //{
            //    return bRes;
            //}
            //��ȡͼ������Ʊ�����
            double dMaxScale = pLayer.MaximumScale;
            double dMinScale = pLayer.MinimumScale;
            if (dMaxScale > 0)  //���������ߴ���
            {
                if (dCurScale < dMaxScale) //������������������
                {
                    return bRes;
                }
            }
            if (dMinScale > 0)  //����С�����ߴ���
            {
                if (dCurScale > dMinScale) //��������С����������
                {
                    return bRes;
                }
            }
            return true;    //�����С�����߲�����  ����  ���������С������
        }
        //added by chulili 20111119 ��ȡͼ���Ƿ������Ŀɼ�
        public static bool GetVisibleOfLayer(double dCurScale,ILayer pLayer)
        {
            bool bRes = false;
            if (pLayer == null) //�ų�NULLֵ
            {
                return bRes;
            }
            if (!pLayer.Visible)    //ͼ�㱾�����ɼ�
            {
                return bRes;
            }
            //��ȡͼ������Ʊ�����
            double dMaxScale = pLayer.MaximumScale;
            double dMinScale = pLayer.MinimumScale;
            if (dMaxScale > 0)  //���������ߴ���
            {
                if (dCurScale < dMaxScale) //������������������
                {
                    return bRes;
                }
            }
            if (dMinScale > 0)  //����С�����ߴ���
            {
                if (dCurScale > dMinScale) //��������С����������
                {
                    return bRes;
                }
            }
            return true;    //�����С�����߲�����  ����  ���������С������
        }
        public static ILayer AddLayerFromXml(IDictionary<string, IWorkspace> DicDataLibWks,XmlNode nodeLayer, IWorkspace pConfigWks, string strConfigTable, ILayer oldLayer,out Exception error)
        {
            error = null;
            try
            {
                #region �������� ��ʱΪ�ع���ӵĴ��� chulili 20111104
                if (nodeLayer.Name == "Layer")
                {
                    string tmpDataType = nodeLayer.Attributes["DataType"].Value;
                    if (tmpDataType == "SERVICE")
                    {
                        try
                        {
                            string strServiceLocation = nodeLayer.Attributes["ConnectKey"].Value; ;
                            string strServiceName = nodeLayer.Attributes["FeatureDatasetName"].Value;
                            if (OpenConn(strServiceLocation, strServiceName, false))
                            {
                                IAGSServerObjectName pServerObjectName = GetMapServer(strServiceLocation, strServiceName, false);
                                IName pName = (IName)pServerObjectName;
                                IAGSServerObject pServerObject = (IAGSServerObject)pName.Open();
                                IMapServer pMapServer = (IMapServer)pServerObject;

                                IMapServerLayer pMapServerLayer = new MapServerLayerClass();

                                pMapServerLayer.ServerConnect(pServerObjectName, pMapServer.DefaultMapName);
                                ILayerGeneralProperties layerProerties = (ILayerGeneralProperties)pMapServerLayer;
                                if (layerProerties != null)
                                {
                                    layerProerties.LayerDescription = nodeLayer.OuterXml;
                                }
                                return pMapServerLayer as ILayer;
                            }
                            return null;
                        }
                        catch(Exception err)
                        {
                            return null;
                        }

                    }
                }
                #endregion
                ILayer returnLayer = null;
                string dataType = "";
                string layerCode = nodeLayer.Attributes["Code"].Value;
                string layerText = nodeLayer.Attributes["NodeText"].Value;
                string FeatureDataset = nodeLayer.Attributes["FeatureDatasetName"].Value;
                string layerView = "1";
                bool LayerVisible = false;
                if (nodeLayer.Attributes["View"] != null)
                    layerView = nodeLayer.Attributes["View"].Value;

                if (layerView.ToLower() == "true" || layerView == "1")
                {
                    LayerVisible = true;
                }
                IFeatureWorkspace featureWorkspace = null;
                if (nodeLayer.Name == "Layer")
                {
                    dataType = nodeLayer.Attributes["DataType"].Value;
                    featureWorkspace = GetWorkspaceFromLayerXmlNode(DicDataLibWks, nodeLayer, pConfigWks, strConfigTable) as IFeatureWorkspace;
                }
                else
                {
                    dataType = nodeLayer.Name;
                    featureWorkspace = GetWorkspaceFromLayerXmlNode(DicDataLibWks, nodeLayer, pConfigWks, strConfigTable) as IFeatureWorkspace;
                }

                if (featureWorkspace == null)
                {
                    if (error == null)
                    {
                        error = new Exception("δ�ҵ�����Դ");
                    }
                    return null;
                }
                IWorkspace2 pWks2 = featureWorkspace as IWorkspace2;

                switch (dataType)
                {
                    case "SubType":
                        #region SubType
                        {
                            IFeatureLayer subTypeLayer = null;
                            if (oldLayer == null)
                            {
                                IFeatureClass subTypeClass = featureWorkspace.OpenFeatureClass(layerCode);
                                subTypeLayer = GetNewFeatureLayer(subTypeClass);
                                subTypeLayer.FeatureClass = subTypeClass;
                            }
                            else
                                subTypeLayer = oldLayer as IFeatureLayer;

                            SetXmlAttrToLayer(subTypeLayer, nodeLayer, pConfigWks);

                            string[] textInfo = layerText.Split('@');
                            if (textInfo.Length > 1)
                                subTypeLayer.Name = layerCode + "@" + textInfo[1];
                            else
                                subTypeLayer.Name = layerCode;
                            subTypeLayer.Visible = true;
                            returnLayer = subTypeLayer as ILayer;
                        }
                        #endregion
                        break;
                    case "FC":
                        #region FC
                        {
                            IFeatureLayer featureLayer = null;
                            if (oldLayer == null)
                            {
                                if (!pWks2.get_NameExists(esriDatasetType.esriDTFeatureClass, layerCode)) return null;

                                IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(layerCode);
                                SetXmlAttrToFeaLayer(out featureLayer, featureClass, nodeLayer, pConfigWks);
                                if (featureLayer == null)
                                {
                                    featureLayer = GetNewFeatureLayer(featureClass);
                                    featureLayer.FeatureClass = featureClass;
                                    SetXmlAttrToLayer(featureLayer, nodeLayer, pConfigWks);
                                }
                            }
                            else
                            {
                                featureLayer = oldLayer as IFeatureLayer;

                                SetXmlAttrToLayer(featureLayer, nodeLayer, pConfigWks);
                            }

                            featureLayer.Name = layerText;
                            featureLayer.Visible = true;
                            returnLayer = featureLayer as ILayer;
                        }
                        #endregion
                        break;
                    case "RC":
                        #region RC
                        {
                            try
                            {
                                IRasterWorkspaceEx rasterWorkspace = (IRasterWorkspaceEx)featureWorkspace;
                                IRasterCatalog pRCatalog = rasterWorkspace.OpenRasterCatalog(FeatureDataset);//changed by chulili ֱ��ȡ���ݼ�
                                IGdbRasterCatalogLayer pGRCatalogLayer = new GdbRasterCatalogLayerClass();
                                if (!pGRCatalogLayer.Setup((ITable)pRCatalog))
                                {

                                }
                                ILayer pLayer = pGRCatalogLayer as ILayer;
                                pLayer.Name = layerText;
                                pLayer.Visible = true;
                                SetXmlAttrToLayer(pLayer, nodeLayer, pConfigWks);
                                returnLayer = pLayer;
                            }
                            catch
                            { }
                        }
                        #endregion
                        break;
                    case "RD":
                        #region RD
                        {
                            IRasterWorkspaceEx rasterWorkspace = (IRasterWorkspaceEx)featureWorkspace;
                            IRasterDataset pRDataset = rasterWorkspace.OpenRasterDataset(FeatureDataset);//changed by chulili ֱ��ȡ���ݼ�
                            IRasterLayer pRLayer = new RasterLayer();
                            pRLayer.CreateFromDataset(pRDataset);
                            pRLayer.Name = layerText;
                            pRLayer.Visible = true;

                            SetXmlAttrToLayer(pRLayer, nodeLayer, pConfigWks);
                            returnLayer = pRLayer;
                        }
                        #endregion
                        break;
                    case "SHP":
                        #region SHP
                        {
                            IFeatureClass shpClass = featureWorkspace.OpenFeatureClass(layerCode);
                            IFeatureLayer shpLayer = new FeatureLayer();
                            shpLayer.FeatureClass = shpClass;
                            shpLayer.Name = layerText;
                            shpLayer.Visible = true;

                            SetXmlAttrToLayer(shpLayer, nodeLayer, pConfigWks);

                            returnLayer = shpLayer;
                        }
                        #endregion
                        break;
                    case "CAD":
                        #region CAD
                        {
                            ICadDrawingWorkspace cadWorkspace = (ICadDrawingWorkspace)featureWorkspace;
                            ICadDrawingDataset cadDataset = cadWorkspace.OpenCadDrawingDataset(layerCode);
                            ICadLayer cadLayer = new CadLayerClass();
                            cadLayer.CadDrawingDataset = cadDataset;
                            cadLayer.Name = layerText;
                            cadLayer.Visible = true;

                            SetXmlAttrToLayer(cadLayer, nodeLayer, pConfigWks);
                            returnLayer = cadLayer;
                        }
                        #endregion
                        break;
                    case "TIF":
                    case "BMP":
                    case "SID":
                        #region TIF,BMP,SID
                        {
                            IRasterWorkspace rasterWorkspace = (IRasterWorkspace)featureWorkspace;
                            IRasterDataset pRDataset = rasterWorkspace.OpenRasterDataset(layerCode);
                            IRasterLayer pRLayer = new RasterLayer();
                            pRLayer.CreateFromDataset(pRDataset);
                            pRLayer.Name = layerText;
                            pRLayer.Visible = true;

                            SetXmlAttrToLayer(pRLayer, nodeLayer, pConfigWks);
                            returnLayer = pRLayer;
                        }
                        #endregion
                        break;
                    case "TILE":
                        {
                            //ILayer tileLayer = ClassTile.ConnectAddMapSeverLayer(nodeLayer, (layerView == "1") ? true : false, clsMain);
                            //returnLayer = tileLayer;
                        }
                        break;
                    default:
                        break;
                }
                returnLayer.Visible = LayerVisible;
                return returnLayer;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static IFeatureWorkspace GetWorkspaceFromLayerXmlNode(IDictionary<string, IWorkspace> DicDataLibWks,XmlNode layerNode, IWorkspace pConfigWks, string strConnectTable)
        {
            //
            //return pConfigWks as IFeatureWorkspace;

            if (layerNode.Attributes["ConnectKey"] == null) return null;

            string ConnectKey = layerNode.Attributes["ConnectKey"].Value;
            if (!DicDataLibWks.Keys.Contains(ConnectKey))
            {
                IWorkspace pTempWks = GetWorkspaceByConnectKey(ConnectKey, pConfigWks, strConnectTable);
                if (pTempWks == null) return null;
                DicDataLibWks.Add(ConnectKey, pTempWks);
            }
            if (DicDataLibWks[ConnectKey]==null)            
            {
                return pConfigWks as IFeatureWorkspace;
            }
            else
            {
                return DicDataLibWks[ConnectKey] as IFeatureWorkspace;
            }
        }

        public static IWorkspace GetWorkspaceByConnectKey(string strConnectKey, IWorkspace pWks, string strConfigName)
        {
            Exception exError = null;
            IWorkspace pWorkspace = null;
            SysCommon.Gis.SysGisTable  sysTable = new SysCommon.Gis.SysGisTable(pWks);
            Dictionary<string, object> DicData = sysTable.GetRow("DATABASEMD", "ID="+strConnectKey+"", out exError);
            if (DicData == null)
                return pWorkspace;
            if (DicData.Count == 0)
                return pWorkspace;
            string connstr = "";
            string dbsourceid="";
            int itype=-1;
            if (DicData["CONNECTIONINFO"] != null)
                connstr = DicData["CONNECTIONINFO"].ToString();
            if (DicData["ID"] != null)
                dbsourceid = DicData["ID"].ToString();
            if (DicData["DATAFORMATID"] != null)
                itype = int.Parse(DicData["DATAFORMATID"].ToString());
            if ((!connstr.Equals("")) && (!dbsourceid.Equals("")))
            {
                pWorkspace = GetWorkSpacefromConninfo(connstr, itype);
                return pWorkspace;
            }
            return pWorkspace;
        }
        public static void SetXmlAttrToFeaLayer(out IFeatureLayer  pFeatureLayer,IFeatureClass pFeaClass, XmlNode layerNode, IWorkspace pConfigWks)
        {
            ////��¼������DLG��DEM����DOM
            //string strDataType = XmlOperation.GetString(layerNode, "", "FeatureType", false);
            

            //����������
            XmlNode vNodeAboutShow = layerNode["AboutShow"];
            string strRenderKey = "";
            if (vNodeAboutShow != null)
            {
                strRenderKey = XmlOperation.GetString(vNodeAboutShow, "", "Renderer", false);
            }
            try
            {
                object objLayer = ModuleRenderer.GetLayerConfigFromBlob("ID='" + strRenderKey + "'", pConfigWks);
                if (objLayer != null)
                {
                    pFeatureLayer = objLayer as IFeatureLayer;
                    pFeatureLayer.FeatureClass = pFeaClass;
                    //�������ڵ���Ϊxml���ݸ�layer���description
                    ILayerGeneralProperties layerProerties = (ILayerGeneralProperties)pFeatureLayer;
                    layerProerties.LayerDescription = layerNode.OuterXml;

                    //����������
                    if (vNodeAboutShow != null)
                    {
                        double dMaxScale = XmlOperation.GetDouble(vNodeAboutShow, 0, "MaxScale", true);
                        double dMinScale = XmlOperation.GetDouble(vNodeAboutShow, 0, "MinScale", true);

                        pFeatureLayer.MaximumScale = dMaxScale;
                        pFeatureLayer.MinimumScale = dMinScale;

                    }
                    return;
                }
            }
            catch
            { }
            pFeatureLayer = null;
        }
        public static void SetXmlAttrToLayer(ILayer pLayer, XmlNode layerNode, IWorkspace pConfigWks)
        {
            //��¼������DLG��DEM����DOM
            string strDataType = XmlOperation.GetString(layerNode, "", "FeatureType", false);
            //�������ڵ���Ϊxml���ݸ�layer���description
            ILayerGeneralProperties layerProerties = (ILayerGeneralProperties)pLayer;
            layerProerties.LayerDescription = layerNode.OuterXml;

            //����������
            XmlNode vNodeAboutShow = layerNode["AboutShow"];
            string strRenderKey = "";
            if (vNodeAboutShow != null)
            {
                double dMaxScale = XmlOperation.GetDouble(vNodeAboutShow, 0, "MaxScale", true);
                double dMinScale = XmlOperation.GetDouble(vNodeAboutShow, 0, "MinScale", true);

                pLayer.MaximumScale = dMaxScale;
                pLayer.MinimumScale = dMinScale;

                strRenderKey = XmlOperation.GetString(vNodeAboutShow, "", "Renderer", false);
            }

            if (pLayer is  IRasterLayer)
            {
                // shduan 20110720 ȥ���ݿ��ж�ȡ���ţ����û�������Ĭ����ʾ����
                IRasterLayer pRLayer = pLayer as IRasterLayer;
                IRasterRenderer pRasterRenderer = ModuleRenderer.DoWithRasterRenderer(pConfigWks, strRenderKey, "", pRLayer);
                if (pRasterRenderer != null)
                {
                    pRLayer.Renderer = pRasterRenderer;
                }
                else
                {
                    //Ĭ��������ɫ �����l�������Ϊ͸��ɫ ����Ϊ��ɫ
                    ModuleRenderer.SetRasterDefaultNoDataColor(pLayer);
                    //if (strDataType == "DOM")
                    //{
                    //    IRasterRGBRenderer pRasterRGBRenderer = new RasterRGBRendererClass();
                    //    IRasterStretch2 pRasterStretch2 = pRasterRGBRenderer as IRasterStretch2;
                    //    //pRasterStretch2.Background = true;
                    //    //IColor pColor = new RgbColorClass();
                    //    //pColor.RGB = 0xFFFFFF;
                    //    //pRasterStretch2.BackgroundColor = pColor;

                    //    pRasterStretch2.StretchType = esriRasterStretchTypesEnum.esriRasterStretch_NONE;
                    //    pRasterStretch2.StretchStatsType = esriRasterStretchStatsTypeEnum.esriRasterStretchStats_Dataset;
                    //    IArray pArrayNew = new ArrayClass();
                    //    pArrayNew.Add(pRasterRGBRenderer);

                    //    pRLayer.Renderer = pRasterRGBRenderer as IRasterRenderer;
                    //}
                }
            }
            else if (pLayer is  IRasterCatalogLayer)
            {
                //Ĭ��������ɫ �����l�������Ϊ͸��ɫ ����Ϊ��ɫ
                ModuleRenderer.SetRasterDefaultNoDataColor(pLayer);
            }
            else if (pLayer is IFeatureLayer)
            {
                //���÷���
                
                IFeatureLayer pFeatureLayer = (IFeatureLayer)pLayer;
                
                if (!(pFeatureLayer is IGeoFeatureLayer)) return;
                IGeoFeatureLayer pGeoFeatLyr = pFeatureLayer as IGeoFeatureLayer;

                IFeatureRenderer pFRenderer = ModuleRenderer.DoWithRenderer(pConfigWks, strRenderKey, "", pFeatureLayer);
                if (pFRenderer != null)
                {
                    pGeoFeatLyr.Renderer = pFRenderer;
                }
                //��ʱդ��Ŀ¼ʱ������
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTRasterCatalogItem) return;

                if (vNodeAboutShow != null)
                {
                    //�Ƿ������������
                    pFeatureLayer.ScaleSymbols = XmlOperation.GetBoolean(vNodeAboutShow, false, "IsReferScale");
                    //ͼ��͸����
                    ILayerEffects pLayerEffects = pFeatureLayer as ILayerEffects;
                    pLayerEffects.Transparency = XmlOperation.GetShort(vNodeAboutShow, 0, "LayerTransparency", true);
                }
                //added by chulili 20110907  ����ͼ������Ҫ�ع���(�ع�����������ʾ��עʱ������Ҫ�ط���)
                if (layerNode["AttrLabel"] != null)
                {
                    bool HideSymbol = XmlOperation.GetBoolean(layerNode["AttrLabel"], false, "HideSymbol");
                    if (HideSymbol)
                    {
                        IFeatureRenderer pTmpRenderer = ModuleRenderer.HideSymbolOfLayer(pGeoFeatLyr);
                        pGeoFeatLyr.Renderer = pTmpRenderer;
                    }
                }
                //end added by chulili 
                //������ʽ
                if (layerNode["ShowDefine"] != null)
                {
                    if (XmlOperation.GetBoolean(layerNode["ShowDefine"], false, "IsDefineDisplay"))
                    {
                        IFeatureLayerDefinition featureLayerDefine = pFeatureLayer as IFeatureLayerDefinition;
                        string express = XmlOperation.GetString(layerNode["ShowDefine"], "", "Express", true);
                        featureLayerDefine.DefinitionExpression = express;
                    }
                }

                //���ñ�ע��Ϣ
                if (layerNode["AttrLabel"] != null)
                {
                    bool isLabel = XmlOperation.GetBoolean(layerNode["AttrLabel"], false, "IsLabel");
                    if (isLabel)
                    {
                        pGeoFeatLyr.DisplayAnnotation = true;
                        ModuleLabel.SetLayerInfoFromXml(pGeoFeatLyr);

                        //ModuleLabel.SetLayerInfoFromXml(pConfigWks, strRenderKey, pGeoFeatLyr);

                    }
                }
                try
                {
                    //ͳһ������䣬�߿�ע����ɫ
                    string fillColor = XmlOperation.GetString(vNodeAboutShow, "", "FillColor", true);
                    string borderColor = XmlOperation.GetString(vNodeAboutShow, "", "BorderColor", true);
                    if (string.IsNullOrEmpty(fillColor) == false && string.IsNullOrEmpty(borderColor) == false)
                    {
                        IRgbColor fillRGBColor = ModuleRenderer.GetRgbColorFromRgbString(fillColor);
                        IRgbColor borderRGBColor = ModuleRenderer.GetRgbColorFromRgbString(borderColor);
                        pGeoFeatLyr.Renderer = ModuleRenderer.SetColorOfRenderer(pGeoFeatLyr, fillRGBColor, borderRGBColor, -1, -1);
                    }
                    //����ע����ɫ
                    if (pFeatureLayer is IFDOGraphicsLayer)
                    {
                        string annoColor = XmlOperation.GetString(vNodeAboutShow, "", "AnnoColor", true);
                        if (string.IsNullOrEmpty(annoColor) == false)
                        {
                            ISymbolSubstitution annoSymbolSubstitution = pFeatureLayer as ISymbolSubstitution;
                            annoSymbolSubstitution.SubstituteType = esriSymbolSubstituteType.esriSymbolSubstituteColor;
                            annoSymbolSubstitution.MassColor = ModuleRenderer.GetRgbColorFromRgbString(annoColor);
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// ����һ��DimensionLayer��FDOGraphicsLayer��FeatureLayer��ͼ�㣬��֧��IFeatureLayer�ӿ�
        /// </summary>
        /// <returns></returns>
        public static IFeatureLayer GetNewFeatureLayer(IFeatureClass pFeatureClass)
        {
            IFeatureLayer pFeatureLayer;
            switch (pFeatureClass.FeatureType)
            {
                case esriFeatureType.esriFTAnnotation:          //ע�ǲ�
                    pFeatureLayer = (IFeatureLayer)(new FDOGraphicsLayer());
                    break;
                case esriFeatureType.esriFTDimension:           //��ע��
                    pFeatureLayer = (IFeatureLayer)(new DimensionLayer());
                    break;
                case esriFeatureType.esriFTRasterCatalogItem:   //Ӱ���
                    pFeatureLayer = (IFeatureLayer)(new GdbRasterCatalogLayer());
                    break;
                default:
                    pFeatureLayer = new FeatureLayer();
                    break;
            }
            pFeatureLayer.FeatureClass = pFeatureClass;
            return pFeatureLayer;
        }
        public static void SetLayersVisibleAttri(ESRI.ArcGIS.Controls.IMapControlDefault pMapcontrol,bool isVisible)
        {
            IMap pMap = pMapcontrol.Map;

            int iCount = pMapcontrol.LayerCount;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = pMap.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = pMap.get_Layer(iIndex) as IGroupLayer;
                
                if (groupTempLayer != null)
                {
                    SetLayersVisibleAttri(groupTempLayer, isVisible);
                    groupTempLayer.Visible = isVisible;
                }

                if (pFeatureLayer == null) continue;
                bool bNodeVisible = getVisibleOfLayerNode(pMap.get_Layer(iIndex));
                if (!isVisible)
                {
                    bNodeVisible = false;
                }
                pFeatureLayer.Visible = bNodeVisible;
            }
        }
        public static void SetLayersVisibleAttri(IMapDocument pMapdoc, bool isVisible)
        {
            if (pMapdoc.MapCount == 0)
            {
                return;
            }
            IMap pMap = pMapdoc.get_Map(0);

            int iCount = pMap.LayerCount;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = pMap.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = pMap.get_Layer(iIndex) as IGroupLayer;

                if (groupTempLayer != null)
                {
                    SetLayersVisibleAttri(groupTempLayer, isVisible);
                    groupTempLayer.Visible = isVisible;
                }

                if (pFeatureLayer == null) continue;
                bool bNodeVisible = getVisibleOfLayerNode(pMap.get_Layer(iIndex));
                if (!isVisible)
                {
                    bNodeVisible = false;
                }
                pFeatureLayer.Visible = bNodeVisible;
            }
        }
        public static bool getVisibleOfLayerNode(ILayer pLayer)
        {
            bool bVisible = false;
            ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
            //��ȡͼ���������ת��xml�ڵ�
            string strNodeXml = pLayerGenPro.LayerDescription;

            if (!strNodeXml.Equals(""))
            {
                XmlDocument pXmldoc = new XmlDocument();
                pXmldoc.LoadXml(strNodeXml);
                //��ȡ�ڵ��NodeKey��Ϣ
                XmlNode pxmlnode = pXmldoc.SelectSingleNode("//Layer");
                XmlElement pxmlEle = pxmlnode as XmlElement;

                if (pxmlEle != null)
                {
                    if (pxmlEle.HasAttribute("View"))
                    {
                        string strView = pxmlEle.GetAttribute("View");
                        if (strView.ToLower() == "true" || strView == "1")
                        {
                            bVisible = true;
                        }
                        else
                        {
                            bVisible = false;
                        }
                    }
                }
                pXmldoc = null;
            }
            return bVisible;
            

        }
        public static void SetLayersVisibleAttri(IGroupLayer groupLayer, bool isVisible)
        {
            ICompositeLayer comLayer = groupLayer as ICompositeLayer;
            int iCount = comLayer.Count;

            //��Dimension���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    SetLayersVisibleAttri(groupTempLayer, isVisible);
                }

                if (pFeatureLayer == null) break;
                pFeatureLayer.Visible = isVisible;
            }
        }
        #region ͼ������
        /// <summary>
        /// ��mapcontrol�ϵ�ͼ���������
        /// </summary>
        /// <param name="vAxMapControl"></param>
        public static void LayersComposeEx(ESRI.ArcGIS.Controls.IMapControlDefault pMapcontrol)
        {
            IMap pMap = pMapcontrol.Map;
            int[] iLayerIndex = new int[2] { 0, 0 };
            int[] iFeatureLayerIndex = new int[3] { 0, 0, 0 };

            int iCount = pMapcontrol.LayerCount;
            //��������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IGroupLayer groupTempLayeri = pMap.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayeri != null)
                {
                    LayersComposeEx(pMap, groupTempLayeri);
                }
            }
            //ͼ��������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                ILayer TempLayeri = pMap.get_Layer(iIndex) as ILayer;
                //LayersComposeEx(groupTempLayeri);
                for (int jindex = iIndex+1; jindex < iCount; jindex++)
                {

                    ILayer TempLayerj = pMap.get_Layer(jindex) as ILayer;
                    int iOrderi = -1;
                    int iOrderj = -1;
                    if (TempLayeri != null && TempLayerj != null)
                    {

                        string strOrderid_i = GetOrderIDofLayer(TempLayeri);
                        string strOrderid_j = GetOrderIDofLayer(TempLayerj);
                        if (!strOrderid_i.Equals("") && !strOrderid_j.Equals(""))
                        {
                            try
                            {
                                iOrderi = int.Parse(strOrderid_i);
                                iOrderj = int.Parse(strOrderid_j);

                            }
                            catch
                            { }
                        }
                        if (iOrderi >0 && iOrderj>0)
                        {
                            if (iOrderi > iOrderj)
                            {
                                pMap.MoveLayer(TempLayerj, iIndex);
                                TempLayeri = pMap.get_Layer(iIndex) as ILayer;
                            }
                        }
                        else
                        {
                            int intDataTypeID_i = GetDataTypeIDofLayer(TempLayeri);
                            int intDataTypeID_j = GetDataTypeIDofLayer(TempLayerj);
                            if (intDataTypeID_i > intDataTypeID_j)
                            {
                                pMap.MoveLayer(TempLayerj, iIndex);
                                TempLayeri = pMap.get_Layer(iIndex) as ILayer;
                            }
                        }
                    }
                }                
            }
        }
        public static int GetDataTypeIDofLayer(ILayer pLayer)
        {
            //����˳���ǣ�ע�ǣ�0 �㣺1 �ߣ�2  �棺3  դ��4
            if (pLayer is IGroupLayer)
                return 9;
            if (pLayer is IFeatureLayer)
            {
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                if(pFeatureLayer.FeatureClass!=null)
                {
                    switch (pFeatureLayer.FeatureClass.FeatureType)
                    {
                        case esriFeatureType.esriFTAnnotation:
                            return 0;
                            break;
                        case esriFeatureType.esriFTSimple:
                            switch (pFeatureLayer.FeatureClass.ShapeType)
                            {
                                case esriGeometryType.esriGeometryPoint:
                                case esriGeometryType.esriGeometryMultipoint:
                                    return 1;
                                    break;
                                case esriGeometryType.esriGeometryLine:
                                case esriGeometryType.esriGeometryPolyline:
                                    return 2;
                                    break;
                                case esriGeometryType.esriGeometryPolygon:
                                    return 3;
                                    break;
                            }
                            break;
                    }
                }

            }
            else if(pLayer is IRasterLayer)
            {
                return 4;
            }
            else if(pLayer is IRasterCatalogLayer)
            {
                return 4;
            }
            return 999;//��Ч����

        }
        //��Ҫ�����һ��ͼ�㣬�Ȼ�ȡ����Map��Ӧ������ʲôλ��
        //�������壺Mapcontrol�ؼ���ͼ��
        public static int GetIndexOfNewLayer(ESRI.ArcGIS.Controls.IMapControlDefault pMapcontrol,ILayer pLayer)
        {
            int intOrderID = -1;
            int intDataTypeID = -1;//Ϊ��ͬ���������ͷ��䲻ͬ����������������
            string strOrderID = GetOrderIDofLayer(pLayer);
            intDataTypeID = GetDataTypeIDofLayer(pLayer);
            if (!strOrderID.Equals(""))
            {
                intOrderID = int.Parse(strOrderID);
            }

            if (pLayer is IGroupLayer)
            {
                LayersComposeEx(pMapcontrol.Map, pLayer as IGroupLayer);
            }
            int IndexOfNewLayer = pMapcontrol.LayerCount;   //Ԥ������������
            for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)    //����������
            {
                ILayer pCurLayer = pMapcontrol.Map.get_Layer(i);
                string strCurOrderID = GetOrderIDofLayer(pCurLayer);
                int intCurOrderID = -1;
                if (!strCurOrderID.Equals(""))
                    intCurOrderID = int.Parse(strCurOrderID);
                if (intOrderID > 0 && intCurOrderID > 0)    //�����˳��ţ�����˳��ţ�˳���ԽС��ͼ����������
                {
                    if (intOrderID < intCurOrderID)//˳��űȵ�ǰͼ��С�����ڵ�ǰͼ�����棬�˳�ѭ������Ϊ�Ǵ��������ң��ҵ���һ���Ϳ����˳���
                    {
                        if (IndexOfNewLayer > i)
                        {
                            IndexOfNewLayer = i;
                            break;
                        }
                    }
                    else if (intOrderID > intCurOrderID)    //˳��űȵ�ǰͼ������ڵ�ǰͼ�������
                    {
                        if (IndexOfNewLayer < i)    //���������������
                        {
                            IndexOfNewLayer = i+1;
                        }
                    }
                }
                else    //���û��˳��ţ���ô�������ͣ�ע�ǲ����������棬���������ǵ㡢�ߡ��棬���ͱ��С����������
                {
                    int intCurDataTypeID = GetDataTypeIDofLayer(pCurLayer);
                    if (intDataTypeID < intCurDataTypeID)  //���ͺűȵ�ǰͼ��С�����ڵ�ǰͼ������� 
                    {
                        if (IndexOfNewLayer > i)
                        {
                            IndexOfNewLayer = i;
                            break;
                        }
                    }
                    else if (intDataTypeID > intCurDataTypeID)  //���ͺűȵ�ǰͼ������ڵ�ǰͼ�������
                    {
                        if (IndexOfNewLayer < i)    //���������������
                        {
                            IndexOfNewLayer = i+1;
                        }
                    }

                }

            }
            return IndexOfNewLayer;
        }
        public static void DealOrderOfNewLayer(ESRI.ArcGIS.Controls.IMapControlDefault pMapcontrol,ILayer pLayer)
        {
            int intOrderID = -1;
            int intDataTypeID = -1;//Ϊ��ͬ���������ͷ��䲻ͬ����������������
            string strOrderID = GetOrderIDofLayer(pLayer);
            intDataTypeID = GetDataTypeIDofLayer(pLayer);
            if (!strOrderID.Equals(""))
            {
                intOrderID = int.Parse(strOrderID);
            }
            
            if (pLayer is IGroupLayer)
            {
                LayersComposeEx(pMapcontrol.Map ,pLayer as IGroupLayer);
            }
            int IndexOfNewLayer = -1;
            for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)
            {
                ILayer pTmpLayer = pMapcontrol.Map.get_Layer(i);
                if (pTmpLayer.Equals(pLayer))
                {
                    if ((pTmpLayer as ILayerGeneralProperties).LayerDescription == (pLayer as ILayerGeneralProperties).LayerDescription)
                    {
                        IndexOfNewLayer = i;
                        break;
                    }
                }
            }
            for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)
            {
                ILayer pCurLayer = pMapcontrol.Map.get_Layer(i);
                string strCurOrderID = GetOrderIDofLayer(pCurLayer);
                int intCurOrderID = -1;
                if (!strCurOrderID.Equals(""))
                    intCurOrderID = int.Parse(strCurOrderID);
                if (intOrderID > 0 && intCurOrderID > 0)
                {
                    if (intOrderID < intCurOrderID)
                    {
                        if (IndexOfNewLayer > i)
                        {
                            pMapcontrol.Map.MoveLayer(pLayer, i);
                            IndexOfNewLayer = i;
                            break;
                        }
                    }
                    else if (intOrderID > intCurOrderID)
                    {
                        if (IndexOfNewLayer < i)
                        {
                            pMapcontrol.Map.MoveLayer(pLayer, i);
                            IndexOfNewLayer = i;
                        }
                    }
                }
                else
                {
                    int intCurDataTypeID = GetDataTypeIDofLayer(pCurLayer);
                    if (intDataTypeID < intCurDataTypeID)
                    {
                        if (IndexOfNewLayer > i)
                        {
                            pMapcontrol.Map.MoveLayer(pLayer, i);
                            IndexOfNewLayer = i;
                            break;
                        }
                    }
                    else if (intDataTypeID > intCurDataTypeID)
                    {
                        if (IndexOfNewLayer < i)
                        {
                            pMapcontrol.Map.MoveLayer(pLayer, i);
                            IndexOfNewLayer = i;
                        }
                    }

                }

            }
        }
        //added by chulili 20110731 ��ȡͼ��˳���
        public static string  GetOrderIDofLayer(ILayer pLayer)
        {
            try
            {
                ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                //��ȡͼ�������
                string strNodeXml = pLayerGenPro.LayerDescription;
                XmlDocument pXmlDoc = new XmlDocument();
                pXmlDoc.LoadXml(strNodeXml);
                //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
                XmlNode pNode = pXmlDoc.ChildNodes[0];
                string strOrder = "";
                try
                {
                    strOrder = pNode.Attributes["OrderID"].Value.ToString();
                }
                catch
                { }
                return strOrder;
            }
            catch
            { 
            }
            return "";

        }
        /// <summary>
        /// ��mapcontrol��groupLayer�ڵ�ͼ���������
        /// </summary>
        /// <param name="groupLayer"></param>
        public static void LayersComposeEx(IMap pMap, IGroupLayer groupLayer)
        {
            //�жϲ�����Ч��
            if (pMap == null) return;
            if (groupLayer == null) return;
            ICompositeLayer comLayer = groupLayer as ICompositeLayer;
            int iCount = comLayer.Count;
            IMapLayers pMapLayers = pMap as IMapLayers;
            //��Dimension��������� ð������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                ILayer TempLayeri = comLayer.get_Layer(iIndex) as ILayer;
                for (int jindex = iIndex + 1; jindex < iCount; jindex++)
                {

                    ILayer TempLayerj = comLayer.get_Layer(jindex) as ILayer;
                    if (TempLayeri != null && TempLayerj != null)
                    {
                        //��ȡͼ��˳���
                        string strOrderid_i = GetOrderIDofLayer(TempLayeri);
                        string strOrderid_j = GetOrderIDofLayer(TempLayerj);
                        int iOrderi = -1;
                        int iOrderj = -1;

                        if (!strOrderid_i.Equals("") && !strOrderid_j.Equals(""))
                        {
                            try
                            {
                                iOrderi = int.Parse(strOrderid_i);
                                iOrderj = int.Parse(strOrderid_j);
                            }
                            catch
                            { }
                        }
                        if (iOrderi>0 &&  iOrderj>0)
                        {
                            if (iOrderi > iOrderj)
                            {
                                groupLayer.Delete(TempLayerj);
                                pMapLayers.InsertLayerInGroup(groupLayer, TempLayerj, false, iIndex);
                                TempLayeri = comLayer.get_Layer(iIndex) as ILayer;
                            }

                        }
                        else
                        {
                            int intDataTypeID_i = GetDataTypeIDofLayer(TempLayeri);
                            int intDataTypeID_j = GetDataTypeIDofLayer(TempLayerj);
                            if (intDataTypeID_i > intDataTypeID_j)
                            {
                                groupLayer.Delete(TempLayerj);
                                pMapLayers.InsertLayerInGroup(groupLayer, TempLayerj, false, iIndex);
                                TempLayeri = comLayer.get_Layer(iIndex) as ILayer;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region ͼ������
        /// <summary>
        /// ��mapcontrol�ϵ�ͼ���������
        /// </summary>
        /// <param name="vAxMapControl"></param>
        public static void LayersCompose(ESRI.ArcGIS.Controls.IMapControlDefault pMapcontrol)
        {
            IMap pMap = pMapcontrol.Map;
            int[] iLayerIndex = new int[2] { 0, 0 };
            int[] iFeatureLayerIndex = new int[3] { 0, 0, 0 };

            int iCount = pMapcontrol.LayerCount;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = pMap.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = pMap.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }

                if (pFeatureLayer == null) continue ;
                switch (pFeatureLayer.FeatureClass.FeatureType)
                {
                    case esriFeatureType.esriFTDimension:
                        pMap.MoveLayer(pFeatureLayer, iLayerIndex[0]);
                        iLayerIndex[0] = iLayerIndex[0] + 1;
                        break;
                    case esriFeatureType.esriFTAnnotation:
                        pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1]);
                        iLayerIndex[1] = iLayerIndex[1] + 1;
                        break;
                    case esriFeatureType.esriFTSimple:
                        switch (pFeatureLayer.FeatureClass.ShapeType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                                pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0]);
                                iFeatureLayerIndex[0] = iFeatureLayerIndex[0] + 1;
                                break;
                            case esriGeometryType.esriGeometryLine:
                            case esriGeometryType.esriGeometryPolyline:
                                pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0] + iFeatureLayerIndex[1]);
                                iFeatureLayerIndex[1] = iFeatureLayerIndex[1] + 1;
                                break;
                            case esriGeometryType.esriGeometryPolygon:
                                pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0] + iFeatureLayerIndex[1] + iFeatureLayerIndex[2]);
                                iFeatureLayerIndex[2] = iFeatureLayerIndex[2] + 1;
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// ��mapcontrol��groupLayer�ڵ�ͼ���������
        /// </summary>
        /// <param name="groupLayer"></param>
        public static void LayersCompose(IGroupLayer groupLayer)
        {
            ICompositeLayer comLayer = groupLayer as ICompositeLayer;
            int iCount = comLayer.Count;

            List<ILayer> listLays = new List<ILayer>();
            //��Dimension���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }

                if (pFeatureLayer == null) continue ;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTDimension)
                {
                    listLays.Add(pFeatureLayer as ILayer);
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //��Annotation���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    listLays.Add(pFeatureLayer as ILayer);
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //�Ե���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        listLays.Add(pFeatureLayer as ILayer);
                    }
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //���߲��������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryLine || pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        listLays.Add(pFeatureLayer as ILayer);
                    }
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = new List<ILayer>();
            //���������
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = comLayer.get_Layer(iIndex) as IFeatureLayer;
                IGroupLayer groupTempLayer = comLayer.get_Layer(iIndex) as IGroupLayer;
                if (groupTempLayer != null)
                {
                    LayersCompose(groupTempLayer);
                }
                if (pFeatureLayer == null) break;
                if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        listLays.Add(pFeatureLayer as ILayer);
                    }
                }
            }
            foreach (ILayer pTempLay in listLays)
            {
                groupLayer.Delete(pTempLay);
                groupLayer.Add(pTempLay);
            }

            listLays = null;
        }
        #endregion

        /// <summary>
        /// ��Blob�ֶ��л�ȡImap����
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="querySql"></param>
        /// <returns></returns>
        public static void GetMapFromBlob(IWorkspace workspace, string tableName, string fieldName, string querySql, IMap map)
        {
            //try
            //{
            //    byte[] mapByte = GeoGISBase.ModuleDatabase.ReadByteFromBlob(workspace, tableName, querySql, fieldName);

            //    //�Ƿ�õ������л���blob���򷵻�NULL
            //    if (mapByte == null || mapByte.Length == 0) return;
            //    IMemoryBlobStreamVariant pMemoryBlobStreamVariant = new MemoryBlobStreamClass();
            //    pMemoryBlobStreamVariant.ImportFromVariant((object)mapByte);
            //    IStream pStream = pMemoryBlobStreamVariant as IStream;
            //    IPersistStream pPersistStream = map as IPersistStream;
            //    pPersistStream.Load(pStream);
            //    map = pPersistStream as IMap;
            //}
            //catch
            //{
            //    return;
            //}
        }

        //�������� �û����������ͼ�Ĺ����ռ�
        public static IDictionary<string, IWorkspace> _DicDataLibWks = new Dictionary<string, IWorkspace>();


        //changed by chulili 20110922 ��Ϊȫ�ֱ����ŵ�SysCommon��������
        //private  static bool _IsLayerTreeChanged = false;//added by chulili 20110701 ��ʾͼ��Ŀ¼�����Ƿ������޸�
        //public static bool IsLayerTreeChanged
        //{
        //    set { _IsLayerTreeChanged = value; }
        //    get { return _IsLayerTreeChanged; }
        //}
        //end changed by chulili 20110922
    }
}
