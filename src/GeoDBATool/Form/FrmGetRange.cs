using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using SysCommon.Gis;

namespace GeoDBATool
{
    public partial class FrmGetRange : DevComponents.DotNetBar.Office2007Form
    {
        private Plugin.Application.IAppGISRef m_AppGIS;               //������Ӧ��APP
        private IGeometry m_Geometry;
        private IList<IDataset> List;
        public IGeometry DrawGeometry
        {
            set
            {
                m_Geometry = value;
            }
        }
        IFeatureLayer m_FeaLayer = null;
        public FrmGetRange(Plugin.Application.IAppGISRef pAppGIS)
        {
            InitializeComponent();
            m_AppGIS = pAppGIS;
            RadioBtnSelectRange.Checked = false;
            RadioBtnInputRange.Checked = false;
            RadioBtnDrawRange.Checked = false;
        }


        private void btnSelectRange_Click(object sender, EventArgs e)
        {
           ITool  _tool = new ControlsSelectFeaturesToolClass();
           ICommand _cmd = _tool as ICommand;
            _cmd.OnCreate(axMapControl1.Object as IMapControlDefault);
            if (_tool == null || _cmd == null) return;
            axMapControl1.CurrentTool = _tool;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            m_Geometry = null;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Exception eError = null;

            m_FeaLayer = axMapControl1.Map.get_Layer(0) as IFeatureLayer;
            IFeatureSelection pFeaSel = m_FeaLayer as IFeatureSelection;
            if (pFeaSel.SelectionSet == null || pFeaSel.SelectionSet.Count == 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͼ��ѡ��Χ��");
                return;
            }
            else
            {
                m_Geometry = GetFeaLayerGeometry(pFeaSel, m_FeaLayer);
            }
            if (m_Geometry == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������ͼ�����ݷ�Χ��");
                return;
            }
            //�Է�Χ���м�鿴�÷�Χ�Ƿ��Ǳ��ع�����ͼ�����ݵ��ܷ�Χ����

            //����Χ��Ϣ������д��XML��
            byte[] xmlByte = xmlSerializer(m_Geometry);
            string base64String = Convert.ToBase64String(xmlByte);

            //XmlDocument DocXml = (m_AppGIS.ProjectTree.SelectedNode.Tag as XmlNode)
            //DocXml.Load(ModData.v_projectXML);
            XmlNode ProNode = m_AppGIS.ProjectTree.SelectedNode.Tag as XmlNode;//DocXml.SelectSingleNode(".//����[@����='" + m_AppGIS.ProjectTree.SelectedNode.Name + "']");
            XmlElement RangeElem = ProNode.SelectSingleNode(".//����//ͼ��������//��Χ��Ϣ") as XmlElement;
            RangeElem.SetAttribute("��Χ", base64String);
            ProNode.OwnerDocument.Save(ModData.v_projectDetalXML);   //cyf 20110628

            //cyf 20110621 modify ;����Χ��Ϣ�洢��ϵͳά���⵱��
            #region ��ȡϵͳά����������Ϣ��������ϵͳά���⣬�������ռ䱣������
            if (ModData.TempWks == null)
            {
                bool blnCanConnect = false;
                SysCommon.Gis.SysGisDB vgisDb = new SysCommon.Gis.SysGisDB();
                if (File.Exists(ModData.v_ConfigPath))
                {
                    //���ϵͳά����������Ϣ
                    SysCommon.Authorize.AuthorizeClass.GetConnectInfo(ModData.v_ConfigPath, out ModData.Server, out ModData.Instance, out ModData.Database, out ModData.User, out ModData.Password, out ModData.Version, out ModData.dbType);
                    //����ϵͳά����
                    blnCanConnect = ModDBOperator.CanOpenConnect(vgisDb, ModData.dbType, ModData.Server, ModData.Instance, ModData.Database, ModData.User, ModData.Password, ModData.Version);
                }
                else
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ȱʧϵͳά����������Ϣ�ļ���" + ModData.v_ConfigPath + "/n����������");
                    return;
                }
                if (!blnCanConnect)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ϵͳ�ܹ�ά��������ʧ�ܣ�����!");
                    return;
                }
                ModData.TempWks = vgisDb.WorkSpace;
            }
            if (ModData.TempWks == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡϵͳά���⹤���ռ�ʧ�ܣ�����!");
                return;
            }
            #endregion
            ////����Χ��Ϣд�����ݿ�
            //if (!File.Exists(ModData.v_DbInterConn)) return;

