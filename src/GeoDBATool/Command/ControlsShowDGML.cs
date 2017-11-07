using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Runtime;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

namespace GeoDBATool
{
    /// <summary>
    /// ���Ƿ����
    /// </summary>
    public class ControlsShowDGML : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        Plugin.Application.IAppFormRef v_AppForm;
        private Dictionary<string, List<string>> DicRequiedFIeld = new Dictionary<string, List<string>>();//��¼ע�ǵ��û��Զ�����ֶ�

        public ControlsShowDGML()
        {
            base._Name = "GeoDBATool.ControlsShowDGML";
            base._Caption = "�鿴DGML����";
            base._Tooltip = "�鿴DGML����";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�鿴DGML����";

        }

        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.CurrentThread != null) return false;
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            string[] pDocPath = null;   //����DGML�ĵ�·��
            ///ѡ��DGML�ĵ�
            pDocPath = SelectDocument("xml");
            if (pDocPath == null || pDocPath.Length == 0) return;
            //��ȡDGML�ĵ������ĵ��е�Ҫ�����ݣ�д����ʱ�����ռ���
            if (!ImportContnent(pDocPath))
            {
                return;
            }
            //���¶Ա��б���ʾ
            ShowUpdateGrid(pDocPath);
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppGISRef;
            v_AppForm = m_Hook as Plugin.Application.IAppFormRef;
        }

        #region ���� ��DGML�ļ�������ArcMap����ʾ����

        /// <summary>
        /// ����DGML�����ݵ���Ҫ��Ҫ��ʱ�ռ���    ���Ƿɱ�д    20091225 �޸�
        /// </summary>
        /// <param name="pDocPath">�ĵ�·��</param>
        /// <returns>�Ƿ���ɹ�</returns>
        private bool ImportContnent(string[] pDocPath)
        {
            try
            {
                //��ȡxml�ĵ�
                XmlDocument tempDGMLxml = new XmlDocument();
                tempDGMLxml.Load(pDocPath[0]);

                //��ʾ������
                ShowProgressBar(true);

                #region ����Workspace
                Exception eError = null;
                //���pdb��·��
                int index = pDocPath[0].LastIndexOf("\\");
                string dbPurePath = pDocPath[0].Substring(0, index);
                string dbPureName = GetPureName(pDocPath[0]);
                string dbPath = dbPurePath + "\\" + dbPureName + ".mdb";//�����xmlĿ¼��ͬ��pdb��·��
                //string prjFile = dbPurePath + "\\" + dbPureName + ".prj";//���ͬĿ¼�µĿռ�ο��ļ�

                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
                //����Workspace
                IWorkspace pWorkSpace = CreateWorkspace(dbPath, "PDB", out eError);
                //IWorkspace pWorkSpace = CreateTempWorkspace(out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���������ռ����");
                    return false;
                }
                IFeatureWorkspace pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;
                #endregion
                //������¼������ͼ����
                List<string> DatasetsName = new List<string>();
                for (int i = 0; i < pDocPath.Length; i++)
                {
                    XmlDocument DGMLxml = new XmlDocument();
                    DGMLxml.Load(pDocPath[i]);
                    //XmlNodeList pLogicFeaNodeList = DGMLxml.SelectNodes(".//DGML//Data//LogicFeature");//����ʵ��ڵ� 

                    #region ��������ṹ
                    //����Ԫ��Ϣ��ȡ������
                    XmlElement ProNode = DGMLxml.SelectSingleNode(".//DGML//MetaInfo//ProjectInfo") as XmlElement;
                    if (ProNode == null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ĵ���DGML�ṹ�ĵ���һ�£���ѡ����ȷ��DGML�ĵ���");
                        return false;
                    }
                    int pScale = int.Parse(ProNode.GetAttribute("Scale").Trim());//��xml�л�ȡ������  
                    //������ݽṹDataStruct�ڵ�
                    XmlNode dataStructNode = DGMLxml.SelectSingleNode(".//DGML//DataStruct");
                    //��������ṹ
                    if (!CreateDataBase(dataStructNode, pFeatureWorkSpace, pScale, DatasetsName, out eError))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                        return false;
                    }
                    #endregion
                    # region �����ݵ��뵽��������ȥ
                    //��ÿ��������е�Ҫ����
                    //List<IDataset> ListFeaCls = new List<IDataset>();
                    //ListFeaCls = GetAllFeatureClass(pWorkSpace);
                    //�������Data�ڵ�
                    XmlNode dataNode = DGMLxml.SelectSingleNode(".//DGML//Data");
                    if (!ImportAllData(dataNode, pWorkSpace, out eError))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                        return false;
                    }
                    #endregion
                }
                ShowErrForm("��ʾ", "�����ɹ�!");
                ShowStatusTips("������");
                //���ؽ�����
                ShowProgressBar(false);
                #region ��Ҫ����ͼ�������ص�arcMap����
                //����oldData��IGroupLayer
                IGroupLayer pOldGroupLayer = new GroupLayerClass();
                pOldGroupLayer.Name = "OldData";
                //����newData��IGroupLayer
                IGroupLayer pNewGroupLayer = new GroupLayerClass();
                pNewGroupLayer.Name = "NewData";
                //��ÿ��������е�Ҫ����
                List<IDataset> ListFeaCls = new List<IDataset>();
                ListFeaCls = GetAllFeatureClass(pWorkSpace);
                foreach (IDataset pdatast in ListFeaCls)
                {
                    try
                    {
                        IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                        //��ȡ����
                        string feaName = pdatast.Name;
                        IFeatureClass pFeatureClass = pdatast as IFeatureClass;
                        if (pFeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            pFeatureLayer = new FDOGraphicsLayerClass();
                        }
                        pFeatureLayer.FeatureClass = pFeatureClass;
                        ILayer mLayer = pFeatureLayer as ILayer;
                        mLayer.Name = feaName;
                        if (feaName.Substring(feaName.Length - 2) == "_t")
                        {
                            pNewGroupLayer.Add(mLayer);
                        }
                        else
                        {
                            pOldGroupLayer.Add(mLayer);
                        }
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
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", "����ͼ��ʧ�ܣ�");
                        return false;
                    }
                }

                m_Hook.MapControl.Map.AddLayer(pOldGroupLayer as ILayer);
                m_Hook.MapControl.Map.AddLayer(pNewGroupLayer as ILayer);
                //��ͼ���������
                SysCommon.Gis.ModGisPub.LayersCompose(m_Hook.MapControl);
                m_Hook.TOCControl.Update();
                m_Hook.MapControl.ActiveView.Refresh();
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                //********************************************************************
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                //���ؽ�����
                ShowProgressBar(false);
                return false;
            }
        }

        /// <summary>
        /// ѡ���ĵ�
        /// </summary>
        /// <param name="pDocPath">�ĵ�·��</param>
        /// <param name="DocType">�ĵ�����</param>
        /// <returns>�Ƿ��Ѿ�ѡ��</returns>
        private string[] SelectDocument(string DocType)
        {
            string[] pDocPath = null;
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Title = "ѡ���ļ�";
            fileDlg.Filter = "�ļ�(*." + DocType + ")|*." + DocType;
            fileDlg.Multiselect = true;
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                pDocPath = fileDlg.FileNames;
            }
            return pDocPath;
        }

        /// <summary>
        /// ����PDB��GDB������  ���Ƿ����
        /// </summary>
        /// <param name="sFilePath">�ļ�·��</param>
        /// <param name="wstype">����������</param>
        /// <returns>�������Exception</returns>
        private IWorkspace CreateWorkspace(string sFilePath, string strType, out Exception eError)
        {
            try
            {
                eError = null;
                IWorkspace TempWorkSpace = null;
                IWorkspaceFactory pWorkspaceFactory = null;
                if (strType.Trim().ToUpper() == "PDB")
                {
                    pWorkspaceFactory = new AccessWorkspaceFactoryClass();
                }
                else if (strType.Trim().ToUpper() == "GDB")
                {
                    pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                }

                if (!File.Exists(sFilePath))
                {
                    FileInfo finfo = new FileInfo(sFilePath);
                    string outputDBPath = finfo.DirectoryName;
                    string outputDBName = finfo.Name;
                    outputDBName = outputDBName.Substring(0, outputDBName.Length - 4);
                    IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(outputDBPath, outputDBName, null, 0);
                    IName pName = (IName)pWorkspaceName;
                    TempWorkSpace = (IWorkspace)pName.Open();
                }
                else
                {
                    TempWorkSpace = pWorkspaceFactory.OpenFromFile(sFilePath, 0);
                }
                return TempWorkSpace;
            }
            catch (Exception eX)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(eX, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(eX, null, DateTime.Now);
                }
                //********************************************************************
                eError = eX;
                return null;
            }
        }
        /// <summary>
        /// �����ڴ湤���ռ�
        /// </summary>
        /// <param name="eError"></param>
        /// <returns></returns>
        private IWorkspace CreateTempWorkspace(out Exception eError)
        {
            try
            {
                eError = null;
                IWorkspace TempWorkSpace = null;
                IWorkspaceFactory pWorkspaceFactory = new InMemoryWorkspaceFactoryClass();

                IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create("", "tempWorkSpace", null, 0);
                IName pName = (IName)pWorkspaceName;
                TempWorkSpace = (IWorkspace)pName.Open();

                return TempWorkSpace;
            }
            catch (Exception eX)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(eX, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(eX, null, DateTime.Now);
                }
                //********************************************************************
                eError = eX;
                return null;
            }
        }
        #region new Function Design 20091225 ���Ƿ����
        /// <summary>
        /// �����е����ݵ��뵽�����Ŀ���֮��   ���Ƿ�  20091225  ���
        /// </summary>
        /// <param name="DataNode"></param>
        /// <param name="ListFeas"></param>
        /// <returns></returns>
        private bool ImportAllData(XmlNode DataNode, IWorkspace pWorkSpace, out Exception eError)
        {
            eError = null;
            Exception ex = null;
            XmlNodeList logicFeaNodeList = DataNode.SelectNodes(".//LogicFeature");
            foreach (XmlNode logicFeaNode in logicFeaNodeList)
            {
                XmlNodeList recordNodeList = logicFeaNode.SelectNodes(".//Record");

                //����״̬����ֵ
                ShowStatusTips("����ʵ��" + (logicFeaNode as XmlElement).GetAttribute("Name").Trim() + "�е����ݿ�ʼ���");
                //���ó�ʼ������
                int tempValue = 0;
                ChangeProgressBar(v_AppForm.ProgressBar, 0, recordNodeList.Count, tempValue);

                foreach (XmlNode RecordNode in recordNodeList)
                {
                    string newFCName = RecordNode.SelectSingleNode(".//NEWFCNAME").InnerText.Trim();//��ͼ����
                    string oldFCName = RecordNode.SelectSingleNode(".//OLDFCNAME").InnerText.Trim();//ԭʼͼ����
                    int pGOFID = int.Parse(RecordNode.SelectSingleNode(".//GOFID").InnerText.Trim());//GOFID
                    string state = RecordNode.SelectSingleNode(".//STATE").InnerText.Trim();//״̬
                    if (state != "ɾ��")
                    {
                        XmlNode newFeaNode = RecordNode.SelectSingleNode(".//NEWFEATURE");
                        if (newFCName == "" || newFeaNode == null)
                        {
                            eError = new Exception("�½����޸ĵ�Ҫ�ص�ͼ������NEWFEATURE�ڵ㲻��Ϊ�գ�");
                            return false;
                        }
                        if (!ImprotData(newFCName, pWorkSpace, newFeaNode, out ex))
                        {
                            eError = ex;
                            return false;
                        }
                    }
                    if (state != "�½�")//���½�������
                    {
                        XmlNode oldFeaNode = RecordNode.SelectSingleNode(".//OLDFEATURE");
                        if (oldFCName == "" || oldFeaNode == null)
                        {
                            eError = new Exception("ɾ�����޸ĵ�Ҫ�ص�ͼ������OLDFEATURE�ڵ㲻��Ϊ�գ�");
                            return false;
                        }
                        if (!ImprotData(oldFCName, pWorkSpace, oldFeaNode, out ex))
                        {
                            eError = ex;
                            return false;
                        }
                    }
                    tempValue += 1;
                    ChangeProgressBar(v_AppForm.ProgressBar, -1, -1, tempValue);
                }
            }
            return true;
        }
        /// <summary>
        /// ����xml�����ݵ��뵽�����Ŀ���֮��  ���Ƿ�  2009 12 25   ��� 
        /// </summary>
        /// <param name="pFeatureClsName"></param>
        /// <param name="ListFeas"></param>
        /// <param name="featureNode"></param>
        /// <returns></returns>
        private bool ImprotData(string pFeatureClsName, IWorkspace pWorkSpace, XmlNode featureNode, out Exception eError)
        {
            //bool bb = false;
            //foreach (IDataset pDataset in ListFeas)
            //{
            //    if (pFeatureClsName == pDataset.Name.Trim())
            //    {
            //bb = true;
            eError = null;

            try
            {
                IFeatureClass pfeatureClass = (pWorkSpace as IFeatureWorkspace).OpenFeatureClass(pFeatureClsName);
                //IEnumDataset pEnumDataST = pWorkSpace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                //IDataset pDT= pEnumDataST.Next();
                //while  (pDT != null)
                //{
                //    string s = pDT.Name;
                //    pDT = pEnumDataST.Next();
                //}

                // ����Ҫ��
                IFeature pFeature = pfeatureClass.CreateFeature();
                XmlNodeList valueNodeList = featureNode.SelectNodes(".//Value");
                #region ����Feature��ÿһ���ֶνڵ㲢����ֵ
                int pAOID = -1; ;
                foreach (XmlNode valueNode in valueNodeList)
                {
                    int fieldIndex = -1;
                    string fieldName = valueNode.SelectSingleNode(".//FieldName").InnerText.Trim();//�ֶ�����
                    string fieldvalue = valueNode.SelectSingleNode(".//FieldValue").InnerText.Trim();//�ֶ�ֵ
                    if (fieldvalue == "") continue;
                    fieldIndex = pFeature.Fields.FindField(fieldName);//�ֶ�����
                    if (fieldIndex == -1) continue;

                    if (fieldName == "OBJECTID")
                    {
                        pAOID = int.Parse(fieldvalue);
                    }
                    IField pField = pFeature.Fields.get_Field(fieldIndex);
                    if (pField.Editable == false) continue;
                    # region �ж��ֶε����Ͳ���������ת��,����ֵ
                    //��ͨ���ֶ�ֱ�Ӹ�ֵ
                    if (pField.Type != esriFieldType.esriFieldTypeGeometry && pField.Type != esriFieldType.esriFieldTypeBlob)
                    {

                        pFeature.set_Value(fieldIndex, fieldvalue as object);
                    }
                    #region �������ֶν��н���
                    if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeBlob)
                    {
                        try
                        {
                            //���ַ�����ԭΪ�ֽ�
                            //char[] c = new char[] { ',' };
                            //string[] xmlstring = fieldvalue.Split(c);
                            byte[] xmlByte = Convert.FromBase64String(fieldvalue);
                            //for (int i = 0; i < xmlstring.Length; i++)
                            //{
                            //    byte b = Convert.ToByte(Convert.ToInt32(xmlstring[i]));
                            //    xmlByte[i] = b;
                            //}
                            object fieldShape = null;//��������������ֵ
                            //������Ķ�������ݲ�ͬ��Ҫ�������ͣ��ò�ͬ���������г�ʼ��
                            if (pfeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                //ע�������������ֶ���Ҫ����
                                if (pField.Type == esriFieldType.esriFieldTypeGeometry)
                                {
                                    fieldShape = new PolygonElementClass();
                                }
                                else if (pField.Type == esriFieldType.esriFieldTypeBlob)
                                {
                                    fieldShape = new TextElementClass();
                                }
                            }
                            else if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                            {
                                fieldShape = new PointClass();
                            }
                            else if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                            {
                                fieldShape = new PolylineClass();
                            }
                            else if (pfeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                            {
                                fieldShape = new PolygonClass();
                            }

                            //�������󣬲����ֶθ�ֵ
                            if (XmlDeSerializer(xmlByte, fieldShape) == true)
                            {
                                if (pField.Type == esriFieldType.esriFieldTypeGeometry)
                                {
                                    pFeature.set_Value(fieldIndex, fieldShape);
                                    //pFeature.Shape = fieldShape as IGeometry;
                                }
                                else if (pField.Type == esriFieldType.esriFieldTypeBlob)
                                {
                                    IAnnotationFeature pAnnoFeature = pFeature as IAnnotationFeature;
                                    pAnnoFeature.Annotation = fieldShape as IElement;
                                }
                            }
                            else
                            {
                                break;
                            }
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
                            eError = new Exception("�����ֶν�������");
                            return false;
                        }
                    }
                    #endregion
                    #endregion
                }
                //��OID��ֵд�����ݿ�����ȥ����һ���ֶα�������
                if (pAOID == -1)
                {
                    eError = new Exception("OBJECTID�ֶ�ֵ����ȷ��");
                    return false;
                }
                int fIndex = pFeature.Fields.FindField("AOID");
                if (fIndex == -1)
                {
                    eError = new Exception("�ֶ�AOID�����ڣ�");
                    return false;
                }
                pFeature.set_Value(fIndex, pAOID as object);
                pFeature.Store();
                #endregion
            }
            catch (Exception ex)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                //********************************************************************
                eError = ex;
                return false;
            }
            //break;
            //    }
            //}
            //if (bb == false)
            //{
            //�Ҳ���������ͬ��Ҫ����
            //return false;
            //}
            return true;
        }

        /// <summary>
        /// ����XML��������ṹ  ���Ƿ�  20091225 ���
        /// </summary>
        /// <param name="dataStructNode"></param>
        /// <param name="feaworkspace"></param>
        /// <param name="intScale"></param>
        /// <param name="prjFile"></param>
        /// <returns></returns>
        private bool CreateDataBase(XmlNode dataStructNode, IFeatureWorkspace feaworkspace, int intScale, List<string> DatasetsName, out Exception eError)
        {
            //��ȡ�����ռ��µ����е�ͼ����
            //List<string> DatasetsName = new List<string>();
            //IEnumDatasetName pEnumDatasetName = pWorkSpace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
            //IDatasetName pDatasetName = pEnumDatasetName.Next();
            //while (pDatasetName != null)
            //{
            //    DatasetsName.Add(pDatasetName.Name);
            //    DatasetsName = pEnumDatasetName.Next();
            //}
            //return DatasetsName;
            eError = null;
            Exception ex = null;
            string feaClsName = "";
            XmlNodeList oldFeaClsNodeList = dataStructNode.SelectNodes(".//oldFeatureClass//FeatureClass");
            foreach (XmlNode FeaClsNode in oldFeaClsNodeList)
            {
                feaClsName = (FeaClsNode as XmlElement).GetAttribute("Name").Trim();//Ҫ��������
                if (!DatasetsName.Contains(feaClsName))
                {
                    DatasetsName.Add(feaClsName);
                    if (!CreateFeatureClass(FeaClsNode, feaworkspace, intScale, out ex))
                    {
                        eError = ex;
                        return false;
                    }
                }
                else
                {
                    continue;
                }
            }

            XmlNodeList newFeaClsNodeList = dataStructNode.SelectNodes(".//newFeatureClass//FeatureClass");
            foreach (XmlNode FeaClsNode in newFeaClsNodeList)
            {
                feaClsName = (FeaClsNode as XmlElement).GetAttribute("Name").Trim();//Ҫ��������
                if (!DatasetsName.Contains(feaClsName))
                {
                    DatasetsName.Add(feaClsName);
                    if (!CreateFeatureClass(FeaClsNode, feaworkspace, intScale, out ex))
                    {
                        eError = ex;
                        return false;
                    }
                }
                else
                {
                    continue;
                }
            }
            return true;
        }
        /// <summary>
        /// ����ͼ��ṹ��������ͨҪ�ز��ע�ǲ�  ���Ƿ�  20091225  ���
        /// </summary>
        /// <param name="FeaClsNode">ͼ��ڵ�</param>
        /// <param name="feaworkspace"></param>
        /// <param name="intScale"></param>
        /// <param name="prjFile"></param>
        /// <returns></returns>
        private bool CreateFeatureClass(XmlNode FeaClsNode, IFeatureWorkspace feaworkspace, int intScale, out Exception eError)
        {
            eError = null;
            try
            {
                string feaClsName = (FeaClsNode as XmlElement).GetAttribute("Name").Trim();//Ҫ��������
                string shapeType = (FeaClsNode as XmlElement).GetAttribute("ShapeType").Trim();//Ҫ�������ͣ��㣬�ߣ���
                XmlNodeList fieldNodeList = FeaClsNode.SelectNodes(".//Field");
                XmlNode geoNode = null;
                #region �����ֶβ������ֶε�����
                IFields fields = new FieldsClass();
                IFieldsEdit fsEdit = fields as IFieldsEdit;
                #region �����û��Զ�����ֶ�
                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    //bool annofield = false;//���ж�ע�ǵı�Ҫ�ֶ�ʱ����Ϊ��־����ѭ��
                    if (fieldNode["Name"].InnerText.ToLower() == "shape")
                    {
                        geoNode = fieldNode.SelectSingleNode(".//GeometryDef");
                        if (geoNode == null)
                        {
                            eError = new Exception("GeometryDef�ڵ㲻���ڣ�");
                            return false;
                        }
                    }
                    if (shapeType == "��")
                    {
                        if (fieldNode["Name"].InnerText == "OBJECTID" || fieldNode["Name"].InnerText.ToLower() == "shape")
                        {
                            continue;
                        }
                    }
                    if (shapeType == "��")
                    {
                        if (fieldNode["Name"].InnerText == "OBJECTID" || fieldNode["Name"].InnerText.ToLower() == "shape" || fieldNode["Name"].InnerText.ToLower() == "shape_length")
                        {
                            continue;
                        }
                    }
                    if (shapeType == "��")
                    {
                        if (fieldNode["Name"].InnerText == "OBJECTID" || fieldNode["Name"].InnerText.ToLower() == "shape" || fieldNode["Name"].InnerText.ToLower() == "shape_length" || fieldNode["Name"].InnerText.ToLower() == "shape_area")
                        {
                            continue;
                        }
                    }
                    if (shapeType == "ע��")//&& fieldNode["Required"].InnerText.ToLower()=="true")
                    {
                        //continue;
                        bool annofield = false;//���ж�ע�ǵı�Ҫ�ֶ�ʱ����Ϊ��־����ѭ��
                        //if (fieldNode["Required"].InnerText.ToLower() == "false")
                        //{

                        ////�ж�������ֶ���ע�ǵı�Ҫ�ֶΣ�������
                        IObjectClassDescription pOCDesc = new AnnotationFeatureClassDescription();
                        IFields pfields = pOCDesc.RequiredFields;
                        for (int k = 0; k < pfields.FieldCount; k++)
                        {

                            if (pfields.get_Field(k).Name.ToLower() == fieldNode["Name"].InnerText.ToLower())
                            {
                                annofield = true;
                                break;
                            }
                        }
                        if (annofield == true)
                        {
                            continue;
                        }
                        //if (annofield == true) continue;
                        //else
                        //{
                        //    if (!DicRequiedFIeld.ContainsKey(feaClsName))
                        //    {
                        //        List<string> temp = new List<string>();
                        //        temp.Add(fieldNode["Name"].InnerText);
                        //        DicRequiedFIeld.Add(feaClsName, temp);//����ע�ǵ�Ҫ�������ƺͷǱ����ֶ�����
                        //    }
                        //    else
                        //    {
                        //        DicRequiedFIeld[feaClsName].Add(fieldNode["Name"].InnerText);
                        //    }
                        //}
                        //}
                    }
                    //���±������������ֶε�����
                    string fieldName = "";//��¼�ֶ�����
                    string fieldType = "";//��¼�ֶ�����
                    int fieldLen;//��¼�ֶγ���
                    bool isNullable = true;//��¼�ֶ��Ƿ������ֵ
                    int precision = 0;//����
                    int scale = 0;
                    bool required = false;
                    bool editable = true;
                    bool domainfixed = false;
                    //����ֶε�����
                    fieldName = fieldNode["Name"].InnerText;
                    fieldType = fieldNode["Type"].InnerText;
                    isNullable = bool.Parse(fieldNode["IsNullable"].InnerText);
                    fieldLen = Convert.ToInt32(fieldNode["Length"].InnerText);
                    precision = Convert.ToInt32(fieldNode["Precision"].InnerText);
                    scale = Convert.ToInt32(fieldNode["Scale"].InnerText);
                    required = false;

                    editable = bool.Parse(fieldNode["Editable"].InnerText);
                    domainfixed = bool.Parse(fieldNode["DomainFixed"].InnerText.ToLower());

                    //�����û��Զ�����ֶ�
                    IField newfield = new FieldClass();
                    IFieldEdit fieldEdit = newfield as IFieldEdit;
                    fieldEdit.Name_2 = fieldName;
                    fieldEdit.AliasName_2 = fieldName;
                    //�ֶ�����Ҫװ��Ϊö������
                    fieldEdit.Type_2 = (esriFieldType)Enum.Parse(typeof(esriFieldType), fieldType, true);
                    fieldEdit.IsNullable_2 = isNullable;
                    fieldEdit.Length_2 = fieldLen;
                    fieldEdit.Precision_2 = precision;
                    fieldEdit.Scale_2 = scale;
                    fieldEdit.Required_2 = required;
                    fieldEdit.Editable_2 = editable;
                    fieldEdit.DomainFixed_2 = domainfixed;
                    newfield = fieldEdit as IField;
                    fsEdit.AddField(newfield);
                }
                #endregion
                //����һ������OID�ֶΣ��Ա�������λ
                IField ppField = new FieldClass();
                IFieldEdit ppFieldEdit = ppField as IFieldEdit;
                ppFieldEdit.Name_2 = "AOID";//�ֶ�����
                ppFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                ppFieldEdit.Length_2 = 4;
                ppFieldEdit.Editable_2 = true;
                ppField = ppFieldEdit as IField;
                fsEdit.AddField(ppField);

                if (shapeType == "ע��")
                {
                    //����ע�ǲ�������ֶ�
                    if (!createAnnoFeatureClass(feaClsName, feaworkspace, fsEdit, intScale, shapeType, geoNode))
                    {
                        eError = new Exception("����ע�ǲ�ʧ�ܣ�");
                        return false;
                    }
                }
                else
                {
                    # region ������ͨfeatureClass�������ֶ�
                    //���Object�ֶ�
                    IField newfield2 = new FieldClass();
                    IFieldEdit fieldEdit2 = newfield2 as IFieldEdit;
                    fieldEdit2.Name_2 = "OBJECTID";
                    fieldEdit2.Type_2 = esriFieldType.esriFieldTypeOID;
                    fieldEdit2.AliasName_2 = "OBJECTID";
                    fieldEdit2.IsNullable_2 = false;
                    fieldEdit2.Required_2 = true;
                    fieldEdit2.Editable_2 = false;
                    newfield2 = fieldEdit2 as IField;
                    fsEdit.AddField(newfield2);

                    //���Geometry�ֶ�
                    IField newfield1 = new FieldClass();
                    newfield1 = GetGeometryField(newfield1, shapeType, geoNode);
                    if (newfield1 == null)
                    {
                        eError = new Exception("�����ֶν�������");
                        return false;
                    }
                    fsEdit.AddField(newfield1);
                    fields = fsEdit as IFields;

                    #endregion
                    feaworkspace.CreateFeatureClass(feaClsName, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
                }
                #endregion
                return true;
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
                eError = new Exception("����ͼ�����");
                return false;
            }
        }

        //���ݹ���XML�п���ṹ�ڵ㴴������(ע�ǲ�)   ���Ƿ�  ��д
        private bool createAnnoFeatureClass(string feaName, IFeatureWorkspace feaworkspace, IFieldsEdit fsEditAnno, int intScale, string shapeType, XmlNode geoNode)
        {
            //����ע�ǵ������ֶ�
            try
            {
                //ע�ǵ�workSpace
                IFeatureWorkspaceAnno pFWSAnno = feaworkspace as IFeatureWorkspaceAnno;
                if (pFWSAnno == null)
                {
                    return false;
                }
                IGraphicsLayerScale pGLS = new GraphicsLayerScaleClass();
                pGLS.Units = esriUnits.esriMeters;
                pGLS.ReferenceScale = Convert.ToDouble(intScale);//����ע�Ǳ���Ҫ���ñ�����

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
                    IField tempField = pOCDesc.RequiredFields.get_Field(j);
                    if (tempField.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }
                    fsEditAnno.AddField(tempField);
                }
                //����xml�ļ���Geometry�ֶο��ܴ��пռ�ο�����˵������Geometry�ֶ�
                //���Geometry�ֶ�
                IField newfield1 = new FieldClass();
                newfield1 = GetGeometryField(newfield1, shapeType, geoNode);
                if (newfield1 == null) return false;
                fsEditAnno.AddField(newfield1);

                fields = fsEditAnno as IFields;
                pFWSAnno.CreateAnnotationClass(feaName, fields, pOCDesc.InstanceCLSID, pOCDesc.ClassExtensionCLSID, pFDesc.ShapeFieldName, "", null, null, pAnnoPropsColl, pGLS, pSymbolColl, true);
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
                return false;
            }
            return true;
        }

        #endregion


        /// <summary>
        /// �������������ֶ�   ���Ƿ� ��д
        /// </summary>
        /// <param name="newfield1"></param>
        /// <param name="shapeType"></param>
        /// <param name="geoNode"></param>
        /// <param name="prjFile"></param>
        /// <returns></returns>
        private IField GetGeometryField(IField newfield1, string shapeType, XmlNode geoNode)
        {
            IFieldEdit fieldEdit1 = newfield1 as IFieldEdit;
            fieldEdit1.Name_2 = "SHAPE";
            fieldEdit1.Type_2 = esriFieldType.esriFieldTypeGeometry;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = geoDef as IGeometryDefEdit;
            if (shapeType == "��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            }
            else if (shapeType == "��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
            }
            else if (shapeType == "��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            }
            else if (shapeType == "ע��")
            {
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            }
            XmlNode spatialNode = geoNode.SelectSingleNode(".//SpatialReference");
            string spatialType = (spatialNode as XmlElement).GetAttribute("Type");
            string spaialDes = (spatialNode as XmlElement).GetAttribute("SpatialDes").Trim();
            int spaialByteRead = int.Parse((spatialNode as XmlElement).GetAttribute("ByteRead").Trim());
            //��xml�ж�ȡ�ڵ��ֵ
            double pXYResolution = Double.Parse(spatialNode["XYResolution"].InnerText);
            bool isHighPrecision = bool.Parse(spatialNode["HighPrecision"].InnerText);
            double xMin = Double.Parse(spatialNode["xMin"].InnerText);
            double xMax = Double.Parse(spatialNode["xMax"].InnerText);
            double yMin = Double.Parse(spatialNode["yMin"].InnerText);
            double yMax = Double.Parse(spatialNode["yMax"].InnerText);
            try
            {
                ISpatialReference pSR = null;
                //ISpatialReferenceFactory pSpatialRefFac = new SpatialReferenceEnvironmentClass();
                IESRISpatialReferenceGEN pESRISpatialGEN = null;
                if (spatialType == "UnknownCoordinateSystem")
                {
                    pSR = new UnknownCoordinateSystemClass();
                }
                else if (spatialType == "ProjectedCoordinateSystem")
                {
                    //if (!File.Exists(prjFile))
                    //{
                    //    //���ռ�ο��ļ��ѱ�ɾ�����޷�����xml
                    //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ռ�ο��ļ��ѱ�ɾ��!");
                    //    return null;
                    //}
                    //pSR = pSpatialRefFac.CreateESRISpatialReferenceFromPRJFile(prjFile);
                    pSR = new ProjectedCoordinateSystemClass();
                    pESRISpatialGEN = pSR as IESRISpatialReferenceGEN;
                    pESRISpatialGEN.ImportFromESRISpatialReference(spaialDes, out spaialByteRead);
                    pSR = pESRISpatialGEN as ISpatialReference;
                }
                else if (spatialType == "GeographicCoordinateSystem")
                {
                    //if (!File.Exists(prjFile))
                    //{
                    //    //���ռ�ο��ļ��ѱ�ɾ�����޷�����xml
                    //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ռ�ο��ļ��ѱ�ɾ��!");
                    //    return null;
                    //}
                    //pSR = pSpatialRefFac.CreateESRISpatialReferenceFromPRJFile(prjFile);
                    pSR = new GeographicCoordinateSystemClass();
                    pESRISpatialGEN = pSR as IESRISpatialReferenceGEN;
                    pESRISpatialGEN.ImportFromESRISpatialReference(spaialDes, out spaialByteRead);
                    pSR = pESRISpatialGEN as ISpatialReference;
                }
                ISpatialReferenceResolution pSRR = pSR as ISpatialReferenceResolution;
                ISpatialReferenceTolerance pSRT = (ISpatialReferenceTolerance)pSR;
                IControlPrecision2 pSpatialPrecision = (IControlPrecision2)pSR;

                pSRR.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon
                pSRR.set_XYResolution(true, pXYResolution);
                pSRT.SetDefaultXYTolerance();
                pSpatialPrecision.IsHighPrecision = isHighPrecision;

                geoDefEdit.SpatialReference_2 = pSR;
                fieldEdit1.GeometryDef_2 = geoDefEdit as GeometryDef;
                newfield1 = fieldEdit1 as IField;
                return newfield1;
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
        /// ��ȡ�����ռ���ָ���������ݼ�  ���Ƿ� ���
        /// </summary>
        /// <param name="pWorkspace">�����ռ�</param>
        /// <param name="aDatasetTyp">���ݼ�����</param>
        /// <returns></returns>
        private List<IDataset> GetDatasets(IWorkspace pWorkspace, esriDatasetType aDatasetTyp)
        {
            List<IDataset> Datasets = new List<IDataset>();
            IEnumDataset pEnumDataset = pWorkspace.get_Datasets(aDatasetTyp);
            IDataset pDataset = pEnumDataset.Next();
            while (pDataset != null)
            {
                Datasets.Add(pDataset);
                pDataset = pEnumDataset.Next();
            }
            return Datasets;
        }

        /// <summary>
        /// ��ȡĳһҪ�ؼ�����FC  ���Ƿ����
        /// </summary>
        /// <param name="pFeaDsName">Ҫ�ؼ�IFeatureDataset</param>
        /// <returns></returns>
        private List<IDataset> GetFeatureClass(IFeatureDataset pFeaDs)
        {
            List<IDataset> FCs = new List<IDataset>();

            IEnumDataset pEnumDs = pFeaDs.Subsets;
            IDataset pDs = pEnumDs.Next();
            while (pDs != null)
            {
                FCs.Add(pDs);
                pDs = pEnumDs.Next();
            }
            return FCs;
        }

        /// <summary>
        /// ��ȡ���ݿ���ȫ����FC  ���Ƿ����
        /// </summary>
        /// <returns></returns>
        private List<IDataset> GetAllFeatureClass(IWorkspace pWorkspace)
        {
            List<IDataset> listFC = new List<IDataset>();

            //�õ�ȫ�������FC����
            List<IDataset> LsFC = GetDatasets(pWorkspace, esriDatasetType.esriDTFeatureClass);
            if (LsFC != null)
            {
                if (LsFC.Count > 0)
                {
                    listFC.AddRange(LsFC);
                }
            }

            //�õ�Ҫ�ؼ�����ȫ��FC����
            IEnumDataset pEnumDs = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            IDataset pDs = pEnumDs.Next();
            while (pDs != null)
            {
                IFeatureDataset pFeatureDs = (IFeatureDataset)pDs;
                List<IDataset> FdFCs = GetFeatureClass(pFeatureDs);
                if (FdFCs != null)
                {
                    if (FdFCs.Count > 0)
                    {
                        listFC.AddRange(FdFCs);
                    }
                }
                pDs = pEnumDs.Next();
            }

            return listFC;
        }

        /// <summary>
        /// �رչ�����
        /// </summary>
        /// <returns>�������</returns>
        public bool CloseWorkspace(IWorkspace mWorkSpace)
        {
            if (mWorkSpace == null) return true;
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mWorkSpace.WorkspaceFactory);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(mWorkSpace);
            mWorkSpace = null;
            return true;
        }
        /// <summary>
        /// ��õ�����xml�Ĵ����ļ�����������׺
        /// </summary>
        /// <param name="xmlPath">������xml���������ļ���������·���ͺ�׺</param>
        /// <returns></returns>
        private string GetPureName(string xmlPath)
        {
            //���xml·���µ��ļ�����
            FileInfo fileInfo = new FileInfo(xmlPath);
            string pureName = "";
            string xmlName = fileInfo.Name;
            int index2 = xmlName.LastIndexOf(".");
            pureName = xmlName.Substring(0, index2);
            return pureName;
        }

        /// <summary>
        /// ��xmlByte����Ϊobj
        /// </summary>
        /// <param name="xmlByte"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool XmlDeSerializer(byte[] xmlByte, object obj)
        {
            try
            {
                //�ж��ַ����Ƿ�Ϊ��
                if (xmlByte != null)
                {
                    ESRI.ArcGIS.esriSystem.IPersistStream pStream = obj as ESRI.ArcGIS.esriSystem.IPersistStream;

                    ESRI.ArcGIS.esriSystem.IXMLStream xmlStream = new ESRI.ArcGIS.esriSystem.XMLStreamClass();

                    xmlStream.LoadFromBytes(ref xmlByte);
                    pStream.Load(xmlStream as ESRI.ArcGIS.esriSystem.IStream);

                    return true;
                }
                return false;
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
                return false;
            }
        }
        #endregion

        #region ��ʾ���¶Ա��б�
        //�����������¸��¶Ա��б���
        private DataTable CreateUpdateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("GOFID", System.Type.GetType("System.String"));
            dt.Columns.Add("������ͼ����", System.Type.GetType("System.String"));
            dt.Columns.Add("������OID", System.Type.GetType("System.String"));
            dt.Columns.Add("�ⲿ��ͼ����", System.Type.GetType("System.String"));
            dt.Columns.Add("�ⲿ��OID", System.Type.GetType("System.String"));
            dt.Columns.Add("����״̬", System.Type.GetType("System.String"));
            dt.Columns.Add("����޸�ʱ��", System.Type.GetType("System.String"));
            return dt;
        }
        //����DGML�ĵ������¶Ա��б���ʾ�������Է��㶨λ�鿴
        private void ShowUpdateGrid(string[] fileNames)
        {
            DataTable tempDT = new DataTable();
            tempDT = CreateUpdateTable();
            # region �����������ֵ
            foreach (string fileName in fileNames)
            {
                //����xml�ĵ�
                XmlDocument DGMLDoc = new XmlDocument();
                DGMLDoc.Load(fileName);
                XmlNodeList recordList = DGMLDoc.SelectNodes(".//Data//Record");
                foreach (XmlNode recordNode in recordList)
                {
                    DataRow newRow = tempDT.NewRow();

                    string pGOFID = recordNode.SelectSingleNode(".//GOFID").InnerText.Trim();
                    string newFCName = recordNode.SelectSingleNode(".//NEWFCNAME").InnerText.Trim();//������ͼ����
                    string OLDFCName = recordNode.SelectSingleNode(".//OLDFCNAME").InnerText.Trim();//�ⲿ��ͼ����
                    string pState = recordNode.SelectSingleNode(".//STATE").InnerText.Trim();//����״̬
                    string pUpdateTime = recordNode.SelectSingleNode(".//UPDATETIME").InnerText.Trim();//������ʱ��
                    string pNewOID = "";//������OID
                    string pOldOID = "";//�ⲿ��OID
                    XmlNodeList valueNodeList = recordNode.SelectNodes(".//NEWFEATURE//Value");
                    foreach (XmlNode valueNode in valueNodeList)
                    {
                        string pfieldName = "";
                        pfieldName = valueNode.SelectSingleNode(".//FieldName").InnerText.Trim();//�ֶ���
                        if (pfieldName == "") return;
                        if (pfieldName == "OBJECTID")
                        {
                            pNewOID = valueNode.SelectSingleNode(".//FieldValue").InnerText.Trim();//OBJECTIDֵ
                        }
                    }
                    XmlNodeList pValueNodeList = recordNode.SelectNodes(".//OLDFEATURE//Value");
                    foreach (XmlNode valueNode in pValueNodeList)
                    {
                        string pfieldName = "";
                        pfieldName = valueNode.SelectSingleNode(".//FieldName").InnerText.Trim();//�ֶ���
                        if (pfieldName == "") return;
                        if (pfieldName == "OBJECTID")
                        {
                            pOldOID = valueNode.SelectSingleNode(".//FieldValue").InnerText.Trim();//OBJECTIDֵ
                        }
                    }
                    if (pOldOID == "" && pNewOID == "") return;

                    //���и�ֵ
                    newRow["GOFID"] = pGOFID;
                    newRow["������ͼ����"] = newFCName;
                    newRow["������OID"] = pNewOID;
                    newRow["�ⲿ��ͼ����"] = OLDFCName;
                    newRow["�ⲿ��OID"] = pOldOID;
                    newRow["����״̬"] = pState;
                    newRow["����޸�ʱ��"] = pUpdateTime;
                    tempDT.Rows.Add(newRow);
                }
            }
            #endregion

            //��ҳ��ʾ���¶Ա��б�
            ModData.TotalTable = tempDT;

            ModDBOperator.LoadPage(m_Hook, tempDT, ModData.CurrentPage, ModData.recNum);

            ////��ո��¶Ա��б�
            //if (m_Hook.UpdateGrid.DataSource!= null)
            //{
            //    m_Hook.UpdateGrid.DataSource = null;
            //}
            ////�����󶨵�DataGrid��
            //m_Hook.UpdateGrid.DataSource = DisTable;
            //m_Hook.UpdateGrid.Visible = true;
            //m_Hook.UpdateGrid.ReadOnly = true;
            //for (int j = 0; j < m_Hook.UpdateGrid.Columns.Count; j++)
            //{
            //    m_Hook.UpdateGrid.Columns[j].Width = (m_Hook.UpdateGrid.Width - 20) / 7;
            //}
            //m_Hook.UpdateGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //m_Hook.UpdateGrid.RowHeadersWidth = 20;
            //m_Hook.UpdateGrid.Refresh();
            ////��ҳ�ı�����Ϣ��ʾ
            //ModDBOperator.DisplayPageInfo(m_Hook.TxtDisplayPage, ModData.CurrentPage, pageCount);
        }
        # endregion

        #region ��������ʾ
        //���ƽ�������ʾ
        private void ShowProgressBar(bool bVisible)
        {
            if (bVisible == true)
            {
                v_AppForm.ProgressBar.Visible = true;
            }
            else
            {
                v_AppForm.ProgressBar.Visible = false;
            }
        }
        //�޸Ľ�����
        private void ChangeProgressBar(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value)
        {
            if (min != -1)
            {
                pProgressBar.Minimum = min;
            }
            if (max != -1)
            {
                pProgressBar.Maximum = max;
            }
            pProgressBar.Value = value;
            pProgressBar.Refresh();
        }


        //�ı�״̬����ʾ����
        private void ShowStatusTips(string strText)
        {
            v_AppForm.OperatorTips = strText;
        }
        #endregion

        #region ��ʾ�Ի���
        private void ShowErrForm(string strCaption, string strText)
        {
            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle(strCaption, strText);
        }
        #endregion
    }
}