            ////��ȡϵͳά����������Ϣ
            //XmlDocument xmlConnDoc = new XmlDocument();
            //xmlConnDoc.Load(ModData.v_DbInterConn);
            //XmlElement ele = xmlConnDoc.SelectSingleNode(".//ϵͳά����������Ϣ") as XmlElement;
            //if (ele == null) return;
            //string sConnect = ele.GetAttribute("�����ַ���");//ϵͳά���������ַ���
            ////����ϵͳά����
            //SysCommon.DataBase.SysTable pTable = new SysCommon.DataBase.SysTable();
            //pTable.SetDbConnection(sConnect, SysCommon.enumDBConType.ORACLE, SysCommon.enumDBType.ORACLE, out eError);
            //if (eError != null)
            //{
            //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʾ������ϵͳά����ʧ�ܣ�");
            //    return;
            //}
            if (m_AppGIS.ProjectTree.SelectedNode.Name.ToString().Trim() == "") return;
           
            ///ԭ�и���ROW�ķ���
            //string upStr = "update DATABASEMD set DBPARA='" + base64String + "' where ID=" + Convert.ToInt32(m_AppGIS.ProjectTree.SelectedNode.Name.ToString().Trim());
            //pTable.UpdateTable(upStr, out eError);
            //if (eError != null)
            //{
            //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʾ������ͼ����Χ��Ϣʧ�ܣ�");
            //    return;
            //}

            //���ڵĸ���ROW xisheng changed 20111018
            SysGisTable sysTable = new SysGisTable(ModData.TempWks);
            Dictionary<string, object> dicData = new Dictionary<string, object>();
                dicData.Add("DBPARA",base64String);
                try { sysTable.UpdateRow("DATABASEMD", "ID='" + Convert.ToInt32(m_AppGIS.ProjectTree.SelectedNode.Name.ToString().Trim()) + "'", dicData, out eError); }
                catch
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʾ������ͼ����Χ��Ϣʧ�ܣ�+��ϸ��Ϣ:" + eError.Message);
                    return;
                }

            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ɷ�Χ��Ϣ�Ļ�ȡ�ʹ洢��");
            //end
            this.Close();
        }

        //���ѡ���ͼ��ķ�Χ
        public IGeometry GetFeaLayerGeometry(IFeatureSelection pFeatureSel, IFeatureLayer pMapLayer)
        {
            IEnumIDs pEnumIDs = pFeatureSel.SelectionSet.IDs;
            int id = pEnumIDs.Next();
            IGeometry pGeo = null;
            while (id != -1)
            {
                IFeature pFeat = pMapLayer.FeatureClass.GetFeature(id);
                if (pGeo == null)
                {
                    pGeo = pFeat.Shape;
                }
                else
                {
                    ITopologicalOperator pTop = pGeo as ITopologicalOperator;
                    pGeo = pTop.Union(pFeat.Shape);
                }
                id = pEnumIDs.Next();
            }
            return pGeo;
        }

        /// <summary>
        /// ���л�(���������л����ַ���)
        /// </summary>
        /// <param name="xmlByte">���л��ֽ�</param>
        /// <param name="obj">���л�����</param>
        /// <returns></returns>
        public static byte[] xmlSerializer(object obj)
        {
            try
            {
                byte[] xmlByte = null;//�������л�����ֽ�
                //�ж��Ƿ�֧��IPersistStream�ӿ�,ֻ��֧�ָýӿڵĶ�����ܽ������л�
                if (obj is ESRI.ArcGIS.esriSystem.IPersistStream)
                {
                    ESRI.ArcGIS.esriSystem.IPersistStream pStream = obj as ESRI.ArcGIS.esriSystem.IPersistStream;

                    ESRI.ArcGIS.esriSystem.IXMLStream xmlStream = new ESRI.ArcGIS.esriSystem.XMLStreamClass();

                    pStream.Save(xmlStream as ESRI.ArcGIS.esriSystem.IStream, 0);

                    xmlByte = xmlStream.SaveToBytes();
                }
                return xmlByte;
            }
            catch(Exception e)
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

        private void btnAddData_Click(object sender, EventArgs e)
        {
            Exception eError=null;
            SysCommon.Gis.SysGisDataSet pSysGISDT = new SysCommon.Gis.SysGisDataSet();
            axMapControl1.ClearLayers();
            //���ط�Χ����
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.CheckFileExists = true;
            OpenFile.CheckPathExists = true;
            OpenFile.Title = "ѡ��ͼ�η�Χ";
            OpenFile.Filter = "ͼ����Χ����(*.mdb)|*.mdb";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                pSysGISDT.SetWorkspace(OpenFile.FileName, SysCommon.enumWSType.PDB, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ݿ�¼����");
                    return;
                }
                IList<IDataset> LstDT = pSysGISDT.GetAllFeatureClass();
                List = LstDT;
                FeaturecomboBox.Items.Clear();
                FeaturecomboBox.Enabled = true;
                this.DataLoad.Enabled = true;
                foreach (IDataset pDT in LstDT)
                {
                    IFeatureClass pFeatureCls = pDT as IFeatureClass;
                    if (pFeatureCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        IFeatureLayer pFeaLayer = new FeatureLayerClass();
                        ILayer pLayer = null;
                        pFeaLayer.FeatureClass = pFeatureCls;
                        pLayer = pFeaLayer as ILayer;

                        FeaturecomboBox.Items.Add(pFeatureCls.AliasName);
                    }
                }
                if (FeaturecomboBox.Items.Count > 0)
                    FeaturecomboBox.SelectedIndex = 0;
                axMapControl1.ActiveView.Refresh();
            }

            //ICommand _cmd = new ControlsAddDataCommandClass();
            //if (_cmd == null) return;
            //_cmd.OnCreate(axMapControl1.Object as IMapControlDefault);

            //_cmd.OnClick();
            //axMapControl1.ActiveView.Refresh();
        }
        
        private void RadioBtnDrawRange_Click(object sender, EventArgs e)
        {
            //RadioBtnDrawRange.Checked = !RadioBtnDrawRange.Checked;
           
        }
        
        private void RadioBtnInputRange_Click(object sender, EventArgs e)
        {
            //RadioBtnInputRange.Checked = !RadioBtnInputRange.Checked;
          
        }

        
        private void RadioBtnSelectRange_Click(object sender, EventArgs e)
        {
            //RadioBtnSelectRange.Checked = !RadioBtnSelectRange.Checked;
            
        }
        //���ⲿTXT�ļ����뷶Χ
        private void RadioBtnInputRange_CheckedChanged(object sender, EventArgs e)
        {
            Exception outError=null;
            if (RadioBtnInputRange.Checked)
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.CheckFileExists = true;
                OpenFile.CheckPathExists = true;
                OpenFile.Title = "ѡ��ͼ�η�Χ����txt";
                OpenFile.Filter = "ͼ�η�Χ�����ı�(*.txt)|*.txt";
                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    try
                    {
                        StreamReader sr = new StreamReader(OpenFile.FileName);
                        while (sr.Peek() >= 0)
                        {
                            string[] strTemp = sr.ReadLine().Split(',');
                            //cyf 20110621 add:�����ı��ļ���ȡ���η�Χ
                            for (int i = 0; i < strTemp.Length-1; i = i + 2)
                            {
                                if (sb.Length != 0)
                                {
                                    sb.Append(",");
                                }
                                sb.Append(strTemp[i] + "@" + strTemp[i+1]);
                            }
                            //end
                        }
                    }
                    catch(Exception er)
                    {
                        //*******************************************************************
                        //guozheng added
                        if (ModData.SysLog != null)
                        {
                            ModData.SysLog.Write(er, null, DateTime.Now);
                        }
                        else
                        {
                            ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                            ModData.SysLog.Write(er, null, DateTime.Now);
                        }
                        //********************************************************************

                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͼ�η�Χ����txt��ʽ����ȷ!\n�ı�ÿ��Ϊ����������','�ָ�");
                        return;
                    }

                    if (sb.Length == 0) return;
                    m_Geometry = ModDBOperator.GetPolygonByCol(sb.ToString()) as IGeometry;
                    //��ü��η�Χ�Ŀռ�ο�
                    //if (ModData.TempWks != null)
                    //{
                    //    IFeatureWorkspace pFeaWs = ModData.TempWks as IFeatureWorkspace;
                    //    if (pFeaWs == null)
                    //    {
                    //        //��ȡϵͳά����������Ϣʧ��
                    //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡϵͳά����������Ϣʧ�ܣ�");
                    //        return;
                    //    }
                    //}
                    //m_Geometry.SpatialReference=

                    //cyf 20110621 ����ʱ�������д���һ����Χͼ��
                    //CreateMapFrameLayerInWorkSpace(m_Geometry, out outError);
                    //if(outError!=null)
                    //{
                    //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������ʱ�ķ�Χͼ��ʧ�ܣ�\nԭ��"+outError.Message);
                    //    return;
                    //}
                    //end
                }
            }
        }

        /// <summary>
        /// ������ķ�Χ�ڹ������н���һ��ͼ����Χͼ�㣬  cyf 20110621
        /// </summary>
        /// <param name="in_pMapRange"></param>
        /// <param name="ex"></param>
        //private void CreateMapFrameLayerInWorkSpace(IGeometry in_pMapRange, out Exception ex)
        //{
        //    ex = null;
        //    //////��һ������ȡ�������Workspace
        //    if (this.m_WorkFeatureDataset == null) { ex = new Exception("������Ĺ����ռ䲻��Ϊ��"); return; }
        //    //////�ڶ������ڹ����ռ��н���һ��ͼ����Χͼ��
        //    try
        //    {
        //        IFields fields = new FieldsClass();
        //        IFieldsEdit fsEdit = fields as IFieldsEdit;
        //        //���Object�ֶ�
        //        IField newfield2 = new FieldClass();
        //        IFieldEdit fieldEdit2 = newfield2 as IFieldEdit;
        //        fieldEdit2.Name_2 = "OBJECTID";
        //        fieldEdit2.Type_2 = esriFieldType.esriFieldTypeOID;
        //        fieldEdit2.AliasName_2 = "OBJECTID";
        //        newfield2 = fieldEdit2 as IField;
        //        fsEdit.AddField(newfield2);

        //        //���Geometry�ֶ�
        //        IField newfield1 = new FieldClass();
        //        IFieldEdit fieldEdit1 = newfield1 as IFieldEdit;
        //        fieldEdit1.Name_2 = "SHAPE";
        //        fieldEdit1.Type_2 = esriFieldType.esriFieldTypeGeometry;
        //        IGeometryDef geoDef = new GeometryDefClass();


        //        IGeometryDefEdit geoDefEdit = geoDef as IGeometryDefEdit;
        //        geoDefEdit.SpatialReference_2 = (this.m_WorkFeatureDataset as IGeoDataset).SpatialReference;
        //        geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
        //        fieldEdit1.GeometryDef_2 = geoDefEdit as GeometryDef;
        //        newfield1 = fieldEdit1 as IField;
        //        fsEdit.AddField(newfield1);
        //        fields = fsEdit as IFields;
        //        ////////����Ҫ�ؼ�
        //        IFeatureClass pMapFrameFeaCls = null;
        //        pMapFrameFeaCls = ModData.GetFeaClsSetInEnum("MapFrameLayer", this.m_WorkFeatureDataset.Subsets);
        //        if (pMapFrameFeaCls == null)
        //        {
        //            //////����������
        //            pMapFrameFeaCls = this.m_WorkFeatureDataset.CreateFeatureClass("MapFrameLayer", fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
        //        }

        //        ////////��������ͼ����д��ͼ����Χͼ��////////
        //        IWorkspaceEdit WsEdit = this.m_WorkFeatureDataset.Workspace as IWorkspaceEdit;
        //        WsEdit.StartEditing(true);
        //        WsEdit.StartEditOperation();

        //        IFeature NewFea = pMapFrameFeaCls.CreateFeature();
        //        NewFea.Shape = in_pMapRange;
        //        NewFea.Store();

        //        WsEdit.StopEditOperation();
        //        WsEdit.StopEditing(true);
        //        ////////���Ĳ�����¼��ͼ�㣬������ͼ��ؼ���
        //        this.m_MapFrameClass = pMapFrameFeaCls;
        //        DeleteLayerByName("MapFrameLayer");
        //        IFeatureLayer NewMapFrameLayer = new FeatureLayerClass();
        //        NewMapFrameLayer.FeatureClass = pMapFrameFeaCls;
        //        NewMapFrameLayer.Name = "MapFrameLayer";
        //        this.m_HookHelp.FocusMap.AddLayer(NewMapFrameLayer as ILayer);
        //        this.m_HookHelp.FocusMap.MoveLayer((NewMapFrameLayer as ILayer), this.m_HookHelp.FocusMap.LayerCount);
        //    }
        //    catch (Exception eError)
        //    {
        //        //******************************************
        //        //ϵͳ������־
        //        if (ModData.SysLog == null)
        //            ModData.SysLog = new clsWriteSystemFunctionLog();
        //        ModData.SysLog.Write(eError);
        //        //******************************************
        //        ex = eError;
        //    }
        //}
        //���յ�ͼ����Χ
        private void RadioBtnDrawRange_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioBtnDrawRange.Checked)
            {
                RadioBtnDrawRange.Checked = true;
                DrawPolygonToolClass drawPolygon = new DrawPolygonToolClass(true, this);
                drawPolygon.OnCreate(axMapControl1.Object as IMapControlDefault);
                axMapControl1.CurrentTool = drawPolygon as ITool;
            }
            else
            {

                RadioBtnDrawRange.Checked = false;
            }
        }
        //�ӽ�����ѡ��Χ
        private void RadioBtnSelectRange_CheckedChanged(object sender, EventArgs e)
        {
            //if (RadioBtnSelectRange.Checked)
            //{
            //    btnSelectRange.Enabled = true;
            //}
            //else
            //{
            //    btnSelectRange.Enabled = false;
            //}
        }

        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            if (axMapControl1.Map.LayerCount == 0)
            {
                RadioBtnSelectRange.Checked = false;
                RadioBtnSelectRange.Enabled = false;
                btnSelectRange.Enabled = false;
            }
            else
            {
                IFeatureLayer pFeaLayer = null;
                for (int i = 0; i < axMapControl1.Map.LayerCount; i++)
                {
                    pFeaLayer = axMapControl1.Map.get_Layer(i) as IFeatureLayer;
                    if (pFeaLayer == null)
                    {
                        continue;
                    }
                    if (pFeaLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        m_FeaLayer = pFeaLayer;
                        break;
                    }
                }
                if (m_FeaLayer == null)
                {
                    RadioBtnSelectRange.Enabled = false;
                    btnSelectRange.Enabled = false;
                }
                else
                {
                    RadioBtnSelectRange.Enabled = true;
                }
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.FeaturecomboBox.Text))
                return;
            foreach (IDataset pDT in List)
            {
                IFeatureClass pFeatureCls = pDT as IFeatureClass;
                if (pFeatureCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    IFeatureLayer pFeaLayer = new FeatureLayerClass();
                    ILayer pLayer = null;

                    if (pFeatureCls.AliasName == this.FeaturecomboBox.Text.Trim())
                    {
                        axMapControl1.Map.ClearLayers();
                        axMapControl1.Refresh();
                        pFeaLayer.FeatureClass = pFeatureCls;
                        pLayer = pFeaLayer as ILayer;
                        axMapControl1.Map.AddLayer(pLayer);
                        IFeatureCursor cursor = pFeatureCls.Search(null, false);
                        IFeature pFeature = cursor.NextFeature();
                        IFeatureSelection pFeatureSelection = pFeaLayer as IFeatureSelection;
                        pFeatureSelection.Add(pFeature);
                        axMapControl1.ActiveView.Refresh();
                        btnSelectRange.Enabled = true;
                        break;
                    }
                    
                }
            }
        }
    }
}